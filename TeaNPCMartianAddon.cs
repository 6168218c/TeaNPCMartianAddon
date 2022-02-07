using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.UI;
using TeaNPCMartianAddon.UI;
using TeaNPCMartianAddon.Effects;

namespace TeaNPCMartianAddon
{
	public class TeaNPCMartianAddon : Mod
	{
        public static TeaNPCMartianAddon Instance { get; private set; }
        public static Effect Trail { get; set; }
        public static Effect FadeIn { get; set; }
        public static Effect PortalEffect { get; set; }
        internal UserInterface TitleUILayer { get; set; }
        internal TitleUI TitleUI { get; set; }
        public override void Load()
        {
            base.Load();
            if (!Main.dedServ)
            {
                Trail = Assets.Request<Effect>("Effects/Trail").Value;
                FadeIn = Assets.Request<Effect>("Effects/FadeIn").Value;
                PortalEffect = Assets.Request<Effect>(("Effects/PortalEffect")).Value;

                TitleUILayer = new UserInterface();
                TitleUI = new TitleUI();
                TitleUILayer.SetState(TitleUI);
                TitleUI.Activate();

                GameShaders.Misc["TeaNPCAddon:FadeIn"] = new MiscShaderData(new Ref<Effect>(FadeIn), "FadeIn").UseImage0("Images/Misc/Perlin");

                Ref<Effect> circularRef = new Ref<Effect>(Assets.Request<Effect>("Effects/CircularDistort").Value);
                Filters.Scene["TeaNPCAddon:CircularDistort"] = new Filter(new ScreenShaderData(circularRef, "CircularDistort"), EffectPriority.VeryHigh);
                Filters.Scene["TeaNPCAddon:CircularDistort"].Load();

                Ref<Effect> blackoutRef = new Ref<Effect>(Assets.Request<Effect>("Effects/Blackout").Value);
                Filters.Scene["TeaNPCAddon:RectBlackout"] = new Filter(new FadeScreenShader(blackoutRef, "RectBlackout"), EffectPriority.VeryHigh);
                Filters.Scene["TeaNPCAddon:RectBlackout"].Load();
                Filters.Scene["TeaNPCAddon:CircularBlackout"] = new Filter(new FadeScreenShader(blackoutRef, "CircularBlackout"), EffectPriority.VeryHigh);
                Filters.Scene["TeaNPCAddon:CircularBlackout"].Load();
            }
            Instance = this;
        }

        public override void Unload()
        {
            Instance = null;
            if (!Main.dedServ)
            {

                Trail = null;
                FadeIn = null;

                TitleUILayer = null;
                TitleUI = null;
            }
            base.Unload();
        }
    }
}