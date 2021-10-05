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

namespace UltimateCopperShortsword.NPCs.Bosses
{
    //第一阶段的最终同志短剑
    [AutoloadBossHead]
    public class ShortSword : FSMnpc
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Short Sword");
            DisplayName.AddTranslation(GameCulture.Chinese, "铜短剑");
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 35000;
            npc.defense = 5;
            npc.damage = 50;
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
            music = mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/Atk1");
            musicPriority = MusicPriority.BossHigh;
        }
        public override void AI()
        {
            if (npc.target <= 0 || npc.target == 255 || Main.player[npc.target].dead||!Main.player[npc.target].active)
            {
                npc.TargetClosest();
            }
            Vector2 ToTarget = (target.Center - npc.Center).SafeNormalize(Vector2.UnitX);
            npc.rotation = ToTarget.ToRotation() + MathHelper.PiOver4;//常态是一直对着玩家的
            int LostSword = ModContent.ProjectileType<LostSword>();
            int LostLaser = ModContent.ProjectileType<LostSwordLaser>();
            if (target.dead)
            {
                npc.life = 0;
                return;
            }
            switch (State1)
            {
                case 0://冲刺，向玩家方向释放弹幕
                    {
                        npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver4;
                        if(Time1<= 0)
                        {
                            Time1 = 60;
                            npc.velocity = ToTarget * 20;
                            Time2++;
                            npc.netUpdate = true;
                        }
                        else
                        {
                            Time1--;
                        }
                        if(Time1 % 5 == 1)
                        {
                            Projectile.NewProjectileDirect(npc.Center, ToTarget * 10, LostSword,
                                120, 2f, Main.myPlayer);
                        }
                        if(Time2 > 5)
                        {
                            ShootSword();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 1;
                        }
                        break;
                    }
                case 1://原地旋转释放弹幕
                    {
                        npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver4;
                        npc.velocity = (npc.velocity * 10 + 
                            npc.rotation.ToRotationVector2()* Main.rand.Next(5,8))/11;
                        Time1++;
                        if(Time1 >= 5 && Main.netMode != 1)
                        {
                             Projectile.NewProjectileDirect(npc.Center, npc.velocity * 2, LostSword,
                                140, 2f, Main.myPlayer);
                            Time1 = 0;
                            Time2++;
                        }
                        if(Time2 >= 50 && Time1 == 3)
                        {
                            ShootSword();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 2;
                        }
                        break;
                    }
                case 2://去到头顶，旋转发射弹幕
                    { 
                        switch(State2)
                        {
                            case 0:
                                {
                                    Vector2 center = new Vector2(target.Center.X, target.Center.Y - 500);
                                    float toHead = Vector2.Distance(npc.Center, center);
                                    npc.velocity = (npc.velocity * 5 + 
                                        (center - npc.Center).SafeNormalize(Vector2.UnitX) * 20)/6;
                                    if(toHead < 30)
                                    {
                                        Time1++;
                                    }
                                    if (Time1 > 30)
                                    {
                                        Time1 = 0;
                                        Time2 = 0;
                                        State2 = 1;
                                    }
                                    break;
                                }
                            case 1:
                                {
                                    Time1++;
                                    float r = Time1;
                                    npc.rotation += r.DegToRad();
                                    npc.velocity = npc.rotation.ToRotationVector2() * 5;
                                    if (Time1 % 5 == 1 && Main.netMode != 1)
                                    {
                                        var proj= Projectile.NewProjectileDirect(npc.Center
                                                , (npc.rotation - MathHelper.Pi / 4).ToRotationVector2() * 10,LostSword,130, 2.3f, Main.myPlayer, 0);
                                        proj.timeLeft = 1000;
                                        Time2++;
                                    }
                                    if(Time2 > 60)
                                    {
                                        ShootSword();
                                        Time1 = 0;
                                        Time2 = 0;
                                        State2 = 0;
                                        State1 = 3;
                                        npc.netUpdate = true;
                                    }
                                    break;
                                }
                        }
                        break;
                    }
                case 3://释放圈形弹幕
                    {
                        npc.velocity = ToTarget * 5;
                        Time1++;
                        if(Time1 >= 80 && Main.netMode != 1)
                        {
                            for(float r = 0;r<MathHelper.TwoPi;r += MathHelper.TwoPi/40)
                            {
                                Vector2 center = npc.Center + r.ToRotationVector2() * 500;
                                Vector2 To = (target.Center - center).SafeNormalize(Vector2.UnitX)*25 + target.velocity * 2;
                                Projectile.NewProjectileDirect(center, To, LostSword, 120, 2f, Main.myPlayer,0,1);
                            }
                            Time2++;
                            Time1 = 0;
                        }
                        if(Time2 >= 2 && Time1 > 30)
                        {
                            ShootSword();
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 4;
                        }
                        break;
                    }
                case 4://从天而降的弹幕,同时铜短剑冲刺
                    {
                        npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver4;
                        if (Time1 <= 0)
                        {
                            Time1 = 60;
                            npc.velocity = ToTarget * 18.5f;
                            npc.netUpdate = true;
                        }
                        else Time1--;
                        if(Time1 % 20 == 0)
                        {
                            for(int i = 0;i< 20;i++)
                            {
                                Vector2 center = new Vector2(target.Center.X + Main.rand.NextFloat(-500,500),
                                    target.Center.Y - 400);
                                Vector2 To = (target.Center - center).SafeNormalize(Vector2.UnitY) * 20 + target.velocity * 0.3f;
                                if (Main.netMode != 1)
                                {
                                    Projectile.NewProjectileDirect(center, To, LostSword, 150, 2f, Main.myPlayer, 0, 1);
                                }
                            }
                        }
                        else if(Time1 == 1)
                        {
                            ShootSword();
                            Time2++;
                        }
                        if(Time2 > 6)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 5;
                        }
                        break;
                    }
                case 5://单面激光
                    {
                        Time1++;
                        npc.velocity = (npc.velocity * 20 + ToTarget) / 21;
                        npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver4;
                        if (Main.netMode != 1)
                        {
                            if (Time1 < 60)
                            {
                                Projectile.NewProjectileDirect(npc.Center, (npc.rotation - MathHelper.PiOver4).ToRotationVector2(),
                                    LostLaser, 140, 2f, Main.myPlayer, 1, 0);
                            }
                            else
                            {
                                Projectile.NewProjectileDirect(npc.Center, (npc.rotation - MathHelper.PiOver4).ToRotationVector2(),
                                   LostLaser, 140, 2f, Main.myPlayer, 0, 0);
                            }
                        }
                        if(Time1 > 300)
                        {
                            Time1 = 0;
                            Time2 = 0;
                            State1 = 0;
                        }
                        break;
                    }
            }
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = npc.rotation;
        }
        public override void BossLoot(ref string name, ref int potionType)
        {
            name = "铜短剑";
            potionType = ItemID.SuperHealingPotion;
        }
        public override void ModifyHitByProjectile(Projectile projectile, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            damage /= 2;
        }
        public override bool CheckActive()
        {
            if (target.dead) return true;
            return false;
        }
        public override bool CheckDead()
        {
            foreach (Projectile projectile in Main.projectile)
            {
                if (projectile.type == ModContent.ProjectileType<LostSword>())
                {
                    projectile.Kill();
                }
            }
            return true;
        }
        public override void NPCLoot()
        {
            NPC.NewNPC((int)target.Center.X + Main.rand.Next(500,600),
                (int)target.Center.Y + Main.rand.Next(500,600),ModContent.NPCType<ShortSword2>());
            foreach (Player player in Main.player)
            {
                int healLife = player.statLifeMax2 - player.statLife;
                player.statLife += healLife;
                player.HealEffect(healLife);
            }
        }
        private void ShootSword()
        {
            foreach(Projectile projectile in Main.projectile)
            {
                if(projectile.type == ModContent.ProjectileType<LostSword>() && projectile.ai[0] != 2 && !projectile.friendly && projectile.hostile)
                {
                    projectile.ai[0] = 2;
                    projectile.ai[1] = 0;
                    projectile.timeLeft = 500;
                }
            }
        }
    }
}
