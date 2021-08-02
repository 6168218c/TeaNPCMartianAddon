using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.Graphics.Effects;
using Terraria.Localization;
using Terraria.DataStructures;


namespace TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer
{
    public static class Util
    {
        public static void SlowDown(this Entity entity, float accle)
        {
            entity.velocity.X *= accle;
            entity.velocity.Y *= accle;
            if (entity.velocity.X > -0.1 && entity.velocity.X < 0.1)
            {
                entity.velocity.X = 0f;
            }
            if (entity.velocity.Y > -0.1 && entity.velocity.Y < 0.1)
            {
                entity.velocity.Y = 0f;
            }
        }
        public static void HoverMovement(this Entity entity, Vector2 dest, float maxSpeed, float accle)
        {
            Vector2 velo = dest - entity.Center;
            velo = velo.SafeNormalize(Vector2.Zero) * maxSpeed;
            if (entity.velocity.X < velo.X)
            {
                entity.velocity.X += accle;
                if (entity.velocity.X < 0f && velo.X > 0f)
                {
                    entity.velocity.X += accle;
                }
            }
            else if (entity.velocity.X > velo.X)
            {
                entity.velocity.X -= accle;
                if (entity.velocity.X > 0f && velo.X < 0f)
                {
                    entity.velocity.X -= accle;
                }
            }
            if (entity.velocity.Y < velo.Y)
            {
                entity.velocity.Y += accle;
                if (entity.velocity.Y < 0f && velo.Y > 0f)
                {
                    entity.velocity.Y += accle;
                }
            }
            else if (entity.velocity.Y > velo.Y)
            {
                entity.velocity.Y -= accle;
                if (entity.velocity.Y > 0f && velo.Y < 0f)
                {
                    entity.velocity.Y -= accle;
                }
            }
        }
        public static void HoverMovementEx(this Entity entity, Vector2 dest, float maxSpeed, float accle)
        {
            Vector2 velo = dest - entity.Center - entity.velocity;
            velo = velo.SafeNormalize(Vector2.Zero) * maxSpeed;
            if (entity.velocity.X < velo.X)
            {
                entity.velocity.X += accle;
                if (entity.velocity.X < 0f && velo.X > 0f)
                {
                    entity.velocity.X += accle;
                }
            }
            else if (entity.velocity.X > velo.X)
            {
                entity.velocity.X -= accle;
                if (entity.velocity.X > 0f && velo.X < 0f)
                {
                    entity.velocity.X -= accle;
                }
            }
            if (entity.velocity.Y < velo.Y)
            {
                entity.velocity.Y += accle;
                if (entity.velocity.Y < 0f && velo.Y > 0f)
                {
                    entity.velocity.Y += accle;
                }
            }
            else if (entity.velocity.Y > velo.Y)
            {
                entity.velocity.Y -= accle;
                if (entity.velocity.Y > 0f && velo.Y < 0f)
                {
                    entity.velocity.Y -= accle;
                }
            }
        }
        public static void WormMovement(this Entity entity, Vector2 dest, float maxSpeed, float turnAccle = 0.1f, float ramAccle = 0.15f)
        {
            Vector2 targetVector = dest - entity.Center;
            targetVector = targetVector.SafeNormalize(Vector2.Zero) * maxSpeed;
            if ((targetVector.X * entity.velocity.X > 0f) && (targetVector.Y * entity.velocity.Y > 0f)) //acclerate
            {
                entity.velocity.X += Math.Sign(targetVector.X - entity.velocity.X) * ramAccle;
                entity.velocity.Y += Math.Sign(targetVector.Y - entity.velocity.Y) * ramAccle;
            }
            if ((targetVector.X * entity.velocity.X > 0f) || (targetVector.Y * entity.velocity.Y > 0f)) //turn
            {
                entity.velocity.X += Math.Sign(targetVector.X - entity.velocity.X) * turnAccle;
                entity.velocity.Y += Math.Sign(targetVector.Y - entity.velocity.Y) * turnAccle;

                if (Math.Abs(targetVector.Y) < maxSpeed * 0.2 && targetVector.X * entity.velocity.X < 0)
                {
                    entity.velocity.Y += Math.Sign(entity.velocity.Y) * turnAccle * 2f;
                }

                if (Math.Abs(targetVector.X) < maxSpeed * 0.2 && targetVector.Y * entity.velocity.Y < 0)
                {
                    entity.velocity.X += Math.Sign(entity.velocity.X) * turnAccle * 2f;
                }
            }
            else if (Math.Abs(targetVector.X) > Math.Abs(targetVector.Y))
            {
                entity.velocity.X += Math.Sign(targetVector.X - entity.velocity.X) * turnAccle * 1.1f;
                if (Math.Abs(entity.velocity.X) + Math.Abs(entity.velocity.Y) < maxSpeed * 0.5)
                {
                    entity.velocity.Y += Math.Sign(entity.velocity.Y) * turnAccle;
                }
            }
            else
            {
                entity.velocity.Y += Math.Sign(targetVector.Y - entity.velocity.Y) * turnAccle * 1.1f;
                if (Math.Abs(entity.velocity.X) + Math.Abs(entity.velocity.Y) < maxSpeed * 0.5)
                {
                    entity.velocity.X += Math.Sign(entity.velocity.X) * turnAccle;
                }
            }
        }
        public static void WormMovementEx(this Entity entity, Vector2 dest, float maxSpeed, float turnAccle = 0.1f, float ramAccle = 0.15f,
            float radiusSpeed = 0.05f, int distLimit = 900, float angleLimit = MathHelper.Pi * 2 / 5)
        {
            Vector2 targetVector = dest - entity.Center;
            targetVector = targetVector.SafeNormalize(Vector2.UnitY) * maxSpeed;
            if (targetVector.HasNaNs()) System.Diagnostics.Debugger.Break();
            if ((targetVector.X * entity.velocity.X > 0f) && (targetVector.Y * entity.velocity.Y > 0f)) //acclerate
            {
                entity.velocity.X += Math.Sign(targetVector.X - entity.velocity.X) * ramAccle;
                entity.velocity.Y += Math.Sign(targetVector.Y - entity.velocity.Y) * ramAccle;
            }
            float angle = MathHelper.WrapAngle(targetVector.ToRotation() - entity.velocity.ToRotation());
            if (Math.Abs(angle) >= angleLimit && (dest - entity.Center).Compare(distLimit) >= 0)
            {
                var speed = entity.velocity.Length();
                entity.velocity = entity.velocity.RotatedBy(radiusSpeed * Math.Sign(angle));
                if (speed < maxSpeed * 0.8f)
                {
                    entity.velocity = entity.velocity.SafeNormalize(Vector2.UnitY) * (speed + ramAccle);
                }
            }
            else
            {
                if ((targetVector.X * entity.velocity.X > 0f) || (targetVector.Y * entity.velocity.Y > 0f)) //turn
                {
                    entity.velocity.X += Math.Sign(targetVector.X - entity.velocity.X) * turnAccle;
                    entity.velocity.Y += Math.Sign(targetVector.Y - entity.velocity.Y) * turnAccle;

                    if (Math.Abs(targetVector.Y) < maxSpeed * 0.2 && targetVector.X * entity.velocity.X < 0)
                    {
                        entity.velocity.Y += Math.Sign(entity.velocity.Y) * turnAccle * 2f;
                    }

                    if (Math.Abs(targetVector.X) < maxSpeed * 0.2 && targetVector.Y * entity.velocity.Y < 0)
                    {
                        entity.velocity.X += Math.Sign(entity.velocity.X) * turnAccle * 2f;
                    }
                }
                else if (Math.Abs(targetVector.X) > Math.Abs(targetVector.Y))
                {
                    entity.velocity.X += Math.Sign(targetVector.X - entity.velocity.X) * turnAccle * 1.1f;
                    if (Math.Abs(entity.velocity.X) + Math.Abs(entity.velocity.Y) < maxSpeed * 0.5)
                    {
                        entity.velocity.Y += Math.Sign(entity.velocity.Y) * turnAccle;
                    }
                }
                else
                {
                    entity.velocity.Y += Math.Sign(targetVector.Y - entity.velocity.Y) * turnAccle * 1.1f;
                    if (Math.Abs(entity.velocity.X) + Math.Abs(entity.velocity.Y) < maxSpeed * 0.5)
                    {
                        entity.velocity.X += Math.Sign(entity.velocity.X) * turnAccle;
                    }
                }
            }
        }
        public static void FastMovement(this Entity npc, Vector2 dest, int ticksToArrive = 10)
        {
            Vector2 vector2 = dest - npc.Center;
            float lerpValue = GetLerpValue(100f * 10 / ticksToArrive, 600f * 10 / ticksToArrive, vector2.Length());
            float num34 = vector2.Length();
            if (num34 > 18f * 10 / ticksToArrive)
                num34 = 18f * 10 / ticksToArrive;

            npc.velocity = Vector2.Lerp(vector2.SafeNormalize(Vector2.Zero) * num34, vector2 / 6f, lerpValue);
        }
        public static void NewOrBoardcastText(this Entity entity, string quote, Color color, bool combatText = true)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(quote, color);
            if (Main.netMode == NetmodeID.Server)
                Terraria.Chat.ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(quote), color);
            if (combatText)
            {
                int num = CombatText.NewText(entity.Hitbox, color, quote, true);
                if (Main.netMode == NetmodeID.MultiplayerClient && num != 100)
                {
                    CombatText text = Main.combatText[num];
                    NetMessage.SendData(MessageID.CombatTextString, -1, -1, NetworkText.FromLiteral(quote), (int)text.color.PackedValue, text.position.X, text.position.Y);
                }
            }
        }

        public static void Loomup(this Projectile projectile,int rate = 25)
        {
            projectile.alpha -= rate;
            if (projectile.alpha < 0) projectile.alpha = 0;
        }
        public static void DrawAim(this Entity entity, Vector2 endpoint, Color color)
        {
            Texture2D aimTexture = ModContent.GetInstance<TeaNPCMartianAddon>().Assets.Request<Texture2D>("Projectiles/Boss/SkyDestroyer/AimLine").Value;
            Vector2 unit = endpoint - entity.Center;
            float length = unit.Length();
            unit.Normalize();
            for (int k = 0; k <= length; k += 4)
            {
                Vector2 drawPos = entity.Center + unit * k - Main.screenPosition;
                Color alphaCenter = color * 0.8f;
                Main.EntitySpriteDraw(aimTexture, drawPos, null, alphaCenter, k, new Vector2(2, 2), 1f, SpriteEffects.None, 0);
            }
        }
        public static bool CheckNPCAlive<T>(int index)where T : ModNPC
        {
            if (Main.npc.IndexInRange(index) && Main.npc[index].active && Main.npc[index].type == ModContent.NPCType<T>())
            {
                return true;
            }
            return false;
        }
        public static void KillAll<T>(bool haslocalAI1DeathAnimation = false, float localAI1Value = 1) where T : ModProjectile
        {
            for(int i = 0; i < Main.projectile.Length; i++)
            {
                if (Util.CheckProjAlive<T>(i))
                {
                    if(haslocalAI1DeathAnimation)
                    {
                        Main.projectile[i].localAI[1] = localAI1Value;
                    }
                    else
                    {
                        Main.projectile[i].Kill();
                    }
                }
            }
        }
        public static bool CheckProjAlive(int index, bool localAI1isDeathAnimation = false)
        {
            if (Main.projectile.IndexInRange(index) && Main.projectile[index].active)
            {
                if (localAI1isDeathAnimation)
                {
                    if (Main.projectile[index].localAI[1] == 0) return true;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        public static bool CheckProjAlive<T>(int index, bool localAI1isDeathAnimation = false) where T : ModProjectile
        {
            if (Main.projectile.IndexInRange(index) && Main.projectile[index].active && Main.projectile[index].type == ModContent.ProjectileType<T>())
            {
                if (localAI1isDeathAnimation)
                {
                    if (Main.projectile[index].localAI[1] == 0) return true;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
        public static IProjectileSource GetProjectileSource(this NPC npc) => new ProjectileSource_NPC(npc);
        public static IProjectileSource GetProjectileSource(this Projectile projectile) => new ProjectileSource_ProjectileParent(projectile);
        public static Vector2 ProjAIToVector(this Projectile projectile)
        {
            return new Vector2(projectile.ai[0], projectile.ai[1]);
        }
        public static int Compare(this Vector2 vector, float length)
        {
            float vecLen = vector.LengthSquared();
            if (vecLen > length * length) return 1;
            else if (vecLen == length * length) return 0;
            else return -1;
        }
        public static float GetLerpValue(float from, float to, float t, bool clamped = false)
        {
            if (clamped)
            {
                if (from < to)
                {
                    if (t < from)
                        return 0f;

                    if (t > to)
                        return 1f;
                }
                else
                {
                    if (t < to)
                        return 1f;

                    if (t > from)
                        return 0f;
                }
            }

            return (t - from) / (to - from);
        }
    }
}
