using Exiled.API.Features.Spawn;
using Exiled.API.Interfaces;
using HintServiceMeow.Core.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PropSpawn
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = true;

        [Description("Configs")]
        public string MapToLoad { get; set; } = "YourMapHere";

        [Description("PropPistol Configs")]
        public CustomItemConfig PropPistolConfig { get; set; } = new();

        [Description("GUI Configs")]
        public HintConfig GuiConfig { get; set; } = new();

        [Description("Prefabs/Props Configs")]
        public float SpawnOffset { get; set; } = 0.1f;
        public float AngularScale { get; set; } = 10f;
    }

    public class CustomItemConfig
    {
        public uint Id { get; set; } = 1;
        public string Name { get; set; } = "PropPistol";
        public string Description { get; set; } = "Spawn, Delete, Move prefab.";
        public ItemType Type { get; set; } = ItemType.GunCOM15;
        public float Weight { get; set; } = 1f;

        public SpawnProperties SpawnProperties { get; set; } = new SpawnProperties();
    }
    public class HintConfig
    {
        [Description("The Hint text")]
        public string HintContent { get; set; } =
            "<color=yellow><b>Toolgun</b></color>\n" +
            "Current Mode: {mode}\n" +
            "{prop_line}\n" +
            "{action_line}";


        [Description("The Font size ")]
        public int FontSize { get; set; } = 30;

        [Description("The X position")]
        public float X { get; set; } = 30f;

        [Description("The Y position")]
        public float Y { get; set; } = 1020f;

        [Description("The Vertical alignment")]
        public HintVerticalAlign VerticalAlign { get; set; } = HintVerticalAlign.Middle;

        [Description("The Hint sync speed")]
        public HintSyncSpeed SyncSpeed { get; set; } = HintSyncSpeed.Normal;

        [Description("The Hint duration")]
        public float HintDuration = 0f;
    }
}
