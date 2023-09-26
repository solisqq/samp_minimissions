using SampSharp.Core;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace partymode
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            new GameModeBuilder()
                .Use<GameMode>()
                .Run();

            NativeServer server = new NativeServer();
            server.Run();
            Console.WriteLine("test");
        }
    }
}
