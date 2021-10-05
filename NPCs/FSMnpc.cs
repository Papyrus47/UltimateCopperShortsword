using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace UltimateCopperShortsword.NPCs
{
    public abstract class FSMnpc : ModNPC
    {
        protected float Time1
        {
            get => npc.ai[0];
            set => npc.ai[0] = value;
        }
        protected float Time2
        {
            get => npc.ai[1];
            set => npc.ai[1] = value;
        }
        protected float State1
        {
            get => npc.ai[2];
            set => npc.ai[2] = value;
        }
        protected float State2
        {
            get => npc.ai[3];
            set => npc.ai[3] = value;
        }
        /// <summary>
        /// npc的目标
        /// </summary>
        protected Player target => Main.player[npc.target];
        /// <summary>
        /// 绘制npc拖尾
        /// </summary>
        /// <param name="npc">npc</param>
        /// <param name="drawColor">drawColor</param>
        /// <param name="TailColor">拖尾颜色</param>
        protected static void NpcDrawTail(NPC npc, Color drawColor, Color TailColor)
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

            for (int i = 1; i < 7; i += 2)
            {
                Color color = Color.Lerp(drawColor, TailColor, 0.5f);
                color = npc.GetAlpha(color);
                color *= (float)(7 - i) / 15f;
                Vector2 DrawPosition = npc.oldPos[i] + new Vector2((float)npc.width, (float)npc.height) / 2f - Main.screenPosition;
                DrawPosition -= new Vector2((float)NPCTexture.Width, (float)(NPCTexture.Height / frameCount)) * npc.scale / 2f;
                DrawPosition += DrawOrigin * npc.scale + new Vector2(0f, 4f + npc.gfxOffY);
                Main.spriteBatch.Draw(NPCTexture, DrawPosition, new Rectangle?(npc.frame), color, npc.rotation, DrawOrigin, npc.scale, spriteEffects, 0f);
            }
        }
        /// <summary>
        /// 绘制粒子状的预判线
        /// </summary>
        /// <param name="max">预判线最远距离</param>
        /// <param name="step">每个粒子的间隔</param>
        /// <param name="unit">预判线的方向</param>
        /// <param name="center">预判线产生中心</param>
        /// <param name="DustID">粒子的ID</param>
        /// <param name="alpha">粒子透明度</param>
        /// <param name="scale">粒子放大倍数</param>
        public void DustAnticipation(float max, float step, Vector2 unit, Vector2 center, int DustID, int alpha = 100, float scale = 1f)
        {
            for (float i = 0f; i < max; i += step)
            {
                var dust = Dust.NewDustDirect(center + unit * i, 1, 1, DustID, 0, 0, alpha, Color.White, scale);
                dust.noGravity = true;
                dust.velocity *= 0f;
            }
        }
    }
}
