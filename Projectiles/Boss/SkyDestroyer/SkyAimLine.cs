using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using TeaNPCMartianAddon.Effects;
using TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer;
using System.IO;
using Terraria.Enums;
using System.Collections.Generic;

namespace TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer
{
    public class SkyAimLine:ModProjectile
    {
        protected Vector2 target;
        public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Aim Line");
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = 10;
            Projectile.height = 10;
            Projectile.friendly = false;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            if (Projectile.ai[1] == 0)
            {
                if (Projectile.localAI[0] == 0)
                {
                    NPC head = Main.npc[(int)Projectile.ai[0]];
                    Player player = Main.player[head.target];
                    target = Projectile.Center + (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 3600;
                }
            }
            else if (Projectile.ai[1] == 1)
            {
                NPC head = Main.npc[(int)Projectile.ai[0]];
                Player player = Main.player[head.target];
                target = Projectile.Center + (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 3600;
            }
            else if (Projectile.ai[1] == 2)
            {
                if (Util.CheckProjAlive((int)Projectile.ai[0]))
                {
                    Projectile mark = Main.projectile[(int)Projectile.ai[0]];
                    target = mark.Center;
                }
            }
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 45)
            {
                Projectile.Kill();
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.ai[1] == 0 || Projectile.ai[1] == 2)
            {
                return Color.Turquoise;
            }
            return null;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            float timer = Projectile.localAI[0];
            if (timer >= 0 && timer <= 45)
            {
                Color alpha = Projectile.GetAlpha(lightColor);
                if (timer <= 10) alpha *= timer / 10;
                else if (timer >= 30) alpha *= (45 - timer) / 25;
                Utils.DrawLine(Main.spriteBatch, Projectile.Center, target, alpha, alpha * 0.8f, 3);
            }
            return false;
        }
    }
    public class SkyAimLineAlt : SkyAimLine
    {
        public override void AI()
        {
            Projectile.Loomup();
            target = Projectile.ProjAIToVector();
            Projectile.localAI[0]++;
            if (Projectile.localAI[0] > 45)
            {
                Projectile.Kill();
            }
        }
    }
}
