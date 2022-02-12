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
using Terraria.GameContent;

namespace TeaNPCMartianAddon.Skies
{
    public class SDEXSky:CustomSky
    {
		private abstract class IUfoController
		{
			public abstract void InitializeUfo(ref Ufo ufo);

			public abstract bool Update(ref Ufo ufo);
		}

		private class ZipBehavior : IUfoController
		{
			private Vector2 _speed;
			private int _ticks;
			private int _maxTicks;

			public override void InitializeUfo(ref Ufo ufo)
			{
				ufo.Position.X = (float)(Ufo.Random.NextDouble() * (double)(Main.maxTilesX << 4));
				ufo.Position.Y = (float)(Ufo.Random.NextDouble() * 5000.0);
				ufo.Opacity = 0f;
				float num = (float)Ufo.Random.NextDouble() * 5f + 10f;
				double num2 = Ufo.Random.NextDouble() * 0.6000000238418579 - 0.30000001192092896;
				ufo.Rotation = (float)num2;
				if (Ufo.Random.Next(2) == 0)
					num2 += 3.1415927410125732;

				_speed = new Vector2((float)Math.Cos(num2) * num, (float)Math.Sin(num2) * num);
				_ticks = 0;
				_maxTicks = Ufo.Random.Next(400, 500);
			}

			public override bool Update(ref Ufo ufo)
			{
				if (_ticks < 10)
					ufo.Opacity += 0.1f;
				else if (_ticks > _maxTicks - 10)
					ufo.Opacity -= 0.1f;

				ufo.Position += _speed;
				if (_ticks == _maxTicks)
					return false;

				_ticks++;
				return true;
			}
		}

		private class HoverBehavior : IUfoController
		{
			private int _ticks;
			private int _maxTicks;

			public override void InitializeUfo(ref Ufo ufo)
			{
				ufo.Position.X = (float)(Ufo.Random.NextDouble() * (double)(Main.maxTilesX << 4));
				ufo.Position.Y = (float)(Ufo.Random.NextDouble() * 5000.0);
				ufo.Opacity = 0f;
				ufo.Rotation = 0f;
				_ticks = 0;
				_maxTicks = Ufo.Random.Next(120, 240);
			}

			public override bool Update(ref Ufo ufo)
			{
				if (_ticks < 10)
					ufo.Opacity += 0.1f;
				else if (_ticks > _maxTicks - 10)
					ufo.Opacity -= 0.1f;

				if (_ticks == _maxTicks)
					return false;

				_ticks++;
				return true;
			}
		}

		private struct Ufo
		{
			private const int MAX_FRAMES = 3;
			private const int FRAME_RATE = 4;
			public static UnifiedRandom Random = new UnifiedRandom();
			private int _frame;
			private Texture2D _texture;
			private IUfoController _controller;
			public Texture2D GlowTexture;
			public Vector2 Position;
			public int FrameHeight;
			public int FrameWidth;
			public float Depth;
			public float Scale;
			public float Opacity;
			public bool IsActive;
			public float Rotation;

			public int Frame
			{
				get
				{
					return _frame;
				}
				set
				{
					_frame = value % 12;
				}
			}

			public Texture2D Texture
			{
				get
				{
					return _texture;
				}
				set
				{
					_texture = value;
					FrameWidth = value.Width;
					FrameHeight = value.Height / 3;
				}
			}

			public IUfoController Controller
			{
				get
				{
					return _controller;
				}
				set
				{
					_controller = value;
					value.InitializeUfo(ref this);
				}
			}

			public Ufo(Texture2D texture, float depth = 1f)
			{
				_frame = 0;
				Position = Vector2.Zero;
				_texture = texture;
				Depth = depth;
				Scale = 1f;
				FrameWidth = texture.Width;
				FrameHeight = texture.Height / 3;
				GlowTexture = null;
				Opacity = 0f;
				Rotation = 0f;
				IsActive = false;
				_controller = null;
			}

			public Rectangle GetSourceRectangle() => new Rectangle(0, _frame / 4 * FrameHeight, FrameWidth, FrameHeight);
			public bool Update() => Controller.Update(ref this);

			public void AssignNewBehavior()
			{
				switch (Random.Next(2))
				{
					case 0:
						Controller = new ZipBehavior();
						break;
					case 1:
						Controller = new HoverBehavior();
						break;
				}
			}
		}

		private Ufo[] _ufos;
		private UnifiedRandom _random = new UnifiedRandom();
		private int _maxUfos;
		private bool _active;
		private bool _leaving;
		private int _activeUfos;
		private float rotation;
		private int timer;
		private int timeMax;

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

			if (Main.gamePaused || !Main.hasFocus)
				return;

			int num = _activeUfos;
			for (int i = 0; i < _ufos.Length; i++)
			{
				Ufo ufo = _ufos[i];
				if (ufo.IsActive)
				{
					ufo.Frame++;
					if (!ufo.Update())
					{
						if (!_leaving)
						{
							ufo.AssignNewBehavior();
						}
						else
						{
							ufo.IsActive = false;
							num--;
						}
					}
				}

				_ufos[i] = ufo;
			}

			if (!_leaving && num != _maxUfos)
			{
				_ufos[num].IsActive = true;
				_ufos[num++].AssignNewBehavior();
			}

			_active = (!_leaving || num != 0);
			_activeUfos = num;
		}

		private float GetIntensity()
		{
			if (UpdateSDEXIndex())
			{
				float x = 0f;
				if (this._sdExIndex != -1)
				{
					Vector2 center = Main.npc[this._sdExIndex].Center;
					if ((Main.npc[this._sdExIndex].ModNPC as SkyDestroyerHead).LocalCloestSegment != -1)
                    {
						center = Main.npc[(Main.npc[this._sdExIndex].ModNPC as SkyDestroyerHead).LocalCloestSegment].Center;
                    }
					x = Vector2.Distance(Main.player[Main.myPlayer].Center, center);
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
			float intensity = this.GetIntensity();
			spriteBatch.Draw(Terraria.GameContent.TextureAssets.BlackTile.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * intensity);
			if (Main.screenPosition.Y <= 10000f)
			{
				int num = -1;
				int num2 = 0;
				for (int i = 0; i < _ufos.Length; i++)
				{
					float depth = _ufos[i].Depth;
					if (num == -1 && depth < maxDepth)
						num = i;

					if (depth <= minDepth)
						break;

					num2 = i;
				}

				if (num != -1)
				{
					Color value = new Color(Main.ColorOfTheSkies.ToVector4() * 0.9f + new Vector4(0.1f));
					Vector2 value2 = Main.screenPosition + new Vector2(Main.screenWidth >> 1, Main.screenHeight >> 1);
					Rectangle rectangle = new Rectangle(-1000, -1000, 4000, 4000);
					for (int j = num; j < num2; j++)
					{
						Vector2 value3 = new Vector2(1f / _ufos[j].Depth, 0.9f / _ufos[j].Depth);
						Vector2 position = _ufos[j].Position;
						position = (position - value2) * value3 + value2 - Main.screenPosition;
						if (_ufos[j].IsActive && rectangle.Contains((int)position.X, (int)position.Y))
						{
							spriteBatch.Draw(_ufos[j].Texture, position, _ufos[j].GetSourceRectangle(), value * _ufos[j].Opacity, _ufos[j].Rotation, Vector2.Zero, value3.X * 5f * _ufos[j].Scale, SpriteEffects.None, 0f);
							if (_ufos[j].GlowTexture != null)
								spriteBatch.Draw(_ufos[j].GlowTexture, position, _ufos[j].GetSourceRectangle(), Color.White * _ufos[j].Opacity, _ufos[j].Rotation, Vector2.Zero, value3.X * 5f * _ufos[j].Scale, SpriteEffects.None, 0f);
						}
					}
				}
			}
			if (maxDepth >= 0f && minDepth < 0f)
			{
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
		private void GenerateUfos()
		{
			float num = (float)Main.maxTilesX / 4200f;
			_maxUfos = (int)(256f * num);
			_ufos = new Ufo[_maxUfos];
			int num2 = _maxUfos >> 4;
			for (int i = 0; i < num2; i++)
			{
				_ = (float)i / (float)num2;
				_ufos[i] = new Ufo(TextureAssets.Extra[5].Value, (float)Main.rand.NextDouble() * 4f + 6.6f);
				_ufos[i].GlowTexture = TextureAssets.GlowMask[90].Value;
			}

			for (int j = num2; j < _ufos.Length; j++)
			{
				_ = (float)(j - num2) / (float)(_ufos.Length - num2);
				_ufos[j] = new Ufo(TextureAssets.Extra[6].Value, (float)Main.rand.NextDouble() * 5f + 1.6f);
				_ufos[j].Scale = 0.5f;
				_ufos[j].GlowTexture = TextureAssets.GlowMask[91].Value;
			}
		}
		public override void Activate(Vector2 position, params object[] args)
		{
			_activeUfos = 0;
			GenerateUfos();
			Array.Sort(_ufos, (Ufo ufo1, Ufo ufo2) => ufo2.Depth.CompareTo(ufo1.Depth));
			_active = true;
			_leaving = false;
			if (args.Count() == 2 && args[0] is float dir && args[1] is int maxTime)
            {
				rotation = dir;
				timer = maxTime;
				timeMax = maxTime;
            }
		}

		public override void Deactivate(params object[] args)
		{
			_leaving = true;
			timer = 0;
		}

		public override void Reset()
		{
			_active = false;
		}

		public override bool IsActive()
		{
			return this._active;
		}
	}
}
