using Terraria.Audio;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer;
using Microsoft.Xna.Framework;

namespace TeaNPCMartianAddon.Items.Bosses.Martians
{
    public class HighTechMechanicalWorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("High-tech mechanical worm");
            //DisplayName.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "高科技机械蠕虫");
            //Tooltip.SetDefault("It seems to be used to call what machine...\nUse it on night.");
            //Tooltip.AddTranslation(GameCulture.FromCultureName(GameCulture.CultureName.Chinese), "似乎是用来呼叫什么机器用的。\n晚上用。");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
        }
        public override void SetDefaults()
        {
            base.Item.width = 28;
            base.Item.height = 18;
            base.Item.useAnimation = 45;
            base.Item.useTime = 45;
            base.Item.useStyle = ItemUseStyleID.HoldUp;
            base.Item.consumable = false;
            base.Item.rare = ItemRarityID.Red;
            base.Item.value = Item.sellPrice(0, 1, 25, 0);
        }
        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<SkyDestroyerHead>()) && !Main.dayTime;
        }
        public override bool? UseItem(Player player)
        {
			int spawnRangeX = (int)((double)(NPC.sWidth / 16) * 0.7);
			int spawnRangeY = (int)((double)(NPC.sHeight / 16) * 0.7);
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                return true;
            }
			bool flag = false;
			int num11 = 0;
			int num12 = 0;
			int num13 = (int)(player.position.X / 16f) - spawnRangeX * 2;
			int num14 = (int)(player.position.X / 16f) + spawnRangeX * 2;
			int num15 = (int)(player.position.Y / 16f) - spawnRangeY * 2;
			int num16 = (int)(player.position.Y / 16f) + spawnRangeY * 2;
			int num17 = (int)(player.position.X / 16f) - NPC.safeRangeX;
			int num18 = (int)(player.position.X / 16f) + NPC.safeRangeX;
			int num19 = (int)(player.position.Y / 16f) - NPC.safeRangeY;
			int num20 = (int)(player.position.Y / 16f) + NPC.safeRangeY;
			if (num13 < 0)
			{
				num13 = 0;
			}
			if (num14 > Main.maxTilesX)
			{
				num14 = Main.maxTilesX;
			}
			if (num15 < 0)
			{
				num15 = 0;
			}
			if (num16 > Main.maxTilesY)
			{
				num16 = Main.maxTilesY;
			}
			for (int l = 0; l < 1000; l++)
			{
				for (int m = 0; m < 100; m++)
				{
					int num21 = Main.rand.Next(num13, num14);
					int num22 = Main.rand.Next(num15, num16);
					if (!Main.tile[num21, num22].IsActiveUnactuated || !Main.tileSolid[Main.tile[num21, num22].type])
					{
						if ((Main.wallHouse[Main.tile[num21, num22].wall] && l < 999))
						{
							continue;
						}
						for (int n = num22; n < Main.maxTilesY; n++)
						{
							if (Main.tile[num21, n].IsActiveUnactuated && Main.tileSolid[Main.tile[num21, n].type])
							{
								if (num21 < num17 || num21 > num18 || n < num19 || n > num20 || l == 999)
								{
									_ = Main.tile[num21, n].type;
									num11 = num21;
									num12 = n;
									flag = true;
								}
								break;
							}
						}
						if (flag && l < 999)
						{
							int num24 = num11 - 3 / 2;
							int num25 = num11 + 3 / 2;
							int num26 = num12 - 3;
							int num27 = num12;
							if (num24 < 0)
							{
								flag = false;
							}
							if (num25 > Main.maxTilesX)
							{
								flag = false;
							}
							if (num26 < 0)
							{
								flag = false;
							}
							if (num27 > Main.maxTilesY)
							{
								flag = false;
							}
							if (flag)
							{
								for (int num28 = num24; num28 < num25; num28++)
								{
									for (int num29 = num26; num29 < num27; num29++)
									{
										if (Main.tile[num28, num29].IsActiveUnactuated && Main.tileSolid[Main.tile[num28, num29].type])
										{
											flag = false;
											break;
										}
									}
								}
							}
						}
					}
					if (flag || flag)
					{
						break;
					}
				}
				if (flag && l < 999)
				{
					Rectangle rectangle = new Rectangle(num11 * 16, num12 * 16, 16, 16);
					for (int num30 = 0; num30 < 255; num30++)
					{
						if (Main.player[num30].active)
						{
							Rectangle rectangle2 = new Rectangle((int)(Main.player[num30].position.X + (float)(Main.player[num30].width / 2) - (float)(NPC.sWidth / 2) - (float)NPC.safeRangeX), (int)(Main.player[num30].position.Y + (float)(Main.player[num30].height / 2) - (float)(NPC.sHeight / 2) - (float)NPC.safeRangeY), NPC.sWidth + NPC.safeRangeX * 2, NPC.sHeight + NPC.safeRangeY * 2);
							if (rectangle.Intersects(rectangle2))
							{
								flag = false;
							}
						}
					}
				}
				if (flag)
				{
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
			int num31 = 200;
			num31 = NPC.NewNPC(num11 * 16 + 8, num12 * 16, ModContent.NPCType<SkyDestroyerHead>(), 1, 0, -2);
			if (num31 == 200)
			{
				return false;
			}
			Main.npc[num31].target = player.whoAmI;
			Main.npc[num31].timeLeft *= 20;
			string typeName3 = Main.npc[num31].TypeName;
			if (Main.netMode == NetmodeID.Server && num31 < 200)
			{
				NetMessage.SendData(MessageID.SyncNPC, -1, -1, null, num31);
			}
			if (Main.netMode == NetmodeID.SinglePlayer)
			{
				Main.NewText(Language.GetTextValue("Announcement.HasAwoken", typeName3), 175, 75);
			}
			else if (Main.netMode == NetmodeID.Server)
			{
				Terraria.Chat.ChatHelper.BroadcastChatMessage(NetworkText.FromKey("Announcement.HasAwoken", Main.npc[num31].GetTypeNetName()), new Color(175, 75, 255));
			}
			SoundEngine.PlaySound(SoundLoader.GetLegacySoundSlot(Mod, "Sounds/Custom/MechanicalRoar"), Main.npc[num31].Center);
			/*if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NPC.NewNPC((int)player.Center.X, (int)player.Center.Y - 450, ModContent.NPCType<SkyDestroyerHead>(), 1, 0, -2);
            }
            Main.PlaySound(SoundID.Roar, player.position, 0);*/
			SDMessage(Language.GetTextValue("Mods.TeaNPCAddon.NPCChat.SDEX0"));
            return true;
        }
        public void SDMessage(string message)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
                Main.NewText(message, 119, 213, 255);
            if (Main.netMode == NetmodeID.Server)
                Terraria.Chat.ChatHelper.BroadcastChatMessage(NetworkText.FromLiteral(message), new Microsoft.Xna.Framework.Color(119, 213, 255));
        }
        public override void AddRecipes()
        {
			Mod.CreateRecipe(Item.type)
				.AddIngredient(ItemID.MartianConduitPlating, 20)
				.AddIngredient(ItemID.LunarBar, 5)
				.AddIngredient(ItemID.Wire, 20)
				.AddTile(TileID.LunarCraftingStation)
				.Register();

		}
    }
}
