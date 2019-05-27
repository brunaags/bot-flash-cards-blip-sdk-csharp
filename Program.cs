using System;
using Take.Blip.Client.Console;

namespace bot_flash_cards_blip_sdk_csharp
{
    class Program
    {
        static int Main(string[] args) => ConsoleRunner.RunAsync(args).GetAwaiter().GetResult();
    }
}