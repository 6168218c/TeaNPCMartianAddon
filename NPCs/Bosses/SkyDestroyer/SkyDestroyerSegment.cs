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
        protected float WarpState { get => NPC.localAI[2]; set => NPC.localAI[2] = value; }
        protected int WarpMark { get => (int)NPC.localAI[3]; set => NPC.localAI[3] = value; }
        public static int warpDistance => 38;
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            if (projectile.maxPenetrate == -1) damage = (int)(damage * 0.6);
            else if (projectile.maxPenetrate != 0) damage = (int)(damage * 1.5 / (projectile.maxPenetrate + 1));
            base.ModifyHitByProjectile(projectile, ref damage, ref knockback, ref crit, ref hitDirection);
        }
    }
}
