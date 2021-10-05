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
    public class UltimateCopperPick : FSMnpc
    {
        public override string Texture => "Terraria/Item_" + ItemID.CopperPickaxe;
        public override string BossHeadTexture => "Terraria/Item_" + ItemID.CopperPickaxe;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimate Copper Pickaxe");
            DisplayName.AddTranslation(GameCulture.Chinese, "最终铜稿");
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
            Vector2 ToCenter = (new Vector2(target.Center.X - 500, target.Center.Y) - npc.Center).SafeNormalize(Vector2.UnitX);
            int LostSword2 = ModContent.ProjectileType<LostSword2>();
            if (target.dead)
            {
                npc.life = 0;
                npc.PlayerInteraction(1);
                return;
            }
            npc.velocity = (npc.velocity * 10 + ToCenter * 10) / 11;
            npc.rotation = Time1.DegToRad() * 15;
            Time1++;
            if (Time1 % 10 == Main.rand.Next(10) && Main.netMode != 1)
            {
                for (int i = 0; i < 5; i++)
                {
                    Projectile.NewProjectile(npc.Center, npc.rotation.ToRotationVector2() * (10+i),
                        LostSword2, 100, 2f, Main.myPlayer, 3, 0);
                }
            }
            if (Time1 > 1000) Time1 = 0;
        }
        public override bool CheckActive()
        {
            return false;
        }
        public override void NPCLoot()
        {
            foreach (Player player in Main.player)
            {
                int healLife = player.statLifeMax2 - player.statLife;
                player.statLife += healLife;
                player.HealEffect(healLife);
            }
        }
        public override void BossHeadRotation(ref float rotation)
        {
            rotation = npc.rotation;
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
