using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using System.Xml.XPath;

namespace GiantBombBot
{
    public class LivestreamChecker
    {
        public static string ApiCallUrl;
        public int RefreshRate;
        private static Timer _chatTimer;//Garbage collection...
        private XElement _latestXElement;
        private bool _wasStreamRunning;
        private int _retry;


        public LivestreamChecker(string gBapi, int streamRefresh)
        {
            if (streamRefresh > 3000)
            {
                ApiCallUrl = $"http://www.giantbomb.com/api/chats/?api_key={gBapi}&field_list=date_added,deck,title";
                RefreshRate = streamRefresh;
                _chatTimer = new Timer();
                _chatTimer.Elapsed += async (s, e) => await RefreshChatsApi();
                _chatTimer.Interval = streamRefresh;
                _chatTimer.AutoReset = true;
                _chatTimer.Enabled = true;
            }
        }

        public void Restart()
        {
            if (_chatTimer == null)
            {
                Console.WriteLine("_chatTimer eaten by GC");
                Environment.Exit(-1);
            }
            else if (_chatTimer.Enabled == false)
            {
                Console.WriteLine("Was off, turning LiveStream back on.");
                _chatTimer.Start();
                if (_chatTimer.AutoReset == false)
                {
                    Console.WriteLine("AutoReset was off");
                    _chatTimer.AutoReset = true;
                }
            }
        }

        public async Task ForceUpdateChannel()
        {
            var temp = await GetXDocumentFromUrl(ApiCallUrl);
            var stream = temp.Element("results")?.Element("stream");
            var title = deGiantBombifyer(stream?.Element("title")?.Value);

            await UpdateChannel("livestream-live",
                            $"Currently Live on Giant Bomb: {title}\n http://www.giantbomb.com/chat/");
        }

        private async Task RefreshChatsApi()
        {
            try
            {
                if (Program.Client.Servers.Any())
                {
                    try
                    {
                        _retry = 0;
                        _latestXElement = await GetXDocumentFromUrl(ApiCallUrl).ConfigureAwait(false);
                        var numberOfResults = _latestXElement.Element("number_of_page_results");

                        if (numberOfResults != null && _wasStreamRunning == false && !numberOfResults.Value.Equals("0"))
                        {
                            _wasStreamRunning = true;

                            var stream = _latestXElement.Element("results")?.Element("stream");
                            var title = deGiantBombifyer(stream?.Element("title")?.Value);
                            var deck = deGiantBombifyer(stream?.Element("deck")?.Value);

                            await
                                Program.Client.GetChannel(106390533974294528)
                                    .SendMessage(title + ": " + deck +
                                                 " is LIVE at http://www.giantbomb.com/chat/ NOW, check it out!")
                                    .ConfigureAwait(false);
                            await
                                UpdateChannel("livestream-live",
                                    $"Currently Live on Giant Bomb: {title}\n http://www.giantbomb.com/chat/")
                                    .ConfigureAwait(false);
                        }
                        else if (numberOfResults != null && _wasStreamRunning && numberOfResults.Value.Equals("0"))
                        {
                            _wasStreamRunning = false;
                            await
                                Program.Client.GetChannel(106390533974294528)
                                    .SendMessage(
                                        "Show is over folks, if you need more Giant Bomb videos, check this out: " +
                                        KiteChat.GetResponseUriFromRandomQlCrew()).ConfigureAwait(false);
                            await
                                UpdateChannel("livestream",
                                    "Chat for live broadcasts.\nTODO: Add upcoming livestreams here.")
                                    .ConfigureAwait(false);
                        }

                    }
                    catch (TimeoutException)
                    {
                        Console.WriteLine("Livestreamchecker timed out. Restarting Timer.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"LivestreamChecker sucks: {ex} \n {ex.Message}");
                var owner = Program.Client.Servers.FirstOrDefault()?
                    .Users.FirstOrDefault(x => x.Id == 85817630560108544);
                if (owner != null)
                    await owner.SendMessage($"LivestreamChecker threw an {ex.GetType()}, check the logs").ConfigureAwait(false);
            }
        }

        private string deGiantBombifyer(string s)
        {
            return s.Replace("<![CDATA[ ", "").Replace(" ]]>", "");
        }

        private async Task<XElement> GetXDocumentFromUrl(string url)
        {
            try
            {
                using (WebClient client = new WebClient())
                {
                    client.Headers.Add("user-agent",
                        $"Bot for fetching livestreams and new content for the GiantBomb Shifty Discord Server. GETs every {RefreshRate/1000/60} minutes.");
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

        private async Task UpdateChannel(string newName, string newTopic)
        {
            try
            {
                var channel = Program.Client.GetChannel(106390533974294528);
                await channel.Edit(newName, newTopic).ConfigureAwait(false);
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't change Channel name");
            }
        }
    }
}
