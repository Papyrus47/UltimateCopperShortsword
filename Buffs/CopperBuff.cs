using Terraria.ModLoader;
using Terraria;

namespace UltimateCopperShortsword.Buffs
{
    class CopperBuff : ModBuff
    {
        public override void SetDefaults()
        {
            DisplayName.SetDefault("铜化");
            Description.SetDefault("变成铜！\n" +
                "获得最后的铜短剑力量");
            Main.buffNoTimeDisplay[Type] = true;
            Main.buffNoSave[Type] = true;
            canBeCleared = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            ShortSwordPlayer shortSword = player.GetModPlayer<ShortSwordPlayer>();
            shortSword.EGO = true;
            if(shortSword.EGO)
            {
                player.buffTime[buffIndex] = 999;
            }
            if(player.dead)
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}
