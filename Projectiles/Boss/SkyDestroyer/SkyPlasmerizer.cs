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
    internal struct Node
    {
        public Vector2 Center { get; set; }
        public float rotation;
        public float width;
    }
    public class SkyPlasmerizerRay:ModProjectile
    {
        public static int DefaultVelocityFactor => 120;
        public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.ShadowBeamHostile;
        List<Node> nodeList = new List<Node>();
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Plasmerizer");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 25;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;

            CooldownSlot = 1; //not in warning line, test?
            Projectile.hide = true; //fixes weird issues on spawn with scaling
        }
        public override void AI()
        {
            Projectile.hide = false;
            Projectile.alpha = 0;
            if (Projectile.velocity == Vector2.Zero) Projectile.velocity = -Vector2.UnitY * 20;
            if (Projectile.localAI[0] % 10 == 0)
            {
                nodeList.Clear();
                var unit = Projectile.velocity;
                for(int i = 0; i < 120; i++)
                {
                    Node node = new Node()
                    {
                        Center = Projectile.Center + unit * i,
                        rotation = unit.ToRotation(),
                        width = 75 + 15 * (float)Math.Sin((float)(Projectile.timeLeft * 2 - i) / 2)
                    };
                    nodeList.Add(node);
                }
            }

            if (Projectile.localAI[0] == 20 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                base.Projectile.NewProjectile(nodeList[nodeList.Count - 1].Center, Vector2.Zero, ModContent.ProjectileType<SkyPlasmerizerExplosion>(),
                    Projectile.damage, 0f, Main.myPlayer);
                for(int i = 0; i < 8; i++)
                {
                    var pos = nodeList[nodeList.Count - 1].Center;
                    var velo = nodeList[nodeList.Count - 1].rotation.ToRotationVector2().RotatedBy(MathHelper.TwoPi / 8 * i);
                    base.Projectile.NewProjectile(pos, velo.RotatedBy(MathHelper.PiOver2) * 12, ModContent.ProjectileType<SkyPlasmaBallChainHead>(),
                        Projectile.damage * 4 / 5, 0f, Main.myPlayer, 120);
                }
            }

            if (Projectile.localAI[0] <= 60)
            {
                if (Projectile.localAI[0] <= 10)
                {
                    Projectile.Opacity = Projectile.localAI[0] / 10;
                }
                else if (Projectile.localAI[0] >= 45)
                {
                    Projectile.Opacity = (60 - Projectile.localAI[0]) / 15;
                }
            }
            else
            {
                Projectile.Kill();
                return;
            }

            Projectile.localAI[0]++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch=Main.spriteBatch;
            List<VertexStripInfo> vertecies = new List<VertexStripInfo>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                /*if (projectile.oldPos[i] == Vector2.Zero) break;

                var dir = projectile.oldPos[i - 1] - projectile.oldPos[i];
                dir = dir.SafeNormalize(Vector2.Zero).RotatedBy(Math.PI / 2);

                var factor = i / (float)projectile.oldPos.Length;
                var alpha = MathHelper.SmoothStep(1f, 0.5f, factor);

                float width = MathHelper.SmoothStep(0, 20, Math.Min(1, 2.5f * factor));

                if (i > 15)
                {
                    width *= (float)(25 - i) / 10;
                }
                if (projectile.localAI[1] == 1)
                {
                    alpha *= (float)(255 - projectile.alpha) / 25;
                    width *= (float)(255 - projectile.alpha) / 25;
                }

                Vector2 d = projectile.oldPos[i - 1] - projectile.oldPos[i];
                vertecies.Add(new VertexStripInfo((projectile.oldPos[i] - d * i * 0.4f) + dir * width, Color.White, new Vector3((float)Math.Sqrt(factor), 1, alpha)));
                vertecies.Add(new VertexStripInfo((projectile.oldPos[i] - d * i * 0.35f) + dir * -width, Color.White, new Vector3((float)Math.Sqrt(factor), 0, alpha)));*/
                var widthUnit = nodeList[i].rotation.ToRotationVector2().RotatedBy(Math.PI / 2);
                float factor = i / (float)nodeList.Count;
                vertecies.Add(new VertexStripInfo(nodeList[i].Center + widthUnit * nodeList[i].width / 2, Color.Turquoise, new Vector3((float)Math.Sqrt(factor), 1, 1)));
                vertecies.Add(new VertexStripInfo(nodeList[i].Center - widthUnit * nodeList[i].width / 2, Color.Turquoise, new Vector3((float)Math.Sqrt(factor), 0, 1)));
            }
            //vertecies.Add(new VertexStripInfo(projectile.Center + Vector2.UnitX * 100, Color.Turquoise, new Vector3((float)Math.Sqrt(0), 1, 1)));
            //vertecies.Add(new VertexStripInfo(projectile.Center - Vector2.UnitX * 100, Color.Turquoise, new Vector3((float)Math.Sqrt(0.04f), 0, 1)));
            //vertecies.Add(new VertexStripInfo(projectile.Center - Vector2.UnitY * 100, Color.Turquoise, new Vector3((float)Math.Sqrt(0), 0, 1)));

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

            TeaNPCMartianAddon.Trail.Parameters["alpha"].SetValue(Projectile.Opacity);
            TeaNPCMartianAddon.Trail.Parameters["uTransform"].SetValue(model * projection);
            TeaNPCMartianAddon.Trail.Parameters["uTime"].SetValue(Projectile.timeLeft * 0.02f);

            Main.graphics.GraphicsDevice.Textures[0] = Mod.RequestTexture("Images/Trail");
            Main.graphics.GraphicsDevice.Textures[1] = Mod.RequestTexture("Images/YellowGrad/img_color");
            Main.graphics.GraphicsDevice.Textures[2] = Mod.RequestTexture("Images/YellowGrad/img_color");

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

            TeaNPCMartianAddon.Trail.CurrentTechnique.Passes[0].Apply();

            if (vertecies.Count >= 3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertecies.ToArray(), 0, vertecies.Count - 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity*nodeList.Count, 22f * Projectile.scale, ref num6))
            {
                return true;
            }
            return false;
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity, (float)Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
        }
    }
    public class SkyPlasmerizerExplosion : ModProjectile
    {
        public static int R => 450;
        public override string Texture => $"{nameof(TeaNPCMartianAddon)}/Projectiles/Boss/SkyDestroyer/GlowRing";
        List<Node> nodeList = new List<Node>();
        List<Node> circleList = new List<Node>();
        List<Node> ballList = new List<Node>();
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Plasmerizer");
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 0.05f;

            CooldownSlot = 1; //not in warning line, test?
            Projectile.hide = true; //fixes weird issues on spawn with scaling
        }
        public override void AI()
        {
            Projectile.hide = false;
            Projectile.alpha = 0;
            if (Projectile.localAI[0] % 3 == 0)
            {
                nodeList.Clear();
                var unit = Vector2.UnitY * R * Projectile.scale;
                for (int i = 0; i < 90; i++)
                {
                    var dist = unit.RotatedBy(Math.PI * 2 / 45 * i).RotatedBy(Math.PI / 180).RotatedByRandom(Math.PI / 90)
                        * Main.rand.NextFloat(0.95f, 1.05f);
                    Node node = new Node()
                    {
                        Center = Projectile.Center + dist,
                        rotation = dist.ToRotation(),
                        width = 100
                    };
                    nodeList.Add(node);
                }
                nodeList.Add(nodeList[0]);
                circleList.Clear();
                unit = Vector2.UnitY * R / 6 * Projectile.scale * MathHelper.Clamp(3 - Projectile.scale, 0, 1.8f);
                for (int i = 0; i < 45; i++)
                {
                    var dist = unit.RotatedBy(Math.PI * 2 / 45 * i) * Main.rand.NextFloat(0.99f, 1.01f);
                    Node node = new Node()
                    {
                        Center = Projectile.Center + dist,
                        rotation = dist.ToRotation(),
                        width = dist.Length()*2
                    };
                    circleList.Add(node);
                }
                circleList.Add(circleList[0]);
            }
            /*if (projectile.localAI[0] % 20 == 0&&Main.netMode!=NetmodeID.MultiplayerClient)
            {
                var unit = Vector2.UnitY * R * projectile.scale;
                ballList.Clear();
                for (int i = 0; i < 36; i++)
                {
                    var dist = unit.RotatedBy(Math.PI * 2 / 36 * i).RotatedBy(Math.PI / 180).RotatedByRandom(Math.PI / 90)
                        * Main.rand.NextFloat(0.95f, 1.05f);
                    Node node = new Node()
                    {
                        Center = projectile.Center + dist
                    };
                    ballList.Add(node);
                }
            }*/
            if (Main.rand.Next(2) == 0)
            {
                Vector2 vector70 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                Dust dust23 = Main.dust[Dust.NewDust(Projectile.Center - vector70 * R*Projectile.scale, 0, 0, DustID.Vortex)];
                dust23.noGravity = true;
                dust23.position = Projectile.Center - vector70 * Main.rand.NextFloat(R * Projectile.scale / 3, R * Projectile.scale * 2 / 3);
                dust23.velocity = vector70.RotatedBy(1.5707963705062866) * 6f;
                dust23.scale = 0.5f + Main.rand.NextFloat();
                dust23.fadeIn = 0.5f;
                dust23.customData = Projectile.Center;
            }

            if (Main.rand.Next(2) == 0)
            {
                Vector2 vector71 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                Dust dust24 = Main.dust[Dust.NewDust(Projectile.Center - vector71 * R*Projectile.scale, 0, 0, DustID.Clentaminator_Cyan)];
                dust24.noGravity = true;
                dust24.position = Projectile.Center - vector71 * R * Projectile.scale;
                dust24.velocity = vector71.RotatedBy(-1.5707963705062866) * 3f;
                dust24.scale = 0.5f + Main.rand.NextFloat();
                dust24.fadeIn = 0.5f;
                dust24.customData = Projectile.Center;
            }
            if (Projectile.scale < 1)
            {
                Projectile.scale += 0.009f;
                if (Projectile.scale >= 1) Projectile.localAI[0] = 0;
            }
            else
            {
                if (Projectile.localAI[0] >= 60 && Projectile.localAI[0] < 90)
                {
                    Projectile.Opacity = (90 - Projectile.localAI[0]) / 30;
                }
                else if (Projectile.localAI[0] >= 90)
                {
                    Projectile.Kill();
                    return;
                }
            }

            Projectile.localAI[0]++;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            /*foreach (Node node in ballList)
            {
                Main.spriteBatch.Draw(texture2D13, node.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(Color.Turquoise), projectile.rotation, origin2, 1.25f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture2D13, node.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(Color.Turquoise), projectile.rotation, origin2, 1f, SpriteEffects.None, 0f);
            }*/
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.Turquoise * 0.6f), Projectile.rotation, origin2, Projectile.scale * 3f, SpriteEffects.None, 0f);
            /*foreach (Node node in ballList)
            {
                Main.spriteBatch.Draw(texture2D13, node.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(Color.Turquoise), projectile.rotation, origin2, 0.75f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture2D13, node.Center - Main.screenPosition + new Vector2(0f, projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), projectile.GetAlpha(Color.Turquoise), projectile.rotation, origin2, 0.6f, SpriteEffects.None, 0f);
            }*/

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
        public override void PostDraw(Color lightColor)
        {
            SpriteBatch spriteBatch=Main.spriteBatch;
            List<VertexStripInfo> vertecies = new List<VertexStripInfo>();
            List<VertexStripInfo> circleVertecies = new List<VertexStripInfo>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                var widthUnit = nodeList[i].rotation.ToRotationVector2();
                float factor = (i / (float)nodeList.Count) * 0.8f + 0.2f;
                vertecies.Add(new VertexStripInfo(nodeList[i].Center + widthUnit * nodeList[i].width / 2, Color.Turquoise, new Vector3((float)Math.Sqrt(factor), 1, 1)));
                vertecies.Add(new VertexStripInfo(nodeList[i].Center - widthUnit * nodeList[i].width / 2, Color.Turquoise, new Vector3((float)Math.Sqrt(factor), 0, 1)));
            }
            for (int i = 0; i < circleList.Count; i++)
            {
                var widthUnit = circleList[i].rotation.ToRotationVector2();
                float factor = i / (float)circleList.Count;
                circleVertecies.Add(new VertexStripInfo(circleList[i].Center + widthUnit * circleList[i].width / 2, Color.Turquoise, new Vector3((float)Math.Sqrt(factor), 1, 1)));
                circleVertecies.Add(new VertexStripInfo(circleList[i].Center - widthUnit * circleList[i].width / 2, Color.Turquoise, new Vector3((float)Math.Sqrt(factor), 0, 1)));
            }
            //vertecies.Add(new VertexStripInfo(projectile.Center + Vector2.UnitX * 100, Color.Turquoise, new Vector3((float)Math.Sqrt(0), 1, 1)));
            //vertecies.Add(new VertexStripInfo(projectile.Center - Vector2.UnitX * 100, Color.Turquoise, new Vector3((float)Math.Sqrt(0.04f), 0, 1)));
            //vertecies.Add(new VertexStripInfo(projectile.Center - Vector2.UnitY * 100, Color.Turquoise, new Vector3((float)Math.Sqrt(0), 0, 1)));

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

            TeaNPCMartianAddon.Trail.Parameters["alpha"].SetValue(Projectile.Opacity);
            TeaNPCMartianAddon.Trail.Parameters["uTransform"].SetValue(model * projection);
            TeaNPCMartianAddon.Trail.Parameters["uTime"].SetValue(Projectile.timeLeft * 0.04f);

            Main.graphics.GraphicsDevice.Textures[0] = Mod.RequestTexture("Images/Trail");
            Main.graphics.GraphicsDevice.Textures[1] = Mod.RequestTexture("Images/YellowGrad/img_color");
            Main.graphics.GraphicsDevice.Textures[2] = Mod.RequestTexture("Images/YellowGrad/img_color");

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

            TeaNPCMartianAddon.Trail.CurrentTechnique.Passes[0].Apply();

            if (vertecies.Count >= 3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertecies.ToArray(), 0, vertecies.Count - 2);
            if(circleVertecies.Count>=3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, circleVertecies.ToArray(), 0, circleVertecies.Count - 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            if (targetHitbox.Distance(Projectile.Center) <= Projectile.scale * R)
            {
                return true;
            }
            return false;
        }
        public override void CutTiles()
        {
            DelegateMethods.tilecut_0 = TileCuttingContext.AttackProjectile;
            Vector2 unit = Projectile.velocity;
            Utils.PlotTileLine(Projectile.Center, Projectile.Center + Projectile.velocity, (float)Projectile.width * Projectile.scale, DelegateMethods.CutTiles);
        }
    }
}
