using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using UltimateCopperShortsword.NPCs.Bosses;
using UltimateCopperShortsword.Projs;

namespace UltimateCopperShortsword.Items
{
    public class LastShortSowrd : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜短剑");
            Tooltip.SetDefault("真正的力量");
        }
        public override void SetDefaults()
        {
            item.Size = new Vector2(40,40);
            item.UseSound = SoundID.Item1;
            item.damage = 130;
            item.melee = true;
            item.crit = 64;
            item.knockBack = 3.4f;
            item.value = 99999;
            item.useTurn = true;
            item.useTime = item.useAnimation = 10;
            item.rare = ItemRarityID.Red;
            item.mana = 0;
            item.useStyle = 3;
            item.autoReuse = true;
            item.shoot = 1;
        }
        public override bool AltFunctionUse(Player player)
        {
            return true;
        }
        public override bool CanUseItem(Player player)
        {
            ShortSwordPlayer shortSword = player.GetModPlayer<ShortSwordPlayer>();
            if (shortSword.PlayerEmotion > 40 && player.altFunctionUse == 2)
            {
                return true;
            }
            else if(shortSword.PlayerEmotion < 40 && player.altFunctionUse == 2)
            {
                return false;
            }
            return true;
        }
        public override bool UseItem(Player player)
        {
            return true;
        }
        public override bool Shoot(Player player, ref Vector2 position, ref float speedX, ref float speedY, ref int type, ref int damage, ref float knockBack)
        {
            if (player.altFunctionUse != 2)
            {
                var proj = Projectile.NewProjectileDirect(player.Center, (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX) * 21,
                    ModContent.ProjectileType<LostSword>(), item.damage, item.knockBack, player.whoAmI);
                proj.friendly = true;
                proj.hostile = false;
                proj.ai[0] = 3;
            }
            if (player.altFunctionUse == 2)
            {
                player.AddBuff(ModContent.BuffType<Buffs.CopperBuff>(), 60);
            }
            return false;
        }
        public override void UpdateInventory(Player player)
        {
            ShortSwordPlayer shortSword = player.GetModPlayer<ShortSwordPlayer>();
            item.useTime = item.useAnimation;
            shortSword.SwordSum = true;
            int LastSowrd = ModContent.ProjectileType<LastSowrd>();
            if (player.ownedProjectileCounts[LastSowrd] < 1 && player.HeldItem.type != ModContent.ItemType<LastShortSowrd>())
            {
                Projectile.NewProjectileDirect(player.Center, Vector2.Zero, LastSowrd,150,3f, player.whoAmI);
            }
            if(player.HeldItem.type == ModContent.ItemType<LastShortSowrd>())
            {
                shortSword.SwordSum = false;
            }
        }
    }
    public class LastSowrd : ModProjectile
    {
        private float State
        {
            get => projectile.ai[0];
            set => projectile.ai[0] = value;
        }
        private float Timer1
        {
            get => projectile.ai[1];
            set => projectile.ai[1] = value;
        }
        private float Timer2
        {
            get => projectile.localAI[1];
            set => projectile.localAI[1] = value;
        }
        public override string Texture => "UltimateCopperShortsword/Items/LastShortSowrd";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("最终铜短剑");
        }
        public override void SetDefaults()
        {
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.minion = true;
            projectile.melee = true;
            projectile.width = 40;
            projectile.height = 40;
            projectile.penetrate = -1;
        }
        public override void SendExtraAI(BinaryWriter writer)
        {
            if(Main.netMode == 2)
            {
                writer.Write(Timer2);
            }
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            if(Main.netMode != 1)
            {
                Timer2 = reader.ReadSingle();
            }
        }
        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            ShortSwordPlayer shortSword = player.GetModPlayer<ShortSwordPlayer>();
            projectile.rotation = projectile.velocity.ToRotation() + MathHelper.PiOver4;
            if (shortSword.SwordSum)
            {
                projectile.timeLeft = 2;
            }
            NPC npc = null;
            float max = 2000;
            foreach(NPC n in Main.npc)
            {
                float ToN = Vector2.Distance(n.Center, player.Center);
                if(ToN < max && !n.friendly && n.CanBeChasedBy())
                {
                    max = ToN;
                    npc = n;
                }    
            }
            if(npc != null)
            {
                float ToNPC = Vector2.Distance(npc.Center,projectile.Center);
                switch (State)
                {
                    case 0://冲刺
                        {
                            if (Timer1 <= 0)
                            {
                                Timer1 = 30;
                                Timer2++;
                                projectile.velocity = (npc.Center - projectile.Center).SafeNormalize(Vector2.UnitX) * 15;
                            }
                            else Timer1--;
                            if(Timer2 > 5)
                            {
                                Timer2 = 0;
                                Timer1 = 0;
                                State = 1;
                            }
                            break;
                        }
                    case 1:
                        {
                            if (ToNPC > 30)
                            {
                                projectile.velocity = (npc.Center - projectile.Center).SafeNormalize(Vector2.UnitX) * 5;
                            }
                            else
                            {
                                projectile.Center = player.Center + (Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 100);
                            }
                            if (Timer1 <= 0)
                            {
                                Timer1 = 30;
                                var proj = Projectile.NewProjectileDirect(projectile.Center, (npc.Center - projectile.Center).SafeNormalize(Vector2.UnitX) * 21 + npc.velocity * 0.6f,
                                        ModContent.ProjectileType<LostSword>(),120,2.3f, player.whoAmI);
                                proj.friendly = true;
                                proj.hostile = false;
                                proj.ai[0] = 3;
                                Timer2++;
                            }
                            else Timer1--;
                            if (Timer2 > 5)
                            {
                                Timer2 = 0;
                                Timer1 = 0;
                                State = 0;
                            }
                            break;
                        }
                }
            }
            else
            {
                projectile.velocity = (projectile.velocity * 99 + 
                    (player.Center - projectile.Center).SafeNormalize(Vector2.One) * 10) / 100;
            }
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            float s = 0;
            return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(),targetHitbox.Size(),
                projectile.Center + (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * 20,
                projectile.Center + (projectile.rotation - MathHelper.PiOver4).ToRotationVector2() * -20,
                6,ref s);
        }
        public override void ModifyHitNPC(NPC target, ref int damage, ref float knockback, ref bool crit, ref int hitDirection)
        {
            Player player = Main.player[projectile.owner];
            ShortSwordPlayer shortSword = player.GetModPlayer<ShortSwordPlayer>();
            damage += shortSword.PlayerEmotion;
        }
    }
}
