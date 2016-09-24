using System;
using Discord;
using Discord.Commands;

namespace GiantBombBot.Commands
{
    class Misc
    {
        public static void RegisterMiscCommands(DiscordClient client)
        {
            Console.WriteLine("Registering Misc Commands");
            client.GetService<CommandService>().CreateCommand("420")
                    .Alias("#420", "blaze", "waifu")
                    .Description("Anime and weed, all you need.")
                    .Hide()                    
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("http://420.moe/");
                    });

            client.GetService<CommandService>().CreateCommand("randomql")
                    .Alias("randql")
                    .Description("Posts a random ql")                    
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage(KiteChat.GetResponseUriFromRandomQlCrew());
                    });
        }        
    }
}
