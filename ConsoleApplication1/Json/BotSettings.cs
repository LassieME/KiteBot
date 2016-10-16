﻿// Generated by Xamasoft JSON Class Generator
// http://www.xamasoft.com/json-class-generator

using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KiteBot.Json
{

    internal class BotSettings
    {

        [JsonProperty("DiscordEmail")]
        public object DiscordEmail { get; set; }

        [JsonProperty("DiscordPassword")]
        public object DiscordPassword { get; set; }

        [JsonProperty("DiscordToken")]
        public string DiscordToken { get; set; }

        [JsonProperty("GiantBombApiKey")]
        public string GiantBombApiKey { get; set; }

        [JsonProperty("OwnerId")]
        public long OwnerId { get; set; }

        [JsonProperty("MarkovChainStart")]
        public bool MarkovChainStart { get; set; }

        [JsonProperty("MarkovChainDepth")]
        public int MarkovChainDepth { get; set; }

        [JsonProperty("GiantBombVideoRefreshRate")]
        public int GiantBombVideoRefreshRate { get; set; }

        [JsonProperty("GiantBombLiveStreamRefreshRate")]
        public int GiantBombLiveStreamRefreshRate { get; set; }
    }

}
