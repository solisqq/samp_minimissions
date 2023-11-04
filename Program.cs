using SampSharp.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace partymode
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            JsonElement paths = new JsonElement();
            string pathspaths = "G:/Solivision/vps/samp/paths.json";
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": running server on Linux.");
                pathspaths = "/home/solis/solivision/samp/paths.json";
            } else Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": running server on Windows.");

            using (StreamReader r = new StreamReader(pathspaths))
            {
                string raw = r.ReadToEnd();
                JsonDocument json = JsonDocument.Parse(raw);
                paths = json.RootElement;
                Console.WriteLine(DateTime.Now.ToString("MM.yy hh:mm:ss") + ": reading configuration at "+pathspaths);
            }
            string dbpath = paths.GetProperty("global").GetProperty("database").ToString();
            Database db = new Database(dbpath);

            new GameModeBuilder()
                .Use<GameMode>()
                .Run();

            NativeServer server = new NativeServer(db);
            server.Run();
        }
    }
}
