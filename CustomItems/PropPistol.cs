using Exiled.API.Features.Attributes;
using Exiled.API.Features.Spawn;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Scp096;
using Exiled.Events.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exiled.CustomItems.API.Features;
using Player = Exiled.Events.Handlers.Player;
using Exiled.Events.EventArgs.Player;
using PropSpawn.Commands;
using System.CodeDom;
using Achievements.Handlers;
using HSMHelper = PropSpawn.Helpers.HintHelper;
using ModeHelper = PropSpawn.Helpers.ModeHelpers;
using MEC;
using PropSpawn.Helpers;
using LabApi.Features.Wrappers;
using UnityEngine;
using Exiled.API.Features.Items;
using InventorySystem.Items;
using System.Net.NetworkInformation;

namespace PropSpawn
{
    [CustomItem(ItemType.GunCOM15)]
    public class PropPistol : CustomItem
    {
        private const string GUII = "Gui";

        public static readonly CustomItemConfig cfg = Main.Instance.Config.PropPistolConfig;
        public static readonly HintConfig hcfg = Main.Instance.Config.GuiConfig;
        public override uint Id { get; set; } = cfg.Id;
        public override string Name { get; set; } = cfg.Name;
        public override string Description { get; set; } = cfg.Description;
        public override ItemType Type { get; set; } = cfg.Type;
        public override float Weight { get; set; } = cfg.Weight;

        public override SpawnProperties SpawnProperties { get; set; } = cfg.SpawnProperties;
        protected override void SubscribeEvents()
        {
            Player.ChangingItem += OnChangingItem;
            Player.Shooting += OnShooting;
            Player.ReloadingWeapon += OnReloadingWeapon;
            Player.TogglingNoClip += OnNoClip;
            base.SubscribeEvents();

        }

        protected override void UnsubscribeEvents()
        {
            Player.ChangingItem -= OnChangingItem;
            Player.Shooting -= OnShooting;
            Player.ReloadingWeapon -= OnReloadingWeapon;
            Player.TogglingNoClip -= OnNoClip;
            base.UnsubscribeEvents();
        }

        private void OnChangingItem(ChangingItemEventArgs ev)
        {
            if (ev.Player == null)
                return;
            var customitem = CustomItem.Get(cfg.Id);
            if (Check(ev.Item))
            {
                HSMHelper.Show(
                    ev.Player,
                    GUII,
                    hcfg,
                    p => HudHelper.Build(p, hcfg)
                     
                );
            }
            else
            {
                HSMHelper.Remove(ev.Player, GUII);
                ModeHelpers.Clear(ev.Player);
                PropHelper.Clear(ev.Player);
                HudHelper.ClearAction(ev.Player);
            }

        }
        private void OnShooting(ShootingEventArgs ev )
        {
            if (ev.Player == null || !Check(ev.Item))
                return;

            Mode mode = ModeHelpers.GetMode(ev.Player);

            ev.Firearm.AmmoDrain = 0;
            ev.Firearm.Damage = 0;
            var camera = ev.Player.CameraTransform;
            Vector3 rayOrigin = camera.position;
            Vector3 rayDirection = camera.forward;

            LayerMask mask = ~LayerMask.GetMask("Player", "Hitbox");

            if (!Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, 100f, mask))
                return;

            Vector3 toHit = hit.point - rayOrigin;
            if (Vector3.Dot(camera.forward, toHit.normalized) < 0.5f)
                return;

            float offset = Main.Instance.Config.SpawnOffset;
            float AngularScale = Main.Instance.Config.AngularScale;

            Vector3 normal = hit.normal;
            if (Vector3.Dot(normal, camera.forward) > 0)
                normal = -normal;

            Vector3 hitposition = hit.point + normal * offset ;
            string prop = PropHelper.Get(ev.Player);

            switch (mode)
            {
                case Mode.Spawn:
                    {
                        
                        if (prop == null)
                        {
                            HudHelper.SetAction(
                            ev.Player,
                            $"You haven't selected any props"
                            );
                            break;
                        }
                        HudHelper.SetAction(
                        ev.Player,
                        $"Spawned {prop} {hitposition.ToString("F1")}"
                        );

                        PropHelper.TrySpawn(ev.Player, prop, hitposition);

                        break;
                    }

                case Mode.Delete:
                    HudHelper.SetAction(
                        ev.Player,
                        $"Deleted Prop at {hitposition.ToString("F1")}"
                        );
                    PropHelper.TryDelete(ev.Player);
                    break;

                case Mode.Move:
                    PropHelper.TryMove(ev.Player);
                    break;

                case Mode.Rotate:
                    {
                        RotateMode rotateMode = ModeHelpers.GetRotateMode(ev.Player);
                        Vector3 rotationDelta = rotateMode switch
                        {
                            RotateMode.RotateX => new Vector3(AngularScale, 0f, 0f),
                            RotateMode.RotateY => new Vector3(0f, AngularScale, 0f),
                            RotateMode.RotateZ => new Vector3(0f, 0f, AngularScale),
                            _ => Vector3.zero
                        };

                        PropHelper.TryRotate(ev.Player, rotationDelta);
                        break;
                    }

            }
        }

        private void OnReloadingWeapon(ReloadingWeaponEventArgs ev)
        {
            if (ev.Player == null || !Check(ev.Item) || ModeHelper.GetMode(ev.Player) != Mode.Rotate)
                return;

            ev.IsAllowed = false; 
            
            ModeHelpers.SetNextRotateMode(ev.Player);
            RotateMode rotateMode = (RotateMode)ModeHelpers.GetRotateMode(ev.Player);

            HudHelper.SetAction(ev.Player, $"Rotation axis changed to {rotateMode}");
        }


        private void OnNoClip(TogglingNoClipEventArgs ev)
        {
            if (ev.Player == null || !Check(ev.Player.CurrentItem))
                return;

            ev.IsAllowed = false;

            ModeHelpers.SetNextMode(ev.Player);
            Mode mode = ModeHelpers.GetMode(ev.Player);
            HudHelper.SetAction(
                ev.Player,
                $"Mode changed to {ModeHelpers.GetMode(ev.Player)}"
            );


        }

        private Mode GetNextMode(Mode mode)
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

    }

    public enum Mode
    {
        Spawn,
        Delete,
        Move,
        Rotate,
    }
    public enum RotateMode
    {
        RotateX,
        RotateY,
        RotateZ,
    }
}


