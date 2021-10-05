using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ModLoader;
using System.IO;
using Terraria;
using Terraria.UI;
using Terraria.DataStructures;
using Terraria.GameContent.UI;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.Localization;
using ReLogic.Graphics;
using Microsoft.Xna.Framework;
using Terraria.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameContent.UI.States;
using Terraria.Map;
using Terraria.Net;
using MonoMod.RuntimeDetour;
using System.Reflection;
using System.Drawing;

namespace UltimateCopperShortsword
{
    class GItem : GlobalItem
    {
        public override bool CanUseItem(Item item, Player player)
        {
            ShortSwordPlayer swordPlayer = player.GetModPlayer<ShortSwordPlayer>();
            if(!swordPlayer.EGO && swordPlayer.PlayerVectorZero > 0)
            {
                return false;
            }
            return true;
        }
    }
}
