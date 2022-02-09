using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using TeaNPCMartianAddon.Effects;
using TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer;
using System.IO;
using Terraria.Enums;
using System.Collections.Generic;
using System.Linq;
using Terraria.Localization;

namespace TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer
{
    /// <summary>
    /// Warp worm, set portals[1] in projectile.Center + projectile.velocity
    /// </summary>
    public class SkyWarpWorm:ModProjectile
    {
        static readonly string _texPrefix = "NPCs/Bosses/SkyDestroyer/SkySearcher";
        static readonly string _glowPrefix = "Glow/NPCs/SkySearcher";
        static readonly string _gorePrefix = "SDminion";
        public static string HeadTex => _texPrefix + "Head";
        public static string BodyTex => _texPrefix + "Body";
        public static string BodyAltTex => _texPrefix + "BodyAlt";
        public static string TailTex => _texPrefix + "Tail";
        public static string HeadGore => _gorePrefix + "HeadGore";
        public static string BodyGore => _gorePrefix + "BodyGore";
        public static string BodyAltGore => _gorePrefix + "BodyAltGore";
        public static string TailGore => _gorePrefix + "TailGore";
        public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.VortexVortexPortal;
        enum SegmentType
        {
            Head, Body, BodyAlt, Tail
        }
        static readonly string[] SegmentTypeNames = new string[] { "Head", "Body", "BodyAlt", "Tail" };
        class WormSegment
        {
            public SegmentType SegmentType { get; set; }
            public Vector2 Center 
            {
                get => new Vector2(position.X + (float)width / 2, position.Y + (float)height / 2);
                set { position.X = value.X - (float)width / 2;position.Y = value.Y - (float)height / 2; }
            }
            public string Texture => _texPrefix + SegmentTypeNames[(int)SegmentType];
            public string GlowTex => _glowPrefix + SegmentTypeNames[(int)SegmentType] + "Glow";
            public string GoreTex => _gorePrefix + SegmentTypeNames[(int)SegmentType] + "Gore";
            public float rotation;
            public float scale = 1.8f;
            public int width = 34;
            public int height = 48;
            public Vector2 position;
            public int alpha = 0;
            public float Opacity
            {
                get => (float)(255 - alpha) / 255;
                set => alpha = (int)(1 - value) * 255;
            }

            public int state;
        }

        List<WormSegment> segments = new List<WormSegment>();
        Vector2[] portals = new Vector2[2];
        float[] scale = new float[2];
        float[] alpha = new float[2];
        float[] rotation = new float[2];
        float[] extraAI = new float[2];

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 15;
            Projectile.aiStyle = -1;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;

            Projectile.extraUpdates = 0;
            CooldownSlot = 1;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 999999;
        }
        public override void AI()
        {
            Projectile.Loomup();
            if (segments.Count == 0)
            {
                CreateSegments();
                portals[0] = Projectile.Center;
                portals[1] = Projectile.Center + Projectile.velocity;
                alpha[0] = alpha[1] = 255;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 12;
            }
            if (Projectile.localAI[1] == 1)//death animation means no gore
            {
                Projectile.ai[1] = 0;
                portals[1] = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * 100;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 12;
            }

            //Update position
            UpdateSegments();

            #region Portal Visuals
            void VisualAI(int index, Vector2 Center, bool fading)
            {
                if (extraAI[index] > 135) return;
                extraAI[index]++;
                if (fading)
                {
                    extraAI[index] = Math.Max(extraAI[index], 71);
                }
                if (extraAI[index] <= 40f)
                {
                    scale[index] = extraAI[index] / 40f;
                    alpha[index] = 255 - (int)(255f * scale[index]);
                    rotation[index] -= (float)Math.PI / 20f;
                    scale[index] *= 2f;

                    if (Main.rand.Next(2) == 0)
                    {
                        Vector2 vector70 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust23 = Main.dust[Dust.NewDust(Center - vector70 * 30f, 0, 0, DustID.Vortex)];
                        dust23.noGravity = true;
                        dust23.position = Center - vector70 * Main.rand.Next(10, 21);
                        dust23.velocity = vector70.RotatedBy(1.5707963705062866) * 6f;
                        dust23.scale = 0.5f + Main.rand.NextFloat();
                        dust23.fadeIn = 0.5f;
                        dust23.customData = Center;
                    }

                    if (Main.rand.Next(2) == 0)
                    {
                        Vector2 vector71 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust24 = Main.dust[Dust.NewDust(Center - vector71 * 30f, 0, 0, DustID.Granite)];
                        dust24.noGravity = true;
                        dust24.position = Center - vector71 * 30f;
                        dust24.velocity = vector71.RotatedBy(-1.5707963705062866) * 3f;
                        dust24.scale = 0.5f + Main.rand.NextFloat();
                        dust24.fadeIn = 0.5f;
                        dust24.customData = Center;
                    }
                }
                else if (extraAI[index] <= 70f)
                {
                    if (!fading && extraAI[index] == 70f)
                        extraAI[index]--;
                    else if(fading)
                        extraAI[index] = Math.Max(extraAI[index], 70);
                    scale[index] = 1f;
                    alpha[index] = 0;
                    rotation[index] -= (float)Math.PI / 60f;
                    scale[index] *= 2f;
                    if (Main.rand.Next(2) == 0)
                    {
                        Vector2 vector78 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust30 = Main.dust[Dust.NewDust(Center - vector78 * 30f, 0, 0, DustID.Vortex)];
                        dust30.noGravity = true;
                        dust30.position = Center - vector78 * Main.rand.Next(10, 21);
                        dust30.velocity = vector78.RotatedBy(1.5707963705062866) * 6f;
                        dust30.scale = 0.5f + Main.rand.NextFloat();
                        dust30.fadeIn = 0.5f;
                        dust30.customData = Center;
                    }
                    else
                    {
                        Vector2 vector79 = Vector2.UnitY.RotatedByRandom(6.2831854820251465);
                        Dust dust31 = Main.dust[Dust.NewDust(Center - vector79 * 30f, 0, 0, DustID.Granite)];
                        dust31.noGravity = true;
                        dust31.position = Center - vector79 * 30f;
                        dust31.velocity = vector79.RotatedBy(-1.5707963705062866) * 3f;
                        dust31.scale = 0.5f + Main.rand.NextFloat();
                        dust31.fadeIn = 0.5f;
                        dust31.customData = Center;
                    }
                }
                else
                {
                    scale[index] = Math.Min(1f - (extraAI[index] - 70f) / 60f, 0);
                    alpha[index] = 255 - (int)(255f * scale[index]);
                    rotation[index] -= (float)Math.PI / 30f;
                    scale[index] *= 2f;
                    for (int num840 = 0; num840 < 2; num840++)
                    {
                        switch (Main.rand.Next(3))
                        {
                            case 0:
                                {
                                    Vector2 vector83 = Vector2.UnitY.RotatedByRandom(6.2831854820251465) * scale[index];
                                    Dust dust35 = Main.dust[Dust.NewDust(Center - vector83 * 30f, 0, 0, DustID.Vortex)];
                                    dust35.noGravity = true;
                                    dust35.position = Center - vector83 * Main.rand.Next(10, 21);
                                    dust35.velocity = vector83.RotatedBy(1.5707963705062866) * 6f;
                                    dust35.scale = 0.5f + Main.rand.NextFloat();
                                    dust35.fadeIn = 0.5f;
                                    dust35.customData = Center;
                                    break;
                                }
                            case 1:
                                {
                                    Vector2 vector82 = Vector2.UnitY.RotatedByRandom(6.2831854820251465) * scale[index];
                                    Dust dust34 = Main.dust[Dust.NewDust(Center - vector82 * 30f, 0, 0, DustID.Granite)];
                                    dust34.noGravity = true;
                                    dust34.position = Center - vector82 * 30f;
                                    dust34.velocity = vector82.RotatedBy(-1.5707963705062866) * 3f;
                                    dust34.scale = 0.5f + Main.rand.NextFloat();
                                    dust34.fadeIn = 0.5f;
                                    dust34.customData = Center;
                                    break;
                                }
                        }
                    }
                }
            }
            if (Projectile.ai[1] == 2 || Projectile.ai[1] == 3)
            {
                VisualAI(0, portals[0], segments.Last().state > 0);
            }
            else
            {
                bool despawning = segments.Last().state == 2 && Projectile.ai[1] != 1;
                VisualAI(0, portals[0], despawning);
                VisualAI(1, portals[1], despawning);
                alpha[0] = Math.Min(alpha[0], 255);
                alpha[1] = Math.Min(alpha[1], 255);
                if (alpha[0] >= 255 && alpha[1] >= 255&&despawning)
                    Projectile.Kill();
            }
            #endregion
        }

        void CreateSegments()
        {
            segments.Clear();
            segments.Add(new WormSegment() { SegmentType = SegmentType.Head,Center=Projectile.Center });
            for (int i = 0; i < 14; i++)
            {
                segments.Add(new WormSegment() { SegmentType = i % 2 == 0 ? SegmentType.BodyAlt : SegmentType.Body, Center = Projectile.Center });
            }
            segments.Add(new WormSegment() { SegmentType = SegmentType.Tail, Center = Projectile.Center });
        }
        void UpdateSegments()
        {
            if (segments.Count == 0)
            {
                CreateSegments();
            }
            //head AI
            if (Vector2.Distance(segments[0].Center, portals[0]) > 20)
            {
                segments[0].alpha = 0;
                segments[0].state = 1;
            }
            if (segments[0].state == 1 && Projectile.ai[1] == 0)
            {
                Projectile.velocity = (portals[1] - Projectile.Center).SafeNormalize(Vector2.UnitX) * 18f;
                if(Vector2.Distance(segments[0].Center, portals[1]) < 20)
                {
                    segments[0].alpha = 255;
                    segments[0].state = 2;
                }
            }
            if (Projectile.ai[1] == 1)
            {
                if (Util.CheckNPCAlive<SkyDestroyerHead>((int)Projectile.ai[0]))
                {
                    NPC head = Main.npc[(int)Projectile.ai[0]];
                    if (head.ai[1] != SkyDestroyerSegment.WarpMove)
                    {
                        Projectile.ai[1] = 0;
                    }
                    else if (segments[0].state == 2)
                    {
                        Projectile.Center = portals[0];
                        segments[0].state = 0;//cycle
                    }
                }
                else
                {
                    Projectile.ai[1] = 0;//back to normal
                }
            }
            else if (Projectile.ai[1] == 2)
            {
                portals[1] = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * 100;
                Projectile.localAI[0]++;
                if (Projectile.localAI[0] >= Projectile.ai[0])
                {
                    Projectile.ai[1] = 0;
                    portals[1] = Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * 100;
                    Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.UnitY) * 12;
                }
                if (Projectile.localAI[0] % 5 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero) * 9,
                        ModContent.ProjectileType<SkyFireThrower>(), Projectile.damage / 2, 0f, Main.myPlayer);
                }
                int player = Player.FindClosest(segments[0].position, segments[0].width, segments[0].height);
                if (player != -1)
                {
                    Projectile.WormMovement(Main.player[player].Center, 13.5f);
                }
            }
            else if (Projectile.ai[1] == 3)
            {
                int player = Player.FindClosest(segments[0].position, segments[0].width, segments[0].height);
                if (player != -1)
                {
                    Projectile.WormMovement(Main.player[player].Center, 18f);
                }
            }
            segments[0].Center = Projectile.Center + Projectile.velocity;
            segments[0].rotation = Projectile.velocity.ToRotation();
            for (int i = 1; i < segments.Count; i++)
            {
                //segment AI
                WormSegment prevSegment = segments[i - 1];
                if (segments[i].state == 0)
                {
                    if (Vector2.DistanceSquared(segments[i].Center, portals[0]) > 20)
                    {
                        segments[i].alpha = 0;
                        segments[i].state = 1;
                    }
                    if (Vector2.Distance(segments[i].Center, prevSegment.Center) > segments[i].height)
                    {
                        Vector2 offset = new Vector2(0, 1f);
                        try//default behavior
                        {
                            offset = prevSegment.Center - segments[i].Center;
                        }
                        catch { }
                        if (offset == Vector2.Zero || offset.HasNaNs()) offset = new Vector2(0, 1f);
                        var dist = segments[i].height * Projectile.scale;
                        segments[i].rotation = offset.ToRotation();
                        offset -= Vector2.Normalize(offset) * dist;
                        segments[i].Center += offset;//handle position first
                    }
                }
                else if (Projectile.ai[1] < 3 && prevSegment.state == 2 && segments[i].state != 2)
                {
                    float dist = Vector2.Distance(segments[i].Center, portals[1]);
                    dist = Math.Max(dist - 9f, 0);
                    segments[i].Center = portals[1] + (segments[i].Center - portals[1]).SafeNormalize(Vector2.Zero) * dist;
                    if (dist < 20)
                    {
                        segments[i].alpha = 255;
                        segments[i].state = 2;
                        if (i == segments.Count - 1)
                        {
                            Projectile.Kill();
                        }
                    }
                }
                else
                {
                    //track prev
                    if (Vector2.Distance(segments[i].Center, prevSegment.Center) > 6)
                    {
                        Vector2 offset = new Vector2(0, 1f);
                        try//default behavior
                        {
                            offset = prevSegment.Center - segments[i].Center;
                        }
                        catch { }
                        if (offset == Vector2.Zero || offset.HasNaNs()) offset = new Vector2(0, 1f);
                        var dist = segments[i].height * Projectile.scale;
                        segments[i].rotation = offset.ToRotation();
                        offset -= Vector2.Normalize(offset) * dist;
                        segments[i].Center += offset;//handle position first
                    }
                }

                if (Projectile.ai[1] == 1)
                {
                    if (Util.CheckNPCAlive<SkyDestroyerHead>((int)Projectile.ai[0]))
                    {
                        if (segments[i].state == 2)
                        {
                            segments[i].Center = portals[0];
                            segments[i].state = 0;//cycle
                        }
                    }
                }
            }

        }
        public override void Kill(int timeLeft)
        {
            foreach(var segment in segments)
            {
                if (segment.alpha == 0)
                {
                    Vector2 velo = Vector2.Zero;
                    if (Projectile.ai[1] == 3)
                    {
                        float distance = 999999f;
                        int index = 0;
                        for(int i = 0; i < Main.maxNPCs; i++)
                        {
                            if(Util.CheckNPCAlive<SkyDestroyerHead>(i)
                                ||Util.CheckNPCAlive<SkyDestroyerBody>(i)
                                ||Util.CheckNPCAlive<SkyDestroyerBodyAlt>(i)
                                || Util.CheckNPCAlive<SkyDestroyerTail>(i))
                            {
                                float dist = Main.npc[i].Distance(segment.Center);
                                if (dist < distance)
                                {
                                    distance = dist;
                                    index = i;
                                }
                            }
                        }
                        velo = (Main.npc[index].Center - segment.Center).SafeNormalize(Vector2.Zero) / 30;
                    }
                    Gore.NewGore(segment.Center, velo, Mod.GetGoreType(segment.GoreTex));
                }
            }
            base.Kill(timeLeft);
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.WriteVector2(portals[0]);
            writer.WriteVector2(portals[1]);
            base.SendExtraAI(writer);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            portals[0] = reader.ReadVector2();
            portals[1] = reader.ReadVector2();
            base.ReceiveExtraAI(reader);
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            foreach(var seg in segments)
            {
                if (seg.alpha != 0) continue;
                Rectangle hitbox = Utils.CenteredRectangle(seg.Center, new Vector2(seg.width, seg.height));
                if (hitbox.Intersects(targetHitbox)) return true;
            }
            return false;
        }
        public float GetOpacity(int index)
        {
            return 1 - (float)alpha[index] / 255;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            SpriteBatch spriteBatch=Main.spriteBatch;
            if (segments.Count != 0)
            {
                foreach(var segment in segments)
                {
                    Texture2D texture2D = Mod.RequestTexture(segment.Texture);
                    Texture2D texture = Mod.RequestTexture(segment.GlowTex);
                    Color glowColor = Color.White;
                    var mainColor = lightColor;
                    spriteBatch.Draw(texture, segment.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, mainColor * segment.Opacity, segment.rotation + MathHelper.Pi / 2, texture2D.Size() / 2f, segment.scale, SpriteEffects.None, 0);
                    spriteBatch.Draw(texture2D, segment.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, glowColor * 0.75f * segment.Opacity, segment.rotation + MathHelper.Pi / 2, texture.Size() / 2f, segment.scale, SpriteEffects.None, 0);
                }
            }

            Texture2D texture2D13 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value;
            int num156 = Terraria.GameContent.TextureAssets.Projectile[Projectile.type].Value.Height / Main.projFrames[Projectile.type]; //ypos of lower right corner of sprite to draw
            int y3 = num156 * Projectile.frame; //ypos of upper left corner of sprite to draw
            Rectangle rectangle = new Rectangle(0, y3, texture2D13.Width, num156);
            Vector2 origin2 = rectangle.Size() / 2f;
            Main.spriteBatch.Draw(texture2D13, portals[0] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * GetOpacity(0), -rotation[0], origin2, scale[0] * 1.25f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, portals[0] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * GetOpacity(0), rotation[0], origin2, scale[0], SpriteEffects.None, 0f);
			Main.spriteBatch.Draw(texture2D13, portals[1] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.Black * GetOpacity(1), -rotation[1], origin2, scale[1] * 1.25f, SpriteEffects.FlipHorizontally, 0f);
			Main.spriteBatch.Draw(texture2D13, portals[1] - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), Color.White * GetOpacity(1), rotation[1], origin2, scale[1], SpriteEffects.None, 0f);
            return false;
        }
    }
}
