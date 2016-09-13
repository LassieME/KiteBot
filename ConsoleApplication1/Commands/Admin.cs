using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace GiantBombBot.Commands
{
    class Admin
    {
        public static void RegisterAdminCommands(DiscordClient client)
        {
            Console.WriteLine("Registering Admin Commands");
            client.GetService<CommandService>().CreateCommand("saveExit")
                    .Alias("se")
                    .Description("Saves and exits.")
                    .Hide()
                    .AddCheck((c, u, ch) => u.Id == 85817630560108544)
                    .Do(async e =>
                    {
                        await e.Channel.SendMessage("OK");
                        Environment.Exit(1);
                    });

            client.GetService<CommandService>().CreateCommand("update")
                    .Alias("up")
                    .Description("Updates the livestream channel, and probably crashes if there is no chat.")
                    .Hide()
                    .AddCheck((c, u, ch) => u.Id == 85817630560108544)
                    .Do(async e =>
                    {
                        await KiteChat.StreamChecker.ForceUpdateChannel();
                        await e.Channel.SendMessage("updated?");
                    });

            client.GetService<CommandService>().CreateCommand("delete")
                    .Alias("del")
                    .Description("Deletes the last message the bot has written.")
                    .Hide()
                    .AddCheck((c, u, ch) => u.Id == 85817630560108544)
                    .Do(async e =>
                    {
                        if (KiteChat.BotMessages.Any()) await KiteChat.BotMessages.Last().Delete();                        
                    });

            client.GetService<CommandService>().CreateCommand("restart")
                    .Alias("re")
                    .Description("Restarts broken timers.")
                    .Hide()
                    .AddCheck((c, u, ch) => u.Id == 85817630560108544)
                    .Do(async e =>
                    {
                        var s = KiteChat.StreamChecker.Restart();
                        await e.Channel.SendMessage(s);
                    });

        }
    }
}
