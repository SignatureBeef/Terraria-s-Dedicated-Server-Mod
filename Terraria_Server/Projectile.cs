
using System;
namespace Terraria_Server
{
	public class Projectile
	{
		public bool wet;
		public byte wetCount;
		public bool lavaWet;
		public int whoAmI;
		public static int maxAI = 2;
        public Vector2 position = default(Vector2);
        public Vector2 velocity = default(Vector2);
		public int width;
		public int height;
		public float scale = 1f;
		public float rotation;
		public int type;
		public int alpha;
		public int owner = 255;
		public bool active;
		public string name = "";
		public float[] ai = new float[Projectile.maxAI];
		public int aiStyle;
		public int timeLeft;
		public int soundDelay;
		public int damage;
		public int direction;
		public bool hostile;
		public float knockBack;
		public bool friendly;
		public int penetrate = 1;
		public int identity;
		public float light;
		public bool netUpdate;
		public int restrikeDelay;
		public bool tileCollide;
		public int maxUpdates;
		public int numUpdates;
		public bool ignoreWater;
		public int[] playerImmune = new int[255];
		
        public void SetDefaults(int Type)
		{
			for (int i = 0; i < Projectile.maxAI; i++)
			{
				this.ai[i] = 0f;
			}
			for (int j = 0; j < 255; j++)
			{
				this.playerImmune[j] = 0;
			}
			this.lavaWet = false;
			this.wetCount = 0;
			this.wet = false;
			this.ignoreWater = false;
			this.hostile = false;
			this.netUpdate = false;
			this.numUpdates = 0;
			this.maxUpdates = 0;
			this.identity = 0;
			this.restrikeDelay = 0;
			this.light = 0f;
			this.penetrate = 1;
			this.tileCollide = true;
			this.position = default(Vector2);
            this.velocity = default(Vector2);
			this.aiStyle = 0;
			this.alpha = 0;
			this.type = Type;
			this.active = true;
			this.rotation = 0f;
			this.scale = 1f;
			this.owner = 255;
			this.timeLeft = 3600;
			this.name = "";
			this.friendly = false;
			this.damage = 0;
			this.knockBack = 0f;
			if (this.type == 1)
			{
				this.name = "Wooden Arrow";
				this.width = 10;
				this.height = 10;
				this.aiStyle = 1;
				this.friendly = true;
			}
			else
			{
				if (this.type == 2)
				{
					this.name = "Fire Arrow";
					this.width = 10;
					this.height = 10;
					this.aiStyle = 1;
					this.friendly = true;
					this.light = 1f;
				}
				else
				{
					if (this.type == 3)
					{
						this.name = "Shuriken";
						this.width = 22;
						this.height = 22;
						this.aiStyle = 2;
						this.friendly = true;
						this.penetrate = 4;
					}
					else
					{
						if (this.type == 4)
						{
							this.name = "Unholy Arrow";
							this.width = 10;
							this.height = 10;
							this.aiStyle = 1;
							this.friendly = true;
							this.light = 0.2f;
							this.penetrate = 3;
						}
						else
						{
							if (this.type == 5)
							{
								this.name = "Jester's Arrow";
								this.width = 10;
								this.height = 10;
								this.aiStyle = 1;
								this.friendly = true;
								this.light = 0.4f;
								this.penetrate = -1;
								this.timeLeft = 40;
								this.alpha = 100;
								this.ignoreWater = true;
							}
							else
							{
								if (this.type == 6)
								{
									this.name = "Enchanted Boomerang";
									this.width = 22;
									this.height = 22;
									this.aiStyle = 3;
									this.friendly = true;
									this.penetrate = -1;
								}
								else
								{
									if (this.type == 7 || this.type == 8)
									{
										this.name = "Vilethorn";
										this.width = 28;
										this.height = 28;
										this.aiStyle = 4;
										this.friendly = true;
										this.penetrate = -1;
										this.tileCollide = false;
										this.alpha = 255;
										this.ignoreWater = true;
									}
									else
									{
										if (this.type == 9)
										{
											this.name = "Starfury";
											this.width = 24;
											this.height = 24;
											this.aiStyle = 5;
											this.friendly = true;
											this.penetrate = 2;
											this.alpha = 50;
											this.scale = 0.8f;
											this.light = 1f;
										}
										else
										{
											if (this.type == 10)
											{
												this.name = "Purification Powder";
												this.width = 64;
												this.height = 64;
												this.aiStyle = 6;
												this.friendly = true;
												this.tileCollide = false;
												this.penetrate = -1;
												this.alpha = 255;
												this.ignoreWater = true;
											}
											else
											{
												if (this.type == 11)
												{
													this.name = "Vile Powder";
													this.width = 48;
													this.height = 48;
													this.aiStyle = 6;
													this.friendly = true;
													this.tileCollide = false;
													this.penetrate = -1;
													this.alpha = 255;
													this.ignoreWater = true;
												}
												else
												{
													if (this.type == 12)
													{
														this.name = "Fallen Star";
														this.width = 16;
														this.height = 16;
														this.aiStyle = 5;
														this.friendly = true;
														this.penetrate = -1;
														this.alpha = 50;
														this.light = 1f;
													}
													else
													{
														if (this.type == 13)
														{
															this.name = "Hook";
															this.width = 18;
															this.height = 18;
															this.aiStyle = 7;
															this.friendly = true;
															this.penetrate = -1;
															this.tileCollide = false;
														}
														else
														{
															if (this.type == 14)
															{
																this.name = "Musket Ball";
																this.width = 4;
																this.height = 4;
																this.aiStyle = 1;
																this.friendly = true;
																this.penetrate = 1;
																this.light = 0.5f;
																this.alpha = 255;
																this.maxUpdates = 1;
																this.scale = 1.2f;
																this.timeLeft = 600;
															}
															else
															{
																if (this.type == 15)
																{
																	this.name = "Ball of Fire";
																	this.width = 16;
																	this.height = 16;
																	this.aiStyle = 8;
																	this.friendly = true;
																	this.light = 0.8f;
																	this.alpha = 100;
																}
																else
																{
																	if (this.type == 16)
																	{
																		this.name = "Magic Missile";
																		this.width = 10;
																		this.height = 10;
																		this.aiStyle = 9;
																		this.friendly = true;
																		this.light = 0.8f;
																		this.alpha = 100;
																	}
																	else
																	{
																		if (this.type == 17)
																		{
																			this.name = "Dirt Ball";
																			this.width = 10;
																			this.height = 10;
																			this.aiStyle = 10;
																			this.friendly = true;
																		}
																		else
																		{
																			if (this.type == 18)
																			{
																				this.name = "Orb of Light";
																				this.width = 32;
																				this.height = 32;
																				this.aiStyle = 11;
																				this.friendly = true;
																				this.light = 1f;
																				this.alpha = 150;
																				this.tileCollide = false;
																				this.penetrate = -1;
																				this.timeLeft *= 5;
																				this.ignoreWater = true;
																			}
																			else
																			{
																				if (this.type == 19)
																				{
																					this.name = "Flamarang";
																					this.width = 22;
																					this.height = 22;
																					this.aiStyle = 3;
																					this.friendly = true;
																					this.penetrate = -1;
																					this.light = 1f;
																				}
																				else
																				{
																					if (this.type == 20)
																					{
																						this.name = "Green Laser";
																						this.width = 4;
																						this.height = 4;
																						this.aiStyle = 1;
																						this.friendly = true;
																						this.penetrate = -1;
																						this.light = 0.75f;
																						this.alpha = 255;
																						this.maxUpdates = 2;
																						this.scale = 1.4f;
																						this.timeLeft = 600;
																					}
																					else
																					{
																						if (this.type == 21)
																						{
																							this.name = "Bone";
																							this.width = 16;
																							this.height = 16;
																							this.aiStyle = 2;
																							this.scale = 1.2f;
																							this.friendly = true;
																						}
																						else
																						{
																							if (this.type == 22)
																							{
																								this.name = "Water Stream";
																								this.width = 12;
																								this.height = 12;
																								this.aiStyle = 12;
																								this.friendly = true;
																								this.alpha = 255;
																								this.penetrate = -1;
																								this.maxUpdates = 1;
																								this.ignoreWater = true;
																							}
																							else
																							{
																								if (this.type == 23)
																								{
																									this.name = "Harpoon";
																									this.width = 4;
																									this.height = 4;
																									this.aiStyle = 13;
																									this.friendly = true;
																									this.penetrate = -1;
																									this.alpha = 255;
																								}
																								else
																								{
																									if (this.type == 24)
																									{
																										this.name = "Spiky Ball";
																										this.width = 14;
																										this.height = 14;
																										this.aiStyle = 14;
																										this.friendly = true;
																										this.penetrate = 3;
																									}
																									else
																									{
																										if (this.type == 25)
																										{
																											this.name = "Ball 'O Hurt";
																											this.width = 22;
																											this.height = 22;
																											this.aiStyle = 15;
																											this.friendly = true;
																											this.penetrate = -1;
																										}
																										else
																										{
																											if (this.type == 26)
																											{
																												this.name = "Blue Moon";
																												this.width = 22;
																												this.height = 22;
																												this.aiStyle = 15;
																												this.friendly = true;
																												this.penetrate = -1;
																											}
																											else
																											{
																												if (this.type == 27)
																												{
																													this.name = "Water Bolt";
																													this.width = 16;
																													this.height = 16;
																													this.aiStyle = 8;
																													this.friendly = true;
																													this.light = 0.8f;
																													this.alpha = 200;
																													this.timeLeft /= 2;
																													this.penetrate = 10;
																												}
																												else
																												{
																													if (this.type == 28)
																													{
																														this.name = "Bomb";
																														this.width = 22;
																														this.height = 22;
																														this.aiStyle = 16;
																														this.friendly = true;
																														this.penetrate = -1;
																													}
																													else
																													{
																														if (this.type == 29)
																														{
																															this.name = "Dynamite";
																															this.width = 10;
																															this.height = 10;
																															this.aiStyle = 16;
																															this.friendly = true;
																															this.penetrate = -1;
																														}
																														else
																														{
																															if (this.type == 30)
																															{
																																this.name = "Grenade";
																																this.width = 14;
																																this.height = 14;
																																this.aiStyle = 16;
																																this.friendly = true;
																																this.penetrate = -1;
																															}
																															else
																															{
																																if (this.type == 31)
																																{
																																	this.name = "Sand Ball";
																																	this.knockBack = 6f;
																																	this.width = 10;
																																	this.height = 10;
																																	this.aiStyle = 10;
																																	this.friendly = true;
																																	this.hostile = true;
																																	this.penetrate = -1;
																																}
																																else
																																{
																																	if (this.type == 32)
																																	{
																																		this.name = "Ivy Whip";
																																		this.width = 18;
																																		this.height = 18;
																																		this.aiStyle = 7;
																																		this.friendly = true;
																																		this.penetrate = -1;
																																		this.tileCollide = false;
																																	}
																																	else
																																	{
																																		if (this.type == 33)
																																		{
																																			this.name = "Thorn Chakrum";
																																			this.width = 28;
																																			this.height = 28;
																																			this.aiStyle = 3;
																																			this.friendly = true;
																																			this.scale = 0.9f;
																																			this.penetrate = -1;
																																		}
																																		else
																																		{
																																			if (this.type == 34)
																																			{
																																				this.name = "Flamelash";
																																				this.width = 14;
																																				this.height = 14;
																																				this.aiStyle = 9;
																																				this.friendly = true;
																																				this.light = 0.8f;
																																				this.alpha = 100;
																																				this.penetrate = 2;
																																			}
																																			else
																																			{
																																				if (this.type == 35)
																																				{
																																					this.name = "Sunfury";
																																					this.width = 22;
																																					this.height = 22;
																																					this.aiStyle = 15;
																																					this.friendly = true;
																																					this.penetrate = -1;
																																				}
																																				else
																																				{
																																					if (this.type == 36)
																																					{
																																						this.name = "Meteor Shot";
																																						this.width = 4;
																																						this.height = 4;
																																						this.aiStyle = 1;
																																						this.friendly = true;
																																						this.penetrate = 2;
																																						this.light = 0.6f;
																																						this.alpha = 255;
																																						this.maxUpdates = 1;
																																						this.scale = 1.4f;
																																						this.timeLeft = 600;
																																					}
																																					else
																																					{
																																						if (this.type == 37)
																																						{
																																							this.name = "Bomb";
																																							this.width = 22;
																																							this.height = 22;
																																							this.aiStyle = 16;
																																							this.friendly = true;
																																							this.penetrate = -1;
																																							this.tileCollide = false;
																																						}
																																						else
																																						{
																																							this.active = false;
																																						}
																																					}
																																				}
																																			}
																																		}
																																	}
																																}
																															}
																														}
																													}
																												}
																											}
																										}
																									}
																								}
																							}
																						}
																					}
																				}
																			}
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			this.width = (int)((float)this.width * this.scale);
			this.height = (int)((float)this.height * this.scale);
		}
		
        public static int NewProjectile(float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner = 255)
		{
			int num = 1000;
			for (int i = 0; i < 1000; i++)
			{
				if (!Main.projectile[i].active)
				{
					num = i;
					break;
				}
			}
			if (num == 1000)
			{
				return num;
			}
			Main.projectile[num].SetDefaults(Type);
			Main.projectile[num].position.X = X - (float)Main.projectile[num].width * 0.5f;
			Main.projectile[num].position.Y = Y - (float)Main.projectile[num].height * 0.5f;
			Main.projectile[num].owner = Owner;
			Main.projectile[num].velocity.X = SpeedX;
			Main.projectile[num].velocity.Y = SpeedY;
			Main.projectile[num].damage = Damage;
			Main.projectile[num].knockBack = KnockBack;
			Main.projectile[num].identity = num;
			Main.projectile[num].wet = Collision.WetCollision(Main.projectile[num].position, Main.projectile[num].width, Main.projectile[num].height);
			if (Main.netMode != 0 && Owner == Main.myPlayer)
			{
				NetMessage.SendData(27, -1, -1, "", num, 0f, 0f, 0f);
			}
			if (Owner == Main.myPlayer)
			{
				if (Type == 28)
				{
					Main.projectile[num].timeLeft = 180;
				}
				if (Type == 29)
				{
					Main.projectile[num].timeLeft = 300;
				}
				if (Type == 30)
				{
					Main.projectile[num].timeLeft = 180;
				}
				if (Type == 37)
				{
					Main.projectile[num].timeLeft = 180;
				}
			}
			return num;
		}
		
        public void Update(int i)
		{
			if (this.active)
			{
				Vector2 value = this.velocity;
				if (this.position.X <= Main.leftWorld || this.position.X + (float)this.width >= Main.rightWorld || this.position.Y <= Main.topWorld || this.position.Y + (float)this.height >= Main.bottomWorld)
				{
					this.active = false;
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
				this.AI();
				if (this.owner < 255 && !Main.player[this.owner].active)
				{
					this.Kill();
				}
				if (!this.ignoreWater)
				{
					bool flag;
					bool flag2;
					try
					{
						flag = Collision.LavaCollision(this.position, this.width, this.height);
						flag2 = Collision.WetCollision(this.position, this.width, this.height);
						if (flag)
						{
							this.lavaWet = true;
						}
					}
					catch
					{
						this.active = false;
						return;
					}
					if (flag2)
					{
						if (this.wetCount == 0)
						{
							this.wetCount = 10;
							if (!this.wet)
							{
								if (!flag)
								{
									for (int k = 0; k < 10; k++)
									{
										int num = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 33, 0f, 0f, 0,  new Color(), 1f);
										Dust expr_1E9_cp_0 = Main.dust[num];
										expr_1E9_cp_0.velocity.Y = expr_1E9_cp_0.velocity.Y - 4f;
										Dust expr_207_cp_0 = Main.dust[num];
										expr_207_cp_0.velocity.X = expr_207_cp_0.velocity.X * 2.5f;
										Main.dust[num].scale = 1.3f;
										Main.dust[num].alpha = 100;
										Main.dust[num].noGravity = true;
									}
									//Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
								}
								else
								{
									for (int l = 0; l < 10; l++)
									{
										int num2 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 35, 0f, 0f, 0,  new Color(), 1f);
										Dust expr_2EF_cp_0 = Main.dust[num2];
										expr_2EF_cp_0.velocity.Y = expr_2EF_cp_0.velocity.Y - 1.5f;
										Dust expr_30D_cp_0 = Main.dust[num2];
										expr_30D_cp_0.velocity.X = expr_30D_cp_0.velocity.X * 2.5f;
										Main.dust[num2].scale = 1.3f;
										Main.dust[num2].alpha = 100;
										Main.dust[num2].noGravity = true;
									}
									//Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
								}
							}
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
								if (!this.lavaWet)
								{
									for (int m = 0; m < 10; m++)
									{
										int num3 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2)), this.width + 12, 24, 33, 0f, 0f, 0,  new Color(), 1f);
										Dust expr_426_cp_0 = Main.dust[num3];
										expr_426_cp_0.velocity.Y = expr_426_cp_0.velocity.Y - 4f;
										Dust expr_444_cp_0 = Main.dust[num3];
										expr_444_cp_0.velocity.X = expr_444_cp_0.velocity.X * 2.5f;
										Main.dust[num3].scale = 1.3f;
										Main.dust[num3].alpha = 100;
										Main.dust[num3].noGravity = true;
									}
									//Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
								}
								else
								{
									for (int n = 0; n < 10; n++)
									{
										int num4 = Dust.NewDust(new Vector2(this.position.X - 6f, this.position.Y + (float)(this.height / 2) - 8f), this.width + 12, 24, 35, 0f, 0f, 0,  new Color(), 1f);
										Dust expr_52C_cp_0 = Main.dust[num4];
										expr_52C_cp_0.velocity.Y = expr_52C_cp_0.velocity.Y - 1.5f;
										Dust expr_54A_cp_0 = Main.dust[num4];
										expr_54A_cp_0.velocity.X = expr_54A_cp_0.velocity.X * 2.5f;
										Main.dust[num4].scale = 1.3f;
										Main.dust[num4].alpha = 100;
										Main.dust[num4].noGravity = true;
									}
									//Main.PlaySound(19, (int)this.position.X, (int)this.position.Y, 1);
								}
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
				if (this.tileCollide)
				{
					Vector2 value2 = this.velocity;
					bool flag3 = true;
					if (this.type == 9 || this.type == 12 || this.type == 15 || this.type == 13 || this.type == 31)
					{
						flag3 = false;
					}
					if (this.aiStyle == 10)
					{
						this.velocity = Collision.AnyCollision(this.position, this.velocity, this.width, this.height);
					}
					else
					{
						if (this.wet)
						{
							Vector2 vector = this.velocity;
							this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, flag3, flag3);
							value = this.velocity * 0.5f;
							if (this.velocity.X != vector.X)
							{
								value.X = this.velocity.X;
							}
							if (this.velocity.Y != vector.Y)
							{
								value.Y = this.velocity.Y;
							}
						}
						else
						{
							this.velocity = Collision.TileCollision(this.position, this.velocity, this.width, this.height, flag3, flag3);
						}
					}
					if (value2 != this.velocity)
					{
						if (this.type == 36)
						{
							if (this.penetrate > 1)
							{
								Collision.HitTiles(this.position, this.velocity, this.width, this.height);
								//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
								this.penetrate--;
								if (this.velocity.X != value2.X)
								{
									this.velocity.X = -value2.X;
								}
								if (this.velocity.Y != value2.Y)
								{
									this.velocity.Y = -value2.Y;
								}
							}
							else
							{
								this.Kill();
							}
						}
						else
						{
							if (this.aiStyle == 3 || this.aiStyle == 13 || this.aiStyle == 15)
							{
								Collision.HitTiles(this.position, this.velocity, this.width, this.height);
								if (this.type == 33)
								{
									if (this.velocity.X != value2.X)
									{
										this.velocity.X = -value2.X;
									}
									if (this.velocity.Y != value2.Y)
									{
										this.velocity.Y = -value2.Y;
									}
								}
								else
								{
									this.ai[0] = 1f;
									if (this.aiStyle == 3)
									{
										this.velocity.X = -value2.X;
										this.velocity.Y = -value2.Y;
									}
								}
								this.netUpdate = true;
								//Main.PlaySound(0, (int)this.position.X, (int)this.position.Y, 1);
							}
							else
							{
								if (this.aiStyle == 8)
								{
									//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
									this.ai[0] += 1f;
									if (this.ai[0] >= 5f)
									{
										this.position += this.velocity;
										this.Kill();
									}
									else
									{
										if (this.velocity.Y != value2.Y)
										{
											this.velocity.Y = -value2.Y;
										}
										if (this.velocity.X != value2.X)
										{
											this.velocity.X = -value2.X;
										}
									}
								}
								else
								{
									if (this.aiStyle == 14)
									{
										if (this.velocity.X != value2.X)
										{
											this.velocity.X = value2.X * -0.5f;
										}
										if (this.velocity.Y != value2.Y && value2.Y > 1f)
										{
											this.velocity.Y = value2.Y * -0.5f;
										}
									}
									else
									{
										if (this.aiStyle == 16)
										{
											if (this.velocity.X != value2.X)
											{
												this.velocity.X = value2.X * -0.4f;
												if (this.type == 29)
												{
													this.velocity.X = this.velocity.X * 0.8f;
												}
											}
											if (this.velocity.Y != value2.Y && (double)value2.Y > 0.7)
											{
												this.velocity.Y = value2.Y * -0.4f;
												if (this.type == 29)
												{
													this.velocity.Y = this.velocity.Y * 0.8f;
												}
											}
										}
										else
										{
											this.position += this.velocity;
											this.Kill();
										}
									}
								}
							}
						}
					}
				}
				if (this.type == 7 || this.type == 8)
				{
					goto IL_B65;
				}
				if (this.wet)
				{
					this.position += value;
					goto IL_B65;
				}
				this.position += this.velocity;
				goto IL_B65;
				IL_B65:
				if ((this.aiStyle != 3 || this.ai[0] != 1f) && (this.aiStyle != 7 || this.ai[0] != 1f) && (this.aiStyle != 13 || this.ai[0] != 1f) && (this.aiStyle != 15 || this.ai[0] != 1f))
				{
					if (this.velocity.X < 0f)
					{
						this.direction = -1;
					}
					else
					{
						this.direction = 1;
					}
				}
				if (!this.active)
				{
					return;
				}
				if (this.light > 0f)
				{
					//Lighting.addLight((int)((this.position.X + (float)(this.width / 2)) / 16f), (int)((this.position.Y + (float)(this.height / 2)) / 16f), this.light);
				}
				if (this.type == 2)
				{
					Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, 0f, 0f, 100,  new Color(), 1f);
				}
				else
				{
					if (this.type == 4)
					{
						if (Main.rand.Next(5) == 0)
						{
							Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 14, 0f, 0f, 150,  new Color(), 1.1f);
						}
					}
					else
					{
						if (this.type == 5)
						{
							Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X * 0.5f, this.velocity.Y * 0.5f, 150,  new Color(), 1.2f);
						}
					}
				}
				Rectangle rectangle = new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height);
				if (this.hostile && Main.myPlayer < 255 && this.damage > 0)
				{
					int myPlayer = Main.myPlayer;
					if (Main.player[myPlayer].active && !Main.player[myPlayer].dead && !Main.player[myPlayer].immune)
					{
						Rectangle value3 = new Rectangle((int)Main.player[myPlayer].position.X, (int)Main.player[myPlayer].position.Y, Main.player[myPlayer].width, Main.player[myPlayer].height);
						if (rectangle.Intersects(value3))
						{
							int hitDirection = this.direction;
							if (Main.player[myPlayer].position.X + (float)(Main.player[myPlayer].width / 2) < this.position.X + (float)(this.width / 2))
							{
								hitDirection = -1;
							}
							else
							{
								hitDirection = 1;
							}
							Main.player[myPlayer].Hurt(this.damage * 2, hitDirection, false, false);
							if (Main.netMode != 0)
							{
								NetMessage.SendData(26, -1, -1, "", myPlayer, (float)this.direction, (float)(this.damage * 2), 0f);
							}
						}
					}
				}
				if (this.friendly && this.type != 18 && this.owner == Main.myPlayer)
				{
					if (this.aiStyle == 16 && this.ai[1] > 0f)
					{
						for (int num5 = 0; num5 < 255; num5++)
						{
							if (Main.player[num5].active && !Main.player[num5].dead && !Main.player[num5].immune)
							{
								Rectangle value4 = new Rectangle((int)Main.player[num5].position.X, (int)Main.player[num5].position.Y, Main.player[num5].width, Main.player[num5].height);
								if (rectangle.Intersects(value4))
								{
									if (Main.player[num5].position.X + (float)(Main.player[num5].width / 2) < this.position.X + (float)(this.width / 2))
									{
										this.direction = -1;
									}
									else
									{
										this.direction = 1;
									}
									Main.player[num5].Hurt(this.damage, this.direction, true, false);
									if (Main.netMode != 0)
									{
										NetMessage.SendData(26, -1, -1, "", num5, (float)this.direction, (float)this.damage, 1f);
									}
								}
							}
						}
					}
					int num6 = (int)(this.position.X / 16f);
					int num7 = (int)((this.position.X + (float)this.width) / 16f) + 1;
					int num8 = (int)(this.position.Y / 16f);
					int num9 = (int)((this.position.Y + (float)this.height) / 16f) + 1;
					if (num6 < 0)
					{
						num6 = 0;
					}
					if (num7 > Main.maxTilesX)
					{
						num7 = Main.maxTilesX;
					}
					if (num8 < 0)
					{
						num8 = 0;
					}
					if (num9 > Main.maxTilesY)
					{
						num9 = Main.maxTilesY;
					}
					for (int num10 = num6; num10 < num7; num10++)
					{
						for (int num11 = num8; num11 < num9; num11++)
						{
							if (Main.tile[num10, num11] != null && (Main.tile[num10, num11].type == 3 || Main.tile[num10, num11].type == 24 || Main.tile[num10, num11].type == 28 || Main.tile[num10, num11].type == 32 || Main.tile[num10, num11].type == 51 || Main.tile[num10, num11].type == 52 || Main.tile[num10, num11].type == 61 || Main.tile[num10, num11].type == 62 || Main.tile[num10, num11].type == 69 || Main.tile[num10, num11].type == 71 || Main.tile[num10, num11].type == 73 || Main.tile[num10, num11].type == 74))
							{
								WorldGen.KillTile(num10, num11, false, false, false);
								if (Main.netMode == 1)
								{
									NetMessage.SendData(17, -1, -1, "", 0, (float)num10, (float)num11, 0f);
								}
							}
						}
					}
					if (this.damage > 0)
					{
						for (int num12 = 0; num12 < 1000; num12++)
						{
							if (Main.npc[num12].active && !Main.npc[num12].friendly && (this.owner < 0 || Main.npc[num12].immune[this.owner] == 0))
							{
								Rectangle value5 = new Rectangle((int)Main.npc[num12].position.X, (int)Main.npc[num12].position.Y, Main.npc[num12].width, Main.npc[num12].height);
								if (rectangle.Intersects(value5))
								{
									if (this.aiStyle == 3)
									{
										if (this.ai[0] == 0f)
										{
											this.velocity.X = -this.velocity.X;
											this.velocity.Y = -this.velocity.Y;
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
											if (Main.npc[num12].position.X + (float)(Main.npc[num12].width / 2) < this.position.X + (float)(this.width / 2))
											{
												this.direction = -1;
											}
											else
											{
												this.direction = 1;
											}
										}
									}
									Main.npc[num12].StrikeNPC(this.damage, this.knockBack, this.direction);
									if (Main.netMode != 0)
									{
										NetMessage.SendData(28, -1, -1, "", num12, (float)this.damage, this.knockBack, (float)this.direction);
									}
									if (this.penetrate != 1)
									{
										Main.npc[num12].immune[this.owner] = 10;
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
					if (this.damage > 0 && Main.player[Main.myPlayer].hostile)
					{
						for (int num13 = 0; num13 < 255; num13++)
						{
							if (num13 != this.owner && Main.player[num13].active && !Main.player[num13].dead && !Main.player[num13].immune && Main.player[num13].hostile && this.playerImmune[num13] <= 0 && (Main.player[Main.myPlayer].team == 0 || Main.player[Main.myPlayer].team != Main.player[num13].team))
							{
								Rectangle value6 = new Rectangle((int)Main.player[num13].position.X, (int)Main.player[num13].position.Y, Main.player[num13].width, Main.player[num13].height);
								if (rectangle.Intersects(value6))
								{
									if (this.aiStyle == 3)
									{
										if (this.ai[0] == 0f)
										{
											this.velocity.X = -this.velocity.X;
											this.velocity.Y = -this.velocity.Y;
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
											if (Main.player[num13].position.X + (float)(Main.player[num13].width / 2) < this.position.X + (float)(this.width / 2))
											{
												this.direction = -1;
											}
											else
											{
												this.direction = 1;
											}
										}
									}
									Main.player[num13].Hurt(this.damage, this.direction, true, false);
									if (Main.netMode != 0)
									{
										NetMessage.SendData(26, -1, -1, "", num13, (float)this.direction, (float)this.damage, 1f);
									}
									this.playerImmune[num13] = 40;
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
				this.timeLeft--;
				if (this.timeLeft <= 0)
				{
					this.Kill();
				}
				if (this.penetrate == 0)
				{
					this.Kill();
				}
				if (this.active && this.netUpdate && this.owner == Main.myPlayer)
				{
					NetMessage.SendData(27, -1, -1, "", i, 0f, 0f, 0f);
				}
				if (this.active && this.maxUpdates > 0)
				{
					this.numUpdates--;
					if (this.numUpdates >= 0)
					{
						this.Update(i);
					}
					else
					{
						this.numUpdates = this.maxUpdates;
					}
				}
				this.netUpdate = false;
			}
		}
		
        public void AI()
		{
			if (this.aiStyle == 1)
			{
				if (this.type == 20 || this.type == 14 || this.type == 36)
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
				if (this.type != 5 && this.type != 14 && this.type != 20 && this.type != 36)
				{
					this.ai[0] += 1f;
				}
				if (this.ai[0] >= 15f)
				{
					this.ai[0] = 15f;
					this.velocity.Y = this.velocity.Y + 0.1f;
				}
				this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) + 1.57f;
				if (this.velocity.Y > 16f)
				{
					this.velocity.Y = 16f;
					return;
				}
			}
			else
			{
				if (this.aiStyle == 2)
				{
					this.ai[0] += 1f;
					if (this.ai[0] >= 20f)
					{
						this.velocity.Y = this.velocity.Y + 0.4f;
						this.velocity.X = this.velocity.X * 0.97f;
					}
					this.rotation += (Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) * 0.03f * (float)this.direction;
					if (this.velocity.Y > 16f)
					{
						this.velocity.Y = 16f;
						return;
					}
				}
				else
				{
					if (this.aiStyle == 3)
					{
						if (this.soundDelay == 0)
						{
							this.soundDelay = 8;
							//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 7);
						}
						if (this.type == 19)
						{
							for (int i = 0; i < 2; i++)
							{
								Vector2 arg_28A_0 = new Vector2(this.position.X, this.position.Y);
								int arg_28A_1 = this.width;
								int arg_28A_2 = this.height;
								int arg_28A_3 = 6;
								float arg_28A_4 = this.velocity.X * 0.2f;
								float arg_28A_5 = this.velocity.Y * 0.2f;
								int arg_28A_6 = 100;
								Color newColor =  new Color();
								int num = Dust.NewDust(arg_28A_0, arg_28A_1, arg_28A_2, arg_28A_3, arg_28A_4, arg_28A_5, arg_28A_6, newColor, 2f);
								Main.dust[num].noGravity = true;
								Dust expr_2A9_cp_0 = Main.dust[num];
								expr_2A9_cp_0.velocity.X = expr_2A9_cp_0.velocity.X * 0.3f;
								Dust expr_2C6_cp_0 = Main.dust[num];
								expr_2C6_cp_0.velocity.Y = expr_2C6_cp_0.velocity.Y * 0.3f;
							}
						}
						else
						{
							if (this.type == 33)
							{
								if (Main.rand.Next(1) == 0)
								{
									Vector2 arg_347_0 = this.position;
									int arg_347_1 = this.width;
									int arg_347_2 = this.height;
									int arg_347_3 = 40;
									float arg_347_4 = this.velocity.X * 0.25f;
									float arg_347_5 = this.velocity.Y * 0.25f;
									int arg_347_6 = 0;
									Color newColor =  new Color();
									int num2 = Dust.NewDust(arg_347_0, arg_347_1, arg_347_2, arg_347_3, arg_347_4, arg_347_5, arg_347_6, newColor, 1.4f);
									Main.dust[num2].noGravity = true;
								}
							}
							else
							{
								if (Main.rand.Next(5) == 0)
								{
									Vector2 arg_3B3_0 = this.position;
									int arg_3B3_1 = this.width;
									int arg_3B3_2 = this.height;
									int arg_3B3_3 = 15;
									float arg_3B3_4 = this.velocity.X * 0.5f;
									float arg_3B3_5 = this.velocity.Y * 0.5f;
									int arg_3B3_6 = 150;
									Color newColor =  new Color();
									Dust.NewDust(arg_3B3_0, arg_3B3_1, arg_3B3_2, arg_3B3_3, arg_3B3_4, arg_3B3_5, arg_3B3_6, newColor, 0.9f);
								}
							}
						}
						if (this.ai[0] == 0f)
						{
							this.ai[1] += 1f;
							if (this.ai[1] >= 30f)
							{
								this.ai[0] = 1f;
								this.ai[1] = 0f;
								this.netUpdate = true;
							}
						}
						else
						{
							this.tileCollide = false;
							float num3 = 9f;
							float num4 = 0.4f;
							if (this.type == 19)
							{
								num3 = 13f;
								num4 = 0.6f;
							}
							else
							{
								if (this.type == 33)
								{
									num3 = 15f;
									num4 = 0.8f;
								}
							}
							Vector2 vector = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
							float num5 = Main.player[this.owner].position.X + (float)(Main.player[this.owner].width / 2) - vector.X;
							float num6 = Main.player[this.owner].position.Y + (float)(Main.player[this.owner].height / 2) - vector.Y;
							float num7 = (float)Math.Sqrt((double)(num5 * num5 + num6 * num6));
							num7 = num3 / num7;
							num5 *= num7;
							num6 *= num7;
							if (this.velocity.X < num5)
							{
								this.velocity.X = this.velocity.X + num4;
								if (this.velocity.X < 0f && num5 > 0f)
								{
									this.velocity.X = this.velocity.X + num4;
								}
							}
							else
							{
								if (this.velocity.X > num5)
								{
									this.velocity.X = this.velocity.X - num4;
									if (this.velocity.X > 0f && num5 < 0f)
									{
										this.velocity.X = this.velocity.X - num4;
									}
								}
							}
							if (this.velocity.Y < num6)
							{
								this.velocity.Y = this.velocity.Y + num4;
								if (this.velocity.Y < 0f && num6 > 0f)
								{
									this.velocity.Y = this.velocity.Y + num4;
								}
							}
							else
							{
								if (this.velocity.Y > num6)
								{
									this.velocity.Y = this.velocity.Y - num4;
									if (this.velocity.Y > 0f && num6 < 0f)
									{
										this.velocity.Y = this.velocity.Y - num4;
									}
								}
							}
							if (Main.myPlayer == this.owner)
							{
								Rectangle rectangle = new Rectangle((int)this.position.X, (int)this.position.Y, this.width, this.height);
								Rectangle value = new Rectangle((int)Main.player[this.owner].position.X, (int)Main.player[this.owner].position.Y, Main.player[this.owner].width, Main.player[this.owner].height);
								if (rectangle.Intersects(value))
								{
									this.Kill();
								}
							}
						}
						this.rotation += 0.4f * (float)this.direction;
						return;
					}
					if (this.aiStyle == 4)
					{
						this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) + 1.57f;
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
									this.position += this.velocity * 1f;
								}
								if (this.type == 7 && Main.myPlayer == this.owner)
								{
									int num8 = this.type;
									if (this.ai[1] >= 6f)
									{
										num8++;
									}
									int num9 = Projectile.NewProjectile(this.position.X + this.velocity.X + (float)(this.width / 2), this.position.Y + this.velocity.Y + (float)(this.height / 2), this.velocity.X, this.velocity.Y, num8, this.damage, this.knockBack, this.owner);
									Main.projectile[num9].damage = this.damage;
									Main.projectile[num9].ai[1] = this.ai[1] + 1f;
									NetMessage.SendData(27, -1, -1, "", num9, 0f, 0f, 0f);
									return;
								}
							}
						}
						else
						{
							if (this.alpha < 170 && this.alpha + 5 >= 170)
							{
								Color newColor;
								for (int j = 0; j < 3; j++)
								{
									Vector2 arg_967_0 = this.position;
									int arg_967_1 = this.width;
									int arg_967_2 = this.height;
									int arg_967_3 = 18;
									float arg_967_4 = this.velocity.X * 0.025f;
									float arg_967_5 = this.velocity.Y * 0.025f;
									int arg_967_6 = 170;
									newColor =  new Color();
									Dust.NewDust(arg_967_0, arg_967_1, arg_967_2, arg_967_3, arg_967_4, arg_967_5, arg_967_6, newColor, 1.2f);
								}
								Vector2 arg_9AA_0 = this.position;
								int arg_9AA_1 = this.width;
								int arg_9AA_2 = this.height;
								int arg_9AA_3 = 14;
								float arg_9AA_4 = 0f;
								float arg_9AA_5 = 0f;
								int arg_9AA_6 = 170;
								newColor =  new Color();
								Dust.NewDust(arg_9AA_0, arg_9AA_1, arg_9AA_2, arg_9AA_3, arg_9AA_4, arg_9AA_5, arg_9AA_6, newColor, 1.1f);
							}
							this.alpha += 5;
							if (this.alpha >= 255)
							{
								this.Kill();
								return;
							}
						}
					}
					else
					{
						if (this.aiStyle == 5)
						{
							if (this.soundDelay == 0)
							{
								this.soundDelay = 20 + Main.rand.Next(40);
								//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 9);
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
							this.rotation += (Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y)) * 0.01f * (float)this.direction;
							if (Main.rand.Next(10) == 0)
							{
								Vector2 arg_B2C_0 = this.position;
								int arg_B2C_1 = this.width;
								int arg_B2C_2 = this.height;
								int arg_B2C_3 = 15;
								float arg_B2C_4 = this.velocity.X * 0.5f;
								float arg_B2C_5 = this.velocity.Y * 0.5f;
								int arg_B2C_6 = 150;
								Color newColor =  new Color();
								Dust.NewDust(arg_B2C_0, arg_B2C_1, arg_B2C_2, arg_B2C_3, arg_B2C_4, arg_B2C_5, arg_B2C_6, newColor, 1.2f);
							}
							if (Main.rand.Next(20) == 0)
							{
								Gore.NewGore(this.position, new Vector2(this.velocity.X * 0.2f, this.velocity.Y * 0.2f), Main.rand.Next(16, 18));
								return;
							}
						}
						else
						{
							if (this.aiStyle == 6)
							{
								this.velocity *= 0.95f;
								this.ai[0] += 1f;
								if (this.ai[0] == 180f)
								{
									this.Kill();
								}
								if (this.ai[1] == 0f)
								{
									this.ai[1] = 1f;
									for (int k = 0; k < 30; k++)
									{
										Vector2 arg_C3C_0 = this.position;
										int arg_C3C_1 = this.width;
										int arg_C3C_2 = this.height;
										int arg_C3C_3 = 10 + this.type;
										float arg_C3C_4 = this.velocity.X;
										float arg_C3C_5 = this.velocity.Y;
										int arg_C3C_6 = 50;
										Color newColor =  new Color();
										Dust.NewDust(arg_C3C_0, arg_C3C_1, arg_C3C_2, arg_C3C_3, arg_C3C_4, arg_C3C_5, arg_C3C_6, newColor, 1f);
									}
								}
								if (this.type == 10)
								{
									int num10 = (int)(this.position.X / 16f) - 1;
									int num11 = (int)((this.position.X + (float)this.width) / 16f) + 2;
									int num12 = (int)(this.position.Y / 16f) - 1;
									int num13 = (int)((this.position.Y + (float)this.height) / 16f) + 2;
									if (num10 < 0)
									{
										num10 = 0;
									}
									if (num11 > Main.maxTilesX)
									{
										num11 = Main.maxTilesX;
									}
									if (num12 < 0)
									{
										num12 = 0;
									}
									if (num13 > Main.maxTilesY)
									{
										num13 = Main.maxTilesY;
									}
									for (int l = num10; l < num11; l++)
									{
										for (int m = num12; m < num13; m++)
										{
                                            Vector2 vector2;
											vector2.X = (float)(l * 16);
											vector2.Y = (float)(m * 16);
											if (this.position.X + (float)this.width > vector2.X && this.position.X < vector2.X + 16f && this.position.Y + (float)this.height > vector2.Y && this.position.Y < vector2.Y + 16f && Main.myPlayer == this.owner && Main.tile[l, m].active)
											{
												if (Main.tile[l, m].type == 23)
												{
													Main.tile[l, m].type = 2;
													WorldGen.SquareTileFrame(l, m, true);
													if (Main.netMode == 1)
													{
														NetMessage.SendTileSquare(-1, l - 1, m - 1, 3);
													}
												}
												if (Main.tile[l, m].type == 25)
												{
													Main.tile[l, m].type = 1;
													WorldGen.SquareTileFrame(l, m, true);
													if (Main.netMode == 1)
													{
														NetMessage.SendTileSquare(-1, l - 1, m - 1, 3);
													}
												}
											}
										}
									}
									return;
								}
							}
							else
							{
								if (this.aiStyle == 7)
								{
									if (Main.player[this.owner].dead)
									{
										this.Kill();
										return;
									}
									Vector2 vector3 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
									float num14 = Main.player[this.owner].position.X + (float)(Main.player[this.owner].width / 2) - vector3.X;
									float num15 = Main.player[this.owner].position.Y + (float)(Main.player[this.owner].height / 2) - vector3.Y;
									float num16 = (float)Math.Sqrt((double)(num14 * num14 + num15 * num15));
									this.rotation = (float)Math.Atan2((double)num15, (double)num14) - 1.57f;
									if (this.ai[0] == 0f)
									{
										if ((num16 > 300f && this.type == 13) || (num16 > 400f && this.type == 32))
										{
											this.ai[0] = 1f;
										}
										int num17 = (int)(this.position.X / 16f) - 1;
										int num18 = (int)((this.position.X + (float)this.width) / 16f) + 2;
										int num19 = (int)(this.position.Y / 16f) - 1;
										int num20 = (int)((this.position.Y + (float)this.height) / 16f) + 2;
										if (num17 < 0)
										{
											num17 = 0;
										}
										if (num18 > Main.maxTilesX)
										{
											num18 = Main.maxTilesX;
										}
										if (num19 < 0)
										{
											num19 = 0;
										}
										if (num20 > Main.maxTilesY)
										{
											num20 = Main.maxTilesY;
										}
										for (int n = num17; n < num18; n++)
										{
											int num21 = num19;
											while (num21 < num20)
											{
												if (Main.tile[n, num21] == null)
												{
													Main.tile[n, num21] = new Tile();
												}
                                                Vector2 vector4;
												vector4.X = (float)(n * 16);
												vector4.Y = (float)(num21 * 16);
												if (this.position.X + (float)this.width > vector4.X && this.position.X < vector4.X + 16f && this.position.Y + (float)this.height > vector4.Y && this.position.Y < vector4.Y + 16f && Main.tile[n, num21].active && Main.tileSolid[(int)Main.tile[n, num21].type])
												{
													if (Main.player[this.owner].grapCount < 10)
													{
														Main.player[this.owner].grappling[Main.player[this.owner].grapCount] = this.whoAmI;
														Main.player[this.owner].grapCount++;
													}
													if (Main.myPlayer == this.owner)
													{
														int num22 = 0;
														int num23 = -1;
														int num24 = 100000;
														for (int num25 = 0; num25 < 1000; num25++)
														{
															if (Main.projectile[num25].active && Main.projectile[num25].owner == this.owner && Main.projectile[num25].aiStyle == 7)
															{
																if (Main.projectile[num25].timeLeft < num24)
																{
																	num23 = num25;
																	num24 = Main.projectile[num25].timeLeft;
																}
																num22++;
															}
														}
														if (num22 > 3)
														{
															Main.projectile[num23].Kill();
														}
													}
													WorldGen.KillTile(n, num21, true, true, false);
													//Main.PlaySound(0, n * 16, num21 * 16, 1);
													this.velocity.X = 0f;
													this.velocity.Y = 0f;
													this.ai[0] = 2f;
													this.position.X = (float)(n * 16 + 8 - this.width / 2);
													this.position.Y = (float)(num21 * 16 + 8 - this.height / 2);
													this.damage = 0;
													this.netUpdate = true;
													if (Main.myPlayer == this.owner)
													{
														NetMessage.SendData(13, -1, -1, "", this.owner, 0f, 0f, 0f);
														break;
													}
													break;
												}
												else
												{
													num21++;
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
										float num26 = 11f;
										if (this.type == 32)
										{
											num26 = 15f;
										}
										if (num16 < 24f)
										{
											this.Kill();
										}
										num16 = num26 / num16;
										num14 *= num16;
										num15 *= num16;
										this.velocity.X = num14;
										this.velocity.Y = num15;
										return;
									}
									if (this.ai[0] == 2f)
									{
										int num27 = (int)(this.position.X / 16f) - 1;
										int num28 = (int)((this.position.X + (float)this.width) / 16f) + 2;
										int num29 = (int)(this.position.Y / 16f) - 1;
										int num30 = (int)((this.position.Y + (float)this.height) / 16f) + 2;
										if (num27 < 0)
										{
											num27 = 0;
										}
										if (num28 > Main.maxTilesX)
										{
											num28 = Main.maxTilesX;
										}
										if (num29 < 0)
										{
											num29 = 0;
										}
										if (num30 > Main.maxTilesY)
										{
											num30 = Main.maxTilesY;
										}
										bool flag = true;
										for (int num31 = num27; num31 < num28; num31++)
										{
											for (int num32 = num29; num32 < num30; num32++)
											{
												if (Main.tile[num31, num32] == null)
												{
													Main.tile[num31, num32] = new Tile();
												}
												Vector2 vector5;
												vector5.X = (float)(num31 * 16);
												vector5.Y = (float)(num32 * 16);
												if (this.position.X + (float)(this.width / 2) > vector5.X && this.position.X + (float)(this.width / 2) < vector5.X + 16f && this.position.Y + (float)(this.height / 2) > vector5.Y && this.position.Y + (float)(this.height / 2) < vector5.Y + 16f && Main.tile[num31, num32].active && Main.tileSolid[(int)Main.tile[num31, num32].type])
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
										if (Main.player[this.owner].grapCount < 10)
										{
											Main.player[this.owner].grappling[Main.player[this.owner].grapCount] = this.whoAmI;
											Main.player[this.owner].grapCount++;
											return;
										}
									}
								}
								else
								{
									if (this.aiStyle == 8)
									{
										if (this.type == 27)
										{
											Vector2 arg_1656_0 = new Vector2(this.position.X + this.velocity.X, this.position.Y + this.velocity.Y);
											int arg_1656_1 = this.width;
											int arg_1656_2 = this.height;
											int arg_1656_3 = 29;
											float arg_1656_4 = this.velocity.X;
											float arg_1656_5 = this.velocity.Y;
											int arg_1656_6 = 100;
											Color newColor =  new Color();
											int num33 = Dust.NewDust(arg_1656_0, arg_1656_1, arg_1656_2, arg_1656_3, arg_1656_4, arg_1656_5, arg_1656_6, newColor, 3f);
											Main.dust[num33].noGravity = true;
											Vector2 arg_16BB_0 = new Vector2(this.position.X, this.position.Y);
											int arg_16BB_1 = this.width;
											int arg_16BB_2 = this.height;
											int arg_16BB_3 = 29;
											float arg_16BB_4 = this.velocity.X;
											float arg_16BB_5 = this.velocity.Y;
											int arg_16BB_6 = 100;
											newColor =  new Color();
											num33 = Dust.NewDust(arg_16BB_0, arg_16BB_1, arg_16BB_2, arg_16BB_3, arg_16BB_4, arg_16BB_5, arg_16BB_6, newColor, 1.5f);
										}
										else
										{
											for (int num34 = 0; num34 < 2; num34++)
											{
												Vector2 arg_172A_0 = new Vector2(this.position.X, this.position.Y);
												int arg_172A_1 = this.width;
												int arg_172A_2 = this.height;
												int arg_172A_3 = 6;
												float arg_172A_4 = this.velocity.X * 0.2f;
												float arg_172A_5 = this.velocity.Y * 0.2f;
												int arg_172A_6 = 100;
												Color newColor =  new Color();
												int num35 = Dust.NewDust(arg_172A_0, arg_172A_1, arg_172A_2, arg_172A_3, arg_172A_4, arg_172A_5, arg_172A_6, newColor, 2f);
												Main.dust[num35].noGravity = true;
												Dust expr_174C_cp_0 = Main.dust[num35];
												expr_174C_cp_0.velocity.X = expr_174C_cp_0.velocity.X * 0.3f;
												Dust expr_176A_cp_0 = Main.dust[num35];
												expr_176A_cp_0.velocity.Y = expr_176A_cp_0.velocity.Y * 0.3f;
											}
										}
										if (this.type != 27)
										{
											this.ai[1] += 1f;
										}
										if (this.ai[1] >= 20f)
										{
											this.velocity.Y = this.velocity.Y + 0.2f;
										}
										this.rotation += 0.3f * (float)this.direction;
										if (this.velocity.Y > 16f)
										{
											this.velocity.Y = 16f;
											return;
										}
									}
									else
									{
										if (this.aiStyle == 9)
										{
											if (this.type == 34)
											{
												Vector2 arg_188B_0 = new Vector2(this.position.X, this.position.Y);
												int arg_188B_1 = this.width;
												int arg_188B_2 = this.height;
												int arg_188B_3 = 6;
												float arg_188B_4 = this.velocity.X * 0.2f;
												float arg_188B_5 = this.velocity.Y * 0.2f;
												int arg_188B_6 = 100;
												Color newColor =  new Color();
												int num36 = Dust.NewDust(arg_188B_0, arg_188B_1, arg_188B_2, arg_188B_3, arg_188B_4, arg_188B_5, arg_188B_6, newColor, 3.5f);
												Main.dust[num36].noGravity = true;
												Dust expr_18A8 = Main.dust[num36];
												expr_18A8.velocity *= 1.4f;
												Vector2 arg_1918_0 = new Vector2(this.position.X, this.position.Y);
												int arg_1918_1 = this.width;
												int arg_1918_2 = this.height;
												int arg_1918_3 = 6;
												float arg_1918_4 = this.velocity.X * 0.2f;
												float arg_1918_5 = this.velocity.Y * 0.2f;
												int arg_1918_6 = 100;
												newColor =  new Color();
												num36 = Dust.NewDust(arg_1918_0, arg_1918_1, arg_1918_2, arg_1918_3, arg_1918_4, arg_1918_5, arg_1918_6, newColor, 1.5f);
											}
											else
											{
												if (this.soundDelay == 0 && Math.Abs(this.velocity.X) + Math.Abs(this.velocity.Y) > 2f)
												{
													this.soundDelay = 10;
													//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 9);
												}
												Vector2 arg_19C0_0 = new Vector2(this.position.X, this.position.Y);
												int arg_19C0_1 = this.width;
												int arg_19C0_2 = this.height;
												int arg_19C0_3 = 15;
												float arg_19C0_4 = 0f;
												float arg_19C0_5 = 0f;
												int arg_19C0_6 = 100;
												Color newColor =  new Color();
												int num37 = Dust.NewDust(arg_19C0_0, arg_19C0_1, arg_19C0_2, arg_19C0_3, arg_19C0_4, arg_19C0_5, arg_19C0_6, newColor, 2f);
												Dust expr_19CF = Main.dust[num37];
												expr_19CF.velocity *= 0.3f;
												Main.dust[num37].position.X = this.position.X + (float)(this.width / 2) + 4f + (float)Main.rand.Next(-4, 5);
												Main.dust[num37].position.Y = this.position.Y + (float)(this.height / 2) + (float)Main.rand.Next(-4, 5);
												Main.dust[num37].noGravity = true;
											}
											if (Main.myPlayer == this.owner && this.ai[0] == 0f)
											{
												if (Main.player[this.owner].channel)
												{
                                                    //float num38 = 12f;
                                                    //Vector2 vector6 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                    //float num39 = (float)Main.mouseState.X + Main.screenPosition.X - vector6.X;
                                                    //float num40 = (float)Main.mouseState.Y + Main.screenPosition.Y - vector6.Y;
                                                    //float num41 = (float)Math.Sqrt((double)(num39 * num39 + num40 * num40));
                                                    //num41 = (float)Math.Sqrt((double)(num39 * num39 + num40 * num40));
                                                    //if (num41 > num38)
                                                    //{
                                                    //    num41 = num38 / num41;
                                                    //    num39 *= num41;
                                                    //    num40 *= num41;
                                                    //    if (num39 != this.velocity.X || num40 != this.velocity.Y)
                                                    //    {
                                                    //        this.netUpdate = true;
                                                    //    }
                                                    //    this.velocity.X = num39;
                                                    //    this.velocity.Y = num40;
                                                    //}
                                                    //else
                                                    //{
                                                    //    if (num39 != this.velocity.X || num40 != this.velocity.Y)
                                                    //    {
                                                    //        this.netUpdate = true;
                                                    //    }
                                                    //    this.velocity.X = num39;
                                                    //    this.velocity.Y = num40;
                                                    //}
												}
												else
												{
													this.Kill();
												}
											}
											if (this.type == 34)
											{
												this.rotation += 0.3f * (float)this.direction;
											}
											else
											{
												if (this.velocity.X != 0f || this.velocity.Y != 0f)
												{
													this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) - 2.355f;
												}
											}
											if (this.velocity.Y > 16f)
											{
												this.velocity.Y = 16f;
												return;
											}
										}
										else
										{
											if (this.aiStyle == 10)
											{
												if (this.type == 31)
												{
													if (Main.rand.Next(2) == 0)
													{
														Vector2 arg_1CFA_0 = new Vector2(this.position.X, this.position.Y);
														int arg_1CFA_1 = this.width;
														int arg_1CFA_2 = this.height;
														int arg_1CFA_3 = 32;
														float arg_1CFA_4 = 0f;
														float arg_1CFA_5 = this.velocity.Y / 2f;
														int arg_1CFA_6 = 0;
														Color newColor =  new Color();
														int num42 = Dust.NewDust(arg_1CFA_0, arg_1CFA_1, arg_1CFA_2, arg_1CFA_3, arg_1CFA_4, arg_1CFA_5, arg_1CFA_6, newColor, 1f);
														Dust expr_1D0E_cp_0 = Main.dust[num42];
														expr_1D0E_cp_0.velocity.X = expr_1D0E_cp_0.velocity.X * 0.4f;
													}
												}
												else
												{
													if (Main.rand.Next(20) == 0)
													{
														Vector2 arg_1D71_0 = new Vector2(this.position.X, this.position.Y);
														int arg_1D71_1 = this.width;
														int arg_1D71_2 = this.height;
														int arg_1D71_3 = 0;
														float arg_1D71_4 = 0f;
														float arg_1D71_5 = 0f;
														int arg_1D71_6 = 0;
														Color newColor =  new Color();
														Dust.NewDust(arg_1D71_0, arg_1D71_1, arg_1D71_2, arg_1D71_3, arg_1D71_4, arg_1D71_5, arg_1D71_6, newColor, 1f);
													}
												}
												if (Main.myPlayer == this.owner && this.ai[0] == 0f)
												{
													if (Main.player[this.owner].channel)
													{
                                                        //float num43 = 12f;
                                                        //Vector2 vector7 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
                                                        //float num44 = (float)Main.mouseState.X + Main.screenPosition.X - vector7.X;
                                                        //float num45 = (float)Main.mouseState.Y + Main.screenPosition.Y - vector7.Y;
                                                        //float num46 = (float)Math.Sqrt((double)(num44 * num44 + num45 * num45));
                                                        //num46 = (float)Math.Sqrt((double)(num44 * num44 + num45 * num45));
                                                        //if (num46 > num43)
                                                        //{
                                                        //    num46 = num43 / num46;
                                                        //    num44 *= num46;
                                                        //    num45 *= num46;
                                                        //    if (num44 != this.velocity.X || num45 != this.velocity.Y)
                                                        //    {
                                                        //        this.netUpdate = true;
                                                        //    }
                                                        //    this.velocity.X = num44;
                                                        //    this.velocity.Y = num45;
                                                        //}
                                                        //else
                                                        //{
                                                        //    if (num44 != this.velocity.X || num45 != this.velocity.Y)
                                                        //    {
                                                        //        this.netUpdate = true;
                                                        //    }
                                                        //    this.velocity.X = num44;
                                                        //    this.velocity.Y = num45;
                                                        //}
													}
													else
													{
														this.ai[0] = 1f;
														this.netUpdate = true;
													}
												}
												if (this.ai[0] == 1f)
												{
													this.velocity.Y = this.velocity.Y + 0.41f;
												}
												this.rotation += 0.1f;
												if (this.velocity.Y > 10f)
												{
													this.velocity.Y = 10f;
													return;
												}
											}
											else
											{
												if (this.aiStyle == 11)
												{
													this.rotation += 0.02f;
													if (Main.myPlayer == this.owner)
													{
														if (Main.player[this.owner].dead)
														{
															this.Kill();
															return;
														}
														float num47 = 4f;
														Vector2 vector8 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
														float num48 = Main.player[this.owner].position.X + (float)(Main.player[this.owner].width / 2) - vector8.X;
														float num49 = Main.player[this.owner].position.Y + (float)(Main.player[this.owner].height / 2) - vector8.Y;
														float num50 = (float)Math.Sqrt((double)(num48 * num48 + num49 * num49));
														num50 = (float)Math.Sqrt((double)(num48 * num48 + num49 * num49));
														if (num50 > (float)Main.screenWidth)
														{
															this.position.X = Main.player[this.owner].position.X + (float)(Main.player[this.owner].width / 2) - (float)(this.width / 2);
															this.position.Y = Main.player[this.owner].position.Y + (float)(Main.player[this.owner].height / 2) - (float)(this.height / 2);
															return;
														}
														if (num50 > 64f)
														{
															num50 = num47 / num50;
															num48 *= num50;
															num49 *= num50;
															if (num48 != this.velocity.X || num49 != this.velocity.Y)
															{
																this.netUpdate = true;
															}
															this.velocity.X = num48;
															this.velocity.Y = num49;
															return;
														}
														if (this.velocity.X != 0f || this.velocity.Y != 0f)
														{
															this.netUpdate = true;
														}
														this.velocity.X = 0f;
														this.velocity.Y = 0f;
														return;
													}
												}
												else
												{
													if (this.aiStyle == 12)
													{
														this.scale -= 0.05f;
														if (this.scale <= 0f)
														{
															this.Kill();
														}
														if (this.ai[0] > 4f)
														{
															this.alpha = 150;
															this.light = 0.8f;
															Vector2 arg_2266_0 = new Vector2(this.position.X, this.position.Y);
															int arg_2266_1 = this.width;
															int arg_2266_2 = this.height;
															int arg_2266_3 = 29;
															float arg_2266_4 = this.velocity.X;
															float arg_2266_5 = this.velocity.Y;
															int arg_2266_6 = 100;
															Color newColor =  new Color();
															int num51 = Dust.NewDust(arg_2266_0, arg_2266_1, arg_2266_2, arg_2266_3, arg_2266_4, arg_2266_5, arg_2266_6, newColor, 2.5f);
															Main.dust[num51].noGravity = true;
															Vector2 arg_22CB_0 = new Vector2(this.position.X, this.position.Y);
															int arg_22CB_1 = this.width;
															int arg_22CB_2 = this.height;
															int arg_22CB_3 = 29;
															float arg_22CB_4 = this.velocity.X;
															float arg_22CB_5 = this.velocity.Y;
															int arg_22CB_6 = 100;
															newColor =  new Color();
															Dust.NewDust(arg_22CB_0, arg_22CB_1, arg_22CB_2, arg_22CB_3, arg_22CB_4, arg_22CB_5, arg_22CB_6, newColor, 1.5f);
														}
														else
														{
															this.ai[0] += 1f;
														}
														this.rotation += 0.3f * (float)this.direction;
														return;
													}
													if (this.aiStyle == 13)
													{
														if (Main.player[this.owner].dead)
														{
															this.Kill();
															return;
														}
														Main.player[this.owner].itemAnimation = 5;
														Main.player[this.owner].itemTime = 5;
														if (this.position.X + (float)(this.width / 2) > Main.player[this.owner].position.X + (float)(Main.player[this.owner].width / 2))
														{
															Main.player[this.owner].direction = 1;
														}
														else
														{
															Main.player[this.owner].direction = -1;
														}
														Vector2 vector9 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
														float num52 = Main.player[this.owner].position.X + (float)(Main.player[this.owner].width / 2) - vector9.X;
														float num53 = Main.player[this.owner].position.Y + (float)(Main.player[this.owner].height / 2) - vector9.Y;
														float num54 = (float)Math.Sqrt((double)(num52 * num52 + num53 * num53));
														if (this.ai[0] == 0f)
														{
															if (num54 > 600f)
															{
																this.ai[0] = 1f;
															}
															this.rotation = (float)Math.Atan2((double)this.velocity.Y, (double)this.velocity.X) + 1.57f;
															this.ai[1] += 1f;
															if (this.ai[1] > 2f)
															{
																this.alpha = 0;
															}
															if (this.ai[1] >= 10f)
															{
																this.ai[1] = 15f;
																this.velocity.Y = this.velocity.Y + 0.3f;
																return;
															}
														}
														else
														{
															if (this.ai[0] == 1f)
															{
																this.tileCollide = false;
																this.rotation = (float)Math.Atan2((double)num53, (double)num52) - 1.57f;
																float num55 = 11f;
																if (num54 < 50f)
																{
																	this.Kill();
																}
																num54 = num55 / num54;
																num52 *= num54;
																num53 *= num54;
																this.velocity.X = num52;
																this.velocity.Y = num53;
																return;
															}
														}
													}
													else
													{
														if (this.aiStyle == 14)
														{
															this.ai[0] += 1f;
															if (this.ai[0] > 5f)
															{
																this.ai[0] = 5f;
																if (this.velocity.Y == 0f && this.velocity.X != 0f)
																{
																	this.velocity.X = this.velocity.X * 0.97f;
																	if ((double)this.velocity.X > -0.01 && (double)this.velocity.X < 0.01)
																	{
																		this.velocity.X = 0f;
																		this.netUpdate = true;
																	}
																}
																this.velocity.Y = this.velocity.Y + 0.2f;
															}
															this.rotation += this.velocity.X * 0.1f;
															return;
														}
														if (this.aiStyle == 15)
														{
															if (this.type == 25)
															{
																if (Main.rand.Next(15) == 0)
																{
																	Vector2 arg_2701_0 = this.position;
																	int arg_2701_1 = this.width;
																	int arg_2701_2 = this.height;
																	int arg_2701_3 = 14;
																	float arg_2701_4 = 0f;
																	float arg_2701_5 = 0f;
																	int arg_2701_6 = 150;
																	Color newColor =  new Color();
																	Dust.NewDust(arg_2701_0, arg_2701_1, arg_2701_2, arg_2701_3, arg_2701_4, arg_2701_5, arg_2701_6, newColor, 1.3f);
																}
															}
															else
															{
																if (this.type == 26)
																{
																	Vector2 arg_2760_0 = this.position;
																	int arg_2760_1 = this.width;
																	int arg_2760_2 = this.height;
																	int arg_2760_3 = 29;
																	float arg_2760_4 = this.velocity.X * 0.4f;
																	float arg_2760_5 = this.velocity.Y * 0.4f;
																	int arg_2760_6 = 100;
																	Color newColor =  new Color();
																	int num56 = Dust.NewDust(arg_2760_0, arg_2760_1, arg_2760_2, arg_2760_3, arg_2760_4, arg_2760_5, arg_2760_6, newColor, 2.5f);
																	Main.dust[num56].noGravity = true;
																	Dust expr_2782_cp_0 = Main.dust[num56];
																	expr_2782_cp_0.velocity.X = expr_2782_cp_0.velocity.X / 2f;
																	Dust expr_27A0_cp_0 = Main.dust[num56];
																	expr_27A0_cp_0.velocity.Y = expr_27A0_cp_0.velocity.Y / 2f;
																}
																else
																{
																	if (this.type == 35)
																	{
																		Vector2 arg_2809_0 = this.position;
																		int arg_2809_1 = this.width;
																		int arg_2809_2 = this.height;
																		int arg_2809_3 = 6;
																		float arg_2809_4 = this.velocity.X * 0.4f;
																		float arg_2809_5 = this.velocity.Y * 0.4f;
																		int arg_2809_6 = 100;
																		Color newColor =  new Color();
																		int num57 = Dust.NewDust(arg_2809_0, arg_2809_1, arg_2809_2, arg_2809_3, arg_2809_4, arg_2809_5, arg_2809_6, newColor, 3f);
																		Main.dust[num57].noGravity = true;
																		Dust expr_282B_cp_0 = Main.dust[num57];
																		expr_282B_cp_0.velocity.X = expr_282B_cp_0.velocity.X * 2f;
																		Dust expr_2849_cp_0 = Main.dust[num57];
																		expr_2849_cp_0.velocity.Y = expr_2849_cp_0.velocity.Y * 2f;
																	}
																}
															}
															if (Main.player[this.owner].dead)
															{
																this.Kill();
																return;
															}
															Main.player[this.owner].itemAnimation = 5;
															Main.player[this.owner].itemTime = 5;
															if (this.position.X + (float)(this.width / 2) > Main.player[this.owner].position.X + (float)(Main.player[this.owner].width / 2))
															{
																Main.player[this.owner].direction = 1;
															}
															else
															{
																Main.player[this.owner].direction = -1;
															}
															Vector2 vector10 = new Vector2(this.position.X + (float)this.width * 0.5f, this.position.Y + (float)this.height * 0.5f);
															float num58 = Main.player[this.owner].position.X + (float)(Main.player[this.owner].width / 2) - vector10.X;
															float num59 = Main.player[this.owner].position.Y + (float)(Main.player[this.owner].height / 2) - vector10.Y;
															float num60 = (float)Math.Sqrt((double)(num58 * num58 + num59 * num59));
															if (this.ai[0] == 0f)
															{
																this.tileCollide = true;
																if (num60 > 300f)
																{
																	this.ai[0] = 1f;
																}
																else
																{
																	this.ai[1] += 1f;
																	if (this.ai[1] > 2f)
																	{
																		this.alpha = 0;
																	}
																	if (this.ai[1] >= 5f)
																	{
																		this.ai[1] = 15f;
																		this.velocity.Y = this.velocity.Y + 0.5f;
																		this.velocity.X = this.velocity.X * 0.95f;
																	}
																}
															}
															else
															{
																if (this.ai[0] == 1f)
																{
																	this.tileCollide = false;
																	float num61 = 11f;
																	if (num60 < 20f)
																	{
																		this.Kill();
																	}
																	num60 = num61 / num60;
																	num58 *= num60;
																	num59 *= num60;
																	this.velocity.X = num58;
																	this.velocity.Y = num59;
																}
															}
															this.rotation += this.velocity.X * 0.03f;
															return;
														}
														else
														{
															if (this.aiStyle == 16)
															{
																if (this.owner == Main.myPlayer && this.timeLeft <= 3 && this.ai[1] == 0f)
																{
																	this.ai[1] = 1f;
																	this.netUpdate = true;
																}
																if (this.type == 37)
																{
																	try
																	{
																		int num62 = (int)(this.position.X / 16f) - 1;
																		int num63 = (int)((this.position.X + (float)this.width) / 16f) + 2;
																		int num64 = (int)(this.position.Y / 16f) - 1;
																		int num65 = (int)((this.position.Y + (float)this.height) / 16f) + 2;
																		if (num62 < 0)
																		{
																			num62 = 0;
																		}
																		if (num63 > Main.maxTilesX)
																		{
																			num63 = Main.maxTilesX;
																		}
																		if (num64 < 0)
																		{
																			num64 = 0;
																		}
																		if (num65 > Main.maxTilesY)
																		{
																			num65 = Main.maxTilesY;
																		}
																		for (int num66 = num62; num66 < num63; num66++)
																		{
																			for (int num67 = num64; num67 < num65; num67++)
																			{
																				if (Main.tile[num66, num67] != null && Main.tile[num66, num67].active && (Main.tileSolid[(int)Main.tile[num66, num67].type] || (Main.tileSolidTop[(int)Main.tile[num66, num67].type] && Main.tile[num66, num67].frameY == 0)))
																				{
                                                                                    Vector2 vector11;
																					vector11.X = (float)(num66 * 16);
																					vector11.Y = (float)(num67 * 16);
																					if (this.position.X + (float)this.width - 4f > vector11.X && this.position.X + 4f < vector11.X + 16f && this.position.Y + (float)this.height - 4f > vector11.Y && this.position.Y + 4f < vector11.Y + 16f)
																					{
																						this.velocity.X = 0f;
																						this.velocity.Y = -0.2f;
																					}
																				}
																			}
																		}
																	}
																	catch
																	{
																	}
																}
																if (this.ai[1] > 0f)
																{
																	this.alpha = 255;
																	if (this.type == 28 || this.type == 37)
																	{
																		this.position.X = this.position.X + (float)(this.width / 2);
																		this.position.Y = this.position.Y + (float)(this.height / 2);
																		this.width = 128;
																		this.height = 128;
																		this.position.X = this.position.X - (float)(this.width / 2);
																		this.position.Y = this.position.Y - (float)(this.height / 2);
																		this.damage = 100;
																		this.knockBack = 8f;
																	}
																	else
																	{
																		if (this.type == 29)
																		{
																			this.position.X = this.position.X + (float)(this.width / 2);
																			this.position.Y = this.position.Y + (float)(this.height / 2);
																			this.width = 250;
																			this.height = 250;
																			this.position.X = this.position.X - (float)(this.width / 2);
																			this.position.Y = this.position.Y - (float)(this.height / 2);
																			this.damage = 250;
																			this.knockBack = 10f;
																		}
																		else
																		{
																			if (this.type == 30)
																			{
																				this.position.X = this.position.X + (float)(this.width / 2);
																				this.position.Y = this.position.Y + (float)(this.height / 2);
																				this.width = 128;
																				this.height = 128;
																				this.position.X = this.position.X - (float)(this.width / 2);
																				this.position.Y = this.position.Y - (float)(this.height / 2);
																				this.knockBack = 8f;
																			}
																		}
																	}
																}
																else
																{
																	if (this.type != 30 && Main.rand.Next(4) == 0)
																	{
																		if (this.type != 30)
																		{
																			this.damage = 0;
																		}
																		Vector2 arg_2FC3_0 = new Vector2(this.position.X, this.position.Y);
																		int arg_2FC3_1 = this.width;
																		int arg_2FC3_2 = this.height;
																		int arg_2FC3_3 = 6;
																		float arg_2FC3_4 = 0f;
																		float arg_2FC3_5 = 0f;
																		int arg_2FC3_6 = 100;
																		Color newColor =  new Color();
																		Dust.NewDust(arg_2FC3_0, arg_2FC3_1, arg_2FC3_2, arg_2FC3_3, arg_2FC3_4, arg_2FC3_5, arg_2FC3_6, newColor, 1f);
																	}
																}
																this.ai[0] += 1f;
																if ((this.type == 30 && this.ai[0] > 10f) || (this.type != 30 && this.ai[0] > 5f))
																{
																	this.ai[0] = 10f;
																	if (this.velocity.Y == 0f && this.velocity.X != 0f)
																	{
																		this.velocity.X = this.velocity.X * 0.97f;
																		if (this.type == 29)
																		{
																			this.velocity.X = this.velocity.X * 0.99f;
																		}
																		if ((double)this.velocity.X > -0.01 && (double)this.velocity.X < 0.01)
																		{
																			this.velocity.X = 0f;
																			this.netUpdate = true;
																		}
																	}
																	this.velocity.Y = this.velocity.Y + 0.2f;
																}
																this.rotation += this.velocity.X * 0.1f;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
		
        public void Kill()
		{
			if (!this.active)
			{
				return;
			}
			if (this.type == 1)
			{
				//Main.PlaySound(0, (int)this.position.X, (int)this.position.Y, 1);
				for (int i = 0; i < 10; i++)
				{
					Vector2 arg_77_0 = new Vector2(this.position.X, this.position.Y);
					int arg_77_1 = this.width;
					int arg_77_2 = this.height;
					int arg_77_3 = 7;
					float arg_77_4 = 0f;
					float arg_77_5 = 0f;
					int arg_77_6 = 0;
					Color newColor =  new Color();
					Dust.NewDust(arg_77_0, arg_77_1, arg_77_2, arg_77_3, arg_77_4, arg_77_5, arg_77_6, newColor, 1f);
				}
			}
			else
			{
				if (this.type == 2)
				{
					//Main.PlaySound(0, (int)this.position.X, (int)this.position.Y, 1);
					for (int j = 0; j < 20; j++)
					{
						Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, 0f, 0f, 100,  new Color(), 1f);
					}
				}
				else
				{
					if (this.type == 3)
					{
						//Main.PlaySound(0, (int)this.position.X, (int)this.position.Y, 1);
						for (int k = 0; k < 10; k++)
						{
							Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 1, this.velocity.X * 0.1f, this.velocity.Y * 0.1f, 0,  new Color(), 0.75f);
						}
					}
					else
					{
						if (this.type == 4)
						{
							//Main.PlaySound(0, (int)this.position.X, (int)this.position.Y, 1);
							for (int l = 0; l < 10; l++)
							{
								Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 14, 0f, 0f, 150,  new Color(), 1.1f);
							}
						}
						else
						{
							if (this.type == 5)
							{
								//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
								for (int m = 0; m < 60; m++)
								{
									Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X * 0.5f, this.velocity.Y * 0.5f, 150,  new Color(), 1.5f);
								}
							}
							else
							{
								if (this.type == 9 || this.type == 12)
								{
									//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
									for (int n = 0; n < 10; n++)
									{
										Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X * 0.1f, this.velocity.Y * 0.1f, 150,  new Color(), 1.2f);
									}
									for (int num = 0; num < 3; num++)
									{
										Gore.NewGore(this.position, new Vector2(this.velocity.X * 0.05f, this.velocity.Y * 0.05f), Main.rand.Next(16, 18));
									}
									if (this.type == 12 && this.damage < 100)
									{
										for (int num2 = 0; num2 < 10; num2++)
										{
											Dust.NewDust(this.position, this.width, this.height, 15, this.velocity.X * 0.1f, this.velocity.Y * 0.1f, 150,  new Color(), 1.2f);
										}
										for (int num3 = 0; num3 < 3; num3++)
										{
											Gore.NewGore(this.position, new Vector2(this.velocity.X * 0.05f, this.velocity.Y * 0.05f), Main.rand.Next(16, 18));
										}
									}
								}
								else
								{
									if (this.type == 14 || this.type == 20 || this.type == 36)
									{
										Collision.HitTiles(this.position, this.velocity, this.width, this.height);
										//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
									}
									else
									{
										if (this.type == 15 || this.type == 34)
										{
											//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
											for (int num4 = 0; num4 < 20; num4++)
											{
												int num5 = Dust.NewDust(new Vector2(this.position.X, this.position.Y), this.width, this.height, 6, -this.velocity.X * 0.2f, -this.velocity.Y * 0.2f, 100,  new Color(), 2f);
												Main.dust[num5].noGravity = true;
												Dust expr_597 = Main.dust[num5];
												expr_597.velocity *= 2f;
												Vector2 arg_609_0 = new Vector2(this.position.X, this.position.Y);
												int arg_609_1 = this.width;
												int arg_609_2 = this.height;
												int arg_609_3 = 6;
												float arg_609_4 = -this.velocity.X * 0.2f;
												float arg_609_5 = -this.velocity.Y * 0.2f;
												int arg_609_6 = 100;
												Color newColor =  new Color();
												num5 = Dust.NewDust(arg_609_0, arg_609_1, arg_609_2, arg_609_3, arg_609_4, arg_609_5, arg_609_6, newColor, 1f);
												Dust expr_618 = Main.dust[num5];
												expr_618.velocity *= 2f;
											}
										}
										else
										{
											if (this.type == 16)
											{
												//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
												for (int num6 = 0; num6 < 20; num6++)
												{
													Vector2 arg_6D2_0 = new Vector2(this.position.X - this.velocity.X, this.position.Y - this.velocity.Y);
													int arg_6D2_1 = this.width;
													int arg_6D2_2 = this.height;
													int arg_6D2_3 = 15;
													float arg_6D2_4 = 0f;
													float arg_6D2_5 = 0f;
													int arg_6D2_6 = 100;
													Color newColor =  new Color();
													int num7 = Dust.NewDust(arg_6D2_0, arg_6D2_1, arg_6D2_2, arg_6D2_3, arg_6D2_4, arg_6D2_5, arg_6D2_6, newColor, 2f);
													Main.dust[num7].noGravity = true;
													Dust expr_6EF = Main.dust[num7];
													expr_6EF.velocity *= 2f;
													Vector2 arg_760_0 = new Vector2(this.position.X - this.velocity.X, this.position.Y - this.velocity.Y);
													int arg_760_1 = this.width;
													int arg_760_2 = this.height;
													int arg_760_3 = 15;
													float arg_760_4 = 0f;
													float arg_760_5 = 0f;
													int arg_760_6 = 100;
													newColor =  new Color();
													num7 = Dust.NewDust(arg_760_0, arg_760_1, arg_760_2, arg_760_3, arg_760_4, arg_760_5, arg_760_6, newColor, 1f);
												}
											}
											else
											{
												if (this.type == 17)
												{
													//Main.PlaySound(0, (int)this.position.X, (int)this.position.Y, 1);
													for (int num8 = 0; num8 < 5; num8++)
													{
														Vector2 arg_7EB_0 = new Vector2(this.position.X, this.position.Y);
														int arg_7EB_1 = this.width;
														int arg_7EB_2 = this.height;
														int arg_7EB_3 = 0;
														float arg_7EB_4 = 0f;
														float arg_7EB_5 = 0f;
														int arg_7EB_6 = 0;
														Color newColor =  new Color();
														Dust.NewDust(arg_7EB_0, arg_7EB_1, arg_7EB_2, arg_7EB_3, arg_7EB_4, arg_7EB_5, arg_7EB_6, newColor, 1f);
													}
												}
												else
												{
													if (this.type == 31)
													{
														//Main.PlaySound(0, (int)this.position.X, (int)this.position.Y, 1);
														for (int num9 = 0; num9 < 5; num9++)
														{
															Vector2 arg_875_0 = new Vector2(this.position.X, this.position.Y);
															int arg_875_1 = this.width;
															int arg_875_2 = this.height;
															int arg_875_3 = 32;
															float arg_875_4 = 0f;
															float arg_875_5 = 0f;
															int arg_875_6 = 0;
															Color newColor =  new Color();
															int num10 = Dust.NewDust(arg_875_0, arg_875_1, arg_875_2, arg_875_3, arg_875_4, arg_875_5, arg_875_6, newColor, 1f);
															Dust expr_884 = Main.dust[num10];
															expr_884.velocity *= 0.6f;
														}
													}
													else
													{
														if (this.type == 21)
														{
															//Main.PlaySound(0, (int)this.position.X, (int)this.position.Y, 1);
															for (int num11 = 0; num11 < 10; num11++)
															{
																Vector2 arg_91A_0 = new Vector2(this.position.X, this.position.Y);
																int arg_91A_1 = this.width;
																int arg_91A_2 = this.height;
																int arg_91A_3 = 26;
																float arg_91A_4 = 0f;
																float arg_91A_5 = 0f;
																int arg_91A_6 = 0;
																Color newColor =  new Color();
																Dust.NewDust(arg_91A_0, arg_91A_1, arg_91A_2, arg_91A_3, arg_91A_4, arg_91A_5, arg_91A_6, newColor, 0.8f);
															}
														}
														else
														{
															if (this.type == 24)
															{
																for (int num12 = 0; num12 < 10; num12++)
																{
																	Vector2 arg_99A_0 = new Vector2(this.position.X, this.position.Y);
																	int arg_99A_1 = this.width;
																	int arg_99A_2 = this.height;
																	int arg_99A_3 = 1;
																	float arg_99A_4 = this.velocity.X * 0.1f;
																	float arg_99A_5 = this.velocity.Y * 0.1f;
																	int arg_99A_6 = 0;
																	Color newColor =  new Color();
																	Dust.NewDust(arg_99A_0, arg_99A_1, arg_99A_2, arg_99A_3, arg_99A_4, arg_99A_5, arg_99A_6, newColor, 0.75f);
																}
															}
															else
															{
																if (this.type == 27)
																{
																	//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 10);
																	for (int num13 = 0; num13 < 30; num13++)
																	{
																		Vector2 arg_A42_0 = new Vector2(this.position.X, this.position.Y);
																		int arg_A42_1 = this.width;
																		int arg_A42_2 = this.height;
																		int arg_A42_3 = 29;
																		float arg_A42_4 = this.velocity.X * 0.1f;
																		float arg_A42_5 = this.velocity.Y * 0.1f;
																		int arg_A42_6 = 100;
																		Color newColor =  new Color();
																		int num14 = Dust.NewDust(arg_A42_0, arg_A42_1, arg_A42_2, arg_A42_3, arg_A42_4, arg_A42_5, arg_A42_6, newColor, 3f);
																		Main.dust[num14].noGravity = true;
																		Vector2 arg_AB3_0 = new Vector2(this.position.X, this.position.Y);
																		int arg_AB3_1 = this.width;
																		int arg_AB3_2 = this.height;
																		int arg_AB3_3 = 29;
																		float arg_AB3_4 = this.velocity.X * 0.1f;
																		float arg_AB3_5 = this.velocity.Y * 0.1f;
																		int arg_AB3_6 = 100;
																		newColor =  new Color();
																		Dust.NewDust(arg_AB3_0, arg_AB3_1, arg_AB3_2, arg_AB3_3, arg_AB3_4, arg_AB3_5, arg_AB3_6, newColor, 2f);
																	}
																}
																else
																{
																	if (this.type == 28 || this.type == 30 || this.type == 37)
																	{
																		//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 14);
																		this.position.X = this.position.X + (float)(this.width / 2);
																		this.position.Y = this.position.Y + (float)(this.height / 2);
																		this.width = 22;
																		this.height = 22;
																		this.position.X = this.position.X - (float)(this.width / 2);
																		this.position.Y = this.position.Y - (float)(this.height / 2);
																		for (int num15 = 0; num15 < 20; num15++)
																		{
																			Vector2 arg_BD3_0 = new Vector2(this.position.X, this.position.Y);
																			int arg_BD3_1 = this.width;
																			int arg_BD3_2 = this.height;
																			int arg_BD3_3 = 31;
																			float arg_BD3_4 = 0f;
																			float arg_BD3_5 = 0f;
																			int arg_BD3_6 = 100;
																			Color newColor =  new Color();
																			int num16 = Dust.NewDust(arg_BD3_0, arg_BD3_1, arg_BD3_2, arg_BD3_3, arg_BD3_4, arg_BD3_5, arg_BD3_6, newColor, 1.5f);
																			Dust expr_BE2 = Main.dust[num16];
																			expr_BE2.velocity *= 1.4f;
																		}
																		for (int num17 = 0; num17 < 10; num17++)
																		{
																			Vector2 arg_C4E_0 = new Vector2(this.position.X, this.position.Y);
																			int arg_C4E_1 = this.width;
																			int arg_C4E_2 = this.height;
																			int arg_C4E_3 = 6;
																			float arg_C4E_4 = 0f;
																			float arg_C4E_5 = 0f;
																			int arg_C4E_6 = 100;
																			Color newColor =  new Color();
																			int num18 = Dust.NewDust(arg_C4E_0, arg_C4E_1, arg_C4E_2, arg_C4E_3, arg_C4E_4, arg_C4E_5, arg_C4E_6, newColor, 2.5f);
																			Main.dust[num18].noGravity = true;
																			Dust expr_C6B = Main.dust[num18];
																			expr_C6B.velocity *= 5f;
																			Vector2 arg_CC3_0 = new Vector2(this.position.X, this.position.Y);
																			int arg_CC3_1 = this.width;
																			int arg_CC3_2 = this.height;
																			int arg_CC3_3 = 6;
																			float arg_CC3_4 = 0f;
																			float arg_CC3_5 = 0f;
																			int arg_CC3_6 = 100;
																			newColor =  new Color();
																			num18 = Dust.NewDust(arg_CC3_0, arg_CC3_1, arg_CC3_2, arg_CC3_3, arg_CC3_4, arg_CC3_5, arg_CC3_6, newColor, 1.5f);
																			Dust expr_CD2 = Main.dust[num18];
																			expr_CD2.velocity *= 3f;
																		}
																		Vector2 arg_D29_0 = new Vector2(this.position.X, this.position.Y);
                                                                        Vector2 vector = default(Vector2);
																		int num19 = Gore.NewGore(arg_D29_0, vector, Main.rand.Next(61, 64));
																		Gore expr_D38 = Main.gore[num19];
																		expr_D38.velocity *= 0.4f;
																		Gore expr_D5A_cp_0 = Main.gore[num19];
																		expr_D5A_cp_0.velocity.X = expr_D5A_cp_0.velocity.X + 1f;
																		Gore expr_D78_cp_0 = Main.gore[num19];
																		expr_D78_cp_0.velocity.Y = expr_D78_cp_0.velocity.Y + 1f;
																		Vector2 arg_DBC_0 = new Vector2(this.position.X, this.position.Y);
																		vector = default(Vector2);
																		num19 = Gore.NewGore(arg_DBC_0, vector, Main.rand.Next(61, 64));
																		Gore expr_DCB = Main.gore[num19];
																		expr_DCB.velocity *= 0.4f;
																		Gore expr_DED_cp_0 = Main.gore[num19];
																		expr_DED_cp_0.velocity.X = expr_DED_cp_0.velocity.X - 1f;
																		Gore expr_E0B_cp_0 = Main.gore[num19];
																		expr_E0B_cp_0.velocity.Y = expr_E0B_cp_0.velocity.Y + 1f;
																		Vector2 arg_E4F_0 = new Vector2(this.position.X, this.position.Y);
                                                                        vector = default(Vector2);
																		num19 = Gore.NewGore(arg_E4F_0, vector, Main.rand.Next(61, 64));
																		Gore expr_E5E = Main.gore[num19];
																		expr_E5E.velocity *= 0.4f;
																		Gore expr_E80_cp_0 = Main.gore[num19];
																		expr_E80_cp_0.velocity.X = expr_E80_cp_0.velocity.X + 1f;
																		Gore expr_E9E_cp_0 = Main.gore[num19];
																		expr_E9E_cp_0.velocity.Y = expr_E9E_cp_0.velocity.Y - 1f;
																		Vector2 arg_EE2_0 = new Vector2(this.position.X, this.position.Y);
                                                                        vector = default(Vector2);
																		num19 = Gore.NewGore(arg_EE2_0, vector, Main.rand.Next(61, 64));
																		Gore expr_EF1 = Main.gore[num19];
																		expr_EF1.velocity *= 0.4f;
																		Gore expr_F13_cp_0 = Main.gore[num19];
																		expr_F13_cp_0.velocity.X = expr_F13_cp_0.velocity.X - 1f;
																		Gore expr_F31_cp_0 = Main.gore[num19];
																		expr_F31_cp_0.velocity.Y = expr_F31_cp_0.velocity.Y - 1f;
																	}
																	else
																	{
																		if (this.type == 29)
																		{
																			//Main.PlaySound(2, (int)this.position.X, (int)this.position.Y, 14);
																			this.position.X = this.position.X + (float)(this.width / 2);
																			this.position.Y = this.position.Y + (float)(this.height / 2);
																			this.width = 200;
																			this.height = 200;
																			this.position.X = this.position.X - (float)(this.width / 2);
																			this.position.Y = this.position.Y - (float)(this.height / 2);
																			for (int num20 = 0; num20 < 50; num20++)
																			{
																				Vector2 arg_103F_0 = new Vector2(this.position.X, this.position.Y);
																				int arg_103F_1 = this.width;
																				int arg_103F_2 = this.height;
																				int arg_103F_3 = 31;
																				float arg_103F_4 = 0f;
																				float arg_103F_5 = 0f;
																				int arg_103F_6 = 100;
																				Color newColor =  new Color();
																				int num21 = Dust.NewDust(arg_103F_0, arg_103F_1, arg_103F_2, arg_103F_3, arg_103F_4, arg_103F_5, arg_103F_6, newColor, 2f);
																				Dust expr_104E = Main.dust[num21];
																				expr_104E.velocity *= 1.4f;
																			}
																			for (int num22 = 0; num22 < 80; num22++)
																			{
																				Vector2 arg_10BA_0 = new Vector2(this.position.X, this.position.Y);
																				int arg_10BA_1 = this.width;
																				int arg_10BA_2 = this.height;
																				int arg_10BA_3 = 6;
																				float arg_10BA_4 = 0f;
																				float arg_10BA_5 = 0f;
																				int arg_10BA_6 = 100;
																				Color newColor =  new Color();
																				int num23 = Dust.NewDust(arg_10BA_0, arg_10BA_1, arg_10BA_2, arg_10BA_3, arg_10BA_4, arg_10BA_5, arg_10BA_6, newColor, 3f);
																				Main.dust[num23].noGravity = true;
																				Dust expr_10D7 = Main.dust[num23];
																				expr_10D7.velocity *= 5f;
																				Vector2 arg_112F_0 = new Vector2(this.position.X, this.position.Y);
																				int arg_112F_1 = this.width;
																				int arg_112F_2 = this.height;
																				int arg_112F_3 = 6;
																				float arg_112F_4 = 0f;
																				float arg_112F_5 = 0f;
																				int arg_112F_6 = 100;
																				newColor =  new Color();
																				num23 = Dust.NewDust(arg_112F_0, arg_112F_1, arg_112F_2, arg_112F_3, arg_112F_4, arg_112F_5, arg_112F_6, newColor, 2f);
																				Dust expr_113E = Main.dust[num23];
																				expr_113E.velocity *= 3f;
																			}
																			for (int num24 = 0; num24 < 2; num24++)
																			{
																				Vector2 arg_11BD_0 = new Vector2(this.position.X + (float)(this.width / 2) - 24f, this.position.Y + (float)(this.height / 2) - 24f);
                                                                                Vector2 vector = default(Vector2);
																				int num25 = Gore.NewGore(arg_11BD_0, vector, Main.rand.Next(61, 64));
																				Main.gore[num25].scale = 1.5f;
																				Gore expr_11E3_cp_0 = Main.gore[num25];
																				expr_11E3_cp_0.velocity.X = expr_11E3_cp_0.velocity.X + 1.5f;
																				Gore expr_1201_cp_0 = Main.gore[num25];
																				expr_1201_cp_0.velocity.Y = expr_1201_cp_0.velocity.Y + 1.5f;
																				Vector2 arg_1265_0 = new Vector2(this.position.X + (float)(this.width / 2) - 24f, this.position.Y + (float)(this.height / 2) - 24f);
                                                                                vector = default(Vector2);
																				num25 = Gore.NewGore(arg_1265_0, vector, Main.rand.Next(61, 64));
																				Main.gore[num25].scale = 1.5f;
																				Gore expr_128B_cp_0 = Main.gore[num25];
																				expr_128B_cp_0.velocity.X = expr_128B_cp_0.velocity.X - 1.5f;
																				Gore expr_12A9_cp_0 = Main.gore[num25];
																				expr_12A9_cp_0.velocity.Y = expr_12A9_cp_0.velocity.Y + 1.5f;
																				Vector2 arg_130D_0 = new Vector2(this.position.X + (float)(this.width / 2) - 24f, this.position.Y + (float)(this.height / 2) - 24f);
                                                                                vector = default(Vector2);
																				num25 = Gore.NewGore(arg_130D_0, vector, Main.rand.Next(61, 64));
																				Main.gore[num25].scale = 1.5f;
																				Gore expr_1333_cp_0 = Main.gore[num25];
																				expr_1333_cp_0.velocity.X = expr_1333_cp_0.velocity.X + 1.5f;
																				Gore expr_1351_cp_0 = Main.gore[num25];
																				expr_1351_cp_0.velocity.Y = expr_1351_cp_0.velocity.Y - 1.5f;
																				Vector2 arg_13B5_0 = new Vector2(this.position.X + (float)(this.width / 2) - 24f, this.position.Y + (float)(this.height / 2) - 24f);
                                                                                vector = default(Vector2);
																				num25 = Gore.NewGore(arg_13B5_0, vector, Main.rand.Next(61, 64));
																				Main.gore[num25].scale = 1.5f;
																				Gore expr_13DB_cp_0 = Main.gore[num25];
																				expr_13DB_cp_0.velocity.X = expr_13DB_cp_0.velocity.X - 1.5f;
																				Gore expr_13F9_cp_0 = Main.gore[num25];
																				expr_13F9_cp_0.velocity.Y = expr_13F9_cp_0.velocity.Y - 1.5f;
																			}
																			this.position.X = this.position.X + (float)(this.width / 2);
																			this.position.Y = this.position.Y + (float)(this.height / 2);
																			this.width = 10;
																			this.height = 10;
																			this.position.X = this.position.X - (float)(this.width / 2);
																			this.position.Y = this.position.Y - (float)(this.height / 2);
																		}
																	}
																}
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			if (this.owner == Main.myPlayer)
			{
				if (this.type == 28 || this.type == 29 || this.type == 37)
				{
					int num26 = 3;
					if (this.type == 29)
					{
						num26 = 7;
					}
					int num27 = (int)(this.position.X / 16f - (float)num26);
					int num28 = (int)(this.position.X / 16f + (float)num26);
					int num29 = (int)(this.position.Y / 16f - (float)num26);
					int num30 = (int)(this.position.Y / 16f + (float)num26);
					if (num27 < 0)
					{
						num27 = 0;
					}
					if (num28 > Main.maxTilesX)
					{
						num28 = Main.maxTilesX;
					}
					if (num29 < 0)
					{
						num29 = 0;
					}
					if (num30 > Main.maxTilesY)
					{
						num30 = Main.maxTilesY;
					}
					bool flag = false;
					for (int num31 = num27; num31 <= num28; num31++)
					{
						for (int num32 = num29; num32 <= num30; num32++)
						{
							float num33 = Math.Abs((float)num31 - this.position.X / 16f);
							float num34 = Math.Abs((float)num32 - this.position.Y / 16f);
							double num35 = Math.Sqrt((double)(num33 * num33 + num34 * num34));
							if (num35 < (double)num26 && Main.tile[num31, num32] != null && Main.tile[num31, num32].wall == 0)
							{
								flag = true;
								break;
							}
						}
					}
					for (int num36 = num27; num36 <= num28; num36++)
					{
						for (int num37 = num29; num37 <= num30; num37++)
						{
							float num38 = Math.Abs((float)num36 - this.position.X / 16f);
							float num39 = Math.Abs((float)num37 - this.position.Y / 16f);
							double num40 = Math.Sqrt((double)(num38 * num38 + num39 * num39));
							if (num40 < (double)num26)
							{
								bool flag2 = true;
								if (Main.tile[num36, num37] != null && Main.tile[num36, num37].active)
								{
									flag2 = false;
									if (this.type == 28 || this.type == 37)
									{
										if (!Main.tileSolid[(int)Main.tile[num36, num37].type] || Main.tileSolidTop[(int)Main.tile[num36, num37].type] || Main.tile[num36, num37].type == 0 || Main.tile[num36, num37].type == 1 || Main.tile[num36, num37].type == 2 || Main.tile[num36, num37].type == 23 || Main.tile[num36, num37].type == 30 || Main.tile[num36, num37].type == 40 || Main.tile[num36, num37].type == 6 || Main.tile[num36, num37].type == 7 || Main.tile[num36, num37].type == 8 || Main.tile[num36, num37].type == 9 || Main.tile[num36, num37].type == 10 || Main.tile[num36, num37].type == 53 || Main.tile[num36, num37].type == 54 || Main.tile[num36, num37].type == 57 || Main.tile[num36, num37].type == 59 || Main.tile[num36, num37].type == 60 || Main.tile[num36, num37].type == 63 || Main.tile[num36, num37].type == 64 || Main.tile[num36, num37].type == 65 || Main.tile[num36, num37].type == 66 || Main.tile[num36, num37].type == 67 || Main.tile[num36, num37].type == 68 || Main.tile[num36, num37].type == 70 || Main.tile[num36, num37].type == 37)
										{
											flag2 = true;
										}
									}
									else
									{
										if (this.type == 29)
										{
											flag2 = true;
										}
									}
									if (Main.tileDungeon[(int)Main.tile[num36, num37].type] || Main.tile[num36, num37].type == 26 || Main.tile[num36, num37].type == 58 || Main.tile[num36, num37].type == 21)
									{
										flag2 = false;
									}
									if (flag2)
									{
										WorldGen.KillTile(num36, num37, false, false, false);
										if (!Main.tile[num36, num37].active && Main.netMode == 1)
										{
											NetMessage.SendData(17, -1, -1, "", 0, (float)num36, (float)num37, 0f);
										}
									}
								}
								if (flag2 && Main.tile[num36, num37] != null && Main.tile[num36, num37].wall > 0 && flag)
								{
									WorldGen.KillWall(num36, num37, false);
									if (Main.tile[num36, num37].wall == 0 && Main.netMode == 1)
									{
										NetMessage.SendData(17, -1, -1, "", 2, (float)num36, (float)num37, 0f);
									}
								}
							}
						}
					}
				}
				if (Main.netMode != 0)
				{
					NetMessage.SendData(29, -1, -1, "", this.identity, (float)this.owner, 0f, 0f);
				}
				int num41 = -1;
				if (this.aiStyle == 10)
				{
					int num42 = (int)(this.position.X + (float)(this.width / 2)) / 16;
					int num43 = (int)(this.position.Y + (float)(this.width / 2)) / 16;
					int num44 = 0;
					int num45 = 2;
					if (this.type == 31)
					{
						num44 = 53;
						num45 = 169;
					}
					if (!Main.tile[num42, num43].active)
					{
						WorldGen.PlaceTile(num42, num43, num44, false, true, -1);
						if (Main.tile[num42, num43].active && (int)Main.tile[num42, num43].type == num44)
						{
							NetMessage.SendData(17, -1, -1, "", 1, (float)num42, (float)num43, (float)num44);
						}
						else
						{
							num41 = Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, num45, 1, false);
						}
					}
					else
					{
						num41 = Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, num45, 1, false);
					}
				}
				if (this.type == 1 && Main.rand.Next(3) < 2)
				{
					num41 = Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 40, 1, false);
				}
				if (this.type == 2 && Main.rand.Next(5) < 4)
				{
					if (Main.rand.Next(4) == 0)
					{
						num41 = Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 41, 1, false);
					}
					else
					{
						num41 = Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 40, 1, false);
					}
				}
				if (this.type == 3 && Main.rand.Next(5) < 4)
				{
					num41 = Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 42, 1, false);
				}
				if (this.type == 4 && Main.rand.Next(5) < 4)
				{
					num41 = Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 47, 1, false);
				}
				if (this.type == 12 && this.damage > 100)
				{
					num41 = Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 75, 1, false);
				}
				if (this.type == 21 && Main.rand.Next(5) < 4)
				{
					num41 = Item.NewItem((int)this.position.X, (int)this.position.Y, this.width, this.height, 154, 1, false);
				}
				if (Main.netMode == 1 && num41 >= 0)
				{
					NetMessage.SendData(21, -1, -1, "", num41, 0f, 0f, 0f);
				}
			}
			this.active = false;
		}
		
        public Color GetAlpha(Color newColor)
		{
			int r;
			int g;
			int b;
			if (this.type == 9 || this.type == 15 || this.type == 34)
			{
				r = (int)newColor.R - this.alpha / 3;
				g = (int)newColor.G - this.alpha / 3;
				b = (int)newColor.B - this.alpha / 3;
			}
			else
			{
				if (this.type == 16 || this.type == 18)
				{
					r = (int)newColor.R;
					g = (int)newColor.G;
					b = (int)newColor.B;
				}
				else
				{
					r = (int)newColor.R - this.alpha;
					g = (int)newColor.G - this.alpha;
					b = (int)newColor.B - this.alpha;
				}
			}
			int num = (int)newColor.A - this.alpha;
			if (num < 0)
			{
				num = 0;
			}
			if (num > 255)
			{
				num = 255;
			}
			return new Color(r, g, b, num);
		}
	}
}
