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
            ipep = new IPEndPoint(IPAddress.Any, 1121);
            Running = false;
            clients = new List<NativeClient>();
            this.cts = new CancellationTokenSource();
        }

        public void Stop()
        {
            Running = false;
            cts.Cancel();
        }
        public async Task Run()
        {
            listener = new TcpListener(ipep);
            listener.Start();
            Running = true;
            Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": Running native server.");
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
            Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": New client");
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
