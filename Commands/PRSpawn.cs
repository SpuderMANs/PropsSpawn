using CommandSystem;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using Mirror;
using PropSpawn.Helpers;
using RemoteAdmin;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Player = Exiled.API.Features.Player;

namespace PropSpawn.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class PropSpawnCommand : ICommand
    {
        private CustomItemConfig cfg => Main.Instance.Config.PropPistolConfig;
        private HintConfig hcfg => Main.Instance.Config.GuiConfig;

        public string Command => "pr";
        public string[] Aliases => Array.Empty<string>();
        public string Description => "Manage spawning, deleting, saving, loading, and listing props.";

        private static readonly Dictionary<Player, List<GameObject>> SpawnedProps = new();

        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (sender is not PlayerCommandSender playerSender)
            {
                response = "This command can only be used by a player.";
                return false;
            }

            Player player = Player.Get(playerSender);

            if (arguments.Count == 0)
            {
                response = "Usage: pr spawn <name> | pr list | pr save <filename> | pr load <filename> | pr toolgun";
                return false;
            }

            string subCmd = arguments.At(0).ToLower();

            switch (subCmd)
            {
                case "spawn":
                    if (arguments.Count < 2)
                    {
                        response = "Specify prop name for toolgun. Example: pr spawn box";
                        return false;
                    }

                    string propName = string.Join(" ", arguments.Skip(1)).ToLower();
                    PropHelper.Set(player, propName);
                    response = $"Toolgun prop set to '{propName}'.";
                    return true;

                case "list":
                    return PropHelper.TryList(out response);

                case "save":
                    if (arguments.Count < 2)
                    {
                        response = "Specify filename. Example: pr save mymap";
                        return false;
                    }
                    return PropHelper.TrySave(player, arguments.At(1), out response);

                case "load":
                    if (arguments.Count < 2)
                    {
                        response = "Specify filename. Example: pr load mymap";
                        return false;
                    }
                    return PropHelper.TryLoad(player, arguments.At(1), out response);

                case "toolgun":
                    response = "You recived toolgun";
                    return CustomItem.TryGive(player, cfg.Name);

                default:
                    response = $"Unknown subcommand: {subCmd}";
                    return false;
            }

        }

    }
}
