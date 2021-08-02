using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer;

namespace TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer
{
    public class SkyDestroyerBodyAlt : SkyDestroyerBody
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Sky Destroyer");
            //DisplayName.AddTranslation(GameCulture.Chinese, "天际毁灭者");
        }
        public override void SetDefaults()
        {
            NPC.damage = 100;
            NPC.width = 150;
            NPC.height = 150;
            NPC.defense = 100;
            this.Music = Mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CosmicSpace");
            NPC.lifeMax = 650000;
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
            if (!Main.npc[(int)NPC.ai[1]].active)
            {
                NPC.life = 0;
                NPC.HitEffect(0, 10.0);
                NPC.active = false;
                return;
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
            if (head.ai[1] == SpaceWarp || head.ai[1] == WarpMove || head.ai[1] == PlasmaWarpBlast || head.ai[1] == AntimatterBomb)
            {
                WarpAI();
            }
            else
            {
                if (NPC.Distance(previousSegment.Center) > 6)
                {
                    Vector2 offset = new Vector2(0, 1f);
                    try//default behavior
                    {
                        offset = previousSegment.Center - NPC.Center;
                    }
                    catch { }
                    if (offset == Vector2.Zero || offset.HasNaNs()) offset = new Vector2(0, 1f);
                    var dist = SegDistance * NPC.scale;
                    NPC.rotation = offset.ToRotation();
                    offset -= Vector2.Normalize(offset) * dist;
                    NPC.velocity = Vector2.Zero;
                    NPC.position += offset;
                }

                if (head.ai[1] == FireballBarrage)
                {
                    if (NPC.localAI[0] > 0)
                    {
                        NPC.localAI[0]--;
                        if (NPC.localAI[0] <= 0 && Main.netMode != NetmodeID.MultiplayerClient)
                        {
                            NPC.localAI[0] = 0;
                            if (NPC.ai[3] == 0)
                                Projectile.NewProjectile(NPC.GetProjectileSource(),NPC.Center, Vector2.UnitY * 10, ModContent.ProjectileType<SkyFireballLauncher>(),
                                    NPC.damage * 2 / 3, 0f, Main.myPlayer, NPC.target, 0);
                            else if (NPC.ai[3] == 1)
                                Projectile.NewProjectile(NPC.GetProjectileSource(),NPC.Center, -Vector2.UnitY * 10, ModContent.ProjectileType<SkyFireballLauncher>(),
                                    NPC.damage * 2 / 3, 0f, Main.myPlayer, NPC.target, 3);
                        }
                    }
                }
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return new bool?(false);
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch,Vector2 screenPos , Color drawColor)
        {
            //Mod mod = ModLoader.GetMod("TeaNPC");
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            Texture2D texture2D = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            texture = Terraria.GameContent.TextureAssets.Npc[NPC.type].Value;
            texture2D = Mod.Assets.Request<Texture2D>("Glow/NPCs/SkyDestroyerBodyAltGlow").Value;
            Color glowColor = Color.White;
            SpriteEffects effects = (NPC.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var mainColor = drawColor;
            spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Rectangle?(NPC.frame), mainColor * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            spriteBatch.Draw(texture2D, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Rectangle?(NPC.frame), glowColor * 0.75f * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            return false;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (NPC.life <= 0)
            {
                Gore.NewGore(NPC.position, NPC.velocity, ModContent.Find<ModGore>("Gores/Martians/SkydestroyerbodyAltGore").Type, 1f);
            }
            if (NPC.life <= 0)
            {
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

