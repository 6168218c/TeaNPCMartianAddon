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

namespace TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer
{
    public class SkyUFO:ModProjectile
    {
        float playerX;
        List<Node> nodeList = new List<Node>();
        public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.UFOMinion;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky UFO");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            base.Projectile.width = 46;
            base.Projectile.height = 46;
            base.Projectile.hostile = true;
            base.Projectile.alpha = 255;
            base.Projectile.ignoreWater = true;
            base.Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
            Projectile.scale = 2.5f;
        }
        public override void AI()
        {
            if (Projectile.localAI[1] == 1)
            {
                Projectile.HoverMovementEx(Projectile.Center - Vector2.UnitY * 150, 60f, 1f);
                int i = Player.FindClosest(Projectile.position, Projectile.width, Projectile.height);
                if (i != -1)
                {
                    if (Projectile.DistanceSQ(Main.player[i].Center) < 1000 * 1000)
                    {
                        return;
                    }
                }
                Projectile.Kill();
                return;
            }
            Projectile.Loomup();
            int index = NPC.FindFirstNPC(ModContent.NPCType<SkyDestroyerHead>());
            if (index == -1)
            {
                Projectile.localAI[1] = 1;
                return;
            }
            NPC head = Main.npc[index];
            if (head.ai[1] != SkyDestroyerSegment.WarpMove)
            {
                Projectile.localAI[1] = 1;
                return;
            }
            Player player = Main.player[head.target];

            if (Projectile.ai[1] == 0)
            {
                Projectile.localAI[0]++;
                if (Projectile.localAI[0] <= 120)
                {
                    playerX = player.Center.X;
                }
                Vector2 target = new Vector2(playerX,player.Center.Y) - Vector2.UnitY * 450 + Vector2.UnitX * Projectile.ai[0] * 180;
                Projectile.HoverMovement(target, 24f, 0.45f);

                if (Projectile.localAI[0] % 5 == 0)
                {
                    nodeList.Clear();
                    Vector2 unit;
                    for (int i = 0; i < 60; i++)
                    {
                        unit = Vector2.UnitY.RotatedBy(-Math.PI / 2).RotatedByRandom(Math.PI) * Main.rand.Next(10, 16);
                        Node node = new Node()
                        {
                            Center = Vector2.UnitY * 30 * i + unit,
                            rotation = Vector2.UnitY.ToRotation(),
                            width = 8 + Main.rand.Next(2, 3)
                        };
                        nodeList.Add(node);
                    }
                }
            }

            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.frameCounter++;
            int num31 = 3;
            if (Projectile.frameCounter >= 4 * num31)
                Projectile.frameCounter = 0;

            Projectile.frame = Projectile.frameCounter / num31;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            var spriteBatch=Main.spriteBatch;
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
                vertecies.Add(new VertexStripInfo(Projectile.Center + nodeList[i].Center + widthUnit * nodeList[i].width / 2, Color.Turquoise, new Vector3((float)Math.Sqrt(factor), 1, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center + nodeList[i].Center - widthUnit * nodeList[i].width / 2, Color.Turquoise, new Vector3((float)Math.Sqrt(factor), 0, 1)));
            }
            //vertecies.Add(new VertexStripInfo(projectile.Center + Vector2.UnitX * 100, Color.Turquoise, new Vector3((float)Math.Sqrt(0), 1, 1)));
            //vertecies.Add(new VertexStripInfo(projectile.Center - Vector2.UnitX * 100, Color.Turquoise, new Vector3((float)Math.Sqrt(0.04f), 0, 1)));
            //vertecies.Add(new VertexStripInfo(projectile.Center - Vector2.UnitY * 100, Color.Turquoise, new Vector3((float)Math.Sqrt(0), 0, 1)));

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

            TeaNPCMartianAddon.Trail.Parameters["alpha"].SetValue(Projectile.Opacity);
            TeaNPCMartianAddon.Trail.Parameters["uTransform"].SetValue(model * projection);
            TeaNPCMartianAddon.Trail.Parameters["uTime"].SetValue(Projectile.timeLeft * 0.04f);

            Main.graphics.GraphicsDevice.Textures[0] = Mod.RequestTexture("Images/Extra_179");

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

            TeaNPCMartianAddon.Trail.CurrentTechnique.Passes[0].Apply();

            if (vertecies.Count >= 3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertecies.ToArray(), 0, vertecies.Count - 2);

            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Vector2.UnitY*1800, Projectile.width * Projectile.scale, ref num6))
            {
                return true;
            }
            return null;
        }
    }
}
