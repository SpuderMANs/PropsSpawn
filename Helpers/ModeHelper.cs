using Exiled.API.Features;
using System.Collections.Generic;

namespace PropSpawn.Helpers
{
    public static class ModeHelpers
    {
        private static readonly Dictionary<Exiled.API.Features.Player, Mode> PlayerModes = new();
        private static readonly Dictionary<Exiled.API.Features.Player, RotateMode> PlayerRotateModes = new();

        public static Mode GetMode(Exiled.API.Features.Player player)
        {
            if (!PlayerModes.TryGetValue(player, out var mode))
            {
                mode = Mode.Spawn;
                PlayerModes[player] = mode;
            }
            return mode;
        }
        public static RotateMode GetRotateMode(Player player)
        {
            if (!PlayerRotateModes.TryGetValue(player, out var mode))
            {
                mode = RotateMode.RotateX; 
                PlayerRotateModes[player] = mode;
            }
            return mode;
        }

        public static void SetNextMode(Exiled.API.Features.Player player)
        {
            PlayerModes[player] = GetNextMode(GetMode(player));
        }
        public static void SetNextRotateMode(Exiled.API.Features.Player player)
        {
            PlayerRotateModes[player] = GetNextRotateMode(GetRotateMode(player));
        }

        public static void Clear(Exiled.API.Features.Player player)
        {
            PlayerModes.Remove(player);
        }

        public static void ClearRotateMode(Exiled.API.Features.Player player)
        {
            PlayerRotateModes.Remove(player);
        }
        private static Mode GetNextMode(Mode mode)
        {
            return mode switch
            {
                Mode.Spawn => Mode.Delete,
                Mode.Delete => Mode.Move,
                Mode.Move => Mode.Rotate,
                Mode.Rotate => Mode.Spawn,
                _ => Mode.Spawn
            };
        }

        private static RotateMode GetNextRotateMode(RotateMode mode)
        {
            return mode switch
            {
                RotateMode.RotateX => RotateMode.RotateY,
                RotateMode.RotateY => RotateMode.RotateZ,
                RotateMode.RotateZ => RotateMode.RotateX,
                _ => RotateMode.RotateX
            };
        }
    }
}
