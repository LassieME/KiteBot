﻿using System;
using System.Net;
using System.Text.RegularExpressions;
using Discord;
using System.Collections.Generic;

namespace KiteBot
{
    class Program
    {
		public static DiscordClient Client;
        public static CryptoRandom Random;

        private static void Main(string[] args)
	    {
		    Client = new DiscordClient();
		    Random = new CryptoRandom();
		    var kiteDunk = new KiteDunk();
		    var kiteChat = new KiteChat();
		    var giantBombRss = new GiantBombRss();
            var diceRoller = new DiceRoller();
		    //bool shutUp = false;
		    
			//Display all log messages in the console
			Client.LogMessage += (s, e) => Console.WriteLine("[{"+e.Severity+"}] {"+e.Source+"}: {"+e.Message+"}");
			
			//TODO: Rewrite this as a State Machine
			Client.MessageReceived += async (s, e) =>
			{
				Console.WriteLine("(" + e.User.Name + "/"+ e.User.Discriminator + ") -" + e.Message.Text);
				if (!e.Message.IsAuthor && e.Message.Text.StartsWith("/roll"))
				{
					await Client.SendMessage(e.Channel , diceRoller.ParseRoll(e.Message.Text));
				}

                else if (!e.Message.IsAuthor && 0 <= e.Message.Text.IndexOf("GetDunked"))
				{
					await Client.SendMessage(e.Channel, "http://i.imgur.com/QhcNUWo.gifv");
				}


				if (!e.Message.IsAuthor && e.Message.Text.StartsWith("@KiteBot"))
				{
					if (e.Message.Text.StartsWith("@KiteBot #420") || e.Message.Text.ToLower().StartsWith("@KiteBot #blaze") ||
					    0 <= e.Message.Text.ToLower().IndexOf("waifu", 0))
					{
						await Client.SendMessage(e.Channel, "http://420.moe/");
					}
					else if (0 <= e.Message.Text.ToLower().IndexOf("help", 5))
					{
						var nl = Environment.NewLine;
						await Client.SendMessage(e.Channel, "Current Commands are:" + nl + "#420" 
							+ nl + "randomql" + nl + "google" + nl + "youtube" + nl + "kitedunk" 
							+ nl + "/pizza" + nl + "Whats for dinner" + nl + "sandwich" + nl + "help");
					}
					else if (0 <= e.Message.Text.ToLower().IndexOf("randomql", 5))
					{
						await Client.SendMessage(e.Channel, getResponseUriFromRandomQLCrew("http://qlcrew.com/main.php?anyone=anyone&inc%5B0%5D=&p=999&exc%5B0%5D=&per_page=15&random"));
					}
					else if (0 <= e.Message.Text.ToLower().IndexOf("google", 0))
					{
						await
							Client.SendMessage(e.Channel, "http://lmgtfy.com/?q=" + e.Message.Text.ToLower().Substring(16).Replace(' ', '+'));
					}
					else if (0 <= e.Message.Text.ToLower().IndexOf("youtube", 0))
					{
						if (e.Message.Text.Length > 16)
						{
							await Client.SendMessage(e.Channel,
								"https://www.youtube.com/results?search_query=" + e.Message.Text.ToLower().Substring(17).Replace(' ', '+'));
						}
						else
						{
							await Client.SendMessage(e.Channel, "Please add a query after youtube, starting with a space.");
						}
					}

					else if (0 <= e.Message.Text.ToLower().IndexOf("dunk", 0))
					{
						await Client.SendMessage(e.Channel, kiteDunk.GetUpdatedKiteDunk());
					}
					else
					{
						await Client.SendMessage(e.Channel, kiteChat.ParseChatResponse(e.Message.User.Name, e.Message.Text ));
					}
				}
			};

			//Convert our sync method to an async one and block the Main function until the bot disconnects
			Client.Run(async () =>
			{
				//Connect to the Discord server using our email and password
				await Client.Connect(Properties.auth.Default.DiscordEmail,Properties.auth.Default.DiscordPassword);
			});
        }

	    public static void SendMessage(string message)
	    {
			//TODO: make this server generic
		    Client.SendMessage(Client.GetChannel(85842104034541568),message);
	    }

	    public static void SendMessage(object s, Feed.UpdatedFeedEventArgs e)
	    {
		    Client.SendMessage(Client.GetChannel(85842104034541568),
			    e.Title + " live now at GiantBomb.com\r\n" + e.Link);
	    }

	    private static string getResponseUriFromRandomQLCrew(string s)
	    {
		    string url = s;
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
		    return response.ResponseUri.AbsoluteUri;
	    }

	    private static void RandomTester()
	    {
			var testarray = new int[100];
			for (int i = 0; i < 100; i++)
			{
				testarray[i] = Random.Next(0, 10);
			}
			Array.Sort(testarray);
			var ta = new int[11];
			foreach (int i in testarray)
			{
				ta[i] += i;
			}
			Console.WriteLine(ta[0] + " " + ta[1] + " " + ta[2] + " " + ta[3] + " " + ta[4] + " " + ta[5] + " " + ta[6] + " " + ta[7] + " " + ta[8] + " " + ta[9] + " " + ta[10] + " ");
	    }
    }
}
