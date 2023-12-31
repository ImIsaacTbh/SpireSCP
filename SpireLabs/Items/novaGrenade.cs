﻿using CustomItems.API;
using Exiled.API.Features;
using Exiled.API.Features.Attributes;
using Exiled.API.Features.Components;
using Exiled.API.Features.Items;
using Exiled.API.Features.Pickups.Projectiles;
using Exiled.API.Features.Spawn;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using InventorySystem.Items.ThrowableProjectiles;
using MEC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static HarmonyLib.Code;

namespace SpireLabs.Items
{
    [CustomItem(ItemType.GrenadeHE)]
    public class novaGrenade : Exiled.CustomItems.API.Features.CustomGrenade
    {
        public override string Name { get; set; } = "Nova Grenade";
        public override uint Id { get; set; } = 4;
        public override string Description { get; set; } = "\t";
        public override float Weight { get; set; } = 0.01f;
        public override SpawnProperties SpawnProperties { get; set; } = new()
        {
            Limit = 2,
            DynamicSpawnPoints = new List<DynamicSpawnPoint>
            {
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideLocker,
                },
                new()
                {
                    Chance = 50,
                    Location = Exiled.API.Enums.SpawnLocationType.InsideHczArmory,
                }
            },
        };
        public override bool ExplodeOnCollision { get; set; } = false;
        public override float FuseTime { get; set; } = 999f;

        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();
        }
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

        }


        public Color[] colors = {
            Color.red, //this is red you fucking twat
            Color.green,
            Color.blue,
            Color.cyan,
            Color.magenta,
            Color.yellow,
            };

        protected override void OnThrownProjectile(ThrownProjectileEventArgs ev)
        {
            ExplosionGrenadeProjectile g = ev.Projectile as ExplosionGrenadeProjectile;
            g.MaxRadius = 15f;
            g.ScpDamageMultiplier = 2f;
            Timing.RunCoroutine(grenadeLight(ev));
        }


        private IEnumerator<float> grenadeLight(ThrownProjectileEventArgs ev)
        {
            ExplosionGrenadeProjectile g = ev.Projectile as ExplosionGrenadeProjectile;
            yield return Timing.WaitForSeconds(1f);

            var rnd = new System.Random();
            Color color = colors[rnd.Next(0, colors.Count())];
            yield return Timing.WaitForOneFrame;
            var target = ev.Projectile.Position;
            g.Destroy();
            var light = Exiled.API.Features.Toys.Light.Create(target, null, Vector3.one, true, color);
            light.Intensity = 1000000;
            light.Range = 25;
            light.ShadowEmission = true;
            light.Spawn();
            Room room = Room.Get(target);
            room.TurnOffLights(5f);
            yield return Timing.WaitForSeconds(0.5f);
            for (int i = 0; i < 250; i++)
            {
                yield return Timing.WaitForSeconds(0.005f);
                light.Range -= 0.10f;
                light.Intensity -= 4000f;
            }
            ExplosiveGrenade grenade = (ExplosiveGrenade)Item.Create(ItemType.GrenadeHE);
            grenade.FuseTime = 0;
            grenade.ScpDamageMultiplier = 2.25f;
            grenade.MaxRadius = 50;
            light.Destroy();
            grenade.SpawnActive(target, ev.Player);

        }
    }
}
