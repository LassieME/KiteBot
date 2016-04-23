using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Discord;
using Discord.API.Client.Rest;

namespace GiantBombBot
{
    public class KiteChat
    {
        public static Random RandomSeed;

        public static LivestreamChecker StreamChecker;
        public static GiantBombVideoChecker GbVideoChecker;

        public static List<Message> BotMessages = new List<Message>();

        public KiteChat(string GBapi, int streamRefresh, int videoRefresh) : this(GBapi, streamRefresh, videoRefresh, new Random())
        {
        }

        public KiteChat(string GBapi,int streamRefresh, int videoRefresh, Random randomSeed)
        {
            RandomSeed = randomSeed;

            StreamChecker = new LivestreamChecker(GBapi, streamRefresh);
            //GbVideoChecker = new GiantBombVideoChecker(GBapi, videoRefresh);
        }

        public async Task AsyncParseChat(object s, MessageEventArgs e, DiscordClient client)
        {
            Console.WriteLine("(" + e.User.Name + "/" + e.User.Id + ") - " + e.Message.Text);

            if (e.Message.IsAuthor)
            {
                BotMessages.Add(e.Message);
            }
            else if (!e.Message.IsAuthor)
            {
                if ( e.User.Id == 85817630560108544)
                {
                    if (e.Message.Text.Contains(@"/saveExit"))
                    {
                        await e.Channel.SendMessage("OK");
                        Environment.Exit(1);
                    }
                    else if (e.Message.Text.Contains(@"/update"))
                    {
                        await StreamChecker.ForceUpdateChannel();
                    }
                    else if (e.Message.Text.Contains("/delete"))
                    {
                        if (BotMessages.Any()) await BotMessages.Last().Delete();
                    }
                }
                else if (e.Message.IsMentioningMe())
                {
                    if (e.Message.Text.StartsWith("@KiteBot #420") ||
                        e.Message.Text.ToLower().StartsWith("@KiteBot #blaze") ||
                        0 <= e.Message.Text.ToLower().IndexOf("waifu", 0))
                    {
                        await e.Channel.SendMessage("http://420.moe/");
                    }
                    else if (0 <= e.Message.Text.ToLower().IndexOf("help", 5))
                    {
                        var nl = Environment.NewLine;
                        await e.Channel.SendMessage("Current Commands are:" + nl + 
                            "#420"+ nl + 
                            "randomql" + nl + 
                            "help");
                    }
                    else if (e.Message.Text.ToLower().Contains("randomql"))
                    {
                        await e.Channel.SendMessage(GetResponseUriFromRandomQlCrew());
                    }
                    else if (e.Message.Text.ToLower().Contains("fuck you") || e.Message.Text.ToLower().Contains("fuckyou"))
                    {
                        List<string> _possibleResponses = new List<string>();
                        _possibleResponses.Add("Hey fuck you too USER!");
                        _possibleResponses.Add("I bet you'd like that wouldn't you USER?");
                        _possibleResponses.Add("No, fuck you USER!");
                        _possibleResponses.Add("Fuck you too USER!");

                        await
                            e.Channel.SendMessage(
                                _possibleResponses[RandomSeed.Next(0, _possibleResponses.Count)].Replace("USER",
                                    e.User.Name));
                    }
                    else
                    {
                        await
                            e.Channel.SendMessage($"GiantBombBot ver. 0.1.2 \"Beastcast, Best cast.\"\n" + 
                            $"Made by {e.Channel.GetUser(85817630560108544).Mention}.");
                    }
                }
            }
        }

        public static string GetResponseUriFromRandomQlCrew()
		{
            string url = "http://qlcrew.com/main.php?anyone=anyone&inc%5B0%5D=&p=999&exc%5B0%5D=&per_page=15&random";

            /*WebClient client = new WebClient();
            client.Headers.Add("user-agent", "LassieMEKiteBot/0.9 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
		    client..OpenRead(url);*/

		    HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.UserAgent = "Giant Bomb Discord bot fetching random quick looks";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			return response.ResponseUri.AbsoluteUri;
		}
    }
}
