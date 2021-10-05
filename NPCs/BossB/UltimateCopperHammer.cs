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
using UltimateCopperShortsword.NPCs.Bosses;

namespace UltimateCopperShortsword.NPCs.BossB
{
    [AutoloadBossHead]
    public class UltimateCopperHammer : FSMnpc
    {
        public override string Texture => "Terraria/Item_" + ItemID.CopperHammer;
        public override string BossHeadTexture => "Terraria/Item_" + ItemID.CopperHammer;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimate Copper Hammer");
            DisplayName.AddTranslation(GameCulture.Chinese, "最终铜锤");
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 30000;
            npc.defense = 15;
            npc.damage = 95;
            npc.knockBackResist = 0;
            npc.noTileCollide = true;
            npc.noGravity = true;
            npc.width = 32;
            npc.height = 32;
            npc.friendly = false;
            npc.aiStyle = -1;
            npc.DeathSound = SoundID.NPCDeath11;
            npc.HitSound = SoundID.NPCHit4;
            npc.boss = true;
            music = mod.GetSoundSlot(Terraria.ModLoader.SoundType.Music, "Sounds/Music/Atk3");
            musicPriority = MusicPriority.BossLow;
        }
        public override void AI()
        {
            if (npc.target <= 0 || npc.target == 255 || Main.player[npc.target].dead || !Main.player[npc.target].active)
            {
                npc.TargetClosest();
            }
            Vector2 ToTarget = (target.Center - npc.Center).SafeNormalize(Vector2.UnitX);
            npc.rotation = npc.velocity.ToRotation() + MathHelper.PiOver4;
            int LostLaser2 = ModContent.ProjectileType<LostSwordLaser2>();
            if (target.dead)
            {
                npc.life = 0;
                npc.PlayerInteraction(1);
                return;
            }
            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<UltimateCopperBow>() && n.active)
                {
                    npc.velocity = (npc.velocity * 50 + ToTarget * 5) / 51;
                    npc.dontTakeDamage = true;
                    return;
                }
            }
            Time1++;
            npc.dontTakeDamage = false;
            if (Time1 > 1000) Time1 = 0;
            else
            {
                npc.velocity = (npc.velocity * 200 + ToTarget * 10) / 201;
                if (Main.netMode != 1)
                {
                    Projectile.NewProjectile(npc.Center,npc.rotation.ToRotationVector2(), LostLaser2, 80, 2f, Main.myPlayer);
                    Projectile.NewProjectile(npc.Center, (npc.rotation - MathHelper.PiOver2).ToRotationVector2(), LostLaser2, 80, 2f, Main.myPlayer);
                }
            }
        }
        public override void NPCLoot()
        {
            foreach (Player player in Main.player)
            {
                int healLife = player.statLifeMax2 - player.statLife;
                player.statLife += healLife;
                player.HealEffect(healLife);
            }
            foreach (NPC n in Main.npc)
            {
                if (n.type == ModContent.NPCType<ShortSword3>())
                {
                    n.ai[2] = 0;
                    n.ai[3] = 3;
                }
            }
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = npc.rotation;
        }
        public override bool CheckActive()
        {
            return false;
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