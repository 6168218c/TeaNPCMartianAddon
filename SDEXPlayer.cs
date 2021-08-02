using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TeaNPCMartianAddon.Effects;
using TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer;
using TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer;
using System.IO;
using Terraria.Enums;
using System.Collections.Generic;
using System.Linq;

namespace TeaNPCMartianAddon
{
    public class SDEXPlayer:ModPlayer
    {
        private int sdExIndex;
        public override void ModifyScreenPosition()
        {
            if (!Util.CheckNPCAlive<SkyDestroyerHead>(sdExIndex))
            {
                sdExIndex = NPC.FindFirstNPC(ModContent.NPCType<SkyDestroyerHead>());
            }

            //check twice
            if (Util.CheckNPCAlive<SkyDestroyerHead>(sdExIndex))
            {
                float factor = 0;
                if ((Main.npc[sdExIndex].ModNPC as SkyDestroyerHead).IsShakescreenEnabled())
                {
                    float distance = Player.Distance(Main.npc[sdExIndex].Center);
                    if (distance <= 1800)
                    {
                        factor = (1800 - distance) / 1800 * 0.5f;
                    }
                }
                if (Main.projectile.Any(proj => Util.CheckProjAlive<SkyPlasmerizerRay>(proj.whoAmI)))
                {
                    factor = 1f;
                }
                Main.screenPosition += new Vector2(Main.rand.Next(-8, 8) * factor, Main.rand.Next(-6, 6) * factor);
            }
        }
        public override void PreUpdateBuffs()
        {
            if (!Util.CheckNPCAlive<SkyDestroyerHead>(sdExIndex))
            {
                sdExIndex = NPC.FindFirstNPC(ModContent.NPCType<SkyDestroyerHead>());
            }

            if (Util.CheckNPCAlive<SkyDestroyerHead>(sdExIndex))
            {
                if (Player.HasBuff(BuffID.ChaosState))
                {
                    Player.electrified = true;
                }
            }
        }
    }
}
