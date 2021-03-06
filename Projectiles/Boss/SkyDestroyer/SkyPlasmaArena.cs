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

namespace TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer
{
    public class SkyPlasmaArena:ModProjectile
    {
        public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plasma Arena");
        }
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            Projectile.alpha = 255;

            CooldownSlot = 1;
        }
        public override void AI()
        {
            int index = NPC.FindFirstNPC(ModContent.NPCType<SkyDestroyerHead>());
            /*if (!Main.dedServ)
            {
                if (!Filters.Scene["TeaNPCAddon:RectBlackout"].IsActive())
                {
                    Filters.Scene.Activate("TeaNPCAddon:RectBlackout");
                }
                Filters.Scene["TeaNPCAddon:RectBlackout"]
                    .GetShader()
                    .UseTargetPosition(projectile.Center)
                    .UseImageOffset(new Vector2(projectile.ai[0], projectile.ai[1]));
            }*/
            if (index == -1)
            {
                Projectile.Kill();
                return;
            }
            NPC head = Main.npc[index];
            if (head.ai[1] != SkyDestroyerSegment.WarpMove && head.ai[1] != SkyDestroyerSegment.PlasmaWarpBlast && head.ai[1] != SkyDestroyerSegment.SpaceWarp)
            {
                Projectile.Kill();
                return;
            }
            Player player = Main.player[head.target];
            float width = Projectile.ai[0];
            float height = Projectile.ai[1];

            if (Projectile.localAI[0] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                for (int i = 0; i < 4; i++)
                {
                    float length = i % 2 == 0 ? Projectile.ai[0] / 2 : Projectile.ai[1] / 2;
                    Vector2 velo = Vector2.UnitX.RotatedBy(Math.PI / 2 * i) * length;
                    base.Projectile.NewProjectile(Projectile.Center + velo, velo, ModContent.ProjectileType<SkyPlasmaRay>(),
                        Projectile.damage, 0f, Main.myPlayer, Projectile.whoAmI, 1);
                }
            }
            Projectile.localAI[0]++;

            if (Math.Abs(player.Center.X - Projectile.Center.X) > width / 2)
            {
                Vector2 DragVel = Math.Sign(Projectile.Center.X - player.Center.X) * Vector2.UnitX;
                player.velocity += DragVel;
                player.Center += DragVel * 10;
                player.controlDown = false;
                player.controlHook = false;
                player.controlJump = false;
                player.controlLeft = false;
                player.controlMount = false;
                player.controlRight = false;
                player.controlThrow = false;
                player.controlUp = false;
                player.controlUseItem = false;
                player.controlUseTile = false;
                if (player.mount.Active)
                {
                    player.mount.Dismount(player);
                }
                if(Math.Abs(player.Center.X - Projectile.Center.X) > width / 2 + 20)
                {
                    player.Center = new Vector2(Projectile.Center.X + Math.Sign(player.Center.X - Projectile.Center.X) * width / 2
                        , player.Center.Y);
                }
            }
            if (Math.Abs(player.Center.Y - Projectile.Center.Y) > height / 2)
            {
                Vector2 DragVel = Math.Sign(Projectile.Center.Y - player.Center.Y) * Vector2.UnitY;
                player.velocity += DragVel;
                player.Center += DragVel * 10;
                player.controlDown = false;
                player.controlHook = false;
                player.controlJump = false;
                player.controlLeft = false;
                player.controlMount = false;
                player.controlRight = false;
                player.controlThrow = false;
                player.controlUp = false;
                player.controlUseItem = false;
                player.controlUseTile = false;
                if (player.mount.Active)
                {
                    player.mount.Dismount(player);
                }
                if (Math.Abs(player.Center.Y - Projectile.Center.Y) > height / 2 + 15)
                {
                    player.Center = new Vector2(player.Center.X
                        , Projectile.Center.Y + Math.Sign(player.Center.Y - Projectile.Center.Y) * height / 2);
                }
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void Kill(int timeLeft)
        {
            Filters.Scene["TeaNPCAddon:RectBlackout"].Deactivate();
            base.Kill(timeLeft);
        }
    }
    public class SkyCircularArena : ModProjectile
    {
        public override string Texture => MigrationUtils.ProjTexturePrefix + ProjectileID.ShadowBeamHostile;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Circular Black Arena");
        }
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;

            Projectile.alpha = 255;

            CooldownSlot = 1;
        }
        public override void AI()
        {
            int index = NPC.FindFirstNPC(ModContent.NPCType<SkyDestroyerHead>());
            /*if (!Main.dedServ)
            {
                if (!Filters.Scene["TeaNPCAddon:CircularBlackout"].IsActive())
                {
                    Filters.Scene.Activate("TeaNPCAddon:CircularBlackout");
                }
                Filters.Scene["TeaNPCAddon:CircularBlackout"]
                    .GetShader()
                    .UseTargetPosition(projectile.Center)
                    .UseIntensity(projectile.ai[0]);
            }*/
            /*if (index == -1)
            {
                projectile.Kill();
                return;
            }
            NPC head = Main.npc[index];
            if (head.ai[1] != SkyDestroyerSegment.LightningStormEx)
            {
                projectile.Kill();
                return;
            }
            Player player = Main.player[head.target];*/
            Player player = Main.LocalPlayer;
            float dist = Projectile.ai[0];

            Projectile.localAI[0]++;

            if (Projectile.DistanceSQ(player.Center) > dist * dist)
            {
                Vector2 DragVel = Projectile.DirectionFrom(player.Center);
                player.velocity += DragVel;
                player.Center += DragVel * 10;
                player.controlDown = false;
                player.controlHook = false;
                player.controlJump = false;
                player.controlLeft = false;
                player.controlMount = false;
                player.controlRight = false;
                player.controlThrow = false;
                player.controlUp = false;
                player.controlUseItem = false;
                player.controlUseTile = false;
                if (player.mount.Active)
                {
                    player.mount.Dismount(player);
                }
                if (Projectile.Distance(player.Center) > dist + 20)
                {
                    player.Center = Projectile.Center + Projectile.DirectionTo(player.Center) * dist;
                }
            }
        }
        public override bool ShouldUpdatePosition() => false;
        public override bool? CanDamage() => false;
        public override void Kill(int timeLeft)
        {
            Filters.Scene["TeaNPCAddon:CircularBlackout"].Deactivate();
            base.Kill(timeLeft);
        }
    }
}
