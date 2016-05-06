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
		private static Timer _chatTimer;//Garbage collection doesnt like local variables that only fire a couple times per hour
		private XElement _latestXElement;
		private bool _wasStreamRunning;
        

        public LivestreamChecker(string gBapi,int streamRefresh)
        {
            RefreshRate = streamRefresh;
            ApiCallUrl = "http://www.giantbomb.com/api/chats/?api_key=" + gBapi;
            _chatTimer = new Timer();
            _chatTimer.Elapsed += async (s,e) => await RefreshChatsApi();
            _chatTimer.Interval = streamRefresh;
            _chatTimer.AutoReset = true;
            _chatTimer.Enabled = true;
        }

        public async Task ForceUpdateChannel()
        {
            var temp = await GetXDocumentFromUrl(ApiCallUrl);
            var stream = temp.Element("results").Element("stream");
            var title = deGiantBombifyer(stream.Element("title").Value);
            var deck = deGiantBombifyer(stream.Element("deck").Value);

            await UpdateChannel("livestream-live",
                            $"Currently Live on Giant Bomb: {title}\n http://www.giantbomb.com/chat/");
        }

        private async Task RefreshChatsApi()
        {
            if (Program.Client.Servers.Any())
            {
                _chatTimer.Stop();
                _latestXElement = await GetXDocumentFromUrl(ApiCallUrl);

                if (_wasStreamRunning == false && !_latestXElement.Element("number_of_page_results").Value.Equals("0"))
                {
                    _wasStreamRunning = true;

                    var stream = _latestXElement.Element("results").Element("stream");
                    var title = deGiantBombifyer(stream.Element("title").Value);
                    var deck = deGiantBombifyer(stream.Element("deck").Value);

                    await
                        Program.Client.GetChannel(106390533974294528)
                            .SendMessage(title + ": " + deck +
                                         " is LIVE at http://www.giantbomb.com/chat/ NOW, check it out!");
                    await
                        UpdateChannel("livestream-live",
                            $"Currently Live on Giant Bomb: {title}\n http://www.giantbomb.com/chat/");
                }
                else if (_wasStreamRunning && _latestXElement.Element("number_of_page_results").Value.Equals("0"))
                {
                    _wasStreamRunning = false;
                    await
                        Program.Client.GetChannel(106390533974294528)
                            .SendMessage("Show is over folks, if you need more Giant Bomb videos, check this out: " +
                                         KiteChat.GetResponseUriFromRandomQlCrew());
                    await
                        UpdateChannel("livestream",
                            $"Chat for live broadcasts.\nTODO: Add upcoming livestreams here.");
                }
                _chatTimer.Start();
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
		        WebClient client = new WebClient();
		        client.Headers.Add("user-agent",
		            $"Bot for fetching livestreams and new content for the GiantBomb Shifty Discord Server. GETs every {RefreshRate/1000/60} minutes.");
		        XDocument document = XDocument.Load(await client.OpenReadTaskAsync(url));
		        return document.XPathSelectElement(@"//response");
		    }
		    catch (Exception ex)
		    {
                Console.WriteLine("Fetching livestreams failed. " + ex.Message);
		        await Task.Delay(5000);
		        return await GetXDocumentFromUrl(url);
		    }
		}

        private async Task UpdateChannel(string newName, string newTopic)
        {
            try
            {
                var channel = Program.Client.GetChannel(106390533974294528);
                await channel.Edit(newName, newTopic);
            }
            catch (Exception)
            {
                Console.WriteLine("Couldn't change Channel name");
            }
        }
	}
}
