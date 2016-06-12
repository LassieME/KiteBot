using System;
using System.Net;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.XPath;
using Discord;
using Discord.Commands;
using Newtonsoft.Json;

namespace GiantBombBot.Commands
{
    class Game
    {
        public static string ApiCallUrl;
        private static int _retry;

        public static void RegisterGameCommand(DiscordClient client, string gBapi)
        {
            ApiCallUrl =
                $"http://www.giantbomb.com/api/games/?api_key={gBapi}&field_list=deck,image,name,original_release_date,platforms,site_detail_url&filter=name:";
            client.Services.Get<CommandService>().CreateCommand("game") //create command greet
                    .Alias("games","giantbomb") //add 2 aliases, so it can be run with ~gr and ~hi
                    .Description("Gets the first game with the given name or alias from the GiantBomb games api endpoint") //add description, it will be shown when ~help is used
                    .Parameter("GameTitle", ParameterType.Unparsed) //as an argument, we have a person we want to greet
                    .Do(async e =>
                    {
                        var args = e.GetArg("GameTitle");
                        if (!string.IsNullOrWhiteSpace(args))
                        {
                            string s = await GetGamesEndpoint(args).ConfigureAwait(false);
                            await e.Channel.SendMessage(s);
                        }
                        else
                        {
                            await e.Channel.SendMessage($"Empty game name given, please spesify a game title");
                        }
                        //await e.Channel.SendMessage($"{e.User.Name} sucks. {e.GetArg("GameTitle")}");
                    });
        }

        private static async Task<string> GetGamesEndpoint(string gameTitle)
        {
            string output;
            try
            {
                var document = await GetXDocumentFromUrl(ApiCallUrl + gameTitle);
                
                var firstResult = document.Element("results")?.Element("game");
                if (firstResult != null)
                {
                    string site_detail_url = firstResult.Element(@"site_detail_url").Value;
                    string original_release_date = firstResult.Element(@"original_release_date").Value;
                    string name = firstResult.Element(@"name").Value;
                    string platforms = "";
                    string deck = firstResult.Element(@"deck").Value;
                    string super_url = firstResult.Element(@"image").Element("super_url").Value;

                    foreach (var element in firstResult.Elements(@"platforms"))
                    {
                        if (platforms.Equals(String.Empty))
                        {
                            platforms += element.Element("platform").Element("name").Value;
                        }
                        else
                        {
                            platforms += ", " + element.Element("platform").Element("name").Value;
                        }
                    }
                    platforms += ".";
                    GameResult gameResult = new GameResult(site_detail_url, original_release_date, name, platforms, deck,
                        super_url);
                    output = gameResult.ToString();
                }
                else
                {
                    output = "Something bad happened. Try again later";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex);
                output = "Something bad happened. Try again later";
            }
            return output;
        }

        private static async Task<XElement> GetXDocumentFromUrl(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent",
                        $"Bot for fetching livestreams, new content and the occasional wiki page for the GiantBomb Shifty Discord Server.");
                    XDocument document = XDocument.Load(await client.OpenReadTaskAsync(url).ConfigureAwait(false));
                    return document.XPathSelectElement(@"//response");
                }
            }
            catch (Exception)
            {
                _retry++;
                if (_retry < 3)
                {
                    await Task.Delay(10000);
                    return await GetXDocumentFromUrl(url).ConfigureAwait(false);
                }
                throw new TimeoutException();
            }
        }
        private static string deGiantBombifyer(string s)
        {
            return s.Replace("<![CDATA[ ", "").Replace(" ]]>", "");
        }
    }
    public class GameResult
    {
        public GameResult(string siteDetailUrl, string originalReleaseDate, string title, string gamingPlatforms, string deck1, string superUrl)
        {
            site_detail_url = siteDetailUrl;
            original_release_date = originalReleaseDate;
            name = title;
            platforms = gamingPlatforms;
            deck = deck1;
            super_url = superUrl;
        }

        public string site_detail_url { get; set; }
        public string original_release_date { get; set; }
        public string name { get; set; }
        public string platforms { get; set; }
        public string deck { get; set; }
        public string super_url { get; set; }

        public override string ToString() =>
            "`Title:` **" + name +
            "**\n`Deck:` " + deck +
            "\n`Release Date:` " + original_release_date +
            "\n`Platforms:` " + platforms +
            "\n`Link:` " + site_detail_url +
            "\n`img:` " + super_url;
    }
}