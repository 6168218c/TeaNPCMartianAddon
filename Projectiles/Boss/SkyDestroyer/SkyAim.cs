using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TeaNPCMartianAddon.Effects;
using TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer;
using System.IO;
using Terraria.Enums;
using System.Collections.Generic;

namespace TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer
{
    public class SkyAim:ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

            Projectile.extraUpdates = 0;
            CooldownSlot = 1;
            Projectile.penetrate = -1;
            Projectile.scale = 0.2f;
        }
        public override void AI()
        {
            if (Projectile.localAI[1] > 0)
            {
                Projectile.alpha += 15;
                if (Projectile.alpha >= 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
                return;
            }
            Projectile.Loomup();
            if (Projectile.ai[0] != -1)
            {
                if (!Util.CheckNPCAlive<SkyDestroyerHead>((int)Projectile.ai[0]))
                {
                    Projectile.localAI[1] = 1;
                    return;
                }
                NPC head = Main.npc[(int)Projectile.ai[0]];
                Player player = Main.player[head.target];
                if (Projectile.localAI[0] < Projectile.ai[1] - 45)
                {
                    if (head.ai[1] == SkyDestroyerSegment.Plasmerizer)
                        Projectile.Center = player.Center + player.velocity * 60f;
                    else Projectile.Center = player.Center;
                }
                Projectile.rotation += 0.075f;
                Projectile.velocity = Projectile.rotation.ToRotationVector2() * 450 *
                    (1 - MathHelper.SmoothStep(0, 1, MathHelper.Clamp((Projectile.localAI[0] - Projectile.ai[1] + 45) / 25, 0, 1)));
                Projectile.localAI[0]++;
            }
            else
            {
                Projectile.localAI[0]++;
                if (Projectile.localAI[0] <= Projectile.ai[1] / 2)
                {
                    Projectile.rotation = MathHelper.SmoothStep(-MathHelper.Pi, 0, Projectile.localAI[0] / (Projectile.ai[1] / 2));
                    Projectile.scale = MathHelper.SmoothStep(1f, 0.2f, Projectile.localAI[0] / (Projectile.ai[1] / 2));
                }
                else
                {
                    Projectile.rotation = 0;
                    Projectile.scale = 0.2f;
                }
            }

            if (Projectile.localAI[0] >= Projectile.ai[1])
            {
                Projectile.localAI[1] = 1;
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            float rotation = Projectile.rotation;
            if (Projectile.ai[0] != -1)
            {
                rotation = 0;
            }
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.Turquoise), rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);
            if (Projectile.ai[0] != -1 && Projectile.localAI[0] > Projectile.ai[1] - 45)
            {
                for (int i = 0; i < 4; i++)
                {
                    Vector2 offset = Projectile.velocity.RotatedBy(Math.PI / 2 * i);
                    Main.spriteBatch.Draw(texture2D13, Projectile.Center + offset - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.Turquoise), offset.ToRotation(), origin2, Projectile.scale, SpriteEffects.None, 0f);
                }
            }
            return false;
        }
    }
}
