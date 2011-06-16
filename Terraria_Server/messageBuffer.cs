using System;
using System.Text;
using Terraria_Server.Events;
using Terraria_Server.Plugin;

namespace Terraria_Server
{
	public class messageBuffer
	{
		public const int readBufferMax = 65535;
		public const int writeBufferMax = 65535;
		public bool broadcast;
		public byte[] readBuffer = new byte[65535];
		public byte[] writeBuffer = new byte[65535];
		public bool writeLocked;
		public int messageLength;
		public int totalData;
		public int whoAmI;
		public int spamCount;
		public int maxSpam;
		public bool checkBytes;
		
        public void Reset()
		{
			this.writeBuffer = new byte[65535];
			this.writeLocked = false;
			this.messageLength = 0;
			this.totalData = 0;
			this.spamCount = 0;
			this.broadcast = false;
			this.checkBytes = false;
		}
		
        public void GetData(int start, int length)
		{
			if (this.whoAmI < 256)
			{
				NetPlay.serverSock[this.whoAmI].timeOut = 0;
			}
			else
			{
				NetPlay.clientSock.timeOut = 0;
			}
			int num = 0;
			num = start + 1;
			byte b = this.readBuffer[start];
			if (Main.netMode == 1 && NetPlay.clientSock.statusMax > 0)
			{
				NetPlay.clientSock.statusCount++;
			}
			if (Main.verboseNetplay)
			{
				for (int i = start; i < start + length; i++)
				{
				}
				for (int j = start; j < start + length; j++)
				{
					byte arg_85_0 = this.readBuffer[j];
				}
			}
			if (Main.netMode == 2 && b != 38 && NetPlay.serverSock[this.whoAmI].state == -1)
			{
				NetMessage.SendData(2, this.whoAmI, -1, "Incorrect password.", 0, 0f, 0f, 0f);
				return;
			}
			if (b == 1 && Main.netMode == 2)
			{
                LoginEvent Event = new LoginEvent();
                Event.setSocket(NetPlay.serverSock[this.whoAmI]);
                Event.setSender(Main.player[this.whoAmI]);
                Program.server.getPluginManager().processHook(Plugin.Hooks.PLAYER_PRELOGIN, Event);
                if (Event.getCancelled())
                {
                    NetMessage.SendData(2, this.whoAmI, -1, "Disconnected By Server.", 0, 0f, 0f, 0f);
                    return;
                }

				if (Main.dedServ && Program.server.getBanList().containsException(NetPlay.serverSock[this.whoAmI].tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0]))
				{
					NetMessage.SendData(2, this.whoAmI, -1, "You are banned from this Server.", 0, 0f, 0f, 0f);
					return;
				}

                if(Program.properties.isUsingWhiteList() && !Program.server.getWhiteList().containsException(NetPlay.serverSock[this.whoAmI].tcpClient.Client.RemoteEndPoint.ToString().Split(':')[0])) {
                    NetMessage.SendData(2, this.whoAmI, -1, "You are not on the WhiteList.", 0, 0f, 0f, 0f);
					return;
                }

				if (NetPlay.serverSock[this.whoAmI].state == 0)
				{
                    string version = Encoding.ASCII.GetString(this.readBuffer, start + 1, length - 1);
					if (!(version == "Terraria" + Statics.currentRelease))
					{
						NetMessage.SendData(2, this.whoAmI, -1, "You are not using the same version as this Server.", 0, 0f, 0f, 0f);
						return;
					}
					if (NetPlay.password == null || NetPlay.password == "")
					{
						NetPlay.serverSock[this.whoAmI].state = 1;
						NetMessage.SendData(3, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
						return;
					}
					NetPlay.serverSock[this.whoAmI].state = -1;
					NetMessage.SendData(37, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
					return;
				}
			}
			else
			{
				if (b == 2 && Main.netMode == 1)
				{
					NetPlay.disconnect = true;
					Main.statusText = Encoding.ASCII.GetString(this.readBuffer, start + 1, length - 1);
					return;
				}
				if (b == 3 && Main.netMode == 1)
				{
					if (NetPlay.clientSock.state == 1)
					{
						NetPlay.clientSock.state = 2;
					}
					int num2 = (int)this.readBuffer[start + 1];
					if (num2 != Main.myPlayer)
					{
						Main.player[num2] = (Player)Main.player[Main.myPlayer].Clone();
						Main.player[Main.myPlayer] = new Player();
						Main.player[num2].whoAmi = num2;
						Main.myPlayer = num2;
					}
					NetMessage.SendData(4, -1, -1, Main.player[Main.myPlayer].name, Main.myPlayer, 0f, 0f, 0f);
					NetMessage.SendData(16, -1, -1, "", Main.myPlayer, 0f, 0f, 0f);
					NetMessage.SendData(42, -1, -1, "", Main.myPlayer, 0f, 0f, 0f);
					for (int k = 0; k < 44; k++)
					{
						NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].inventory[k].name, Main.myPlayer, (float)k, 0f, 0f);
					}
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[0].name, Main.myPlayer, 44f, 0f, 0f);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[1].name, Main.myPlayer, 45f, 0f, 0f);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[2].name, Main.myPlayer, 46f, 0f, 0f);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[3].name, Main.myPlayer, 47f, 0f, 0f);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[4].name, Main.myPlayer, 48f, 0f, 0f);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[5].name, Main.myPlayer, 49f, 0f, 0f);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[6].name, Main.myPlayer, 50f, 0f, 0f);
					NetMessage.SendData(5, -1, -1, Main.player[Main.myPlayer].armor[7].name, Main.myPlayer, 51f, 0f, 0f);
					NetMessage.SendData(6, -1, -1, "", 0, 0f, 0f, 0f);
					if (NetPlay.clientSock.state == 2)
					{
						NetPlay.clientSock.state = 3;
						return;
					}
				}
				else
				{
					if (b == 4)
					{
						//bool flag = false;
						int num3 = (int)this.readBuffer[start + 1];
						if (Main.netMode == 2)
						{
							num3 = this.whoAmI;
						}
						if (num3 == Main.myPlayer)
						{
							return;
						}
						int hair = (int)this.readBuffer[start + 2];
						Main.player[num3].hair = hair;
						Main.player[num3].whoAmi = num3;
						num += 2;
						Main.player[num3].hairColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].hairColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].hairColor.B = this.readBuffer[num];
						num++;
						Main.player[num3].skinColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].skinColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].skinColor.B = this.readBuffer[num];
						num++;
						Main.player[num3].eyeColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].eyeColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].eyeColor.B = this.readBuffer[num];
						num++;
						Main.player[num3].shirtColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].shirtColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].shirtColor.B = this.readBuffer[num];
						num++;
						Main.player[num3].underShirtColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].underShirtColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].underShirtColor.B = this.readBuffer[num];
						num++;
						Main.player[num3].pantsColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].pantsColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].pantsColor.B = this.readBuffer[num];
						num++;
						Main.player[num3].shoeColor.R = this.readBuffer[num];
						num++;
						Main.player[num3].shoeColor.G = this.readBuffer[num];
						num++;
						Main.player[num3].shoeColor.B = this.readBuffer[num];
						num++;
						string string2 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
						Main.player[num3].name = string2;
						if (Main.netMode == 2)
                        {
                            //if (NetPlay.serverSock[this.whoAmI].state < 10)
                            //{
                            //    for (int l = 0; l < 255; l++)
                            //    {
                            //        if (l != num3 && string2 == Main.player[l].name && NetPlay.serverSock[l].active)
                            //        {
                            //            flag = true;
                            //        }
                            //    }
                            //}
                            //if (flag)
                            //{
                            //    NetMessage.SendData(2, this.whoAmI, -1, string2 + " is already on this server.", 0, 0f, 0f, 0f);
                            //    return;
                            //}
                            if (NetPlay.serverSock[this.whoAmI].state < 10)
                            {
                                for (int l = 0; l < 255; l++)
                                {
                                    if (l != num3 && string2 == Main.player[l].name && NetPlay.serverSock[l].active)
                                    {
                                        NetMessage.SendData(2, Main.player[l].whoAmi, -1, string2 + " Logged in from a Different Location.", 0, 0f, 0f, 0f);
                                    }
                                }
                            }
                            if (string2.Length > 20)
                            {
                                NetMessage.SendData(2, this.whoAmI, -1, "Name is too long.", 0, 0f, 0f, 0f);
                            }
							NetPlay.serverSock[this.whoAmI].oldName = string2;
							NetPlay.serverSock[this.whoAmI].name = string2;
							NetMessage.SendData(4, -1, this.whoAmI, string2, num3, 0f, 0f, 0f);
							return;
						}
					}
					else
					{
                        if (b == 5)
                        {
                            int num2 = (int)this.readBuffer[start + 1];
                            if (Main.netMode == 2)
                            {
                                num2 = this.whoAmI;
                            }
                            if (num2 != Main.myPlayer)
                            {
                                lock (Main.player[num2])
                                {
                                    int num3 = (int)this.readBuffer[start + 2];
                                    int stack = (int)this.readBuffer[start + 3];
                                    string string3 = Encoding.ASCII.GetString(this.readBuffer, start + 4, length - 4);
                                    if (num3 < 44)
                                    {
                                        Main.player[num2].inventory[num3] = new Item();
                                        Main.player[num2].inventory[num3].SetDefaults(string3);
                                        Main.player[num2].inventory[num3].stack = stack;
                                    }
                                    else
                                    {
                                        Main.player[num2].armor[num3 - 44] = new Item();
                                        Main.player[num2].armor[num3 - 44].SetDefaults(string3);
                                        Main.player[num2].armor[num3 - 44].stack = stack;
                                    }
                                    if (Main.netMode == 2 && num2 == this.whoAmI)
                                    {
                                        NetMessage.SendData(5, -1, this.whoAmI, string3, num2, (float)num3, 0f, 0f);
                                    }
                                }
                            }
                        }
						else
						{
							if (b == 6)
							{
								if (Main.netMode == 2)
								{
									if (NetPlay.serverSock[this.whoAmI].state == 1)
									{
										NetPlay.serverSock[this.whoAmI].state = 2;
									}
									NetMessage.SendData(7, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
									return;
								}
							}
							else
							{
								if (b == 7)
								{
									if (Main.netMode == 1)
									{
										Main.time = (double)BitConverter.ToInt32(this.readBuffer, num);
										num += 4;
										Main.dayTime = false;
										if (this.readBuffer[num] == 1)
										{
											Main.dayTime = true;
										}
										num++;
										Main.moonPhase = (int)this.readBuffer[num];
										num++;
										int num6 = (int)this.readBuffer[num];
										num++;
										if (num6 == 1)
										{
											Main.bloodMoon = true;
										}
										else
										{
											Main.bloodMoon = false;
										}
										Main.maxTilesX = BitConverter.ToInt32(this.readBuffer, num);
										num += 4;
										Main.maxTilesY = BitConverter.ToInt32(this.readBuffer, num);
										num += 4;
										Main.spawnTileX = BitConverter.ToInt32(this.readBuffer, num);
										num += 4;
										Main.spawnTileY = BitConverter.ToInt32(this.readBuffer, num);
										num += 4;
										Main.worldSurface = (double)BitConverter.ToInt32(this.readBuffer, num);
										num += 4;
										Main.rockLayer = (double)BitConverter.ToInt32(this.readBuffer, num);
										num += 4;
										Main.worldID = BitConverter.ToInt32(this.readBuffer, num);
										num += 4;
										Main.worldName = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
										if (NetPlay.clientSock.state == 3)
										{
											NetPlay.clientSock.state = 4;
											return;
										}
									}
								}
								else
								{
									if (b == 8)
									{
										if (Main.netMode == 2)
										{
											int num7 = BitConverter.ToInt32(this.readBuffer, num);
											num += 4;
											int num8 = BitConverter.ToInt32(this.readBuffer, num);
											num += 4;
											bool flag2 = true;
											if (num7 == -1 || num8 == -1)
											{
												flag2 = false;
											}
											else
											{
												if (num7 < 10 || num7 > Main.maxTilesX - 10)
												{
													flag2 = false;
												}
												else
												{
													if (num8 < 10 || num8 > Main.maxTilesY - 10)
													{
														flag2 = false;
													}
												}
											}
											int num9 = 1350;
											if (flag2)
											{
												num9 *= 2;
											}
											if (NetPlay.serverSock[this.whoAmI].state == 2)
											{
												NetPlay.serverSock[this.whoAmI].state = 3;
											}
											NetMessage.SendData(9, this.whoAmI, -1, "Receiving tile data", num9, 0f, 0f, 0f);
											NetPlay.serverSock[this.whoAmI].statusText2 = "is receiving tile data";
											NetPlay.serverSock[this.whoAmI].statusMax += num9;
											int sectionX = NetPlay.GetSectionX(Main.spawnTileX);
											int sectionY = NetPlay.GetSectionY(Main.spawnTileY);
											for (int m = sectionX - 2; m < sectionX + 3; m++)
											{
												for (int n = sectionY - 1; n < sectionY + 2; n++)
												{
													NetMessage.SendSection(this.whoAmI, m, n);
												}
											}
											if (flag2)
											{
												num7 = NetPlay.GetSectionX(num7);
												num8 = NetPlay.GetSectionY(num8);
												for (int num10 = num7 - 2; num10 < num7 + 3; num10++)
												{
													for (int num11 = num8 - 1; num11 < num8 + 2; num11++)
													{
														NetMessage.SendSection(this.whoAmI, num10, num11);
													}
												}
												NetMessage.SendData(11, this.whoAmI, -1, "", num7 - 2, (float)(num8 - 1), (float)(num7 + 2), (float)(num8 + 1));
											}
											NetMessage.SendData(11, this.whoAmI, -1, "", sectionX - 2, (float)(sectionY - 1), (float)(sectionX + 2), (float)(sectionY + 1));
											NetMessage.SendData(49, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
											for (int num12 = 0; num12 < 200; num12++)
											{
												if (Main.item[num12].active)
												{
													NetMessage.SendData(21, this.whoAmI, -1, "", num12, 0f, 0f, 0f);
													NetMessage.SendData(22, this.whoAmI, -1, "", num12, 0f, 0f, 0f);
												}
											}
											for (int num13 = 0; num13 < 1000; num13++)
											{
												if (Main.npc[num13].active)
												{
													NetMessage.SendData(23, this.whoAmI, -1, "", num13, 0f, 0f, 0f);
												}
											}
											return;
										}
									}
									else
									{
										if (b == 9)
										{
											if (Main.netMode == 1)
											{
												int num14 = BitConverter.ToInt32(this.readBuffer, start + 1);
												string string4 = Encoding.ASCII.GetString(this.readBuffer, start + 5, length - 5);
												NetPlay.clientSock.statusMax += num14;
												NetPlay.clientSock.statusText = string4;
												return;
											}
										}
										else
										{
											if (b == 10)
											{
												short num15 = BitConverter.ToInt16(this.readBuffer, start + 1);
												int num16 = BitConverter.ToInt32(this.readBuffer, start + 3);
												int num17 = BitConverter.ToInt32(this.readBuffer, start + 7);
												num = start + 11;
												for (int num18 = num16; num18 < num16 + (int)num15; num18++)
												{
													if (Main.tile[num18, num17] == null)
													{
														Main.tile[num18, num17] = new Tile();
													}
													byte b2 = this.readBuffer[num];
													num++;
													bool active = Main.tile[num18, num17].active;
													if ((b2 & 1) == 1)
													{
														Main.tile[num18, num17].active = true;
													}
													else
													{
														Main.tile[num18, num17].active = false;
													}
													if ((b2 & 2) == 2)
													{
														Main.tile[num18, num17].lighted = true;
													}
													if ((b2 & 4) == 4)
													{
														Main.tile[num18, num17].wall = 1;
													}
													else
													{
														Main.tile[num18, num17].wall = 0;
													}
													if ((b2 & 8) == 8)
													{
														Main.tile[num18, num17].liquid = 1;
													}
													else
													{
														Main.tile[num18, num17].liquid = 0;
													}
													if (Main.tile[num18, num17].active)
													{
														int type = (int)Main.tile[num18, num17].type;
														Main.tile[num18, num17].type = this.readBuffer[num];
														num++;
														if (Main.tileFrameImportant[(int)Main.tile[num18, num17].type])
														{
															Main.tile[num18, num17].frameX = BitConverter.ToInt16(this.readBuffer, num);
															num += 2;
															Main.tile[num18, num17].frameY = BitConverter.ToInt16(this.readBuffer, num);
															num += 2;
														}
														else
														{
															if (!active || (int)Main.tile[num18, num17].type != type)
															{
																Main.tile[num18, num17].frameX = -1;
																Main.tile[num18, num17].frameY = -1;
															}
														}
													}
													if (Main.tile[num18, num17].wall > 0)
													{
														Main.tile[num18, num17].wall = this.readBuffer[num];
														num++;
													}
													if (Main.tile[num18, num17].liquid > 0)
													{
														Main.tile[num18, num17].liquid = this.readBuffer[num];
														num++;
														byte b3 = this.readBuffer[num];
														num++;
														if (b3 == 1)
														{
															Main.tile[num18, num17].lava = true;
														}
														else
														{
															Main.tile[num18, num17].lava = false;
														}
													}
												}
												if (Main.netMode == 2)
												{
													NetMessage.SendData((int)b, -1, this.whoAmI, "", (int)num15, (float)num16, (float)num17, 0f);
													return;
												}
											}
											else
											{
												if (b == 11)
												{
													if (Main.netMode == 1)
													{
														int startX = (int)BitConverter.ToInt16(this.readBuffer, num);
														num += 4;
														int startY = (int)BitConverter.ToInt16(this.readBuffer, num);
														num += 4;
														int endX = (int)BitConverter.ToInt16(this.readBuffer, num);
														num += 4;
														int endY = (int)BitConverter.ToInt16(this.readBuffer, num);
														num += 4;
														WorldGen.SectionTileFrame(startX, startY, endX, endY);
														return;
													}
												}
												else
												{
													if (b == 12)
													{
														int num19 = (int)this.readBuffer[num];
														num++;
														Main.player[num19].SpawnX = BitConverter.ToInt32(this.readBuffer, num);
														num += 4;
														Main.player[num19].SpawnY = BitConverter.ToInt32(this.readBuffer, num);
														num += 4;
														Main.player[num19].Spawn();
														if (Main.netMode == 2 && NetPlay.serverSock[this.whoAmI].state >= 3)
														{
															NetMessage.buffer[this.whoAmI].broadcast = true;
															NetMessage.SendData(12, -1, this.whoAmI, "", this.whoAmI, 0f, 0f, 0f);
															if (NetPlay.serverSock[this.whoAmI].state == 3)
															{
																NetPlay.serverSock[this.whoAmI].state = 10;
																NetMessage.greetPlayer(this.whoAmI);
																NetMessage.syncPlayers();
																return;
															}
														}
													}
													else
													{
														if (b == 13)
														{
															int num20 = (int)this.readBuffer[num];
															if (Main.netMode == 1 && !Main.player[num20].active)
															{
																NetMessage.SendData(15, -1, -1, "", 0, 0f, 0f, 0f);
															}
															num++;
															int num21 = (int)this.readBuffer[num];
															num++;
															int selectedItem = (int)this.readBuffer[num];
															num++;
															float x = BitConverter.ToSingle(this.readBuffer, num);
															num += 4;
															float num22 = BitConverter.ToSingle(this.readBuffer, num);
															num += 4;
															float x2 = BitConverter.ToSingle(this.readBuffer, num);
															num += 4;
															float y = BitConverter.ToSingle(this.readBuffer, num);
															num += 4;
															Main.player[num20].selectedItem = selectedItem;
															Main.player[num20].position.X = x;
															Main.player[num20].position.Y = num22;
															Main.player[num20].velocity.X = x2;
															Main.player[num20].velocity.Y = y;
															Main.player[num20].oldVelocity = Main.player[num20].velocity;
															Main.player[num20].fallStart = (int)(num22 / 16f);
															Main.player[num20].controlUp = false;
															Main.player[num20].controlDown = false;
															Main.player[num20].controlLeft = false;
															Main.player[num20].controlRight = false;
															Main.player[num20].controlJump = false;
															Main.player[num20].controlUseItem = false;
															Main.player[num20].direction = -1;
															if ((num21 & 1) == 1)
															{
																Main.player[num20].controlUp = true;
															}
															if ((num21 & 2) == 2)
															{
																Main.player[num20].controlDown = true;
															}
															if ((num21 & 4) == 4)
															{
																Main.player[num20].controlLeft = true;
															}
															if ((num21 & 8) == 8)
															{
																Main.player[num20].controlRight = true;
															}
															if ((num21 & 16) == 16)
															{
																Main.player[num20].controlJump = true;
															}
															if ((num21 & 32) == 32)
															{
																Main.player[num20].controlUseItem = true;
															}
															if ((num21 & 64) == 64)
															{
																Main.player[num20].direction = 1;
															}
															if (Main.netMode == 2 && NetPlay.serverSock[this.whoAmI].state == 10)
															{
																NetMessage.SendData(13, -1, this.whoAmI, "", num20, 0f, 0f, 0f);
																return;
															}
														}
														else
														{
															if (b == 14)
															{
																if (Main.netMode == 1)
																{
																	int num23 = (int)this.readBuffer[num];
																	num++;
																	int num24 = (int)this.readBuffer[num];
																	if (num24 == 1)
																	{
																		if (Main.player[num23].active)
																		{
																			Main.player[num23] = new Player();
																		}
																		Main.player[num23].active = true;
																		return;
																	}
																	Main.player[num23].active = false;
																	return;
																}
															}
															else
															{
																if (b == 15)
																{
																	if (Main.netMode == 2)
																	{
																		NetMessage.syncPlayers();
																		return;
																	}
																}
																else
																{
																	if (b == 16)
																	{
																		int num25 = (int)this.readBuffer[num];
																		num++;
																		int statLife = (int)BitConverter.ToInt16(this.readBuffer, num);
																		num += 2;
																		int statLifeMax = (int)BitConverter.ToInt16(this.readBuffer, num);
																		if (Main.netMode == 2)
																		{
																			num25 = this.whoAmI;
																		}
																		Main.player[num25].statLife = statLife;
																		Main.player[num25].statLifeMax = statLifeMax;
																		if (Main.player[num25].statLife <= 0)
																		{
																			Main.player[num25].dead = true;
																		}
																		if (Main.netMode == 2)
																		{
																			NetMessage.SendData(16, -1, this.whoAmI, "", num25, 0f, 0f, 0f);
																			return;
																		}
																	}
																	else
																	{
																		if (b == 17)
																		{
																			byte b4 = this.readBuffer[num];
																			num++;
																			int num26 = BitConverter.ToInt32(this.readBuffer, num);
																			num += 4;
																			int num27 = BitConverter.ToInt32(this.readBuffer, num);
																			num += 4;
																			byte b5 = this.readBuffer[num];
																			bool fail = false;
																			if (b5 == 1)
																			{
																				fail = true;
																			}
                                                                            Tile tile = new Tile();


                                                                            if (Main.tile[num26, num27] != null)
                                                                            {
                                                                                tile = WorldGen.cloneTile(Main.tile[num26, num27]);
                                                                            }
                                                                            if (Main.tile[num26, num27] == null)
                                                                            {
                                                                                Main.tile[num26, num27] = new Tile();
                                                                            }
																			if (Main.netMode == 2 && !NetPlay.serverSock[this.whoAmI].tileSection[NetPlay.GetSectionX(num26), NetPlay.GetSectionY(num27)])
																			{
																				fail = true;
																			}

                                                                            tile.tileX = num26;
                                                                            tile.tileY = num27;

                                                                            TileBreakEvent Event = new TileBreakEvent(); 
                                                                            Event.setSender(Main.player[this.whoAmI]);
                                                                            Event.setTile(tile);
                                                                            Event.setTileType(b5);
                                                                            Event.setTilePos(new Vector2(num26, num27));
                                                                            Program.server.getPluginManager().processHook(Hooks.TILE_BREAK, Event);
                                                                            if (Event.getCancelled())
                                                                            {
                                                                                NetMessage.SendTileSquare(this.whoAmI, num26, num27, 1);
                                                                                return;
                                                                            }

																			if (b4 == 0)
																			{
																				WorldGen.KillTile(num26, num27, fail, false, false);
																			}
																			else
																			{
																				if (b4 == 1)
																				{
																					WorldGen.PlaceTile(num26, num27, (int)b5, false, true, -1);
																				}
																				else
																				{
																					if (b4 == 2)
																					{
																						WorldGen.KillWall(num26, num27, fail);
																					}
																					else
																					{
																						if (b4 == 3)
																						{
																							WorldGen.PlaceWall(num26, num27, (int)b5, false);
																						}
																						else
																						{
																							if (b4 == 4)
																							{
																								WorldGen.KillTile(num26, num27, fail, false, true);
																							}
																						}
																					}
																				}
																			}
																			if (Main.netMode == 2)
																			{
																				NetMessage.SendData(17, -1, this.whoAmI, "", (int)b4, (float)num26, (float)num27, (float)b5);
																				if (b4 == 1 && b5 == 53)
																				{
																					NetMessage.SendTileSquare(-1, num26, num27, 1);
																					return;
																				}
																			}
																		}
																		else
																		{
																			if (b == 18)
																			{
																				if (Main.netMode == 1)
																				{
																					byte b6 = this.readBuffer[num];
																					num++;
																					int num28 = BitConverter.ToInt32(this.readBuffer, num);
																					num += 4;
																					short sunModY = BitConverter.ToInt16(this.readBuffer, num);
																					num += 2;
																					short moonModY = BitConverter.ToInt16(this.readBuffer, num);
																					num += 2;
																					if (b6 == 1)
																					{
																						Main.dayTime = true;
																					}
																					else
																					{
																						Main.dayTime = false;
																					}
																					Main.time = (double)num28;
																					Main.sunModY = sunModY;
																					Main.moonModY = moonModY;
																					if (Main.netMode == 2)
																					{
																						NetMessage.SendData(18, -1, this.whoAmI, "", 0, 0f, 0f, 0f);
																						return;
																					}
																				}
																			}
																			else
																			{
																				if (b == 19)
																				{
																					byte b7 = this.readBuffer[num];
																					num++;
																					int num29 = BitConverter.ToInt32(this.readBuffer, num);
																					num += 4;
																					int num30 = BitConverter.ToInt32(this.readBuffer, num);
																					num += 4;
																					int num31 = (int)this.readBuffer[num];
																					int direction = 0;
																					if (num31 == 0)
																					{
																						direction = -1;
																					}
																					if (b7 == 0)
																					{
																						WorldGen.OpenDoor(num29, num30, direction);
																					}
																					else
																					{
																						if (b7 == 1)
																						{
																							WorldGen.CloseDoor(num29, num30, true);
																						}
																					}
																					if (Main.netMode == 2)
																					{
																						NetMessage.SendData(19, -1, this.whoAmI, "", (int)b7, (float)num29, (float)num30, (float)num31);
																						return;
																					}
																				}
																				else
																				{
																					if (b == 20)
																					{
																						short num32 = BitConverter.ToInt16(this.readBuffer, start + 1);
																						int num33 = BitConverter.ToInt32(this.readBuffer, start + 3);
																						int num34 = BitConverter.ToInt32(this.readBuffer, start + 7);
																						num = start + 11;
																						for (int num35 = num33; num35 < num33 + (int)num32; num35++)
																						{
																							for (int num36 = num34; num36 < num34 + (int)num32; num36++)
																							{
																								if (Main.tile[num35, num36] == null)
																								{
																									Main.tile[num35, num36] = new Tile();
																								}
																								byte b8 = this.readBuffer[num];
																								num++;
																								bool active2 = Main.tile[num35, num36].active;
																								if ((b8 & 1) == 1)
																								{
																									Main.tile[num35, num36].active = true;
																								}
																								else
																								{
																									Main.tile[num35, num36].active = false;
																								}
																								if ((b8 & 2) == 2)
																								{
																									Main.tile[num35, num36].lighted = true;
																								}
																								if ((b8 & 4) == 4)
																								{
																									Main.tile[num35, num36].wall = 1;
																								}
																								else
																								{
																									Main.tile[num35, num36].wall = 0;
																								}
																								if ((b8 & 8) == 8)
																								{
																									Main.tile[num35, num36].liquid = 1;
																								}
																								else
																								{
																									Main.tile[num35, num36].liquid = 0;
																								}
																								if (Main.tile[num35, num36].active)
																								{
																									int type2 = (int)Main.tile[num35, num36].type;
																									Main.tile[num35, num36].type = this.readBuffer[num];
																									num++;
																									if (Main.tileFrameImportant[(int)Main.tile[num35, num36].type])
																									{
																										Main.tile[num35, num36].frameX = BitConverter.ToInt16(this.readBuffer, num);
																										num += 2;
																										Main.tile[num35, num36].frameY = BitConverter.ToInt16(this.readBuffer, num);
																										num += 2;
																									}
																									else
																									{
																										if (!active2 || (int)Main.tile[num35, num36].type != type2)
																										{
																											Main.tile[num35, num36].frameX = -1;
																											Main.tile[num35, num36].frameY = -1;
																										}
																									}
																								}
																								if (Main.tile[num35, num36].wall > 0)
																								{
																									Main.tile[num35, num36].wall = this.readBuffer[num];
																									num++;
																								}
																								if (Main.tile[num35, num36].liquid > 0)
																								{
																									Main.tile[num35, num36].liquid = this.readBuffer[num];
																									num++;
																									byte b9 = this.readBuffer[num];
																									num++;
																									if (b9 == 1)
																									{
																										Main.tile[num35, num36].lava = true;
																									}
																									else
																									{
																										Main.tile[num35, num36].lava = false;
																									}
																								}
																							}
																						}
																						WorldGen.RangeFrame(num33, num34, num33 + (int)num32, num34 + (int)num32);
																						if (Main.netMode == 2)
																						{
																							NetMessage.SendData((int)b, -1, this.whoAmI, "", (int)num32, (float)num33, (float)num34, 0f);
																							return;
																						}
																					}
																					else
																					{
																						if (b == 21)
																						{
																							short num37 = BitConverter.ToInt16(this.readBuffer, num);
																							num += 2;
																							float num38 = BitConverter.ToSingle(this.readBuffer, num);
																							num += 4;
																							float num39 = BitConverter.ToSingle(this.readBuffer, num);
																							num += 4;
																							float x3 = BitConverter.ToSingle(this.readBuffer, num);
																							num += 4;
																							float y2 = BitConverter.ToSingle(this.readBuffer, num);
																							num += 4;
																							byte stack2 = this.readBuffer[num];
																							num++;
																							string string5 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
																							if (Main.netMode == 1)
																							{
																								if (string5 == "0")
																								{
																									Main.item[(int)num37].active = false;
																									return;
																								}
																								Main.item[(int)num37].SetDefaults(string5);
																								Main.item[(int)num37].stack = (int)stack2;
																								Main.item[(int)num37].position.X = num38;
																								Main.item[(int)num37].position.Y = num39;
																								Main.item[(int)num37].velocity.X = x3;
																								Main.item[(int)num37].velocity.Y = y2;
																								Main.item[(int)num37].active = true;
																								Main.item[(int)num37].wet = Collision.WetCollision(Main.item[(int)num37].position, Main.item[(int)num37].width, Main.item[(int)num37].height);
																								return;
																							}
																							else
																							{
																								if (string5 == "0")
																								{
																									if (num37 < 200)
																									{
																										Main.item[(int)num37].active = false;
																										NetMessage.SendData(21, -1, -1, "", (int)num37, 0f, 0f, 0f);
																										return;
																									}
																								}
																								else
																								{
																									bool flag3 = false;
																									if (num37 == 200)
																									{
																										flag3 = true;
																									}
																									if (flag3)
																									{
																										Item item = new Item();
																										item.SetDefaults(string5);
																										num37 = (short)Item.NewItem((int)num38, (int)num39, item.width, item.height, item.type, (int)stack2, true);
																									}
																									Main.item[(int)num37].SetDefaults(string5);
																									Main.item[(int)num37].stack = (int)stack2;
																									Main.item[(int)num37].position.X = num38;
																									Main.item[(int)num37].position.Y = num39;
																									Main.item[(int)num37].velocity.X = x3;
																									Main.item[(int)num37].velocity.Y = y2;
																									Main.item[(int)num37].active = true;
																									Main.item[(int)num37].owner = Main.myPlayer;
																									if (flag3)
																									{
																										NetMessage.SendData(21, -1, -1, "", (int)num37, 0f, 0f, 0f);
																										Main.item[(int)num37].ownIgnore = this.whoAmI;
																										Main.item[(int)num37].ownTime = 100;
																										Main.item[(int)num37].FindOwner((int)num37);
																										return;
																									}
																									NetMessage.SendData(21, -1, this.whoAmI, "", (int)num37, 0f, 0f, 0f);
																									return;
																								}
																							}
																						}
																						else
																						{
																							if (b == 22)
																							{
																								short num40 = BitConverter.ToInt16(this.readBuffer, num);
																								num += 2;
																								byte b10 = this.readBuffer[num];
																								Main.item[(int)num40].owner = (int)b10;
																								if ((int)b10 == Main.myPlayer)
																								{
																									Main.item[(int)num40].keepTime = 15;
																								}
																								else
																								{
																									Main.item[(int)num40].keepTime = 0;
																								}
																								if (Main.netMode == 2)
																								{
																									Main.item[(int)num40].owner = 255;
																									Main.item[(int)num40].keepTime = 15;
																									NetMessage.SendData(22, -1, -1, "", (int)num40, 0f, 0f, 0f);
																									return;
																								}
																							}
																							else
																							{
																								if (b == 23)
																								{
																									short num41 = BitConverter.ToInt16(this.readBuffer, num);
																									num += 2;
																									float x4 = BitConverter.ToSingle(this.readBuffer, num);
																									num += 4;
																									float y3 = BitConverter.ToSingle(this.readBuffer, num);
																									num += 4;
																									float x5 = BitConverter.ToSingle(this.readBuffer, num);
																									num += 4;
																									float y4 = BitConverter.ToSingle(this.readBuffer, num);
																									num += 4;
																									int target = (int)BitConverter.ToInt16(this.readBuffer, num);
																									num += 2;
																									int direction2 = (int)(this.readBuffer[num] - 1);
																									num++;
																									byte arg_212E_0 = this.readBuffer[num];
																									num++;
																									int num42 = (int)BitConverter.ToInt16(this.readBuffer, num);
																									num += 2;
																									float[] array = new float[NPC.maxAI];
																									for (int num43 = 0; num43 < NPC.maxAI; num43++)
																									{
																										array[num43] = BitConverter.ToSingle(this.readBuffer, num);
																										num += 4;
																									}
																									string string6 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
																									if (!Main.npc[(int)num41].active || Main.npc[(int)num41].name != string6)
																									{
																										Main.npc[(int)num41].active = true;
																										Main.npc[(int)num41].SetDefaults(string6);
																									}
																									Main.npc[(int)num41].position.X = x4;
																									Main.npc[(int)num41].position.Y = y3;
																									Main.npc[(int)num41].velocity.X = x5;
																									Main.npc[(int)num41].velocity.Y = y4;
																									Main.npc[(int)num41].target = target;
																									Main.npc[(int)num41].direction = direction2;
																									Main.npc[(int)num41].life = num42;
																									if (num42 <= 0)
																									{
																										Main.npc[(int)num41].active = false;
																									}
																									for (int num44 = 0; num44 < NPC.maxAI; num44++)
																									{
																										Main.npc[(int)num41].ai[num44] = array[num44];
																									}
																									return;
																								}
																								if (b == 24)
																								{
																									short num45 = BitConverter.ToInt16(this.readBuffer, num);
																									num += 2;
																									byte b11 = this.readBuffer[num];
																									Main.npc[(int)num45].StrikeNPC(Main.player[(int)b11].inventory[Main.player[(int)b11].selectedItem].damage, Main.player[(int)b11].inventory[Main.player[(int)b11].selectedItem].knockBack, Main.player[(int)b11].direction);
																									if (Main.netMode == 2)
																									{
																										NetMessage.SendData(24, -1, this.whoAmI, "", (int)num45, (float)b11, 0f, 0f);
																										NetMessage.SendData(23, -1, -1, "", (int)num45, 0f, 0f, 0f);
																										return;
																									}
																								}
																								else
																								{
																									if (b == 25)
																									{
																										int num46 = (int)this.readBuffer[start + 1];
																										if (Main.netMode == 2)
																										{
																											num46 = this.whoAmI;
																										}
																										byte b12 = this.readBuffer[start + 2];
																										byte b13 = this.readBuffer[start + 3];
																										byte b14 = this.readBuffer[start + 4];
																										string string7 = Encoding.ASCII.GetString(this.readBuffer, start + 5, length - 5);
																										
																										if (Main.netMode == 2)
																										{
																											string Chat = string7.ToLower().Trim();

                                                                                                            if (Chat.Length > 0)
                                                                                                            {
                                                                                                                if(Chat.Substring(0, 1).Equals("/")) {
                                                                                                                    PlayerCommandEvent Event = new PlayerCommandEvent();
                                                                                                                    Event.setMessage(Chat);
                                                                                                                    Event.setSender(Main.player[this.whoAmI]);
                                                                                                                    Program.server.getPluginManager().processHook(Plugin.Hooks.PLAYER_COMMAND, Event);

                                                                                                                    if (Event.getCancelled())
                                                                                                                    {
                                                                                                                        return;
                                                                                                                    }

                                                                                                                    Console.WriteLine(Main.player[this.whoAmI].name + " Sent Command: " + string7);

                                                                                                                    Program.commandParser.parsePlayerCommand(Main.player[this.whoAmI], Chat);
                                                                                                                    return;
                                                                                                                } else {

                                                                                                                    PlayerChatEvent Event = new PlayerChatEvent();
                                                                                                                    Event.setMessage(Chat);
                                                                                                                    Event.setSender(Main.player[this.whoAmI]);
                                                                                                                    Program.server.getPluginManager().processHook(Plugin.Hooks.PLAYER_CHAT, Event);

                                                                                                                    if (Event.getCancelled())
                                                                                                                    {
                                                                                                                        return;
                                                                                                                    }
                                                                                                                }

                                                                                                                NetMessage.SendData(25, -1, -1, string7, num46, (float)b12, (float)b13, (float)b14);
                                                                                                                if (Main.dedServ)
                                                                                                                {
                                                                                                                    Console.WriteLine("<" + Main.player[this.whoAmI].name + "> " + string7);
                                                                                                                    return;
                                                                                                                }
                                                                                                            }
																											
																										}
																									}
																									else
																									{
																										if (b == 26)
																										{
																											byte b15 = this.readBuffer[num];
																											num++;
																											int num49 = (int)(this.readBuffer[num] - 1);
																											num++;
																											short num50 = BitConverter.ToInt16(this.readBuffer, num);
																											num += 2;
																											byte b16 = this.readBuffer[num];
																											bool pvp = false;
																											if (b16 != 0)
																											{
																												pvp = true;
																											}

																											//if(
                                                                                                                Main.player[(int)b15].Hurt((int)num50, num49, pvp, true); // <= 0.0) {
                                                                                                            //    return;
                                                                                                            //}
																											if (Main.netMode == 2)
																											{
																												//NetMessage.SendData(26, -1, this.whoAmI, "", (int)b15, (float)num49, (float)num50, (float)b16);
																												return;
																											}
																										}
																										else
																										{
																											if (b == 27)
																											{
																												short num51 = BitConverter.ToInt16(this.readBuffer, num);
																												num += 2;
																												float x6 = BitConverter.ToSingle(this.readBuffer, num);
																												num += 4;
																												float y5 = BitConverter.ToSingle(this.readBuffer, num);
																												num += 4;
																												float x7 = BitConverter.ToSingle(this.readBuffer, num);
																												num += 4;
																												float y6 = BitConverter.ToSingle(this.readBuffer, num);
																												num += 4;
																												float knockBack = BitConverter.ToSingle(this.readBuffer, num);
																												num += 4;
																												short damage = BitConverter.ToInt16(this.readBuffer, num);
																												num += 2;
																												byte b17 = this.readBuffer[num];
																												num++;
																												byte b18 = this.readBuffer[num];
																												num++;
																												float[] array2 = new float[Projectile.maxAI];
																												for (int num52 = 0; num52 < Projectile.maxAI; num52++)
																												{
																													array2[num52] = BitConverter.ToSingle(this.readBuffer, num);
																													num += 4;
																												}
																												int num53 = 1000;
																												for (int num54 = 0; num54 < 1000; num54++)
																												{
																													if (Main.projectile[num54].owner == (int)b17 && Main.projectile[num54].identity == (int)num51 && Main.projectile[num54].active)
																													{
																														num53 = num54;
																														break;
																													}
																												}
																												if (num53 == 1000)
																												{
																													for (int num55 = 0; num55 < 1000; num55++)
																													{
																														if (!Main.projectile[num55].active)
																														{
																															num53 = num55;
																															break;
																														}
																													}
																												}
																												if (!Main.projectile[num53].active || Main.projectile[num53].type != (int)b18)
																												{
																													Main.projectile[num53].SetDefaults((int)b18);
																												}
																												Main.projectile[num53].identity = (int)num51;
																												Main.projectile[num53].position.X = x6;
																												Main.projectile[num53].position.Y = y5;
																												Main.projectile[num53].velocity.X = x7;
																												Main.projectile[num53].velocity.Y = y6;
																												Main.projectile[num53].damage = (int)damage;
																												Main.projectile[num53].type = (int)b18;
																												Main.projectile[num53].owner = (int)b17;
																												Main.projectile[num53].knockBack = knockBack;
																												for (int num56 = 0; num56 < Projectile.maxAI; num56++)
																												{
																													Main.projectile[num53].ai[num56] = array2[num56];
																												}
																												if (Main.netMode == 2)
																												{
																													NetMessage.SendData(27, -1, this.whoAmI, "", num53, 0f, 0f, 0f);
																													return;
																												}
																											}
																											else
																											{
																												if (b == 28)
																												{
																													short num57 = BitConverter.ToInt16(this.readBuffer, num);
																													num += 2;
																													short num58 = BitConverter.ToInt16(this.readBuffer, num);
																													num += 2;
																													float num59 = BitConverter.ToSingle(this.readBuffer, num);
																													num += 4;
																													int num60 = (int)(this.readBuffer[num] - 1);
																													if (num58 >= 0)
																													{
																														Main.npc[(int)num57].StrikeNPC((int)num58, num59, num60);
																													}
																													else
																													{
																														Main.npc[(int)num57].life = 0;
																														Main.npc[(int)num57].HitEffect(0, 10.0);
																														Main.npc[(int)num57].active = false;
																													}
																													if (Main.netMode == 2)
																													{
																														NetMessage.SendData(28, -1, this.whoAmI, "", (int)num57, (float)num58, num59, (float)num60);
																														NetMessage.SendData(23, -1, -1, "", (int)num57, 0f, 0f, 0f);
																														return;
																													}
																												}
																												else
																												{
																													if (b == 29)
																													{
																														short num61 = BitConverter.ToInt16(this.readBuffer, num);
																														num += 2;
																														byte b19 = this.readBuffer[num];
																														for (int num62 = 0; num62 < 1000; num62++)
																														{
																															if (Main.projectile[num62].owner == (int)b19 && Main.projectile[num62].identity == (int)num61 && Main.projectile[num62].active)
																															{
																																Main.projectile[num62].Kill();
																																break;
																															}
																														}
																														if (Main.netMode == 2)
																														{
																															NetMessage.SendData(29, -1, this.whoAmI, "", (int)num61, (float)b19, 0f, 0f);
																															return;
																														}
																													}
																													else
																													{
																														if (b == 30)
																														{
																															byte b20 = this.readBuffer[num];
																															num++;
																															byte b21 = this.readBuffer[num];
																															if (b21 == 1)
																															{
																																Main.player[(int)b20].hostile = true;
																															}
																															else
																															{
																																Main.player[(int)b20].hostile = false;
																															}
																															if (Main.netMode == 2)
																															{
																																NetMessage.SendData(30, -1, this.whoAmI, "", (int)b20, 0f, 0f, 0f);
																																string str = " has enabled PvP!";
																																if (b21 == 0)
																																{
																																	str = " has disabled PvP!";
																																}
																																NetMessage.SendData(25, -1, -1, Main.player[(int)b20].name + str, 255, (float)Main.teamColor[Main.player[(int)b20].team].R, (float)Main.teamColor[Main.player[(int)b20].team].G, (float)Main.teamColor[Main.player[(int)b20].team].B);
																																return;
																															}
																														}
																														else
																														{
																															if (b == 31)
																															{
																																if (Main.netMode == 2)
																																{
																																	int x8 = BitConverter.ToInt32(this.readBuffer, num);
																																	num += 4;
																																	int y7 = BitConverter.ToInt32(this.readBuffer, num);
																																	num += 4;
																																	int num63 = Chest.FindChest(x8, y7);
                                                                                                                                    var Event = new ChestOpenEvent();
                                                                                                                                    Event.setSender(Main.player[whoAmI]);
                                                                                                                                    Event.setChestID(num63);
                                                                                                                                    Program.server.getPluginManager().processHook(Hooks.PLAYER_CHEST, Event);
                                                                                                                                    if (Event.getCancelled()) return;
																																	if (num63 > -1 && Chest.UsingChest(num63) == -1)
																																	{
																																		for (int num64 = 0; num64 < Chest.maxItems; num64++)
																																		{
																																			NetMessage.SendData(32, this.whoAmI, -1, "", num63, (float)num64, 0f, 0f);
																																		}
																																		NetMessage.SendData(33, this.whoAmI, -1, "", num63, 0f, 0f, 0f);
																																		Main.player[this.whoAmI].chest = num63;
																																		return;
																																	}
																																}
																															}
																															else
																															{
																																if (b == 32)
																																{
																																	int num65 = (int)BitConverter.ToInt16(this.readBuffer, num);
																																	num += 2;
																																	int num66 = (int)this.readBuffer[num];
																																	num++;
																																	int stack3 = (int)this.readBuffer[num];
																																	num++;
																																	string string8 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
																																	if (Main.chest[num65] == null)
																																	{
																																		Main.chest[num65] = new Chest();
																																	}
																																	if (Main.chest[num65].item[num66] == null)
																																	{
																																		Main.chest[num65].item[num66] = new Item();
																																	}
																																	Main.chest[num65].item[num66].SetDefaults(string8);
																																	Main.chest[num65].item[num66].stack = stack3;
																																	return;
																																}
																																if (b == 33)
																																{
																																	int num67 = (int)BitConverter.ToInt16(this.readBuffer, num);
																																	num += 2;
																																	int chestX = BitConverter.ToInt32(this.readBuffer, num);
																																	num += 4;
																																	int chestY = BitConverter.ToInt32(this.readBuffer, num);
																																	if (Main.netMode == 1)
																																	{
																																		if (Main.player[Main.myPlayer].chest == -1)
																																		{
																																			Main.playerInventory = true;
																																			//Main.PlaySound(10, -1, -1, 1);
																																		}
																																		else
																																		{
																																			if (Main.player[Main.myPlayer].chest != num67 && num67 != -1)
																																			{
																																				Main.playerInventory = true;
																																				//Main.PlaySound(12, -1, -1, 1);
																																			}
																																			else
																																			{
																																				if (Main.player[Main.myPlayer].chest != -1 && num67 == -1)
																																				{
																																					//Main.PlaySound(11, -1, -1, 1);
																																				}
																																			}
																																		}
																																		Main.player[Main.myPlayer].chest = num67;
																																		Main.player[Main.myPlayer].chestX = chestX;
																																		Main.player[Main.myPlayer].chestY = chestY;
																																		return;
																																	}
																																	Main.player[this.whoAmI].chest = num67;
																																	return;
																																}
																																else
																																{
																																	if (b == 34)
																																	{
																																		if (Main.netMode == 2)
																																		{
																																			int num68 = BitConverter.ToInt32(this.readBuffer, num);
																																			num += 4;
																																			int num69 = BitConverter.ToInt32(this.readBuffer, num);
																																			WorldGen.KillTile(num68, num69, false, false, false);
																																			if (!Main.tile[num68, num69].active)
																																			{
																																				NetMessage.SendData(17, -1, -1, "", 0, (float)num68, (float)num69, 0f);
																																				return;
																																			}
																																		}
																																	}
																																	else
																																	{
																																		if (b == 35)
																																		{
																																			int num70 = (int)this.readBuffer[num];
																																			num++;
																																			int num71 = (int)BitConverter.ToInt16(this.readBuffer, num);
																																			num += 2;
																																			if (num70 != Main.myPlayer)
																																			{
																																				Main.player[num70].HealEffect(num71);
																																			}
																																			if (Main.netMode == 2)
																																			{
																																				NetMessage.SendData(35, -1, this.whoAmI, "", num70, (float)num71, 0f, 0f);
																																				return;
																																			}
																																		}
																																		else
																																		{
																																			if (b == 36)
																																			{
																																				int num72 = (int)this.readBuffer[num];
																																				num++;
																																				int num73 = (int)this.readBuffer[num];
																																				num++;
																																				int num74 = (int)this.readBuffer[num];
																																				num++;
																																				int num75 = (int)this.readBuffer[num];
																																				num++;
																																				int num76 = (int)this.readBuffer[num];
																																				num++;
																																				if (num73 == 0)
																																				{
																																					Main.player[num72].zoneEvil = false;
																																				}
																																				else
																																				{
																																					Main.player[num72].zoneEvil = true;
																																				}
																																				if (num74 == 0)
																																				{
																																					Main.player[num72].zoneMeteor = false;
																																				}
																																				else
																																				{
																																					Main.player[num72].zoneMeteor = true;
																																				}
																																				if (num75 == 0)
																																				{
																																					Main.player[num72].zoneDungeon = false;
																																				}
																																				else
																																				{
																																					Main.player[num72].zoneDungeon = true;
																																				}
																																				if (num76 == 0)
																																				{
																																					Main.player[num72].zoneJungle = false;
																																					return;
																																				}
																																				Main.player[num72].zoneJungle = true;
																																				return;
																																			}
																																			else
																																			{
																																				if (b == 37)
																																				{
																																					if (Main.netMode == 1)
																																					{
																																						if (Main.autoPass)
																																						{
																																							NetMessage.SendData(38, -1, -1, NetPlay.password, 0, 0f, 0f, 0f);
																																							Main.autoPass = false;
																																							return;
																																						}
																																						NetPlay.password = "";
																																						Main.menuMode = 31;
																																						return;
																																					}
																																				}
																																				else
																																				{
																																					if (b == 38)
																																					{
																																						if (Main.netMode == 2)
																																						{
																																							string pasword = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
                                                                                                                                                            if (pasword == NetPlay.password)
                                                                                                                                                            {
                                                                                                                                                                Main.player[this.whoAmI].setOp(true);
                                                                                                                                                            }
                                                                                                                                                            else
                                                                                                                                                            {
                                                                                                                                                                Main.player[this.whoAmI].setOp(false);
                                                                                                                                                            }
                                                                                                                                                            NetPlay.serverSock[this.whoAmI].state = 1;
                                                                                                                                                            NetMessage.SendData(3, this.whoAmI, -1, "", 0, 0f, 0f, 0f);
                                                                                                                                                            return;
																																						}
																																					}
																																					else
																																					{
																																						if (b == 39 && Main.netMode == 1)
																																						{
																																							short num77 = BitConverter.ToInt16(this.readBuffer, num);
																																							Main.item[(int)num77].owner = 255;
																																							NetMessage.SendData(22, -1, -1, "", (int)num77, 0f, 0f, 0f);
																																							return;
																																						}
																																						if (b == 40)
																																						{
																																							byte b22 = this.readBuffer[num];
																																							num++;
																																							int talkNPC = (int)BitConverter.ToInt16(this.readBuffer, num);
																																							num += 2;
																																							Main.player[(int)b22].talkNPC = talkNPC;
																																							if (Main.netMode == 2)
																																							{
																																								NetMessage.SendData(40, -1, this.whoAmI, "", (int)b22, 0f, 0f, 0f);
																																								return;
																																							}
																																						}
																																						else
																																						{
																																							if (b == 41)
																																							{
																																								byte b23 = this.readBuffer[num];
																																								num++;
																																								float itemRotation = BitConverter.ToSingle(this.readBuffer, num);
																																								num += 4;
																																								int itemAnimation = (int)BitConverter.ToInt16(this.readBuffer, num);
																																								Main.player[(int)b23].itemRotation = itemRotation;
																																								Main.player[(int)b23].itemAnimation = itemAnimation;
																																								if (Main.netMode == 2)
																																								{
																																									NetMessage.SendData(41, -1, this.whoAmI, "", (int)b23, 0f, 0f, 0f);
																																									return;
																																								}
																																							}
																																							else
																																							{
																																								if (b == 42)
																																								{
																																									int num78 = (int)this.readBuffer[num];
																																									num++;
																																									int statMana = (int)BitConverter.ToInt16(this.readBuffer, num);
																																									num += 2;
																																									int statManaMax = (int)BitConverter.ToInt16(this.readBuffer, num);
																																									if (Main.netMode == 2)
																																									{
																																										num78 = this.whoAmI;
																																									}
																																									Main.player[num78].statMana = statMana;
																																									Main.player[num78].statManaMax = statManaMax;
																																									if (Main.netMode == 2)
																																									{
																																										NetMessage.SendData(42, -1, this.whoAmI, "", num78, 0f, 0f, 0f);
																																										return;
																																									}
																																								}
																																								else
																																								{
																																									if (b == 43)
																																									{
																																										int num79 = (int)this.readBuffer[num];
																																										num++;
																																										int num80 = (int)BitConverter.ToInt16(this.readBuffer, num);
																																										num += 2;
																																										if (num79 != Main.myPlayer)
																																										{
																																											Main.player[num79].ManaEffect(num80);
																																										}
																																										if (Main.netMode == 2)
																																										{
																																											NetMessage.SendData(43, -1, this.whoAmI, "", num79, (float)num80, 0f, 0f);
																																											return;
																																										}
																																									}
																																									else
																																									{
																																										if (b == 44)
																																										{
																																											byte b24 = this.readBuffer[num];
																																											num++;
																																											int num81 = (int)(this.readBuffer[num] - 1);
																																											num++;
																																											short num82 = BitConverter.ToInt16(this.readBuffer, num);
																																											num += 2;
																																											byte b25 = this.readBuffer[num];
																																											bool pvp2 = false;
																																											if (b25 != 0)
																																											{
																																												pvp2 = true;
																																											}
																																											Main.player[(int)b24].KillMe((double)num82, num81, pvp2);
																																											if (Main.netMode == 2)
																																											{
																																												NetMessage.SendData(44, -1, this.whoAmI, "", (int)b24, (float)num81, (float)num82, (float)b25);
																																												return;
																																											}
																																										}
																																										else
																																										{
																																											if (b == 45)
																																											{
																																												int num83 = (int)this.readBuffer[num];
																																												num++;
																																												int num84 = (int)this.readBuffer[num];
																																												num++;
																																												int team = Main.player[num83].team;
																																												if (Main.netMode == 2)
																																												{
																																													NetMessage.SendData(45, -1, this.whoAmI, "", num83, 0f, 0f, 0f);
                                                                                                                                                                                    Party party = Party.NONE;
																																													string str2 = "";
																																													if (num84 == 0)
																																													{
																																														str2 = " is no longer on a party.";
																																													}
																																													else
																																													{
																																														if (num84 == 1)
																																														{
																																															str2 = " has joined the red party.";
                                                                                                                                                                                            party = Party.RED;
																																														}
																																														else
																																														{
																																															if (num84 == 2)
																																															{
																																																str2 = " has joined the green party.";
                                                                                                                                                                                                party = Party.GREEN;
																																															}
																																															else
																																															{
																																																if (num84 == 3)
																																																{
																																																	str2 = " has joined the blue party.";
                                                                                                                                                                                                    party = Party.BLUE;
																																																}
																																																else
																																																{
																																																	if (num84 == 4)
																																																	{
																																																		str2 = " has joined the yellow party.";
                                                                                                                                                                                                        party = Party.YELLOW;
																																																	}
																																																}
																																															}
																																														}
                                                                                                                                                                                    }
                                                                                                                                                                                    PartyChangeEvent Event = new PartyChangeEvent();
                                                                                                                                                                                    Event.setPartyType(party);
                                                                                                                                                                                    Event.setSender(Main.player[this.whoAmI]);
                                                                                                                                                                                    Program.server.getPluginManager().processHook(Plugin.Hooks.PLAYER_PARTYCHANGE, Event);
                                                                                                                                                                                    if (Event.getCancelled())
                                                                                                                                                                                    {
                                                                                                                                                                                        return;
                                                                                                                                                                                    }

                                                                                                                                                                                    Main.player[num83].team = num84;
                                                                                                                                                                                    for (int num85 = 0; num85 < 255; num85++)
																																													{
																																														if (num85 == this.whoAmI || (team > 0 && Main.player[num85].team == team) || (num84 > 0 && Main.player[num85].team == num84))
																																														{
																																															NetMessage.SendData(25, num85, -1, Main.player[num83].name + str2, 255, (float)Main.teamColor[num84].R, (float)Main.teamColor[num84].G, (float)Main.teamColor[num84].B);
																																														}
																																													}
																																													return;
																																												}
																																											}
																																											else
																																											{
																																												if (b == 46)
																																												{
																																													if (Main.netMode == 2)
																																													{
																																														int i2 = BitConverter.ToInt32(this.readBuffer, num);
																																														num += 4;
																																														int j2 = BitConverter.ToInt32(this.readBuffer, num);
																																														num += 4;
																																														int num86 = Sign.ReadSign(i2, j2);
																																														if (num86 >= 0)
																																														{
																																															NetMessage.SendData(47, this.whoAmI, -1, "", num86, 0f, 0f, 0f);
																																															return;
																																														}
																																													}
																																												}
																																												else
																																												{
																																													if (b == 47)
																																													{
																																														int num87 = (int)BitConverter.ToInt16(this.readBuffer, num);
																																														num += 2;
																																														int x9 = BitConverter.ToInt32(this.readBuffer, num);
																																														num += 4;
																																														int y8 = BitConverter.ToInt32(this.readBuffer, num);
																																														num += 4;
																																														string string10 = Encoding.ASCII.GetString(this.readBuffer, num, length - num + start);
																																														Main.sign[num87] = new Sign();
																																														Main.sign[num87].x = x9;
																																														Main.sign[num87].y = y8;
																																														Sign.TextSign(num87, string10);
																																														if (Main.netMode == 1 && Main.sign[num87] != null && num87 != Main.player[Main.myPlayer].sign)
																																														{
																																															Main.playerInventory = false;
																																															Main.player[Main.myPlayer].talkNPC = -1;
																																															Main.editSign = false;
																																															//Main.PlaySound(10, -1, -1, 1);
																																															Main.player[Main.myPlayer].sign = num87;
																																															Main.npcChatText = Main.sign[num87].text;
																																															return;
																																														}
																																													}
																																													else
																																													{
																																														if (b == 48)
																																														{
																																															int num88 = BitConverter.ToInt32(this.readBuffer, num);
																																															num += 4;
																																															int num89 = BitConverter.ToInt32(this.readBuffer, num);
																																															num += 4;
																																															byte liquid = this.readBuffer[num];
																																															num++;
																																															byte b26 = this.readBuffer[num];
																																															num++;
																																															if (Main.tile[num88, num89] == null)
																																															{
																																																Main.tile[num88, num89] = new Tile();
																																															}
																																															lock (Main.tile[num88, num89])
																																															{
																																																Main.tile[num88, num89].liquid = liquid;
																																																if (b26 == 1)
																																																{
																																																	Main.tile[num88, num89].lava = true;
																																																}
																																																else
																																																{
																																																	Main.tile[num88, num89].lava = false;
																																																}
																																																if (Main.netMode == 2)
																																																{
																																																	WorldGen.SquareTileFrame(num88, num89, true);
																																																}
																																																return;
																																															}
																																														}
																																														if (b == 49 && NetPlay.clientSock.state == 6)
																																														{
																																															NetPlay.clientSock.state = 10;
																																															Main.player[Main.myPlayer].Spawn();
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
