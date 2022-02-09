using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.GameContent;
using TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer;

namespace TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer
{
    public class SkyDestroyerBodyAlt : SkyDestroyerBody
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Sky Destroyer");
            //DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "天际毁灭者");
        }
        public override void SetDefaults()
        {
            NPC.damage = 100;
            NPC.width = 150;
            NPC.height = 150;
            NPC.defense = 100;
            this.Music = MusicLoader.GetMusicSlot($"{nameof(TeaNPCMartianAddon)}/Sounds/Music/BuryTheLight");
            NPC.lifeMax = baseMaxLife;
            NPC.aiStyle = -1;
            this.AnimationType = 10;
            NPC.knockBackResist = 0f;
            NPC.alpha = 255;
            NPC.behindTiles = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.canGhostHeal = false;
            NPC.HitSound = SoundID.NPCHit4;
            NPC.DeathSound = SoundID.NPCDeath14;
            NPC.netAlways = true;
            for (int i = 0; i < NPC.buffImmune.Length; i++)
            {
                NPC.buffImmune[i] = true;
            }
        }
        public override void AI()
        {
            bool expertMode = Main.expertMode;
            Lighting.AddLight((int)((NPC.position.X + (float)(NPC.width / 2)) / 16f), (int)((NPC.position.Y + (float)(NPC.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
            if (!Main.npc[(int)NPC.ai[1]].active || !Util.CheckNPCAlive<SkyDestroyerHead>(NPC.realLife))
            {
                NPC.life = 0;
                if (NPC.ai[3] < 0)
                    NPC.HitEffect(0, 10.0);
                NPC.checkDead();
                return;
            }
            if (Main.npc[(int)NPC.realLife].ai[1] < DeathAnimation0)
            {
                SetViberation(false);
            }
            else if (Main.npc[NPC.realLife].ai[1] >= DeathAnimation1)
            {
                NPC hd = Main.npc[NPC.realLife];
                if (NPC.ai[3] > 0)
                {
                    NPC.ai[3]--;
                    //viberation
                    SetViberation();

                    if (NPC.ai[3] == 0)
                    {
                        NPC.ai[3] = -1;
                        NPC.life = 0;
                        NPC.HitEffect(0, 10.0);
                        NPC.checkDead();
                        return;
                    }
                }
            }
            if (NPC.target < 0 || NPC.target == 255 || Main.player[NPC.target].dead)
            {
                NPC.TargetClosest(true);
            }
            if (Main.npc[(int)NPC.ai[1]].alpha < 128)
            {
                if (NPC.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int num = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, MyDustId.RedBubble, 0f, 0f, 100, default(Color), 2f);
                        Main.dust[num].noGravity = true;
                        Main.dust[num].noLight = true;
                    }
                }
                NPC.alpha -= 42;
                if (NPC.alpha < 0)
                {
                    NPC.alpha = 0;
                }
            }
            Player player = Main.player[NPC.target];
            NPC previousSegment = Main.npc[(int)NPC.ai[1]];
            NPC head = Main.npc[NPC.realLife];
            if (head.ai[1] == -1)
            {
                ShowupAI();
            }
            else
            {
                if (WarpState == 0)
                {
                    NPC prevSegment = Main.npc[(int)NPC.ai[1]];
                    if (prevSegment.localAI[2] != 1 && prevSegment.localAI[3] != WarpMark)
                    {
                        WarpMark = (int)prevSegment.localAI[3];
                        WarpState = 1;
                    }
                }

                if (Util.CheckProjAlive(WarpMark))
                {
                    if (Util.CheckProjAlive<SkyWarpMarkEx>(WarpMark))
                    {
                        WarpAIEx();
                    }
                    else
                    {
                        WarpAI();
                    }
                }
                else
                {
                    WarpAI();
                }
            }
            //if (npc.Center.HasNaNs() || float.IsInfinity(npc.Center.X) || float.IsInfinity(npc.Center.Y)) System.Diagnostics.Debugger.Break();
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return new bool?(false);
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Mod mod = ModLoader.GetMod("TeaNPC");
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D texture2D = TextureAssets.Npc[NPC.type].Value;
            texture = TextureAssets.Npc[NPC.type].Value;
            texture2D = Mod.RequestTexture("Glow/NPCs/SkyDestroyerBodyAltGlow");
            Color glowColor = GetGlowColor();
            SpriteEffects effects = (NPC.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var mainColor = NPC.GetAlpha(drawColor);
            spriteBatch.Draw(texture, GetDrawPosition(screenPos), new Rectangle?(NPC.frame), mainColor * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            spriteBatch.Draw(texture2D, GetDrawPosition(screenPos), new Rectangle?(NPC.frame), glowColor * 0.75f * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            return false;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, MyDustId.CyanShortFx1, (float)hitDirection, -1f, 0, default(Color), 1f);
            }
            if (NPC.life <= 0)
            {
                BodyPreventDeath();
                if (NPC.life <= 0)
                {
                    Gore.NewGore(NPC.position, NPC.velocity, Mod.GetGoreType("SkydestroyerbodyAltGore"), 1f);
                    for (int i = 0; i < 20; i++)
                    {
                        int num = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, MyDustId.CyanShortFx1, 0f, 0f, 100, default(Color), 2f);
                        Main.dust[num].velocity *= 3f;
                        if (Main.rand.Next(2) == 0)
                        {
                            Main.dust[num].scale = 0.5f;
                            Main.dust[num].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                        }
                    }
                }
            }
        }
    }
}

