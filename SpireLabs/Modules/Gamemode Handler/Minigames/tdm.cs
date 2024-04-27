using AudioPlayer.Commands.SubCommands;
using Exiled.Events.EventArgs.Server;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Warhead;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using MapEditorReborn;
using UnityEngine;
using Exiled.API.Enums;
using Exiled.API.Features.Items;
using PluginAPI.Events;
using System.Runtime.Remoting.Messaging;

namespace ObscureLabs.Gamemode_Handler
{


    internal static class tdm
    {

        static string mapName = string.Empty;
        public static string[] maps = { "pvpA1_2t", "pvpA2_2t", "pvpMZA1_2t" };
        static bool gamemodeactive = false;
        static bool team = true;
        static string mapDisplayName = string.Empty;
        public static IEnumerator<float> runJbTDM()
        {
            Vector3 spawnCI = new Vector3(0f, 1000f, 0f);
            Vector3 spawnNTF = new Vector3(0f, 1000f, 0f);

            var rndMap = new System.Random();
            int numMap = rndMap.Next(0, maps.Count() + 1);
            var rnd69 = new System.Random();
            int num69 = 0;
            switch (rndMap.Next(0, 3))
            {
                case 0:     // pvpA1_2t
                    mapName = maps[0];
                    mapDisplayName = "\"Low Effort\"";
                    spawnCI = new Vector3(8.48f, 1106.5f, 30.46f);
                    spawnNTF = new Vector3(-20.8f, 1107.5f, 51.66f);
                    num69 = rnd69.Next(0, 4);
                    break;
                case 1:     // pvpA2_2t
                    mapName = maps[1];
                    mapDisplayName = "\"Tilted Towers\"";
                    spawnCI = new Vector3(9.7f, 1102f, 52.46f);
                    spawnNTF = new Vector3(-20.20f, 1102f, 35.37f);
                    num69 = rnd69.Next(0, 4);
                    break;
                case 2:     // pvpMZA1_2t
                    mapName = maps[2];
                    mapDisplayName = "\"The Maze\"";
                    spawnCI = new Vector3(23.96f, 1126f, 29.14f);
                    spawnNTF = new Vector3(-30.74f, 1126f, -16.93f);
                    num69 = rnd69.Next(0, 3); // This is to prevent balls and grenades on this map due to it being far too big and easy for players to run out of items before anyone actually dies
                    break;
            }

            Log.Warn("Unloading all default maps");
            MapEditorReborn.API.Features.MapUtils.LoadMap("empty");
            Log.Warn("Loading Map: pvpA1_2t");
            MapEditorReborn.API.Features.MapUtils.LoadMap($"{mapName}");
            Log.Warn("Starting checks for players with wrong roles");


            Timing.WaitForSeconds(0.1f);
            List<Player> newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime = new List<Player>();

            newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime = Plugin.PlayerList;

            for (int i = 0; i < (Math.Ceiling((double)Plugin.PlayerList.Count) / 2)+1; i++)
            {

                int playerid = rnd69.Next(0, Plugin.PlayerList.Count());
                Player p = newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime.ElementAt(playerid);
                yield return Timing.WaitForSeconds(0.5f);
                if (p.Role != RoleTypeId.ChaosConscript)
                {
                    p.Role.Set(RoleTypeId.ChaosConscript);
                    p.ClearInventory(true);
                    p.Teleport(spawnCI);
                    p.EnableEffect(EffectType.RainbowTaste, 999, false);
                    p.EnableEffect(EffectType.Ensnared, 10, false);
                    p.EnableEffect(EffectType.Flashed, 10, false);
                    p.EnableEffect(EffectType.SoundtrackMute, 999, false);
                    p.EnableEffect(EffectType.DamageReduction, 10, false);
                    p.ChangeEffectIntensity(EffectType.DamageReduction, 255, 1f);
                    yield return Timing.WaitForOneFrame;
                    p.ChangeAppearance(RoleTypeId.ChaosConscript);

                }
                newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime.Remove(p);
            }
            foreach (Player p in newSuperDuperGoodPlayerListThatKevinLikesFinallyThisTime)
            {
                if (p.Role != RoleTypeId.ChaosConscript)
                {
                    p.Role.Set(RoleTypeId.NtfSergeant);

                    p.ClearInventory(true);
                    p.Teleport(spawnNTF);
                    p.EnableEffect(EffectType.RainbowTaste, 999, false);
                    p.EnableEffect(EffectType.Ensnared, 10, false);
                    p.EnableEffect(EffectType.Flashed, 10, false);
                    p.EnableEffect(EffectType.SoundtrackMute, 999, false);
                    p.EnableEffect(EffectType.DamageReduction, 10, false);
                    p.ChangeEffectIntensity(EffectType.DamageReduction, 255, 1f);
                    yield return Timing.WaitForOneFrame;
                    p.ChangeAppearance(RoleTypeId.NtfSergeant);
                }

            }
            //Log.Info($"Setting player: {p.Nickname} to team: {team}");

            foreach (Player p in Player.List)
            {

                yield return Timing.WaitForSeconds(0.1f);


                p.Broadcast(10, $"<color=green><b>TEAM DEATHMATCH BEGINS IN 10S! \nThe current map is: {mapDisplayName}");
                if (num69 == 0)
                {
                    Log.Warn("Giving Snipers");
                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)1).Give(p);
                    p.AddAmmo(AmmoType.Ammo44Cal, 99);
                }
                if (num69 == 1)
                {
                    Log.Warn("Giving Jailbirds");
                    var item = p.AddItem(ItemType.Jailbird);
                    p.CurrentItem = item;
                }
                if (num69 == 2)
                {
                    Log.Warn("Giving Particle Disrupters");
                    Item item = p.AddItem(ItemType.ParticleDisruptor);
                    p.AddItem(ItemType.ParticleDisruptor);
                    p.CurrentItem = item;
                }
                if (num69 == 3)
                {
                    Log.Warn("Giving ER16's");
                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)5).Give(p);

                }
                if (num69 == 4)
                {
                    Log.Warn("Giving Grenades and Balls");
                    var item = p.AddItem(ItemType.SCP018);
                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)2).Give(p);
                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)2).Give(p);
                    p.AddItem(ItemType.SCP018);
                    p.AddItem(ItemType.GrenadeHE);
                    Exiled.CustomItems.API.Features.CustomItem.Get((uint)4).Give(p);
                    p.AddItem(ItemType.SCP018);
                    p.AddItem(ItemType.GrenadeHE);
                    p.EnableEffect(EffectType.Scp207, 999, false);
                    p.CurrentItem = item;
                }
                p.Scale = new Vector3(1, 1, 1);
                
                yield return Timing.WaitForOneFrame;
            }

            Respawn.TimeUntilNextPhase = 3600f;
        }
        }
    }