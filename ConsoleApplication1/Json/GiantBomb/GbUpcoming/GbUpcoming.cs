﻿using Newtonsoft.Json;

namespace KiteBot.Json.GiantBomb.GbUpcoming
{

    internal class GbUpcoming
    {

        [JsonProperty("liveNow")]
        public LiveNow LiveNow { get; set; }

        [JsonProperty("upcoming")]
        public Upcoming[] Upcoming { get; set; }
    }

    internal class LiveNow
    {

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }
    }

    internal class Upcoming
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("date")]
        public string Date { get; set; }

        [JsonProperty("premium")]
        public bool Premium { get; set; }
    }

}
