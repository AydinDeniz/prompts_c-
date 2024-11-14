
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TcpChatServer
{
    private static List<TcpClient> clients = new List<TcpClient>();
    private static TcpListener server;

    public static void Main(string[] args)
    {
        server = new TcpListener(IPAddress.Any, 8888);
        server.Start();
        Console.WriteLine("Server started on port 8888...");

        while (true)
        {
            TcpClient client = server.AcceptTcpClient();
            clients.Add(client);
            Console.WriteLine("New client connected.");
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    private static void HandleClient(object clientObject)
    {
        TcpClient client = (TcpClient)clientObject;
        NetworkStream stream = client.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

        writer.WriteLine("Welcome to the chat server! Type /exit to disconnect.");

        while (true)
        {
            try
            {
                string message = reader.ReadLine();
                if (message == "/exit")
                {
                    Console.WriteLine("Client disconnected.");
                    clients.Remove(client);
                    client.Close();
                    break;
                }

                Console.WriteLine("Received: " + message);
                BroadcastMessage(message, client);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                clients.Remove(client);
                client.Close();
                break;
            }
        }
    }

    private static void BroadcastMessage(string message, TcpClient sender)
    {
        foreach (TcpClient client in clients)
        {
            if (client != sender)
            {
                NetworkStream stream = client.GetStream();
                StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };
                writer.WriteLine(message);
            }
        }
    }
}

public class TcpChatClient
{
    public static void Main(string[] args)
    {
        TcpClient client = new TcpClient("127.0.0.1", 8888);
        NetworkStream stream = client.GetStream();
        StreamReader reader = new StreamReader(stream);
        StreamWriter writer = new StreamWriter(stream) { AutoFlush = true };

        Console.WriteLine(reader.ReadLine());  // Welcome message from server

        Thread readThread = new Thread(() =>
        {
            while (true)
            {
                try
                {
                    string serverMessage = reader.ReadLine();
                    Console.WriteLine("Server: " + serverMessage);
                }
                catch
                {
                    Console.WriteLine("Disconnected from server.");
                    break;
                }
            }
        });
        readThread.Start();

        while (true)
        {
            string message = Console.ReadLine();
            writer.WriteLine(message);
            if (message == "/exit")
            {
                client.Close();
                break;
            }
        }
    }
}
