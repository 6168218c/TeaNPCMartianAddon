using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer
{
    public class SkyProbe : ModNPC
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("天际探测器");
        }
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            this.AIType = -1;
            NPC.npcSlots = 10f;
            NPC.damage = 70;
            NPC.width = 32;
            NPC.height = 32;
            NPC.defense = 70;
            NPC.lifeMax = 800;
            NPC.knockBackResist = 0.9f;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.value = 10000;
            for (int i = 0; i < NPC.buffImmune.Length; i++)
            {
                NPC.buffImmune[i] = true;
            }
        }
        public override void AI()
        {
            if (NPC.AnyNPCs(ModContent.NPCType<SkyDestroyerHead>()))
            {
                NPC.value = 0;
            }
            bool expertMode = Main.expertMode;
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(true);
            }
            float num = 14f;
            float num2 = 0.15f;
            Vector2 vector = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);
            float num3 = Main.player[NPC.target].position.X + (float)(Main.player[NPC.target].width / 2);
            float num4 = Main.player[NPC.target].position.Y + (float)(Main.player[NPC.target].height / 2);
            num3 = (float)((int)(num3 / 8f) * 8);
            num4 = (float)((int)(num4 / 8f) * 8);
            vector.X = (float)((int)(vector.X / 8f) * 8);
            vector.Y = (float)((int)(vector.Y / 8f) * 8);
            num3 -= vector.X;
            num4 -= vector.Y;
            float num5 = (float)Math.Sqrt((double)(num3 * num3 + num4 * num4));
            float num6 = num5;
            bool flag = false;
            if (num5 > 600f)
            {
                flag = true;
            }
            if (num5 == 0f)
            {
                num3 = NPC.velocity.X;
                num4 = NPC.velocity.Y;
            }
            else
            {
                num5 = num / num5;
                num3 *= num5;
                num4 *= num5;
            }
            if (num6 > 100f)
            {
                NPC.ai[0] += 1f;
                if (NPC.ai[0] > 0f)
                {
                    NPC.velocity.Y = NPC.velocity.Y + 0.023f;
                }
                else
                {
                    NPC.velocity.Y = NPC.velocity.Y - 0.023f;
                }
                if (NPC.ai[0] < -100f || NPC.ai[0] > 100f)
                {
                    NPC.velocity.X = NPC.velocity.X + 0.023f;
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X - 0.023f;
                }
                if (NPC.ai[0] > 200f)
                {
                    NPC.ai[0] = -200f;
                }
            }
            if (Main.player[NPC.target].dead)
            {
                num3 = (float)NPC.direction * num / 2f;
                num4 = -num / 2f;
            }
            if (NPC.velocity.X < num3)
            {
                NPC.velocity.X = NPC.velocity.X + num2;
            }
            else if (NPC.velocity.X > num3)
            {
                NPC.velocity.X = NPC.velocity.X - num2;
            }
            if (NPC.velocity.Y < num4)
            {
                NPC.velocity.Y = NPC.velocity.Y + num2;
            }
            else if (NPC.velocity.Y > num4)
            {
                NPC.velocity.Y = NPC.velocity.Y - num2;
            }
            NPC.localAI[0] += 1f;
            if (NPC.justHit)
            {
                NPC.localAI[0] = 0f;
            }
            if (Main.netMode != NetmodeID.MultiplayerClient && NPC.localAI[0] >= 90f)
            {
                NPC.localAI[0] = 0f;
                if (Collision.CanHit(NPC.position, NPC.width, NPC.height, Main.player[NPC.target].position, Main.player[NPC.target].width, Main.player[NPC.target].height))
                {
                    int damage = expertMode ? 28 : 88;
                    int type = ProjectileID.RayGunnerLaser;
                    int num7 = Projectile.NewProjectile(NPC.GetProjectileSource(),vector.X, vector.Y, num3 * 0.55f, num4 * 0.55f, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[num7].timeLeft = 600;
                }
            }
            int num8 = (int)NPC.position.X + NPC.width / 2;
            int num9 = (int)NPC.position.Y + NPC.height / 2;
            num8 /= 16;
            num9 /= 16;
            if (!WorldGen.SolidTile(num8, num9))
            {
                Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (float)(NPC.height / 2)) / 16f), 0.3f, 0f, 0.25f);
            }
            if (num3 > 0f)
            {
                NPC.spriteDirection = 1;
                NPC.rotation = (float)Math.Atan2((double)num4, (double)num3);
            }
            if (num3 < 0f)
            {
                NPC.spriteDirection = -1;
                NPC.rotation = (float)Math.Atan2((double)num4, (double)num3) + 3.14f;
            }
            float num10 = 0.7f;
            if (NPC.collideX)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = NPC.oldVelocity.X * -num10;
                if (NPC.direction == -1 && NPC.velocity.X > 0f && NPC.velocity.X < 2f)
                {
                    NPC.velocity.X = 2f;
                }
                if (NPC.direction == 1 && NPC.velocity.X < 0f && NPC.velocity.X > -2f)
                {
                    NPC.velocity.X = -2f;
                }
            }
            if (NPC.collideY)
            {
                NPC.netUpdate = true;
                NPC.velocity.Y = NPC.oldVelocity.Y * -num10;
                if (NPC.velocity.Y > 0f && (double)NPC.velocity.Y < 1.5)
                {
                    NPC.velocity.Y = 2f;
                }
                if (NPC.velocity.Y < 0f && (double)NPC.velocity.Y > -1.5)
                {
                    NPC.velocity.Y = -2f;
                }
            }
            if (flag)
            {
                if ((NPC.velocity.X > 0f && num3 > 0f) || (NPC.velocity.X < 0f && num3 < 0f))
                {
                    if (Math.Abs(NPC.velocity.X) < 12f)
                    {
                        NPC.velocity.X = NPC.velocity.X * 1.05f;
                    }
                }
                else
                {
                    NPC.velocity.X = NPC.velocity.X * 0.9f;
                }
            }
            if (((NPC.velocity.X > 0f && NPC.oldVelocity.X < 0f) || (NPC.velocity.X < 0f && NPC.oldVelocity.X > 0f) || (NPC.velocity.Y > 0f && NPC.oldVelocity.Y < 0f) || (NPC.velocity.Y < 0f && NPC.oldVelocity.Y > 0f)) && !NPC.justHit)
            {
                NPC.netUpdate = true;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Texture2D texture2D = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            SpriteEffects effects = SpriteEffects.None;
            if (NPC.spriteDirection == -1)
            {
                effects = SpriteEffects.FlipHorizontally;
            }
            texture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            texture2D = Mod.Assets.Request<Texture2D>("Glow/NPCs/SkyProbeGlow").Value;
            Vector2 vector = new Vector2((float)(Terraria.GameContent.TextureAssets.Npc[NPC.type].Value.Width / 2), (float)(Terraria.GameContent.TextureAssets.Npc[NPC.type].Value.Height / Main.npcFrameCount[NPC.type] / 2));
            int num = Main.npcFrameCount[NPC.type];
            Rectangle frame = NPC.frame;
            float scale = NPC.scale;
            float rotation = NPC.rotation;
            float gfxOffY = NPC.gfxOffY;
            Main.spriteBatch.Draw(texture, new Vector2(NPC.position.X - screenPos.X + (float)(NPC.width / 2) - (float)Terraria.GameContent.TextureAssets.Npc[NPC.type].Value.Width * scale / 2f + vector.X * scale, NPC.position.Y - screenPos.Y + (float)NPC.height - (float)Terraria.GameContent.TextureAssets.Npc[NPC.type].Value.Height * scale / (float)Main.npcFrameCount[NPC.type] + 4f + vector.Y * scale + 0f + gfxOffY), new Rectangle?(frame), NPC.GetAlpha(drawColor), rotation, vector, scale, effects, 0f);
            if (NPC.ai[0] != 1f)
            {
                Vector2 value = new Vector2(NPC.Center.X, NPC.Center.Y);
                Vector2 vector2 = value - screenPos;
                vector2 -= new Vector2((float)texture2D.Width, (float)(texture2D.Height / Main.npcFrameCount[NPC.type])) * 1f / 2f;
                vector2 += vector * 1f + new Vector2(-1f, 3f + gfxOffY);
                Color color = new Microsoft.Xna.Framework.Color(200, 200, 200, 0);
                Main.spriteBatch.Draw(texture2D, vector2, new Rectangle?(frame), color, rotation, vector, 1f, effects, 0f);
            }
            return false;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                NPC.position.X = NPC.position.X + (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y + (float)(NPC.height / 2);
                NPC.width = 30;
                NPC.height = 30;
                NPC.position.X = NPC.position.X - (float)(NPC.width / 2);
                NPC.position.Y = NPC.position.Y - (float)(NPC.height / 2);
                for (int i = 0; i < 5; i++)
                {
                    int num = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, MyDustId.CyanShortFx1, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num].velocity *= 3f;
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[num].scale = 0.5f;
                        Main.dust[num].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
                for (int j = 0; j < 10; j++)
                {
                    int num2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, MyDustId.CyanShortFx1, 0f, 0f, 100, default(Color), 3f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity *= 5f;
                    num2 = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, 206, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num2].velocity *= 2f;
                }
                for (int k = 0; k < 3; k++)
                {
                    float scaleFactor = 0.33f;
                    if (k == 1)
                    {
                        scaleFactor = 0.66f;
                    }
                    if (k == 2)
                    {
                        scaleFactor = 1f;
                    }
                    int num3 = Gore.NewGore(new Vector2(NPC.position.X + (float)(NPC.width / 2) - 24f, NPC.position.Y + (float)(NPC.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                    Main.gore[num3].velocity *= scaleFactor;
                    Gore gore = Main.gore[num3];
                    gore.velocity.X = gore.velocity.X + 1f;
                    Gore gore2 = Main.gore[num3];
                    gore2.velocity.Y = gore2.velocity.Y + 1f;
                    num3 = Gore.NewGore(new Vector2(NPC.position.X + (float)(NPC.width / 2) - 24f, NPC.position.Y + (float)(NPC.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                    Main.gore[num3].velocity *= scaleFactor;
                    Gore gore3 = Main.gore[num3];
                    gore3.velocity.X = gore3.velocity.X - 1f;
                    Gore gore4 = Main.gore[num3];
                    gore4.velocity.Y = gore4.velocity.Y + 1f;
                    num3 = Gore.NewGore(new Vector2(NPC.position.X + (float)(NPC.width / 2) - 24f, NPC.position.Y + (float)(NPC.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                    Main.gore[num3].velocity *= scaleFactor;
                    Gore gore5 = Main.gore[num3];
                    gore5.velocity.X = gore5.velocity.X + 1f;
                    Gore gore6 = Main.gore[num3];
                    gore6.velocity.Y = gore6.velocity.Y - 1f;
                    num3 = Gore.NewGore(new Vector2(NPC.position.X + (float)(NPC.width / 2) - 24f, NPC.position.Y + (float)(NPC.height / 2) - 24f), default(Vector2), Main.rand.Next(61, 64), 1f);
                    Main.gore[num3].velocity *= scaleFactor;
                    Gore gore7 = Main.gore[num3];
                    gore7.velocity.X = gore7.velocity.X - 1f;
                    Gore gore8 = Main.gore[num3];
                    gore8.velocity.Y = gore8.velocity.Y - 1f;
                }
            }
        }
    }
}
