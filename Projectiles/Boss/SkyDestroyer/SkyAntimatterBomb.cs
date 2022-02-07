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
using Terraria.Localization;
using Terraria.Audio;

namespace TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer
{
    public class SkyMatterMissile : ModProjectile
    {
        public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.SaucerMissile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Matter Missile");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "正物质导弹");
            Main.projFrames[Projectile.type] = 3;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

            Projectile.extraUpdates = 0;
            CooldownSlot = 1;
            Projectile.penetrate = -1;
            Projectile.scale = 2f;
        }
        public override void AI()
        {
            if (!Util.CheckNPCAlive<SkyDestroyerHead>((int)Projectile.ai[0]))
            {
                Projectile.Kill();
                return;
            }
            NPC head = Main.npc[(int)Projectile.ai[0]];
            Projectile.Loomup();
            if (Projectile.localAI[0] % 5f == 0f)
            {
                for (int num648 = 0; num648 < 4; num648++)
                {
                    Vector2 spinningpoint10 = Vector2.UnitX * -8f;
                    spinningpoint10 += -Vector2.UnitY.RotatedBy((float)num648 * (float)Math.PI / 4f) * new Vector2(2f, 4f);
                    spinningpoint10 = spinningpoint10.RotatedBy(Projectile.rotation - (float)Math.PI / 2f);
                    int num649 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldFlame);
                    Main.dust[num649].scale = 1.5f;
                    Main.dust[num649].noGravity = true;
                    Main.dust[num649].position = Projectile.Center + spinningpoint10;
                    Main.dust[num649].velocity = Projectile.velocity * 0f;
                }
            }
            if (Projectile.ai[1] == 0)
            {
                Player player = Main.player[head.target];
                Projectile.localAI[0]++;
                //slightly home in
                if (Projectile.localAI[0] >= 30 && Projectile.localAI[0] <= 180)
                {
                    int direction = Math.Sign(MathHelper.WrapAngle((player.Center - Projectile.Center).ToRotation() - Projectile.velocity.ToRotation()));
                    Projectile.velocity = Projectile.velocity.RotatedBy(direction * 0.008f);
                }
                if (Projectile.localAI[0] >= 480)
                {
                    Projectile.Kill();
                }
            }
            else if (Projectile.ai[1] == 1)
            {
                Projectile.localAI[0]++;

                if (Projectile.velocity.Compare(45f) < 0)
                {
                    if (Projectile.localAI[0] <= 30) Projectile.velocity *= 1.035f;
                    else Projectile.velocity *= 1.05f;
                }
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + (float)Math.PI / 2f;
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            for (int num650 = 0; (float)num650 < 1f; num650++)
            {
                Vector2 value20 = Vector2.UnitY.RotatedBy(Projectile.rotation) * 8f * (num650 + 1);
                int num651 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GoldFlame);
                Main.dust[num651].position = Projectile.Center + value20;
                Main.dust[num651].scale = 1f;
                Main.dust[num651].noGravity = true;
            }
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            Projectile.position = Projectile.Center;
            Projectile.width = (Projectile.height = 112);
            Projectile.position.X -= Projectile.width / 2;
            Projectile.position.Y -= Projectile.height / 2;
            for (int num363 = 0; num363 < 4; num363++)
            {
                Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f, 100, default(Color), 1.5f);
            }

            for (int num364 = 0; num364 < 40; num364++)
            {
                int num365 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.GoldFlame, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[num365].noGravity = true;
                Dust dust = Main.dust[num365];
                dust.velocity *= 3f;
                num365 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.GoldFlame, 0f, 0f, 100, default(Color), 1.5f);
                dust = Main.dust[num365];
                dust.velocity *= 2f;
                Main.dust[num365].noGravity = true;
            }

            for (int num366 = 0; num366 < 1; num366++)
            {
                int num367 = Gore.NewGore(Projectile.position + new Vector2((float)(Projectile.width * Main.rand.Next(100)) / 100f, (float)(Projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default(Vector2), Main.rand.Next(61, 64));
                Gore gore = Main.gore[num367];
                gore.velocity *= 0.3f;
                Main.gore[num367].velocity.X += (float)Main.rand.Next(-10, 11) * 0.05f;
                Main.gore[num367].velocity.Y += (float)Main.rand.Next(-10, 11) * 0.05f;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return null;
        }
    }
    public class SkyAntimatterBombCenter : ModProjectile
    {
        int globalTimer = 0;
        public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Antimatter Target");
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
            if (!Util.CheckNPCAlive<SkyDestroyerHead>((int)Projectile.ai[0]))
            {
                Projectile.Kill();
                return;
            }
            NPC head = Main.npc[(int)Projectile.ai[0]];
            Player player = Main.player[head.target];
            //Player player = Main.player[0];
            Projectile.Center = player.Center;
            if (globalTimer == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Projectile.direction = Main.rand.NextBool() ? -1 : 1;
                for(int i = 0; i < 8; i++)
                {
                    Vector2 offset = Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.TwoPi / 8 * i) * 450;
                    base.Projectile.NewProjectile(Projectile.Center + offset, Vector2.Zero, ModContent.ProjectileType<SkyAntimatterBomb>(),
                        Projectile.damage, 0f, Main.myPlayer, Projectile.whoAmI, i);
                }
            }
            //detect
            int counter = 0;
            for(int i = 0; i < Main.projectile.Length; i++)
            {
                if (Util.CheckProjAlive<SkyAntimatterBomb>(i))
                {
                    Main.projectile[i].ai[1] = counter;
                    counter++;
                }
            }
            if (counter == 0)
            {
                Projectile.Kill();
                return;
            }
            Projectile.localAI[1] = counter;

            globalTimer++;

            if (Projectile.ai[1] == 0)
            {
                Projectile.rotation = MathHelper.WrapAngle(Projectile.rotation + 0.015f * Projectile.direction);
            }
            else if (Projectile.ai[1] == 1)
            {

            }
        }
        public override bool ShouldUpdatePosition() => false;
    }
    public class SkyAntimatterBomb : ModProjectile
    {
        public override string Texture => $"{nameof(TeaNPCMartianAddon)}/Projectiles/Boss/SkyDestroyer/GlowRing";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Antimatter Bomb");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "反物质炸弹");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            base.SetStaticDefaults();
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

            Projectile.extraUpdates = 0;
            CooldownSlot = 1;
            Projectile.penetrate = -1;

            Projectile.scale = 1f;
        }
        public override void AI()
        {
            if (Projectile.localAI[1] == 1)
            {
                Projectile.alpha += 25;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                }
                return;
            }
            if (!Util.CheckProjAlive<SkyAntimatterBombCenter>((int)Projectile.ai[0]))
            {
                Projectile.localAI[1] = 1;
                return;
            }
            Projectile.Loomup();
            Projectile parent = Main.projectile[(int)Projectile.ai[0]];
            
            //check matter missile
            for(int i = 0; i < Main.projectile.Length; i++)
            {
                if (Util.CheckProjAlive<SkyMatterMissile>(i))
                {
                    Projectile missile = Main.projectile[i];
                    if ((missile.Center - Projectile.Center).Compare(Projectile.width) < 0)
                    {
                        missile.Kill();
                        Projectile.Kill();
                        if (Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            base.Projectile.NewProjectile(Projectile.Center, Vector2.Zero, ModContent.ProjectileType<SkyAntimatterExplosion>(),
                                Projectile.damage, 0f, Main.myPlayer, MathHelper.Pi, 360);
                        }
                        return;
                    }
                }
            }

            if (parent.ai[1] == 0)
            {
                Projectile.localAI[0]++;
                Vector2 offset = parent.rotation.ToRotationVector2().RotatedBy(MathHelper.TwoPi / parent.localAI[1] * Projectile.ai[1])
                    * (250 + (float)Math.Sin(Projectile.localAI[0] * Math.PI / 30) * 200);
                Vector2 dest = parent.Center + offset;
                if (Projectile.DistanceSQ(dest) > 60 * 60)
                {
                    Projectile.FastMovement(dest);
                }
                else
                {
                    Projectile.Center = dest;
                    Projectile.velocity = Vector2.Zero;
                }
            }
            else if (parent.ai[1] == 1)
            {

            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.White), Projectile.rotation, origin2, Projectile.scale * 1.25f, SpriteEffects.None, 0f);
            /*List<VertexStripInfo> vertices = new List<VertexStripInfo>();
            for (int i = 1; i < projectile.oldPos.Length; i++)
            {
                if (projectile.oldPos[i] == Vector2.Zero) break;

                var dir = projectile.oldPos[i - 1] - projectile.oldPos[i];
                dir = dir.SafeNormalize(Vector2.Zero).RotatedBy(Math.PI / 2);

                var factor = i / (float)projectile.oldPos.Length;
                var alpha = MathHelper.SmoothStep(1f, 0.5f, factor);

                float width = MathHelper.SmoothStep(0, 20, Math.Min(1, 2.5f * factor));

                if (i > 2)
                {
                    width *= (float)(3 - i) / 3;
                }

                Vector2 d = projectile.oldPos[i - 1] - projectile.oldPos[i];
                vertices.Add(new VertexStripInfo((projectile.oldPos[i] - d * i * 0.4f) + dir * width, Color.White * (1 - factor), new Vector3((float)Math.Sqrt(factor), 1, alpha)));
                vertices.Add(new VertexStripInfo((projectile.oldPos[i] - d * i * 0.35f) + dir * -width, Color.White * (1 - factor), new Vector3((float)Math.Sqrt(factor), 0, alpha)));
            }

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

            TeaNPCAddon.Trail.Parameters["uTransform"].SetValue(model * projection);
            TeaNPCAddon.Trail.Parameters["uTime"].SetValue(projectile.timeLeft * 0.04f);
            TeaNPCAddon.Trail.Parameters["alpha"].SetValue(projectile.Opacity);

            Main.graphics.GraphicsDevice.Textures[0] = Mod.RequestTexture("Images/Extra_193");

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

            TeaNPCAddon.Trail.CurrentTechnique.Passes[0].Apply();

            if (vertices.Count >= 3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);*/

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.Black), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
    public class SkyAntimatterExplosion:ModProjectile
    {
        public static float DistanceToScreenDistance(float dist)
        {
            return dist * Main.GameViewMatrix.Zoom.Y / Main.screenHeight;
        }
        public float R { get => Projectile.ai[1]; set => Projectile.ai[1] = value; }
        public override string Texture => $"{nameof(TeaNPCMartianAddon)}/Projectiles/Boss/SkyDestroyer/GlowRing";
        public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Antimatter Explosion");
			base.SetStaticDefaults();
		}
		public override void SetDefaults()
		{
			Projectile.hostile = true;
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
            /*if (!Main.dedServ)
            {
                if (!Filters.Scene["TeaNPCAddon:CircularDistort"].IsActive())
                {
					Filters.Scene.Activate("TeaNPCAddon:CircularDistort")
						.GetShader()
						.UseTargetPosition(projectile.Center)
						.UseIntensity(projectile.ai[0])
						.UseProgress(projectile.ai[1]);
				}
            }*/

            if (Projectile.localAI[1] == 1)
            {
                Projectile.ai[0] -= 0.15f;
                if (Projectile.ai[0] <= 0)
                {
                    Projectile.Kill();
                }
                return;
            }
            Projectile.localAI[0]++;

            if (Projectile.localAI[0] <= 105)
            {
                if (Projectile.localAI[0] >= 60)
                    Projectile.ai[0] += 0.05f;
                Projectile.Loomup();
            }
            else if (Projectile.localAI[0] > 105 && Projectile.localAI[0] <= 130)
            {
                Projectile.alpha += 25;
                if (Projectile.alpha > 255) Projectile.alpha = 255;
            }
            else if (Projectile.localAI[0] >= 180)
            {
                Projectile.localAI[1] = 1;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch=Main.spriteBatch;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            float scale = R / rectangle.Width * 2.5f;

            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.White), Projectile.rotation, origin2, scale * 1.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.White), Projectile.rotation, origin2, scale, SpriteEffects.None, 0f);
            Texture2D mask = Mod.RequestTexture("Images/Extra_49");
            rectangle = new Rectangle(0, 0, mask.Width, mask.Height);
            scale = R / mask.Width * 2.5f;
            origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(mask,Projectile.Center-Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle),Color.White*Projectile.Opacity, Projectile.rotation, origin2, scale, SpriteEffects.None, 0f);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
        public override void Kill(int timeLeft)
        {
			base.Kill(timeLeft);
        }
        [Obsolete]
		public static void DrawWithShaders_EndCapture(On.Terraria.Graphics.Effects.FilterManager.orig_EndCapture orig, FilterManager self)
        {
			GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            RenderTarget2D renderTarget2D = null;
            RenderTarget2D renderTarget2D2 = Main.screenTarget;
            foreach (Projectile proj in Main.projectile)
            {
                if (proj.type == ModContent.ProjectileType<SkyAntimatterExplosion>() && proj.active && proj.localAI[0] >= 60)
                {
                    renderTarget2D = ((renderTarget2D2 != Main.screenTarget) ? Main.screenTarget : Main.screenTargetSwap);
                    graphicsDevice.SetRenderTarget(renderTarget2D);
                    graphicsDevice.Clear(Color.Black);
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                    Filters.Scene["TeaNPCAddon:CircularDistort"]
                        .GetShader()
                        .UseTargetPosition(proj.Center)
                        .UseIntensity(proj.ai[0])
                        .UseProgress(DistanceToScreenDistance(proj.ai[1]))
                        .Apply();
                    Main.spriteBatch.Draw(renderTarget2D2, Vector2.Zero, Main.ColorOfTheSkies);
                    Main.spriteBatch.End();
                    renderTarget2D2 = ((renderTarget2D2 != Main.screenTarget) ? Main.screenTarget : Main.screenTargetSwap);
                }
            }
            if (renderTarget2D2 == Main.screenTargetSwap)//copy back
            {
                renderTarget2D = ((renderTarget2D2 != Main.screenTarget) ? Main.screenTarget : Main.screenTargetSwap);
                graphicsDevice.SetRenderTarget(renderTarget2D);
                graphicsDevice.Clear(Color.Black);
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.spriteBatch.Draw(renderTarget2D2, Vector2.Zero, Color.White);
                Main.spriteBatch.End();
                renderTarget2D2 = ((renderTarget2D2 != Main.screenTarget) ? Main.screenTarget : Main.screenTargetSwap);
            }

            
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Distance(Projectile.Center) <= R)
            {
                return true;
            }
            return false;
        }
    }
}
