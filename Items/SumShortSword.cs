using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UltimateCopperShortsword.NPCs.Bosses;

namespace UltimateCopperShortsword.Items
{
    public class SumShortSword : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜短剑");
            Tooltip.SetDefault("怒火照耀全身");
            ItemID.Sets.SortingPriorityBossSpawns[item.type] = 13;
        }
        public override void SetDefaults()
        {
            item.width = 20;
            item.height = 20;
            item.maxStack = 20;
            item.value = 100;
            item.rare = ItemRarityID.Blue;
            item.useAnimation = 30;
            item.useTime = 30;
            item.useStyle = ItemUseStyleID.HoldingUp;
            item.consumable = true;
        }
        public override void AddRecipes()
        {
            ModRecipe recipe = new ModRecipe(mod);
            recipe.AddIngredient(ItemID.CopperBar, 10);
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.SetResult(this, 1);
            recipe.AddRecipe();
        }
        public override bool CanUseItem(Player player)
        {
            foreach(NPC npc in Main.npc)
            {
                if ((npc.type == ModContent.NPCType<ShortSword>() && npc.active)|| 
                    (npc.type == ModContent.NPCType<ShortSword2>() && npc.active)||
                    (npc.type == ModContent.NPCType<ShortSword3>() && npc.active))
                {
                    return false;
                }
            }
            return true;
        }
        public override bool UseItem(Player player)
        {
            NPC.SpawnOnPlayer(player.whoAmI,ModContent.NPCType<ShortSword>());
            Main.PlaySound(SoundID.Roar, player.position, 0);
            return true;
        }
    }
}
