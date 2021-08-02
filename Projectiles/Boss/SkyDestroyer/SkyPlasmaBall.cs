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
using Terraria.DataStructures;

namespace TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer
{
    public class SkyPlasmaBall:ModProjectile
    {
        protected int alphaSecondary;
        public override string Texture => $"{nameof(TeaNPCMartianAddon)}/Projectiles/Boss/SkyDestroyer/GlowRing";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Ball");
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 48;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.scale = 1f;
            alphaSecondary = 255;

            CooldownSlot = 1;
        }
        public override void AI()
        {
            if (Projectile.localAI[1] == 1)
            {
                FadeoutAndKill();
                return;
            }
            Loomup();
            Projectile.localAI[0]++;
            int maxTime = Projectile.ai[0] == 0 ? 600 : (int)Projectile.ai[0];
            if (Projectile.localAI[0] > maxTime)
            {
                Projectile.localAI[1] = 1;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            float secondaryOpacity = 1 - (float)alphaSecondary / 255;

            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.Turquoise), Projectile.rotation, origin2, Projectile.scale * 1.25f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.Turquoise), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Turquoise * secondaryOpacity, Projectile.rotation, origin2, Projectile.scale * 0.75f, SpriteEffects.None, 0f);
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Turquoise * secondaryOpacity, Projectile.rotation, origin2, Projectile.scale * 0.6f, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
        protected void Loomup(int rate = 25)
        {
            Projectile.alpha -= rate;
            if (Projectile.alpha < 0) Projectile.alpha = 0;
            if (Projectile.alpha < 180)
            {
                alphaSecondary -= rate;
                if (alphaSecondary < 0) alphaSecondary = 0;
            }
        }
        protected void Fadeout(int rate = 25)
        {
            alphaSecondary += rate;
            if (alphaSecondary > 255) alphaSecondary = 255;
            if (alphaSecondary > 75)
            {
                Projectile.alpha += rate;
                if (Projectile.alpha > 255) Projectile.alpha = 255;
            }
        }
        protected void FadeoutAndKill(int rate = 25)
        {
            Fadeout();
            if (Projectile.alpha >= 255)
            {
                Projectile.Kill();
            }
        }
    }

    public class SkyPlasmaBallChainHead : SkyPlasmaBall
    {
        public int SegDistance => 80;
        public int maxSpeed => 9;
        struct ChainSegment
        {
            public Vector2 Center { get; set; }
            public float scale;
        }
        ChainSegment[] chainSegments;
        float[] extraAI = new float[2];
        public override void AI()
        {
            if (Projectile.localAI[1] == 1)
            {
                Projectile.velocity = Vector2.Zero;
                FadeoutAndKill();
                return;
            }
            Loomup();
            if (chainSegments == null)//MARK: MIGHT BE MULTIPLAYER BUG
            {
                chainSegments = new ChainSegment[4];
                chainSegments[0].Center = Projectile.Center;
                chainSegments[0].scale = Projectile.scale;
                for(int i = 1; i <= 3; i++)
                {
                    chainSegments[i].Center = Projectile.Center;
                    chainSegments[i].scale = 1 - i * 0.1f;
                }
                /*int prev = projectile.whoAmI;
                int curr;
                for(int i = 0; i < 3; i++)
                {
                    curr = Projectile.NewProjectile(projectile.Center, Vector2.Zero, ModContent.ProjectileType<SkyPlasmaBallChainBody>(), projectile.damage * 4 / 5,
                        0f, Main.myPlayer, 0, prev);
                    Main.projectile[prev].ai[0] = curr;
                    Main.projectile[prev].netUpdate = true;
                    Main.projectile[curr].scale = 1 - (i + 1) * 0.1f;
                    prev = curr;
                    NetMessage.SendData(MessageID.SyncProjectile, -1, -1, null, curr);
                }
                Main.projectile[prev].ai[0] = -1;
                Main.projectile[prev].netUpdate = true;*/
            }
            UpdateChain();

            if (Projectile.ai[1] == 0)
            {
                if (Projectile.localAI[0] == 0)
                {
                    extraAI[0] = Projectile.velocity.ToRotation();
                }
                var baseVelocity = extraAI[0].ToRotationVector2() * maxSpeed;
                Projectile.velocity = baseVelocity + baseVelocity.RotatedBy(MathHelper.PiOver2) 
                    * (float)Math.Cos(Math.PI / 36 * Projectile.localAI[0]) * maxSpeed / 12;
                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= 360)
                {
                    Projectile.localAI[1] = 1;
                    return;
                }
            }
            else if (Projectile.ai[1] == 1)
            {
                int index = NPC.FindFirstNPC(ModContent.NPCType<SkyDestroyerHead>());
                if (index == -1)
                {
                    Projectile.localAI[1] = 1;
                    return;
                }
                NPC head = Main.npc[index];
                if (head.ai[1] != SkyDestroyerSegment.WarpMove && head.ai[1] != SkyDestroyerSegment.PlasmaWarpBlast && head.ai[1] != SkyDestroyerSegment.SpaceWarp)
                {
                    Projectile.localAI[1] = 1;
                    return;
                }
                if (Projectile.localAI[0] == 0)
                {
                    extraAI[0] = -Projectile.velocity.ToRotation();
                    extraAI[1] = -Projectile.velocity.Length();
                }
                if (extraAI[1] < maxSpeed * 2)
                {
                    extraAI[1] += 0.05f;
                    if (Math.Abs(extraAI[1]) < 4.5f)
                    {
                        extraAI[1] += 0.05f;
                    }
                }
                var baseVelocity = extraAI[0].ToRotationVector2() * extraAI[1];
                Projectile.velocity = baseVelocity + baseVelocity.RotatedBy(MathHelper.PiOver2)
                    * (float)Math.Cos(Math.PI / 36 * Projectile.localAI[0]) * extraAI[1] / 12;
                Projectile.localAI[0]++;

                if (Projectile.localAI[0] >= 960)
                {
                    Projectile.localAI[1] = 1;
                    return;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            float secondaryOpacity = 1 - (float)alphaSecondary / 255;

            for(int i = 0; i < chainSegments.Length; i++)
            {
                Main.spriteBatch.Draw(texture2D13, chainSegments[i].Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.Turquoise), Projectile.rotation, origin2, chainSegments[i].scale * 1.25f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture2D13, chainSegments[i].Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.Turquoise), Projectile.rotation, origin2, chainSegments[i].scale, SpriteEffects.None, 0f);
            }


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);

            for(int i = 0; i < chainSegments.Length; i++)
            {
                Main.spriteBatch.Draw(texture2D13, chainSegments[i].Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Turquoise * secondaryOpacity, Projectile.rotation, origin2, chainSegments[i].scale * 0.75f, SpriteEffects.None, 0f);
                Main.spriteBatch.Draw(texture2D13, chainSegments[i].Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Turquoise * secondaryOpacity, Projectile.rotation, origin2, chainSegments[i].scale * 0.6f, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
        }
        void UpdateChain()
        {
            chainSegments[0].Center = Projectile.Center;
            for(int i = 1; i <= 3; i++)
            {
                ref ChainSegment previousSegment = ref chainSegments[i - 1];
                if (Vector2.Distance(chainSegments[i].Center, previousSegment.Center) > 6)
                {
                    Vector2 offset = new Vector2(0, 1f);
                    try//default behavior
                    {
                        offset = previousSegment.Center - chainSegments[i].Center;
                    }
                    catch { }
                    if (offset == Vector2.Zero || offset.HasNaNs()) offset = new Vector2(0, 1f);
                    var dist = SegDistance * chainSegments[i].scale;
                    offset -= Vector2.Normalize(offset) * dist;
                    chainSegments[i].Center += offset;
                }
            }
            
        }
    }
    public class SkyPlasmaRayLauncher : ModProjectile
    {        public override string Texture => "Terraria/Projectile_" + ProjectileID.MagnetSphereBall;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Ball");
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.light = 0.5f;
            Projectile.alpha = 255;

            CooldownSlot = 1;
        }
        public override void AI()
        {
            if (Projectile.localAI[1] == 1)
            {
                Projectile.alpha += 15;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
                return;
            }

            Projectile.localAI[0]++;
            Projectile.Loomup();

            if (Projectile.localAI[0] >= 30 && Projectile.localAI[0] < 180)
            {
                Projectile.SlowDown(0.98f);
            }
            else
            {
                if (Projectile.localAI[0] >= 180)
                {
                    if(Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 velo = Projectile.ai[0].ToRotationVector2() * Projectile.ai[1];
                        Projectile.NewProjectile(new ProjectileSource_ProjectileParent(Projectile),Projectile.Center, velo, ModContent.ProjectileType<SkyPlasmaRay>(),
                            Projectile.damage, 0f, Main.myPlayer, 30);
                    }
                    Projectile.localAI[1] = 1;
                }
            }

            Projectile.frameCounter++;
            if (Projectile.frameCounter > 6)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 4)
                    Projectile.frame = 0;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }
    }
    public class SkyPlasmaRay : ModProjectile
    {
        public int RayWidth => 25;
        public Vector2 GetRayVector()
        {            
            if (Projectile.ai[1] == 0)
            {
                return Projectile.velocity;
            }
            else if (Projectile.ai[1] == 1)
            {
                if (Util.CheckProjAlive<SkyPlasmaArena>((int)Projectile.ai[0]))
                {
                    return Projectile.velocity.RotatedBy(Math.PI / 2).SafeNormalize(Vector2.Zero)
                        * Math.Max(Main.projectile[(int)Projectile.ai[0]].ai[0], 3600);
                }
                else
                    return Projectile.velocity.RotatedBy(Math.PI / 2).SafeNormalize(Vector2.Zero) * 3600;
            }
            else
            {
                return Vector2.Zero;
            }
        }
        public override string Texture => "Terraria/Projectile_" + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Ray");
        }
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            Projectile.alpha = 255;

            CooldownSlot = 1;
        }
        public override void AI()
        {
            if (Projectile.localAI[1] == 1)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha > 255)
                {
                    Projectile.alpha = 255;
                    Projectile.Kill();
                }
                return;
            }
            if (Projectile.ai[1] == 0)
            {
                Projectile.Loomup();
                Projectile.localAI[0]++;
                int maxTime = Projectile.ai[0] == 0 ? 60 : (int)Projectile.ai[0];
                if (Projectile.localAI[0] > maxTime)
                {
                    Projectile.localAI[1] = 1;
                    return;
                }
            }
            else if (Projectile.ai[1] == 1)
            {
                Projectile.Loomup();
                if (!Util.CheckProjAlive<SkyPlasmaArena>((int)Projectile.ai[0]))
                {
                    Projectile.localAI[1] = 1;
                    return;
                }
                Projectile parent = Main.projectile[(int)Projectile.ai[0]];
                Projectile.Center = parent.Center + Projectile.velocity;
            }
        }
        public override void PostDraw(Color lightColor)
        {
            List<VertexStripInfo> vertecies = new List<VertexStripInfo>();
            Color color = Color.Turquoise;
            var RayVector = GetRayVector();
            var unitY = RayVector.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
            vertecies.Add(new VertexStripInfo(Projectile.Center - RayVector - unitY * RayWidth, color, new Vector3((float)Math.Sqrt(1), 0.35f, 1)));
            vertecies.Add(new VertexStripInfo(Projectile.Center - RayVector + unitY * RayWidth, color, new Vector3((float)Math.Sqrt(1), 0.65f, 1)));
            vertecies.Add(new VertexStripInfo(Projectile.Center - unitY * RayWidth, color, new Vector3((float)Math.Sqrt(0), 0.35f, 1)));
            vertecies.Add(new VertexStripInfo(Projectile.Center + unitY * RayWidth, color, new Vector3((float)Math.Sqrt(0), 0.65f, 1)));
            vertecies.Add(new VertexStripInfo(Projectile.Center + RayVector - unitY * RayWidth, color, new Vector3((float)Math.Sqrt(1), 0.35f, 1)));
            vertecies.Add(new VertexStripInfo(Projectile.Center + RayVector + unitY * RayWidth, color, new Vector3((float)Math.Sqrt(1), 0.65f, 1)));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

            var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
            var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

            TeaNPCMartianAddon.Trail.Parameters["alpha"].SetValue(Projectile.Opacity);
            TeaNPCMartianAddon.Trail.Parameters["uTransform"].SetValue(model * projection);
            TeaNPCMartianAddon.Trail.Parameters["uTime"].SetValue(Projectile.timeLeft * 0.04f);

            Main.graphics.GraphicsDevice.Textures[0] = Mod.Assets.Request<Texture2D>("Images/Extra_197").Value;
            Main.graphics.GraphicsDevice.Textures[1] = Mod.Assets.Request<Texture2D>("Images/YellowGrad/img_color").Value;
            Main.graphics.GraphicsDevice.Textures[2] = Mod.Assets.Request<Texture2D>("Images/YellowGrad/img_color").Value;

            Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
            Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
            Main.graphics.GraphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

            TeaNPCMartianAddon.Trail.CurrentTechnique.Passes[0].Apply();

            if (vertecies.Count >= 3)
                Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertecies.ToArray(), 0, vertecies.Count - 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        }
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
            var RayVector = GetRayVector();
            if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - RayVector, Projectile.Center + RayVector, 22f * Projectile.scale, ref num6))
            {
                return true;
            }
            return null;
        }
    }
}
