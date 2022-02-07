using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using TeaNPCMartianAddon.UI;

namespace TeaNPCMartianAddon
{
    internal class TeaNPCMartianModSystem:ModSystem
    {
        private GameTime _lastUpdateUiGameTime;
        public override void UpdateUI(GameTime gameTime)
        {
            _lastUpdateUiGameTime = gameTime;
            if (TeaNPCMartianAddon.Instance.TitleUILayer.CurrentState != null)
            {
                TeaNPCMartianAddon.Instance.TitleUILayer.Update(gameTime);
            }
            base.UpdateUI(gameTime);
        }
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int num2 = layers.FindIndex((GameInterfaceLayer layer) => layer.Name.Equals("Vanilla: Mouse Text"));
            if (num2 != -1)
            {
                layers.Insert(num2, new LegacyGameInterfaceLayer("TeaNPCAddon:Title", delegate
                {
                    if (TitleUI.IsVisible && _lastUpdateUiGameTime != null && TeaNPCMartianAddon.Instance.TitleUILayer.CurrentState != null)
                    {
                        TeaNPCMartianAddon.Instance.TitleUILayer.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                    }
                    return true;
                }, InterfaceScaleType.UI));
            }
            base.ModifyInterfaceLayers(layers);
        }
    }
}
