
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.Localization;
using TeaNPCMartianAddon.NPCs.Bosses.SkyDestroyer;

namespace TeaNPCMartianAddon.Items.Bosses.Martians
{
    public class HighTechMechanicalWorm : ModItem
    {
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("High-tech mechanical worm");
            //DisplayName.AddTranslation(GameCulture.Chinese, "高科技机械蠕虫");
            //Tooltip.SetDefault("It seems to be used to call what machine...\nUse it on night.");
            //Tooltip.AddTranslation(GameCulture.Chinese, "似乎是用来呼叫什么机器用的。\n晚上用。");
            ItemID.Sets.SortingPriorityBossSpawns[Item.type] = 13;
        }
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 18;
            Item.useAnimation = 45;
            Item.useTime = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.consumable = false;
            Item.rare = ItemRarityID.Red;
            Item.value = Item.sellPrice(0, 1, 25, 0);
        }
        public override bool CanUseItem(Player player)
        {
            return !NPC.AnyNPCs(ModContent.NPCType<SkyDestroyerHead>()); //&& !Main.dayTime;
        }
        public override bool? UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI, ModContent.NPCType<SkyDestroyerHead>());
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Roar, player.position, 0);
            //SDMessage(Localization.Lang.BossSummon("SDSummon"));
            return true;
        }
        public void SDMessage(string message)
        {
            Main.NewText(message, 119, 213, 255);
        }
        public override void AddRecipes()
        {
            /*ModRecipe modRecipe = new ModRecipe(base.mod);
            modRecipe.AddIngredient(ItemID.MartianConduitPlating, 20);
            modRecipe.AddIngredient(ItemID.LunarBar, 5);
            modRecipe.AddIngredient(ItemID.Wire, 20);
            modRecipe.AddTile(TileID.LunarCraftingStation);
            modRecipe.SetResult(this, 1);
            modRecipe.AddRecipe();*/
            CreateRecipe()
                .AddIngredient(ItemID.MartianConduitPlating, 20)
                .AddIngredient(ItemID.LunarBar, 5)
                .AddIngredient(ItemID.Wire)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
