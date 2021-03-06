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
using Terraria.DataStructures;

namespace TeaNPCMartianAddon
{
    public class SDEXPlayer : ModPlayer
    {
        private int sdExIndex;
        public bool gentleScreenMove;
        public Vector2 targetScreenPos;
        public Vector2 sourcePos;
        protected float currProgress;
        public override void ModifyScreenPosition()
        {
            //check twice
            if (Util.CheckNPCAlive<SkyDestroyerHead>(sdExIndex))
            {
                float factor = (Main.npc[sdExIndex].ModNPC as SkyDestroyerHead).GetShakescreenIntensity(Player);
                Main.screenPosition += new Vector2(Main.rand.Next(-8, 8) * factor, Main.rand.Next(-6, 6) * factor);

                if (gentleScreenMove)
                {
                    sourcePos += new Vector2(Main.rand.Next(-8, 8) * factor, Main.rand.Next(-6, 6) * factor);
                    Main.screenPosition = Vector2.Lerp(sourcePos, targetScreenPos, (float)Math.Sin(Math.PI / 2 * currProgress));
                }
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
        public override void PreUpdate()
        {
            if (!Util.CheckNPCAlive<SkyDestroyerHead>(sdExIndex))
            {
                sdExIndex = NPC.FindFirstNPC(ModContent.NPCType<SkyDestroyerHead>());
            }
            if (Util.CheckNPCAlive<SkyDestroyerHead>(sdExIndex))
            {
                NPC head = Main.npc[sdExIndex];
                if (head.ai[1] == SkyDestroyerSegment.LightningStormEx)
                {
                    if (Player.mount.Active)
                    {
                        Player.mount.Dismount(Player);
                    }
                    if (Math.Abs(head.localAI[1] - 0) < MathHelper.Pi / 18)
                    {
                        Player.velocity.X = Math.Max(0, Player.velocity.X);
                    }
                    if(Math.Abs(head.localAI[1] - MathHelper.Pi) < MathHelper.Pi / 18)
                    {
                        Player.velocity.X = Math.Min(0, Player.velocity.X);
                    }
                }
            }
            base.PreUpdate();
        }
        public override void PostUpdate()
        {
            gentleScreenMove = false;//reset
        }
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            gentleScreenMove = false;
            base.Kill(damage, hitDirection, pvp, damageSource);
        }
        public SDEXPlayer WithTargetScreenPos(Vector2 pos)
        {
            targetScreenPos = pos;
            return this;
        }
        public SDEXPlayer WithSourceScreenPos(Vector2 pos)
        {
            sourcePos = pos;
            return this;
        }
        public SDEXPlayer UseProgress(float progress)
        {
            currProgress = progress;
            return this;
        }
        public void UpdateScreenMove()
        {
            gentleScreenMove = true;
        }
    }
}
