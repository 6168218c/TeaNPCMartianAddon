using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace TeaNPCMartianAddon
{
	public class TeaNPCMartianAddon : Mod
	{
        public static TeaNPCMartianAddon Instance { get; private set; }
        public static Effect Trail { get; set; }
        public override void Load()
        {
            base.Load();
            if (!Main.dedServ)
            {
                Trail = Assets.Request<Effect>("Effects/Trail").Value;

                Ref<Effect> circularRef = new Ref<Effect>(Assets.Request<Effect>("Effects/CircularDistort").Value);
                Filters.Scene["TeaNPCAddon:CircularDistort"] = new Filter(new ScreenShaderData(circularRef, "CircularDistort"), EffectPriority.VeryHigh);
                Filters.Scene["TeaNPCAddon:CircularDistort"].Load();

                On.Terraria.Graphics.Effects.FilterManager.EndCapture += Projectiles.Boss.SkyDestroyer.SkyAntimatterExplosion.DrawWithShaders_EndCapture;
            }
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
            if (!Main.dedServ)
            {
                On.Terraria.Graphics.Effects.FilterManager.EndCapture -= Projectiles.Boss.SkyDestroyer.SkyAntimatterExplosion.DrawWithShaders_EndCapture;

                Trail = null;
            }
            base.Unload();
        }
    }
}