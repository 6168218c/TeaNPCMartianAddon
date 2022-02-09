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
    public class SkyLightningOrbCenter : ModProjectile
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
            if (!Util.CheckNPCAlive<SkyDestroyerHead>((int)Projectile.ai[0]))
            {
                Projectile.Kill();
                return;
            }
            NPC head = Main.npc[(int)Projectile.ai[0]];
            Player player = Main.player[head.target];
            if (Projectile.ai[1] == 0)
            {
                if (Projectile.localAI[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.rotation = Main.rand.NextFloat(-MathHelper.PiOver2, MathHelper.PiOver2);
                    var baseVector = Projectile.rotation.ToRotationVector2() * RotateDistance;
                    for(int i = 0; i < 4; i++)
                    {
                        base.Projectile.NewProjectile(Projectile.Center + baseVector.RotatedBy(MathHelper.PiOver2 * i), Vector2.Zero,
                            ModContent.ProjectileType<SkyLightningOrb>(), Projectile.damage, 0f, Main.myPlayer, Projectile.whoAmI, i);
                    }
                    Projectile.direction = Main.rand.NextBool() ? -1 : 1;
                }
                Projectile.localAI[0]++;
                if (Projectile.localAI[0] % 120 <= 75)
                {
                    Projectile.Center = Vector2.Lerp(Projectile.Center, player.Center, 1 / 2f);
                    if (Projectile.localAI[0] % 120 == 60 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.localAI[1] = base.Projectile.NewProjectile(player.Center, Vector2.Zero, ModContent.ProjectileType<SkyAim>(), 0,
                            0f, Main.myPlayer, -1, 40);
                        Projectile.netUpdate = true;
                    }
                }
                Projectile.rotation = Projectile.rotation + 0.015f * Projectile.direction;
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
    public class SkyLightningOrb:ModProjectile
    {
        public override string Texture => MigrationUtils.ProjTexturePrefix+ProjectileID.CultistBossLightningOrb;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Orb");
            Main.projFrames[Projectile.type] = 4;
            ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 2400;
        }
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.aiStyle = -1;
            Projectile.alpha = 255;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            CooldownSlot = 1;
        }
        public override void AI()
        {
            if (Projectile.localAI[1] == 1)
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
            Lighting.AddLight(Projectile.Center, 0.4f, 0.85f, 0.9f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }

            if (Main.rand.Next(3) == 0)
            {
                float num11 = (float)(Main.rand.NextDouble() * 1.0 - 0.5); //vanilla dust
                if ((double)num11 < -0.5)
                    num11 = -0.5f;
                if ((double)num11 > 0.5)
                    num11 = 0.5f;
                Vector2 vector21 = new Vector2((float)-Projectile.width * 0.2f * Projectile.scale, 0.0f).RotatedBy((double)num11 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index21 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, DustID.Electric, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index21].position = Projectile.Center + vector21 * Projectile.scale;
                Main.dust[index21].velocity = Vector2.Normalize(Main.dust[index21].position - Projectile.Center) * 2f;
                Main.dust[index21].noGravity = true;
                float num1 = (float)(Main.rand.NextDouble() * 1.0 - 0.5);
                if ((double)num1 < -0.5)
                    num1 = -0.5f;
                if ((double)num1 > 0.5)
                    num1 = 0.5f;
                Vector2 vector2 = new Vector2((float)-Projectile.width * 0.6f * Projectile.scale, 0.0f).RotatedBy((double)num1 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index2 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, DustID.Electric, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index2].velocity = Vector2.Zero;
                Main.dust[index2].position = Projectile.Center + vector2 * Projectile.scale;
                Main.dust[index2].noGravity = true;
            }

            if (Projectile.ai[0] < 0)
            {
                if (Projectile.ai[1] == 0)
                {
                    Projectile.localAI[0]++;
                    byte index = Player.FindClosest(Projectile.Center, Projectile.width, Projectile.height);
                    int maxTime = Math.Max((int)Math.Abs(Projectile.ai[0]), 90);
                    if (index >= 0 && index < 255 && Projectile.localAI[0] <= maxTime - 15)
                    {
                        Projectile.rotation = (Main.player[index].Center - Projectile.Center).ToRotation();
                    }
                    if (Projectile.localAI[0] >= maxTime)
                    {
                        if (index >= 0 && index <= 255 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            Vector2 velo = Projectile.rotation.ToRotationVector2();
                            velo = velo.RotatedBy(-Math.PI / 12);
                            for (int i = 0; i < 3; i++)
                            {
                                base.Projectile.NewProjectile(Projectile.Center, velo * 9, ProjectileID.CultistBossLightningOrbArc, Projectile.damage * 3 / 5,
                                    0f, Main.myPlayer, velo.ToRotation());
                                velo = velo.RotatedBy(Math.PI / 12);
                            }
                        }
                        Projectile.localAI[1] = 1;
                        return;
                    }
                }
            }
            else
            {
                if (!Util.CheckProjAlive<SkyLightningOrbCenter>((int)Projectile.ai[0]))
                {
                    Projectile.localAI[1] = 1;
                    return;
                }
                Projectile parent = Main.projectile[(int)Projectile.ai[0]];
                if (parent.ai[1] == 0)
                {
                    Projectile.localAI[0]++;

                    if (Projectile.localAI[0] % 120 <= 75)
                    {
                        var tarPos = parent.Center + parent.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * Projectile.ai[1])
                            * SkyLightningOrbCenter.RotateDistance;
                        Projectile.rotation = parent.rotation;
                        //projectile.HoverMovementEx(tarPos,36f,1.5f);
                        Projectile.Center = tarPos;
                    }
                    else
                    {
                        var tarPos = parent.Center + Projectile.rotation.ToRotationVector2().RotatedBy(MathHelper.PiOver2 * Projectile.ai[1])
                            * SkyLightningOrbCenter.RotateDistance;
                        Projectile.FastMovement(tarPos);
                    }
                    if (Projectile.localAI[0] % 120 == 100 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        if (Util.CheckProjAlive<SkyAim>((int)parent.localAI[1]))
                        {
                            Projectile aim = Main.projectile[(int)parent.localAI[1]];
                            var target = aim.Center;
                            var velo = (target - Projectile.Center).SafeNormalize(Vector2.Zero) * 15f;
                            base.Projectile.NewProjectile(Projectile.Center, velo, ProjectileID.CultistBossLightningOrbArc,
                                Projectile.damage, 0f, Main.myPlayer, velo.ToRotation());
                        }
                    }
                }
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[0] < 0 && Projectile.type == ModContent.ProjectileType<SkyLightningOrb>())//ensure not inherited ones
            {
                if (Projectile.ai[1] == 0)
                {
                    int maxTime = Math.Max((int)Math.Abs(Projectile.ai[0]), 90);
                    float timer = Projectile.localAI[0] - (maxTime - 60);
                    if (timer >= 0 && timer <= 45)
                    {
                        Color alpha = Projectile.GetAlpha(lightColor);
                        if (timer <= 10) alpha *= timer / 10;
                        else if (timer >= 30) alpha *= (45 - timer) / 25;
                        Projectile.DrawAim(Main.spriteBatch, Projectile.Center + Projectile.rotation.ToRotationVector2() * 3600, alpha);
                    }
                }
            }
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
    public class SkyLightningOrbEx : SkyLightningOrb
    {
        public override void AI()
        {
            if (Projectile.localAI[1] == 1)
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
            Lighting.AddLight(Projectile.Center, 0.4f, 0.85f, 0.9f);
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 3)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 3)
                    Projectile.frame = 0;
            }

            if (Main.rand.Next(3) == 0)
            {
                float num11 = (float)(Main.rand.NextDouble() * 1.0 - 0.5); //vanilla dust
                if ((double)num11 < -0.5)
                    num11 = -0.5f;
                if ((double)num11 > 0.5)
                    num11 = 0.5f;
                Vector2 vector21 = new Vector2((float)-Projectile.width * 0.2f * Projectile.scale, 0.0f).RotatedBy((double)num11 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index21 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, DustID.Electric, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index21].position = Projectile.Center + vector21 * Projectile.scale;
                Main.dust[index21].velocity = Vector2.Normalize(Main.dust[index21].position - Projectile.Center) * 2f;
                Main.dust[index21].noGravity = true;
                float num1 = (float)(Main.rand.NextDouble() * 1.0 - 0.5);
                if ((double)num1 < -0.5)
                    num1 = -0.5f;
                if ((double)num1 > 0.5)
                    num1 = 0.5f;
                Vector2 vector2 = new Vector2((float)-Projectile.width * 0.6f * Projectile.scale, 0.0f).RotatedBy((double)num1 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index2 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, DustID.Electric, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index2].velocity = Vector2.Zero;
                Main.dust[index2].position = Projectile.Center + vector2 * Projectile.scale;
                Main.dust[index2].noGravity = true;
            }
            if (Projectile.ai[1] == 0)
            {
                Projectile.localAI[0]++;
                int maxTime = (int)(Projectile.ai[0]);
                if (Projectile.localAI[0] >= maxTime)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 unit = Vector2.UnitX;
                        for (int i = 0; i < 8; i++)
                        {
                            Projectile.NewProjectile(Projectile.Center, unit, ModContent.ProjectileType<SkyLightningBolt>(),
                                Projectile.damage, 0f, Main.myPlayer, 90);
                            unit = unit.RotatedBy(Math.PI / 4);
                        }
                    }
                    Projectile.localAI[1] = 1;
                }
            }
            else if (Projectile.ai[1] == 1)
            {
                Projectile.localAI[0]++;
                int maxTime = (int)(Projectile.ai[0]);
                if (Projectile.localAI[0] >= maxTime)
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 unit = Vector2.UnitX.RotatedBy(Math.PI / 8);
                        for (int i = 0; i < 8; i++)
                        {
                            Projectile.NewProjectile(Projectile.Center, unit, ModContent.ProjectileType<SkyLightningBolt>(),
                                Projectile.damage, 0f, Main.myPlayer, 90);
                            unit = unit.RotatedBy(Math.PI / 4);
                        }
                    }
                    Projectile.localAI[1] = 1;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] == 0)
            {
                int maxTime = (int)(Projectile.ai[0]);
                float timer = Projectile.localAI[0] + 60 - maxTime;
                if (timer >= 0 && timer <= 45)
                {
                    Color alpha = Projectile.GetAlpha(lightColor);
                    if (timer <= 10) alpha *= timer / 10;
                    else if (timer >= 30) alpha *= (45 - timer) / 25;
                    Vector2 unit = Vector2.UnitX;
                    for (int i = 0; i < 8; i++)
                    {
                        Projectile.DrawAim(Main.spriteBatch, Projectile.Center + unit * 3600, alpha);
                        unit = unit.RotatedBy(Math.PI / 4);
                    }
                }
            }
            else if (Projectile.ai[1] == 1)
            {
                int maxTime = (int)(Projectile.ai[0]);
                float timer = Projectile.localAI[0] + 60 - maxTime;
                if (timer >= 0 && timer <= 45)
                {
                    Color alpha = Projectile.GetAlpha(lightColor);
                    if (timer <= 10) alpha *= timer / 10;
                    else if (timer >= 30) alpha *= (45 - timer) / 25;
                    Vector2 unit = Vector2.UnitX.RotatedBy(Math.PI / 8);
                    for (int i = 0; i < 8; i++)
                    {
                        Projectile.DrawAim(Main.spriteBatch, Projectile.Center + unit * 3600, alpha);
                        unit = unit.RotatedBy(Math.PI / 4);
                    }
                }
            }
            return base.PreDraw(ref lightColor);
        }
    }
    public class SkyLightningBolt : ModProjectile
    {
        List<Node> nodeList = new List<Node>();
        public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Sky Lightning Bolt");
            DisplayName.AddChineseTranslation("天际雷电");
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
        }
        public override void AI()
        {
            if (Projectile.localAI[1] == 1)
            {
                Projectile.alpha += 12;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
                return;
            }
            Projectile.Loomup();

            if (Projectile.ai[1] == 0)
            {
                Projectile.localAI[0]++;
                if (Projectile.localAI[0] % 5 == 0)
                {
                    nodeList.Clear();
                    int nMax = (int)Math.Min(Projectile.localAI[0], 40 * 5) / 5 * 3;
                    Vector2 unit;
                    for (int i = 0; i < nMax; i++)
                    {
                        unit = Projectile.velocity.RotatedBy(-Math.PI / 2).RotatedByRandom(Math.PI) * Main.rand.Next(10, 16);
                        Node node = new Node()
                        {
                            Center = Projectile.velocity * 30 * i + unit,
                            rotation = Projectile.velocity.ToRotation(),
                            width = 5 + Main.rand.Next(2, 3)
                        };
                        nodeList.Add(node);
                    }
                }
                if (Projectile.localAI[0] >= Projectile.ai[0])
                {
                    Projectile.localAI[1] = 1;
                }
            }
            else if (Projectile.ai[1] == 1)
            {
                Projectile.localAI[0]++;
                if (Projectile.localAI[0] % 5 == 0)
                {
                    nodeList.Clear();
                    int nMax = (int)Math.Min(Projectile.localAI[0], 20 * 5) / 5 * 6;
                    Vector2 unit;
                    for (int i = 0; i < nMax; i++)
                    {
                        unit = Projectile.velocity.RotatedBy(-Math.PI / 2).RotatedByRandom(Math.PI) * Main.rand.Next(10, 16);
                        Node node = new Node()
                        {
                            Center = Projectile.velocity * 30 * i + unit,
                            rotation = Projectile.velocity.ToRotation(),
                            width = 5 + ((float)i / 120) * 80 + Main.rand.Next(2, 3)
                        };
                        nodeList.Add(node);
                    }
                }
                if (Projectile.localAI[0] >= Projectile.ai[0])
                {
                    Projectile.localAI[1] = 1;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            List<VertexStripInfo> vertecies = new List<VertexStripInfo>();
            for (int i = 0; i < nodeList.Count; i++)
            {
                var widthUnit = nodeList[i].rotation.ToRotationVector2().RotatedBy(Math.PI / 2);
                float factor = i / (float)nodeList.Count;
                vertecies.Add(new VertexStripInfo(Projectile.Center + nodeList[i].Center + widthUnit * nodeList[i].width / 2, Color.Turquoise, new Vector3((float)Math.Sqrt(factor), 1, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center + nodeList[i].Center - widthUnit * nodeList[i].width / 2, Color.Turquoise, new Vector3((float)Math.Sqrt(factor), 0, 1)));
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

            Main.graphics.GraphicsDevice.Textures[0] = Mod.RequestTexture("Images/Extra_179");

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
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (projHitbox.Intersects(targetHitbox))
            {
                return true;
            }
            float num6 = 0f;
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * 3600, Projectile.width * Projectile.scale, ref num6))
            {
                return true;
            }
            return null;
        }
        public override bool ShouldUpdatePosition() => false;
    }
}
