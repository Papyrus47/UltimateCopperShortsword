using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace UltimateCopperShortsword
{
    class Gproj : GlobalProjectile
    {
        public override void AI(Projectile projectile)
        {
            if(projectile.type == ProjectileID.FlyingPiggyBank)
            {
                var player = Main.player[projectile.owner];
                projectile.minion = true;
                projectile.timeLeft = 3;
                if (player.dead)
                {
                    projectile.Kill();
                    return;
                }
                else
                {
                    Vector2 vector = new Vector2(player.Center.X, player.Center.Y - 50);
                    Vector2 vector2 = (vector - projectile.Center).SafeNormalize(Vector2.UnitX) * 10;
                    projectile.velocity = (projectile.velocity * 150 + vector2) / 151;
                    float dis = Vector2.Distance(projectile.Center, vector);
                    if(dis< 0.5f)
                    {
                        projectile.velocity = Main.rand.NextFloatDirection().ToRotationVector2() * 1.2f;
                    }
                }
            }
            base.AI(projectile);
        }
    }
}
