using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;

namespace TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer
{
    public class SkySearcherTail : SkySearcherBody
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("天际搜寻者");
        }
        public override void SetDefaults()
        {
            base.NPC.damage = 0;
            base.NPC.width = 34;
            base.NPC.height = 32;
            base.NPC.defense = 50;
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
        public override bool CheckActive()
        {
            return false;
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture2D = Mod.Assets.Request<Texture2D>("TeaNPCAddon/Glow/NPCs/SkySearcherTailGlow").Value;
            Texture2D texture = Terraria.GameContent.TextureAssets.Npc[base.NPC.type].Value;
            Color glowColor = Color.White;
            SpriteEffects effects = (NPC.direction < 0) ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            var mainColor = drawColor;
            spriteBatch.Draw(texture, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Rectangle?(NPC.frame), mainColor * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
            spriteBatch.Draw(texture2D, NPC.Center - screenPos + new Vector2(0f, NPC.gfxOffY), new Rectangle?(NPC.frame), glowColor * 0.75f * NPC.Opacity, NPC.rotation + MathHelper.Pi / 2, NPC.frame.Size() / 2f, NPC.scale, effects, 0f);
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
                    Dust.NewDust(base.NPC.position, base.NPC.width, base.NPC.height, MyDustId.ElectricCyan, (float)hitDirection, -1f, 0, default(Color), 1f);
                }
                Gore.NewGore(base.NPC.position, base.NPC.velocity, ModContent.Find<ModGore>("Gores/Martians/SDminionTailGore").Type, 1f);
            }
        }
        public float speed = 6f;
        public float turnSpeed = 0.1f;
    }
}
