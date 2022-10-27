using System.ComponentModel;
using Discord;
using Discord.Commands;
using Discord.Interactions;

namespace BitsyBot.Modules
{
    public class ReactionRoleModule : InteractionModuleBase<SocketInteractionContext>
    {
        [Discord.Interactions.Group("reaction_role", "Collection of reactionRole related commands")]
        public class ReactionRole : InteractionModuleBase<SocketInteractionContext>
        {
            [SlashCommand("add", "Add a reaction role")]
            [Alias("create", "new")]
            public async Task AddReactionRole(
                [Description("Message the reaction should be applied to.")]
                String message,
                [Description("Emoji to react with.")] String emote,
                [Description("Role to apply.")] IRole role)
            {
                ulong messageId = Convert.ToUInt64(message);
                IMessage target = Context.Channel.GetMessageAsync(messageId).Result;
                if (target == null)
                {
                    await Context.Interaction.RespondAsync("Message not found.");
                    return;
                }

                try
                {
                    IEmote trigger = new Emoji(emote);
                    await target.AddReactionAsync(trigger);
                }
                catch (ArgumentException argumentException)
                {
                    await ReplyAsync("Invalid emote.");
                    Console.Error.WriteLine(argumentException);
                    return;
                }

                Data.DatabaseLoader.AddReactionRoleRole(messageId,emote,role.Id);
                await RespondAsync("Added!");
            }
        }
    }
}