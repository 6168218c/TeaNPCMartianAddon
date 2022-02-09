using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.UI;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Graphics;

namespace TeaNPCMartianAddon.UI
{
    class TitleUI:UIState
    {
        string quote;
        Vector2 position;
        public static bool IsVisible = false;
        int fadeTime;
        int maxTime;
        int timer;
        public void ShowTitle(string text, Vector2? pos = null, int fade = 60, int max = 300)
        {
            if (pos == null) pos = new Vector2(Main.screenWidth / 2, Main.screenHeight / 12);
            quote = text;
            position = pos.Value;
            fadeTime = fade;
            maxTime = max;
            timer = 0;
            IsVisible = true;
        }
        public override void Update(GameTime gameTime)
        {
            if (timer <= maxTime)
            {
                timer++;
            }
            if (timer > maxTime)
            {
                IsVisible = false;
            }
            base.Update(gameTime);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible) return;
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            float opacity = 1;
            if (timer <= fadeTime) opacity = (float)timer / fadeTime;
            else if (timer >= maxTime-fadeTime && timer < maxTime) opacity = (float)(maxTime - timer) / fadeTime;
            else if (timer >= maxTime) opacity = 0;
            GameShaders.Misc["TeaNPCAddon:FadeIn"].UseImage1("Images/Misc/Perlin").UseOpacity(opacity).UseColor(Color.Turquoise).Apply();
            var font = Terraria.GameContent.FontAssets.DeathText.Value;
            var text = quote;
            Vector2 offset = font.MeasureString(text);
            spriteBatch.DrawString(font, text, position*Main.GameViewMatrix.Zoom - offset / 2, Color.Turquoise);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            base.Draw(spriteBatch);
        }
    }
}
