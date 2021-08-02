using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer;
using System.IO;

namespace TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer
{
    public class SkyWarpMark:ModProjectile
    {
		bool firstMark;
		bool despawning = false;
		float[] extraAI = new float[2];
        public override string Texture => "Terraria/Projectile_" + ProjectileID.VortexVortexPortal;
		public Vector2 Start => Projectile.Center;
		public Vector2 End => Projectile.ProjAIToVector();
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

            Projectile.extraUpdates = 0;
            CooldownSlot = 1;
            Projectile.penetrate = -1;
			Projectile.scale = 8f;
        }
        public override void AI()
        {
			if (Projectile.localAI[1] < 0) firstMark = true;
			if (Projectile.localAI[1] >= 2 || (firstMark && Projectile.localAI[1] == 1))
			{
				despawning = true;
			}
			if (!NPC.AnyNPCs(ModContent.NPCType<SkyDestroyerHead>()))
			{
				despawning = true;
			}
			#region Portal Visuals
			extraAI[0]++;
			if(despawning)
			{
				extraAI[0] = Math.Max(extraAI[0], 71);
			}
			if (extraAI[0] <= 40f)
			{
				Projectile.scale = extraAI[0] / 40f;
				Projectile.alpha = 255 - (int)(255f * Projectile.scale);
				Projectile.rotation -= (float)Math.PI / 20f;
				Projectile.scale *= 6f;

				if (Main.rand.Next(2) == 0)
				{
					Vector2 vector70 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
					Dust dust23 = Main.dust[Dust.NewDust(Projectile.Center - vector70 * 30f, 0, 0, DustID.Vortex)];
					dust23.noGravity = true;
					dust23.position = Projectile.Center - vector70 * Main.rand.Next(10, 21);
					dust23.velocity = vector70.RotatedBy(1.5707963705062866) * 6f;
					dust23.scale = 0.5f + Main.rand.NextFloat();
					dust23.fadeIn = 0.5f;
					dust23.customData = Projectile.Center;
				}

				if (Main.rand.Next(2) == 0)
				{
					Vector2 vector71 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
					Dust dust24 = Main.dust[Dust.NewDust(Projectile.Center - vector71 * 30f, 0, 0, DustID.Granite)];
					dust24.noGravity = true;
					dust24.position = Projectile.Center - vector71 * 30f;
					dust24.velocity = vector71.RotatedBy(-1.5707963705062866) * 3f;
					dust24.scale = 0.5f + Main.rand.NextFloat();
					dust24.fadeIn = 0.5f;
					dust24.customData = Projectile.Center;
				}
			}
			else if (extraAI[0] <= 70f)
			{
				if (!despawning)
					extraAI[0]--;
				else
					extraAI[0] = Math.Max(extraAI[0], 70);
				Projectile.scale = 1f;
				Projectile.alpha = 0;
				Projectile.rotation -= (float)Math.PI / 60f;
				Projectile.scale *= 6f;
				if (Main.rand.Next(2) == 0)
				{
					Vector2 vector78 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
					Dust dust30 = Main.dust[Dust.NewDust(Projectile.Center - vector78 * 30f, 0, 0, DustID.Vortex)];
					dust30.noGravity = true;
					dust30.position = Projectile.Center - vector78 * Main.rand.Next(10, 21);
					dust30.velocity = vector78.RotatedBy(1.5707963705062866) * 6f;
					dust30.scale = 0.5f + Main.rand.NextFloat();
					dust30.fadeIn = 0.5f;
					dust30.customData = Projectile.Center;
				}
				else
				{
					Vector2 vector79 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
					Dust dust31 = Main.dust[Dust.NewDust(Projectile.Center - vector79 * 30f, 0, 0, DustID.Granite)];
					dust31.noGravity = true;
					dust31.position = Projectile.Center - vector79 * 30f;
					dust31.velocity = vector79.RotatedBy(-1.5707963705062866) * 3f;
					dust31.scale = 0.5f + Main.rand.NextFloat();
					dust31.fadeIn = 0.5f;
					dust31.customData = Projectile.Center;
				}
			}
			else
			{
				Projectile.scale = 1f - (extraAI[0] - 70f) / 60f;
				Projectile.alpha = 255 - (int)(255f * Projectile.scale);
				Projectile.rotation -= (float)Math.PI / 30f;
				Projectile.scale *= 6f;
				if (Projectile.alpha >= 255)
					Projectile.Kill();

				for (int num840 = 0; num840 < 2; num840++)
				{
					switch (Main.rand.Next(3))
					{
						case 0:
							{
								Vector2 vector83 = Vector2.UnitY.RotatedByRandom(6.2831854820251465) * Projectile.scale;
								Dust dust35 = Main.dust[Dust.NewDust(Projectile.Center - vector83 * 30f, 0, 0, DustID.Vortex)];
								dust35.noGravity = true;
								dust35.position = Projectile.Center - vector83 * Main.rand.Next(10, 21);
								dust35.velocity = vector83.RotatedBy(1.5707963705062866) * 6f;
								dust35.scale = 0.5f + Main.rand.NextFloat();
								dust35.fadeIn = 0.5f;
								dust35.customData = Projectile.Center;
								break;
							}
						case 1:
							{
								Vector2 vector82 = Vector2.UnitY.RotatedByRandom(6.2831854820251465) * Projectile.scale;
								Dust dust34 = Main.dust[Dust.NewDust(Projectile.Center - vector82 * 30f, 0, 0, DustID.Granite)];
								dust34.noGravity = true;
								dust34.position = Projectile.Center - vector82 * 30f;
								dust34.velocity = vector82.RotatedBy(-1.5707963705062866) * 3f;
								dust34.scale = 0.5f + Main.rand.NextFloat();
								dust34.fadeIn = 0.5f;
								dust34.customData = Projectile.Center;
								break;
							}
					}
				}
			}
			#endregion
		}
        public override bool PreDraw(ref Color lightColor)
        {
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			Main.EntitySpriteDraw(texture2D13, Start - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * Projectile.Opacity, -Projectile.rotation, origin2, Projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0);
			Main.EntitySpriteDraw(texture2D13, Start - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.White), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);
			Main.EntitySpriteDraw(texture2D13, End - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * Projectile.Opacity, -Projectile.rotation, origin2, Projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0);
			Main.EntitySpriteDraw(texture2D13, End - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.White), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

			return false;
		}
    }
}
