using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace GiantBombBot.Commands
{
    class Subscribe
    {
        public static string ContentDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent?.Parent?.FullName;
        private static string SubscriberFilePath => ContentDirectory + "/Content/subscriber.json";

        public static List<ulong> SubscriberList = File.Exists(SubscriberFilePath) ? JsonConvert.DeserializeObject<List<ulong>>(File.ReadAllText(SubscriberFilePath))
                : new List<ulong>();

        public static DiscordClient Client;

        public static void RegisterSubscribeCommands(DiscordClient client)
        {
            Client = client;

            Console.WriteLine("Registering Subscribe/Unsubscribe Commands");

            client.GetService<CommandService>().CreateCommand("subscribe")
                    .Alias("sub")
                    .Description("Subscribe to livestream updates over PM")
                    .Do(async e =>
                    {
                        if (SubscriberList.Contains(e.User.Id))
                        {
                            await
                                e.Channel.SendMessage("You're already subscribed, to unsubscribe use \"~unsubscribe\"");
                        }
                        else
                        {
                            AddToList(e.User.Id);
                            await
                                e.Channel.SendMessage("You are now subscribed, to unsubscribe use \"~unsubscribe\". You have to stay in the GB server to continue to get messages.");
                        }
                    });

            client.GetService<CommandService>().CreateCommand("unsubscribe")
                    .Alias("unsub")
                    .Description("Unsubscribe to livestream updates over PM")
                    .Do(async e =>
                    {
                        if (SubscriberList.Contains(e.User.Id))
                        {
                            RemoveFromList(e.User.Id);
                            await
                                e.Channel.SendMessage("You are now unsubscribed, thanks for trying it out.");
                        }
                        else
                        {
                            await
                                e.Channel.SendMessage("You are already unsubscribed.");
                        }
                    });
        }

        public static async Task PostLivestream(XElement stream)
        {
            var title = stream.Element("title")?.Value;
            var deck = stream.Element("deck")?.Value;
            var image = stream.Element("image")?.Element("screen_url")?.Value;

            foreach (ulong user in SubscriberList)
            {
                try
                {
                    await
                        Program.Client.GetServer(106386929506873344).GetUser(user)
                            .SendMessage(title + ": " + deck + " is LIVE at <http://www.giantbomb.com/chat/> NOW, check it out!" +
                                Environment.NewLine + image);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex + Environment.NewLine + ex.Message);
                }
            }
        }

        private static void AddToList(ulong user)
        {
            SubscriberList.Add(user);
            Save();
        }

        private static void RemoveFromList(ulong user)
        {
            SubscriberList.Remove(user);
            Save();
        }

        private static void Save()
        {
            File.WriteAllText(SubscriberFilePath, JsonConvert.SerializeObject(SubscriberList));
        }
    }
}