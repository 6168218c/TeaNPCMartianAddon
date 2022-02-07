using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer;
using System.IO;
using TeaNPCMartianAddon.Effects;
using System.Collections.Generic;

namespace TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer
{
	internal static class WarpMarkDrawer
    {
		private static bool hasBegun = false;
		internal static void BeginDraw(SpriteBatch spriteBatch,BlendState blendState)
        {
			hasBegun = true;
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
		internal static void EndDraw(SpriteBatch spriteBatch)
        {
			hasBegun = false;
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		}
		internal static void Draw(this Projectile projectile, SpriteBatch spriteBatch, Vector2 position, Color color,float opacity,float rotation,float scale)
        {
			if (!hasBegun) throw new InvalidOperationException($"{nameof(WarpMarkDrawer.BeginDraw)} has not been called");
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[projectile.type].Value.Height / Main.projFrames[projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;

			TeaNPCMartianAddon.PortalEffect.Parameters["opacity"].SetValue(opacity);
			Main.graphics.GraphicsDevice.Textures[0] = texture2D13;
			Main.graphics.GraphicsDevice.Textures[1] = TeaNPCMartianAddon.Instance.RequestTexture("Images/Stars");
			Main.graphics.GraphicsDevice.Textures[2] = TeaNPCMartianAddon.Instance.RequestTexture("Images/Extra_49");
			TeaNPCMartianAddon.PortalEffect.CurrentTechnique.Passes[0].Apply();

			spriteBatch.Draw(texture2D13, position - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color, rotation, origin2, scale, SpriteEffects.None, 0);
		}
    }
    public class SkyWarpMark:ModProjectile
    {
		protected bool firstMark;
		protected bool despawning = false;
		protected float[] extraAI = new float[2];
        public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.VortexVortexPortal;
		public virtual Vector2 Start => Projectile.Center;
		public virtual Vector2 End => Projectile.ProjAIToVector();
        public override void SetDefaults()
        {
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
			Projectile.width = Projectile.height = 40;

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
				Projectile.scale *= 4.5f;

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
				Projectile.scale *= 4.5f;
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
				Projectile.scale *= 4.5f;
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
			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			/*Main.spriteBatch.Draw(texture2D13, Start - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * projectile.Opacity, -projectile.rotation, origin2, projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, Start - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(Color.White), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(texture2D13, End - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * projectile.Opacity, -projectile.rotation, origin2, projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, End - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(Color.White), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);*/
			WarpMarkDrawer.BeginDraw(spriteBatch, BlendState.Additive);
			Projectile.Draw(spriteBatch,Start, Projectile.GetAlpha(Color.Turquoise),Projectile.Opacity, Projectile.rotation,Projectile.scale);
			Projectile.Draw(spriteBatch,End, Projectile.GetAlpha(Color.Turquoise), Projectile.Opacity, Projectile.rotation, Projectile.scale);
			WarpMarkDrawer.EndDraw(spriteBatch);

			WarpMarkDrawer.BeginDraw(spriteBatch, BlendState.AlphaBlend);
			Main.spriteBatch.Draw(texture2D13, Start - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.White), -Projectile.rotation, origin2, Projectile.scale * 0.8f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, End - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.White), -Projectile.rotation, origin2, Projectile.scale * 0.8f, SpriteEffects.FlipHorizontally, 0f);
			WarpMarkDrawer.EndDraw(spriteBatch);

			return false;
		}
        public override void SendExtraAI(BinaryWriter writer)
        {
			writer.Write(Projectile.localAI[0]);
			writer.Write(Projectile.localAI[1]);
			base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
			Projectile.localAI[0] = reader.ReadSingle();
			Projectile.localAI[1] = reader.ReadSingle();
            base.ReceiveExtraAI(reader);
        }
    }
	public class SkyShowupWarpMark : SkyWarpMark
    {
		public override bool PreDraw(ref Color lightColor)
		{
			SpriteBatch spriteBatch = Main.spriteBatch;
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			Vector2 value2 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
			float depth = 1f;
			Vector2 value3 = new Vector2(1f / depth, 0.9f / depth);
			Vector2 position = Start + new Vector2(0f, Projectile.gfxOffY);
			position = (position - value2) * value3 + value2 - Main.screenPosition;
			/*Main.spriteBatch.Draw(texture2D13, Start - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * projectile.Opacity, -projectile.rotation, origin2, projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, Start - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(Color.White), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(texture2D13, End - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * projectile.Opacity, -projectile.rotation, origin2, projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, End - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(Color.White), projectile.rotation, origin2, projectile.scale, SpriteEffects.None, 0f);*/
			WarpMarkDrawer.BeginDraw(spriteBatch, BlendState.Additive);
			Projectile.Draw(spriteBatch, position, Projectile.GetAlpha(Color.Lerp(Main.ColorOfTheSkies, Color.Turquoise, 0.1f)), Projectile.Opacity, Projectile.rotation, Projectile.scale);
			Projectile.Draw(spriteBatch, End, Projectile.GetAlpha(Color.Turquoise), Projectile.Opacity, Projectile.rotation, Projectile.scale);
			WarpMarkDrawer.EndDraw(spriteBatch);

			WarpMarkDrawer.BeginDraw(spriteBatch, BlendState.AlphaBlend);
			Main.spriteBatch.Draw(texture2D13, position, new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.Lerp(Main.ColorOfTheSkies, Color.White, 0.1f)), -Projectile.rotation, origin2, Projectile.scale * 0.2f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, End - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.White), -Projectile.rotation, origin2, Projectile.scale * 0.8f, SpriteEffects.None, 0f);
			WarpMarkDrawer.EndDraw(spriteBatch);

			return false;
		}
	}
	public class SkyFadebackWarpMark : SkyWarpMark
	{
		public override bool PreDraw(ref Color lightColor)
		{
			SpriteBatch spriteBatch=Main.spriteBatch;
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			Vector2 value2 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
			float depth = 1f;
			Vector2 value3 = new Vector2(1f / depth, 0.9f / depth);
			Vector2 position = Start + new Vector2(0f, Projectile.gfxOffY);
			position = (position - value2) * value3 + value2 - Main.screenPosition;
			Main.spriteBatch.Draw(texture2D13, End - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Lerp(Main.ColorOfTheSkies, Color.Black, 0.1f) * Projectile.Opacity, -Projectile.rotation, origin2, Projectile.scale * 1.25f * 0.2f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, End - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.Lerp(Main.ColorOfTheSkies, Color.White, 0.1f)), Projectile.rotation, origin2, Projectile.scale * 0.2f, SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(texture2D13, position, new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * Projectile.Opacity, -Projectile.rotation, origin2, Projectile.scale * 1.25f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, position, new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.White), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);

			return false;
		}
	}
	public class SkyWarpMarkEx : ModProjectile
    {		float[] scale = new float[2];
		float[] alpha = new float[2];
		float[] rotation = new float[2];
		
		protected bool firstMark;
		protected bool despawning = false;
		protected float[] extraAI = new float[2];
		public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.VortexVortexPortal;
		public Vector2 Start => Projectile.Center;
		public Vector2 End => Projectile.velocity;
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
		public override bool ShouldUpdatePosition()
		{
			return false;
		}
        public override void AI()
        {
			bool[] despawn = new bool[2];
			if (Projectile.ai[1] == 0|| Projectile.ai[1] == 1)
            {
				int flag = (int)Projectile.ai[0];
				despawn[0] = flag / 10 == 0;
				despawn[1] = flag % 10 == 0;
            }
			if (Projectile.localAI[1] < 0) firstMark = true;
			if (Projectile.localAI[1] >= 2 || (firstMark && Projectile.localAI[1] == 1))
			{
				despawning = true;
			}
			if (!NPC.AnyNPCs(ModContent.NPCType<SkyDestroyerHead>()))
			{
				despawning = true;
			}
			despawn[0] |= despawning;
			despawn[1] |= despawning;
			#region Portal Visuals
			void VisualAI(int index,Vector2 Center,bool fading)
            {
				extraAI[index]++;
				if (fading)
				{
					if (extraAI[index] < 20) extraAI[index] = 0;
					else extraAI[index] = Math.Max(extraAI[index], 71);
				}
				if (extraAI[index] <= 40f)
				{
					scale[index] = extraAI[index] / 40f;
					alpha[index] = 255 - (int)(255f * scale[index]);
					rotation[index] -= (float)Math.PI / 20f;
					scale[index] *= 4.5f;

					if (Main.rand.Next(2) == 0)
					{
						Vector2 vector70 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
						Dust dust23 = Main.dust[Dust.NewDust(Center - vector70 * 30f, 0, 0, DustID.Vortex)];
						dust23.noGravity = true;
						dust23.position = Center - vector70 * Main.rand.Next(10, 21);
						dust23.velocity = vector70.RotatedBy(1.5707963705062866) * 6f;
						dust23.scale = 0.5f + Main.rand.NextFloat();
						dust23.fadeIn = 0.5f;
						dust23.customData = Center;
					}

					if (Main.rand.Next(2) == 0)
					{
						Vector2 vector71 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
						Dust dust24 = Main.dust[Dust.NewDust(Center - vector71 * 30f, 0, 0, DustID.Granite)];
						dust24.noGravity = true;
						dust24.position = Center - vector71 * 30f;
						dust24.velocity = vector71.RotatedBy(-1.5707963705062866) * 3f;
						dust24.scale = 0.5f + Main.rand.NextFloat();
						dust24.fadeIn = 0.5f;
						dust24.customData = Center;
					}
				}
				else if (extraAI[index] <= 70f)
				{
					if (!fading && extraAI[index] == 70f)
						extraAI[index]--;
					else if(fading)
						extraAI[index] = Math.Max(extraAI[index], 70);
					scale[index] = 1f;
					alpha[index] = 0;
					rotation[index] -= (float)Math.PI / 60f;
					scale[index] *= 4.5f;
					if (Main.rand.Next(2) == 0)
					{
						Vector2 vector78 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
						Dust dust30 = Main.dust[Dust.NewDust(Center - vector78 * 30f, 0, 0, DustID.Vortex)];
						dust30.noGravity = true;
						dust30.position = Center - vector78 * Main.rand.Next(10, 21);
						dust30.velocity = vector78.RotatedBy(1.5707963705062866) * 6f;
						dust30.scale = 0.5f + Main.rand.NextFloat();
						dust30.fadeIn = 0.5f;
						dust30.customData = Center;
					}
					else
					{
						Vector2 vector79 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
						Dust dust31 = Main.dust[Dust.NewDust(Center - vector79 * 30f, 0, 0, DustID.Granite)];
						dust31.noGravity = true;
						dust31.position = Center - vector79 * 30f;
						dust31.velocity = vector79.RotatedBy(-1.5707963705062866) * 3f;
						dust31.scale = 0.5f + Main.rand.NextFloat();
						dust31.fadeIn = 0.5f;
						dust31.customData = Center;
					}
				}
				else
				{
					scale[index] = 1f - (extraAI[index] - 70f) / 60f;
					alpha[index] = 255 - (int)(255f * scale[index]);
					rotation[index] -= (float)Math.PI / 30f;
					scale[index] *= 4.5f;

					for (int num840 = 0; num840 < 2; num840++)
					{
						switch (Main.rand.Next(3))
						{
							case 0:
								{
									Vector2 vector83 = Vector2.UnitY.RotatedByRandom(6.2831854820251465) * scale[index];
									Dust dust35 = Main.dust[Dust.NewDust(Center - vector83 * 30f, 0, 0, DustID.Vortex)];
									dust35.noGravity = true;
									dust35.position = Center - vector83 * Main.rand.Next(10, 21);
									dust35.velocity = vector83.RotatedBy(1.5707963705062866) * 6f;
									dust35.scale = 0.5f + Main.rand.NextFloat();
									dust35.fadeIn = 0.5f;
									dust35.customData = Center;
									break;
								}
							case 1:
								{
									Vector2 vector82 = Vector2.UnitY.RotatedByRandom(6.2831854820251465) * scale[index];
									Dust dust34 = Main.dust[Dust.NewDust(Center - vector82 * 30f, 0, 0, DustID.Granite)];
									dust34.noGravity = true;
									dust34.position = Center - vector82 * 30f;
									dust34.velocity = vector82.RotatedBy(-1.5707963705062866) * 3f;
									dust34.scale = 0.5f + Main.rand.NextFloat();
									dust34.fadeIn = 0.5f;
									dust34.customData = Center;
									break;
								}
						}
					}
				}
			}
			VisualAI(0, Start, despawn[0]);
			VisualAI(1, End, despawn[1]);
			alpha[0] = Math.Min(alpha[0], 255);
			alpha[1] = Math.Min(alpha[1], 255);
			if (alpha[0] >= 255 && alpha[1] >= 255 && despawning)
				Projectile.Kill();
			#endregion
			if (Projectile.Center.HasNaNs() || float.IsInfinity(Projectile.Center.X)) System.Diagnostics.Debugger.Break();
		}
		public void ResetExtraAI(int index)
        {
			extraAI[index] = 0;
        }
		float GetOpacity(int index)
        {
			return (float)(255 - alpha[index]) / 255;
        }
        public Color GetAlpha(Color lightColor,int index)
        {
			if (index == 1 && Projectile.ai[1] == 1)
				return Projectile.GetAlpha(Color.Lerp(Color.Yellow, Color.Red,
					0.5f * (float)Math.Sin(Projectile.timeLeft * 0.15f) + 0.5f));
			return Projectile.GetAlpha(lightColor);
        }
        public override bool PreDraw(ref Color lightColor)
		{
			SpriteBatch spriteBatch=Main.spriteBatch;
			Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
			int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
			int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
			Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
			Vector2 origin2 = rectangle.Size() / 2f;
			/*Main.spriteBatch.Draw(texture2D13, Start - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * GetOpacity(0), -rotation[0], origin2, scale[0] * 1.25f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, Start - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White*GetOpacity(0), rotation[0], origin2, scale[0], SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(texture2D13, End - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * GetOpacity(1), -rotation[1], origin2, scale[1] * 1.25f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, End - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * GetOpacity(1), rotation[1], origin2, scale[1], SpriteEffects.None, 0f);
			*///projectile.DrawAim(spriteBatch, End, Color.AliceBlue);
			if (Projectile.ai[1] == 1)
            {
				List<VertexStripInfo> vertecies = new List<VertexStripInfo>();
				int RayWidth = 15;
				Color color = Color.Yellow * (1 - (float)Math.Max(alpha[0], alpha[1]) / 255);
				var unitY = (Projectile.velocity-Projectile.Center).SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
				vertecies.Add(new VertexStripInfo(Start - unitY * RayWidth, color * 0.5f, new Vector3((float)Math.Sqrt(1), 0.3f, 1)));
				vertecies.Add(new VertexStripInfo(Start, color, new Vector3((float)Math.Sqrt(1), 0.7f, 1)));
				vertecies.Add(new VertexStripInfo((Start+End)/2 - unitY * RayWidth, color * 0.5f, new Vector3((float)Math.Sqrt(0), 0.3f, 1)));
				vertecies.Add(new VertexStripInfo((Start+End)/2, color, new Vector3((float)Math.Sqrt(0), 0.7f, 1)));
				vertecies.Add(new VertexStripInfo(End-unitY*RayWidth, color * 0.5f, new Vector3((float)Math.Sqrt(1), 0.3f, 1)));
				vertecies.Add(new VertexStripInfo(End, color, new Vector3((float)Math.Sqrt(1), 0.7f, 1)));
				vertecies.Add(new VertexStripInfo(End, color, new Vector3((float)Math.Sqrt(1), 0.3f, 1)));
				vertecies.Add(new VertexStripInfo(End, color * 0.5f, new Vector3((float)Math.Sqrt(1), 0.7f, 1)));
				vertecies.Add(new VertexStripInfo((Start + End) / 2, color, new Vector3((float)Math.Sqrt(0), 0.3f, 1)));
				vertecies.Add(new VertexStripInfo((Start + End) / 2 + unitY * RayWidth, color * 0.5f, new Vector3((float)Math.Sqrt(0), 0.7f, 1)));
				vertecies.Add(new VertexStripInfo(Start, color, new Vector3((float)Math.Sqrt(1), 0.3f, 1)));
				vertecies.Add(new VertexStripInfo(Start + unitY * RayWidth, color * 0.5f, new Vector3((float)Math.Sqrt(1), 0.7f, 1)));

				WarpMarkDrawer.BeginDraw(spriteBatch, BlendState.Additive);
				var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
				var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

				TeaNPCMartianAddon.Trail.Parameters["alpha"].SetValue(1);
				TeaNPCMartianAddon.Trail.Parameters["uTransform"].SetValue(model * projection);
				TeaNPCMartianAddon.Trail.Parameters["uTime"].SetValue(Projectile.timeLeft * 0.004f);
				TeaNPCMartianAddon.Trail.Parameters["complexTexture"].SetValue(false);

				Main.graphics.GraphicsDevice.Textures[0] = Mod.RequestTexture("Images/Trail");
				Main.graphics.GraphicsDevice.Textures[2] = Mod.RequestTexture("Images/Extra_193");
				//Main.graphics.GraphicsDevice.Textures[1] = Mod.RequestTexture("Images/YellowGrad/img_color");

				Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
				Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
				Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

				TeaNPCMartianAddon.Trail.CurrentTechnique.Passes[0].Apply();

				if (vertecies.Count >= 3)
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertecies.ToArray(), 0, vertecies.Count - 2);

				TeaNPCMartianAddon.Trail.Parameters["complexTexture"].SetValue(false);
				WarpMarkDrawer.EndDraw(spriteBatch);
			}
			WarpMarkDrawer.BeginDraw(spriteBatch, BlendState.Additive);
			Projectile.Draw(spriteBatch, Start, GetAlpha(Color.Turquoise, 0),GetOpacity(0), rotation[0], scale[0]);
			Projectile.Draw(spriteBatch, End, GetAlpha(Color.Turquoise, 1),GetOpacity(1), rotation[1], scale[1]);
			WarpMarkDrawer.EndDraw(spriteBatch);

			//projectile.DrawAim(spriteBatch, projectile.velocity, Color.Yellow);

			WarpMarkDrawer.BeginDraw(spriteBatch, BlendState.AlphaBlend);
			Main.spriteBatch.Draw(texture2D13, Start - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * GetOpacity(0), -rotation[0], origin2, scale[0] * 0.8f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, End - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), GetAlpha(Color.White,1), -rotation[1], origin2, scale[1] * 0.8f, SpriteEffects.FlipHorizontally, 0f);
			WarpMarkDrawer.EndDraw(spriteBatch);

			return false;
		}
		public override void SendExtraAI(BinaryWriter writer)
		{
			writer.Write(Projectile.localAI[0]);
			writer.Write(Projectile.localAI[1]);
			base.SendExtraAI(writer);
		}
		public override void ReceiveExtraAI(BinaryReader reader)
		{
			Projectile.localAI[0] = reader.ReadSingle();
			Projectile.localAI[1] = reader.ReadSingle();
			base.ReceiveExtraAI(reader);
		}
	}
}
