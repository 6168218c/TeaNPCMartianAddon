using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer
{
    public class SkySearcherBody : ModNPC
    {
        protected int segDistance => NPC.height;
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("天际搜寻者");
        }
        public override void SetDefaults()
        {
            base.NPC.damage = 50;
            base.NPC.width = 34;
            base.NPC.height = 32;
            base.NPC.defense = 100;
            base.NPC.lifeMax = 6000;
            base.NPC.aiStyle = -1;
            this.AIType = -1;
            base.NPC.knockBackResist = 0f;
            base.NPC.alpha = 255;
            for (int i = 0; i < base.NPC.buffImmune.Length; i++)
            {
                base.NPC.buffImmune[i] = true;
            }
            base.NPC.behindTiles = true;
            base.NPC.noGravity = true;
            base.NPC.noTileCollide = true;
            base.NPC.HitSound = SoundID.NPCHit4;
            base.NPC.DeathSound = SoundID.NPCDeath14;
            base.NPC.netAlways = true;
            base.NPC.dontCountMe = true;
            base.NPC.dontTakeDamage = true;
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return new bool?(false);
        }
        public override void AI()
        {
            if (base.NPC.ai[3] > 0f)
            {
                base.NPC.realLife = (int)base.NPC.ai[3];
            }
            if (base.NPC.target < 0 || base.NPC.target == 255 || Main.player[base.NPC.target].dead)
            {
                base.NPC.TargetClosest(true);
            }
            base.NPC.velocity.Length();
            if (base.NPC.velocity.X < 0f)
            {
                base.NPC.spriteDirection = 1;
            }
            else if (base.NPC.velocity.X > 0f)
            {
                base.NPC.spriteDirection = -1;
            }
            bool flag = false;
            if (base.NPC.ai[1] <= 0f)
            {
                flag = true;
            }
            else if (Main.npc[(int)base.NPC.ai[1]].life <= 0)
            {
                flag = true;
            }
            if (flag)
            {
                base.NPC.life = 0;
                base.NPC.HitEffect(0, 10.0);
                base.NPC.checkDead();
            }
            if (!NPC.AnyNPCs(ModContent.NPCType<SkySearcherHead>()))
            {
                base.NPC.active = false;
            }
            if (Main.npc[(int)base.NPC.ai[1]].alpha < 128)
            {
                base.NPC.alpha -= 42;
                if (base.NPC.alpha < 0)
                {
                    base.NPC.alpha = 0;
                }
            }
            int num = (int)(base.NPC.position.X / 16f) - 1;
            int num2 = (int)((base.NPC.position.X + (float)base.NPC.width) / 16f) + 2;
            int num3 = (int)(base.NPC.position.Y / 16f) - 1;
            int num4 = (int)((base.NPC.position.Y + (float)base.NPC.height) / 16f) + 2;
            if (num < 0)
            {
            }
            if (num2 > Main.maxTilesX)
            {
                num2 = Main.maxTilesX;
            }
            if (num3 < 0)
            {
            }
            if (num4 > Main.maxTilesY)
            {
                num4 = Main.maxTilesY;
            }
            if (Main.player[base.NPC.target].dead)
            {
                base.NPC.TargetClosest(false);
            }
            bool expertMode = Main.expertMode;
            /*if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                float num22 = 3f + (expertMode ? 2f : 0f);
                NPC.localAI[0] += num22;
                int type = ProjectileID.BrainScramblerBolt;
                int damage = expertMode ? 28 : 88;
                float num33 = 12f;
                if (NPC.localAI[0] >= (float)Main.rand.Next(2800, 32000))
                {
                    NPC.localAI[0] = 0f;
                    NPC.TargetClosest(true);
                    Vector2 vector2 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)(NPC.height / 2));
                    float num44 = Main.player[NPC.target].position.X + (float)Main.player[NPC.target].width * 0.5f - vector2.X + (float)Main.rand.Next(-20, 21);
                    float num55 = Main.player[NPC.target].position.Y + (float)Main.player[NPC.target].height * 0.5f - vector2.Y + (float)Main.rand.Next(-20, 21);
                    float num66 = (float)Math.Sqrt((double)(num44 * num44 + num55 * num55));
                    num66 = num33 / num66;
                    num44 *= num66;
                    num55 *= num66;
                    num44 += (float)Main.rand.Next(-5, 6) * 0.05f;
                    num55 += (float)Main.rand.Next(-5, 6) * 0.05f;
                    vector2.X += num44 * 5f;
                    vector2.Y += num55 * 5f;
                    int i = Projectile.NewProjectile(vector2.X, vector2.Y, num44, num55, type, damage, 0f, Main.myPlayer, 0f, 0f);
                    Main.projectile[i].tileCollide = false;
                }
            }*/
            Player player = Main.player[NPC.target];
            NPC previousSegment = Main.npc[(int)NPC.ai[1]];
            NPC head = Main.npc[NPC.realLife];

            if (NPC.Distance(previousSegment.Center) > 6)
            {
                Vector2 offset = new Vector2(0, 1f);
                try//default behavior
                {
                    offset = previousSegment.Center - NPC.Center;
                }
                catch { }
                if (offset == Vector2.Zero || offset.HasNaNs()) offset = new Vector2(0, 1f);
                var dist = segDistance * NPC.scale;
                NPC.rotation = offset.ToRotation();
                offset -= Vector2.Normalize(offset) * dist;
                NPC.velocity = Vector2.Zero;
                NPC.position += offset;
            }
        }
        public override bool CheckActive()
        {
            return false;
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
                    Dust.NewDust(base.NPC.position, base.NPC.width, base.NPC.height, MyDustId.CyanShortFx1, (float)hitDirection, -1f, 0, default(Color), 1f);
                }
                Gore.NewGore(base.NPC.position, base.NPC.velocity, ModContent.Find<ModGore>("Gores/Martians/SDminionBodyGore").Type, 1f);
            }
        }

        public override bool PreDraw(SpriteBatch spriteBatch,Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D = Mod.Assets.Request<Texture2D>("TeaNPCAddon/Glow/NPCs/SkySearcherBodyGlow").Value;
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[base.NPC.type].Value;
            Color glowColor = Color.White;
            SpriteEffects effects = (NPC.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var mainColor = drawColor;
            spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Rectangle?(NPC.frame), mainColor * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            spriteBatch.Draw(texture2D, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Rectangle?(NPC.frame), glowColor * 0.75f * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            return false;
        }
        public float speed = 6f;
        public float turnSpeed = 0.1f;
    }
}
