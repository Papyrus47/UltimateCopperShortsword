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
    public class UltimateCopperBow : FSMnpc
    {
        public override string Texture => "Terraria/Item_" + ItemID.CopperBow;
        public override string BossHeadTexture => "Terraria/Item_" + ItemID.CopperBow;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ultimate Copper Bow");
            DisplayName.AddTranslation(GameCulture.Chinese, "最终铜弓");
        }
        public override void SetDefaults()
        {
            npc.lifeMax = 30000;
            npc.defense = 2;
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
            npc.velocity = (npc.velocity * 10 + ToTarget * 5) / 11;
            npc.rotation = npc.velocity.ToRotation();
            int LostSword2 = ModContent.ProjectileType<LostSword2>();
            if (target.dead)
            {
                npc.life = 0;
                npc.PlayerInteraction(1);
                return;
            }
            foreach (NPC n in Main.npc)
            {
                if ((n.type == ModContent.NPCType<UltimateCopperAxe>() || n.type == ModContent.NPCType<UltimateCopperPick>()) && n.active)
                {
                    npc.dontTakeDamage = true;
                    return;
                }
            }
            Time1++;
            npc.dontTakeDamage = false;
            if (Time1 % 30 == 0 && Main.netMode != 1)
            {
                for(int i = (int)(-1 - Time2); i<=1 + Time2;i++)
                {
                    float r = npc.rotation + i * MathHelper.Pi / (10 + Time2);
                    Vector2 ToTar = r.ToRotationVector2() * 10;
                    var proj = Projectile.NewProjectileDirect(npc.Center,ToTar,
                    ModContent.ProjectileType<LostSword2>(), 100, 2f, Main.myPlayer,3,0);
                    proj.timeLeft = 1000;
                }
                Time2++;
            }
            if (Time2 > 5) Time2 = 0;
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
