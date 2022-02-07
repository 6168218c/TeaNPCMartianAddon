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
        public static int LightningStorm => 0;
        public static int FireballBarrage => 1;
        public static int Plasmerizer => 2;
        public static int WarpMove => 3;
        public static int PlasmaWarpBlast => 4;
        public static int SpaceWarp => 5;
        public static int AntimatterBomb => 6;
        public static int AntimatterBomb2 => 7;
        protected float WarpState { get => base.NPC.localAI[2]; set => base.NPC.localAI[2] = value; }
        protected int WarpMark { get => (int)base.NPC.localAI[3]; set => base.NPC.localAI[3] = value; }
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
        public override void DrawBehind(int index)
        {
            if (NPC.hide)
            {
                Main.instance.DrawCacheNPCsMoonMoon.Add(index);
            }
        }
        public static float BackgroundScale => 0.2f;
        public override Color? GetAlpha(Color drawColor)
        {
            if (NPC.hide)
            {
                return Color.Lerp(Main.ColorOfTheSkies, drawColor, 0.2f);
            }
            return null;
        }
        public Vector2 GetDrawPosition()
        {
            if (NPC.hide)
            {
                Vector2 value2 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
                float depth = 1f;
                Vector2 value3 = new Vector2(1f / depth, 0.9f / depth);
                Vector2 position = NPC.Center + new Vector2(0f, NPC.gfxOffY);
                position = (position - value2) * value3 + value2 - Main.screenPosition;
                return position;
            }
            else
            {
                return NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);
            }
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.maxPenetrate == -1) damage = (int)(damage * 0.6);
            else if (projectile.maxPenetrate != 0) damage = (int)(damage * 1.5 / (projectile.maxPenetrate + 1));
            base.ModifyHitByProjectile(projectile, ref damage, ref knockback, ref crit, ref hitDirection);
        }
        protected void SetBackground()
        {
            NPC.hide = true;
            NPC.scale = 0.1f;
            NPC.dontTakeDamage = true;
        }
        protected void SetForeground()
        {
            NPC.hide = false;
            NPC.scale = 1f;
            NPC.dontTakeDamage = false;
        }
    }
}
