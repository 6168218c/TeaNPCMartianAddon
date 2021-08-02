using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TeaNPCMartianAddon.Items.Bosses.Martians;

namespace TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer
{
    public class SkySearcherHead : ModNPC
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("天际搜寻者");
        }
        public override void SetDefaults()
        {
            base.NPC.damage = 120;
            base.NPC.width = 34;
            base.NPC.height = 48;
            base.NPC.defense = 45;
            base.NPC.lifeMax = 4000;
            base.NPC.aiStyle = -1;
            this.AIType = -1;
            for (int i = 0; i < base.NPC.buffImmune.Length; i++)
            {
                base.NPC.buffImmune[i] = true;
            }
            base.NPC.knockBackResist = 0f;
            base.NPC.value = 50000;
            base.NPC.behindTiles = true;
            base.NPC.noGravity = true;
            base.NPC.noTileCollide = true;
            base.NPC.HitSound = SoundID.NPCHit4;
            base.NPC.DeathSound = SoundID.NPCDeath14;
            base.NPC.netAlways = true;
            //this.banner = NPC.type;
            //this.bannerItem = ModContent.ItemType<SkySearcherBanner>();
        }
        private float passedVar;
        private bool tail;
        public override void AI()
        {
            if (!NPC.AnyNPCs(ModContent.NPCType<SkyDestroyerHead>()))
            {
                NPC.life = 0;
                NPC.HitEffect();
                NPC.checkDead();
                return;
            }
            if (base.NPC.target < 0 || base.NPC.target == 255 || Main.player[base.NPC.target].dead)
            {
                base.NPC.TargetClosest(true);
            }
            base.NPC.velocity.Length();
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                #region Spawn
                if (!this.tail && base.NPC.ai[0] == 0f)
                {
                    int num = base.NPC.whoAmI;
                    for (int i = 0; i < 15; i++)
                    {
                        int num2;
                        if (i >= 0 && i < 14 && i % 2 == 0)
                        {
                            num2 = NPC.NewNPC((int)base.NPC.position.X + base.NPC.width / 2, (int)base.NPC.position.Y + base.NPC.height / 2, ModContent.NPCType<SkySearcherBodyAlt>(), base.NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                            Main.npc[num2].localAI[0] += this.passedVar;
                            this.passedVar += 36f;
                        }
                        else if (i >= 0 && i < 14)
                        {
                            num2 = NPC.NewNPC((int)base.NPC.position.X + base.NPC.width / 2, (int)base.NPC.position.Y + base.NPC.height / 2, ModContent.NPCType<SkySearcherBody>(), base.NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                            if (base.NPC.localAI[0] % 2f == 0f)
                            {
                                Main.npc[num2].localAI[3] = 1f;
                                base.NPC.localAI[0] = 1f;
                            }
                            else
                            {
                                base.NPC.localAI[0] = 2f;
                            }
                        }
                        else
                        {
                            num2 = NPC.NewNPC((int)base.NPC.position.X + base.NPC.width / 2, (int)base.NPC.position.Y + base.NPC.height / 2, ModContent.NPCType<SkySearcherTail>(), base.NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                        }
                        Main.npc[num2].realLife = base.NPC.whoAmI;
                        Main.npc[num2].ai[2] = (float)base.NPC.whoAmI;
                        Main.npc[num2].ai[1] = (float)num;
                        Main.npc[num].ai[0] = (float)num2;
                        num = num2;
                    }
                    this.tail = true;
                }
                #endregion
            }
            NPC head = Main.npc[NPC.FindFirstNPC(ModContent.NPCType<SkyDestroyerHead>())];
            Player player = Main.player[base.NPC.target];
            if (base.NPC.velocity.X < 0f)
            {
                base.NPC.spriteDirection = 1;
            }
            else if (base.NPC.velocity.X > 0f)
            {
                base.NPC.spriteDirection = -1;
            }
            base.NPC.localAI[1] = 0f;
            if (Main.player[base.NPC.target].dead)
            {
                base.NPC.TargetClosest(false);
            }
            base.NPC.alpha -= 42;
            if (base.NPC.alpha < 0)
            {
                base.NPC.alpha = 0;
            }
            float maxSpeed = this.speed;
            float turnAcc = this.turnSpeed;
            if (head.ai[1] == SkyDestroyerSegment.Plasmerizer)
            {
                if (head.ai[2] % 180 >= 80)
                {
                    NPC.WormMovement(Main.player[head.target].Center, maxSpeed, turnAcc);
                }
                else
                {
                    NPC.WormMovementEx(head.Center, maxSpeed, turnAcc);
                }
            }
            else
            {// vanilla crawlipede , with cooperation(vanilla)
                Vector2 vector = new Vector2(base.NPC.position.X + (float)base.NPC.width * 0.5f, base.NPC.position.Y + (float)base.NPC.height * 0.5f);
                float num9 = Main.player[base.NPC.target].position.X + (float)(Main.player[base.NPC.target].width / 2);
                float num10 = Main.player[base.NPC.target].position.Y + (float)(Main.player[base.NPC.target].height / 2);
                int num11 = -1;
                int num12 = (int)(Main.player[base.NPC.target].Center.X / 16f);
                int num13 = (int)(Main.player[base.NPC.target].Center.Y / 16f);
                for (int j = num12 - 2; j <= num12 + 2; j++)
                {
                    for (int k = num13; k <= num13 + 15; k++)
                    {
                        if (WorldGen.SolidTile2(j, k))
                        {
                            num11 = k;
                            break;
                        }
                    }
                    if (num11 > 0)
                    {
                        break;
                    }
                }
                if (num11 > 0)
                {
                    num11 *= 16;
                    float num14 = (float)(num11 - 200);
                    if ((Main.player[base.NPC.target].Center - base.NPC.Center).Length() > 200f)
                    {
                        num10 = num14;
                        if (Math.Abs(base.NPC.Center.X - Main.player[base.NPC.target].Center.X) < 250f)
                        {
                            if (base.NPC.velocity.X > 0f)
                            {
                                num9 = player.Center.X + 300f;
                            }
                            else
                            {
                                num9 = player.Center.X - 300f;
                            }
                        }
                    }
                }
                float num15 = maxSpeed * 1.3f;
                float num16 = maxSpeed * 0.7f;
                float num17 = base.NPC.velocity.Length();
                if (num17 > 0f)
                {
                    if (num17 > num15)
                    {
                        base.NPC.velocity.Normalize();
                        base.NPC.velocity *= num15;
                    }
                    else if (num17 < num16)
                    {
                        base.NPC.velocity.Normalize();
                        base.NPC.velocity *= num16;
                    }
                }
                if ((player.Center - base.NPC.Center).Length() > 200f)
                {
                    for (int l = 0; l < 200; l++)
                    {
                        if (Main.npc[l].active && Main.npc[l].type == NPC.type && l != base.NPC.whoAmI)
                        {
                            Vector2 value = Main.npc[l].Center - base.NPC.Center;
                            if (value.Length() < 400f)
                            {
                                value.Normalize();
                                value *= 1000f;
                                num9 -= value.X;
                                num10 -= value.Y;
                            }
                        }
                    }
                }
                else
                {
                    for (int m = 0; m < 200; m++)
                    {
                        if (Main.npc[m].active && Main.npc[m].type == base.NPC.type && m != base.NPC.whoAmI)
                        {
                            Vector2 value2 = Main.npc[m].Center - base.NPC.Center;
                            if (value2.Length() < 60f)
                            {
                                value2.Normalize();
                                value2 *= 200f;
                                num9 -= value2.X;
                                num10 -= value2.Y;
                            }
                        }
                    }
                }
                num9 = (float)((int)(num9 / 16f) * 16);
                num10 = (float)((int)(num10 / 16f) * 16);
                vector.X = (float)((int)(vector.X / 16f) * 16);
                vector.Y = (float)((int)(vector.Y / 16f) * 16);
                num9 -= vector.X;
                num10 -= vector.Y;
                float num18 = (float)Math.Sqrt((double)(num9 * num9 + num10 * num10));
                float num19 = Math.Abs(num9);
                float num20 = Math.Abs(num10);
                float num21 = maxSpeed / num18;
                num9 *= num21;
                num10 *= num21;
                if ((base.NPC.velocity.X > 0f && num9 > 0f) || (base.NPC.velocity.X < 0f && num9 < 0f) || (base.NPC.velocity.Y > 0f && num10 > 0f) || (base.NPC.velocity.Y < 0f && num10 < 0f))
                {
                    if (base.NPC.velocity.X < num9)
                    {
                        base.NPC.velocity.X = base.NPC.velocity.X + turnAcc;
                    }
                    else if (base.NPC.velocity.X > num9)
                    {
                        base.NPC.velocity.X = base.NPC.velocity.X - turnAcc;
                    }
                    if (base.NPC.velocity.Y < num10)
                    {
                        base.NPC.velocity.Y = base.NPC.velocity.Y + turnAcc;
                    }
                    else if (base.NPC.velocity.Y > num10)
                    {
                        base.NPC.velocity.Y = base.NPC.velocity.Y - turnAcc;
                    }
                    if ((double)Math.Abs(num10) < (double)maxSpeed * 0.2 && ((base.NPC.velocity.X > 0f && num9 < 0f) || (base.NPC.velocity.X < 0f && num9 > 0f)))
                    {
                        if (base.NPC.velocity.Y > 0f)
                        {
                            base.NPC.velocity.Y = base.NPC.velocity.Y + turnAcc * 2f;
                        }
                        else
                        {
                            base.NPC.velocity.Y = base.NPC.velocity.Y - turnAcc * 2f;
                        }
                    }
                    if ((double)Math.Abs(num9) < (double)maxSpeed * 0.2 && ((base.NPC.velocity.Y > 0f && num10 < 0f) || (base.NPC.velocity.Y < 0f && num10 > 0f)))
                    {
                        if (base.NPC.velocity.X > 0f)
                        {
                            base.NPC.velocity.X = base.NPC.velocity.X + turnAcc * 2f;
                        }
                        else
                        {
                            base.NPC.velocity.X = base.NPC.velocity.X - turnAcc * 2f;
                        }
                    }
                }
                else if (num19 > num20)
                {
                    if (base.NPC.velocity.X < num9)
                    {
                        base.NPC.velocity.X = base.NPC.velocity.X + turnAcc * 1.1f;
                    }
                    else if (base.NPC.velocity.X > num9)
                    {
                        base.NPC.velocity.X = base.NPC.velocity.X - turnAcc * 1.1f;
                    }
                    if ((double)(Math.Abs(base.NPC.velocity.X) + Math.Abs(base.NPC.velocity.Y)) < (double)maxSpeed * 0.5)
                    {
                        if (base.NPC.velocity.Y > 0f)
                        {
                            base.NPC.velocity.Y = base.NPC.velocity.Y + turnAcc;
                        }
                        else
                        {
                            base.NPC.velocity.Y = base.NPC.velocity.Y - turnAcc;
                        }
                    }
                }
                else
                {
                    if (base.NPC.velocity.Y < num10)
                    {
                        base.NPC.velocity.Y = base.NPC.velocity.Y + turnAcc * 1.1f;
                    }
                    else if (base.NPC.velocity.Y > num10)
                    {
                        base.NPC.velocity.Y = base.NPC.velocity.Y - turnAcc * 1.1f;
                    }
                    if ((double)(Math.Abs(base.NPC.velocity.X) + Math.Abs(base.NPC.velocity.Y)) < (double)maxSpeed * 0.5)
                    {
                        if (base.NPC.velocity.X > 0f)
                        {
                            base.NPC.velocity.X = base.NPC.velocity.X + turnAcc;
                        }
                        else
                        {
                            base.NPC.velocity.X = base.NPC.velocity.X - turnAcc;
                        }
                    }
                }
            }
            base.NPC.rotation = NPC.velocity.ToRotation();
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 3; i++)
            {
                Dust.NewDust(base.NPC.position, base.NPC.width, base.NPC.height, MyDustId.CyanShortFx1, (float)hitDirection, -1f, 0, default(Color), 1f);
            }
            if (base.NPC.life <= 0)
            {
                for (int j = 0; j < 10; j++)
                {
                    Dust.NewDust(base.NPC.position, base.NPC.width, base.NPC.height, MyDustId.ElectricCyan, (float)hitDirection, -1f, 0, default(Color), 1f);
                }
                Gore.NewGore(base.NPC.position, base.NPC.velocity, ModContent.Find<ModGore>("Gores/Martians/SDminionHeadGore").Type, 1f);
            }
        }
        public override bool CheckActive()
        {
            if (base.NPC.timeLeft <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = (int)base.NPC.ai[0]; i > 0; i = (int)Main.npc[i].ai[0])
                {
                    if (Main.npc[i].active)
                    {
                        Main.npc[i].active = false;
                        if (Main.netMode == NetmodeID.Server)
                        {
                            Main.npc[i].life = 0;
                            Main.npc[i].netSkip = -1;
                            NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, i, 0f, 0f, 0f, 0, 0, 0);
                        }
                    }
                }
            }
            return true;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D = Mod.Assets.Request<Texture2D>("TeaNPCAddon/Glow/NPCs/SkySearcherHeadGlow").Value;
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[base.NPC.type].Value;
            Color glowColor = Color.White;
            SpriteEffects effects = (NPC.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var mainColor = drawColor;
            spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Rectangle?(NPC.frame), mainColor * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            spriteBatch.Draw(texture2D, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Rectangle?(NPC.frame), glowColor * 0.75f * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            return false;
        }
        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Electrified, 120, true);
        }
        public const int minLength = 3;
        public const int maxLength = 4;
        public bool flies = true;
        public float speed = 12f;
        public float turnSpeed = 0.3f;
        public bool HeadA = false;
        public bool HeadA2 = false;
    }
}
