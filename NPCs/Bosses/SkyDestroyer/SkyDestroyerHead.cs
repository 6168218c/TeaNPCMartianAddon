using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using Terraria.Localization;
using TeaNPCMartianAddon.Items.Bosses.Martians;
using TeaNPCMartianAddon.Projectiles.Boss;
using TeaNPCMartianAddon.Projectiles.Boss.SkyDestroyer;

namespace TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer
{
    [AutoloadBossHead]
    public class SkyDestroyerHead : SkyDestroyerSegment
    {
        #region Fields
        private int BoltCountdown;
        private bool spawned;
        private bool Dead = false;
        Vector2 centerVector;
        public float DynDR;
        public int DynDRTimer;
        public int lastHealth;
        #endregion
        #region Attack Control
        public int State { get => (int)NPC.ai[1]; set => NPC.ai[1] = value; }
        public class ModulePhase
        {
            private int _attackIndex;
            public int ID { get; }
            public List<int> Attacks { get; }
            public float MaxLifeLimit { get; }
            public string EnterQuoteID { get; }
            public ModulePhase(int id, int minAttack, int maxAttack, float maxLifeLimit, string enterQuote = null)
            {
                ID = id;
                Attacks = Enumerable.Range(minAttack, maxAttack - minAttack + 1).ToList();
                _attackIndex = 0;
                MaxLifeLimit = maxLifeLimit; EnterQuoteID = enterQuote;
            }
            public ModulePhase(int id, IEnumerable<int> attacks, float maxLifeLimit, string enterQuote = null)
            {
                ID = id;
                Attacks = new List<int>(attacks);_attackIndex = 0;
                MaxLifeLimit = maxLifeLimit; EnterQuoteID = enterQuote;
            }
            public void SwitchToNext(SkyDestroyerHead head)
            {
                if (!Attacks.IndexInRange(_attackIndex + 1))
                {
                    _attackIndex = 0;
                    head.SwitchTo(Attacks[_attackIndex]);
                }
                else
                {
                    _attackIndex++;
                    head.SwitchTo(Attacks[_attackIndex]);
                }
            }
        }
        public ModulePhase[] Modules { get; private set; } = new ModulePhase[4]
        {
            new ModulePhase(0,LightningStorm,FireballBarrage,1),
            new ModulePhase(1,LightningStorm,Plasmerizer,0.85f,"Mods.TeaNPCAddon.NPCChat.SDEX1"),
            new ModulePhase(2,FireballBarrage,PlasmaWarpBlast,0.60f,"Mods.TeaNPCAddon.NPCChat.SDEX2"),
            new ModulePhase(3,new int[]{ SpaceWarp,PlasmaWarpBlast,WarpMove,SpaceWarp,AntimatterBomb,PlasmaWarpBlast },0.35f,"Mods.TeaNPCAddon.NPCChat.SDEX3")
        };
        private int _currModuleIndex = 0;
        protected ModulePhase CurrentModule => Modules[_currModuleIndex];
        protected void UpdateCurrentModule()
        {
            if (_currModuleIndex < Modules.Length - 1)
            {
                if (NPC.life <= Modules[_currModuleIndex + 1].MaxLifeLimit * NPC.lifeMax)
                {
                    _currModuleIndex++;
                    SDMessage(Language.GetTextValue(CurrentModule.EnterQuoteID));
                }
            }
        }
        void SwitchTo(float ai1, bool resetCounter = true, bool resetAllTimer = true)
        {
            NPC.ai[1] = ai1;
            NPC.ai[2] = 0;
            if (resetCounter)
            {
                NPC.ai[3] = 0;
                NPC.localAI[2] = 0;
            }
            NPC.localAI[0] = 0;
            if (resetAllTimer)
            {
                NPC.localAI[1] = 0;
            }
            NPC.netUpdate = true;
        }
        #endregion
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Sky Destroyer");
            //DisplayName.AddTranslation(GameCulture.Chinese, "天际毁灭者");
        }
        public override void SetDefaults()
        {
            base.NPC.damage = 450;
            base.NPC.npcSlots = 10f;
            base.NPC.width = 150;
            base.NPC.height = 150;
            base.NPC.defense = 45;
            this.Music = Mod.GetSoundSlot(SoundType.Music, "Sounds/Music/CosmicSpace");
            base.NPC.lifeMax = 590000;
            base.NPC.aiStyle = -1;
            this.AnimationType = 10;
            base.NPC.knockBackResist = 0f;
            base.NPC.boss = true;
            base.NPC.value = Item.buyPrice(15, 50, 0, 0);
            base.NPC.alpha = 255;
            base.NPC.behindTiles = true;
            base.NPC.noGravity = true;
            base.NPC.noTileCollide = true;
            base.NPC.chaseable = false;
            base.NPC.HitSound = SoundID.NPCHit4;
            base.NPC.DeathSound = SoundID.NPCDeath14;
            base.NPC.netAlways = true;
            //this.bossBag = ModContent.ItemType<TreasureBagSkyDestroyer>();
            for (int i = 0; i < base.NPC.buffImmune.Length; i++)
            {
                base.NPC.buffImmune[i] = true;
            }
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(NPC.localAI[0]);
            writer.Write(WarpState);
            writer.Write(WarpMark);
            writer.Write(centerVector.X);
            writer.Write(centerVector.Y);
            writer.Write(this.BoltCountdown);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            NPC.localAI[0] = reader.ReadSingle();
            WarpState = reader.ReadSingle();
            WarpMark = reader.ReadInt32();
            centerVector.X = reader.ReadSingle();
            centerVector.Y = reader.ReadSingle();
            this.BoltCountdown = reader.ReadInt32();
        }
        public override void AI()
        {
            bool reTargeted = false;
            if (base.NPC.target < 0 || base.NPC.target == 255 || Main.player[base.NPC.target].dead)
            {
                base.NPC.TargetClosest(true);
                reTargeted = true;
            }
            Player player = Main.player[base.NPC.target];
            bool expertMode = Main.expertMode;
            Lighting.AddLight((int)((base.NPC.position.X + (float)(base.NPC.width / 2)) / 16f), (int)((base.NPC.position.Y + (float)(base.NPC.height / 2)) / 16f), 0.2f, 0.05f, 0.2f);
            if (player.dead && !Dead)
            {
                SDMessage(Language.GetTextValue("Mods.TeaNPCAddon.NPCChat.SD1"));
                Dead = true;
            }
            #region Loomup
            if (base.NPC.alpha != 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    int num2 = Dust.NewDust(new Vector2(base.NPC.position.X, base.NPC.position.Y), base.NPC.width, base.NPC.height, MyDustId.RedBubble, 0f, 0f, 100, default(Color), 2f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].noLight = true;
                }
            }
            base.NPC.alpha -= 12;
            if (base.NPC.alpha < 0)
            {
                base.NPC.alpha = 0;
            }
            #endregion
            #region Despawn
            if (Main.player[base.NPC.target].dead) //|| (Main.dayTime && !Main.eclipse))
            {
                base.NPC.velocity = new Vector2(0f, -80f);
                this.canDespawn = true;
                if (base.NPC.timeLeft > 150)
                {
                    base.NPC.timeLeft = 150;
                }
                return;
            }
            #endregion
            #region Spawn Wont change
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (!this.spawned && base.NPC.ai[0] == 0f)
                {
                    int prev = base.NPC.whoAmI;
                    for (int i = 0; i < 64; i++)
                    {
                        int curr;
                        if (i >= 0 && i < 63 && i % 2 == 0)
                        {
                            curr = NPC.NewNPC((int)base.NPC.position.X + base.NPC.width / 2, (int)base.NPC.position.Y + base.NPC.height / 2, ModContent.NPCType<SkyDestroyerBodyAlt>(), base.NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                        }
                        else if (i >= 0 && i < 63)
                        {
                            curr = NPC.NewNPC((int)base.NPC.position.X + base.NPC.width / 2, (int)base.NPC.position.Y + base.NPC.height / 2, ModContent.NPCType<SkyDestroyerBody>(), base.NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                        }
                        else
                        {
                            curr = NPC.NewNPC((int)base.NPC.position.X + base.NPC.width / 2, (int)base.NPC.position.Y + base.NPC.height / 2, ModContent.NPCType<SkyDestroyerTail>(), base.NPC.whoAmI, 0f, 0f, 0f, 0f, 255);
                        }
                        Main.npc[curr].realLife = base.NPC.whoAmI;
                        Main.npc[curr].ai[2] = (float)base.NPC.whoAmI;
                        Main.npc[curr].ai[1] = (float)prev;
                        Main.npc[prev].ai[0] = (float)curr;
                        if (i == 63)
                        {
                            Main.npc[curr].ai[0] = -1;
                        }
                        prev = curr;
                    }
                    this.spawned = true;
                }
                if (!base.NPC.active && Main.netMode == NetmodeID.Server)
                {
                    NetMessage.SendData(MessageID.StrikeNPC, -1, -1, null, base.NPC.whoAmI, -1f, 0f, 0f, 0, 0, 0);
                }
            }
            #endregion
            /*#region Enter phase3
            if (!phase3 && (double)NPC.life <= (double)NPC.lifeMax * 0.35)
            {
                SDMessage(Language.GetTextValue("Mods.TeaNPCAddon.NPCChat.SD2"));
                this.phase3 = true;
            }
            #endregion*/
            var maxSpeed = 18f + player.velocity.Length() / 3;
            float turnAcc = 0.125f;
            float ramAcc = 0.15f;
            if (Main.expertMode)
                maxSpeed *= 1.125f;
            //if (Main.getGoodWorld)
            //    maxSpeed *= 1.25f;
            maxSpeed = maxSpeed * 0.9f + maxSpeed * ((NPC.lifeMax - NPC.life) / (float)NPC.lifeMax) * 0.2f;
            maxSpeed = maxSpeed * 0.9f + maxSpeed * 0.1f * NPC.Distance(player.Center) / 1200;
            maxSpeed = Math.Max(player.velocity.Length() * 1.5f, maxSpeed);

            //handle life phase
            UpdateCurrentModule();

            if (NPC.ai[1] == LightningStorm)
            {
                NPC.ai[2]++;
                if (NPC.Distance(player.Center) >= 2500)
                    NPC.WormMovementEx(player.Center, maxSpeed * 1.5f, turnAcc, ramAcc, radiusSpeed: 0.08f, angleLimit: MathHelper.Pi / 4);
                else
                    CrawlipedeMove(player, maxSpeed, turnAcc, ramAcc);

                if (NPC.ai[2] % 225 == 0 && NPC.ai[2] != 450 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetProjectileSource(),player.Center, Vector2.Zero, ModContent.ProjectileType<SkyLightningOrbCenter>(),
                      NPC.damage / 6, 0f, Main.myPlayer, NPC.whoAmI, 0);
                }

                if (NPC.ai[2] == 120 || NPC.ai[2] == 560 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Vector2 start = player.Center - Vector2.UnitY * 900;
                    for (int i = 1; i <= 3; i++)
                    {
                        var pos = start - Vector2.UnitX * 360 - i * Vector2.UnitX * 45;
                        var velo = (player.Center - pos).SafeNormalize(Vector2.Zero) * 12f;
                        Projectile.NewProjectile(NPC.GetProjectileSource(),pos, velo, ProjectileID.CultistBossLightningOrbArc, NPC.damage / 6, 0f, Main.myPlayer, velo.ToRotation());
                        pos = start + Vector2.UnitX * 360 + i * Vector2.UnitX * 45;
                        velo = (player.Center - pos).SafeNormalize(Vector2.Zero) * 12f;
                        Projectile.NewProjectile(NPC.GetProjectileSource(),pos, velo, ProjectileID.CultistBossLightningOrbArc, NPC.damage / 4, 0f, Main.myPlayer, velo.ToRotation());
                    }
                }

                if (NPC.ai[2] >= 570)
                {
                    CurrentModule.SwitchToNext(this);
                }
            }
            else if (NPC.ai[1] == FireballBarrage)
            {
                FireballBarrageAI(player, maxSpeed, turnAcc, ramAcc,reTargeted);
            }
            else if (NPC.ai[1] == Plasmerizer)
            {
                PlasmerizerAI(player, maxSpeed, turnAcc, ramAcc,reTargeted);
            }
            else if (NPC.ai[1] == WarpMove)
            {
                WarpMoveAI(player, maxSpeed, turnAcc, ramAcc,reTargeted);
            }
            else if (NPC.ai[1] == PlasmaWarpBlast)
            {
                PlasmaWarpBlastAI(player, maxSpeed, turnAcc, ramAcc, reTargeted);
            }
            else if (NPC.ai[1] == SpaceWarp)
            {
                SpaceWarpAI(player, maxSpeed, turnAcc, ramAcc,reTargeted);
            }
            else if (NPC.ai[1] == AntimatterBomb)
            {
                AntimatterBombAI(player, maxSpeed, turnAcc, ramAcc,reTargeted);
            }
            NPC.rotation = NPC.velocity.ToRotation();
        }
        #region CommonHelper
        void CrawlipedeMove(Player player, float maxSpeed, float turnAcc, float ramAcc,int offset = 200, bool alwaysPassive = false)
        {
            var targetPos = player.Center;
            int playerTileX = (int)(targetPos.X / 16f);
            int playerTileY = (int)(targetPos.Y / 16f);
            int OffgroundTile = -1;
            for (int i = playerTileX - 2; i <= playerTileX + 2; i++)
            {
                for (int j = playerTileY; j <= playerTileY + 18; j++)
                {
                    if (WorldGen.SolidTile2(i, j))
                    {
                        OffgroundTile = j;
                        break;
                    }
                }
                if (OffgroundTile > 0)
                {
                    break;
                }
            }
            if (OffgroundTile > 0 || alwaysPassive)
            {
                OffgroundTile *= 16;
                float heightOffset = OffgroundTile - 600;
                if (player.Center.Y > heightOffset)
                {
                    targetPos.Y = heightOffset - offset;
                    if (Math.Abs(NPC.Center.X - player.Center.X) < 500f)
                    {
                        targetPos.X = targetPos.X + Math.Sign(NPC.velocity.X) * 600f;
                    }
                    turnAcc *= 1.5f;
                }
                else
                {
                    turnAcc *= 1.2f;
                }
            }
            else
            {
                maxSpeed *= 1.125f;//charge
                turnAcc *= 2f;
            }
            float speed = NPC.velocity.Length();
            if (OffgroundTile > 0)
            {
                float num47 = maxSpeed * 1.3f;
                float num48 = maxSpeed * 0.7f;
                float num49 = NPC.velocity.Length();
                if (num49 > 0f)
                {
                    if (num49 > num47)
                    {
                        NPC.velocity.Normalize();
                        NPC.velocity *= num47;
                    }
                    else if (num49 < num48)
                    {
                        NPC.velocity.Normalize();
                        NPC.velocity *= num48;
                    }
                }
            }
            NPC.WormMovementEx(targetPos, maxSpeed, turnAcc);
        }
        #endregion
        void FireballBarrageAI(Player player, float maxSpeed, float turnAcc, float ramAcc,bool reTargeted)
        {
            NPC.ai[2]++;
            if (NPC.ai[2] <= 180)
            {
                if (NPC.ai[3] == 0)
                {
                    NPC.ai[2]--;//revert
                    NPC.WormMovementEx(player.Center - Vector2.UnitY * 450 + Vector2.UnitX * Math.Sign(NPC.Center.X - player.Center.X) * 2400f,
                        maxSpeed, turnAcc, ramAcc, 0.06f, 450);
                    if (Math.Abs(NPC.Center.Y - player.Center.Y + 450) <= 60)
                    {
                        NPC.ai[3]++;
                        NPC.velocity.X = Math.Sign(player.Center.X - NPC.Center.X) * maxSpeed / 3;
                    }
                }
                else
                {
                    //npc.WormMovementEx(npc.Center + Vector2.UnitX * Math.Sign(npc.velocity.X) * 1000,
                    //    maxSpeed * 4.5f, turnAcc / 2, ramAcc);
                    if (NPC.Distance(player.Center) >= 3600)
                    {
                        maxSpeed *= 0.5f;
                        NPC.velocity.X *= 0.98f;
                    }
                    if (Math.Abs(NPC.velocity.X) <= maxSpeed * 1.2f) NPC.velocity.X *= 1.1f;
                    NPC.velocity.Y *= 0.95f;
                    if (NPC.ai[2] % 10 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        int mode = (NPC.ai[2] / 15 % 2 == 0) ? 1 : 3;
                        int direction = (NPC.ai[2] / 15 % 2 == 0) ? -1 : 1;
                        Projectile.NewProjectile(NPC.GetProjectileSource(),NPC.Center, Vector2.UnitY * direction * 8f, ModContent.ProjectileType<SkyFireballLauncher>(),
                            NPC.damage / 6, 0f, Main.myPlayer, NPC.target, mode);
                    }
                }
            }
            else if (NPC.ai[2] <= 450)
            {
                if (NPC.ai[2] == 200 && CurrentModule.ID > 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    var unitX = Vector2.UnitX * 900;
                    var unitY = Vector2.UnitY * 270;
                    int sign = Main.rand.NextBool() ? -1 : 1;
                    for (int i = -5; i <= 5; i++)
                    {
                        var pos = player.Center + unitX * sign + unitY * i;
                        Projectile.NewProjectile(NPC.GetProjectileSource(),pos, Vector2.UnitX * (-sign) * 6, ModContent.ProjectileType<SkyFireballLauncher>(),
                            NPC.damage / 6, 0f, Main.myPlayer, NPC.target, 2);
                        sign = -sign;
                    }
                }

                NPC.WormMovementEx(player.Center, maxSpeed, turnAcc, ramAcc, radiusSpeed: 0.08f, distLimit: 1500);
            }
            else
            {
                CurrentModule.SwitchToNext(this);
            }
        }
        void PlasmerizerAI(Player player,float maxSpeed,float turnAcc,float ramAcc,bool reTargeted)
        {
            NPC.ai[2]++;
            if (NPC.Distance(player.Center) > 3600)
            {
                NPC.ai[2]--;
                NPC.WormMovement(player.Center, maxSpeed * 1.5f, turnAcc, ramAcc);
                return;
            }
            maxSpeed *= 0.4f;
            NPC.WormMovement(player.Center, maxSpeed, turnAcc, ramAcc);

            if (NPC.ai[2] % 45 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 velo = Vector2.UnitX.RotatedBy(MathHelper.Pi / 4 * Main.rand.Next(8));
                Vector2 offset = -velo * 1200 + velo.RotatedBy(Math.PI / 2) * Main.rand.Next(-10, 10) * 60;
                Projectile.NewProjectile(NPC.GetProjectileSource(),player.Center + offset, velo, ModContent.ProjectileType<SkyPlasmaBallChainHead>(), NPC.damage / 6, 0f, Main.myPlayer, -1, 0);
            }
            if (NPC.ai[2] % 180 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 velo = (player.Center + player.velocity * 60f - NPC.Center) / 120;
                Projectile.NewProjectile(NPC.GetProjectileSource(),NPC.Center, velo, ModContent.ProjectileType<SkyPlasmerizerRay>(), NPC.damage / 6, 0f, Main.myPlayer);
            }
            if (NPC.ai[2] % 180 >= 80)
            {
                if (NPC.ai[2] % 180 == 80 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetProjectileSource(),player.Center, Vector2.Zero, ModContent.ProjectileType<SkyAim>(),
                        0, 0f, Main.myPlayer, NPC.whoAmI, 100);
                }
                #region Visuals
                if (Main.rand.Next(2) == 0)
                {
                    float num11 = (float)(Main.rand.NextDouble() * 1.0 - 0.5); //vanilla dust
                    if ((double)num11 < -0.5)
                        num11 = -0.5f;
                    if ((double)num11 > 0.5)
                        num11 = 0.5f;
                    Vector2 vector21 = new Vector2((float)-NPC.width * 0.2f * NPC.scale, 0.0f).RotatedBy((double)num11 * 6.28318548202515, new Vector2()).RotatedBy((double)NPC.velocity.ToRotation(), new Vector2());
                    int index21 = Dust.NewDust(NPC.Center - Vector2.One * 5f, 10, 10, DustID.Electric, (float)(-(double)NPC.velocity.X / 3.0), (float)(-(double)NPC.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                    Main.dust[index21].position = NPC.Center + vector21 * NPC.scale;
                    Main.dust[index21].velocity = Vector2.Normalize(Main.dust[index21].position - NPC.Center) * 2f;
                    Main.dust[index21].noGravity = true;
                    float num1 = (float)(Main.rand.NextDouble() * 1.0 - 0.5);
                    if ((double)num1 < -0.5)
                        num1 = -0.5f;
                    if ((double)num1 > 0.5)
                        num1 = 0.5f;
                    Vector2 vector2 = new Vector2((float)-NPC.width * 0.6f * NPC.scale, 0.0f).RotatedBy((double)num1 * 6.28318548202515, new Vector2()).RotatedBy((double)NPC.velocity.ToRotation(), new Vector2());
                    int index2 = Dust.NewDust(NPC.Center - Vector2.One * 5f, 10, 10, DustID.Electric, (float)(-(double)NPC.velocity.X / 3.0), (float)(-(double)NPC.velocity.Y / 3.0), 150, Color.Transparent, 0.7f);
                    Main.dust[index2].velocity = Vector2.Zero;
                    Main.dust[index2].position = NPC.Center + vector2 * NPC.scale;
                    Main.dust[index2].noGravity = true;
                    for(int i = 0; i < 2; i++)
                    {
                        int num = Dust.NewDust(new Vector2(NPC.position.X, NPC.position.Y), NPC.width, NPC.height, DustID.Electric, 0f, 0f, 100, default, 2f);
                        Main.dust[num].noGravity = true;
                        Main.dust[num].noLight = true;
                        Main.dust[num].scale = 0.4f;
                        Main.dust[num].velocity = Main.rand.NextVector2Unit(NPC.rotation - 0.001f, NPC.rotation + 0.001f) * 9f;
                    }
                }
                #endregion
            }
            if (NPC.ai[2] >= 600)
            {
                for (int i = 0; i < Main.projectile.Length; i++)
                {
                    if (Util.CheckProjAlive<SkyPlasmaBallChainHead>(i))
                    {
                        Main.projectile[i].localAI[1] = 1;
                    }
                }
                CurrentModule.SwitchToNext(this);
            }
        }
        void AntimatterBombAI(Player player, float maxSpeed, float turnAcc, float ramAcc, bool reTargeted)
        {
            if (NPC.ai[2] == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.localAI[0] = Projectile.NewProjectile(NPC.GetProjectileSource(),player.Center, Vector2.Zero, ModContent.ProjectileType<SkyAntimatterBombCenter>(),
                    NPC.damage / 3, 0f, Main.myPlayer, NPC.whoAmI, 0);
                NPC.netUpdate = true;
            }

            NPC.ai[2]++;

            if (NPC.ai[3] == 0)
            {
                NPC.WormMovementEx(player.Center - Vector2.UnitY * 450 + Vector2.UnitX * Math.Sign(NPC.Center.X - player.Center.X) * 2400f,
                    maxSpeed, turnAcc, ramAcc, 0.06f, 450);
                if (NPC.ai[2] > 1) NPC.ai[2]--;//revert but prevent spawning again.
                if (Math.Abs(NPC.Center.Y - player.Center.Y + 450) <= 80)
                {
                    NPC.ai[3]++;
                    NPC.velocity.X = Math.Sign(player.Center.X - NPC.Center.X) * maxSpeed / 3;
                }
            }
            else if (NPC.ai[3] == 1)
            {
                if (Math.Abs(NPC.velocity.X) <= maxSpeed * 1.2f) NPC.velocity.X *= 1.1f;
                //y position hover movement

                float yDist = player.Center.Y - 450 - NPC.Center.Y;
                if (NPC.velocity.Y < yDist)
                {
                    NPC.velocity.Y += 0.5f;
                    if (NPC.velocity.Y < 0f && yDist > 0f)
                    {
                        NPC.velocity.Y += Math.Max(0.5f, NPC.velocity.Y / 10);
                    }
                }
                else if (NPC.velocity.Y > yDist)
                {
                    NPC.velocity.Y -= 0.5f;
                    if (NPC.velocity.Y > 0f && yDist < 0f)
                    {
                        NPC.velocity.Y -= Math.Max(0.5f, NPC.velocity.Y / 10);
                    }
                }

                if (Math.Abs(player.Center.X - NPC.Center.X) >= 2000 && (Math.Sign(NPC.velocity.X) != Math.Sign(player.Center.X - NPC.Center.X)))
                {
                    ResetWarpStates();
                    NPC.ai[2] = 1;
                    NPC.ai[3]++;
                }
            }
            else if (NPC.ai[3] == 2)
            {
                if (NPC.ai[2] == 10 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    var warpTarget = player.Center * 2 - NPC.Center;
                    WarpMark = Projectile.NewProjectile(NPC.GetProjectileSource(),NPC.Center + NPC.velocity * 20, Vector2.Zero, ModContent.ProjectileType<SkyWarpMark>(),
                        NPC.damage * 2 / 7, 0f, Main.myPlayer, warpTarget.X, warpTarget.Y);
                    WarpState = 1;
                    NPC.netUpdate = true;
                }
                if (NPC.ai[2] >= 10)
                {
                    if (WarpState == 0)
                    {
                        //npc.ai[2] = 1;
                        //npc.ai[3]++;
                    }
                    else
                    {
                        if (Util.CheckProjAlive<SkyWarpMark>(WarpMark))
                        {
                            Projectile mark = Main.projectile[WarpMark];
                            if (NPC.DistanceSQ(mark.Center) <= warpDistance * warpDistance)
                            {
                                WarpState = 0;
                                mark.localAI[1]++;
                                NPC.ai[2] = 1;
                                NPC.ai[3]++;
                                NPC.Center = mark.ProjAIToVector();
                            }
                            else
                            {
                                NPC.velocity = (mark.Center - NPC.Center).SafeNormalize(Vector2.Zero) * maxSpeed;
                            }
                        }
                        else
                        {
                            if (Main.netMode != NetmodeID.MultiplayerClient)
                            {
                                NPC.WormMovementEx(player.Center, maxSpeed, turnAcc, ramAcc);
                            }
                        }
                    }
                }
            }
            else if (NPC.ai[3] == 3)
            {
                if (Math.Abs(NPC.velocity.X) <= maxSpeed * 1.2f) NPC.velocity.X *= 1.1f;
                //y position hover movement

                float yDist = player.Center.Y + 450 - NPC.Center.Y;
                if (NPC.velocity.Y < yDist)
                {
                    NPC.velocity.Y += 0.5f;
                    if (NPC.velocity.Y < 0f && yDist > 0f)
                    {
                        NPC.velocity.Y += Math.Max(0.5f, NPC.velocity.Y / 10);
                    }
                }
                else if (NPC.velocity.Y > yDist)
                {
                    NPC.velocity.Y -= 0.5f;
                    if (NPC.velocity.Y > 0f && yDist < 0f)
                    {
                        NPC.velocity.Y -= Math.Max(0.5f, NPC.velocity.Y / 10);
                    }
                }

                if (Math.Abs(player.Center.X - NPC.Center.X) >= 2000 && (Math.Sign(NPC.velocity.X) != Math.Sign(player.Center.X - NPC.Center.X)))
                {
                    NPC.ai[2] = 1;
                    NPC.ai[3]++;
                }
            }
            else if (NPC.ai[3] == 4)
            {
                if (NPC.ai[2] >= 180&&!IsWarping())
                {
                    for(int i = 0; i < Main.projectile.Length; i++)
                    {
                        if (Util.CheckProjAlive<SkyAntimatterBombCenter>(i))
                        {
                            Main.projectile[i].Kill();
                        }
                    }
                    CurrentModule.SwitchToNext(this);
                }
            }
            if (NPC.ai[3] == 1 || NPC.ai[3] == 3)
            {
                if (NPC.ai[2] % 15 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    Projectile.NewProjectile(NPC.GetProjectileSource(),NPC.Center, Vector2.UnitY * 2 * Math.Sign(player.Center.Y - NPC.Center.Y), ModContent.ProjectileType<SkyMatterMissile>(),
                        NPC.damage / 5, 0f, Main.myPlayer, NPC.whoAmI, 1);
                }
            }
        }
        void ResetWarpStates()
        {
            int i = NPC.whoAmI;
            int counter = 0;
            while (i != -1)
            {
                counter++;
                NPC tmpNPC = Main.npc[i];
                tmpNPC.localAI[2] = 0;
                tmpNPC.localAI[3] = -1;
                tmpNPC.netUpdate = true;
                i = (int)Main.npc[i].ai[0];
            }
        }
        bool IsWarping()
        {
            int i = NPC.whoAmI;
            int counter = 0;
            bool warping = false;
            while (i != -1)
            {
                counter++;
                NPC tmpNPC = Main.npc[i];
                if (tmpNPC.localAI[2] == 1)
                {
                    warping = true;
                }
                i = (int)Main.npc[i].ai[0];
            }
            return warping;
        }
        /// <summary>
        /// Find first warp mark based on localAI[1] == -1 , only used in linked list situations
        /// </summary>
        /// <returns>index in Main.npc</returns>
        int FindFirstWarpMark()
        {
            for(int i = 0; i < Main.npc.Length; i++)
            {
                if (Util.CheckProjAlive<SkyWarpMark>(i))
                {
                    if (Main.projectile[i].localAI[1] == -1)
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        void WarpMoveAI(Player player, float maxSpeed, float turnAcc, float ramAcc,bool reTargeted)
        {
            if (NPC.ai[2] == 0)
            {
                ResetWarpStates();
                //spawn arena
                centerVector = player.Center;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[0] = Projectile.NewProjectile(NPC.GetProjectileSource(),player.Center, Vector2.Zero, ModContent.ProjectileType<SkyPlasmaArena>(),
                        NPC.damage / 4, 0f, Main.myPlayer, 2700, 1500);
                    NPC.netUpdate = true;
                }
            }
            else if (reTargeted)
            {
                centerVector = player.Center;
                Main.projectile[(int)NPC.localAI[0]].Center = centerVector;
            }
            
            NPC.ai[2]++;
            //no timer
            if (NPC.Distance(centerVector) >= 1600 && NPC.ai[2] >= 45 && WarpState == 0)
            {
                var warpTarget = centerVector + NPC.DirectionTo(centerVector) * 1800;
                var warpStart = centerVector - NPC.DirectionTo(centerVector) * 1800;
                if(NPC.ai[3] < 6)
                {
                    if(Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        WarpMark = Projectile.NewProjectile(NPC.GetProjectileSource(),warpStart, Vector2.Zero, ModContent.ProjectileType<SkyWarpMark>(),
                        NPC.damage / 6, 0f, Main.myPlayer, warpTarget.X, warpTarget.Y);
                        Main.projectile[WarpMark].localAI[0] = -1;
                        Main.projectile[WarpMark].netUpdate = true;
                        Projectile.NewProjectile(NPC.GetProjectileSource(),warpTarget, Vector2.Zero, ModContent.ProjectileType<SkyAimLine>(),
                            0, 0f, Main.myPlayer, NPC.whoAmI, 0);
                        NPC.netUpdate = true;
                    }
                    WarpState = 1;
                }
                else
                {
                    NPC.WormMovementEx(player.Center + player.DirectionTo(NPC.Center) * 1500, maxSpeed, turnAcc, ramAcc);
                }
            }

            if (WarpState == 0)
            {
                if (NPC.velocity.Compare(maxSpeed * 1.2f) < 0) NPC.velocity *= 1.05f;
                if (NPC.ai[3] >= 6 && !IsWarping())
                {
                    ResetWarpStates();
                    Util.KillAll<SkyWarpMark>(true, 2);
                    if (Util.CheckProjAlive<SkyPlasmaArena>((int)NPC.localAI[0]))
                    {
                        Main.projectile[(int)NPC.localAI[0]].Kill();
                    }
                    CurrentModule.SwitchToNext(this);
                }
            }
            else
            {
                if (Util.CheckProjAlive<SkyWarpMark>(WarpMark))
                {
                    Projectile mark = Main.projectile[WarpMark];
                    if (NPC.DistanceSQ(mark.Center) <= warpDistance * warpDistance)
                    {
                        WarpState = 0;
                        mark.localAI[1]++;
                        NPC.ai[3]++;
                        NPC.ai[2] = 1;
                        NPC.Center = mark.ProjAIToVector();
                        NPC.velocity = (player.Center - NPC.Center).SafeNormalize(Vector2.Zero) * NPC.velocity.Length();
                    }
                    else
                    {
                        NPC.velocity = (mark.Center - NPC.Center).SafeNormalize(Vector2.Zero) * maxSpeed;
                    }
                }
                else
                {
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        NPC.WormMovementEx(player.Center, maxSpeed, turnAcc, ramAcc);
                    }
                }
            }
        }
        void PlasmaWarpBlastAI(Player player, float maxSpeed, float turnAcc,float ramAcc,bool reTargeted)
        {
            if (NPC.ai[2] == 0)
            {
                ResetWarpStates();
                //spawn arena
                centerVector = player.Center;
                if (Main.netMode != NetmodeID.MultiplayerClient)
                {
                    NPC.localAI[0] = Projectile.NewProjectile(NPC.GetProjectileSource(),player.Center, Vector2.Zero, ModContent.ProjectileType<SkyPlasmaArena>(),
                        NPC.damage / 4, 0f, Main.myPlayer, 2700, 1800);
                    NPC.netUpdate = true;

                    int directionX = NPC.direction = Main.rand.NextBool() ? -1 : 1;
                    int directionY = Main.rand.NextBool() ? -1 : 1;
                    Vector2 visibleStart = centerVector + Vector2.UnitX * 1500 * directionX + Vector2.UnitY * 900 * directionY;
                    Vector2 visibleEnd = centerVector - Vector2.UnitX * 1500 * directionX + Vector2.UnitY * 900 * directionY;
                    Vector2 visibleStart2 = centerVector + Vector2.UnitX * 1500 * directionX - Vector2.UnitY * 900 * directionY;
                    Vector2 visibleEnd2 = centerVector - Vector2.UnitX * 1500 * directionX - Vector2.UnitY * 900 * directionY;

                    WarpMark = Projectile.NewProjectile(NPC.GetProjectileSource(),NPC.Center + NPC.velocity * 30, Vector2.Zero, ModContent.ProjectileType<SkyWarpMark>(),
                    NPC.damage / 6, 0f, Main.myPlayer, visibleStart.X, visibleStart.Y);
                    Main.projectile[WarpMark].localAI[1] = -1;
                    int portal = Projectile.NewProjectile(NPC.GetProjectileSource(),visibleEnd, Vector2.Zero, ModContent.ProjectileType<SkyWarpMark>(),
                    NPC.damage / 6, 0f, Main.myPlayer, visibleStart2.X, visibleStart2.Y);
                    var warpTarget = NPC.Center + NPC.DirectionTo(centerVector) * 1800;
                    int portal2 = Projectile.NewProjectile(NPC.GetProjectileSource(),visibleEnd2, Vector2.Zero, ModContent.ProjectileType<SkyWarpMark>(),
                    NPC.damage / 6, 0f, Main.myPlayer, warpTarget.X, warpTarget.Y);

                    Main.projectile[WarpMark].localAI[0] = portal;
                    Main.projectile[WarpMark].netUpdate = true;
                    Main.projectile[portal].localAI[0] = portal2;
                    Main.projectile[portal].netUpdate = true;
                    Main.projectile[portal2].localAI[0] = -1;
                    Main.projectile[portal2].netUpdate = true;
                    NPC.netUpdate = true;
                }
                WarpState = 1;
            }
            else if (reTargeted)
            {
                centerVector = player.Center;
                Main.projectile[(int)NPC.localAI[0]].Center = centerVector;
            }

            NPC.ai[2]++;

            if (NPC.ai[2] % 60 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
            {
                if (NPC.ai[3] > 0)//one passed
                {
                    Vector2 top = centerVector + Vector2.UnitX * 1350 * NPC.direction + Vector2.UnitY * 900;
                    Vector2 bottom = centerVector - Vector2.UnitX * 1350 * NPC.direction + Vector2.UnitY * 900;
                    Vector2 velo = Vector2.UnitY * Math.Sign(player.Center.Y - top.Y) * 15f;
                    Projectile.NewProjectile(NPC.GetProjectileSource(),Vector2.Lerp(top, bottom, Main.rand.NextFloat()), velo, ModContent.ProjectileType<SkyPlasmaBallChainHead>(),
                        NPC.damage / 6, 0f, Main.myPlayer, -1, 1);
                }

                if (NPC.ai[3] > 1)//other side passed
                {
                    Vector2 top = centerVector + Vector2.UnitX * 1350 * NPC.direction - Vector2.UnitY * 900;
                    Vector2 bottom = centerVector - Vector2.UnitX * 1350 * NPC.direction - Vector2.UnitY * 900;
                    Vector2 velo = Vector2.UnitY * Math.Sign(player.Center.Y - top.Y) * 15f;
                    Projectile.NewProjectile(NPC.GetProjectileSource(),Vector2.Lerp(top, bottom, Main.rand.NextFloat()), velo, ModContent.ProjectileType<SkyPlasmaBallChainHead>(),
                        NPC.damage / 6, 0f, Main.myPlayer, -1, 1);
                }
            }

            if (WarpState == 0)
            {
                NPC.WormMovementEx(player.Center + player.DirectionTo(NPC.Center) * 1800, maxSpeed, turnAcc, ramAcc);
                if (Util.CheckProjAlive<SkyWarpMark>(WarpMark))
                {
                    if (Main.projectile[WarpMark].localAI[0] != -1)
                    {
                        WarpMark = (int)Main.projectile[WarpMark].localAI[0];
                        WarpState = 1;
                    }
                }
                if (WarpState == 0 && !IsWarping())
                {
                    ResetWarpStates();
                    Util.KillAll<SkyWarpMark>(true, 2);
                    if (Util.CheckProjAlive<SkyPlasmaArena>((int)NPC.localAI[0]))
                    {
                        Main.projectile[(int)NPC.localAI[0]].Kill();
                    }
                    Util.KillAll<SkyPlasmaBallChainHead>(true);
                    CurrentModule.SwitchToNext(this);
                }
            }
            else
            {
                if (Util.CheckProjAlive<SkyWarpMark>(WarpMark))
                {
                    Projectile mark = Main.projectile[WarpMark];
                    if (NPC.DistanceSQ(mark.Center) <= warpDistance * warpDistance)
                    {
                        WarpState = 0;
                        mark.localAI[1]++;
                        NPC.ai[3]++;
                        NPC.Center = mark.ProjAIToVector();
                    }
                    else
                    {
                        NPC.velocity = (mark.Center - NPC.Center).SafeNormalize(Vector2.Zero) * maxSpeed;
                    }
                }
                else
                {
                    WarpMark = FindFirstWarpMark();
                }
            }
        }
        void SpaceWarpAI(Player player,float maxSpeed,float turnAcc,float ramAcc,bool reTargeted)
        {
            if (NPC.ai[2] == 0)
            {
                ResetWarpStates();
                centerVector = player.Center;
                NPC.localAI[0] = Projectile.NewProjectile(NPC.GetProjectileSource(),player.Center, Vector2.Zero, ModContent.ProjectileType<SkyPlasmaArena>(),
                        NPC.damage / 4, 0f, Main.myPlayer, 480 * 4 * 2, 600 * 2);
                NPC.netUpdate = true;
            }
            NPC.ai[2]++;

            if (NPC.ai[2] <= 120)
            {
                NPC.WormMovementEx(player.Center, maxSpeed, turnAcc, ramAcc);
                if (NPC.ai[2] == 120 && Main.netMode != NetmodeID.MultiplayerClient)
                {
                    int sign = Main.rand.NextBool() ? -1 : 1;
                    int signX = Main.rand.NextBool() ? -1 : 1;
                    int lastId = -1;
                    Projectile proj = null;
                    for (int k = -4; k <= 3; k++)
                    {
                        var end = centerVector + Vector2.UnitX * (k + 1) * 480 * signX + Vector2.UnitY * 600 * sign;
                        var begin = centerVector + Vector2.UnitX * k * 480 * signX + Vector2.UnitY * 600 * sign;
                        if (lastId == -1)
                        {
                            var begin2 = centerVector + Vector2.UnitX * k * 480 * signX - Vector2.UnitY * 600 * sign;
                            var start = Projectile.NewProjectileDirect(NPC.GetProjectileSource(),NPC.Center + NPC.velocity * 20, Vector2.Zero, ModContent.ProjectileType<SkyWarpMark>(),
                            0, 0f, Main.myPlayer, begin2.X, begin2.Y);
                            start.localAI[0] = -1;
                            start.localAI[1] = -1;
                            start.netUpdate = true;
                            WarpMark = lastId = start.whoAmI;
                        }
                        proj = Projectile.NewProjectileDirect(NPC.GetProjectileSource(),begin, Vector2.Zero, ModContent.ProjectileType<SkyWarpMark>(),
                            0, 0f, Main.myPlayer, end.X, end.Y);
                        proj.localAI[0] = -1;
                        proj.netUpdate = true;
                        Main.projectile[lastId].localAI[0] = proj.whoAmI;
                        lastId = proj.whoAmI;
                        sign = -sign;
                    }
                    var start2 = centerVector + Vector2.UnitX * 4 * 480 * signX + Vector2.UnitY * 600 * sign;
                    var warpTarget = centerVector + Vector2.UnitX * 1800 * signX;
                    proj = Projectile.NewProjectileDirect(NPC.GetProjectileSource(),start2, Vector2.Zero, ModContent.ProjectileType<SkyWarpMark>(),
                            0, 0f, Main.myPlayer, warpTarget.X, warpTarget.Y);
                    proj.localAI[0] = -1;
                    Main.projectile[lastId].localAI[0] = proj.whoAmI;
                    proj.netUpdate = true;
                    WarpState = 1;
                    NPC.ai[3]++;
                    NPC.netUpdate = true;
                }
            }
            else
            {
                if (WarpState == 0)
                {
                    /*if (npc.ai[2] % 180 == 0 && npc.ai[3] < 3 && Main.netMode != NetmodeID.MultiplayerClient)
                    {
                        var target = player.Center + player.velocity * 60;
                        WarpState = 1;
                        Projectile proj = Projectile.NewProjectileDirect(npc.Center + npc.velocity * 90, Vector2.Zero,
                            ModContent.ProjectileType<SkyWarpMark>(), 0, 0f, Main.myPlayer, target.X, target.Y);
                        proj.localAI[0] = -1;
                        proj.netUpdate = true;
                        if (WarpMark == -1)
                        {
                            WarpMark = proj.whoAmI;
                        }
                        else if (Util.CheckProjAlive<SkyWarpMark>(WarpMark))
                        {
                            Main.projectile[WarpMark].localAI[0] = proj.whoAmI;
                            proj.localAI[1] = WarpMark;
                            Main.projectile[WarpMark].netUpdate = true;
                        }
                        npc.ai[3]++;
                    }*/
                    NPC.WormMovementEx(player.Center + player.DirectionTo(NPC.Center) * 1800, maxSpeed, turnAcc, ramAcc);
                    if (Util.CheckProjAlive<SkyWarpMark>(WarpMark))
                    {
                        if (Main.projectile[WarpMark].localAI[0] != -1)
                        {
                            WarpMark = (int)Main.projectile[WarpMark].localAI[0];
                            WarpState = 1;
                            NPC.ai[3] = 0;
                        }
                        else
                        {
                            NPC.ai[3] = 1;
                        }
                    }
                    int i = NPC.whoAmI;
                    int counter = 0;
                        
                    if ((!IsWarping() && NPC.ai[3] == 1) || NPC.ai[2] >= 1800)
                    {
                        i = NPC.whoAmI;
                        counter = 0;
                        while (i != -1)
                        {
                            counter++;
                            NPC tmpNPC = Main.npc[i];
                            tmpNPC.localAI[2] = 0;
                            tmpNPC.localAI[3] = -1;
                            tmpNPC.netUpdate = true;
                            i = (int)Main.npc[i].ai[0];
                        }
                        WarpState = 0;
                        WarpMark = -1;
                        if (Util.CheckProjAlive<SkyPlasmaArena>((int)NPC.localAI[0]))
                        {
                            Main.projectile[(int)NPC.localAI[0]].Kill();
                        }
                        Util.KillAll<SkyWarpMark>(true, 2);
                        CurrentModule.SwitchToNext(this);
                    }
                }
                else if (WarpState == 1)
                {
                    if (Util.CheckProjAlive<SkyWarpMark>(WarpMark))
                    {
                        Projectile mark = Main.projectile[WarpMark];
                        if (NPC.DistanceSQ(mark.Center) <= warpDistance * warpDistance)
                        {
                            WarpState = 0;
                            mark.localAI[1]++;

                            if (Main.netMode != NetmodeID.MultiplayerClient && mark.localAI[1] != 0)
                            {
                                var list = Enumerable.Range(0, 9).ToList();
                                int gaps = Main.rand.Next(2, 4);
                                for (int i = 0; i < gaps; i++)
                                {
                                    int remove = Main.rand.Next(0, list.Count());
                                    list.RemoveAt(remove);
                                }

                                foreach (int i in list)
                                {
                                    var start = new Vector2(mark.Center.X, centerVector.Y - 600);
                                    var end = new Vector2(mark.Center.X, centerVector.Y + 600);
                                    var pos = Vector2.Lerp(start, end, (float)(i + 1) / 10);
                                    var velo = Math.Sign(player.Center.X - NPC.Center.X) * Vector2.UnitX * 9f;
                                    Projectile.NewProjectile(NPC.GetProjectileSource(),pos, velo, ModContent.ProjectileType<SkyPlasmaRayLauncher>(),
                                        NPC.damage / 6, 0f, Main.myPlayer, velo.ToRotation(), 3600);
                                }
                            }

                            NPC.Center = mark.ProjAIToVector();
                        }
                        else
                        {
                            NPC.velocity = (mark.Center - NPC.Center).SafeNormalize(Vector2.Zero) * maxSpeed / 2;
                        }
                    }
                    else
                    {
                        WarpMark = FindFirstWarpMark();
                    }
                }
            }
        }
        public override void PostAI()
        {
            if (DynDRTimer == 0)
            {
                if (NPC.life < lastHealth - NPC.lifeMax / 4200)
                {
                    DynDR = Math.Max(DynDR - 0.01f, 1 - ((float)NPC.lifeMax / 4200 / (lastHealth - NPC.life)));
                    DynDR = Math.Max(DynDR, 0.45f);
                }
                else
                {
                    DynDR = Math.Max(DynDR - 0.005f, 0.5f);
                }
                lastHealth = NPC.life;
            }
            DynDR = Math.Max(DynDR, 0.35f);
            DynDRTimer++;
            if (DynDRTimer > 3) DynDRTimer = 0;
            //Main.NewText(DynDR.ToString());
            base.PostAI();
        }

        public bool phase3 = false;
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            cooldownSlot = 1;
            return true;
        }
        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Electrified, 720, true);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            //Mod Mod = ModLoader.GetMod("TeaNPC");
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[base.NPC.type].Value;
            Texture2D texture2D = Mod.Assets.Request<Texture2D>("Glow/NPCs/SkyDestroyerHeadGlow").Value;
            Color glowColor = Color.White;
            SpriteEffects effects = (NPC.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var mainColor = drawColor;
            spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Rectangle?(NPC.frame), mainColor * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            spriteBatch.Draw(texture2D, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Rectangle?(NPC.frame), glowColor * 0.75f * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);

            #region Effects
            if (NPC.ai[1] == Plasmerizer)
            {
                /*if (npc.ai[2] % 300 >= 240)
                {
                    float timer = npc.ai[2] % 300 - 240;
                    if (timer <= 60)
                    {
                        Color alpha = Color.BlueViolet;
                        if (timer <= 10)
                        {
                            alpha *= timer / 10f;
                        }
                        else if (timer >= 35 && timer <= 60)
                        {
                            alpha *= (60 - timer) / 25;
                        }
                        npc.DrawAim(spriteBatch, npc.Center + npc.rotation.ToRotationVector2() * 3600, alpha);
                    }
                }*/
            }
            #endregion
            return false;
        }
        public override bool CheckActive()
        {
            return this.canDespawn;
        }
        public bool canDespawn;
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(base.NPC.position, base.NPC.width, base.NPC.height, MyDustId.CyanShortFx1, (float)hitDirection, -1f, 0, default(Color), 1f);
            }
            if (base.NPC.life <= 0)
            {
                for (int j = 0; j < 20; j++)
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
        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.SuperHealingPotion;
        }
        public override bool CheckDead()
        {
            return true;
        }
        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            base.NPC.lifeMax = (int)((float)base.NPC.lifeMax * 0.8f * bossLifeScale);
            base.NPC.damage = (int)((float)base.NPC.damage * 0.675f);
        }

        public void SDMessage(string message)
        {
            Main.NewText(message, new Color(119, 213, 255));
        }
        public bool IsShakescreenEnabled()
        {
            return CurrentModule.ID > 0;
        }
    }
}
