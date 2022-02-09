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
using Terraria.GameContent;

namespace TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer
{
    public class SkyDestroyerBody : SkyDestroyerSegment
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
            NPC.defense = 150;
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
            NPC.chaseable = false;
            for (int i = 0; i < NPC.buffImmune.Length; i++)
            {
                NPC.buffImmune[i] = true;
            }
        }
        public static int SegDistance => 150;
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
                    }
                    return;
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
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {

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
                if (NPC.Center.HasNaNs()||float.IsInfinity(NPC.Center.X)||float.IsInfinity(NPC.Center.Y)) System.Diagnostics.Debugger.Break();
            }
        }
        public void ShowupAI()
        {
            NPC head = Main.npc[NPC.realLife];
            NPC prevSegment = Main.npc[(int)NPC.ai[1]];
            if (WarpState == 0)
            {
                if (prevSegment.localAI[2] != 1 && prevSegment.localAI[3] != WarpMark)
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
                if (NPC.DistanceSQ(mark.ProjAIToVector()) <= warpDistance * warpDistance * 0.1f)
                {
                    WarpState = 0;
                    NPC.Center = mark.Center;
                    //npc.alpha = 255;
                    NPC.alpha = 100;
                    NPC.hide = false;
                    NPC.scale = 1f;
                    NPC.dontTakeDamage = false;
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
                    var target = mark.ProjAIToVector() + NPC.DirectionFrom(mark.ProjAIToVector()) * (SegDistance * NPC.scale + warpDistance - prevSegment.Distance(mark.Center));
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
                    NPC.rotation = NPC.DirectionTo(mark.ProjAIToVector()).ToRotation();
                    NPC.Opacity = (NPC.Distance(mark.ProjAIToVector()) - warpDistance) / (SegDistance);
                }
            }
        }
        public void WarpAI(int tracingStrategy = 0)
        {
            NPC head = Main.npc[NPC.realLife];
            NPC prevSegment = Main.npc[(int)NPC.ai[1]];
            if (WarpState == 0)
            {
                if (NPC.Distance(prevSegment.Center) > 6)
                {
                    NormalSegmentAI(prevSegment);
                }
            }
            if (WarpState == 1)
            {
                Projectile mark = Main.projectile[WarpMark];
                if (NPC.DistanceSQ(mark.ProjAIToVector()) <= warpDistance * warpDistance)
                {
                    WarpState = 0;
                    NPC.Center = mark.Center;
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
                    var target = mark.ProjAIToVector() + NPC.DirectionFrom(mark.ProjAIToVector()) * (SegDistance * NPC.scale + warpDistance - prevSegment.Distance(mark.Center));
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
                    if (tracingStrategy == 0)
                        NPC.FastMovement(target, 5);
                    else if (tracingStrategy == 1)
                        NPC.Center = target;
                    else if (tracingStrategy == 2)
                        NPC.velocity = (mark.ProjAIToVector() - NPC.Center).SafeNormalize(Vector2.Zero) * head.velocity.Length();
                    NPC.rotation = NPC.DirectionTo(mark.ProjAIToVector()).ToRotation();
                    NPC.Opacity = (NPC.Distance(mark.ProjAIToVector()) -warpDistance) / (SegDistance);
                }
            }
        }
        public void WarpAIEx()
        {
            NPC head = Main.npc[NPC.realLife];
            NPC prevSegment = Main.npc[(int)NPC.ai[1]];
            if (WarpState == 0)
            {
                if (NPC.Distance(prevSegment.Center) > 6 && prevSegment.localAI[2] != 2)
                {
                    NormalSegmentAI(prevSegment);
                }
            }
            if (WarpState == 1)
            {
                Projectile mark = Main.projectile[WarpMark];
                if (NPC.DistanceSQ(mark.Center) <= warpDistance * warpDistance)
                {
                    WarpState = 2;
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
                    NPC.velocity = NPC.DirectionTo(mark.Center) * (head.ModNPC as SkyDestroyerHead).GetBodyMaxSpeed();
                    NPC.rotation = NPC.DirectionTo(mark.Center).ToRotation();
                    NPC.Opacity = (NPC.Distance(mark.Center) - warpDistance) / (SegDistance);
                }
            }
            else if (WarpState == 2)//awaiting
            {
                NPC.alpha = 255;
                Projectile mark = Main.projectile[WarpMark];
                NPC.velocity = Vector2.Zero;
                if (prevSegment.localAI[2] != 2)
                {
                    Vector2 end = mark.velocity;
                    if ((GetLinkPoint(prevSegment) - end).LengthSquared() >= SegDistance * SegDistance)
                    {
                        NPC.Center = mark.velocity;
                        WarpState = 0;
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
                }
                else
                {
                    NPC.alpha = 255;
                }
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
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
            Texture2D texture = TextureAssets.Npc[NPC.type].Value;
            Texture2D texture2D = TextureAssets.Npc[NPC.type].Value;
            texture = TextureAssets.Npc[NPC.type].Value;
            texture2D = Mod.RequestTexture("Glow/NPCs/SkyDestroyerBodyGlow");
            Color glowColor = NPC.GetAlpha(Color.White);
            SpriteEffects effects = (NPC.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var mainColor = NPC.GetAlpha(drawColor);
            spriteBatch.Draw(texture, GetDrawPosition(screenPos), new Rectangle?(NPC.frame), mainColor * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            spriteBatch.Draw(texture2D, GetDrawPosition(screenPos), new Rectangle?(NPC.frame), glowColor * 0.75f * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            return false;
        }
        public override bool StrikeNPC(ref double damage, int defense, ref float knockback, int hitDirection, ref bool crit)
        {
            if (NPC.realLife != -1 && Main.npc[NPC.realLife].type == ModContent.NPCType<SkyDestroyerHead>())
            {
                NPC head = Main.npc[NPC.realLife];
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
        public override bool PreKill()
        {
            return false;
        }
        public override bool CheckDead()
        {
            BodyPreventDeath();
            return NPC.life <= 0;
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
                    Gore.NewGore(NPC.position, NPC.velocity, base.Mod.GetGoreType("SkydestroyerbodyGore"), 1f);
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
        public int spawn;
    }
}

