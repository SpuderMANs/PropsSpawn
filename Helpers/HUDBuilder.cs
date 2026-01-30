using Exiled.API.Features;
using System.Collections.Generic;

namespace PropSpawn.Helpers
{
    public static class HudHelper
    {
        private static readonly Dictionary<int, string> LastAction = new();

        public static void SetAction(Player player, string text)
        {
            if (player == null) return;
            LastAction[player.Id] = text;
        }

        public static void ClearAction(Player player)
        {
            if (player == null) return;
            LastAction.Remove(player.Id);
        }

        private static string GetAction(Player player)
        {
            if (player == null) return "";
            return LastAction.TryGetValue(player.Id, out var text) ? text : "";
        }
        public static string Build(Player player, HintConfig cfg)
        {
            if (player == null)
                return string.Empty;

            Mode mode = ModeHelpers.GetMode(player);
            string prop = PropHelper.Get(player);
            string action = GetAction(player);

            string propLine = "";
            if (mode == Mode.Spawn && !string.IsNullOrEmpty(prop))
            {
                propLine =
                    $"<color=white>Prop:</color> <color=green>{prop}</color>\n";
            }

            string actionLine = string.IsNullOrEmpty(action)
                ? ""
                : $"<color=grey>{action}</color>\n";

            return cfg.HintContent
                .Replace("{mode}", mode.ToString())
                .Replace("{prop_line}", propLine)
                .Replace("{action_line}", actionLine);
        }
    }
}
