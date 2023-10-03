using SampSharp.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
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
            using (StreamReader r = new StreamReader("G:/Solivision/vps/samp/paths.json"))
            {
                string raw = r.ReadToEnd();
                JsonDocument json = JsonDocument.Parse(raw);
                paths = json.RootElement;
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
