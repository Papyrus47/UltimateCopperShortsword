using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.Localization;
using Microsoft.Xna.Framework;
using Terraria.Audio;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using UltimateCopperShortsword.Projs;
using Terraria.Graphics.Effects;

namespace UltimateCopperShortsword.NPCs.Bosses
{
    [AutoloadBossHead]
    public class ShortSword2 : FSMnpc
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Short Sword");
            DisplayName.AddTranslation(GameCulture.Chinese, "铜短剑");
            NPCID.Sets.TrailCacheLength[npc.type] = 20;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 40000;
            npc.defense = 10;
            npc.damage = 90;
            npc.knockBackResist = 0;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.boss = true;
            npc.width = 32;
            npc.height = 32;
            npc.friendly = false;
            npc.aiStyle = -1;
            npc.DeathSound = SoundID.NPCDeath11;
            npc.HitSound = SoundID.NPCHit4;
            music = mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/Atk2");
            musicPriority = MusicPriority.BossHigh;
        }
        public override void AI()
        {
            if (npc.target <= 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
            }
            if (Main.netMode != NetmodeID.Server && !Filters.Scene["Screen"].IsActive())
            {
                // 开启滤镜
                Filters.Scene.Activate("Screen");
            }
            Vector2 ToTarget = (target.Center - npc.Center).SafeNormalize(Vector2.UnitX);
            npc.rotation = ToTarget.ToRotation() + MathHelper.PiOver4;//常态是一直对着玩家的
            int LostSword2 = ModContent.ProjectileType<LostSword2>();
            int LostLaser2 = ModContent.ProjectileType<LostSwordLaser2>();
            if (target.dead)
            {
                npc.life = 0;
                return;
            }
            switch (State1)
            {
                case 0://你承受不了，这铜剑的数量（
                    {
                        npc.velocity = ToTarget;
                        if (Time1 <= 0)
                        {
                            Time1 = 30;
                            if (Main.netMode != 1)
                            {
                                for (float r = 0; r < MathHelper.TwoPi; r += MathHelper.TwoPi / (5 + Time2))
                                {
                                    Vector2 center = target.Center + r.ToRotationVector2() * 800;
                                    Vector2 toCenter = (target.Center - center).SafeNormalize(Vector2.UnitX) * 10;
                                    var proj = Projectile.NewProjectileDirect(center, toCenter, LostSword2,
                                        0, 3.4f, Main.myPlayer, 0, 1);
                                    proj.timeLeft = 10000;
                                    proj.extraUpdates = 2;
                                }
                            }
                            Time2++;
                        }
                        else Time1--;
                        if(Time2 >= 5)
                        {
                            ShootSword();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 1;
                        }
                        break;
                    }
                case 1://横向冲刺，四个方向释放弹幕
                    {
                        if (Time1 <= 0)
                        {
                            Time1 = 60;
                            npc.velocity = (npc.Center.X - target.Center.X > 0) ? new Vector2(-20,0) : new Vector2(20, 0);
                            npc.netUpdate = true;
                        }
                        else Time1--;
                        if(Time1 % 10 ==0)
                        {
                            Vector2[] vector = new Vector2[4];
                            vector[0] = (npc.rotation - MathHelper.PiOver4).ToRotationVector2();
                            //npc正方向上的弹幕
                            vector[1] = (npc.rotation - MathHelper.PiOver4 + MathHelper.PiOver2).ToRotationVector2();
                            //右方向
                            vector[2] = (npc.rotation - MathHelper.PiOver4 - MathHelper.PiOver2).ToRotationVector2();
                            //左方向
                            vector[3] = (npc.rotation - MathHelper.PiOver4 - MathHelper.Pi).ToRotationVector2();
                            //反方向
                            if (Main.netMode != 1)
                            {
                                for (int i = 0; i < vector.Length; i++)
                                {
                                    var proj = Projectile.NewProjectileDirect(npc.Center, vector[i] * 25, LostSword2,
                                        0, 2.3f, Main.myPlayer, 0, 1);
                                    proj.timeLeft = 2000;
                                }
                            }
                            Time2++;
                        }
                        if(Time2 >= 20)
                        {
                            ShootSword();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 2;
                        }
                        break;
                    }
                case 2://跨栏（
                    {
                        npc.velocity = ToTarget * 2;
                        if (Time1 <= 0)
                        {
                            Time1 = 100;
                            if (Main.netMode != 1)
                            {
                                for (int i = 0; i <= 30; i++)
                                {
                                    Vector2 center = target.Center + new Vector2(Main.rand.NextFloat(-2000, 2000), 1000);
                                    Vector2 vector = center - new Vector2(center.X, center.Y + 20);
                                    var proj = Projectile.NewProjectileDirect(center, vector, LostSword2,
                                        0, 2f, Main.myPlayer, 0, 1);
                                    proj.timeLeft = 1000;
                                }
                            }
                            Time2++;
                        }
                        else Time1--;
                        if (Time2 == 1 && Time1 == 60)
                        {
                            ShootSword();
                        }
                        if (Time2 == 2 && Time1 == 60)
                        {
                            foreach (Projectile projectile in Main.projectile)
                            {
                                if (projectile.type == ModContent.ProjectileType<LostSword2>() && projectile.ai[0] == 2 && !projectile.friendly && projectile.hostile)
                                {
                                    projectile.velocity = Vector2.Normalize(new Vector2(projectile.Center.X + 1f, projectile.Center.Y + 3f) - projectile.Center) * 10f;
                                    projectile.ai[0] = 0;
                                    projectile.ai[1] = 1;
                                    projectile.extraUpdates = 3;
                                    projectile.timeLeft = 1000;
                                }
                            }
                        }
                        if(Time2 == 3 && Time1 == 70)
                        {
                            ShootSword();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 3;
                        }
                        break;
                    }
                case 3://散射弹幕
                    {
                        Time1++;
                        npc.velocity = ToTarget * 5f;
                        if (Time1 >= 20 && Main.netMode != 1)
                        {
                            for(int i = -4;i<= 4;i++)
                            {
                                Vector2 center = npc.Center + (npc.rotation - MathHelper.PiOver4).ToRotationVector2() * -200;
                                float r = ToTarget.ToRotation() + i * MathHelper.Pi / 114;
                                Vector2 ToTar = r.ToRotationVector2() * 5;
                                Projectile proj = Projectile.NewProjectileDirect(center, ToTar,
                                    LostSword2, 120, 2.3f, Main.myPlayer,0,1);
                                proj.timeLeft = 5000;
                                proj.extraUpdates = 5;
                            }
                            Time1 = 0;
                            Time2++;
                        }
                        if(Time2 > 10 && Time1 > 10)
                        {
                            ShootSword();
                            HealPlayerLife();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 4;
                        }
                        break;
                    }
                case 4://四面激光
                    {
                        Time1++;
                        npc.dontTakeDamage = true;
                        npc.velocity *= 0.7f;
                        npc.rotation = Time1.DegToRad();
                        float dis = Vector2.Distance(npc.Center, target.Center);
                        for(float i =0;i<MathHelper.TwoPi;i+= MathHelper.TwoPi / 60)
                        {
                            var dust = Dust.NewDustDirect(npc.Center + i.ToRotationVector2() * 2000, 1, 1, DustID.GreenFairy);
                            dust.noGravity = true;
                        }
                        if(dis > 2000)
                        {
                            target.velocity = (npc.Center - target.Center).SafeNormalize(Vector2.One) * 3.3f;
                        }
                        if (Main.netMode != 1)
                        {
                            Vector2[] vector = new Vector2[4];
                            vector[0] = (npc.rotation - MathHelper.PiOver4).ToRotationVector2();
                            //npc正方向上的弹幕
                            vector[1] = (npc.rotation - MathHelper.PiOver4 + MathHelper.PiOver2).ToRotationVector2();
                            //右方向
                            vector[2] = (npc.rotation - MathHelper.PiOver4 - MathHelper.PiOver2).ToRotationVector2();
                            //左方向
                            vector[3] = (npc.rotation - MathHelper.PiOver4 - MathHelper.Pi).ToRotationVector2();
                            //反方向
                            for (int i = 0; i < vector.Length; i++)
                            {
                                if (Time1 < 150)
                                {
                                    Projectile.NewProjectileDirect(npc.Center, vector[i],
                                        LostLaser2, 140, 2f, Main.myPlayer, 1, 0);
                                }
                                else
                                {
                                    Projectile.NewProjectileDirect(npc.Center, vector[i],
                                       LostLaser2, 140, 2f, Main.myPlayer, 0, 0);
                                }
                            }
                        }
                        if (Time1 > 600)
                        {
                            npc.dontTakeDamage = false;
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 5;
                        }
                        break;
                    }
                case 5://激光冲
                    {
                        npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver4;
                        Vector2[] center = new Vector2[4]
                        {
                            new Vector2(npc.Center.X + 1000,npc.Center.Y + 1000),
                            new Vector2(npc.Center.X + 1000,npc.Center.Y - 1000),
                            new Vector2(npc.Center.X - 1000,npc.Center.Y + 1000),
                            new Vector2(npc.Center.X - 1000,npc.Center.Y - 1000),
                        };
                        Vector2[] ves = new Vector2[4]
                        {
                             new Vector2(0,-1),
                             new Vector2(-1,0),
                             new Vector2(1,0),
                             new Vector2(0,1)
                        };
                        if (Main.netMode != 1)
                        {
                            for (int i = 0; i < center.Length; i++)
                            {
                                Projectile.NewProjectileDirect(center[i], ves[i], LostLaser2,
                                    130, 2f, Main.myPlayer);
                            }
                        }
                        if (Time1 <= 0)
                        {
                            Time1 = 100;
                            npc.velocity = ToTarget * 20;
                            Time2++;
                        }
                        else Time1--;
                        if (Time2 > 10)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 6;
                        }
                        break;
                    }
                case 6://反复冲刺的弹幕（
                    {
                        Time1++;
                        npc.velocity = ToTarget;
                        if(Time1 >= 90 && Main.netMode != 1)
                        {
                            if (Time2 == 0)
                            {
                                for (float r = 0; r < MathHelper.TwoPi; r += MathHelper.TwoPi / 30)
                                {
                                    Vector2 vector = target.Center + r.ToRotationVector2() * 1000;
                                    Projectile.NewProjectileDirect(vector, (target.Center - vector)
                                        .SafeNormalize(Vector2.UnitX) * 15, LostSword2, 0, 2f, Main.myPlayer,0,1);
                                }
                            }
                            Time1 = 0;
                            Time2++;
                        }
                        if(Time2 == 1 && Time1 == 60)
                        {
                            ShootSword();
                        }
                        if(Time2 == 2 && Time1 == 60)
                        {
                            foreach (Projectile projectile in Main.projectile)
                            {
                                if (projectile.type == ModContent.ProjectileType<LostSword2>() && projectile.ai[0] == 2 && !projectile.friendly && projectile.hostile)
                                {
                                    projectile.velocity = (target.Center - projectile.Center).SafeNormalize(Vector2.One) * 10;
                                    projectile.ai[0] = 0;
                                    projectile.ai[1] = 1;
                                    projectile.extraUpdates = 3;
                                    projectile.timeLeft = 1000;
                                }
                            }
                        }
                        if(Time2 >= 3 && Time1 >= 10)
                        {
                            ShootSword();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 0;
                        }
                        break;
                    }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Color color = Color.White;
            if(State1 ==1 || State1 == 5) color = Color.DarkGreen;
            TheWayOfSword.NpcDrawTail(npc, drawColor,color);
            TheWayOfSword.SaveOldRot(npc);
            return true;
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = npc.rotation;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 2;
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "轻微氧化铜短剑";
            potionType = ItemID.SuperHealingPotion;
        }
        public override bool CheckActive()
        {
            if (target.dead)
            {
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.type == ModContent.ProjectileType<LostSword2>())
                    {
                        projectile.Kill();
                    }
                }
                return true; 
            }
            return false;
        }
        public override bool CheckDead()
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<LostSword2>())
                {
                    projectile.Kill();
                }
            }
            return true;
        }
        public override void NPCLoot()
        {
            NPC.NewNPC((int)target.Center.X + Main.rand.Next(500, 600),
                (int)target.Center.Y + Main.rand.Next(500, 600), ModContent.NPCType<ShortSword3>());
            foreach (Player player in Main.player)
            {
                int healLife = player.statLifeMax2 - player.statLife;
                player.statLife += healLife;
                player.HealEffect(healLife);
            }
        }
        private void ShootSword()
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<LostSword2>() && projectile.ai[0] != 2 && !projectile.friendly && projectile.hostile)
                {
                    projectile.ai[0] = 2;
                    projectile.ai[1] = 0;
                    projectile.timeLeft = 500;
                    if(projectile.damage <= 0)
                    {
                        projectile.damage = 120;
                    }
                }
            }
        }
        private void HealPlayerLife()
        {
            foreach (Player player in Main.player)
            {
                int healLife = player.statLifeMax2 - player.statLife;
                player.statLife += healLife;
                player.HealEffect(healLife);
            }
        }
    }
}
