using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Terraria.IO;
using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace UltimateCopperShortsword
{
    public static class BaseUlCoShortsword
    {
        public static NPC NewNPCDirect(this NPC npc, int X, int Y, int Type, int Start = 0,float ai0=0,
            float ai1 = 0,float ai2 =0,float ai3=0,int Target = 255)
        {
            int v = NPC.NewNPC(X, Y, Type, Start, ai0, ai1, ai2, ai3, Target);
            return Main.npc[v];//返回这个npc的whoAmI
        }
        public static float RadToDeg(this float rad)
        {
            return rad * (180 / MathHelper.Pi);
        }
        public static float DegToRad(this float deg)
        {
            return deg * (MathHelper.Pi / 180);
        }
    }
}
