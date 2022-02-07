using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using TeaNPCMartianAddon.Projectiles.Boss;
using System.IO;
using TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer;

namespace TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer
{
    public class SkyDestroyerBody : SkyDestroyerSegment
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Sky Destroyer");
            //DisplayName.AddTranslation(GameCulture.Chinese, "天际毁灭者");
        }
        public override void SetDefaults()
        {
            base.NPC.damage = 100;
            base.NPC.width = 150;
            base.NPC.height = 150;
            base.NPC.defense = 150;
            this.Music = Mod.GetSoundSlot(SoundType.Music, "Sounds/Music/BuryTheLight0");
            base.NPC.lifeMax = 650000;
            base.NPC.aiStyle = -1;
            this.AnimationType = 10;
            base.NPC.knockBackResist = 0f;
            base.NPC.alpha = 255;
            base.NPC.behindTiles = true;
            base.NPC.noGravity = true;
            base.NPC.noTileCollide = true;
            base.NPC.canGhostHeal = false;
            base.NPC.HitSound = SoundID.NPCHit4;
            base.NPC.DeathSound = SoundID.NPCDeath14;
            base.NPC.netAlways = true;
            base.NPC.chaseable = false;
            for (int i = 0; i < base.NPC.buffImmune.Length; i++)
            {
                base.NPC.buffImmune[i] = true;
            }
            NPC.hide = true;
        }
        public static int SegDistance => 150;
        public override void AI()
        {
            bool expertMode = Main.expertMode;
            Lighting.AddLight((int)((base.NPC.position.X + (float)(base.NPC.width / 2)) / 16f), (int)((base.NPC.position.Y + (float)(base.NPC.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
            if (!Main.npc[(int)base.NPC.ai[1]].active)
            {
                base.NPC.life = 0;
                base.NPC.HitEffect(0, 10.0);
                base.NPC.active = false;
                return;
            }
            if (base.NPC.target < 0 || base.NPC.target == 255 || Main.player[base.NPC.target].dead)
            {
                base.NPC.TargetClosest(true);
            }
            if (Main.npc[(int)base.NPC.ai[1]].alpha < 128)
            {
                if (base.NPC.alpha != 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        int num = Dust.NewDust(new Vector2(base.NPC.position.X, base.NPC.position.Y), base.NPC.width, base.NPC.height, MyDustId.RedBubble, 0f, 0f, 100, default(Color), 2f);
                        Main.dust[num].noGravity = true;
                        Main.dust[num].noLight = true;
                    }
                }
                base.NPC.alpha -= 42;
                if (base.NPC.alpha < 0)
                {
                    base.NPC.alpha = 0;
                }
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {

                }
            }
            Player player = Main.player[NPC.target];
            NPC previousSegment = Main.npc[(int)NPC.ai[1]];
            NPC head = Main.npc[NPC.realLife];
            if (head.ai[1] >= 0 && (head.ModNPC as SkyDestroyerHead).CurrentModule.ID > 1)
            {
                Music = Mod.GetSoundSlot(SoundType.Music, "Sounds/Music/BuryTheLight1");
            }
            if (head.ai[1] == -1)
            {
                ShowupAI();
            }
            else if (head.ai[1] == SpaceWarp || head.ai[1] == WarpMove || head.ai[1] == PlasmaWarpBlast || head.ai[1] == AntimatterBomb)
            {
                WarpAI();
            }
            else
            {
                NormalSegmentAI(previousSegment);

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
        public void NormalSegmentAI(NPC previousSegment)
        {
            Vector2 thislinkPoint = GetLinkPoint(NPC);
            Vector2 prevlinkPoint = GetLinkPoint(previousSegment);
            if (Vector2.Distance(thislinkPoint, prevlinkPoint) > 4)
            {
                Vector2 offset = new Vector2(0, 1f);
                try//default behavior
                {
                    offset = prevlinkPoint - thislinkPoint;
                }
                catch { }
                if (offset == Vector2.Zero || offset.HasNaNs()) offset = new Vector2(0, 1f);
                var dist = SegDistance * NPC.scale;
                offset -= Vector2.Normalize(offset) * dist;
                thislinkPoint += offset;
                NPC.velocity = Vector2.Zero;
                NPC.rotation = (prevlinkPoint - thislinkPoint).ToRotation();
                NPC.Center = (prevlinkPoint + thislinkPoint) / 2;
                if (NPC.Center.HasNaNs()) System.Diagnostics.Debugger.Break();
            }
        }
        public void ShowupAI()
        {
            NPC head = Main.npc[NPC.realLife];
            NPC prevSegment = Main.npc[(int)NPC.ai[1]];
            if (WarpState == 0)
            {
                if (prevSegment.localAI[2] == 0 && prevSegment.localAI[3] != WarpMark)
                {
                    WarpMark = (int)prevSegment.localAI[3];
                    WarpState = 1;
                }
                else if (NPC.Distance(prevSegment.Center) > 6)
                {
                    NormalSegmentAI(prevSegment);
                }
            }
            if (WarpState == 1)
            {
                Projectile mark = Main.projectile[WarpMark];
                if (NPC.DistanceSQ(mark.Center) <= warpDistance * warpDistance * 0.1f)
                {
                    WarpState = 0;
                    NPC.Center = mark.ProjAIToVector();
                    //npc.alpha = 255;
                    NPC.alpha = 100;
                    SetForeground();
                    if (NPC.type == ModContent.NPCType<SkyDestroyerTail>())
                    {
                        mark.localAI[1]++;
                    }
                    if (NPC.Distance(prevSegment.Center) > 6)
                    {
                        Vector2 offset = new Vector2(0, 1f);
                        try//default behavior
                        {
                            offset = prevSegment.Center - NPC.Center;
                        }
                        catch { }
                        if (offset == Vector2.Zero || offset.HasNaNs()) offset = new Vector2(0, 1f);
                        var dist = SegDistance * NPC.scale;
                        NPC.rotation = offset.ToRotation();
                        offset -= Vector2.Normalize(offset) * dist;
                        NPC.velocity = Vector2.Zero;
                        NPC.position += offset;//handle position first
                        NormalSegmentAI(prevSegment);
                    }
                }
                else
                {
                    //npc.Center = mark.Center + npc.DirectionFrom(mark.Center) * (segDistance*npc.scale + warpDistance - prevSegment.Distance(mark.ProjAIToVector()));
                    var target = mark.Center + NPC.DirectionFrom(mark.Center) * (SegDistance * NPC.scale + warpDistance - prevSegment.Distance(mark.ProjAIToVector()));
                    //npc.velocity = npc.DirectionTo(mark.Center) * head.velocity.Length();
                    /*if (npc.Distance(target) > head.velocity.Length())
                    {
                        npc.FastMovement(target);
                    }
                    else
                    {
                        npc.velocity = npc.DirectionTo(mark.Center) * head.velocity.Length();
                    }*/
                    //npc.velocity = npc.DirectionTo(mark.Center) * Math.Max(npc.Distance(target), head.velocity.Length());
                    NPC.FastMovement(target);
                    NPC.rotation = NPC.DirectionTo(mark.Center).ToRotation();
                    NPC.Opacity = (NPC.Distance(mark.Center) - warpDistance) / (SegDistance);
                }
            }
        }
        public void WarpAI()
        {
            NPC head = Main.npc[NPC.realLife];
            NPC prevSegment = Main.npc[(int)NPC.ai[1]];
            if (WarpState == 0)
            {
                if (prevSegment.localAI[2] == 0 && prevSegment.localAI[3] != WarpMark)
                {
                    WarpMark = (int)prevSegment.localAI[3];
                    WarpState = 1;
                }
                else if (NPC.Distance(prevSegment.Center) > 6)
                {
                    NormalSegmentAI(prevSegment);
                }
            }
            if (WarpState == 1)
            {
                Projectile mark = Main.projectile[WarpMark];
                if (NPC.DistanceSQ(mark.Center) <= warpDistance * warpDistance)
                {
                    WarpState = 0;
                    NPC.Center = mark.ProjAIToVector();
                    //npc.alpha = 255;
                    NPC.alpha = 100;
                    if (NPC.type == ModContent.NPCType<SkyDestroyerTail>())
                    {
                        mark.localAI[1]++;
                    }
                    if (NPC.Distance(prevSegment.Center) > 6)
                    {
                        Vector2 offset = new Vector2(0, 1f);
                        try//default behavior
                        {
                            offset = prevSegment.Center - NPC.Center;
                        }
                        catch { }
                        if (offset == Vector2.Zero || offset.HasNaNs()) offset = new Vector2(0, 1f);
                        var dist = SegDistance * NPC.scale;
                        NPC.rotation = offset.ToRotation();
                        offset -= Vector2.Normalize(offset) * dist;
                        NPC.velocity = Vector2.Zero;
                        NPC.position += offset;//handle position first
                        NormalSegmentAI(prevSegment);
                    }
                }
                else
                {
                    //npc.Center = mark.Center + npc.DirectionFrom(mark.Center) * (segDistance*npc.scale + warpDistance - prevSegment.Distance(mark.ProjAIToVector()));
                    var target = mark.Center + NPC.DirectionFrom(mark.Center) * (SegDistance * NPC.scale + warpDistance - prevSegment.Distance(mark.ProjAIToVector()));
                    //npc.velocity = npc.DirectionTo(mark.Center) * head.velocity.Length();
                    /*if (npc.Distance(target) > head.velocity.Length())
                    {
                        npc.FastMovement(target);
                    }
                    else
                    {
                        npc.velocity = npc.DirectionTo(mark.Center) * head.velocity.Length();
                    }*/
                    //npc.velocity = npc.DirectionTo(mark.Center) * Math.Max(npc.Distance(target), head.velocity.Length());
                    NPC.FastMovement(target);
                    NPC.rotation = NPC.DirectionTo(mark.Center).ToRotation();
                    NPC.Opacity = (NPC.Distance(mark.Center) - warpDistance) / (SegDistance);
                }
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(base.NPC.localAI[0]);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.NPC.localAI[0] = reader.ReadSingle();
            base.ReceiveExtraAI(reader);
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
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[base.NPC.type].Value;
            Texture2D texture2D = Terraria.GameContent.TextureAssets.Npc[base.NPC.type].Value;
            texture = Terraria.GameContent.TextureAssets.Npc[base.NPC.type].Value;
            texture2D = Mod.Assets.Request<Texture2D>("Glow/NPCs/SkyDestroyerBodyGlow").Value;
            Color glowColor = NPC.GetAlpha(Color.White);
            SpriteEffects effects = (base.NPC.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var mainColor = NPC.GetAlpha(drawColor);
            spriteBatch.Draw(texture, base.NPC.Center - screenPos + new Vector2(0f, base.NPC.gfxOffY), new Rectangle?(base.NPC.frame), mainColor * base.NPC.Opacity, base.NPC.rotation + MathHelper.Pi / 2, base.NPC.frame.Size() / 2f, base.NPC.scale, effects, 0f);
            spriteBatch.Draw(texture2D, base.NPC.Center - screenPos + new Vector2(0f, base.NPC.gfxOffY), new Rectangle?(base.NPC.frame), glowColor * 0.75f * base.NPC.Opacity, base.NPC.rotation + MathHelper.Pi / 2, base.NPC.frame.Size() / 2f, base.NPC.scale, effects, 0f);
            return false;
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (base.NPC.realLife != -1 && Main.npc[base.NPC.realLife].type == ModContent.NPCType<SkyDestroyerHead>())
            {
                NPC head = Main.npc[base.NPC.realLife];
                /*if (head.life <= head.lifeMax * LumiteDestroyerArguments.Phase2HealthFactor)
                {
                    if (head.ai[1] >= DivideAttackStart && head.ai[1] <= DivideAttackStart + DivideAILength)
                    {
                        damage *= (1 - 0.80);
                    }
                    else
                    {
                        damage *= (1 - 0.75);
                    }
                }*/
                damage *= (1 - (head.ModNPC as SkyDestroyerHead).DynDR);
            }
            return true;
        }
        public override void HitEffect(int hitDirection, double damage)
        {
            if (base.NPC.life <= 0)
            {
                Gore.NewGore(base.NPC.position, base.NPC.velocity, ModContent.Find<ModGore>("Gores/Martians/SkydestroyerbodyGore").Type, 1f);
            }
            if (base.NPC.life <= 0)
            {
                for (int i = 0; i < 20; i++)
                {
                    int num = Dust.NewDust(new Vector2(base.NPC.position.X, base.NPC.position.Y), base.NPC.width, base.NPC.height, MyDustId.CyanShortFx1, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num].velocity *= 3f;
                    if (Main.rand.Next(2) == 0)
                    {
                        Main.dust[num].scale = 0.5f;
                        Main.dust[num].fadeIn = 1f + (float)Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }
        public int spawn;
    }
}

