using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using TeaNPCMartianAddon.Effects;
using TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer;
using TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer;
using System.IO;
using Terraria.Enums;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.Utilities;

namespace TeaNPCMartianAddon.Skies
{
    public class SDEXSky:CustomSky
    {
		private float rotation;
		private int timer;
		private int timeMax;
		private bool _isActive;

		private int _sdExIndex = -1;

		public override void OnLoad()
		{
		}

		public override void Update(GameTime gameTime)
		{
			if (timer > 0) timer--;
			if (_sdExIndex != -1 && Main.npc[_sdExIndex].ai[1] == SkyDestroyerSegment.LightningStormEx && timer == 0)
			{
				timer = timeMax;
			}
            if (!UpdateSDEXIndex())
            {
				this.Deactivate();
            }
		}

		private float GetIntensity()
		{
			if (UpdateSDEXIndex())
			{
				float x = 0f;
				if (this._sdExIndex != -1)
				{
					x = Vector2.Distance(Main.player[Main.myPlayer].Center, Main.npc[this._sdExIndex].Center);
				}
				return 1f - Utils.SmoothStep(3000f, 6000f, x);
			}
			return 0f;
		}

		public override Color OnTileColor(Color inColor)
		{
			float intensity = this.GetIntensity();
			return new Color(Vector4.Lerp(new Vector4(0.5f, 0.8f, 1f, 1f), inColor.ToVector4(), 1f - intensity));
		}

		private bool UpdateSDEXIndex()
		{
			if (this._sdExIndex >= 0 && Main.npc[this._sdExIndex].active && Main.npc[this._sdExIndex].type == 398)
			{
				return true;
			}
			int num = -1;
			for (int i = 0; i < Main.npc.Length; i++)
			{
				if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<SkyDestroyerHead>())
				{
					num = i;
					break;
				}
			}
			this._sdExIndex = num;
			return num != -1;
		}

		public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
		{
			if (maxDepth >= 0f && minDepth < 0f)
			{
				float intensity = this.GetIntensity();
				spriteBatch.Draw(Terraria.GameContent.TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * intensity);
				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

				if (_sdExIndex != -1 && Main.npc[_sdExIndex].ai[1] == SkyDestroyerSegment.LightningStormEx)
                {
					float prog = 1 - (float)timer / timeMax;
					for(int i = -1; i <= 1; i++)
                    {
						DrawArrow(new Vector2(Main.screenWidth, Main.screenHeight)/2 + i * rotation.ToRotationVector2() * 180f,
							MathHelper.Clamp(prog - i * 0.3f, 0, 1));
                    }
                }

				spriteBatch.End();
				spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			}
		}
		private float GetArrowOpacity(float progress)
        {
            if (progress <= 0.5f)
            {
				return MathHelper.SmoothStep(0, 1, MathHelper.Clamp((progress - 0.1f) / 0.4f, 0, 1));
            }
            else
            {
				return 1 - MathHelper.SmoothStep(0, 1, MathHelper.Clamp((0.9f - progress) / 0.4f, 0, 1));
            }
        }
		private void DrawArrow(Vector2 center, float opacity)
        {
			float arrowWidth = 30f;
			float height = 270;

			float rectWidth = height + arrowWidth;
			float rectHeight = height * 2;
			Vector2 unitX = Vector2.UnitX.RotatedBy(rotation);
			Vector2 unitY = Vector2.UnitY.RotatedBy(rotation);

			List<VertexStripInfo> vertecies = new List<VertexStripInfo>();
			vertecies.Add(new VertexStripInfo(center - (unitX*rectWidth / 2+unitY* rectHeight), Color.White, new Vector3(0.5f, 0.5f, 1)));
			vertecies.Add(new VertexStripInfo(center - (unitX*(rectWidth / 2 - arrowWidth)+unitY*rectHeight), Color.White, new Vector3(0.5f, 0.5f, 1)));
			vertecies.Add(new VertexStripInfo(center + (unitX*(rectWidth / 2 - arrowWidth)+unitY*0), Color.White, new Vector3(0.5f, 0.5f, 1)));
			vertecies.Add(new VertexStripInfo(center + (unitX*rectWidth / 2+unitY*0), Color.White, new Vector3(0.5f, 0.5f, 1)));
			vertecies.Add(new VertexStripInfo(center - (unitX*rectWidth / 2+unitY*-rectHeight), Color.White, new Vector3(0.5f, 0.5f, 1)));
			vertecies.Add(new VertexStripInfo(center - (unitX*(rectWidth / 2 - arrowWidth)+unitY*-rectHeight), Color.White, new Vector3(0.5f, 0.5f, 1)));

			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3()) * Main.GameViewMatrix.TransformationMatrix;

			TeaNPCMartianAddon.Trail.Parameters["alpha"].SetValue(opacity);
			TeaNPCMartianAddon.Trail.Parameters["uTransform"].SetValue(model * projection);

			Main.graphics.GraphicsDevice.Textures[0] = TeaNPCMartianAddon.Instance.RequestTexture("Images/Trail");

			Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
			Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
			Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

			TeaNPCMartianAddon.Trail.CurrentTechnique.Passes[0].Apply();

			if (vertecies.Count >= 3)
				Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertecies.ToArray(), 0, vertecies.Count - 2);
		}
		public override float GetCloudAlpha()
		{
			return 0f;
		}

		public override void Activate(Vector2 position, params object[] args)
		{
			this._isActive = true;
			if (args.Count() == 2 && args[0] is float dir && args[1] is int maxTime)
            {
				rotation = dir;
				timer = maxTime;
				timeMax = maxTime;
            }
		}

		public override void Deactivate(params object[] args)
		{
			this._isActive = false;
			if (args.Count() == 2 && args[0] is float dir && args[1] is int maxTime)
			{
				rotation = dir;
				timer = maxTime;
				timeMax = maxTime;
			}
		}

		public override void Reset()
		{
			this._isActive = false;
		}

		public override bool IsActive()
		{
			return this._isActive;
		}
	}
}
