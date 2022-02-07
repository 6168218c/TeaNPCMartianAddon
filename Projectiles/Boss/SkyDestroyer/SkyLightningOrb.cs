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
using Terraria.DataStructures;

namespace TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer
{
    public class SkyLightningOrbCenter : ModProjectile
    {
        public static int RotateDistance => 450;
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.ShadowBeamHostile;
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
                        Projectile.NewProjectile(new ProjectileSource_ProjectileParent(Projectile),Projectile.Center + baseVector.RotatedBy(MathHelper.PiOver2 * i), Vector2.Zero,
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
                        Projectile.localAI[1] = Projectile.NewProjectile(new ProjectileSource_ProjectileParent(Projectile),player.Center, Vector2.Zero, ModContent.ProjectileType<SkyAim>(), 0,
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
        public override string Texture => "Terraria/Images/Projectile_"+ProjectileID.CultistBossLightningOrb;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightning Orb");
            Main.projFrames[Projectile.type] = 4;
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
                int index21 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index21].position = Projectile.Center + vector21 * Projectile.scale;
                Main.dust[index21].velocity = Vector2.Normalize(Main.dust[index21].position - Projectile.Center) * 2f;
                Main.dust[index21].noGravity = true;
                float num1 = (float)(Main.rand.NextDouble() * 1.0 - 0.5);
                if ((double)num1 < -0.5)
                    num1 = -0.5f;
                if ((double)num1 > 0.5)
                    num1 = 0.5f;
                Vector2 vector2 = new Vector2((float)-Projectile.width * 0.6f * Projectile.scale, 0.0f).RotatedBy((double)num1 * 6.28318548202515, new Vector2()).RotatedBy((double)Projectile.velocity.ToRotation(), new Vector2());
                int index2 = Dust.NewDust(Projectile.Center - Vector2.One * 5f, 10, 10, 226, (float)(-(double)Projectile.velocity.X / 3.0), (float)(-(double)Projectile.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                Main.dust[index2].velocity = Vector2.Zero;
                Main.dust[index2].position = Projectile.Center + vector2 * Projectile.scale;
                Main.dust[index2].noGravity = true;
            }

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
                        Projectile.NewProjectile(Projectile.GetProjectileSource(),Projectile.Center, velo, ProjectileID.CultistBossLightningOrbArc,
                            Projectile.damage, 0f, Main.myPlayer, velo.ToRotation());
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
            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Projectile.GetAlpha(lightColor), Projectile.rotation, origin2, Projectile.scale, SpriteEffects.None, 0f);
            return false;
        }
    }
}
