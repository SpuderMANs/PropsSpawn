using System;
using Exiled.API.Features;
using Exiled.CustomItems.API.Features;
using PropSpawn.EventHandler;

namespace PropSpawn
{
    public class Main : Plugin<Config>
    {
        public override string Name => "PropSpawn";
        public override string Author => "@SpuderMANs";
        public override string Prefix => "PropSpawn";
        public override Version RequiredExiledVersion => new Version(9, 12, 5);
        public override Version Version => new Version(1, 0, 0);

        public static Main Instance { get; private set; }

        private EventHandlers EventHandler;
        private PropPistol PropPistol;

        public override void OnEnabled()
        {
            Instance = this;

            Loader.Initialize(); 
            EventHandler = new EventHandlers();
            EventHandler.RegisterEvent();

            PropPistol = new PropPistol();
            CustomItem.RegisterItems();

            base.OnEnabled();
        }

        public override void OnDisabled()
        {
            Instance = null;

            CustomItem.UnregisterItems();

            EventHandler?.UnregisterEvent();
            EventHandler = null;

            PropPistol = null;

            base.OnDisabled();
        }
    }
}
