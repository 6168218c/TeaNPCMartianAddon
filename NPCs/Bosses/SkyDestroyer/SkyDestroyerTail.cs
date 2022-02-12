using Terraria.Localization;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;

namespace TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer
{
    public class SkyDestroyerTail : SkyDestroyerBody
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("The Sky Destroyer");
            //DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "天际毁灭者");
        }
        public override void SetDefaults()
        {
            NPC.damage = baseDamage * 3 / 4;
            NPC.width = 150;
            NPC.height = 150;
            NPC.defense = 40;
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
        public override void HitEffect(int hitDirection, double damage)
        {
            for (int i = 0; i < 5; i++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, MyDustId.CyanShortFx1, (float)hitDirection, -1f, 0, default(Color), 1f);
            }
            if (NPC.life <= 0)
            {
                BodyPreventDeath();
                for (int j = 0; j < 20; j++)
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
            texture2D = Mod.RequestTexture("Glow/NPCs/SkyDestroyerTailGlow");
            Color glowColor = GetGlowColor();
            SpriteEffects effects = (NPC.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var mainColor = NPC.GetAlpha(drawColor);
            spriteBatch.Draw(texture, GetDrawPosition(screenPos), new Rectangle?(NPC.frame), mainColor * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            spriteBatch.Draw(texture2D, GetDrawPosition(screenPos), new Rectangle?(NPC.frame), glowColor * 0.75f * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            return false;
        }
        public override void OnHitPlayer(Player player, int damage, bool crit)
        {
            player.AddBuff(BuffID.Electrified, 360, true);
        }
        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return new bool?(false);
        }
    }
}
