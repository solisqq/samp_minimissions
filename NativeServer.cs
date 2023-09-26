using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace partymode
{
    class NativeServer
    {
        IPEndPoint ipep;
        TcpListener listener;
        bool Running;
        List<NativeClient> clients;
        private CancellationTokenSource cts;

        public NativeServer()
        {
            /*IPAddress ip = IPAddress.Parse(host);*/
            Console.WriteLine("test3");
            ipep = new IPEndPoint(IPAddress.Any, 1121);
            Running = false;
            clients = new List<NativeClient>();

            this.cts = new CancellationTokenSource();
            Console.WriteLine("test2");
        }

        public void Stop()
        {
            Running = false;
            cts.Cancel();
        }
        public async Task Run()
        {
            Console.WriteLine("test1");
            listener = new TcpListener(ipep);
            listener.Start();
            Running = true;
            Console.WriteLine("Running native server");
            while (Running)
            {
                var c = await listener.AcceptTcpClientAsync();
                var client = new NativeClient(c);
                clients.Add(client);
                var clientTask = client.Run(); //don't await
                await clientTask.ContinueWith(t => clients.Remove(client));
            }
        }
    }

    class NativeClient
    {
        TcpClient client;
        NetworkStream stream;

        public NativeClient(TcpClient client)
        {
            this.client = client;
            stream = client.GetStream();

        }

        public async Task Run()
        {
            var r = new StreamReader(stream);
            var w = new StreamWriter(stream);
            Console.WriteLine("New client");
/*            while (true)
            {
                Console.WriteLine("Test");
                await w.WriteLineAsync("Connection test.");
                await w.WriteAsync(">");
                await w.FlushAsync();

                var l = await r.ReadLineAsync();
                await w.WriteLineAsync("Invalid command " + l);
            }*/
        }
    }
}
