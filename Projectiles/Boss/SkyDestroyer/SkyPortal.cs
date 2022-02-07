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
	public class SkySentryCenter : ModProjectile
    {
		public static int RotateDistance => 450;
		public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.ShadowBeamHostile;
		public override void SetDefaults()
		{
			Projectile.aiStyle = -1;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;

			Projectile.extraUpdates = 0;
			CooldownSlot = 1;
			Projectile.penetrate = -1;
		}
		public override void AI()
		{
			/*if (!Util.CheckNPCAlive<SkyDestroyerHead>((int)projectile.ai[0]))
			{
				projectile.Kill();
				return;
			}
			NPC head = Main.npc[(int)projectile.ai[0]];
			Player player = Main.player[head.target];*/
			Player player = Main.LocalPlayer;
			if (Projectile.ai[1] == 0 || Projectile.ai[1] == 1)
			{
				if (Projectile.localAI[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile.rotation = Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
					var baseVector = Projectile.rotation.ToRotationVector2() * RotateDistance;
					for (int i = 0; i < 4; i++)
					{
						base.Projectile.NewProjectile(Projectile.Center + baseVector.RotatedBy(MathHelper.PiOver2 * i), Vector2.Zero,
                            ModContent.ProjectileType<SkyPortalSentry>(), Projectile.damage, 0f, Main.myPlayer, SkyPortalSentry.PackAi0(2, i), Projectile.whoAmI);
					}
					Projectile.direction = Main.rand.NextBool() ? -1 : 1;
				}
				Projectile.localAI[0]++;

				if (Projectile.ai[1] == 0)
					Projectile.Center = player.Center;
				else
					Projectile.HoverMovementEx(player.Center, 15f, 0.45f);
				
				Projectile.rotation = Projectile.rotation + 0.015f * MathHelper.SmoothStep(0, 1, Projectile.localAI[0] / 360) * Projectile.direction;
				if (Projectile.localAI[0] >= 360)
				{
					Projectile.Kill();
				}
			}
		}
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(Projectile.localAI[0]);
			writer.Write(Projectile.localAI[1]);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Projectile.localAI[0] = reader.ReadSingle();
			Projectile.localAI[1] = reader.ReadSingle();
		}
	}
	public class SkyPortalSentry : ModProjectile
	{
		int lastai3 = 0;
		int parentMode = 0;
		Vector2 parentCenter;
		int[] extraAI = new int[2];
		public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.VortexVortexPortal;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sentry Portal");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 3;
			ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		}
		public override void SetDefaults()
		{
			Projectile.width = 41;
			Projectile.height = 41;
			Projectile.aiStyle = -1;
			Projectile.hostile = true;
			Projectile.ignoreWater = true;
			Projectile.tileCollide = false;
			Projectile.alpha = 255;

			Projectile.extraUpdates = 0;
			CooldownSlot = 1;
			Projectile.penetrate = -1;

			Projectile.scale = 10f;
		}
		public override void AI()
		{
			float scaleFactor = 4.5f;
			float angularSpeed = (float)Math.PI / 60f;
			Projectile.localAI[0]++;

			var para = UnpackAi0((int)Projectile.ai[0]);

			if (para.mode == 0)
			{
				if (Projectile.localAI[0] == 81f && Main.netMode != NetmodeID.MultiplayerClient)
				{
					int temp = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
					if (temp < 0 || temp == 255)
					{
						return;
					}
					Player player = Main.player[temp];
					Vector2 velo = (player.Center - Projectile.Center).SafeNormalize(Vector2.UnitY);
					velo.RotatedBy(-Math.PI / 6);
					para.count = Math.Max(para.count, 1);
					for (int i = 0; i < para.count; i++)
					{
						if (para.count == 1)
						{
							base.Projectile.NewProjectile(Projectile.Center, velo.RotatedBy(Math.PI / 6)* Projectile.ai[1], ProjectileID.CultistBossLightningOrbArc, Projectile.damage,
								0f, Main.myPlayer, velo.RotatedBy(Math.PI / 6).ToRotation());
						}
						else
						{
							base.Projectile.NewProjectile(Projectile.Center, velo* Projectile.ai[1], ProjectileID.CultistBossLightningOrbArc, Projectile.damage,
								0f, Main.myPlayer, velo.ToRotation());
							velo = velo.RotatedBy(Math.PI / 3 / (para.count - 1));
						}
					}
				}
			}
			else if (para.mode == 1)
            {
				int index = NPC.FindFirstNPC(ModContent.NPCType<SkyDestroyerHead>());
				if (index != -1 && Main.npc[index].ai[1] == SkyDestroyerSegment.LightningStormEx)
				{
                    if (Main.npc[index].ai[3] != lastai3)
                    {
						lastai3 = (int)Main.npc[index].ai[3];
						scaleFactor = 2.5f * (1 + (float)lastai3 / 2);
                        #region Visuals
						for(int i = 0; i < 30; i++)
                        {
							Vector2 vector64 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
							Dust dust18 = Main.dust[Dust.NewDust(Projectile.Center - vector64 * 30f, 0, 0, DustID.Vortex)];
							dust18.noGravity = true;
							dust18.position = Projectile.Center - vector64 * Main.rand.Next(10, 21);
							dust18.velocity = vector64.RotatedBy(1.5707963705062866) * 4f;
							dust18.scale = 0.5f + Main.rand.NextFloat();
							dust18.fadeIn = 0.5f;
						}
                        #endregion
                    }
                    if (Main.npc[index].ai[3] < 4)
					{
						Projectile.localAI[0] = Math.Min(80, Projectile.localAI[0]);
					}
					if (Projectile.localAI[0] == 81f && Main.netMode != NetmodeID.MultiplayerClient)
					{
						float rMin1 = Projectile.velocity.ToRotation() + MathHelper.Pi / 12;
						float rMax1 = Projectile.velocity.ToRotation() + MathHelper.Pi - MathHelper.Pi / 12;
						float rMin2 = Projectile.velocity.ToRotation() + MathHelper.Pi + MathHelper.Pi / 12;
						float rMax2 = Projectile.velocity.ToRotation() + MathHelper.TwoPi - MathHelper.Pi / 12;
						for (int i = 0; i < para.count; i++)
						{
							float rotation = Main.rand.NextBool() ? Main.rand.NextFloat(rMin1, rMax1) : Main.rand.NextFloat(rMin2, rMax2);
							base.Projectile.NewProjectile(Projectile.Center, rotation.ToRotationVector2(), ModContent.ProjectileType<SkyLightningBolt>(),
                                Projectile.damage, 0f, Main.myPlayer, 180, 1);
						}
					}
				}
			}
			else if (para.mode == 2)
            {
                if (Util.CheckProjAlive<SkySentryCenter>((int)Projectile.ai[1]))
                {
					Projectile.localAI[0] = Math.Min(Projectile.localAI[0], 80);
					Projectile parent = Main.projectile[(int)Projectile.ai[1]];

					Projectile.Center = parent.Center +
						parent.rotation.ToRotationVector2().RotatedBy(Math.PI / 2 * para.count) * SkySentryCenter.RotateDistance;
					
					parentMode = (int)parent.ai[1];
					parentCenter = parent.Center;

                    if (parent.ai[1] == 0)
                    {
						extraAI[0]++;
                        if (extraAI[0] % 30 == 0&&Main.netMode!=NetmodeID.MultiplayerClient)
                        {
							base.Projectile.NewProjectile(Projectile.Center, Projectile.DirectionFrom(parentCenter) * 8f, ModContent.ProjectileType<SkyPlasmaAcclerator>() ,
                                Projectile.damage, 0f, Main.myPlayer);
						}
                    }
                }
                else
                {
					if (parentMode == 0)
					{
						extraAI[1]++;
						if (extraAI[1] <= 60)
						{
							Projectile.localAI[0] = Math.Min(Projectile.localAI[0], 80);
							angularSpeed *= MathHelper.Clamp(1 - (extraAI[1] - 10) / 50, 0, 1);
							if (extraAI[1] == 60 && Main.netMode != NetmodeID.MultiplayerClient)
							{
								for (int i = 0; i < 4; i++)
								{
									Vector2 unit = Projectile.rotation.ToRotationVector2().RotatedBy(Math.PI / 2 * i);
									base.Projectile.NewProjectile(Projectile.Center, unit * 18, ProjectileID.CultistBossLightningOrbArc,
                                        Projectile.damage, 0f, Main.myPlayer, unit.ToRotation());
								}
							}
						}
						else
						{
							Projectile.localAI[0] = Math.Max(Projectile.localAI[0], 81);
						}
					}
					else if (parentMode == 1)
                    {
						Projectile.HoverMovement(parentCenter, 12f, 0.2f);
                        if (Projectile.DistanceSQ(parentCenter) <= 30 * 30)
                        {

                        }
                    }
                }
            }

			#region Visuals
			if (Projectile.localAI[0] <= 10f)
			{
				if (Main.rand.Next(4) == 0)
				{
					Vector2 vector64 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
					Dust dust18 = Main.dust[Dust.NewDust(Projectile.Center - vector64 * 30f, 0, 0, DustID.Vortex)];
					dust18.noGravity = true;
					dust18.position = Projectile.Center - vector64 * Main.rand.Next(10, 21);
					dust18.velocity = vector64.RotatedBy(1.5707963705062866) * 4f;
					dust18.scale = 0.5f + Main.rand.NextFloat();
					dust18.fadeIn = 0.5f;
				}

				if (Main.rand.Next(4) == 0)
				{
					Vector2 vector65 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
					Dust dust19 = Main.dust[Dust.NewDust(Projectile.Center - vector65 * 30f, 0, 0, DustID.Granite)];
					dust19.noGravity = true;
					dust19.position = Projectile.Center - vector65 * 30f;
					dust19.velocity = vector65.RotatedBy(-1.5707963705062866) * 2f;
					dust19.scale = 0.5f + Main.rand.NextFloat();
					dust19.fadeIn = 0.5f;
				}
			}
			else if (Projectile.localAI[0] <= 50f)
			{
				Projectile.scale = (Projectile.localAI[0] - 10f) / 40f;
				Projectile.alpha = 255 - (int)(255f * Projectile.scale);
				Projectile.rotation -= angularSpeed;
				Projectile.scale *= scaleFactor;

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
			else if (Projectile.localAI[0] <= 80f + Projectile.ai[1])
			{
				Projectile.scale = 1f;
				Projectile.alpha = 0;
				Projectile.rotation -= angularSpeed;
				Projectile.scale *= scaleFactor;

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
				Projectile.scale = 1f - (Projectile.localAI[0] - 80f - Projectile.ai[1]) / 60f;
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
        public static (int mode,int count) UnpackAi0(int ai0)
        {
			return (ai0 % 10, ai0 / 10);
        }
		public static int PackAi0(int mode, int count) => PackAi0((mode, count));
		public static int PackAi0((int mode, int count) tuple)
        {
			return tuple.mode + tuple.count * 10;
        }
        public override bool ShouldUpdatePosition()
        {
			var para = UnpackAi0((int)Projectile.ai[0]);
			if (para.mode == 1) return false;
			return true;
        }
        public override bool? CanDamage() => false;
        public override bool PreDraw(ref Color lightColor)
		{
			SpriteBatch spriteBatch=Main.spriteBatch;
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			Color glow2 = new Color(Main.DiscoR + 50, Main.DiscoG + 50, Main.DiscoB + 50) * Projectile.Opacity;

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
			for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
			{
				Vector2 value4 = Projectile.oldPos[i];
				float num165 = Projectile.oldRot[i];
				spriteBatch.Draw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, num165, origin2, Projectile.scale, SpriteEffects.None, 0f);
			}
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
			spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * Projectile.Opacity, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
			spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);

			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            if (UnpackAi0((int)Projectile.ai[0]).mode == 0)
            {
				int temp = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
				if (!(temp < 0 || temp == 255))
				{
					float timer = Projectile.localAI[0] - 30;
					if (timer >= 0 && timer <= 45)
					{
						Color alpha = Color.Turquoise;
						if (timer <= 10) alpha *= timer / 10;
						else if (timer >= 30) alpha *= (45 - timer) / 25;
						Projectile.DrawAim(spriteBatch, Projectile.Center+Projectile.DirectionTo(Main.player[temp].Center)*3600, alpha);
					}
				}
			}
			else if (UnpackAi0((int)Projectile.ai[0]).mode == 2)
			{
                if (parentMode == 0)
                {
					float timer = extraAI[1] - 12;
					if (timer >= 0 && timer <= 45)
					{
						Color alpha = Color.Turquoise;
						if (timer <= 10) alpha *= timer / 10;
						else if (timer >= 30) alpha *= (45 - timer) / 25;
						for (int i = 0; i < 4; i++)
						{
							Vector2 unit = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.Pi / 2 * i);
							Projectile.DrawAim(spriteBatch, Projectile.Center + unit * 1800, alpha);
						}
					}
				}
			}

			return false;
		}
	}
}
