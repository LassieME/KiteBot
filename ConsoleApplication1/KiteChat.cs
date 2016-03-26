using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Discord;

namespace KiteBot
{
    public class KiteChat
    {
        //private static Timer _chatTimer;
        public static Random RandomSeed;

		public static int RaeCounter;

        private static string[] _greetings;
        private static string[] _responses;
        private static string[] _mealResponses;
        private static string[] _bekGreetings;

        public static KitePizza KitePizza = new KitePizza();
        public static KiteSandwich KiteSandwich = new KiteSandwich();
		public static KiteDunk KiteDunk = new KiteDunk();
		public static DiceRoller DiceRoller = new DiceRoller();
		public static KitCoGame KiteGame = new KitCoGame();
		public static LivestreamChecker StreamChecker = new LivestreamChecker();
        public static GiantBombVideoChecker GbVideoChecker = new GiantBombVideoChecker();
        public static MultiTextMarkovChainHelper MultiDeepMarkovChain = new MultiTextMarkovChainHelper(Program.Client, 3);

        public static string ChatDirectory = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent?.Parent?.FullName;
        public static string GreetingFileLocation = ChatDirectory + "\\Content\\Greetings.txt";
        public static string ResponseFileLocation = ChatDirectory + "\\Content\\Responses.txt";
        public static string MealFileLocation = ChatDirectory + "\\Content\\Meals.txt";


        public KiteChat() : this(File.ReadAllLines(GreetingFileLocation), File.ReadAllLines(ResponseFileLocation),
                                File.ReadAllLines(MealFileLocation), new Random(DateTime.Now.Millisecond))
        {
        }

        public KiteChat(string[] arrayOfGreetings, string[] arrayOfResponses, string[] arrayOfMeals, Random randomSeed)
        {
			LoadBekGreetings();
            _greetings = arrayOfGreetings;
            _responses = arrayOfResponses;
            _mealResponses = arrayOfMeals;
            RandomSeed = randomSeed;
	        RaeCounter = 0;
        }

        public async Task AsyncParseChat(object s, MessageEventArgs e, DiscordClient client)
	    {
			Console.WriteLine("(" + e.User.Name + "/" + e.User.Id + ") - " + e.Message.Text);
		    IsRaeTyping(e);

            //add all messages to the Markov Chain list
            MultiDeepMarkovChain.Feed(e.Message);

            if (e.Channel.Name.ToLower().Contains("vinncorobocorps"))
			{
				string response = KiteGame.GetGameResponse(e.Message);
				if (response != null)
					await e.Channel.SendMessage(response);
			}

			else if (!e.Message.IsAuthor && e.Message.Text.StartsWith("/roll"))
			{
				await e.Channel.SendMessage(DiceRoller.ParseRoll(e.Message.Text));
			}

			else if (!e.Message.IsAuthor && 0 <= e.Message.Text.IndexOf("GetDunked"))
			{
				await e.Channel.SendMessage("http://i.imgur.com/QhcNUWo.gifv");
			}

            else if (!e.Message.IsAuthor && e.Message.Text.Contains(@"/saveJSON") && e.User.Id == 85817630560108544)
            {
                MultiDeepMarkovChain.Save();
                await e.Channel.SendMessage("Done.");
            }
            else if (!e.Message.IsAuthor && e.Message.Text.Contains(@"/saveExit") && e.User.Id == 85817630560108544)
            {
                MultiDeepMarkovChain.Save();
                await e.Channel.SendMessage("Done.");
                Environment.Exit(1);
            }
            else if (!e.Message.IsAuthor && (e.Message.Text.Contains(@"/testMarkov") || e.Message.Text.StartsWith(@"@KiteBot /tm")))
            {
                try
                {
                    await e.Channel.SendMessage(MultiDeepMarkovChain.GetSequence());
                    if (e.User.Id == 85817630560108544)
                    {
                        await e.Message.Delete();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    MultiDeepMarkovChain.Save();
                    Environment.Exit(1);
                }
            }

            else if (!e.Message.IsAuthor && e.Message.Text.StartsWith("@KiteBot"))
            {
                if (e.Message.Text.StartsWith("@KiteBot #420") || e.Message.Text.ToLower().StartsWith("@KiteBot #blaze") ||
                    0 <= e.Message.Text.ToLower().IndexOf("waifu", 0))
                {
                    await e.Channel.SendMessage("http://420.moe/");
                }
                else if (0 <= e.Message.Text.ToLower().IndexOf("help", 5))
                {
                    var nl = Environment.NewLine;
                    await e.Channel.SendMessage("Current Commands are:" + nl + "#420"
                                                        + nl + "randomql" + nl + "google" + nl + "youtube" + nl +
                                                        "kitedunk"
                                                        + nl + "/pizza" + nl + "Whats for dinner" + nl + "sandwich" + nl +
                                                        "RaeCounter"
                                                        + nl + "help");
                }
                else if (0 <= e.Message.Text.ToLower().IndexOf("randomql", 5))
                {
                    await
                        e.Channel.SendMessage(GetResponseUriFromRandomQlCrew());
                }
                else if (0 <= e.Message.Text.ToLower().IndexOf("raecounter", 0))
                {
                    await e.Channel.SendMessage(@"Rae has ghost-typed " + RaeCounter);
                }
                else if (0 <= e.Message.Text.ToLower().IndexOf("google", 0))
                {
                    await
                        e.Channel.SendMessage("http://lmgtfy.com/?q=" + e.Message.Text.ToLower().Substring(16).Replace(' ', '+'));
                }
                else if (0 <= e.Message.Text.ToLower().IndexOf("youtube", 0))
                {
                    if (e.Message.Text.Length > 16)
                    {
                        await e.Channel.SendMessage("https://www.youtube.com/results?search_query=" +
                            e.Message.Text.ToLower().Substring(17).Replace(' ', '+'));
                    }
                    else
                    {
                        await e.Channel.SendMessage("Please add a query after youtube, starting with a space.");
                    }
                }

                else if (0 <= e.Message.Text.ToLower().IndexOf("dunk", 0))
                {
                    await e.Channel.SendMessage(KiteDunk.GetUpdatedKiteDunk());
                }
                else if (0 <= e.Message.Text.ToLower().IndexOf("fuck you", 0) ||
                         0 <= e.Message.Text.ToLower().IndexOf("fuckyou", 0))
                {
                    List<string> _possibleResponses = new List<string>();
                    _possibleResponses.Add("Hey fuck you too USER!");
                    _possibleResponses.Add("I bet you'd like that wouldn't you USER?");
                    _possibleResponses.Add("No, fuck you USER!");
                    _possibleResponses.Add("Fuck you too USER!");

                    await
                        e.Channel.SendMessage(_possibleResponses[RandomSeed.Next(0, _possibleResponses.Count)].Replace("USER",
                                e.User.Name));
                }
                else if (0 <= e.Message.Text.ToLower().IndexOf("/pizza", 0))
                {
                    await e.Channel.SendMessage(KitePizza.ParsePizza(e.User.Name, e.Message.Text));
                }
                else if (0 <= e.Message.Text.ToLower().IndexOf("sandwich", 0))
                {
                    await e.Channel.SendMessage(KiteSandwich.ParseSandwich(e.User.Name));
                }
                else if (0 <= e.Message.Text.ToLower().IndexOf("hi", 0) ||
                         0 <= e.Message.Text.ToLower().IndexOf("hey", 0) ||
                         0 <= e.Message.Text.ToLower().IndexOf("hello", 0))
                {
                    await e.Channel.SendMessage(ParseGreeting(e.User.Name));
                }
                else if (0 <= e.Message.Text.ToLower().IndexOf("/meal", 0) ||
                         0 <= e.Message.Text.ToLower().IndexOf("dinner", 0)
                         || 0 <= e.Message.Text.ToLower().IndexOf("lunch", 0))
                {
                    await e.Channel.SendMessage(_mealResponses[RandomSeed.Next(0, _mealResponses.Length)].Replace("USER",
                                e.User.Name));
                }
                else
                {
                    await
                        e.Channel.SendMessage("KiteBot ver. 1.0.1 \"Done. I hate computers.\"");
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
            request.UserAgent = "LassieMEKiteBot/0.9 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			return response.ResponseUri.AbsoluteUri;
		}
        
        //returns a greeting from the greetings.txt list on a per user or generic basis
	    private string ParseGreeting(string userName)
        {
		    if (userName.Equals("Bekenel") || userName.Equals("Pete"))
		    {
			    return (_bekGreetings[RandomSeed.Next(0, _bekGreetings.Length)]);
		    }
			else
			{
				List<string> _possibleResponses = new List<string>();

				for (int i = 0; i < _greetings.Length - 2; i += 2)
				{
					if (userName.ToLower().Contains(_greetings[i]))
					{
						_possibleResponses.Add(_greetings[i + 1]);
					}
				}

                if (_possibleResponses.Count == 0)
                {
                    for (int i = 0; i < _greetings.Length - 2; i += 2)
                    {
                        if (_greetings[i] == "generic")
                        {
                            _possibleResponses.Add(_greetings[i + 1]);
                        }
                    }
                }

				//return a random response from the context provided, replacing the string "USER" with the appropriate username
				return (_possibleResponses[RandomSeed.Next(0, _possibleResponses.Count)].Replace("USER", userName));
		    }
		    
        }

        //grabs random greetings for user bekenel from a reddit profile
		private void LoadBekGreetings()
		{
			const string url = "https://www.reddit.com/user/UWotM8_SS";
			string htmlCode = null;
		    try
		    {
		        using (WebClient client = new WebClient())
		        {
		            htmlCode = client.DownloadString(url);
		        }
		    }
		    catch (Exception e)
		    {
		        Console.WriteLine("Could not load Bek greetings, server not found: " + e.Message);
		    }
		    finally
		    {
		        var regex1 = new Regex(@"<div class=""md""><p>(?<quote>.+)</p>");
		        if (htmlCode != null)
		        {
		            var matches = regex1.Matches(htmlCode);
		            var stringArray = new string[matches.Count];
		            var i = 0;
		            foreach (Match match in matches)
		            {
		                var s = match.Groups["quote"].Value.Replace("&#39;", "'").Replace("&quot;", "\"");
		                stringArray[i] = s;
		                i++;
		            }
		            _bekGreetings = stringArray;
		        }
		    }
		}

        public void IsRaeTyping(MessageEventArgs e)
        {
            if (e.User.Id == 85876755797139456)
            {
                RaeCounter += -1;
            }
        }

        public void IsRaeTyping(ChannelUserEventArgs channelUserEventArgs)
        {
            if (channelUserEventArgs.User.Id == 85876755797139456)
            {
                RaeCounter += 1;
            }
        }
    }
}
