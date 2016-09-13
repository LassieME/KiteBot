using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Discord;
using System.Net;

namespace GiantBombBot
{
    public class KiteChat
    {
        public static Random RandomSeed;

        public static LivestreamChecker StreamChecker;
        public static GiantBombVideoChecker GbVideoChecker;
        //public static MixlrChecker MixlrChecker;

        public static List<Message> BotMessages = new List<Message>();

        public KiteChat(string giantBombapi, int streamRefresh, int videoRefresh, int mixlrRefresh) : this(giantBombapi, streamRefresh, videoRefresh, mixlrRefresh, new Random())
        {
        }

        public KiteChat(string giantBombapi, int streamRefresh, int videoRefresh, int mixlrRefresh, Random randomSeed)
        {
            RandomSeed = randomSeed;

            StreamChecker = new LivestreamChecker(giantBombapi, streamRefresh);
            //GbVideoChecker = new GiantBombVideoChecker(GBapi, videoRefresh);
            //MixlrChecker = new MixlrChecker(mixlrRefresh);
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
                 if (e.Message.IsMentioningMe())
                {                                       
                    if (e.Message.Text.ToLower().Contains("fuck you") || e.Message.Text.ToLower().Contains("fuckyou"))
                    {
                        List<string> _possibleResponses = new List<string>();
                        _possibleResponses.Add("Hey fuck you too &USER!");
                        _possibleResponses.Add("I bet you'd like that wouldn't you &USER?");
                        _possibleResponses.Add("No, fuck you &USER!");
                        _possibleResponses.Add("Fuck you too &USER!");

                        await
                            e.Channel.SendMessage(
                                _possibleResponses[RandomSeed.Next(0, _possibleResponses.Count)].Replace("&USER",
                                    e.User.Name));
                    }
                    else
                    {
                        await
                            e.Channel.SendMessage($"GiantBombBot ver. 0.2.0 \"Beastcast, Best cast.\"" + Environment.NewLine + 
                            "use !help" +
                            $"Made by {e.Channel.GetUser(85817630560108544).Mention}.");
                    }
                }
            }
        }

        public static string GetResponseUriFromRandomQlCrew()
        {
            string url = "http://qlcrew.com/main.php?anyone=anyone&inc%5B0%5D=&p=999&exc%5B0%5D=&per_page=15&random";

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            // ReSharper disable once PossibleNullReferenceException
            request.UserAgent = "Giant Bomb Discord bot fetching random quick looks";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            return response.ResponseUri.AbsoluteUri;
        }
    }
}
