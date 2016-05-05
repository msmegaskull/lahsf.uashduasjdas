using System;
using System.Collections.Generic;
using System.Linq;
using Discord.Modules;
using Discord.Commands;
using NadekoBot.Modules.Permissions.Classes;
using NadekoBot.Extensions;
using System.Text.RegularExpressions;

namespace NadekoBot.Modules.CustomReactions
{
    class CustomReactionsModule : DiscordModule
    {
        public override string Prefix { get; } = "";

        public override void Install(ModuleManager manager)
        {

            manager.CreateCommands("", cgb =>
             {

                 cgb.AddCheck(PermissionChecker.Instance);
                 Random range = new Random();
                 Dictionary<string, Func<CommandEventArgs, string>> commandFuncs = new Dictionary<string, Func<CommandEventArgs, string>>
                 {
                    {"%rng%", (e) =>  range.Next().ToString()},
                    {"%mention%", (e) => NadekoBot.BotMention },
                    {"%user%", e => e.User.Mention },
                    {"%target%", e => e.GetArg("args")?.Trim() ?? "" },
                 };

                

                 foreach (var command in NadekoBot.Config.CustomReactions)
                 {
                     var commandName = command.Key.Replace("%mention%", NadekoBot.BotMention);

                     var c = cgb.CreateCommand(commandName);
                     c.Description($"Custom reaction.\n**Usage**:{command.Key}");
                     c.Parameter("args", ParameterType.Unparsed);
                     c.Do(async e =>
                     {
                         string str = command.Value[range.Next(0, command.Value.Count())];
                         commandFuncs.Keys.ForEach(k => str = str.Replace(k, commandFuncs[k](e)));
                         await e.Channel.SendMessage(str).ConfigureAwait(false);
                     });
                 }

             });
        }
    }
}