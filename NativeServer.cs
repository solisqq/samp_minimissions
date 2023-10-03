using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.Net.Http;


namespace partymode
{
    class NativeServer
    {
        IPEndPoint ipep;
        TcpListener listener;
        bool Running;
        List<NativeClient> clients;
        private CancellationTokenSource cts;
        Database db=null;

        public NativeServer(Database db)
        {
            ipep = new IPEndPoint(IPAddress.Any, 1121);
            Running = false;
            clients = new List<NativeClient>();
            this.cts = new CancellationTokenSource();
            this.db = db;
        }

        public void Stop()
        {
            Running = false;
            cts.Cancel();
        }
        public async void Run()
        {
            listener = new TcpListener(ipep);
            listener.Start();
            Running = true;
            Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": Running native server.");
            while (Running)
            {
                var c = await listener.AcceptTcpClientAsync();
                var client = new NativeClient(c,db);
                clients.Add(client);
                var clientTask = client.Run();
                clientTask.ContinueWith(t => clients.Remove(client));
                Console.WriteLine(clients.Count);
            }
        }
    }

    class NativeClient
    {
        TcpClient client;
        NetworkStream stream;
        Database db=null;

        public NativeClient(TcpClient client, Database db)
        {
            this.client = client;
            stream = client.GetStream();
            this.db = db;
        }
        public bool IsConnected
        {
            get
            {
                try
                {
                    if (this.client != null && this.client.Client != null && this.client.Client.Connected)
                    {
                        if (this.client.Client.Poll(0, SelectMode.SelectRead))
                        {
                            byte[] buff = new byte[1];
                            if (this.client.Client.Receive(buff, SocketFlags.Peek) == 0)
                            {
                                // Client disconnected
                                return false;
                            }
                            else
                            {
                                return true;
                            }
                        }

                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
        public async Task Run()
        {
            var r = new StreamReader(stream);
            var w = new StreamWriter(stream);
            Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": New client");
            while (IsConnected)
            {
                var dataTask = await r.ReadLineAsync();
                if (dataTask == null) continue;
                Console.WriteLine(dataTask);
                JsonDocument json = JsonDocument.Parse(dataTask);
                JsonElement jsonElement = json.RootElement;
                string response = TCPMsgHandler.instance.handle(
                    jsonElement.GetProperty("action").ToString(),
                    jsonElement.GetProperty("data"));
                string jsonTextRespond = "{ \"action\": \"" + jsonElement.GetProperty("action").ToString() + "\", \"data\": { " + response + "}}";
                Console.WriteLine(jsonTextRespond);
                var bytes = System.Text.Encoding.UTF8.GetBytes(jsonTextRespond);
                await stream.WriteAsync(bytes, 0, bytes.Length);
                w.Write(jsonTextRespond + "\n");
            }
            this.client.Close();
        }
    }
}
