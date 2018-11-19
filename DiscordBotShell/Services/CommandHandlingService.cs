using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace DiscordBot.Services
{
    public class CommandHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private IServiceProvider _provider;
        List<string> _authorizedUsers = new List<string>() {};

        public CommandHandlingService(IServiceProvider provider, DiscordSocketClient discord, CommandService commands)
        {
            _discord = discord;
            _commands = commands;
            _provider = provider;

            _discord.MessageReceived += MessageReceived;
        }

        public async Task InitializeAsync(IServiceProvider provider)
        {
            _provider = provider;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
            // Add additional initialization code here...
        }

        private async Task MessageReceived(SocketMessage rawMessage)
        {
            // Ignore system messages and messages from bots
            if (!(rawMessage is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            int argPos = 0;
            //if (!message.HasMentionPrefix(_discord.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_discord, message);
            var result = await _commands.ExecuteAsync(context, argPos, _provider);

            if (result.Error.HasValue && 
                result.Error.Value != CommandError.UnknownCommand)
                await context.Channel.SendMessageAsync(result.ToString());


            ParseCommands(rawMessage);
        }
        public async void ParseCommands(SocketMessage message)
        {
            var user = message.Author as SocketGuildUser;
            if (message.Channel.Name != "movie-night" && message.Channel.GetType() != typeof(SocketDMChannel))
            {
                return;
            }
            string[] splits = message.Content.Split(' ');
            string command = splits[0];
            string restOfMessage = String.Join(" ", (splits.Skip(1).Take(splits.Length - 1).ToArray()));

            var user2 = message.Author as SocketGuildUser;
            int userGuildCount = 0;
            SocketGuild guild = null;
            ISocketMessageChannel channel = null;
            ulong currentGuildID = 0;
            switch (command)
            {
                case "Example":

                    break;
            }
            
        }
        public async void DisplayMessage(ISocketMessageChannel channel, string message)
        {
               await channel.SendMessageAsync(message);
        }
       
        public bool CheckForAdmin(SocketMessage message)
        {
            var user = message.Author as SocketGuildUser;
            if (user.GuildPermissions.Administrator == true)
            {
                return true; 
            }
            else
            {
                return false;
            }
        }
    }
}
