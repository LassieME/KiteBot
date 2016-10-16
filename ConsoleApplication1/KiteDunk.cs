﻿using Discord.Commands;
using System;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using KiteBot.Commands;

namespace KiteBot
{
    public class KiteDunk
	{
		private static string[,] _updatedKiteDunks;
	    private static Random _random;
		private const string GoogleSpreadsheetApiUrl = "http://spreadsheets.google.com/feeds/list/11024r_0u5Mu-dLFd-R9lt8VzOYXWgKX1I5JamHJd8S4/od6/public/values?hl=en_US&&alt=json";
		// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		private static Timer _kiteDunkTimer;

        public KiteDunk()
        {
	        _random = new Random();
	        UpdateKiteDunks().Wait();

			_kiteDunkTimer = new Timer();
            _kiteDunkTimer.Elapsed += async (s, e) => await UpdateKiteDunks();
			_kiteDunkTimer.Interval = 86400000;//24 hours
			_kiteDunkTimer.AutoReset = true;
			_kiteDunkTimer.Enabled = true;
        }

        // ~say hello -> hello
        [Command("KiteDunk"), Summary("Posts a hot Kite Dunk"), Alias("dunk")]
        public async Task KiteDunkCommand(IUserMessage msg)
        {
            await msg.Channel.SendMessageAsync(GetUpdatedKiteDunk());
        }

        [Command("KiteDunk"), Summary("Posts a hot Kite Dunk"), RequireOwner]
        public async Task KiteDunkAllCommand(IUserMessage msg)
        {
            var stringBuilder = new System.Text.StringBuilder(2000);
            for (int i = 0; i < _updatedKiteDunks.GetLength(0); i++)
            {
                var entry = "\"" + _updatedKiteDunks[i, 1] + "\" - " + _updatedKiteDunks[i, 0] + Environment.NewLine;
                if (stringBuilder.Length + entry.Length > 2000)
                {
                    await msg.Channel.SendMessageAsync(stringBuilder.ToString());
                    stringBuilder.Clear();
                }
                else
                {
                    stringBuilder.Append(entry);
                }
            }
            await msg.Channel.SendMessageAsync(stringBuilder.ToString());
        }

        public string GetUpdatedKiteDunk()
		{
			var i = _random.Next(_updatedKiteDunks.GetLength(0));
			return "\"" + _updatedKiteDunks[i, 1] + "\" - " + _updatedKiteDunks[i, 0];
		}

        public async Task UpdateKiteDunks()
        {
            try
            {
                string response;
                using (var client = new WebClient())
                {
                    response = await client.DownloadStringTaskAsync(GoogleSpreadsheetApiUrl);
                }
                var regex1 =
                    new Regex(
                        @"""gsx\$name"":{""\$t"":""(?<name>[0-9A-Za-z'""., +\-?!\[\]]+?)""},""gsx\$quote"":{""\$t"":""(?<quote>[0-9A-Za-z'""., +\-?!\[\]]+?)""}}",
                        RegexOptions.Singleline);
                var matches = regex1.Matches(response);
                string[,] kiteDunks = new string[matches.Count, 2];
                int i = 0;
                foreach (Match match in matches)
                {
                    kiteDunks[i, 0] = match.Groups["name"].Value;
                    kiteDunks[i++, 1] = match.Groups["quote"].Value;
                }
                _updatedKiteDunks = kiteDunks;
            }
            catch (Exception e)
            {
                Console.WriteLine("Update of KiteDunks failed, retrying... "+ e.Message);
                await Task.Delay(5000);
                await UpdateKiteDunks();
            }
        }
    }
}
