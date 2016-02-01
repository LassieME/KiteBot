﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace KiteBot
{
    public class TextMarkovChainHelper
    {
        private const int MaxMessages = 500;
        private static readonly Dictionary<long,List<Message>> ChannelMessages = 
            new Dictionary<long,List<Message>>();
        private static readonly Dictionary<long, TextMarkovChain> ChannelMarkovChains =
            new Dictionary<long, TextMarkovChain>();   
        private static DiscordClient _client;
        private static bool _isInitialized = false;

        public TextMarkovChainHelper() : this(Program.Client)
        {
        }

        public TextMarkovChainHelper(DiscordClient client)
        {
            _client = client;
        }

        public async Task<bool> Initialize()
        {
            if (!_isInitialized)
            {
                foreach (Channel channel in _client.AllServers.SelectMany(server => server.Channels).Where(channel => ChannelType.Text == channel.Type))
                {
                    if (channel.Members.Contains(_client.GetUser(channel.Server,_client.CurrentUser.Id)))
                    {
                        ChannelMessages.Add(channel.Id, await GetMessagesFromChannel(_client, channel, MaxMessages));
                    }
                }
                _isInitialized = true;
            }
            return true;
        }

        public void Feed(Message message)
        {
            if (ChannelMessages.ContainsKey(message.Channel.Id))
            {
                ChannelMessages[message.Channel.Id].Add(message);

                if (ChannelMarkovChains.ContainsKey(message.Channel.Id))
                {
                    ChannelMarkovChains[message.Channel.Id].feed(message.Text);
                }
            }
            else
            {
                ChannelMessages.Add(message.Channel.Id, GetMessagesFromChannel(_client,message.Channel, MaxMessages).Result);
            }
        }

        public string GetSequenceForChannel(Channel channel)
        {
            TextMarkovChain textMarkovChain;
            if (ChannelMarkovChains.TryGetValue(channel.Id, out textMarkovChain))
            {
                return textMarkovChain.generateSentence();
            }
            else
            {
                textMarkovChain = new TextMarkovChain();
                foreach (Message message in ChannelMessages[channel.Id])
                {
                    textMarkovChain.feed(message.Text);
                }
                ChannelMarkovChains.Add(channel.Id, textMarkovChain);
                return textMarkovChain.generateSentence();
            }
        }

        //
        private async Task<List<Message>> GetMessagesFromChannel(DiscordClient client, Channel channel, int i)
        {
            List<Message> messages = new List<Message>();
            var latestMessage = client.DownloadMessages(channel, i);
            messages.AddRange(client.DownloadMessages(channel, i).Result);

            long tmpMessageTracker = latestMessage.Id;

            while (messages.Count < MaxMessages)
            {
                messages.AddRange(await Program.Client.DownloadMessages(channel, 100, tmpMessageTracker, RelativeDirection.Before));

                long newMessageTracker = messages[messages.Count - 1].Id;
                if (tmpMessageTracker != newMessageTracker)     //Checks if there are any more messages in channel, and if not, returns the List
                {
                    tmpMessageTracker = newMessageTracker;      //grabs the last message added, and uses the new Id as the start of the next query
                }
                else
                {
                    messages.RemoveAt(messages.Count - 1);//removes the excessive object
                    return messages;
                }
            }
            return messages;
        }
    }
}
