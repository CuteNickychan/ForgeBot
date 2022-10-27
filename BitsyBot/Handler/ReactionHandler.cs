using BitsyBot.Data;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;

namespace BitsyBot.Handler;

public class ReactionHandler
{

    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _config;
    private readonly IServiceProvider _services;

    public ReactionHandler(DiscordSocketClient client, IServiceProvider services,
        IConfiguration config)
    {
        _client = client;
        _services = services;
        _config = config;
    }

    public async Task InitializeAsync()
    {
        _client.ReactionAdded += HandleReactionAddedAsync;
        _client.ReactionRemoved += HandleReactionRemovedAsync;
        // _client.ReactionsCleared += HandleReactionsClearedAsync;
        // _client.ReactionsRemovedForEmote += HandleReactionsRemovedForEmoteAsync;
    }

    private Task HandleReactionAddedAsync(Cacheable<IUserMessage, ulong> messageId, Cacheable<IMessageChannel, ulong> channelId, SocketReaction reaction)
    {
        if (reaction.User.Value.IsBot)
            return Task.CompletedTask;

        if (DatabaseLoader.GetRole(messageId.Id, reaction.Emote.Name) != 0)
        {
            var guild = (channelId.Value as SocketGuildChannel)?.Guild;
            if (guild == null)
            {
                return Task.CompletedTask;
            }

            var role = guild.GetRole(DatabaseLoader.GetRole(messageId.Id, reaction.Emote.Name));
            var user = guild.GetUser(reaction.UserId);
            var guildUser = guild.GetUser(user.Id);

            guildUser.AddRoleAsync(role);
        }

        return Task.CompletedTask;
    }

    private Task HandleReactionRemovedAsync(Cacheable<IUserMessage, ulong> messageId, Cacheable<IMessageChannel, ulong> channelId, SocketReaction reaction)
    {
        if (reaction.User.Value.IsBot)
            return Task.CompletedTask;

        if (DatabaseLoader.GetRole(messageId.Id, reaction.Emote.Name) != 0)
        {
            //Get Role off message
            var guild = (channelId.Value as SocketGuildChannel).Guild;

            var role = guild.GetRole(DatabaseLoader.GetRole(messageId.Id, reaction.Emote.Name));
            var user = guild.GetUser(reaction.UserId);
            var guildUser = guild.GetUser(user.Id);

            guildUser.RemoveRoleAsync(role);
        }

        return Task.CompletedTask;
    }

    private Task HandleReactionsRemovedForEmoteAsync(Cacheable<IUserMessage, ulong> messageId, Cacheable<IMessageChannel, ulong> channelId, IEmote emote)
    {
        throw new NotImplementedException();
    }

    private Task HandleReactionsClearedAsync(Cacheable<IUserMessage, ulong> messageId, Cacheable<IMessageChannel, ulong> channelId)
    {
        throw new NotImplementedException();
    }
}