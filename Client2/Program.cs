using System;
using System.Threading;
using System.Net.Sockets;
using System.IO;
namespace Client2
{
    class Program
    {
        static void Main(string[] args)
        {
            string myIp = "109.173.170.6";
            int port = 3000;

            TcpClient client = new TcpClient(myIp, port);
            Socket s = client.Client;
            NetworkStream stream = client.GetStream();
            Console.WriteLine("Connected");
            Thread.Sleep(1000);
            Console.Clear();

            try
            {
                string messageToServer = "";
                string messageFromServer = "";
                string PathFile = "";

                while (s.Connected)
                {
                    messageToServer = Console.ReadLine();
                    Byte[] data = System.Text.Encoding.ASCII.GetBytes(messageToServer);

                    if (messageToServer == "exit")
                    {
                        s.Shutdown(SocketShutdown.Both);
                        s.Close();
                        client.Close();
                        
                    }
                    string key = messageToServer.Substring(0, 1);
                    if (key == "f" || key == "F")
                    {
                        Console.WriteLine("Type Format");
                        string passw ="sending file." + Console.ReadLine();
                        data = System.Text.Encoding.ASCII.GetBytes(passw);
                        s.Send(data, data.Length, 0);
                        PathFile = messageToServer.Remove(0, 2);
                        Console.WriteLine("Sending {0}.", PathFile);
                        try
                        {
                            s.SendFile(PathFile);
                            Console.WriteLine("File Sent");
                        }
                        catch
                        {
                            Console.WriteLine("Cannot send file");
                        }
                        continue;
                    }

                    if (messageToServer != "exit")
                    {
                        try
                        {
                            s.Send(data, data.Length, 0);
                            Console.WriteLine("Me: {0}", messageToServer);
                        }
                        catch
                        {
                            Console.WriteLine("Something went wrong :/");
                        }

                        Byte[] datarec = new Byte[256];
                        int bytes = 0;
                        messageFromServer = String.Empty;

                        do
                        {
                            bytes = s.Receive(datarec, datarec.Length, 0);
                            string check = System.Text.Encoding.ASCII.GetString(datarec, 0, bytes);
                            key = check.Substring(0, 12);
                            if (key == "sending file")
                            {
                                string filename ="file" + check.Substring(12);
                                var output = File.Create(filename);
                                do
                                {
                                    bytes = s.Receive(datarec, datarec.Length, 0);
                                    output.Write(datarec);
                                }
                                while (bytes > 0);
                                continue;
                            }

                            messageFromServer = messageFromServer + check;
                        }
                        while (bytes > 0);

                        if (messageFromServer != "")
                        {
                            Console.WriteLine("Received: " + messageFromServer);
                        }
                    }
                }
            }
            catch
            {
                Console.WriteLine("Cannot read from server");
            }

            s.Shutdown(SocketShutdown.Both);
            s.Close();
            client.Close();
        }
    }
}
