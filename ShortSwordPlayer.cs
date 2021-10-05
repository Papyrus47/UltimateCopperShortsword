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
using UltimateCopperShortsword.NPCs.Bosses;

namespace UltimateCopperShortsword
{
    public class ShortSwordPlayer : ModPlayer
    {
        public bool SwordSum = false;
        public bool EGO = false;
        public int PlayerEmotion = 0;
        public int PlayerVectorZero = 0;
        public override void ResetEffects()
        {
            SwordSum = false;
            if (EGO)
            {
                PlayerVectorZero = 300;
                player.allDamage += 3;
                player.moveSpeed += 5;
                player.accRunSpeed += 5;
                player.maxRunSpeed += 5;
                player.meleeSpeed += 1.2f;
                player.noFallDmg = true;
                player.fallStart += 10;
                player.maxFallSpeed += 10;
                if(player.HeldItem.type == ModContent.ItemType<Items.LastShortSowrd>()) player.maxRunSpeed += 5;
            }
            else if(PlayerVectorZero > 0 && !EGO)
            {
                PlayerVectorZero--;
            }
            EGO = false;
            if(PlayerEmotion < 0)
            {
                PlayerEmotion = 0;
            }
            if (PlayerEmotion < 40)
            {
                EGO = false;
                player.ClearBuff(ModContent.BuffType<Buffs.CopperBuff>());
            }
            if (PlayerEmotion > 100)
            {
                PlayerEmotion = 100;
            }
            if (Filters.Scene["Screen"].IsActive())
            {
                Filters.Scene.Deactivate("Screen");
            }
            if (Filters.Scene["ScreenTwo"].IsActive())
            {
                Filters.Scene.Deactivate("ScreenTwo");
            }
        }
        public override void OnHitNPC(Item item, NPC target, int damage, float knockback, bool crit)
        {
            PlayerEmotion++;
        }
        public override void OnHitNPCWithProj(Projectile proj, NPC target, int damage, float knockback, bool crit)
        {
            if(Main.rand.Next(3)==0)
            {
                PlayerEmotion++;
            }
        }
        public override void OnHitByNPC(NPC npc, int damage, bool crit)
        {
            PlayerEmotion--;
        }
        public override void OnHitByProjectile(Projectile proj, int damage, bool crit)
        {
            if (Main.rand.Next(3) == 0)
            {
                PlayerEmotion--;
            }
        }
        public override void SetControls()
        {
            if (PlayerVectorZero > 0 && !EGO)
            {
                PlayerVectorZero--;
                player.controlUp = false;
                player.controlDown = false;
                player.controlLeft = false;
                player.controlRight = false;
                player.controlUseItem = false;
                player.gravControl = false;
                player.gravControl2 = false;
                player.controlSmart = false;
                player.controlThrow = false;
                player.controlTorch = false;
                player.controlQuickHeal = false;
                player.controlQuickMana = false;
                player.controlMount = false;
                player.controlMap = false;
                player.controlInv = false;
                player.controlUseTile = false;
                player.controlHook = false;
                Main.mapFullscreen = false;
                Main.playerInventory = false;
            }
        }
        public override void PreUpdateMovement()
        {
            if (PlayerVectorZero > 0 && !EGO)
            {
                player.velocity = Vector2.Zero;
                //player.direction = Math.Sign(Main.screenPosition.X - player.Center.X);
            }
        }
    }
}
