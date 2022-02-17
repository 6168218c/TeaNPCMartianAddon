using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.Localization;
using TeaNPCMartianAddon.Items.Bosses.Martians;
using TeaNPCMartianAddon.Projectiles.Boss;

namespace TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer
{
    public abstract class SkyDestroyerSegment:ModNPC
    {
        protected static float BackgroundScale => 0.2f;
        public static int LightningStorm => 0;
        public static int FireballBarrage => 1;
        public static int Plasmerizer => 2;
        public static int WarpMove => 3;
        public static int PlasmaWarpBlast => 4;
        public static int SpaceWarp => 5;
        public static int AntimatterBomb => 6;
        public static int LightningStormEx => 7;
        public static int WormBarrage => 8;
        public static int HandleWarpIssues => 18;
        public static int ResetStates => 19;
        public static int DeathAnimation0 => 20;
        public static int DeathAnimation1 => 21;
        protected int baseMaxLife = 180000;
        protected int baseDamage = 105;
        protected float WarpState { get => NPC.localAI[2]; set { NPC.localAI[2] = value;NPC.netUpdate = true; } }
        protected int WarpMark { get => (int)NPC.localAI[3]; set { NPC.localAI[3] = value; } }
        public static int warpDistance => 78;
        public static Vector2 GetLinkPoint(NPC seg)
        {
            return seg.Center - seg.rotation.ToRotationVector2() * seg.height / 2 * seg.scale;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            if (NPC.alpha > 0 || NPC.hide) return false;
            return base.CanHitPlayer(target, ref cooldownSlot);
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            base.NPC.lifeMax = (int)((float)base.NPC.lifeMax * 0.8f * bossLifeScale);
            base.NPC.damage = (int)((float)base.NPC.damage * 0.625f);
            if (Main.masterMode)
            {
                base.NPC.damage = NPC.damage * 3 / 4;
                base.NPC.life /= 3;
            }
        }
        public override void DrawBehind(int index)
        {
            if (NPC.hide)
            {
                Main.instance.DrawCacheNPCsMoonMoon.Add(index);
            }
        }
        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.hide)
            {
                return Color.Lerp(Main.ColorOfTheSkies, drawColor, 0.2f);
            }
            return null;
        }
        public Color GetGlowColor()
        {
            NPC head = null;
            if (Util.CheckNPCAlive<SkyDestroyerHead>(NPC.realLife))
            {
                head=Main.npc[NPC.realLife];
            }
            else
            {
                head = NPC;
            }
            if(head.ai[1]>=0&&(head.ModNPC as SkyDestroyerHead)?.CurrentModule?.ID > 3)
            {
                return Color.Lerp(Color.Red, Color.White, 0.5f);
            }
            return NPC.GetAlpha(Color.White);
        }
        protected bool viberation;
        protected void SetViberation(bool value=true) => viberation = value;
        public Vector2 GetDrawPosition(Vector2 screenPos)
        {
            if (NPC.hide)
            {
                NPC head = NPC;
                if (NPC.realLife != -1) head = Main.npc[NPC.realLife];
                if (head.ai[1] == -2)
                {
                    Vector2 value2 = screenPos + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
                    float depth = MathHelper.SmoothStep(1.6f, 1f, head.ai[2] / 12 / 60);
                    Vector2 value3 = new Vector2(1f / depth, 0.9f / depth);
                    Vector2 position = NPC.Center + new Vector2(0f, NPC.gfxOffY);
                    position = (position - value2) * value3 + value2 - screenPos;
                    return position;
                }
                else
                {
                    return NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY);
                }
            }
            else
            {
                Vector2 offset = Vector2.Zero;
                if (viberation) offset = Main.rand.NextVector2CircularEdge(20, 20) * (1 - NPC.ai[3] / 600);
                return NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY) + offset;
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (NPC.alpha > 0) damage = 0;
            if (projectile.maxPenetrate == -1) damage = (int)(damage * 0.6);
            else if (projectile.maxPenetrate != 0) damage = (int)(damage * 1.5 / (projectile.maxPenetrate + 1));
            base.ModifyHitByProjectile(projectile, ref damage, ref knockback, ref crit, ref hitDirection);
        }
        protected void SetBackground()
        {
            NPC.hide = true;
            NPC.scale = BackgroundScale;
        }
        protected void SetForeground()
        {
            NPC.hide = false;
            NPC.scale = 1f;
        }
        protected void BodyPreventDeath()
        {
            if (NPC.ai[3] < 0)
            {
                return;
            }
            if (Util.CheckNPCAlive<SkyDestroyerHead>(NPC.realLife))
            {
                NPC head = Main.npc[NPC.realLife];
                if (head.ai[1] < DeathAnimation1)
                {
                    NPC.life = 1;
                    NPC.dontTakeDamage = true;
                    return;
                }
            }
        }
    }
}
