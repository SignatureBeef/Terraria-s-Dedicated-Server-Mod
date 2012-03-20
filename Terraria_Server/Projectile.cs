using System;
using System.Collections.Generic;

using Terraria_Server.Commands;
using Terraria_Server.Plugins;
using Terraria_Server.Misc;
using Terraria_Server.Definitions;
using Terraria_Server.Collections;
using Terraria_Server.WorldMod;
using Terraria_Server.Logging;

namespace Terraria_Server
{
	/// <summary>
	/// Projectile includes things like bullets, arrows, knives, explosives, boomerangs, and possibly ball/chain, orbs, and flamelash/spells.
	/// </summary>
	public class Projectile : BaseEntity, ISender
	{
		public const Int32 MAX_PROJECTILES = 111;

		[System.Xml.Serialization.XmlIgnore]
		public ISender Creator { get; set; }

		bool ISender.Op
		{
			get
			{
				var creator = Creator;
				if (creator == null)
					return true;
				else
					return creator.Op;
			}

			set { }
		}

		void ISender.sendMessage(string a, int b, float c, float d, float e)
		{
			var creator = Creator;
			if (creator != null)
				creator.sendMessage(a, b, c, d, e);
		}

		/// <summary>
		/// Whether the projectile is currently wet
		/// </summary>
		public bool wet;
		/// <summary>
		/// 
		/// </summary>
		public byte wetCount;
		/// <summary>
		/// Whether the projectile is currently immersed in lava
		/// </summary>
		public bool lavaWet;
		/// <summary>
		/// Projectile index
		/// </summary>
		public int whoAmI;
		/// <summary>
		/// 
		/// </summary>
		public const int MAX_AI = 2;

		/// <summary>
		/// Projectile's current 2-direction speed
		/// </summary>
		public Vector2 Velocity;

		// summary>
		// Scaled size of projectile
		// /summary>
		//public float scale;
		/// <summary>
		/// Degrees of rotation for projectile sprite
		/// </summary>
		public float rotation;
		/// <summary>
		/// Projectile type
		/// </summary>
		public ProjectileType type { get; set; }

		/// <summary>
		/// Integer representation of projectile type
		/// </summary>
		public override int Type
		{
			get
			{
				return (int)type;
			}
			set
			{
				type = (ProjectileType)value;
			}
		}

		// [TODO] 1.1 -- are these really neccesary ?
		public Vector2[] oldPos;
		public Vector2 lastPosition;

		/// <summary>
		/// Projectile's visibility, 255 == fully visible, 0 == invisible
		/// </summary>
		public int alpha;
		/// <summary>
		/// Index of owning player
		/// </summary>
		public int Owner = 255;
		/// <summary>
		/// 
		/// </summary>
		public float[] ai = new float[Projectile.MAX_AI];
		// summary>
		// Value of artificial intelligence style to use for motion
		// /summary>
		//public int aiStyle;
		/// <summary>
		/// Amount of time left in the projectile's life cycle?
		/// </summary>
		public int timeLeft;
		/// <summary>
		/// Amount of time to delay sound.  Unit unknown (likely milliseconds)
		/// </summary>
		public int soundDelay;
		// summary>
		// Amount of damage the projectile can cause on a target entity
		// /summary>
		//public int damage;
		/// <summary>
		/// Integer representing x direction of travel.  Values unknown
		/// </summary>
		public int direction;
		/// <summary>
		///  
		/// </summary>
		public bool hostile;
		/// <summary>
		/// Amount of knockback the projectile causes
		/// </summary>
		public float knockBack;
		/// <summary>
		/// 
		/// </summary>
		public bool friendly;
		/// <summary>
		/// How many entities the projectile can penetrate
		/// </summary>
		public int penetrate = 1;
		/// <summary>
		/// Projectile's index number in Main.projectile[]
		/// </summary>
		public int identity;
		/// <summary>
		/// Amount of light the projectile gives off
		/// </summary>
		public float light;
		/// <summary>
		/// 
		/// </summary>
		public bool netUpdate;
		public bool netUpdate2;
		public int netSpam;
		/// <summary>
		/// Delay before striking an entity the projectile has already struck
		/// </summary>
		public int restrikeDelay;
		/// <summary>
		/// Whether the projectile has struck a solid tile
		/// </summary>
		public bool tileCollide;
		/// <summary>
		/// 
		/// </summary>
		public int maxUpdates;
		/// <summary>
		/// 
		/// </summary>
		public int numUpdates;
		/// <summary>
		/// Whether or not to ignore water conditional effects
		/// </summary>
		public bool ignoreWater;
		/// <summary>
		/// 
		/// </summary>
		public bool hide;
		/// <summary>
		/// 
		/// </summary>
		public bool ownerHitCheck;
		/// <summary>
		/// Any immune players
		/// </summary>
		public byte[] playerImmune = new byte[255];
		/// <summary>
		/// Miscellaneous text associated with the projectile.  Only used in hardcore deaths and sign edits?
		/// </summary>
		public string miscText = "";

		public bool melee { get; set; }
		public bool ranged { get; set; }
		public bool magic { get; set; }

		public Projectile()
		{
			for (int i = 0; i < Projectile.MAX_AI; i++)
			{
				this.ai[i] = 0f;
			}
			for (int j = 0; j < 255; j++)
			{
				this.playerImmune[j] = 0;
			}
			penetrate = 1;
			tileCollide = true;
			Position = default(Vector2);
			Velocity = default(Vector2);
			scale = 1f;
			Owner = 255;
			timeLeft = 3600;
			Name = "";
			miscText = "";
			Width = (int)((float)this.Width * this.scale);
			Height = (int)((float)this.Height * this.scale);

			this.oldPos = new Vector2[10];
			for (int k = 0; k < this.oldPos.Length; k++)
			{
				this.oldPos[k] = new Vector2(0f, 0f);
			}
		}

		/// <summary>
		/// Creates a copy of the projectile's instance
		/// </summary>
		/// <returns>Copy of the projectile instance</returns>
		public override object Clone()
		{
			var cloned = (Projectile)base.MemberwiseClone();
			cloned.ai = new float[Projectile.MAX_AI];
			Array.Copy(ai, cloned.ai, Projectile.MAX_AI);
			cloned.playerImmune = new byte[playerImmune.Length];
			Array.Copy(playerImmune, cloned.playerImmune, playerImmune.Length);
			return cloned;
		}

		public static int NewProjectile(float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner = 255)
		{
			return NewProjectile(X, Y, SpeedX, SpeedY, (ProjectileType)Type, Damage, KnockBack, Owner);
		}

		/// <summary>
		/// Creates a new projectile instance with the specified parameters
		/// </summary>
		/// <param name="X">Starting X coordinate</param>
		/// <param name="Y">Starting Y coordinate</param>
		/// <param name="SpeedX">Starting horizontal speed</param>
		/// <param name="SpeedY">Starting vertical speed</param>
		/// <param name="Type">Type of projectile to create</param>
		/// <param name="Damage">Amount of damage the projectile takes before self-destructing? (unknown)</param>
		/// <param name="KnockBack">Whether the projectile creates knockback</param>
		/// <param name="Owner">Index of owning player</param>
		/// <returns>New projectile's index</returns>
		public static int NewProjectile(float X, float Y, float SpeedX, float SpeedY, ProjectileType Type, int Damage, float KnockBack, int Owner = 255)
		{
			int num = ReserveSlot(Owner);
			if (num == 1000)
			{
				return num;
			}
			var projectile = Registries.Projectile.Create((int)Type);
			if (Owner < 255)
				projectile.Creator = Main.players[Owner];
			projectile.Position.X = X - (float)projectile.Width * 0.5f;
			projectile.Position.Y = Y - (float)projectile.Height * 0.5f;
			projectile.Owner = Owner;
			projectile.Velocity.X = SpeedX;
			projectile.Velocity.Y = SpeedY;
			projectile.damage = Damage;
			projectile.knockBack = KnockBack;
			projectile.identity = num;
			projectile.whoAmI = num;
			projectile.wet = Collision.WetCollision(projectile.Position, projectile.Width, projectile.Height);
			Main.projectile[num] = projectile;
			if (Owner == Main.myPlayer)
			{
				NetMessage.SendData(27, -1, -1, "", num);
			}
			//            if (Owner == Main.myPlayer)
			//            {
			//                if (Type == ProjectileType.BOMB)
			//                {
			//                    Main.projectile[num].timeLeft = 180;
			//                }
			//                if (Type == ProjectileType.DYNAMITE)
			//                {
			//                    Main.projectile[num].timeLeft = 300;
			//                }
			//                if (Type == ProjectileType.GRENADE)
			//                {
			//                    Main.projectile[num].timeLeft = 180;
			//                }
			//                if (Type == ProjectileType.BOMB_STICKY)
			//                {
			//                    Main.projectile[num].timeLeft = 180;
			//                }
			//            }
			return num;
		}

		public void StatusNPC(NPC npc)
		{
			switch (this.type)
			{
				case ProjectileType.N2_FIRE_ARROW:
					if (Main.rand.Next(3) == 0)
						npc.AddBuff(24, 180, false);
					break;

				case ProjectileType.N15_BALL_OF_FIRE:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(24, 300, false);
					break;

				case ProjectileType.N19_FLAMARANG:
					if (Main.rand.Next(5) == 0)
						npc.AddBuff(24, 180, false);
					break;

				case ProjectileType.N34_FLAMELASH:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(24, 240, false);
					break;

				case ProjectileType.N35_SUNFURY:
					if (Main.rand.Next(4) == 0)
						npc.AddBuff(24, 180, false);
					break;

				case ProjectileType.N33_THORN_CHAKRUM:
					if (Main.rand.Next(5) == 0)
						npc.AddBuff(20, 420, false);
					break;

				case ProjectileType.N54_POISONED_KNIFE:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(20, 600, false);
					break;

				case ProjectileType.N63_THE_DAO_OF_POW:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(31, 120, false);
					break;

				case ProjectileType.N85_FLAMES:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(24, 1200, false);
					break;

				case ProjectileType.N95_CURSED_FLAME:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(39, 420, false);
					break;

				case ProjectileType.N103_CURSED_ARROW:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(39, 420, false);
					break;

				case ProjectileType.N104_CURSED_BULLET:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(39, 420, false);
					break;

				case ProjectileType.N98_POISON_DART:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(20, 600, false);
					break;
			}
		}

		public void StatusPvP(Player player)
		{
			switch (this.type)
			{
				case ProjectileType.N2_FIRE_ARROW:
					if (Main.rand.Next(3) == 0)
						player.AddBuff(24, 180, false);
					break;

				case ProjectileType.N15_BALL_OF_FIRE:
					if (Main.rand.Next(2) == 0)
						player.AddBuff(24, 300, false);
					break;

				case ProjectileType.N19_FLAMARANG:
					if (Main.rand.Next(5) == 0)
						player.AddBuff(24, 180, false);
					break;

				case ProjectileType.N34_FLAMELASH:
					if (Main.rand.Next(2) == 0)
						player.AddBuff(24, 240, false);
					break;

				case ProjectileType.N35_SUNFURY:
					if (Main.rand.Next(4) == 0)
						player.AddBuff(24, 180, false);
					break;

				case ProjectileType.N33_THORN_CHAKRUM:
					if (Main.rand.Next(5) == 0)
						player.AddBuff(20, 420, false);
					break;

				case ProjectileType.N54_POISONED_KNIFE:
					if (Main.rand.Next(2) == 0)
						player.AddBuff(20, 600, false);
					break;
				case ProjectileType.N63_THE_DAO_OF_POW:
					if (Main.rand.Next(3) != 0)
						player.AddBuff(31, 120, true);
					break;
				case ProjectileType.N85_FLAMES:
					player.AddBuff(24, 1200, false);
					break;
				case (ProjectileType.N95_CURSED_FLAME):
					player.AddBuff(39, 420, true);
					break;
				case (ProjectileType.N103_CURSED_ARROW):
					player.AddBuff(39, 420, true);
					break;
				case (ProjectileType.N104_CURSED_BULLET):
					player.AddBuff(39, 420, true);
					break;
			}
		}

		public void StatusPlayer(Player player)
		{
			switch (this.type)
			{
				case ProjectileType.N55_STINGER:
					if (Main.rand.Next(3) == 0)
						player.AddBuff(20, 600, true);
					break;

				case ProjectileType.N44_DEMON_SICKLE:
					if (Main.rand.Next(3) == 0)
						player.AddBuff(22, 900, true);
					break;
				case ProjectileType.N82_FLAMING_ARROW:
					if (Main.rand.Next(3) == 0)
						player.AddBuff(24, 420, true);
					break;
				case ProjectileType.N96_CURSED_FLAME:
					if (Main.rand.Next(3) == 0)
						player.AddBuff(39, 480, true);
					break;
				case ProjectileType.N101_EYE_FIRE:
					if (Main.rand.Next(3) == 0)
						player.AddBuff(39, 480, true);
					break;
				case ProjectileType.N98_POISON_DART:
					player.AddBuff(20, 600, true);
					break;
			}
		}

		/// <summary>
		/// Runs damage calculation on hostile mobs and players
		/// </summary>
		public void Damage(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (this.type == ProjectileType.N18_ORB_OF_LIGHT || this.type == ProjectileType.N72_BLUE_FAIRY ||
				this.type == ProjectileType.N86_PINK_FAIRY || this.type == ProjectileType.N87_PINK_FAIRY ||
				this.type == ProjectileType.N111_BUNNY)
			{
				return;
			}
			int playerIndex = Main.myPlayer;
			Player player = Main.players[playerIndex];

			Rectangle rectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);
			if (this.type == ProjectileType.N85_FLAMES || this.type == ProjectileType.N101_EYE_FIRE)
			{
				int num = 30;
				rectangle.X -= num;
				rectangle.Y -= num;
				rectangle.Width += num * 2;
				rectangle.Height += num * 2;
			}
			if (this.friendly && this.type != ProjectileType.N18_ORB_OF_LIGHT)
			{
				var creat = Creator as Player;
				if (this.Owner == playerIndex)// || (Owner == 255 && creat != null && creat.whoAmi == playerIndex))
				{
					if (creat != null) player = creat;
					if ((this.aiStyle == 16 || this.type == ProjectileType.N41_HELLFIRE_ARROW) && (this.timeLeft <= 1 || this.type == ProjectileType.N108_EXPLOSIVES))
					{
						if (player.Active && !player.dead && !player.immune && (!this.ownerHitCheck || Collision.CanHit(Main.players[this.Owner].Position, Main.players[this.Owner].Width, Main.players[this.Owner].Height, player.Position, player.Width, player.Height)))
						{
							if (player.Intersects(rectangle))
							{
								if (player.Position.X + (float)(player.Width / 2) < this.Position.X + (float)(this.Width / 2))
								{
									this.direction = -1;
								}
								else
								{
									this.direction = 1;
								}
								int dmg = Main.DamageVar((float)this.damage);
								this.StatusPlayer(player);

								player.Hurt(this, dmg, this.direction, true, false, Player.getDeathMessage(this.Owner, -1, this.whoAmI, -1));
								NetMessage.SendData(26, -1, -1, Player.getDeathMessage(this.Owner, -1, this.whoAmI, -1), playerIndex, (float)this.direction, (float)dmg, 1f);
							}
						}
					}
					if (this.type != ProjectileType.N69_HOLY_WATER && this.type != ProjectileType.N70_UNHOLY_WATER &&
						this.type != ProjectileType.N10_PURIFICATION_POWDER && this.type != ProjectileType.N11_VILE_POWDER)
					{
						int num = (int)(this.Position.X / 16f);
						int num2 = (int)((this.Position.X + (float)this.Width) / 16f) + 1;
						int num3 = (int)(this.Position.Y / 16f);
						int num4 = (int)((this.Position.Y + (float)this.Height) / 16f) + 1;
						if (num < 0)
						{
							num = 0;
						}
						if (num2 > Main.maxTilesX)
						{
							num2 = Main.maxTilesX;
						}
						if (num3 < 0)
						{
							num3 = 0;
						}
						if (num4 > Main.maxTilesY)
						{
							num4 = Main.maxTilesY;
						}
						for (int i = num; i < num2; i++)
						{
							for (int j = num3; j < num4; j++)
							{
								if (TileRefs(i, j + 1).Exists && Main.tileCut[(int)TileRefs(i, j).Type] && TileRefs(i, j + 1).Type != 78)
								{
									var plr = Creator as Player;
									if (plr == null || WorldModify.InvokeAlterationHook(this, plr, i, j, 0))
									{
										WorldModify.KillTile(TileRefs, sandbox, i, j);
										NetMessage.SendData(17, -1, -1, "", 0, (float)i, (float)j);
									}
								}
							}
						}
					}
					if (this.damage > 0)
					{
						NPC npc;
						for (int i = 0; i < NPC.MAX_NPCS; i++)
						{
							npc = Main.npcs[i];
							if (npc.Active && !npc.dontTakeDamage && (!npc.friendly || (npc.type == NPCType.N22_GUIDE && this.Owner < 255 && Main.players[this.Owner].killGuide)) && (this.Owner < 0 || npc.immune[this.Owner] == 0))
							{
								bool flag = false;
								if (this.type == ProjectileType.N11_VILE_POWDER &&
									(npc.type == NPCType.N47_CORRUPT_BUNNY ||
									npc.type == NPCType.N57_CORRUPT_GOLDFISH))
								{
									flag = true;
								}
								else
								{
									if (this.type == ProjectileType.N31_SAND_BALL &&
										npc.type == NPCType.N69_ANTLION)
									{
										flag = true;
									}
								}
								if (!flag && (npc.noTileCollide || !this.ownerHitCheck || Collision.CanHit(Main.players[this.Owner].Position, Main.players[this.Owner].Width, Main.players[this.Owner].Height, npc.Position, npc.Width, npc.Height)))
								{
									if (npc.Intersects(rectangle))
									{
										if (this.aiStyle == 3)
										{
											if (this.ai[0] == 0f)
											{
												this.Velocity.X = -this.Velocity.X;
												this.Velocity.Y = -this.Velocity.Y;
												this.netUpdate = true;
											}
											this.ai[0] = 1f;
										}
										else
										{
											if (this.aiStyle == 16)
											{
												if (this.timeLeft > 3)
												{
													this.timeLeft = 3;
												}
												if (npc.Position.X + (float)(npc.Width / 2) < this.Position.X + (float)(this.Width / 2))
												{
													this.direction = -1;
												}
												else
												{
													this.direction = 1;
												}
											}
										}
										if (this.type == ProjectileType.N41_HELLFIRE_ARROW && this.timeLeft > 1)
										{
											this.timeLeft = 1;
										}
										bool crit = false;
										if (this.Owner < 255)
										{
											var owner = Main.players[this.Owner];
											var rand = Main.rand.Next(1, 101);
											crit = (this.melee && rand <= owner.meleeCrit)
												|| (this.ranged && rand <= owner.rangedCrit)
												|| (this.magic && rand <= owner.magicCrit);
										}
										int dmg = Main.DamageVar(this.damage);

										if (npc.StrikeNPC(this, dmg, this.knockBack, this.direction, crit))
										{
											this.StatusNPC(npc);
											NetMessage.SendData(28, -1, -1, "", i, (float)dmg, this.knockBack, (float)this.direction, crit ? 1 : 0);
										}

										if (this.penetrate != 1)
										{
											npc.immune[this.Owner] = 10;
										}
										if (this.penetrate > 0)
										{
											this.penetrate--;
											if (this.penetrate == 0)
											{
												break;
											}
										}
										if (this.aiStyle == 7)
										{
											this.ai[0] = 1f;
											this.damage = 0;
											this.netUpdate = true;
										}
										else
										{
											if (this.aiStyle == 13)
											{
												this.ai[0] = 1f;
												this.netUpdate = true;
											}
										}
									}
								}
							}
						}
					}
					if (this.damage > 0 && player.hostile)
					{
						Player playerIt;
						for (int i = 0; i < Main.MAX_PLAYERS; i++)
						{
							playerIt = Main.players[i];
							if (i != this.Owner && playerIt.Active && !playerIt.dead && !playerIt.immune && playerIt.hostile && this.playerImmune[i] <= 0 && (player.team == 0 || player.team != playerIt.team) && (!this.ownerHitCheck || Collision.CanHit(Main.players[this.Owner].Position, Main.players[this.Owner].Width, Main.players[this.Owner].Height, playerIt.Position, playerIt.Width, playerIt.Height)))
							{
								if (playerIt.Intersects(rectangle))
								{
									if (this.aiStyle == 3)
									{
										if (this.ai[0] == 0f)
										{
											this.Velocity.X = -this.Velocity.X;
											this.Velocity.Y = -this.Velocity.Y;
											this.netUpdate = true;
										}
										this.ai[0] = 1f;
									}
									else
									{
										if (this.aiStyle == 16)
										{
											if (this.timeLeft > 3)
											{
												this.timeLeft = 3;
											}
											if (playerIt.Position.X + (float)(playerIt.Width / 2) < this.Position.X + (float)(this.Width / 2))
											{
												this.direction = -1;
											}
											else
											{
												this.direction = 1;
											}
										}
									}
									if (this.type == ProjectileType.N41_HELLFIRE_ARROW && this.timeLeft > 1)
									{
										this.timeLeft = 1;
									}
									bool crit = false;
									if (this.Owner < 255)
									{
										var owner = Main.players[this.Owner];
										var rand = Main.rand.Next(1, 101);
										crit = (this.melee && rand <= owner.meleeCrit);
									}
									int dmg = Main.DamageVar(this.damage);

									if (!playerIt.immune) this.StatusPvP(playerIt);

									var crea = Creator as Player;
									playerIt.Hurt(this, dmg, this.direction, true, false, Player.getDeathMessage(this.Owner < 255 ? this.Owner : (crea == null ? 255 : crea.whoAmi), -1, this.whoAmI, -1), crit);

									//NetMessage.SendData(26, -1, -1, Player.getDeathMessage(this.Owner, -1, this.whoAmI, -1), i, (float)this.direction, (float)dmg, 1f, crit ? 1 : 0);

									this.playerImmune[i] = 40;
									if (this.penetrate > 0)
									{
										this.penetrate--;
										if (this.penetrate == 0)
										{
											break;
										}
									}
									if (this.aiStyle == 7)
									{
										this.ai[0] = 1f;
										this.damage = 0;
										this.netUpdate = true;
									}
									else
									{
										if (this.aiStyle == 13)
										{
											this.ai[0] = 1f;
											this.netUpdate = true;
										}
									}
								}
							}
						}
					}
				}
				if (this.type == ProjectileType.N11_VILE_POWDER)
				{
					NPC npc;
					for (int i = 0; i < NPC.MAX_NPCS; i++)
					{
						npc = Main.npcs[i];
						if (npc.Active)
						{
							if (npc.type == NPCType.N46_BUNNY)
							{
								if (npc.Intersects(rectangle))
								{
									npc.Transform(47);
								}
							}
							else
							{
								if (npc.type == NPCType.N55_GOLDFISH)
								{
									if (npc.Intersects(rectangle))
									{
										npc.Transform(57);
									}
								}
							}
						}
					}
				}
			}
#if CLIENT_CODE
            if (this.hostile && Main.myPlayer < 255 && this.damage > 0) // client-only, but we may use it later
            {
                if (player.Active && !player.dead && !player.immune)
                {
                    if (player.Intersects(rectangle))
                    {
                        int hitDirection = this.direction;
                        if (player.Position.X + (float)(player.Width / 2) < this.Position.X + (float)(this.Width / 2))
                        {
                            hitDirection = -1;
                        }
                        else
                        {
                            hitDirection = 1;
                        }
                        
                        if (! player.immune) this.StatusPlayer (player);

                        int dmg = Main.DamageVar (this.damage);
                        player.Hurt (dmg * 2, hitDirection, false, false, " was slain...");
                        
                        //NetMessage.SendData(26, -1, -1, "", playerIndex, (float)this.direction, (float)(this.damage * 2));
                    }
                }
            }
#endif //CLIENT_CODE
		}

		/// <summary>
		/// Updates the projectile's position, damage variables, etc.
		/// </summary>
		/// <param name="TileRefs">Reference to the ITile method, For usage between Sandbox and Realtime</param>
		/// <param name="sandbox">Sandbox instance if needed</param>
		/// <param name="i">Projectile index</param>
		public void Update(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox, int i)
		{
			if (this.Active)
			{
				Vector2 value = this.Velocity;
				if (this.Position.X <= Main.leftWorld || this.Position.X + (float)this.Width >= Main.rightWorld || this.Position.Y <= Main.topWorld || this.Position.Y + (float)this.Height >= Main.bottomWorld)
				{
					this.Active = false;
					Reset(i);
					return;
				}
				this.whoAmI = i;
				if (this.soundDelay > 0)
				{
					this.soundDelay--;
				}
				this.netUpdate = false;
				for (int j = 0; j < 255; j++)
				{
					if (this.playerImmune[j] > 0)
					{
						this.playerImmune[j]--;
					}
				}
				this.AI(TileRefs, sandbox);
				if (this.Owner < 255 && !Main.players[this.Owner].Active)
				{
					this.Kill(TileRefs, sandbox); // FIXME!
				}
				if (!this.ignoreWater)
				{
					bool flag;
					bool flag2;
					try
					{
						flag = Collision.LavaCollision(this.Position, this.Width, this.Height);
						flag2 = Collision.WetCollision(this.Position, this.Width, this.Height);
						if (flag)
						{
							this.lavaWet = true;
						}
					}
					catch
					{
						this.Active = false;
						Reset(i);
						return;
					}
					if (this.wet && !this.lavaWet)
					{
						if (this.type == ProjectileType.N85_FLAMES ||
							this.type == ProjectileType.N15_BALL_OF_FIRE ||
							this.type == ProjectileType.N34_FLAMELASH)
						{
							this.Kill(TileRefs, sandbox);
						}
						if (this.type == ProjectileType.N2_FIRE_ARROW)
						{
							this.type = ProjectileType.N1_WOODEN_ARROW;
							this.light = 0f;
						}
					}
					if (this.type == ProjectileType.N80_ICE_BLOCK)
					{
						flag2 = false;
						this.wet = false;
						if (flag && this.ai[0] >= 0f)
						{
							this.Kill(TileRefs, sandbox);
						}
					}
					if (flag2)
					{
						if (this.wetCount == 0)
						{
							this.wetCount = 10;
							this.wet = true;
						}
					}
					else
					{
						if (this.wet)
						{
							this.wet = false;
							if (this.wetCount == 0)
							{
								this.wetCount = 10;
							}
						}
					}
					if (!this.wet)
					{
						this.lavaWet = false;
					}
					if (this.wetCount > 0)
					{
						this.wetCount -= 1;
					}
				}
				this.lastPosition = this.Position;
				if (this.tileCollide)
				{
					Vector2 value2 = this.Velocity;
					bool flag3 = true;
					if (this.type == ProjectileType.N9_STARFURY ||
						this.type == ProjectileType.N12_FALLING_STAR ||
						this.type == ProjectileType.N15_BALL_OF_FIRE ||
						this.type == ProjectileType.N13_HOOK ||
						this.type == ProjectileType.N31_SAND_BALL ||
						this.type == ProjectileType.N39_MUD_BALL ||
						this.type == ProjectileType.N40_ASH_BALL)
					{
						flag3 = false;
					}
					if (this.aiStyle == 10)
					{
						if (this.type == ProjectileType.N42_SAND_BALL ||
							this.type == ProjectileType.N65_EBONSAND_BALL ||
							this.type == ProjectileType.N68_PEARL_SAND_BALL ||
							(this.type == ProjectileType.N31_SAND_BALL && this.ai[0] == 2f))
						{
							this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.Width, this.Height, flag3, flag3);
						}
						else
						{
							this.Velocity = Collision.AnyCollision(this.Position, this.Velocity, this.Width, this.Height);
						}
					}
					else
					{
						if (this.aiStyle == 18)
						{
							int num5 = this.Width - 36;
							int num6 = this.Height - 36;
							Vector2 vector = new Vector2(this.Position.X + (float)(this.Width / 2) - (float)(num5 / 2), this.Position.Y + (float)(this.Height / 2) - (float)(num6 / 2));
							this.Velocity = Collision.TileCollision(vector, this.Velocity, num5, num6, flag3, flag3);
						}
						else
						{
							if (this.wet)
							{
								Vector2 vector2 = this.Velocity;
								this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.Width, this.Height, flag3, flag3);
								value = this.Velocity * 0.5f;
								if (this.Velocity.X != vector2.X)
								{
									value.X = this.Velocity.X;
								}
								if (this.Velocity.Y != vector2.Y)
								{
									value.Y = this.Velocity.Y;
								}
							}
							else
							{
								this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.Width, this.Height, flag3, flag3);
							}
						}
					}
					if (value2 != this.Velocity && this.type != ProjectileType.N111_BUNNY)
					{
						if (this.type == ProjectileType.N94_CRYSTAL_STORM)
						{
							if (this.Velocity.X != value2.X)
							{
								this.Velocity.X = -value2.X;
							}
							if (this.Velocity.Y != value2.Y)
							{
								this.Velocity.Y = -value2.Y;
							}
						}
						else if (this.type == ProjectileType.N99_BOULDER)
						{
							if (this.Velocity.Y != value2.Y && value2.Y > 5f)
							{
								Collision.HitTiles(TileRefs, sandbox, this.Position, this.Velocity, this.Width, this.Height);
								this.Velocity.Y = -value2.Y * 0.2f;
							}
							if (this.Velocity.X != value2.X)
							{
								this.Kill(TileRefs, sandbox);
							}
						}
						else if (this.type == ProjectileType.N36_METEOR_SHOT)
						{
							if (this.penetrate > 1)
							{
								Collision.HitTiles(TileRefs, sandbox, this.Position, this.Velocity, this.Width, this.Height);
								this.penetrate--;
								if (this.Velocity.X != value2.X)
								{
									this.Velocity.X = -value2.X;
								}
								if (this.Velocity.Y != value2.Y)
								{
									this.Velocity.Y = -value2.Y;
								}
							}
							else
							{
								this.Kill(TileRefs, sandbox);
							}
						}
						else
						{
							if (this.aiStyle == 21)
							{
								if (this.Velocity.X != value2.X)
								{
									this.Velocity.X = -value2.X;
								}
								if (this.Velocity.Y != value2.Y)
								{
									this.Velocity.Y = -value2.Y;
								}
							}
							if (this.aiStyle == 17)
							{
								if (this.Velocity.X != value2.X)
								{
									this.Velocity.X = value2.X * -0.75f;
								}
								if (this.Velocity.Y != value2.Y && (double)value2.Y > 1.5)
								{
									this.Velocity.Y = value2.Y * -0.7f;
								}
							}
							else
							{
								if (this.aiStyle == 15)
								{
									bool flag4 = false;
									if (value2.X != this.Velocity.X)
									{
										if (Math.Abs(value2.X) > 4f)
										{
											flag4 = true;
										}
										this.Position.X = this.Position.X + this.Velocity.X;
										this.Velocity.X = -value2.X * 0.2f;
									}
									if (value2.Y != this.Velocity.Y)
									{
										if (Math.Abs(value2.Y) > 4f)
										{
											flag4 = true;
										}
										this.Position.Y = this.Position.Y + this.Velocity.Y;
										this.Velocity.Y = -value2.Y * 0.2f;
									}
									this.ai[0] = 1f;
									if (flag4)
									{
										this.netUpdate = true;
										Collision.HitTiles(TileRefs, sandbox, this.Position, this.Velocity, this.Width, this.Height);
									}
								}
								else if (this.aiStyle == 3 || this.aiStyle == 13)
								{
									Collision.HitTiles(TileRefs, sandbox, this.Position, this.Velocity, this.Width, this.Height);
									if (this.type == ProjectileType.N33_THORN_CHAKRUM)
									{
										if (this.Velocity.X != value2.X)
										{
											this.Velocity.X = -value2.X;
										}
										if (this.Velocity.Y != value2.Y)
										{
											this.Velocity.Y = -value2.Y;
										}
									}
									else
									{
										this.ai[0] = 1f;
										if (this.aiStyle == 3)
										{
											this.Velocity.X = -value2.X;
											this.Velocity.Y = -value2.Y;
										}
									}
									this.netUpdate = true;
								}
								else
								{
									if (this.aiStyle == 8 && this.type != ProjectileType.N96_CURSED_FLAME)
									{
										this.ai[0] += 1f;
										if (this.ai[0] >= 5f)
										{
											this.Position += this.Velocity;
											this.Kill(TileRefs, sandbox);
										}
										else
										{
											if (this.type == ProjectileType.N15_BALL_OF_FIRE && this.Velocity.Y > 4f)
											{
												if (this.Velocity.Y != value2.Y)
												{
													this.Velocity.Y = -value2.Y * 0.8f;
												}
											}
											else if (this.Velocity.Y != value2.Y)
											{
												this.Velocity.Y = -value2.Y;
											}
											if (this.Velocity.X != value2.X)
											{
												this.Velocity.X = -value2.X;
											}
										}
									}
									else
									{
										if (this.aiStyle == 14)
										{
											if (this.type == ProjectileType.N50_GLOWSTICK)
											{
												if (this.Velocity.X != value2.X)
												{
													this.Velocity.X = value2.X * -0.2f;
												}
												if (this.Velocity.Y != value2.Y && (double)value2.Y > 1.5)
												{
													this.Velocity.Y = value2.Y * -0.2f;
												}
											}
											else
											{
												if (this.Velocity.X != value2.X)
												{
													this.Velocity.X = value2.X * -0.5f;
												}
												if (this.Velocity.Y != value2.Y && value2.Y > 1f)
												{
													this.Velocity.Y = value2.Y * -0.5f;
												}
											}
										}
										else
										{
											if (this.aiStyle == 16)
											{
												if (this.Velocity.X != value2.X)
												{
													this.Velocity.X = value2.X * -0.4f;
													if (this.type == ProjectileType.N29_DYNAMITE)
													{
														this.Velocity.X = this.Velocity.X * 0.8f;
													}
												}
												if (this.Velocity.Y != value2.Y && (double)value2.Y > 0.7 && this.type != ProjectileType.N102_BOMB)
												{
													this.Velocity.Y = value2.Y * -0.4f;
													if (this.type == ProjectileType.N29_DYNAMITE)
													{
														this.Velocity.Y = this.Velocity.Y * 0.8f;
													}
												}
											}
											else if (this.aiStyle != 9 || this.Owner == Main.myPlayer)
											{
												this.Position += this.Velocity;
												this.Kill(TileRefs, sandbox);
											}
										}
									}
								}
							}
						}
					}
				}
				if (this.type == ProjectileType.N7_VILETHORN || this.type == ProjectileType.N8_VILETHORN)
				{
					goto IL_D48;
				}
				if (this.wet)
				{
					this.Position += value;
					goto IL_D48;
				}
				this.Position += this.Velocity;
			IL_D48:
				if ((this.aiStyle != 3 || this.ai[0] != 1f) && (this.aiStyle != 7 || this.ai[0] != 1f) && (this.aiStyle != 13 || this.ai[0] != 1f) && (this.aiStyle != 15 || this.ai[0] != 1f))
				{
					if (this.Velocity.X < 0f)
					{
						this.direction = -1;
					}
					else
					{
						this.direction = 1;
					}
				}
				if (!this.Active)
				{
					return;
				}
				this.Damage(TileRefs, sandbox);
				if (this.type == ProjectileType.N99_BOULDER)
				{
					Collision.SwitchTiles(TileRefs, sandbox, this.Position, this.Width, this.Height, this.lastPosition, this.Creator);
				}
				if (this.type == ProjectileType.N94_CRYSTAL_STORM)
				{
					//for (int num12 = this.oldPos.Length - 1; num12 > 0; num12--)
					//{
					//     this.oldPos[num12] = this.oldPos[num12 - 1];
					// }
					//  this.oldPos[0] = this.Position;
				}
				this.timeLeft--;
				if (this.timeLeft <= 0)
				{
					this.Kill(TileRefs, sandbox);
				}
				if (this.penetrate == 0)
				{
					this.Kill(TileRefs, sandbox);
				}
				if (this.Active && this.Owner == Main.myPlayer)
				{
					if (this.netUpdate2)
					{
						this.netUpdate = true;
					}
					if (!this.Active)
					{
						this.netSpam = 0;
					}
					if (this.netUpdate)
					{
						if (this.netSpam < 60)
						{
							this.netSpam += 5;
							NetMessage.SendData(27, -1, -1, "", i);
							this.netUpdate2 = false;
						}
						else
						{
							this.netUpdate2 = true;
						}
					}
					if (this.netSpam > 0)
					{
						this.netSpam--;
					}
				}
				if (this.Active && this.maxUpdates > 0)
				{
					this.numUpdates--;
					if (this.numUpdates >= 0)
					{
						this.Update(TileRefs, sandbox, i);
					}
					else
					{
						this.numUpdates = this.maxUpdates;
					}
				}
				this.netUpdate = false;
			}
		}

		/// <summary>
		/// Moves the projectile according to the projectile's motion parameters, or AI
		/// </summary>
		public void AI(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			switch (this.aiStyle)
			{
				case 1:
					{
						if (this.type == ProjectileType.N20_GREEN_LASER || this.type == ProjectileType.N14_BULLET ||
							this.type == ProjectileType.N36_METEOR_SHOT || this.type == ProjectileType.N83_EYE_LASER ||
							this.type == ProjectileType.N84_PINK_LASER || this.type == ProjectileType.N89_CRYSTAL_BULLET ||
							this.type == ProjectileType.N100_DEATH_LASER || this.type == ProjectileType.N104_CURSED_BULLET)
						{
							if (this.alpha > 0)
							{
								this.alpha -= 15;
							}
							if (this.alpha < 0)
							{
								this.alpha = 0;
							}
						}
						if (this.type == ProjectileType.N88_PURPLE_LASER)
						{
							if (this.alpha > 0)
								this.alpha -= 10;
							if (this.alpha < 0)
								this.alpha = 0;
						}
						if (this.type != ProjectileType.N5_JESTERS_ARROW &&
							this.type != ProjectileType.N14_BULLET &&
							this.type != ProjectileType.N20_GREEN_LASER &&
							this.type != ProjectileType.N36_METEOR_SHOT &&
							this.type != ProjectileType.N38_HARPY_FEATHER &&
							this.type != ProjectileType.N83_EYE_LASER &&
							this.type != ProjectileType.N84_PINK_LASER &&
							this.type != ProjectileType.N88_PURPLE_LASER &&
							this.type != ProjectileType.N89_CRYSTAL_BULLET &&
							this.type != ProjectileType.N98_POISON_DART &&
							this.type != ProjectileType.N100_DEATH_LASER &&
							this.type != ProjectileType.N104_CURSED_BULLET)
						{
							this.ai[0] += 1f;
						}
						if (this.type == ProjectileType.N81_WOODEN_ARROW || this.type == ProjectileType.N91_HOLY_ARROW)
						{
							if (this.ai[0] >= 20f)
							{
								this.ai[0] = 20f;
								this.Velocity.Y = this.Velocity.Y + 0.07f;
							}
						}
						else
						{
							if (this.ai[0] >= 15f)
							{
								this.ai[0] = 15f;
								this.Velocity.Y = this.Velocity.Y + 0.1f;
							}
						}
						this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 1.57f;
						if (this.Velocity.Y > 16f)
						{
							this.Velocity.Y = 16f;
							return;
						}
					}
					break;
				case 2:
					{
						this.rotation += (Math.Abs(this.Velocity.X) + Math.Abs(this.Velocity.Y)) * 0.03f * (float)this.direction;

						if (this.type == ProjectileType.N69_HOLY_WATER || this.type == ProjectileType.N70_UNHOLY_WATER)
						{
							this.ai[0] += 1f;
							if (this.ai[0] >= 10f)
							{
								this.Velocity.Y = this.Velocity.Y + 0.25f;
								this.Velocity.X = this.Velocity.X * 0.99f;
							}
						}
						else
						{
							this.ai[0] += 1f;
							if (this.ai[0] >= 20f)
							{
								this.Velocity.Y = this.Velocity.Y + 0.4f;
								this.Velocity.X = this.Velocity.X * 0.97f;
							}
							else
							{
								if (this.type == ProjectileType.N48_THROWING_KNIFE || this.type == ProjectileType.N54_POISONED_KNIFE)
								{
									this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 1.57f;
								}
							}
						}
						if (this.Velocity.Y > 16f)
						{
							this.Velocity.Y = 16f;
						}
						if (this.type == ProjectileType.N54_POISONED_KNIFE && Main.rand.Next(20) == 0)
						{
							return;
						}
					}
					break;
				case 3:
					{
						if (this.soundDelay == 0)
						{
							this.soundDelay = 8;
						}
						if (this.ai[0] == 0f)
						{
							this.ai[1] += 1f;
							if (this.type == ProjectileType.N106_LIGHT_DISC)
							{
								if (this.ai[1] >= 45f)
								{
									this.ai[0] = 1f;
									this.ai[1] = 0f;
									this.netUpdate = true;
								}
							}
							else
							{
								if (this.ai[1] >= 30f)
								{
									this.ai[0] = 1f;
									this.ai[1] = 0f;
									this.netUpdate = true;
								}
							}
						}
						else
						{
							this.tileCollide = false;
							float num4 = 9f;
							float num5 = 0.4f;
							if (this.type == ProjectileType.N19_FLAMARANG)
							{
								num4 = 13f;
								num5 = 0.6f;
							}
							else if (this.type == ProjectileType.N33_THORN_CHAKRUM)
							{
								num4 = 15f;
								num5 = 0.8f;
							}
							else if (this.type == ProjectileType.N106_LIGHT_DISC)
							{
								num4 = 16f;
								num5 = 1.2f;
							}
							Vector2 vector = new Vector2(this.Position.X + (float)this.Width * 0.5f, this.Position.Y + (float)this.Height * 0.5f);
							float num6 = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2) - vector.X;
							float num7 = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].Height / 2) - vector.Y;
							float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
							num8 = num4 / num8;
							num6 *= num8;
							num7 *= num8;
							if (num8 > 3000f)
							{
								this.Kill(TileRefs, sandbox);
							}
							if (this.Velocity.X < num6)
							{
								this.Velocity.X = this.Velocity.X + num5;
								if (this.Velocity.X < 0f && num6 > 0f)
								{
									this.Velocity.X = this.Velocity.X + num5;
								}
							}
							else
							{
								if (this.Velocity.X > num6)
								{
									this.Velocity.X = this.Velocity.X - num5;
									if (this.Velocity.X > 0f && num6 < 0f)
									{
										this.Velocity.X = this.Velocity.X - num5;
									}
								}
							}
							if (this.Velocity.Y < num7)
							{
								this.Velocity.Y = this.Velocity.Y + num5;
								if (this.Velocity.Y < 0f && num7 > 0f)
								{
									this.Velocity.Y = this.Velocity.Y + num5;
								}
							}
							else
							{
								if (this.Velocity.Y > num7)
								{
									this.Velocity.Y = this.Velocity.Y - num5;
									if (this.Velocity.Y > 0f && num7 < 0f)
									{
										this.Velocity.Y = this.Velocity.Y - num5;
									}
								}
							}
							if (Main.myPlayer == this.Owner)
							{
								Rectangle rectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);
								if (Main.players[Owner].Intersects(rectangle))
								{
									this.Kill(TileRefs, sandbox);
								}
							}
						}
						if (this.type == ProjectileType.N106_LIGHT_DISC)
						{
							this.rotation += 0.3f * (float)this.direction;
							return;
						}
						this.rotation += 0.4f * (float)this.direction;
						return;
					}
				//break;
				case 4:
					{
						this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 1.57f;
						if (this.ai[0] == 0f)
						{
							this.alpha -= 50;
							if (this.alpha <= 0)
							{
								this.alpha = 0;
								this.ai[0] = 1f;
								if (this.ai[1] == 0f)
								{
									this.ai[1] += 1f;
									this.Position += this.Velocity * 1f;
								}
								if (this.type == ProjectileType.N7_VILETHORN && Main.myPlayer == this.Owner)
								{
									int num9 = (int)this.type;
									if (this.ai[1] >= 6f)
									{
										num9++;
									}
									int num10 = Projectile.NewProjectile(this.Position.X + this.Velocity.X + (float)(this.Width / 2), this.Position.Y + this.Velocity.Y + (float)(this.Height / 2), this.Velocity.X, this.Velocity.Y, (ProjectileType)Enum.ToObject(typeof(ProjectileType), num9), this.damage, this.knockBack, this.Owner);
									Main.projectile[num10].damage = this.damage;
									Main.projectile[num10].ai[1] = this.ai[1] + 1f;
									NetMessage.SendData(27, -1, -1, "", num10);
									return;
								}
							}
						}
						else
						{
							this.alpha += 5;
							if (this.alpha >= 255)
							{
								this.Kill(TileRefs, sandbox);
								return;
							}
						}
					}
					break;
				case 5:
					{
						if (this.type == ProjectileType.N92_HALLOW_STAR)
						{
							if (this.Position.Y > this.ai[1])
							{
								this.tileCollide = true;
							}
						}
						else
						{
							if (this.ai[1] == 0f && !Collision.SolidCollision(this.Position, this.Width, this.Height))
							{
								this.ai[1] = 1f;
								this.netUpdate = true;
							}
							if (this.ai[1] != 0f)
							{
								this.tileCollide = true;
							}
						}
						if (this.soundDelay == 0)
						{
							this.soundDelay = 20 + Main.rand.Next(40);
						}
						if (this.ai[0] == 0f)
						{
							this.ai[0] = 1f;
						}
						this.alpha += (int)(25f * this.ai[0]);
						if (this.alpha > 200)
						{
							this.alpha = 200;
							this.ai[0] = -1f;
						}
						if (this.alpha < 0)
						{
							this.alpha = 0;
							this.ai[0] = 1f;
						}
						this.rotation += (Math.Abs(this.Velocity.X) + Math.Abs(this.Velocity.Y)) * 0.01f * (float)this.direction;
						if (this.ai[1] == 1f || this.type == ProjectileType.N92_HALLOW_STAR)
						{
							this.light = 0.9f;
						}
					}
					break;
				case 6:
					{
						this.Velocity *= 0.95f;
						this.ai[0] += 1f;
						if (this.ai[0] == 180f)
						{
							this.Kill(TileRefs, sandbox);
						}
						if (this.ai[1] == 0f)
						{
							this.ai[1] = 1f;
						}
						if (this.type == ProjectileType.N10_PURIFICATION_POWDER || this.type == ProjectileType.N11_VILE_POWDER)
						{
							int num11 = (int)(this.Position.X / 16f) - 1;
							int num12 = (int)((this.Position.X + (float)this.Width) / 16f) + 2;
							int num13 = (int)(this.Position.Y / 16f) - 1;
							int num14 = (int)((this.Position.Y + (float)this.Height) / 16f) + 2;
							if (num11 < 0)
							{
								num11 = 0;
							}
							if (num12 > Main.maxTilesX)
							{
								num12 = Main.maxTilesX;
							}
							if (num13 < 0)
							{
								num13 = 0;
							}
							if (num14 > Main.maxTilesY)
							{
								num14 = Main.maxTilesY;
							}
							for (int l = num11; l < num12; l++)
							{
								for (int m = num13; m < num14; m++)
								{
									Vector2 vector2;
									vector2.X = (float)(l * 16);
									vector2.Y = (float)(m * 16);

									if (this.Position.X + (float)this.Width > vector2.X && this.Position.X < vector2.X + 16f && this.Position.Y + (float)this.Height > vector2.Y && this.Position.Y < vector2.Y + 16f
										&& (Program.properties.TileSquareMessages != "accept" || Main.myPlayer == this.Owner)
										&& TileRefs(l, m).Active)
									{
										if (this.type == ProjectileType.N10_PURIFICATION_POWDER)
										{
											if (TileRefs(l, m).Type == 23)
											{
												TileRefs(l, m).SetType(2);
												WorldModify.SquareTileFrame(TileRefs, sandbox,l, m, true);
												NetMessage.SendTileSquare(-1, l, m, 1);
											}
											if (TileRefs(l, m).Type == 25)
											{
												TileRefs(l, m).SetType(1);
												WorldModify.SquareTileFrame(TileRefs, sandbox,l, m, true);
												NetMessage.SendTileSquare(-1, l, m, 1);
											}
											if (TileRefs(l, m).Type == 112)
											{
												TileRefs(l, m).SetType(53);
												WorldModify.SquareTileFrame(TileRefs, sandbox,l, m, true);
												NetMessage.SendTileSquare(-1, l, m, 1);
											}
										}
										else if (this.type == ProjectileType.N11_VILE_POWDER)
										{
											if (TileRefs(l, m).Type == 109)
											{
												TileRefs(l, m).SetType(2);
												WorldModify.SquareTileFrame(TileRefs, sandbox,l, m, true);
												NetMessage.SendTileSquare(-1, l, m, 1);
											}
											if (TileRefs(l, m).Type == 116)
											{
												TileRefs(l, m).SetType(53);
												WorldModify.SquareTileFrame(TileRefs, sandbox,l, m, true);
												NetMessage.SendTileSquare(-1, l, m, 1);
											}
											if (TileRefs(l, m).Type == 117)
											{
												TileRefs(l, m).SetType(1);
												WorldModify.SquareTileFrame(TileRefs, sandbox,l, m, true);
												NetMessage.SendTileSquare(-1, l, m, 1);
											}
										}
									}
								}
							}
							return;
						}
					}
					break;
				case 7:
					{
						if (Main.players[this.Owner].dead)
						{
							this.Kill(TileRefs, sandbox);
							return;
						}
						Vector2 vector3 = new Vector2(this.Position.X + (float)this.Width * 0.5f, this.Position.Y + (float)this.Height * 0.5f);
						float num15 = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2) - vector3.X;
						float num16 = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].Height / 2) - vector3.Y;
						float num17 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
						this.rotation = (float)Math.Atan2((double)num16, (double)num15) - 1.57f;
						if (this.ai[0] == 0f)
						{
							if ((num17 > 300f && this.type == ProjectileType.N13_HOOK) || (num17 > 400f && this.type == ProjectileType.N32_IVY_WHIP))
							{
								this.ai[0] = 1f;
							}
							int num18 = (int)(this.Position.X / 16f) - 1;
							int num19 = (int)((this.Position.X + (float)this.Width) / 16f) + 2;
							int num20 = (int)(this.Position.Y / 16f) - 1;
							int num21 = (int)((this.Position.Y + (float)this.Height) / 16f) + 2;
							if (num18 < 0)
							{
								num18 = 0;
							}
							if (num19 > Main.maxTilesX)
							{
								num19 = Main.maxTilesX;
							}
							if (num20 < 0)
							{
								num20 = 0;
							}
							if (num21 > Main.maxTilesY)
							{
								num21 = Main.maxTilesY;
							}
							for (int n = num18; n < num19; n++)
							{
								int num22 = num20;
								while (num22 < num21)
								{
									Vector2 vector4;
									vector4.X = (float)(n * 16);
									vector4.Y = (float)(num22 * 16);
									if (this.Position.X + (float)this.Width > vector4.X && this.Position.X < vector4.X + 16f && this.Position.Y + (float)this.Height > vector4.Y && this.Position.Y < vector4.Y + 16f && TileRefs(n, num22).Active && Main.tileSolid[(int)TileRefs(n, num22).Type])
									{
										if (Main.players[this.Owner].grapCount < 10)
										{
											Main.players[this.Owner].grappling[Main.players[this.Owner].grapCount] = this.whoAmI;
											Main.players[this.Owner].grapCount++;
										}
										if (Main.myPlayer == this.Owner)
										{
											int num23 = 0;
											int num24 = -1;
											int num25 = 100000;
											if (this.type == ProjectileType.N73_HOOK || this.type == ProjectileType.N74_HOOK)
											{
												for (int num31 = 0; num31 < 1000; num31++)
												{
													if (num31 != this.whoAmI && Main.projectile[num31].Active && Main.projectile[num31].Owner == this.Owner &&
														Main.projectile[num31].aiStyle == 7 && Main.projectile[num31].ai[0] == 2f)
													{
														Main.projectile[num31].Kill(TileRefs, sandbox);
													}
												}
											}
											else
											{
												for (int num26 = 0; num26 < 1000; num26++)
												{
													if (Main.projectile[num26].Active && Main.projectile[num26].Owner == this.Owner && Main.projectile[num26].aiStyle == 7)
													{
														if (Main.projectile[num26].timeLeft < num25)
														{
															num24 = num26;
															num25 = Main.projectile[num26].timeLeft;
														}
														num23++;
													}
												}
												if (num23 > 3)
												{
													Main.projectile[num24].Kill(TileRefs, sandbox);
												}
											}
										}
										var plr = Creator as Player;
										if (plr == null || WorldModify.InvokeAlterationHook(this, plr, n, num22, 0))
										{
											WorldModify.KillTile(TileRefs, sandbox, n, num22, true, true);
										}
										this.Velocity.X = 0f;
										this.Velocity.Y = 0f;
										this.ai[0] = 2f;
										this.Position.X = (float)(n * 16 + 8 - this.Width / 2);
										this.Position.Y = (float)(num22 * 16 + 8 - this.Height / 2);
										this.damage = 0;
										this.netUpdate = true;
										if (Main.myPlayer == this.Owner)
										{
											NetMessage.SendData(13, -1, -1, "", this.Owner);
											break;
										}
										break;
									}
									else
									{
										num22++;
									}
								}
								if (this.ai[0] == 2f)
								{
									return;
								}
							}
							return;
						}
						if (this.ai[0] == 1f)
						{
							float num27 = 11f;
							if (this.type == ProjectileType.N32_IVY_WHIP)
							{
								num27 = 15f;
							}
							if (this.type == ProjectileType.N73_HOOK || this.type == ProjectileType.N74_HOOK)
							{
								num27 = 17f;
							}
							if (num17 < 24f)
							{
								this.Kill(TileRefs, sandbox);
							}
							num17 = num27 / num17;
							num15 *= num17;
							num16 *= num17;
							this.Velocity.X = num15;
							this.Velocity.Y = num16;
							return;
						}
						if (this.ai[0] == 2f)
						{
							int num28 = (int)(this.Position.X / 16f) - 1;
							int num29 = (int)((this.Position.X + (float)this.Width) / 16f) + 2;
							int num30 = (int)(this.Position.Y / 16f) - 1;
							int num31 = (int)((this.Position.Y + (float)this.Height) / 16f) + 2;
							if (num28 < 0)
							{
								num28 = 0;
							}
							if (num29 > Main.maxTilesX)
							{
								num29 = Main.maxTilesX;
							}
							if (num30 < 0)
							{
								num30 = 0;
							}
							if (num31 > Main.maxTilesY)
							{
								num31 = Main.maxTilesY;
							}
							bool flag = true;
							for (int num32 = num28; num32 < num29; num32++)
							{
								for (int num33 = num30; num33 < num31; num33++)
								{
									Vector2 vector5;
									vector5.X = (float)(num32 * 16);
									vector5.Y = (float)(num33 * 16);
									if (this.Position.X + (float)(this.Width / 2) > vector5.X && this.Position.X + (float)(this.Width / 2) < vector5.X + 16f && this.Position.Y + (float)(this.Height / 2) > vector5.Y && this.Position.Y + (float)(this.Height / 2) < vector5.Y + 16f && TileRefs(num32, num33).Active && Main.tileSolid[(int)TileRefs(num32, num33).Type])
									{
										flag = false;
									}
								}
							}
							if (flag)
							{
								this.ai[0] = 1f;
								return;
							}
							if (Main.players[this.Owner].grapCount < 10)
							{
								Main.players[this.Owner].grappling[Main.players[this.Owner].grapCount] = this.whoAmI;
								Main.players[this.Owner].grapCount++;
								return;
							}
						}
					}
					break;
				case 8:
					{
						if (this.type == ProjectileType.N96_CURSED_FLAME && this.ai[0] == 0f)
						{
							this.ai[0] = 1f;
						}
						if (this.type != ProjectileType.N27_WATER_BOLT && this.type != ProjectileType.N96_CURSED_FLAME)
						{
							this.ai[1] += 1f;
						}
						if (this.ai[1] >= 20f)
						{
							this.Velocity.Y = this.Velocity.Y + 0.2f;
						}
						this.rotation += 0.3f * (float)this.direction;
						if (this.Velocity.Y > 16f)
						{
							this.Velocity.Y = 16f;
							return;
						}
					}
					break;
				case 9:
					{
						if (Main.myPlayer == this.Owner && this.ai[0] == 0f)
						{
							// client-code, not updated from 1.0.6
							if (!Main.players[this.Owner].channel)
							{
								this.Kill(TileRefs, sandbox);
							}
						}
						if (this.type == ProjectileType.N34_FLAMELASH)
						{
							this.rotation += 0.3f * (float)this.direction;
						}
						else
						{
							if (this.Velocity.X != 0f || this.Velocity.Y != 0f)
							{
								this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) - 2.355f;
							}
						}
						if (this.Velocity.Y > 16f)
						{
							this.Velocity.Y = 16f;
							return;
						}
					}
					break;
				case 10:
					{
						if (Main.myPlayer == this.Owner && this.ai[0] == 0f)
						{
							// client-code, not updated from 1.0.6
							if (!Main.players[this.Owner].channel)
							{
								this.ai[0] = 1f;
								this.netUpdate = true;
							}
						}
						if (this.ai[0] == 1f)
						{
							if (this.type == ProjectileType.N42_SAND_BALL ||
								this.type == ProjectileType.N65_EBONSAND_BALL ||
								this.type == ProjectileType.N68_PEARL_SAND_BALL)
							{
								this.ai[1] += 1f;
								if (this.ai[1] >= 60f)
								{
									this.ai[1] = 60f;
									this.Velocity.Y = this.Velocity.Y + 0.2f;
								}
							}
							else
							{
								this.Velocity.Y = this.Velocity.Y + 0.41f;
							}
						}
						else
						{
							if (this.ai[0] == 2f)
							{
								this.Velocity.Y = this.Velocity.Y + 0.2f;
								if ((double)this.Velocity.X < -0.04)
								{
									this.Velocity.X = this.Velocity.X + 0.04f;
								}
								else
								{
									if ((double)this.Velocity.X > 0.04)
									{
										this.Velocity.X = this.Velocity.X - 0.04f;
									}
									else
									{
										this.Velocity.X = 0f;
									}
								}
							}
						}
						this.rotation += 0.1f;
						if (this.Velocity.Y > 10f)
						{
							this.Velocity.Y = 10f;
							return;
						}
					}
					break;
				case 11:
					{
						if (this.type == ProjectileType.N72_BLUE_FAIRY || this.type == ProjectileType.N86_PINK_FAIRY || this.type == ProjectileType.N87_PINK_FAIRY)
						{
							if (this.Velocity.X > 0f)
							{
								this.direction = -1;
							}
							else if (this.Velocity.X < 0f)
							{
								this.direction = 1;
							}
							this.rotation = this.Velocity.X * 0.1f;
						}
						else
						{
							this.rotation += 0.02f;
						}
						if (Main.myPlayer == this.Owner)
						{
							if (this.type == ProjectileType.N72_BLUE_FAIRY || this.type == ProjectileType.N86_PINK_FAIRY || this.type == ProjectileType.N87_PINK_FAIRY)
							{
								// [TODO] 1.1 : double check the player class
								if (Main.players[this.Owner].fairy)
								{
									this.timeLeft = 2;
								}
							}
							else
							{
								if (Main.players[this.Owner].lightOrb)
								{
									this.timeLeft = 2;
								}
							}
							if (Main.players[this.Owner].dead)
							{
								this.Kill(TileRefs, sandbox);
								return;
							}
							float num51 = 2.5f;
							int num81 = 70;
							if (this.type == ProjectileType.N72_BLUE_FAIRY || this.type == ProjectileType.N86_PINK_FAIRY || this.type == ProjectileType.N87_PINK_FAIRY)
							{
								num51 = 3.5f;
								num81 = 40;
							}
							Vector2 vector8 = new Vector2(this.Position.X + (float)this.Width * 0.5f, this.Position.Y + (float)this.Height * 0.5f);
							float num52 = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2) - vector8.X;
							float num53 = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].Height / 2) - vector8.Y;
							float num54 = (float)Math.Sqrt((double)(num52 * num52 + num53 * num53));
							num54 = (float)Math.Sqrt((double)(num52 * num52 + num53 * num53));
							if (num54 > 800f)
							{
								this.Position.X = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2) - (float)(this.Width / 2);
								this.Position.Y = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].Height / 2) - (float)(this.Height / 2);
								return;
							}
							if (num54 > (float)num81)
							{
								num54 = num51 / num54;
								num52 *= num54;
								num53 *= num54;
								// [TODO] 1.1 - double check this area of code in aiStyle 11
								// Commented out in 1.1
								//if (num52 != this.Velocity.X || num53 != this.Velocity.Y)
								//{
								//   this.netUpdate = true;
								//}
								this.Velocity.X = num52;
								this.Velocity.Y = num53;
								return;
							}
							// Commented out in 1.1
							//if (this.Velocity.X != 0f || this.Velocity.Y != 0f)
							//{
							//    this.netUpdate = true;
							//}
							this.Velocity.X = 0f;
							this.Velocity.Y = 0f;
							return;
						}
					}
					break;
				case 12:
					{
						this.scale -= 0.04f;
						if (this.scale <= 0f)
						{
							this.Kill(TileRefs, sandbox);
						}
						if (this.ai[0] > 4f)
						{
							this.alpha = 150;
							this.light = 0.8f;
						}
						else
						{
							this.ai[0] += 1f;
						}
						this.rotation += 0.3f * (float)this.direction;
						return;
					}
				//break;
				case 13:
					{
						if (Main.players[this.Owner].dead)
						{
							this.Kill(TileRefs, sandbox);
							return;
						}
						Main.players[this.Owner].itemAnimation = 5;
						Main.players[this.Owner].itemTime = 5;
						if (this.Position.X + (float)(this.Width / 2) > Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2))
						{
							Main.players[this.Owner].direction = 1;
						}
						else
						{
							Main.players[this.Owner].direction = -1;
						}
						Vector2 vector9 = new Vector2(this.Position.X + (float)this.Width * 0.5f, this.Position.Y + (float)this.Height * 0.5f);
						float num56 = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2) - vector9.X;
						float num57 = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].Height / 2) - vector9.Y;
						float num58 = (float)Math.Sqrt((double)(num56 * num56 + num57 * num57));
						if (this.ai[0] == 0f)
						{
							if (num58 > 700f)
							{
								this.ai[0] = 1f;
							}
							this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 1.57f;
							this.ai[1] += 1f;
							if (this.ai[1] > 2f)
							{
								this.alpha = 0;
							}
							if (this.ai[1] >= 10f)
							{
								this.ai[1] = 15f;
								this.Velocity.Y = this.Velocity.Y + 0.3f;
								return;
							}
						}
						else
						{
							if (this.ai[0] == 1f)
							{
								this.tileCollide = false;
								this.rotation = (float)Math.Atan2((double)num57, (double)num56) - 1.57f;
								float num59 = 11f;
								if (num58 < 50f)
								{
									this.Kill(TileRefs, sandbox);
								}
								num58 = num59 / num58;
								num56 *= num58;
								num57 *= num58;
								this.Velocity.X = num56;
								this.Velocity.Y = num57;
								return;
							}
						}
					}
					break;
				case 14:
					{
						if (this.type == ProjectileType.N53_STICKY_GLOWSTICK)
						{
							try
							{
								Vector2 vector10 = Collision.TileCollision(this.Position, this.Velocity, this.Width, this.Height, false, false);
								bool flag1 = this.Velocity != vector10;
								int num66 = ((int)(this.Position.X / 16f)) - 1;
								int num67 = ((int)((this.Position.X + this.Width) / 16f)) + 2;
								int num68 = ((int)(this.Position.Y / 16f)) - 1;
								int num69 = ((int)((this.Position.Y + this.Height) / 16f)) + 2;
								if (num66 < 0)
								{
									num66 = 0;
								}
								if (num67 > Main.maxTilesX)
								{
									num67 = Main.maxTilesX;
								}
								if (num68 < 0)
								{
									num68 = 0;
								}
								if (num69 > Main.maxTilesY)
								{
									num69 = Main.maxTilesY;
								}
								for (int num70 = num66; num70 < num67; num70++)
								{
									for (int num71 = num68; num71 < num69; num71++)
									{
										if ((TileRefs(num70, num71).Active) && (Main.tileSolid[TileRefs(num70, num71).Type] || (Main.tileSolidTop[TileRefs(num70, num71).Type] && (TileRefs(num70, num71).FrameY == 0))))
										{
											Vector2 vector11;
											vector11.X = num70 * 0x10;
											vector11.Y = num71 * 0x10;
											if ((((this.Position.X + this.Width) > vector11.X) && (this.Position.X < (vector11.X + 16f))) && (((this.Position.Y + this.Height) > vector11.Y) && (this.Position.Y < (vector11.Y + 16f))))
											{
												this.Velocity.X = 0f;
												this.Velocity.Y = -0.2f;
											}
										}
									}
								}
							}
							catch
							{
							}
						}

						this.ai[0] += 1f;
						if (this.ai[0] > 5f)
						{
							this.ai[0] = 5f;
							if (this.Velocity.Y == 0f && this.Velocity.X != 0f)
							{
								this.Velocity.X = this.Velocity.X * 0.97f;
								if ((double)this.Velocity.X > -0.01 && (double)this.Velocity.X < 0.01)
								{
									this.Velocity.X = 0f;
									this.netUpdate = true;
								}
							}
							this.Velocity.Y = this.Velocity.Y + 0.2f;
						}
						this.rotation += this.Velocity.X * 0.1f;
						if (this.Velocity.Y > 16f)
						{
							this.Velocity.Y = 16f;
							return;
						}
					}
					break;
				case 15:
					{
						if (Main.players[this.Owner].dead)
						{
							this.Kill(TileRefs, sandbox);
							return;
						}
						Main.players[this.Owner].itemAnimation = 10;
						Main.players[this.Owner].itemTime = 10;
						if (this.Position.X + (float)(this.Width / 2) > Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2))
						{
							Main.players[this.Owner].direction = 1;
							this.direction = 1;
						}
						else
						{
							Main.players[this.Owner].direction = -1;
							this.direction = -1;
						}
						Vector2 vector11 = new Vector2(this.Position.X + (float)this.Width * 0.5f, this.Position.Y + (float)this.Height * 0.5f);
						float num73 = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2) - vector11.X;
						float num74 = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].Height / 2) - vector11.Y;
						float num75 = (float)Math.Sqrt((double)(num73 * num73 + num74 * num74));

						if (this.ai[0] == 0f)
						{
							float num98 = 160f;
							if (this.type == ProjectileType.N63_THE_DAO_OF_POW)
							{
								num98 *= 1.5f;
							}
							this.tileCollide = true;
							if (num75 > num98)
							{
								this.ai[0] = 1f;
								this.netUpdate = true;
							}
							else
							{
								if (!Main.players[this.Owner].channel)
								{
									if (this.Velocity.Y < 0f)
									{
										this.Velocity.Y = this.Velocity.Y * 0.9f;
									}
									this.Velocity.Y = this.Velocity.Y + 1f;
									this.Velocity.X = this.Velocity.X * 0.9f;
								}
							}
						}
						else
						{
							if (this.ai[0] == 1f)
							{
								var owner = Main.players[this.Owner];
								float num76 = 14f / owner.meleeSpeed;
								float num77 = 0.9f / owner.meleeSpeed;
								float num101 = 300f;
								if (this.type == ProjectileType.N63_THE_DAO_OF_POW)
								{
									num101 *= 1.5f;
									num76 *= 1.5f;
									num77 *= 1.5f;
								}
								Math.Abs(num73);
								Math.Abs(num74);
								if (this.ai[1] == 1f)
								{
									this.tileCollide = false;
								}
								if (!owner.channel || num75 > num101 || !this.tileCollide)
								{
									this.ai[1] = 1f;
									if (this.tileCollide)
									{
										this.netUpdate = true;
									}
									this.tileCollide = false;
									if (num75 < 20f)
									{
										this.Kill(TileRefs, sandbox);
									}
								}
								if (!this.tileCollide)
								{
									num77 *= 2f;
								}
								if (num75 > 60f || !this.tileCollide)
								{
									num75 = num76 / num75;
									num73 *= num75;
									num74 *= num75;
									float num78 = num73 - this.Velocity.X;
									float num79 = num74 - this.Velocity.Y;
									float num80 = (float)Math.Sqrt((double)(num78 * num78 + num79 * num79));
									num80 = num77 / num80;
									num78 *= num80;
									num79 *= num80;
									this.Velocity.X = this.Velocity.X * 0.98f;
									this.Velocity.Y = this.Velocity.Y * 0.98f;
									this.Velocity.X = this.Velocity.X + num78;
									this.Velocity.Y = this.Velocity.Y + num79;
								}
								else
								{
									if (Math.Abs(this.Velocity.X) + Math.Abs(this.Velocity.Y) < 6f)
									{
										this.Velocity.X = this.Velocity.X * 0.96f;
										this.Velocity.Y = this.Velocity.Y + 0.2f;
									}
									if (owner.Velocity.X == 0f)
									{
										this.Velocity.X = this.Velocity.X * 0.96f;
									}
								}
							}
						}
						this.rotation = (float)Math.Atan2((double)num74, (double)num73) - this.Velocity.X * 0.1f;
						return;
					}
				//break;
				case 16:
					{
						if (this.type == ProjectileType.N108_EXPLOSIVES)
						{
							this.ai[0] += 1f;
							if (this.ai[0] > 3f)
							{
								this.Kill(TileRefs, sandbox);
							}
						}
						if (this.type == ProjectileType.N37_STICKY_BOMB)
						{
							try
							{
								int num72 = (int)(this.Position.X / 16f) - 1;
								int num73 = (int)((this.Position.X + (float)this.Width) / 16f) + 2;
								int num74 = (int)(this.Position.Y / 16f) - 1;
								int num75 = (int)((this.Position.Y + (float)this.Height) / 16f) + 2;
								if (num72 < 0)
								{
									num72 = 0;
								}
								if (num73 > Main.maxTilesX)
								{
									num73 = Main.maxTilesX;
								}
								if (num74 < 0)
								{
									num74 = 0;
								}
								if (num75 > Main.maxTilesY)
								{
									num75 = Main.maxTilesY;
								}
								for (int num76 = num72; num76 < num73; num76++)
								{
									for (int num77 = num74; num77 < num75; num77++)
									{
										if (TileRefs(num76, num77).Active && (Main.tileSolid[(int)TileRefs(num76, num77).Type] || (Main.tileSolidTop[(int)TileRefs(num76, num77).Type] && TileRefs(num76, num77).FrameY == 0)))
										{
											Vector2 vector12;
											vector12.X = (float)(num76 * 16);
											vector12.Y = (float)(num77 * 16);
											if (this.Position.X + (float)this.Width - 4f > vector12.X && this.Position.X + 4f < vector12.X + 16f && this.Position.Y + (float)this.Height - 4f > vector12.Y && this.Position.Y + 4f < vector12.Y + 16f)
											{
												this.Velocity.X = 0f;
												this.Velocity.Y = -0.2f;
											}
										}
									}
								}
							}
							catch
							{
							}
						}
						if (this.type == ProjectileType.N102_BOMB)
						{
							// [TODO] 1.1 - double check this code
							if (this.Velocity.Y > 10f)
							{
								this.Velocity.Y = 10f;
							}
							if (this.ai[0] == 0f)
							{
								this.ai[0] = 1f;
							}
							if (this.Velocity.Y == 0f)
							{
								this.Position.X = this.Position.X + (float)(this.Width / 2);
								this.Position.Y = this.Position.Y + (float)(this.Height / 2);
								this.Width = 128;
								this.Height = 128;
								this.Position.X = this.Position.X - (float)(this.Width / 2);
								this.Position.Y = this.Position.Y - (float)(this.Height / 2);
								this.damage = 40;
								this.knockBack = 8f;
								this.timeLeft = 3;
								this.netUpdate = true;
							}
						}
						if (this.Owner == Main.myPlayer && this.timeLeft <= 3)
						{
							this.ai[1] = 0f;
							this.alpha = 255;
							if (this.type == ProjectileType.N28_BOMB ||
								this.type == ProjectileType.N37_STICKY_BOMB ||
								this.type == ProjectileType.N75_HAPPY_BOMB)
							{
								this.Position.X = this.Position.X + (float)(this.Width / 2);
								this.Position.Y = this.Position.Y + (float)(this.Height / 2);
								this.Width = 128;
								this.Height = 128;
								this.Position.X = this.Position.X - (float)(this.Width / 2);
								this.Position.Y = this.Position.Y - (float)(this.Height / 2);
								this.damage = 100;
								this.knockBack = 8f;
							}
							else
							{
								if (this.type == ProjectileType.N29_DYNAMITE)
								{
									this.Position.X = this.Position.X + (float)(this.Width / 2);
									this.Position.Y = this.Position.Y + (float)(this.Height / 2);
									this.Width = 250;
									this.Height = 250;
									this.Position.X = this.Position.X - (float)(this.Width / 2);
									this.Position.Y = this.Position.Y - (float)(this.Height / 2);
									this.damage = 250;
									this.knockBack = 10f;
								}
								else
								{
									if (this.type == ProjectileType.N30_GRENADE)
									{
										this.Position.X = this.Position.X + (float)(this.Width / 2);
										this.Position.Y = this.Position.Y + (float)(this.Height / 2);
										this.Width = 128;
										this.Height = 128;
										this.Position.X = this.Position.X - (float)(this.Width / 2);
										this.Position.Y = this.Position.Y - (float)(this.Height / 2);
										this.knockBack = 8f;
									}
								}
							}
						}
						else
						{
							if (this.type != ProjectileType.N30_GRENADE && this.type != ProjectileType.N108_EXPLOSIVES)
							{
								this.damage = 0;
							}
						}
						this.ai[0] += 1f;
						if ((this.type == ProjectileType.N30_GRENADE && this.ai[0] > 10f) || (this.type != ProjectileType.N30_GRENADE && this.ai[0] > 5f))
						{
							this.ai[0] = 10f;
							if (this.Velocity.Y == 0f && this.Velocity.X != 0f)
							{
								this.Velocity.X = this.Velocity.X * 0.97f;
								if (this.type == ProjectileType.N29_DYNAMITE)
								{
									this.Velocity.X = this.Velocity.X * 0.99f;
								}
								if ((double)this.Velocity.X > -0.01 && (double)this.Velocity.X < 0.01)
								{
									this.Velocity.X = 0f;
									this.netUpdate = true;
								}
							}
							this.Velocity.Y = this.Velocity.Y + 0.2f;
						}
						this.rotation += this.Velocity.X * 0.1f;
						return;
					}
				//break;
				case 17:
					{
						if (this.Velocity.Y == 0f)
						{
							this.Velocity.X = this.Velocity.X * 0.98f;
						}
						this.rotation += this.Velocity.X * 0.1f;
						this.Velocity.Y = this.Velocity.Y + 0.2f;
						if (this.Owner == Main.myPlayer)
						{
							int num78 = (int)((this.Position.X + (float)this.Width) / 16f);
							int num79 = (int)((this.Position.Y + (float)this.Height) / 16f);
							if (TileRefs(num78, num79).Exists && !TileRefs(num78, num79).Active)
							{
								WorldModify.PlaceTile(TileRefs, sandbox, num78, num79, 85, false, false, -1, 0);
								if (TileRefs(num78, num79).Active)
								{

									NetMessage.SendData(17, -1, -1, "", 1, (float)num78, (float)num79, 85f);

									int num80 = Sign.ReadSign(num78, num79);
									if (num80 >= 0)
									{
										//Need to check if this works :3
										//PlayerEditSignEvent playerEvent = new PlayerEditSignEvent();
										//playerEvent.Sender = Main.players[this.Owner];
										//playerEvent.Sign = Main.sign[num80];
										//playerEvent.Text = this.miscText;
										//playerEvent.isPlayer = false;
										//Server.PluginManager.processHook(Hooks.PLAYER_EDITSIGN, playerEvent);
										//if (playerEvent.Cancelled)
										//{
										//    return;
										//}

										Sign.TextSign(num80, this.miscText);
									}
									this.Kill(TileRefs, sandbox);
									return;
								}
							}
						}
					}
					break;
				case 18:
					{
						if (this.ai[1] == 0f && this.type == ProjectileType.N44_DEMON_SICKLE)
						{
							this.ai[1] = 1f;
						}
						this.rotation += (float)this.direction * 0.8f;
						this.ai[0] += 1f;
						if (this.ai[0] >= 30f)
						{
							if (this.ai[0] < 100f)
							{
								this.Velocity *= 1.06f;
							}
							else
							{
								this.ai[0] = 200f;
							}
						}
						return;
					}
				//break;
				case 19:
					{
						this.direction = Main.players[this.Owner].direction;
						Main.players[this.Owner].heldProj = this.whoAmI;
						Main.players[this.Owner].itemTime = Main.players[this.Owner].itemAnimation;
						this.Position.X = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2) - (float)(this.Width / 2);
						this.Position.Y = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].Height / 2) - (float)(this.Height / 2);
						if (this.type == ProjectileType.N46_DARK_LANCE)
						{
							if (this.ai[0] == 0f)
							{
								this.ai[0] = 3f;
								this.netUpdate = true;
							}
							if (Main.players[this.Owner].itemAnimation < Main.players[this.Owner].itemAnimationMax / 3)
							{
								this.ai[0] -= 1.6f;
							}
							else
							{
								this.ai[0] += 1.4f;
							}
						}
						else
						{
							if (this.type == ProjectileType.N105_GUNGNIR)
							{
								if (this.ai[0] == 0f)
								{
									this.ai[0] = 3f;
									this.netUpdate = true;
								}
								if (Main.players[this.Owner].itemAnimation < Main.players[this.Owner].itemAnimationMax / 3)
								{
									this.ai[0] -= 2.4f;
								}
								else
								{
									this.ai[0] += 2.1f;
								}
							}
							if (this.type == ProjectileType.N47_TRIDENT)
							{
								if (this.ai[0] == 0f)
								{
									this.ai[0] = 4f;
									this.netUpdate = true;
								}
								if (Main.players[this.Owner].itemAnimation < Main.players[this.Owner].itemAnimationMax / 3)
								{
									this.ai[0] -= 1.2f;
								}
								else
								{
									this.ai[0] += 0.9f;
								}
							}
							else if (this.type == ProjectileType.N49_SPEAR)
							{
								if (this.ai[0] == 0f)
								{
									this.ai[0] = 4f;
									this.netUpdate = true;
								}
								if (Main.players[this.Owner].itemAnimation < Main.players[this.Owner].itemAnimationMax / 3)
								{
									this.ai[0] -= 1.1f;
								}
								else
								{
									this.ai[0] += 0.85f;
								}
							}
							else if (this.type == ProjectileType.N64_MYTHRIL_HALBERD)
							{
								if (this.ai[0] == 0f)
								{
									this.ai[0] = 3f;
									this.netUpdate = true;
								}
								if (Main.players[this.Owner].itemAnimation < Main.players[this.Owner].itemAnimationMax / 3)
								{
									this.ai[0] -= 1.9f;
								}
								else
								{
									this.ai[0] += 1.7f;
								}
							}
							else if (this.type == ProjectileType.N66_ADAMANTITE_GLAIVE || this.type == ProjectileType.N97_COBALT_NAGINATA)
							{
								if (this.ai[0] == 0f)
								{
									this.ai[0] = 3f;
									this.netUpdate = true;
								}
								if (Main.players[this.Owner].itemAnimation < Main.players[this.Owner].itemAnimationMax / 3)
								{
									this.ai[0] -= 2.1f;
								}
								else
								{
									this.ai[0] += 1.9f;
								}
							}
						}
						this.Position += this.Velocity * this.ai[0];
						if (Main.players[this.Owner].itemAnimation == 0)
						{
							this.Kill(TileRefs, sandbox);
						}
						this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 2.355f;
					}
					break;
				case 20:
					{
						if (this.soundDelay <= 0)
						{
							this.soundDelay = 30;
						}
						if (Main.myPlayer == this.Owner)
						{
							if (!Main.players[this.Owner].channel)
							{
								this.Kill(TileRefs, sandbox);
							}
						}
						if (this.Velocity.X > 0f)
						{
							Main.players[this.Owner].direction = 1;
						}
						else
						{
							if (this.Velocity.X < 0f)
							{
								Main.players[this.Owner].direction = -1;
							}
						}
						Main.players[this.Owner].direction = this.direction;
						Main.players[this.Owner].heldProj = this.whoAmI;
						Main.players[this.Owner].itemTime = 2;
						Main.players[this.Owner].itemAnimation = 2;
						this.Position.X = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2) - (float)(this.Width / 2);
						this.Position.Y = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].Height / 2) - (float)(this.Height / 2);
						this.rotation = (float)(Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 1.5700000524520874);
						if (Main.players[this.Owner].direction == 1)
						{
							Main.players[this.Owner].itemRotation = (float)Math.Atan2((double)(this.Velocity.Y * (float)this.direction), (double)(this.Velocity.X * (float)this.direction));
						}
						else
						{
							Main.players[this.Owner].itemRotation = (float)Math.Atan2((double)(this.Velocity.Y * (float)this.direction), (double)(this.Velocity.X * (float)this.direction));
						}
						this.Velocity.X = this.Velocity.X * (1f + (float)Main.rand.Next(-3, 4) * 0.01f);
						if (Main.rand.Next(6) == 0)
						{
							return;
						}
					}
					break;
				case 21:
					{
						this.rotation = this.Velocity.X * 0.1f;
						if (this.ai[1] == 1f)
						{
							this.ai[1] = 0f;
							Main.harpNote = this.ai[0];
							return;
						}
					}
					break;
				case 22:
					{
						if (this.Velocity.X == 0f && this.Velocity.Y == 0f)
						{
							this.alpha = 255;
						}
						if (this.ai[1] < 0f)
						{
							if (this.Velocity.X > 0f)
							{
								this.rotation += 0.3f;
							}
							else
							{
								this.rotation -= 0.3f;
							}
							int num125 = (int)(this.Position.X / 16f) - 1;
							int num126 = (int)((this.Position.X + (float)this.Width) / 16f) + 2;
							int num127 = (int)(this.Position.Y / 16f) - 1;
							int num128 = (int)((this.Position.Y + (float)this.Height) / 16f) + 2;
							if (num125 < 0)
							{
								num125 = 0;
							}
							if (num126 > Main.maxTilesX)
							{
								num126 = Main.maxTilesX;
							}
							if (num127 < 0)
							{
								num127 = 0;
							}
							if (num128 > Main.maxTilesY)
							{
								num128 = Main.maxTilesY;
							}
							int num129 = (int)this.Position.X + 4;
							int num130 = (int)this.Position.Y + 4;
							for (int num131 = num125; num131 < num126; num131++)
							{
								for (int num132 = num127; num132 < num128; num132++)
								{
									if (TileRefs(num131, num132).Exists && TileRefs(num131, num132).Active && TileRefs(num131, num132).Type != 127 &&
										Main.tileSolid[(int)TileRefs(num131, num132).Type] && !Main.tileSolidTop[(int)TileRefs(num131, num132).Type])
									{
										Vector2 vector15;
										vector15.X = (float)(num131 * 16);
										vector15.Y = (float)(num132 * 16);
										if ((float)(num129 + 8) > vector15.X && (float)num129 < vector15.X + 16f &&
											(float)(num130 + 8) > vector15.Y && (float)num130 < vector15.Y + 16f)
										{
											this.Kill(TileRefs, sandbox);
										}
									}
								}
							}
							return;
						}
						if (this.ai[0] < 0f)
						{
							int num137 = (int)this.Position.X / 16;
							int num138 = (int)this.Position.Y / 16;
							if (!TileRefs(num137, num138).Exists || !TileRefs(num137, num138).Active)
							{
								this.Kill(TileRefs, sandbox);
							}
							this.ai[0] -= 1f;
							if (this.ai[0] <= -300f && (Main.myPlayer == this.Owner) && TileRefs(num137, num138).Active && TileRefs(num137, num138).Type == 127)
							{
								WorldModify.KillTile(TileRefs, sandbox, num137, num138);
								NetMessage.SendData(17, -1, -1, "", 0, (float)num137, (float)num138, 0f, 0);
								this.Kill(TileRefs, sandbox);
								return;
							}
						}
						else
						{
							int num139 = (int)(this.Position.X / 16f) - 1;
							int num140 = (int)((this.Position.X + (float)this.Width) / 16f) + 2;
							int num141 = (int)(this.Position.Y / 16f) - 1;
							int num142 = (int)((this.Position.Y + (float)this.Height) / 16f) + 2;
							if (num139 < 0)
							{
								num139 = 0;
							}
							if (num140 > Main.maxTilesX)
							{
								num140 = Main.maxTilesX;
							}
							if (num141 < 0)
							{
								num141 = 0;
							}
							if (num142 > Main.maxTilesY)
							{
								num142 = Main.maxTilesY;
							}
							int num143 = (int)this.Position.X + 4;
							int num144 = (int)this.Position.Y + 4;
							for (int num145 = num139; num145 < num140; num145++)
							{
								for (int num146 = num141; num146 < num142; num146++)
								{
									if (TileRefs(num145, num146).Exists && TileRefs(num145, num146).Active && TileRefs(num145, num146).Type != 127
										&& Main.tileSolid[(int)TileRefs(num145, num146).Type] && !Main.tileSolidTop[(int)TileRefs(num145, num146).Type])
									{
										Vector2 vector16;
										vector16.X = (float)(num145 * 16);
										vector16.Y = (float)(num146 * 16);
										if ((float)(num143 + 8) > vector16.X && (float)num143 < vector16.X + 16f &&
											(float)(num144 + 8) > vector16.Y && (float)num144 < vector16.Y + 16f)
										{
											this.Kill(TileRefs, sandbox);
										}
									}
								}
							}
							if (this.lavaWet)
							{
								this.Kill(TileRefs, sandbox);
							}
							if (this.Active)
							{
								int num148 = (int)this.ai[0];
								int num149 = (int)this.ai[1];
								if (this.Velocity.X > 0f)
								{
									this.rotation += 0.3f;
								}
								else
								{
									this.rotation -= 0.3f;
								}
								if (Main.myPlayer == this.Owner)
								{
									int num150 = (int)((this.Position.X + (float)(this.Width / 2)) / 16f);
									int num151 = (int)((this.Position.Y + (float)(this.Height / 2)) / 16f);
									bool flag2 = false;
									if (num150 == num148 && num151 == num149)
									{
										flag2 = true;
									}
									if (((this.Velocity.X <= 0f && num150 <= num148) || (this.Velocity.X >= 0f && num150 >= num148)) &&
										((this.Velocity.Y <= 0f && num151 <= num149) || (this.Velocity.Y >= 0f && num151 >= num149)))
									{
										flag2 = true;
									}
									if (flag2)
									{
										if (WorldModify.PlaceTile(TileRefs, sandbox, num148, num149, 127, false, false, this.Owner, 0))
										{
											NetMessage.SendData(17, -1, -1, "", 1, (float)((int)this.ai[0]), (float)((int)this.ai[1]), 127f, 0);
											this.damage = 0;
											this.ai[0] = -1f;
											this.Velocity *= 0f;
											this.alpha = 255;
											this.Position.X = (float)(num148 * 16);
											this.Position.Y = (float)(num149 * 16);
											this.netUpdate = true;
											return;
										}
										this.ai[1] = -1f;
										return;
									}
								}
							}
						}
					}
					break;
				case 23:
					{
						if (this.timeLeft > 60)
						{
							this.timeLeft = 60;
						}
						if (this.ai[0] > 7f)
						{
							this.ai[0] += 1f;
						}
						else
						{
							this.ai[0] += 1f;
						}
						this.rotation += 0.3f * (float)this.direction;
						return;
					}
				//break;
				case 24:
					{
						this.light = this.scale * 0.5f;
						this.rotation += this.Velocity.X * 0.2f;
						this.ai[1] += 1f;
						if (this.type == ProjectileType.N94_CRYSTAL_STORM)
						{
							this.Velocity *= 0.985f;
							if (this.ai[1] > 130f)
							{
								this.scale -= 0.05f;
								if ((double)this.scale <= 0.2)
								{
									this.scale = 0.2f;
									this.Kill(TileRefs, sandbox);
									return;
								}
							}
						}
						else
						{
							this.Velocity *= 0.96f;
							if (this.ai[1] > 15f)
							{
								this.scale -= 0.05f;
								if ((double)this.scale <= 0.2)
								{
									this.scale = 0.2f;
									this.Kill(TileRefs, sandbox);
									return;
								}
							}
						}
					}
					break;
				case 25:
					{
						if (this.ai[0] != 0f && this.Velocity.Y <= 0f && this.Velocity.X == 0f)
						{
							float num157 = 0.5f;
							int i2 = (int)((this.Position.X - 8f) / 16f);
							int num158 = (int)(this.Position.Y / 16f);
							bool flag3 = false;
							bool flag4 = false;
							if (WorldModify.SolidTile(TileRefs, i2, num158) || WorldModify.SolidTile(TileRefs, i2, num158 + 1))
							{
								flag3 = true;
							}
							i2 = (int)((this.Position.X + (float)this.Width + 8f) / 16f);
							if (WorldModify.SolidTile(TileRefs, i2, num158) || WorldModify.SolidTile(TileRefs, i2, num158 + 1))
							{
								flag4 = true;
							}
							if (flag3)
							{
								this.Velocity.X = num157;
							}
							else
							{
								if (flag4)
								{
									this.Velocity.X = -num157;
								}
								else
								{
									i2 = (int)((this.Position.X - 8f - 16f) / 16f);
									num158 = (int)(this.Position.Y / 16f);
									flag3 = false;
									flag4 = false;
									if (WorldModify.SolidTile(TileRefs, i2, num158) || WorldModify.SolidTile(TileRefs, i2, num158 + 1))
									{
										flag3 = true;
									}
									i2 = (int)((this.Position.X + (float)this.Width + 8f + 16f) / 16f);
									if (WorldModify.SolidTile(TileRefs, i2, num158) || WorldModify.SolidTile(TileRefs, i2, num158 + 1))
									{
										flag4 = true;
									}
									if (flag3)
									{
										this.Velocity.X = num157;
									}
									else
									{
										if (flag4)
										{
											this.Velocity.X = -num157;
										}
										else
										{
											i2 = (int)((this.Position.X + 4f) / 16f);
											num158 = (int)((this.Position.Y + (float)this.Height + 8f) / 16f);
											if (WorldModify.SolidTile(TileRefs, i2, num158) || WorldModify.SolidTile(TileRefs, i2, num158 + 1))
											{
												flag3 = true;
											}
											if (!flag3)
											{
												this.Velocity.X = num157;
											}
											else
											{
												this.Velocity.X = -num157;
											}
										}
									}
								}
							}
						}
						this.rotation += this.Velocity.X * 0.06f;
						this.ai[0] = 1f;
						if (this.Velocity.Y > 16f)
						{
							this.Velocity.Y = 16f;
						}
						if (this.Velocity.Y <= 6f)
						{
							if (this.Velocity.X > 0f && this.Velocity.X < 7f)
							{
								this.Velocity.X = this.Velocity.X + 0.05f;
							}
							if (this.Velocity.X < 0f && this.Velocity.X > -7f)
							{
								this.Velocity.X = this.Velocity.X - 0.05f;
							}
						}
						this.Velocity.Y = this.Velocity.Y + 0.3f;
					}
					break;
				default:
					{
						// there are only aistyles 1 to 25
					}
					break;
			}
		}

		/// <summary>
		/// Destroys the projectile
		/// </summary>
		public void Kill(Func<Int32, Int32, ITile> TileRefs, ISandbox sandbox)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (!this.Active)
			{
				return;
			}
			this.timeLeft = 0;
			switch (this.type)
			{
				case ProjectileType.N14_BULLET:
				case ProjectileType.N20_GREEN_LASER:
				case ProjectileType.N36_METEOR_SHOT:
				case ProjectileType.N83_EYE_LASER:
				case ProjectileType.N84_PINK_LASER:
				case ProjectileType.N100_DEATH_LASER:
					{
						Collision.HitTiles(TileRefs, sandbox, this.Position, this.Velocity, this.Width, this.Height);
					}
					break;

				case ProjectileType.N28_BOMB:
				case ProjectileType.N30_GRENADE:
				case ProjectileType.N37_STICKY_BOMB:
				case ProjectileType.N75_HAPPY_BOMB:
				case ProjectileType.N102_BOMB:
					{
						this.Position.X = this.Position.X + (float)(this.Width / 2);
						this.Position.Y = this.Position.Y + (float)(this.Height / 2);
						this.Width = 22;
						this.Height = 22;
						this.Position.X = this.Position.X - (float)(this.Width / 2);
						this.Position.Y = this.Position.Y - (float)(this.Height / 2);
					}
					break;

				case ProjectileType.N29_DYNAMITE:
					{
						this.Position.X = this.Position.X + (float)(this.Width / 2);
						this.Position.Y = this.Position.Y + (float)(this.Height / 2);
						this.Width = 200;
						this.Height = 200;
						this.Position.X = this.Position.X - (float)(this.Width / 2);
						this.Position.Y = this.Position.Y - (float)(this.Height / 2);
					}
					break;

				case ProjectileType.N108_EXPLOSIVES:
					{
						this.Position.X = this.Position.X + (float)(this.Width / 2);
						this.Position.Y = this.Position.Y + (float)(this.Height / 2);
						this.Width = 10;
						this.Height = 10;
						this.Position.X = this.Position.X - (float)(this.Width / 2);
						this.Position.Y = this.Position.Y - (float)(this.Height / 2);
					}
					break;

				case ProjectileType.N41_HELLFIRE_ARROW:
					{
						if (this.Owner == Main.myPlayer)
						{
							this.penetrate = -1;
							this.Position.X = this.Position.X + (float)(this.Width / 2);
							this.Position.Y = this.Position.Y + (float)(this.Height / 2);
							this.Width = 64;
							this.Height = 64;
							this.Position.X = this.Position.X - (float)(this.Width / 2);
							this.Position.Y = this.Position.Y - (float)(this.Height / 2);
							this.Damage(TileRefs, sandbox);
						}
					}
					break;

				case ProjectileType.N80_ICE_BLOCK:
					{
						int num16 = (int)this.Position.X / 16;
						int num17 = (int)this.Position.Y / 16;
						if (!TileRefs(num16, num17).Exists)
						{
							Main.tile.CreateTileAt(num16, num17);
						}
						if (TileRefs(num16, num17).Type == 127 && TileRefs(num16, num17).Active)
						{
							WorldModify.KillTile(TileRefs, sandbox, num16, num17);
						}
					}
					break;

				case ProjectileType.N89_CRYSTAL_BULLET:
					{
						if (this.Owner == Main.myPlayer)
						{
							for (int num12 = 0; num12 < 3; num12++)
							{
								float num13 = -this.Velocity.X * (float)Main.rand.Next(40, 70) * 0.01f + (float)Main.rand.Next(-20, 21) * 0.4f;
								float num14 = -this.Velocity.Y * (float)Main.rand.Next(40, 70) * 0.01f + (float)Main.rand.Next(-20, 21) * 0.4f;
								Projectile.NewProjectile(this.Position.X + num13, this.Position.Y + num14, num13, num14, 90, (int)((double)this.damage * 0.6), 0f, this.Owner);
							}
						}
					}
					break;

				case ProjectileType.N91_HOLY_ARROW:
					{
						float x = this.Position.X + (float)Main.rand.Next(-400, 400);
						float y = this.Position.Y - (float)Main.rand.Next(600, 900);
						Vector2 vector = new Vector2(x, y);
						float num4 = this.Position.X + (float)(this.Width / 2) - vector.X;
						float num5 = this.Position.Y + (float)(this.Height / 2) - vector.Y;
						int num6 = 22;
						float num7 = (float)Math.Sqrt((double)(num4 * num4 + num5 * num5));
						num7 = (float)num6 / num7;
						num4 *= num7;
						num5 *= num7;
						int num8 = (int)((float)this.damage * 0.5f);
						int num9 = Projectile.NewProjectile(x, y, num4, num5, 92, num8, this.knockBack, this.Owner);
						Main.projectile[num9].ai[1] = this.Position.Y;
						Main.projectile[num9].ai[0] = 1f;
					}
					break;

				case ProjectileType.N92_HALLOW_STAR:
					{
						float x_2 = this.Position.X + (float)Main.rand.Next(-400, 400);
						float y_2 = this.Position.Y - (float)Main.rand.Next(600, 900);
						Vector2 vector_2 = new Vector2(x_2, y_2);
						float num4_2 = this.Position.X + (float)(this.Width / 2) - vector_2.X;
						float num5_2 = this.Position.Y + (float)(this.Height / 2) - vector_2.Y;
						int num6_2 = 22;
						float num7_2 = (float)Math.Sqrt((double)(num4_2 * num4_2 + num5_2 * num5_2));
						num7_2 = (float)num6_2 / num7_2;
						num4_2 *= num7_2;
						num5_2 *= num7_2;
						int num9_2 = Projectile.NewProjectile(x_2, y_2, num4_2, num5_2, 92, this.damage, this.knockBack, this.Owner);
						Main.projectile[num9_2].ai[1] = this.Position.Y;
					}
					break;

				case ProjectileType.N99_BOULDER:
					this.Velocity *= 1.9f;
					break;

				default:
					break;
			}
			if (this.Owner == Main.myPlayer)
			{
				bool explode = false;
				if (this.type == ProjectileType.N28_BOMB ||
					this.type == ProjectileType.N29_DYNAMITE ||
					this.type == ProjectileType.N37_STICKY_BOMB ||
					this.type == ProjectileType.N75_HAPPY_BOMB ||
					this.type == ProjectileType.N108_EXPLOSIVES)
				{
					var ctx = new HookContext
					{
						Sender = this,
					};

					var args = new HookArgs.Explosion
					{
						Source = this,
					};

					HookPoints.Explosion.Invoke(ref ctx, ref args);

					if (ctx.Result != HookResult.IGNORE)
					{
						explode = true;
					}
				}

				if (explode)
				{
					int num38 = 3;
					if (this.type == ProjectileType.N29_DYNAMITE)
					{
						num38 = 7;
					}
					if (this.type == ProjectileType.N108_EXPLOSIVES)
					{
						num38 = 10;
					}
					int num39 = (int)(this.Position.X / 16f - (float)num38);
					int num40 = (int)(this.Position.X / 16f + (float)num38);
					int num41 = (int)(this.Position.Y / 16f - (float)num38);
					int num42 = (int)(this.Position.Y / 16f + (float)num38);
					if (num39 < 0)
					{
						num39 = 0;
					}
					if (num40 > Main.maxTilesX)
					{
						num40 = Main.maxTilesX;
					}
					if (num41 < 0)
					{
						num41 = 0;
					}
					if (num42 > Main.maxTilesY)
					{
						num42 = Main.maxTilesY;
					}
					bool flag = false;
					for (int num43 = num39; num43 <= num40; num43++)
					{
						for (int num44 = num41; num44 <= num42; num44++)
						{
							float num45 = Math.Abs((float)num43 - this.Position.X / 16f);
							float num46 = Math.Abs((float)num44 - this.Position.Y / 16f);
							double num47 = Math.Sqrt((double)(num45 * num45 + num46 * num46));
							if (num47 < (double)num38 && TileRefs(num43, num44).Exists && TileRefs(num43, num44).Wall == 0)
							{
								flag = true;
								break;
							}
						}
					}
					for (int num89 = num39; num89 <= num40; num89++)
					{
						for (int num90 = num41; num90 <= num42; num90++)
						{
							float num50 = Math.Abs((float)num89 - this.Position.X / 16f);
							float num51 = Math.Abs((float)num90 - this.Position.Y / 16f);
							double num52 = Math.Sqrt((double)(num50 * num50 + num51 * num51));
							int alter = -1;
							if (num52 < (double)num38)
							{
								bool flag2 = true;
								if (TileRefs(num89, num90).Exists && TileRefs(num89, num90).Active)
								{
									if (Main.tileDungeon[(int)TileRefs(num89, num90).Type] || TileRefs(num89, num90).Type == 21 ||
									TileRefs(num89, num90).Type == 26 || TileRefs(num89, num90).Type == 107 ||
									TileRefs(num89, num90).Type == 108 || TileRefs(num89, num90).Type == 111)
									{
										flag2 = false;
									}
									if (!Main.hardMode && TileRefs(num89, num90).Type == 58)
									{
										flag2 = false;
									}
									if (flag2)
									{
										alter = 0;
									}
								}
								if (flag2 && TileRefs(num89, num90).Exists && TileRefs(num89, num90).Wall > 0 && flag)
								{
									alter = alter == 0 ? 100 : 2;
								}
								if (alter >= 0)
								{
									var plr = Creator as Player;
									if (plr == null || WorldModify.InvokeAlterationHook(this, plr, num89, num90, (byte)alter))
									{
										if (alter == 0 || alter == 100)
										{
											WorldModify.KillTile(TileRefs, sandbox, num89, num90);
											NetMessage.SendData(17, -1, -1, "", 0, (float)num89, (float)num90, 0f, 0);
										}
										if (alter == 2 || alter == 100)
										{
											WorldModify.KillWall(TileRefs, sandbox, num89, num90);
											NetMessage.SendData(17, -1, -1, "", 2, (float)num89, (float)num90, 0f, 0);
										}
									}
								}
							}
						}
					}
				}

				NetMessage.SendData(29, -1, -1, "", this.identity, (float)this.Owner);

				int num96 = -1;
				if (this.aiStyle == 10)
				{
					int num54 = (int)(this.Position.X + (float)(this.Width / 2)) / 16;
					int num55 = (int)(this.Position.Y + (float)(this.Width / 2)) / 16;
					int num99 = 0;
					int num100 = 2;
					if (this.type == ProjectileType.N31_SAND_BALL)
					{
						num99 = 53;
						num100 = 0;
					}
					else if (this.type == ProjectileType.N42_SAND_BALL)
					{
						num99 = 53;
						num100 = 0;
					}
					else if (this.type == ProjectileType.N39_MUD_BALL)
					{
						num99 = 59;
						num100 = 176;
					}
					else if (this.type == ProjectileType.N40_ASH_BALL)
					{
						num99 = 57;
						num100 = 172;
					}
					else if (this.type == ProjectileType.N56_EBONSAND_BALL)
					{
						num99 = 112;
						num100 = 0;
					}
					else if (this.type == ProjectileType.N65_EBONSAND_BALL)
					{
						num99 = 112;
						num100 = 0;
					}
					else if (this.type == ProjectileType.N67_PEARL_SAND_BALL)
					{
						num99 = 116;
						num100 = 0;
					}
					else if (this.type == ProjectileType.N68_PEARL_SAND_BALL)
					{
						num99 = 116;
						num100 = 0;
					}
					else if (this.type == ProjectileType.N71_GRAVEL_BALL)
					{
						num99 = 123;
						num100 = 0;
					}
					if (!TileRefs(num54, num55).Active)
					{
						WorldModify.PlaceTile(TileRefs, sandbox, num54, num55, num99, false, true, -1, 0);
						if (TileRefs(num54, num55).Active && (int)TileRefs(num54, num55).Type == num99)
						{
							NetMessage.SendData(17, -1, -1, "", 1, (float)num54, (float)num55, (float)num99);
						}
						else
						{
							if (num100 > 0)
							{
								num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, num100, 1, false);
							}
						}
					}
					else
					{
						if (num100 > 0)
						{
							num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, num100, 1, false);
						}
					}
				}
				if (this.type == ProjectileType.N1_WOODEN_ARROW && Main.rand.Next(2) == 0)
				{
					num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 40, 1, false);
				}
				if (this.type == ProjectileType.N103_CURSED_ARROW && Main.rand.Next(6) == 0)
				{
					if (Main.rand.Next(3) == 0)
					{
						num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 545, 1, false);
					}
					else
					{
						num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 40, 1, false);
					}
				}
				if (this.type == ProjectileType.N2_FIRE_ARROW && Main.rand.Next(2) == 0)
				{
					if (Main.rand.Next(3) == 0)
					{
						num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 41, 1, false);
					}
					else
					{
						num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 40, 1, false);
					}
				}
				if (this.type == ProjectileType.N91_HOLY_ARROW && Main.rand.Next(6) == 0)
				{
					num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 516, 1, false);
				}
				if (this.type == ProjectileType.N50_GLOWSTICK && Main.rand.Next(3) == 0)
				{
					num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 282, 1, false);
				}
				if (this.type == ProjectileType.N53_STICKY_GLOWSTICK && Main.rand.Next(3) == 0)
				{
					num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 286, 1, false);
				}
				if (this.type == ProjectileType.N48_THROWING_KNIFE && Main.rand.Next(2) == 0)
				{
					num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 279, 1, false);
				}
				if (this.type == ProjectileType.N54_POISONED_KNIFE && Main.rand.Next(2) == 0)
				{
					num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 287, 1, false);
				}
				if (this.type == ProjectileType.N3_SHURIKEN && Main.rand.Next(2) == 0)
				{
					num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 42, 1, false);
				}
				if (this.type == ProjectileType.N4_UNHOLY_ARROW && Main.rand.Next(2) == 0)
				{
					num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 47, 1, false);
				}
				if (this.type == ProjectileType.N12_FALLING_STAR && this.damage > 100)
				{
					num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 75, 1, false);
				}
				if (this.type == ProjectileType.N21_BONE && Main.rand.Next(2) == 0)
				{
					num96 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 154, 1, false);
				}

				if (this.type == ProjectileType.N69_HOLY_WATER || this.type == ProjectileType.N70_UNHOLY_WATER)
				{
					int num101 = (int)(this.Position.X + (float)(this.Width / 2)) / 16;
					int num102 = (int)(this.Position.Y + (float)(this.Height / 2)) / 16;
					for (int num103 = num101 - 4; num103 <= num101 + 4; num103++)
					{
						for (int num104 = num102 - 4; num104 <= num102 + 4; num104++)
						{
							if (Math.Abs(num103 - num101) + Math.Abs(num104 - num102) < 6)
							{
								if (this.type == ProjectileType.N69_HOLY_WATER)
								{
									switch (TileRefs(num103, num104).Type)
									{
										case 2:
											{
												TileRefs(num103, num104).SetType(109);
												WorldModify.SquareTileFrame(TileRefs, sandbox,num103, num104, true);
												NetMessage.SendTileSquare(-1, num103, num104, 1);
											}
											break;
										case 1:
											{
												TileRefs(num103, num104).SetType(117);
												WorldModify.SquareTileFrame(TileRefs, sandbox,num103, num104, true);
												NetMessage.SendTileSquare(-1, num103, num104, 1);
											}
											break;
										case 53:
											{
												TileRefs(num103, num104).SetType(116);
												WorldModify.SquareTileFrame(TileRefs, sandbox,num103, num104, true);
												NetMessage.SendTileSquare(-1, num103, num104, 1);
											}
											break;
										case 23:
											{
												TileRefs(num103, num104).SetType(109);
												WorldModify.SquareTileFrame(TileRefs, sandbox,num103, num104, true);
												NetMessage.SendTileSquare(-1, num103, num104, 1);
											}
											break;
										case 25:
											{
												TileRefs(num103, num104).SetType(117);
												WorldModify.SquareTileFrame(TileRefs, sandbox,num103, num104, true);
												NetMessage.SendTileSquare(-1, num103, num104, 1);
											}
											break;
										case 112:
											{
												TileRefs(num103, num104).SetType(116);
												WorldModify.SquareTileFrame(TileRefs, sandbox,num103, num104, true);
												NetMessage.SendTileSquare(-1, num103, num104, 1);
											}
											break;

										default:
											break;
									}
								}
								else
								{
									switch (TileRefs(num103, num104).Type)
									{
										case 2:
											{
												TileRefs(num103, num104).SetType(23);
												WorldModify.SquareTileFrame(TileRefs, sandbox,num103, num104, true);
												NetMessage.SendTileSquare(-1, num103, num104, 1);
											}
											break;

										case 1:
											{
												TileRefs(num103, num104).SetType(25);
												WorldModify.SquareTileFrame(TileRefs, sandbox,num103, num104, true);
												NetMessage.SendTileSquare(-1, num103, num104, 1);
											}
											break;

										case 53:
											{
												TileRefs(num103, num104).SetType(112);
												WorldModify.SquareTileFrame(TileRefs, sandbox,num103, num104, true);
												NetMessage.SendTileSquare(-1, num103, num104, 1);
											}
											break;

										case 109:
											{
												TileRefs(num103, num104).SetType(23);
												WorldModify.SquareTileFrame(TileRefs, sandbox,num103, num104, true);
												NetMessage.SendTileSquare(-1, num103, num104, 1);
											}
											break;

										case 117:
											{
												TileRefs(num103, num104).SetType(25);
												WorldModify.SquareTileFrame(TileRefs, sandbox,num103, num104, true);
												NetMessage.SendTileSquare(-1, num103, num104, 1);
											}
											break;

										case 116:
											{
												TileRefs(num103, num104).SetType(112);
												WorldModify.SquareTileFrame(TileRefs, sandbox,num103, num104, true);
												NetMessage.SendTileSquare(-1, num103, num104, 1);
											}
											break;
										default:
											break;
									}

								}
							}
						}
					}
				}

				//if (num96 >= 0)
				// {
				//     NetMessage.SendData(21, -1, -1, "", num96, 0f, 0f, 0f, 0);
				// }
			}
			this.Active = false;
			//Reset(whoAmI);
		}

		static Dictionary<int, int> identityMap = new Dictionary<int, int>();
		static Stack<int> freeSlots = new Stack<int>();

		internal static void ResetProjectiles()
		{
			lock (identityMap)
			{
				identityMap.Clear();
				freeSlots.Clear();

				for (int i = 999; i >= 0; i--)
				{
					freeSlots.Push(i);
					Main.projectile[i] = new Projectile();
					Main.projectile[i].whoAmI = i;
				}
			}
		}

		internal static void Reset(int i)
		{
			var proj = Main.projectile[i];
			int owner = proj.Owner;
			int id = proj.identity;
			var key = (id << 8) | owner;

			lock (identityMap)
			{
				int slot;
				if (identityMap.TryGetValue(key, out slot))
				{
					identityMap.Remove(key);
					freeSlots.Push(i);

					if (slot != i)
						ProgramLog.Error.Log("Mismatch in projectile slot assignment.");
				}
			}
		}

		public void Reset()
		{
			Reset(whoAmI);
		}

		public static void Register(int identity, int owner, int whoAmI)
		{
			lock (identityMap)
			{
				identityMap[(identity << 8) | owner] = whoAmI;
			}
		}

		public static int FindExisting(int identity, int owner)
		{
			int key = (identity << 8) | owner;

			lock (identityMap)
			{
				int index;
				if (identityMap.TryGetValue(key, out index))
				{
					return index;
				}
				return 1000;
			}
		}

		public static int ReserveSlot(int owner)
		{
			lock (identityMap)
			{
				if (freeSlots.Count == 0) return 1000;

				int i = freeSlots.Pop();
				identityMap[(i << 8) | owner] = i;

				return i;

				//				for (int i = 0; i < 1000; i++)
				//				{
				//					if (Main.projectile[i].Active == false)
				//					{
				//						identityMap[(i << 8) | owner] = i;
				//						return i;
				//					}
				//				}
				//				
				//				return 1000;
			}
		}

		public static int ReserveSlot(int identity, int owner)
		{
			lock (identityMap)
			{
				if (freeSlots.Count == 0) return 1000;

				int i = freeSlots.Pop();
				identityMap[(identity << 8) | owner] = i;

				return i;

				//				for (int i = 0; i < 1000; i++)
				//				{
				//					if (Main.projectile[i].Active == false)
				//					{
				//						identityMap[(identity << 8) | owner] = i;
				//						return i;
				//					}
				//				}
				//				
				//				return 1000;
			}
		}

		public static void FreeSlot(int id, int owner, int slot)
		{
			int key = (id << 8) | owner;

			lock (identityMap)
			{
				int index;
				if (identityMap.TryGetValue(key, out index))
				{
					if (index != slot)
						ProgramLog.Error.Log("Mismatch in projectile slot assignment.");

					freeSlots.Push(slot);
				}
				else
				{
					ProgramLog.Error.Log("Double free in projectile slot assignment.");
				}
			}
		}

		// transfer ownership to the server
		public void Repossess()
		{
			int id = identity;
			int slot = whoAmI;
			int oldowner = Owner;

			int key = (id << 8) | oldowner;
			int nkey = (slot << 8) | 255;

			lock (identityMap)
			{
				int index;
				if (identityMap.TryGetValue(key, out index))
				{
					if (index != slot)
						ProgramLog.Error.Log("Mismatch in projectile slot assignment.");

					identity = slot;
					Owner = 255;

					identityMap.Remove(key);
					identityMap[nkey] = slot;
				}
			}
		}
	}
}
