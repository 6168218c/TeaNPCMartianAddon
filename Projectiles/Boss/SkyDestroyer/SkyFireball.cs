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
using Terraria.Localization;
using Terraria.Audio;

namespace TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer
{
    public class SkyFireball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "火球");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            byte a = lightColor.A;
            lightColor = Color.Lerp(lightColor, Color.White, 1f);
            lightColor.A = a;
            return lightColor;
        }
        public override void AI()
        {
            Projectile.Loomup(40);
            if (Projectile.ai[1] == 1)
            {
                Player player = Main.player[(int)Projectile.ai[0]];
                Projectile.localAI[0]++;

                if (Projectile.localAI[0] <= 240)
                {
                    if (Projectile.velocity.Compare(20f) < 0)
                    {
                        Projectile.velocity *= 1.035f;
                    }
                }
                else
                {
                    Projectile.Kill();
                }
            }
            
            if (Projectile.localAI[1] == 0f)
            {
                SoundEngine.PlaySound(SoundID.Item34, Projectile.position);
                Projectile.localAI[1] += 1f;
            }
            Projectile.rotation = (float)Math.Atan2((double)Projectile.velocity.Y, (double)Projectile.velocity.X) + 1.57f;
            #region Visual
            int num;
            Lighting.AddLight(Projectile.Center, 0.2f, 0.1f, 0.6f);
            Projectile.localAI[1] += 1f;
            if (Projectile.localAI[1] == 12f)
            {
                Projectile.localAI[1] = 0f;
                for (int num21 = 0; num21 < 12; num21 = num + 1)
                {
                    Vector2 vector5 = Vector2.UnitX * -(float)Projectile.width / 2f;
                    vector5 += -Vector2.UnitY.RotatedBy((double)((float)num21 * 3.14159274f / 6f), default(Vector2)) * new Vector2(8f, 16f);
                    vector5 = vector5.RotatedBy((double)(Projectile.rotation - 1.57079637f), default(Vector2));
                    int num22 = Dust.NewDust(Projectile.Center, 0, 0, MyDustId.YellowGoldenFire, 0f, 0f, 160, default(Color), 1f);
                    Main.dust[num22].scale = 1.1f;
                    Main.dust[num22].noGravity = true;
                    Main.dust[num22].position = Projectile.Center + vector5;
                    Main.dust[num22].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num22].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num22].position) * 1.25f;
                    num = num21;
                }
            }
            if (Main.rand.Next(4) == 0)
            {
                for (int num23 = 0; num23 < 1; num23 = num + 1)
                {
                    Vector2 value4 = -Vector2.UnitX.RotatedByRandom(0.19634954631328583).RotatedBy((double)Projectile.velocity.ToRotation(), default(Vector2));
                    int num24 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, MyDustId.Smoke, 0f, 0f, 100, default(Color), 1f);
                    Dust dust3 = Main.dust[num24];
                    dust3.velocity *= 0.1f;
                    Main.dust[num24].position = Projectile.Center + value4 * (float)Projectile.width / 2f;
                    Main.dust[num24].fadeIn = 0.9f;
                    num = num23;
                }
            }
            if (Main.rand.Next(32) == 0)
            {
                for (int num25 = 0; num25 < 1; num25 = num + 1)
                {
                    Vector2 value5 = -Vector2.UnitX.RotatedByRandom(0.39269909262657166).RotatedBy((double)Projectile.velocity.ToRotation(), default(Vector2));
                    int num26 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, MyDustId.Smoke, 0f, 0f, 155, default(Color), 0.8f);
                    Dust dust3 = Main.dust[num26];
                    dust3.velocity *= 0.3f;
                    Main.dust[num26].position = Projectile.Center + value5 * (float)Projectile.width / 2f;
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[num26].fadeIn = 1.4f;
                    }
                    num = num25;
                }
            }
            if (Main.rand.Next(2) == 0)
            {
                for (int num27 = 0; num27 < 2; num27 = num + 1)
                {
                    Vector2 value6 = -Vector2.UnitX.RotatedByRandom(0.78539818525314331).RotatedBy((double)Projectile.velocity.ToRotation(), default(Vector2));
                    int num28 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, MyDustId.YellowGoldenFire, 0f, 0f, 0, default(Color), 1.2f);
                    Dust dust3 = Main.dust[num28];
                    dust3.velocity *= 0.3f;
                    Main.dust[num28].noGravity = true;
                    Main.dust[num28].position = Projectile.Center + value6 * (float)Projectile.width / 2f;
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[num28].fadeIn = 1.4f;
                    }
                    num = num27;
                }
            }
            int mun = Projectile.frameCounter;
            Projectile.frameCounter = mun + 1;
            if (Projectile.frameCounter >= 3)
            {
                mun = Projectile.frame;
                Projectile.frame = mun + 1;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            #endregion
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600, false);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 360, true);
        }
        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = (Projectile.height = 176);
            Projectile.Center = Projectile.position;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            int num3;
            for (int num215 = 0; num215 < 4; num215 = num3 + 1)
            {
                int num216 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num216].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                num3 = num215;
            }
            for (int num217 = 0; num217 < 20; num217 = num3 + 1)
            {
                int num218 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.YellowGoldenFire, 0f, 0f, 200, default(Color), 3.7f);
                Main.dust[num218].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                Main.dust[num218].noGravity = true;
                Dust dust = Main.dust[num218];
                dust.velocity *= 3f;
                num218 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.YellowGoldenFire, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num218].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust = Main.dust[num218];
                dust.velocity *= 2f;
                Main.dust[num218].noGravity = true;
                Main.dust[num218].fadeIn = 2.5f;
                num3 = num217;
            }
            for (int num219 = 0; num219 < 10; num219 = num3 + 1)
            {
                int num220 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.YellowGoldenFire, 0f, 0f, 0, default(Color), 2.7f);
                Main.dust[num220].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default(Vector2)) * (float)Projectile.width / 2f;
                Main.dust[num220].noGravity = true;
                Dust dust = Main.dust[num220];
                dust.velocity *= 3f;
                num3 = num219;
            }
            for (int num221 = 0; num221 < 10; num221 = num3 + 1)
            {
                int num222 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.Smoke, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[num222].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default(Vector2)) * (float)Projectile.width / 2f;
                Main.dust[num222].noGravity = true;
                Dust dust = Main.dust[num222];
                dust.velocity *= 3f;
                num3 = num221;
            }
            for (int num223 = 0; num223 < 2; num223 = num3 + 1)
            {
                int num224 = Gore.NewGore(Projectile.position + new Vector2((float)(Projectile.width * Main.rand.Next(100)) / 100f, (float)(Projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default(Vector2), Main.rand.Next(61, 64), 1f);
                Main.gore[num224].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                Gore gore = Main.gore[num224];
                gore.velocity *= 0.3f;
                Gore gore33 = Main.gore[num224];
                gore33.velocity.X = gore33.velocity.X + (float)Main.rand.Next(-10, 11) * 0.05f;
                Gore gore34 = Main.gore[num224];
                gore34.velocity.Y = gore34.velocity.Y + (float)Main.rand.Next(-10, 11) * 0.05f;
                num3 = num223;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] == 1)
            {
                float timer = Projectile.localAI[0];
                if (timer >= 0 && timer <= 90)
                {
                    Color alpha = Color.Yellow;
                    if (timer <= 20) alpha *= timer / 20;
                    else if (timer >= 60) alpha *= (90 - timer) / 30;
                    Projectile.DrawAim(Main.spriteBatch, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 4800f, alpha);
                }
            }
            return PreDraw(ref lightColor);
        }
    }
    public class SkyFireballLauncher : ModProjectile//weird inheriting
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Fireball");
            DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "火球");
            Main.projFrames[Projectile.type] = 4;
        }
        public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.CultistBossFireBall;
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.hostile = true;
            Projectile.alpha = 255;
            Projectile.ignoreWater = true;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.aiStyle = -1;
        }
        public override void AI()
        {
            Projectile.Loomup();
            Projectile.localAI[0]++;
            if (Projectile.ai[1] == 0)//homein
            {
                Player player = Main.player[(int)Projectile.ai[0]];
                if (Projectile.localAI[0] <= 150 * 2)
                {
                    Projectile.SlowDown(0.98f);
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Vector2 baseVector = (player.Center - Projectile.Center)
                            .SafeNormalize(Vector2.Zero)
                            .RotatedBy(-MathHelper.Pi / 12) * 8f;
                        for (int i = 0; i < 3; i++)
                            Projectile.NewProjectile(Projectile.Center, baseVector.RotatedBy(MathHelper.Pi / 12 * i),
                                ProjectileID.CultistBossFireBall, Projectile.damage, 0f, Main.myPlayer, Projectile.ai[0]);
                    }
                    Projectile.Kill();
                }
            }
            else if (Projectile.ai[1] == 1)//retarget
            {
                Player player = Main.player[(int)Projectile.ai[0]];
                if (Projectile.localAI[0] <= 75 * 4)
                {
                    Projectile.SlowDown(0.98f);
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Projectile.Center, (player.Center - Projectile.Center).SafeNormalize(Vector2.UnitY),
                            ModContent.ProjectileType<SkyFireball>(), Projectile.damage, 0f, Main.myPlayer, Projectile.ai[0], 1);
                    }
                    Projectile.Kill();
                }
            }
            else if (Projectile.ai[1] == 2)//horizonal ray
            {
                if (Projectile.localAI[0] <= 75 * 3)
                {
                    Projectile.SlowDown(0.98f);
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Projectile.Center, Vector2.UnitX * 60,
                            ModContent.ProjectileType<SkyFireThrower>(), Projectile.damage, 0f, Main.myPlayer, 0, 2);
                    }
                    Projectile.Kill();
                }
            }
            else if (Projectile.ai[1] == 3)
            {
                if (Projectile.localAI[0] <= 75 * 3)
                {
                    Projectile.SlowDown(0.98f);
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        Projectile.NewProjectile(Projectile.Center, Vector2.UnitY * 60,
                            ModContent.ProjectileType<SkyFireThrower>(), Projectile.damage, 0f, Main.myPlayer, 0, 2);
                    }
                    Projectile.Kill();
                }
            }

            int mun = Projectile.frameCounter;
            Projectile.frameCounter = mun + 1;
            if (Projectile.frameCounter >= 3)
            {
                mun = Projectile.frame;
                Projectile.frame = mun + 1;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 600, false);
        }
        public override void OnHitPlayer(Player target, int damage, bool crit)
        {
            target.AddBuff(BuffID.OnFire, 360, true);
        }
        public override void Kill(int timeLeft)
        {
            Projectile.position = Projectile.Center;
            Projectile.width = (Projectile.height = 176);
            Projectile.Center = Projectile.position;
            Projectile.Damage();
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            int num3;
            for (int num215 = 0; num215 < 4; num215 = num3 + 1)
            {
                int num216 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.Smoke, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num216].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                num3 = num215;
            }
            for (int num217 = 0; num217 < 20; num217 = num3 + 1)
            {
                int num218 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.YellowGoldenFire, 0f, 0f, 200, default(Color), 3.7f);
                Main.dust[num218].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                Main.dust[num218].noGravity = true;
                Dust dust = Main.dust[num218];
                dust.velocity *= 3f;
                num218 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.YellowGoldenFire, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num218].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                dust = Main.dust[num218];
                dust.velocity *= 2f;
                Main.dust[num218].noGravity = true;
                Main.dust[num218].fadeIn = 2.5f;
                num3 = num217;
            }
            for (int num219 = 0; num219 < 10; num219 = num3 + 1)
            {
                int num220 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.YellowGoldenFire, 0f, 0f, 0, default(Color), 2.7f);
                Main.dust[num220].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default(Vector2)) * (float)Projectile.width / 2f;
                Main.dust[num220].noGravity = true;
                Dust dust = Main.dust[num220];
                dust.velocity *= 3f;
                num3 = num219;
            }
            for (int num221 = 0; num221 < 10; num221 = num3 + 1)
            {
                int num222 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, MyDustId.Smoke, 0f, 0f, 0, default(Color), 1.5f);
                Main.dust[num222].position = Projectile.Center + Vector2.UnitX.RotatedByRandom(3.1415927410125732).RotatedBy((double)Projectile.velocity.ToRotation(), default(Vector2)) * (float)Projectile.width / 2f;
                Main.dust[num222].noGravity = true;
                Dust dust = Main.dust[num222];
                dust.velocity *= 3f;
                num3 = num221;
            }
            for (int num223 = 0; num223 < 2; num223 = num3 + 1)
            {
                int num224 = Gore.NewGore(Projectile.position + new Vector2((float)(Projectile.width * Main.rand.Next(100)) / 100f, (float)(Projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default(Vector2), Main.rand.Next(61, 64), 1f);
                Main.gore[num224].position = Projectile.Center + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * (float)Projectile.width / 2f;
                Gore gore = Main.gore[num224];
                gore.velocity *= 0.3f;
                Gore gore33 = Main.gore[num224];
                gore33.velocity.X = gore33.velocity.X + (float)Main.rand.Next(-10, 11) * 0.05f;
                Gore gore34 = Main.gore[num224];
                gore34.velocity.Y = gore34.velocity.Y + (float)Main.rand.Next(-10, 11) * 0.05f;
                num3 = num223;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] == 2)
            {
                float timer = Projectile.localAI[0];
                if (timer >= 0 && timer <= 90)
                {
                    Color alpha = Color.Yellow;
                    if (timer <= 20) alpha *= timer / 20;
                    else if (timer >= 60) alpha *= (90 - timer) / 30;
                    Projectile.DrawAim(Main.spriteBatch, Projectile.Center + Vector2.UnitX * 2400f, alpha);
                    Projectile.DrawAim(Main.spriteBatch, Projectile.Center - Vector2.UnitX * 2400f, alpha);
                }
            }
            if (Projectile.ai[1] == 3)
            {
                float timer = Projectile.localAI[0];
                if (timer >= 0 && timer <= 90)
                {
                    Color alpha = Color.Yellow;
                    if (timer <= 20) alpha *= timer / 20;
                    else if (timer >= 60) alpha *= (90 - timer) / 30;
                    Projectile.DrawAim(Main.spriteBatch, Projectile.Center + Vector2.UnitY * 2400f, alpha);
                    Projectile.DrawAim(Main.spriteBatch, Projectile.Center - Vector2.UnitY * 2400f, alpha);
                }
            }
            return base.PreDraw(ref lightColor);
        }
    }
    public class SkyFireThrower : ModProjectile
	{
        public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.ShadowBeamHostile;
        public static int RayWidth => 25;
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Sky Inferno");
			DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "天际烈焰");
		}
		public override void SetDefaults()
		{
			base.Projectile.width = 6;
			base.Projectile.height = 6;
			base.Projectile.hostile = true;
			base.Projectile.ignoreWater = true;
			base.Projectile.tileCollide = false;
			base.Projectile.penetrate = -1;
			base.Projectile.extraUpdates = 3;

            Projectile.alpha = 255;

			this.CooldownSlot = 1;
		}
		public override void AI()
		{
			if (base.Projectile.scale <= 1.5f)
			{
				base.Projectile.scale *= 1.01f;
			}
            if (Projectile.ai[1] == 2f)
            {
                if (Projectile.localAI[1] == 1)
                {
                    Projectile.alpha += 1;
                    if (Projectile.alpha > 255)
                    {
                        Projectile.alpha = 255;
                        Projectile.Kill();
                    }
                    return;
                }
                Projectile.localAI[0]++;
                if (CanLaunchRay())
                {
                    Projectile.Loomup(5);
                }
                if (Projectile.localAI[0] >= 180 * 3)
                {
                    Projectile.localAI[1] = 1;
                }
            }
            else
            {
                Projectile.alpha = 0;
                if (base.Projectile.ai[1] == 0f)
                {
                    if (base.Projectile.timeLeft > 60)
                    {
                        base.Projectile.timeLeft = 60;
                    }
                }
                else if (base.Projectile.ai[1] == 1f)
                {
                    if (base.Projectile.timeLeft > 12)
                    {
                        base.Projectile.timeLeft = 12;
                    }
                }
                else if (base.Projectile.timeLeft > 80)
                {
                    base.Projectile.timeLeft = 80;
                }
                if (base.Projectile.ai[0] > 5f)
                {
                    float num = 1f;
                    if (base.Projectile.ai[0] == 6f)
                    {
                        num = 0.25f;
                    }
                    else if (base.Projectile.ai[0] == 7f)
                    {
                        num = 0.5f;
                    }
                    else if (base.Projectile.ai[0] == 8f)
                    {
                        num = 0.75f;
                    }
                    base.Projectile.ai[0] += 1f;
                    int num2 = MyDustId.YellowGoldenFire;
                    if (Main.rand.Next(2) == 0)
                    {
                        for (int i = 0; i < 1; i++)
                        {
                            int num3 = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, num2, base.Projectile.velocity.X * 0.2f, base.Projectile.velocity.Y * 0.2f, 100, default(Color), 1f);
                            if (num2 == MyDustId.YellowGoldenFire && Main.rand.Next(3) == 0)
                            {
                                Main.dust[num3].noGravity = true;
                                Main.dust[num3].scale *= 3f;
                                Dust dust = Main.dust[num3];
                                dust.velocity.X = dust.velocity.X * 2f;
                                Dust dust2 = Main.dust[num3];
                                dust2.velocity.Y = dust2.velocity.Y * 2f;
                            }
                            else
                            {
                                Main.dust[num3].scale *= 1.5f;
                            }
                            Dust dust3 = Main.dust[num3];
                            dust3.velocity.X = dust3.velocity.X * 1.2f;
                            Dust dust4 = Main.dust[num3];
                            dust4.velocity.Y = dust4.velocity.Y * 1.2f;
                            Main.dust[num3].scale *= num;
                            if (num2 == MyDustId.YellowGoldenFire)
                            {
                                Main.dust[num3].velocity += base.Projectile.velocity;
                                if (!Main.dust[num3].noGravity)
                                {
                                    Main.dust[num3].velocity *= 0.5f;
                                }
                            }
                        }
                    }
                }
                else
                {
                    base.Projectile.ai[0] += 1f;
                }
                base.Projectile.rotation += 0.3f * (float)base.Projectile.direction;
            }
            Lighting.AddLight(base.Projectile.Center, (float)(255 - base.Projectile.alpha) * 0.35f / 255f, (float)(255 - base.Projectile.alpha) * 0f / 255f, (float)(255 - base.Projectile.alpha) * 0.45f / 255f);
		}
        public override bool ShouldUpdatePosition()
        {
            if (Projectile.ai[1] == 2)
            {
                return false;
            }
            return true;
        }
        public override bool? CanDamage()
        {
            return Projectile.alpha <= 80;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (CanLaunchRay())
            {
                if (projHitbox.Intersects(targetHitbox))
                {
                    return true;
                }
                float num6 = 0f;
                if (Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center - Projectile.velocity * 60, Projectile.Center + Projectile.velocity * 60, 22f * Projectile.scale, ref num6))
                {
                    return true;
                }
            }
            return null;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 1200, false);
		}
		public override void OnHitPlayer(Player target, int damage, bool crit)
		{
			target.AddBuff(BuffID.OnFire, 600, true);
		}
        public override void PostDraw(Color lightColor)
        {
            var spriteBatch = Main.spriteBatch;
            if (CanLaunchRay())
            {
                List<VertexStripInfo> vertecies = new List<VertexStripInfo>();
                Color color = Color.Lerp(Color.Orange, Color.Yellow, 0.5f);
                var unitY = Projectile.velocity.SafeNormalize(Vector2.UnitX).RotatedBy(MathHelper.PiOver2);
                vertecies.Add(new VertexStripInfo(Projectile.Center - Projectile.velocity * 75 - unitY * RayWidth, color * 0.5f, new Vector3((float)Math.Sqrt(1), 0.3f, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center - Projectile.velocity * 75, color, new Vector3((float)Math.Sqrt(1), 0.7f, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center - unitY * RayWidth, color * 0.5f, new Vector3((float)Math.Sqrt(0), 0.3f, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center, color, new Vector3((float)Math.Sqrt(0), 0.7f, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center + Projectile.velocity * 75 - unitY * RayWidth, color * 0.5f, new Vector3((float)Math.Sqrt(1), 0.3f, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center + Projectile.velocity * 75, color, new Vector3((float)Math.Sqrt(1), 0.7f, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center + Projectile.velocity * 75, color, new Vector3((float)Math.Sqrt(1), 0.3f, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center + Projectile.velocity * 75 + unitY * RayWidth, color * 0.5f, new Vector3((float)Math.Sqrt(1), 0.7f, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center, color, new Vector3((float)Math.Sqrt(0), 0.3f, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center + unitY * RayWidth, color * 0.5f, new Vector3((float)Math.Sqrt(0), 0.7f, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center - Projectile.velocity * 75, color, new Vector3((float)Math.Sqrt(1), 0.3f, 1)));
                vertecies.Add(new VertexStripInfo(Projectile.Center - Projectile.velocity * 75 + unitY * RayWidth, color * 0.5f, new Vector3((float)Math.Sqrt(1), 0.7f, 1)));

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone);

                var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
                var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;

                TeaNPCMartianAddon.Trail.Parameters["alpha"].SetValue(Projectile.Opacity);
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
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            }
        }
        public bool CanLaunchRay()
        {
            return Projectile.ai[1] == 2 && Projectile.localAI[0] >= 5 * 3;
        }
    }
}
