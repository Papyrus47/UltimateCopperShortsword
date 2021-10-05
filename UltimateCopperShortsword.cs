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

namespace UltimateCopperShortsword
{
	public class UltimateCopperShortsword : Mod
	{
		public static Effect ScreenTwo;
        public static Effect Screen;
        public override void Load()
        {
            if(!Main.dedServ)
            {
                Screen = GetEffect("Effects/Screen");
                Filters.Scene["Screen"] = new Filter
                    (new NewShader(new Ref<Effect>(GetEffect("Effects/Screen")), "GreenSword"),
                    EffectPriority.Medium);
                Filters.Scene["Screen"].Load();
                ScreenTwo = GetEffect("Effects/Screen");
                Filters.Scene["ScreenTwo"] = new Filter
                    (new NewShader(new Ref<Effect>(GetEffect("Effects/Screen")), "GreenSwordTwo"),
                    EffectPriority.Medium);
                Filters.Scene["ScreenTwo"].Load();
            }
        }
        public override void Unload()
        {
            Screen = null;
            ScreenTwo = null;
        }
        public override void UpdateMusic(ref int music, ref MusicPriority priority)
        {
            if (Main.gameMenu)
            {
                return;
            }
            Player player = Main.LocalPlayer;
            ShortSwordPlayer swordPlayer = player.GetModPlayer<ShortSwordPlayer>();
            if(swordPlayer.EGO)
            {
                music = GetSoundSlot(SoundType.Music, "Sounds/Music/RedMist");
                priority = MusicPriority.BossHigh;
            }
        }
    }
    public class NewShader : ScreenShaderData
    {
        public NewShader(string passName) : base(passName)
        {
        }
        public NewShader(Ref<Effect> shader, string passName) : base(shader, passName)
        {
        }
        public override void Apply()
        {
            base.Apply();
        }
    }
    public static class TheWayOfSword
    {
        public static void SaveOldRot(NPC npc)
        {
            for (int j = npc.oldRot.Length - 1; j > 0; j--)
            {
                npc.oldRot[j] = npc.oldRot[j - 1];
            }
            npc.oldRot[0] = npc.rotation;
        }
        public static void NpcDrawTail(NPC npc, Color drawColor, Color TailColor)
        {
            Texture2D NPCTexture = Main.npcTexture[npc.type];
            SpriteEffects spriteEffects = 0;
            if (npc.spriteDirection == 1)
            {
                spriteEffects = SpriteEffects.FlipHorizontally;
            }
            int frameCount = Main.npcFrameCount[npc.type];
            Vector2 DrawOrigin;
            DrawOrigin = new Vector2((float)(Main.npcTexture[npc.type].Width / 2), (float)(Main.npcTexture[npc.type].Height / frameCount / 2));

            for (int i = 1; i < npc.oldPos.Length; i += 2)
            {
                Color color = Color.Lerp(drawColor, TailColor, 0.5f);
                color = npc.GetAlpha(color);
                color *= (float)(npc.oldPos.Length - i) / 15f;
                Vector2 DrawPosition = npc.oldPos[i] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
                DrawPosition -= new Vector2((float)NPCTexture.Width, (float)(NPCTexture.Height / frameCount)) * npc.scale / 2f;
                DrawPosition += DrawOrigin * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
                Main.spriteBatch.Draw(NPCTexture, DrawPosition, new Rectangle?(npc.frame), color,npc.oldRot[i], DrawOrigin, npc.scale, spriteEffects, 0f);
            }
        }
    }
}