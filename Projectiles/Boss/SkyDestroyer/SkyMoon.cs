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
    public class SkyMoon:ModProjectile
    {
        protected int AttackHoverDist => 320;

        protected float State { get => Projectile.localAI[0]; set => Projectile.localAI[0] = value; }
        protected float Timer { get => Projectile.localAI[1]; set => Projectile.localAI[1] = value; }
        public override void SetDefaults()
        {
            Projectile.width = 41;
            Projectile.height = 41;
            Projectile.aiStyle = -1;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

            //projectile.timeLeft = 450;
            Projectile.extraUpdates = 0;
            CooldownSlot = 1;
            Projectile.penetrate = -1;

            Projectile.scale = 0.3f;
        }
        public override void AI()
        {
            /*if (!Util.CheckNPCAlive<SkyDestroyerHead>((int)projectile.ai[1]))
            {
                projectile.Kill();
                return;
            }
            NPC head = Main.npc[(int)projectile.ai[1]];
            Player player = Main.player[head.target];*/
            //testing
            Player player = Main.player[0];
            Projectile.timeLeft = 3600;//everlasting
            if (State == 0)
            {
                //temp
                Projectile.Loomup();
                int position = (int)Projectile.ai[0];
                var dest = player.Center + Vector2.UnitY * 300;
                dest -= Vector2.UnitY.RotatedBy(-MathHelper.Pi / 3).
                    RotatedBy(MathHelper.Pi / 3 * 2 / 3 * (position - 1)) * 750;

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
            else if (State == 1)
            {
                Projectile.Loomup();
                int position = (int)Projectile.ai[0];
                var dest = player.Center + Vector2.UnitY * 300;
                dest -= Vector2.UnitY.RotatedBy(-MathHelper.Pi / 3).
                    RotatedBy(MathHelper.Pi / 3 * 2 / 3 * (position - 1)) * 750;

                HoverEx(dest);
            }
            else if (State == 2)//Preparing to attack
            {
                Projectile.Loomup();
                Timer++;
                Vector2 dest;
                if (Timer <= 30)
                {
                    int position = (int)Projectile.ai[0];
                    var source = player.Center + Vector2.UnitY * 300;
                    source -= Vector2.UnitY.RotatedBy(-MathHelper.Pi / 3).
                        RotatedBy(MathHelper.Pi / 3 * 2 / 3 * (position - 1)) * 750;
                    dest = player.Center - Vector2.UnitY * 375;
                    float lerpValue = (float)Math.Sin(Math.PI / 2 * Timer / 30);
                    dest = Vector2.Lerp(source, dest, lerpValue);
                    Projectile.scale = MathHelper.Lerp(0.3f, 0.5f, lerpValue);
                }
                else
                {
                    dest = player.Center - Vector2.UnitY * AttackHoverDist;
                    Projectile.scale = 0.5f;
                    SwitchTo(3);
                }
                HoverEx(dest);
            }
            else if (State == 3)//attacking
            {
                HoverEx(player.Center - Vector2.UnitY * AttackHoverDist);
            }
            else if (State == 4)//exiting attack stage
            {
                Projectile.Loomup();
                Timer++;
                Vector2 dest;
                if (Timer <= 30)
                {
                    int position = (int)Projectile.ai[0];
                    var source = player.Center + Vector2.UnitY * 300;
                    source -= Vector2.UnitY.RotatedBy(-MathHelper.Pi / 3).
                        RotatedBy(MathHelper.Pi / 3 * 2 / 3 * (position - 1)) * 750;
                    dest = player.Center - Vector2.UnitY * AttackHoverDist;
                    float lerpValue = (float)Math.Sin(Math.PI / 2 * Timer / 30);
                    dest = Vector2.Lerp(dest, source, lerpValue);
                    Projectile.scale = MathHelper.Lerp(0.5f, 0.3f, lerpValue);
                }
                else
                {
                    int position = (int)Projectile.ai[0];
                    dest = player.Center + Vector2.UnitY * 300;
                    dest -= Vector2.UnitY.RotatedBy(-MathHelper.Pi / 3).
                        RotatedBy(MathHelper.Pi / 3 * 2 / 3 * (position - 1)) * 750;
                    Projectile.scale = 0.3f;
                    SwitchTo(1);
                }
                HoverEx(dest);
            }
        }
        public void SwitchTo(float state, bool resetTimer=true)
        {
            State = state;
            if (resetTimer) Timer = 0;
            Projectile.netUpdate = true;
        }
        protected void HoverEx(Vector2 dest)
        {
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
        public Texture2D TextureFromType(int type)
        {
            string name = "";
            switch (type)
            {
                case 1:
                    name = "Solar";
                    break;
                case 2:
                    name = "Nebula";
                    break;
                case 3:
                    name = "Vortex";
                    break;
                case 4:
                    name = "Stardust";
                    break;
            }
            return Mod.Assets.Request<Texture2D>("Projectiles/Boss/SkyDestroyer/Planets/" + name).Value;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            Color color = lightColor * Projectile.Opacity;
            if (State == 1)
            {
                color = Color.Lerp(color, Color.Black, 0.5f) * 0.75f;
            }
            return color;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureFromType((int)Projectile.ai[0]);
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Color glow = new Color(Main.DiscoR + 210, Main.DiscoG + 210, Main.DiscoB + 210);
            Color glow2 = Projectile.GetAlpha(new Color(Main.DiscoR + 50, Main.DiscoG + 50, Main.DiscoB + 50));

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                Vector2 value4 = Projectile.oldPos[i];
                float num165 = Projectile.oldRot[i];
                Main.EntitySpriteDraw(texture2D13, value4 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, num165, origin2, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(Color.White), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Main.EntitySpriteDraw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), glow2 * 0.35f, Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            return false;
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
}
