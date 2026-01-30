using HintServiceMeow.Core.Utilities;
using Exiled.API.Features;
using System;
using System.Collections.Generic;
using Hint = HintServiceMeow.Core.Models.Hints.Hint;
using PropSpawn;

namespace PropSpawn.Helpers
{
    public static class HintHelper
    {

        private static readonly Dictionary<string, Hint> ActiveHints = new();

        public static void Show(
            Player player,
            string hintId,
            HintConfig config,
            Func<Player, string> autoText
        )
        {
            if (player == null)
                return;

            var display = PlayerDisplay.Get(player);
            if (display == null)
                return;

            var key = $"{player.Id}:{hintId}";

            if (!ActiveHints.TryGetValue(key, out var hint))
            {
                hint = new Hint
                {
                    Id = hintId,

                    AutoText = arg =>
                    {
                        var p = Player.Get(arg.PlayerDisplay.ReferenceHub);
                        return p == null ? string.Empty : autoText(p);
                    },

                    FontSize = config.FontSize,
                    XCoordinate = config.X,
                    YCoordinate = config.Y,
                    YCoordinateAlign = config.VerticalAlign,
                    SyncSpeed = config.SyncSpeed
                };

                ActiveHints[key] = hint;
                display.AddHint(hint);
            }
            else
            {
                hint.Hide = false;
            }
        }

        public static void Remove(Player player, string hintId)
        {
            if (player == null)
                return;

            var key = $"{player.Id}:{hintId}";

            if (ActiveHints.TryGetValue(key, out var hint))
            {
                hint.Hide = true;
                ActiveHints.Remove(key);
            }
        }

        public static void RemoveAll(Player player)
        {
            if (player == null)
                return;

            var keysToRemove = new List<string>();

            foreach (var kvp in ActiveHints)
            {
                if (!kvp.Key.StartsWith(player.Id.ToString()))
                    continue;

                kvp.Value.Hide = true;
                keysToRemove.Add(kvp.Key);
            }

            foreach (var key in keysToRemove)
                ActiveHints.Remove(key);
        }
    }
}