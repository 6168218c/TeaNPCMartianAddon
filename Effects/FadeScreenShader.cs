using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TeaNPCMartianAddon.Effects
{
    public class FadeScreenShader:ScreenShaderData
    {
        public FadeScreenShader(string passName) : base(passName) { }
        public FadeScreenShader(Ref<Effect> shader, string passName)
            : base(shader, passName)
        {
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
