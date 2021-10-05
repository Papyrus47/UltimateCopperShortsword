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
using UltimateCopperShortsword.NPCs.BossB;
using UltimateCopperShortsword.Items;

namespace UltimateCopperShortsword.NPCs.Bosses
{
    [AutoloadBossHead]
    public class ShortSword3 : FSMnpc
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimate Copper Shortsword");
            DisplayName.AddTranslation(GameCulture.Chinese, "最终铜短剑");
            NPCID.Sets.TrailCacheLength[npc.type] = 20;
            NPCID.Sets.TrailingMode[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 60000;
            npc.defense = 15;
            npc.damage = 95;
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
            music = mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/Atk3");
            musicPriority = MusicPriority.BossHigh;
        }
        public override void AI()
        {
            if (npc.target <= 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
            }
            if (Main.netMode != NetmodeID.Server && !Filters.Scene["ScreenTwo"].IsActive())
            {
                // 开启滤镜
                Filters.Scene.Activate("ScreenTwo");
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
            if(npc.life < npc.lifeMax * 0.5f && State2 == 0)
            {
                State2 = 1;
                int X = (int)npc.Center.X;
                int Y = (int)npc.Center.Y;
                NPC.NewNPC(X, Y, ModContent.NPCType<UltimateCopperAxe>());
                NPC.NewNPC(X, Y, ModContent.NPCType<UltimateCopperBow>());
                NPC.NewNPC(X, Y, ModContent.NPCType<UltimateCopperPick>());
                NPC.NewNPC(X, Y, ModContent.NPCType<UltimateCopperHammer>());
                npc.dontTakeDamage = true;
                foreach (Projectile projectile in Main.projectile)
                {
                    if (projectile.type == ModContent.ProjectileType<LostSword2>())
                    {
                        projectile.Kill();
                    }
                }
                State1 = 114;
            }
            if(State2 == 2)
            {
                npc.dontTakeDamage = false;
                State2++;
            }
            if(State2 == 3) npc.dontTakeDamage = false;
            switch (State1)
            {
                case 0://魔 弹 射 手
                    {
                        npc.velocity = ToTarget * 5;
                        Time1++;
                        if(Time1 >= 60)
                        {
                            Vector2 center;
                            Vector2 ves;
                            if (Main.netMode != 1)
                            {
                                if (target.velocity.X > 0)
                                {
                                    center = new Vector2(target.Center.X + 500, target.Center.Y);
                                    ves = (target.Center - center).SafeNormalize(Vector2.UnitX) * 20 + target.velocity * 0.9f;
                                    Dust.NewDustDirect(center, 5, 5, DustID.GreenTorch);
                                    Projectile.NewProjectileDirect(center, ves, LostSword2,
                                        180, 2f, Main.myPlayer,3,0);

                                }
                                else
                                {
                                    center = new Vector2(target.Center.X - 500, target.Center.Y);
                                    ves = (target.Center - center).SafeNormalize(Vector2.UnitX) * 20 + target.velocity * 0.9f;
                                    Dust.NewDustDirect(center, 5, 5, DustID.GreenTorch);
                                    Projectile.NewProjectileDirect(center, ves, LostSword2,
                                        180, 2f, Main.myPlayer, 3, 0);
                                }
                            }
                            Time1 = 0;
                            Time2++;
                        }
                        if(Time2 > 7)
                        {
                            npc.life -= 50;
                            npc.checkDead();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 1;
                        }
                        break;
                    }
                case 1://魔弹(
                    {
                        npc.velocity = ToTarget;
                        Time1++;
                        if (Time1 >= 10)
                        {
                            Vector2[] center = new Vector2[2];
                            Vector2 ves;
                            if (Main.netMode != 1)
                            {
                                if (target.velocity.X > 0)
                                {
                                    center[0] = new Vector2(target.Center.X + 1000, target.Center.Y) + target.velocity;
                                    center[1] = new Vector2(target.Center.X, target.Center.Y + 1000) + target.velocity;
                                    for (int i = 0; i < center.Length; i++)
                                    {
                                        ves = (target.Center - center[i]).SafeNormalize(Vector2.UnitX) * 20;
                                        Dust.NewDustDirect(center[i], 5, 5, DustID.GreenTorch);
                                        Projectile.NewProjectileDirect(center[i], ves, LostSword2,
                                            180, 2f, Main.myPlayer, 0, 1);
                                    }

                                }
                                else
                                {
                                    center[0] = new Vector2(target.Center.X - 1000, target.Center.Y) + target.velocity;
                                    center[1] = new Vector2(target.Center.X, target.Center.Y - 1000) + target.velocity;
                                    for (int i = 0; i < center.Length; i++)
                                    {
                                        ves = (target.Center - center[i]).SafeNormalize(Vector2.UnitX) * 20;
                                        Dust.NewDustDirect(center[i], 5, 5, DustID.GreenTorch);
                                        Projectile.NewProjectileDirect(center[i], ves, LostSword2,
                                            180, 2f, Main.myPlayer, 0, 1);
                                    }
                                }
                            }
                            Time1 = 0;
                            Time2++;
                        }
                        if(Time2 > 20)
                        {
                            ShootSword();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 2;
                        }
                        break;
                    }
                case 2://白夜的圈圈加强版（
                    {
                        npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver4;
                        if (Time1 <= 0)
                        {
                            npc.velocity = ToTarget * 15;
                            Time2++;
                            Time1 = 50;
                            npc.netUpdate = true;
                            if(Main.netMode != 1)
                            {
                                for(float r =0;r<MathHelper.TwoPi;r += MathHelper.TwoPi / 10)
                                {
                                    Vector2 ves = r.ToRotationVector2() * 10;
                                    var proj = Projectile.NewProjectileDirect(npc.Center, ves, LostSword2,
                                        120, 2.3f, Main.myPlayer,0,1);
                                    proj.timeLeft = 1000;
                                }
                            }
                        }
                        else Time1--;
                        if(Time2 >= 5 && Time1 < 40)
                        {
                            ShootSword();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 3;
                        }
                        break;
                    }
                case 3://白给乐团
                    {
                        npc.velocity = (npc.velocity * 10 + ToTarget * 8)/11;
                        if (Time1 <= 0)
                        {
                            Time2++;
                            Time1 = 50;
                            npc.netUpdate = true;
                            if (Main.netMode != 1)
                            {
                                for (float r = 0; r < MathHelper.TwoPi; r += MathHelper.TwoPi / (10 + (Time2 * 2)))
                                {
                                    Vector2 center = target.Center + r.ToRotationVector2() * 150 * Time2;
                                    Vector2 ves = (target.Center - center).SafeNormalize(Vector2.UnitX) * 10;
                                    var proj = Projectile.NewProjectileDirect(center, ves, LostSword2,
                                        120, 2.3f, Main.myPlayer, 0, 1);
                                    proj.timeLeft = 1000;
                                }
                            }
                        }
                        else Time1--;
                        if (Time2 >= 4 && Time1 < 40)
                        {
                            ShootSword();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 4;
                        }
                        break;
                    }
                case 4://真正的跨栏
                    {
                        Time1++;
                        npc.velocity = ToTarget;
                        if(Time1 >= 60)
                        {
                            HealPlayerLife();
                            for (int j = 0; j < 10; j++)
                            {
                                Vector2[] centers = new Vector2[4]
{
                                new Vector2(target.Center.X + 1500, target.Center.Y + Main.rand.Next(-1500, 1500)),
                                new Vector2(target.Center.X - 1500, target.Center.Y + Main.rand.Next(-1500, 1500)),
                                new Vector2(target.Center.X + Main.rand.Next(-1500, 1500), target.Center.Y + 1500),
                                new Vector2(target.Center.X + Main.rand.Next(-1500, 1500), target.Center.Y - 1500)
                                };
                                for (int i = 0; i < centers.Length; i++)
                                {
                                    var proj = Projectile.NewProjectileDirect(centers[i],Vector2.Normalize(target.Center - centers[i]) * 10, LostSword2,
                                                120, 2.3f, Main.myPlayer, 0, 1);
                                    proj.timeLeft = 1000;
                                }
                                Time2++;
                            }
                            Time1 = 0;
                        }
                        if(Time2 >= 1 && Time1 >= 50)
                        {
                            ShootSword();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 5;
                        }
                        break;
                    }
                case 5://死亡激光
                    {
                        Time1++;
                        npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver4;
                        npc.velocity = (npc.velocity * 300 + ToTarget * 25) / 301;
                        float npcR = npc.rotation;
                        Vector2[] center = new Vector2[4] {
                            npc.Center + (npcR.ToRotationVector2() * 60),
                            npc.Center + ((npcR + MathHelper.PiOver2).ToRotationVector2() * 60),
                            npc.Center + ((npcR - MathHelper.PiOver2).ToRotationVector2() * 60),
                            npc.Center + ((npcR - MathHelper.Pi).ToRotationVector2() * 60)
                        };
                        Vector2[] vse = new Vector2[4] {
                            (npcR + MathHelper.PiOver2).ToRotationVector2(),
                            (npcR + MathHelper.Pi).ToRotationVector2(),
                            (npcR).ToRotationVector2(),
                            (npcR - MathHelper.PiOver2).ToRotationVector2()
                        };
                        if(Time1 < 60)
                        {
                            for (int i = 0; i < center.Length; i++)
                            {
                                Projectile.NewProjectileDirect(center[i],vse[i], LostLaser2,
                                    90, 2.3f, Main.myPlayer, 1,0);
                            }
                        }
                        else
                        {
                            for (int i = 0; i < center.Length; i++)
                            {
                                Projectile.NewProjectileDirect(center[i], vse[i], LostLaser2,
                                    90, 2.3f, Main.myPlayer, 0, 0);
                            }
                        }
                        if(Time1 > 500)
                        {
                            Time2 = 0;
                            Time1 = 0;
                            State1 = 6;
                        }
                        break;
                    }
                case 6://绕圈放弹幕
                    {
                        npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver4;
                        npc.velocity = (npc.velocity * 30 +
                            npc.rotation.ToRotationVector2() * Main.rand.Next(10, 12)) / 31;
                        Time1++;
                        if (Time1 >= 5 && Main.netMode != 1)
                        {
                            var proj = Projectile.NewProjectileDirect(npc.Center, npc.velocity * 5, LostSword2,
                               140, 2f, Main.myPlayer,0,1);
                            proj.timeLeft = 1000;
                            Time1 = 0;
                            Time2++;
                        }
                        if (Time2 >= 100 && Time1 == 3)
                        {
                            ShootSword();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 7;
                        }
                        break;
                    }
                case 7://同上，只不过不会变成绕回一个圆
                    {
                        Time1++;
                        npc.rotation = npc.velocity.ToRotation();
                        npc.velocity = (npc.rotation - 0.1f).ToRotationVector2() * 5;
                        if (Time1 % 5 == 1 && Main.netMode != 1)
                        {
                            Projectile.NewProjectileDirect(npc.Center,(npc.rotation - MathHelper.Pi / 4)
                                .ToRotationVector2() * 28,LostSword2,120, 2.3f, Main.myPlayer, 0,1).timeLeft = 1000;
                            Time2++;
                        }
                        if (Time2 >= 90 && Main.netMode != 1 && Time1 % 5 == 2)
                        {
                            ShootSword();
                            Time2 = 0;
                            Time1 = 0;
                            if(State2 == 3) State1 = 8;
                            else State1 = 0;
                        }
                        break;
                    }
                case 8://海 豚 机 枪
                    {
                        switch (Time2)
                        {
                            case 0:
                                {
                                    Vector2 Center = target.Center + new Vector2(0, -800);
                                    npc.velocity = (npc.velocity * 8 + Vector2.Normalize(Center - npc.Center) * 20) / 9;
                                    float ToC = Vector2.Distance(Center, npc.Center);
                                    if (ToC < 30)
                                    {
                                        Time1++;
                                        if (Time1 > 10)
                                        {
                                            Time2++;
                                        }
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    Time1++;
                                    npc.velocity.X = 20;
                                    npc.velocity.Y = 20;
                                    npc.rotation = new Vector2(-1, 0).ToRotation();
                                    if (Time1 % 8 == 0)
                                    {
                                        for (int i = -1; i <= 1; i++)
                                        {
                                            float r = (npc.rotation - MathHelper.PiOver4) + (i * MathHelper.Pi / 72);
                                            var proj = Projectile.NewProjectileDirect(npc.Center, r.ToRotationVector2() * 20,
                                                LostSword2, 2, 2.3f, Main.myPlayer, 0, 1);
                                            proj.timeLeft = 5000;
                                        }
                                    }
                                    if (Time1 > 80)
                                    {
                                        Time1 = 0;
                                        Time2++;
                                    }
                                    break;
                                }
                            case 2:
                                {
                                    Time1++;
                                    npc.velocity.X = -20;
                                    npc.velocity.Y = 20;
                                    npc.rotation = new Vector2(0, -1).ToRotation();
                                    if (Time1 % 8 == 0)
                                    {
                                        for (int i = -1; i <= 1; i++)
                                        {
                                            float r = (npc.rotation - MathHelper.PiOver4) + (i * MathHelper.Pi / 72);
                                            var proj = Projectile.NewProjectileDirect(npc.Center, r.ToRotationVector2() * 20,
                                                LostSword2, 2, 2.3f, Main.myPlayer, 0, 1);
                                            proj.timeLeft = 5000;
                                        }
                                    }
                                    if (Time1 > 80)
                                    {
                                        Time1 = 0;
                                        Time2++;
                                    }
                                    break;
                                }
                            case 3:
                                {
                                    Time1++;
                                    npc.velocity.X = -20;
                                    npc.velocity.Y = -20;
                                    npc.rotation = new Vector2(1, 0).ToRotation();
                                    if (Time1 % 8 == 0)
                                    {
                                        for (int i = -1; i <= 1; i++)
                                        {
                                            float r = (npc.rotation - MathHelper.PiOver4) + (i * MathHelper.Pi / 72);
                                            var proj = Projectile.NewProjectileDirect(npc.Center, r.ToRotationVector2() * 20,
                                                LostSword2, 2, 2.3f, Main.myPlayer, 0, 1);
                                            proj.timeLeft = 5000;
                                        }
                                    }
                                    if (Time1 > 80)
                                    {
                                        Time1 = 0;
                                        Time2++;
                                    }
                                    break;
                                }
                            case 4:
                                {
                                    Time1++;
                                    npc.velocity.X = 20;
                                    npc.velocity.Y = -20;
                                    npc.rotation = new Vector2(0,1).ToRotation();
                                    if (Time1 % 8 == 0)
                                    {
                                        for (int i = -1; i <= 1; i++)
                                        {
                                            float r = (npc.rotation - MathHelper.PiOver4) + (i * MathHelper.Pi / 72);
                                            var proj = Projectile.NewProjectileDirect(npc.Center, r.ToRotationVector2() * 20,
                                                LostSword2, 2, 2.3f, Main.myPlayer, 0, 1);
                                            proj.timeLeft = 5000;
                                        }
                                    }
                                    if (Time1 > 80)
                                    {
                                        Time1 = 0;
                                        Time2++;
                                    }
                                    break;
                                }
                            default:
                                {
                                    ShootSword();
                                    State1 = 9;
                                    Time1 = 0;
                                    Time2 = 0;
                                    break;
                                }
                        }
                        break;
                    }
                case 9:
                    {
                        State1 = 0;
                        break;
                    }
                case 114:
                    {
                        Vector2 center = new Vector2(target.Center.X, target.Center.Y - 500);
                        npc.velocity = (npc.velocity * 5 +
                                        (center - npc.Center).SafeNormalize(Vector2.UnitX) * 20) / 6;
                        break;
                    }
            }
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Color drawColor)
        {
            Color color = Color.White;
            if (State1 == 2 || State1 == 5) color = Color.Green;
            TheWayOfSword.NpcDrawTail(npc, drawColor, color);
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
            if (State1 == 7) damage = 1;
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "最终铜短剑";
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
            Item.NewItem(npc.Hitbox,ModContent.ItemType<LastShortSowrd>());
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
                    if (projectile.damage <= 0)
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
