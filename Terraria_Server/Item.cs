using System;
using System.Collections.Generic;
using Terraria_Server.Misc;
using Terraria_Server.Collections;
using Terraria_Server.Definitions;
using Terraria_Server.WorldMod;
using Terraria_Server.Logging;
using Terraria_Server.Plugins;

namespace Terraria_Server
{
	/// <summary>
	/// Items class.  Controls all items, item interactions and item changes.
	/// </summary>
	public class Item : BaseEntity
	{
		public const Int32 MAX_HEAD_TYPES = 44;
		public const Int32 MAX_BODY_TYPES = 25;
		public const Int32 MAX_LEG_TYPES = 24;
		public const Int32 MAX_ITEMS = 200;

		public const Int32 MAX_AFFIXS = 83;

		public static int[] headType = new int[MAX_HEAD_TYPES];
		public static int[] bodyType = new int[MAX_BODY_TYPES];
		public static int[] legType = new int[MAX_LEG_TYPES];

		/// <summary>
		/// Potion delay time
		/// </summary>
		public const int POTION_DELAY = 720;
		/// <summary>
		/// ProjectileType of ammo used when Item used.
		/// </summary>
		public ProjectileType Ammo;
		/// <summary>
		/// Whether the item is automatically reused if player is holding mouse button
		/// </summary>
		public bool AutoReuse;
		/// <summary>
		/// Type of axe this item is.  0 means not an axe
		/// </summary>
		public int Axe;
		/// <summary>
		/// Whether the item is currently being grabbed
		/// </summary>
		public bool BeingGrabbed;
		public int BodySlot = -1;
		/// <summary>
		/// Amount of time this buff effect lasts
		/// </summary>
		public int BuffTime;
		/// <summary>
		/// Type of buff this item gives.  Values currently unknown
		/// </summary>
		public int BuffType;
		public bool Buy;
		public bool Channel;
		/// <summary>
		/// Whether item is consumable
		/// </summary>
		public bool Consumable;
		/// <summary>
		/// Tile "type" this item creates when used
		/// </summary>
		public int CreateTile;
		/// <summary>
		/// Wall "type" this item creates when used
		/// </summary>
		public int CreateWall;
		/// <summary>
		/// Type of hammer.  0 means not a hammer
		/// </summary>
		public int Hammer;
		public int HeadSlot;
		/// <summary>
		/// Amount of life restoration provided by this item
		/// </summary>
		public int HealLife;
		/// <summary>
		/// Amount of mana restoration provided by this item
		/// </summary>
		public int HealMana;
		public int HoldStyle;
		public int KeepTime;
		/// <summary>
		/// Knockback provided by this item
		/// </summary>
		public float KnockBack;
		/// <summary>
		/// Whether item is currently in lava
		/// </summary>
		public bool LavaWet;
		public int LegSlot;
		/// <summary>
		/// Amount of life regeneration provided by this item
		/// </summary>
		public int LifeRegen;
		/// <summary>
		/// Whether item is magical or not
		/// </summary>
		public bool Magic;
		/// <summary>
		/// Mana required to use this item
		/// </summary>
		public int Mana;
		/// <summary>
		/// Amount of life regeneration provided by this item
		/// </summary>
		public int ManaRegen;
		/// <summary>
		/// Maximum number of this item that can be in one stack
		/// </summary>
		public int MaxStack;
		/// <summary>
		/// Whether this is a melee item or not
		/// </summary>
		public bool Melee;
		/// <summary>
		/// Amount of time before item can be grabbed
		/// </summary>
		public int NoGrabDelay;
		public bool NoMelee;
		/// <summary>
		/// Whether this item can be wet or not
		/// </summary>
		public bool NoWet;
		/// <summary>
		/// Player index value of item's owner
		/// </summary>
		public int Owner = 255;
		/// <summary>
		/// Time left to ignore pickup
		/// </summary>
		public int OwnIgnore = -1;
		public int OwnTime;
		/// <summary>
		/// Type of pick this item is.  0 means not a pick
		/// </summary>
		public int Pick;
		public int PlaceStyle;
		/// <summary>
		/// Whether item is a potion
		/// </summary>
		public bool Potion;
		/// <summary>
		/// Whether item is ranged or not
		/// </summary>
		public bool Ranged;
		/// <summary>
		/// Rarity level of item
		/// </summary>
		public int Rare;
		public int Release;
		/// <summary>
		/// Projectile this item shoots
		/// </summary>
		public ProjectileType Shoot;
		/// <summary>
		/// Speed at which this item shoots projectiles
		/// </summary>
		public float ShootSpeed;
		public int SpawnTime;
		/// <summary>
		/// Current item stack amount
		/// </summary>
		public int Stack;
		public int TileBoost;
		/// <summary>
		/// String representing hovering tool tip
		/// </summary>
		public string ToolTip;
		/// <summary>
		/// Secondary tool tip string
		/// </summary>
		public string ToolTip2;
		/// <summary>
		/// Essentially the same as Ammo
		/// </summary>
		public ProjectileType UseAmmo;
		/// <summary>
		/// The time an animation of use takes
		/// </summary>
		public int UseAnimation;
		public int UseStyle;
		/// <summary>
		/// Time the item takes to use
		/// </summary>
		public int UseTime;
		public bool UseTurn;
		/// <summary>
		/// Currency value in Terraria economy
		/// </summary>
		public int Value;
		/// <summary>
		/// Speed and direction of item
		/// </summary>
		public Vector2 Velocity;
		/// <summary>
		/// Whether the item is currently wet
		/// </summary>
		public bool Wet;
		public byte WetCount;

		public byte Prefix;

		/// <summary>
		/// Item class contstructor
		/// </summary>
		public Item()
		{
			BodySlot = -1;
			CreateTile = -1;
			CreateWall = -1;
			damage = -1;
			LegSlot = -1;
			HeadSlot = -1;
			MaxStack = 1;
			Owner = 255;
			scale = 1f;
			ToolTip = null;
			UseTime = 100;
			UseAnimation = 100;
		}

		//[ToDo] Implement the ReuseDelay and w/e else I added.
		public bool SetPrefix(int prefix)
		{
			Prefix = (byte)prefix;

			if (prefix == 0 || Type == 0)
				return false;

			int _pre = prefix;
			float _dmgMod = 1f;
			float _kncMod = 1f;
			float _useMod = 1f;
			float _sclMod = 1f;
			float _shtMod = 1f;
			float _mnaMod = 1f;
			int _ctlMod = 0;
			bool valsNotSet = true;
			while (valsNotSet)
			{
				_dmgMod = 1f;
				_kncMod = 1f;
				_useMod = 1f;
				_sclMod = 1f;
				_shtMod = 1f;
				_mnaMod = 1f;
				_ctlMod = 0;
				valsNotSet = false;

				if (_pre == -1 && Main.rand.Next(4) == 0)
					_pre = 0;

				if (prefix < -1)
					_pre = -1;

				if (_pre == -1 || _pre == -2 || _pre == -3)
				{
					if (Type == 1 || Type == 4 || Type == 6 || Type == 7 || Type == 10 || Type == 24 ||
						Type == 45 || Type == 46 || Type == 103 || Type == 104 || Type == 121 || Type == 122 ||
						Type == 155 || Type == 190 || Type == 196 || Type == 198 || Type == 199 || Type == 200 ||
						Type == 201 || Type == 202 || Type == 203 || Type == 204 || Type == 213 || Type == 217 ||
						Type == 273 || Type == 367 || Type == 368 || Type == 426 || Type == 482 || Type == 483 ||
						Type == 484)
					{
						int _randPre = Main.rand.Next(40);

						if (_randPre >= 0 && _randPre <= 14)
							_pre = _randPre + 1;
						else if (_randPre >= 15 && _randPre <= 17)
							_pre = _randPre + 21;
						else if (_randPre >= 18 && _randPre <= 20)
							_pre = _randPre + 35;
						else if (_randPre >= 21 && _randPre <= 22)
							_pre = _randPre + 18;
						else if (_randPre >= 26 && _randPre <= 35)
							_pre = _randPre + 26;
						else if (_randPre >= 36 && _randPre <= 38)
							_pre = _randPre + 23;
						else if (_randPre == 23 || _randPre == 25)
							_pre = _randPre + 33;
						else
						{
							switch (_randPre)
							{
								case 24:
									_pre = 41;
									break;
								case 39:
									_pre = 81;
									break;
							}
						}
					}
					else
					{
						if (Type == 162 || Type == 163 || Type == 220 || Type == 274 || Type == 277 || Type == 280 ||
							Type == 383 || Type == 384 || Type == 385 || Type == 386 || Type == 387 || Type == 388 ||
							Type == 389 || Type == 390 || Type == 406 || Type == 537 || Type == 550 || Type == 579)
						{
							int _randPre = Main.rand.Next(14);

							if (_randPre >= 0 && _randPre <= 2)
								_pre = _randPre + 36;
							if (_randPre >= 3 && _randPre <= 5)
								_pre = _randPre + 50;
							if (_randPre >= 6 && _randPre <= 7)
								_pre = _randPre + 33;
							if (_randPre >= 11 && _randPre <= 13)
								_pre = _randPre + 48;
							else
							{
								switch (_randPre)
								{
									case 8:
										_pre = 56;
										break;
									case 9:
										_pre = 41;
										break;
									case 10:
										_pre = 57;
										break;
								}
							}
						}
						else
						{
							if (Type == 39 || Type == 44 || Type == 95 || Type == 96 || Type == 98 || Type == 99 ||
								Type == 120 || Type == 164 || Type == 197 || Type == 219 || Type == 266 || Type == 281 ||
								Type == 434 || Type == 435 || Type == 436 || Type == 481 || Type == 506 || Type == 533 ||
								Type == 534 || Type == 578)
							{
								int _randPre = Main.rand.Next(36);

								if (_randPre >= 0 && _randPre <= 9)
									_pre = _randPre + 16;
								else if (_randPre >= 11 && _randPre <= 13)
									_pre = _randPre + 25;
								else if (_randPre >= 14 && _randPre <= 16)
									_pre = _randPre + 39;
								else if (_randPre >= 17 && _randPre <= 18)
									_pre = _randPre + 22;
								else if (_randPre >= 22 && _randPre <= 31)
									_pre = _randPre + 20;
								else if (_randPre >= 32 && _randPre <= 34)
									_pre = _randPre + 27;
								else
								{
									switch (_randPre)
									{
										case 10:
											_pre = 58;
											break;
										case 19:
											_pre = 56;
											break;
										case 20:
											_pre = 41;
											break;
										case 21:
											_pre = 57;
											break;
										case 35:
											_pre = 82;
											break;
									}
								}
							}
							else
							{
								if (Type == 64 || Type == 65 || Type == 112 || Type == 113 || Type == 127 || Type == 157 ||
									Type == 218 || Type == 272 || Type == 494 || Type == 495 || Type == 496 || Type == 514 ||
									Type == 517 || Type == 518 || Type == 519)
								{
									int _randPre = Main.rand.Next(36);

									if (_randPre >= 0 && _randPre <= 9)
										_pre = _randPre + 26;
									else if (_randPre >= 11 && _randPre <= 13)
										_pre = _randPre + 25;
									else if (_randPre >= 14 && _randPre <= 16)
										_pre = _randPre + 39;
									else if (_randPre >= 17 && _randPre <= 18)
										_pre = _randPre + 22;
									else if (_randPre >= 19 && _randPre <= 21)
										_pre = _randPre + 21;
									else if (_randPre >= 22 && _randPre <= 31)
										_pre = _randPre + 20;
									else if (_randPre >= 32 && _randPre <= 34)
										_pre = _randPre + 27;
									else
									{
										switch (_randPre)
										{
											case 10:
												_pre = 52;
												break;
											case 35:
												_pre = 83;
												break;
										}
									}
								}
								else
								{
									if (Type == 55 || Type == 119 || Type == 191 || Type == 284)
									{
										int _randPre = Main.rand.Next(14);

										if (_randPre >= 0 && _randPre <= 2)
											_pre = _randPre + 36;
										else if (_randPre >= 3 && _randPre <= 5)
											_pre = _randPre + 50;
										else if (_randPre >= 6 && _randPre <= 7)
											_pre = _randPre + 33;
										else if (_randPre >= 11 && _randPre <= 13)
											_pre = _randPre + 48;
										else
										{
											switch (_randPre)
											{
												case 8:
													_pre = 56;
													break;
												case 9:
													_pre = 41;
													break;
												case 10:
													_pre = 57;
													break;
											}
										}
									}
									else
									{
										if (!Accessory ||
											Type == 267 || Type == 562 || Type == 563 || Type == 564 || Type == 565 || Type == 566 ||
											Type == 567 || Type == 568 || Type == 569 || Type == 570 || Type == 571 || Type == 572 ||
											Type == 573 || Type == 574 || Type == 576)
										{
											return false;
										}
										_pre = Main.rand.Next(62, 81);
									}
								}
							}
						}
					}
				}
				if (prefix == -3)
				{
					return true;
				}
				if (prefix == -1 &&
					(_pre == 7 || _pre == 8 || _pre == 9 || _pre == 10 || _pre == 11 || _pre == 22 ||
					_pre == 23 || _pre == 24 || _pre == 29 || _pre == 30 || _pre == 31 || _pre == 39 ||
					_pre == 40 || _pre == 56 || _pre == 41 || _pre == 47 || _pre == 48 || _pre == 49)
					&& Main.rand.Next(3) != 0)
				{
					_pre = 0;
				}

				switch (_pre)
				{
					case 1:
						_sclMod = 1.12f;
						break;
					case 2:
						_sclMod = 1.18f;
						break;
					case 3:
						_dmgMod = 1.05f;
						_ctlMod = 2;
						_sclMod = 1.05f;
						break;
					case 4:
						_dmgMod = 1.1f;
						_sclMod = 1.1f;
						_kncMod = 1.1f;
						break;
					case 5:
						_dmgMod = 1.15f;
						break;
					case 6:
						_dmgMod = 1.1f;
						break;
					case 7:
						_sclMod = 0.82f;
						break;
					case 8:
						_kncMod = 0.85f;
						_dmgMod = 0.85f;
						_sclMod = 0.87f;
						break;
					case 9:
						_sclMod = 0.9f;
						break;
					case 10:
						_dmgMod = 0.85f;
						break;
					case 11:
						_useMod = 1.1f;
						_kncMod = 0.9f;
						_sclMod = 0.9f;
						break;
					case 12:
						_kncMod = 1.1f;
						_dmgMod = 1.05f;
						_sclMod = 1.1f;
						_useMod = 1.15f;
						break;
					case 13:
						_kncMod = 0.8f;
						_dmgMod = 0.9f;
						_sclMod = 1.1f;
						break;
					case 14:
						_kncMod = 1.15f;
						_useMod = 1.1f;
						break;
					case 15:
						_kncMod = 0.9f;
						_useMod = 0.85f;
						break;
					case 16:
						_dmgMod = 1.1f;
						_ctlMod = 3;
						break;
					case 17:
						_useMod = 0.85f;
						_shtMod = 1.1f;
						break;
					case 18:
						_useMod = 0.9f;
						_shtMod = 1.15f;
						break;
					case 19:
						_kncMod = 1.15f;
						_shtMod = 1.05f;
						break;
					case 20:
						_kncMod = 1.05f;
						_shtMod = 1.05f;
						_dmgMod = 1.1f;
						_useMod = 0.95f;
						_ctlMod = 2;
						break;
					case 21:
						_kncMod = 1.15f;
						_dmgMod = 1.1f;
						break;
					case 22:
						_kncMod = 0.9f;
						_shtMod = 0.9f;
						_dmgMod = 0.85f;
						break;
					case 23:
						_useMod = 1.15f;
						_shtMod = 0.9f;
						break;
					case 24:
						_useMod = 1.1f;
						_kncMod = 0.8f;
						break;
					case 25:
						_useMod = 1.1f;
						_dmgMod = 1.15f;
						_ctlMod = 1;
						break;
					case 26:
						_mnaMod = 0.85f;
						_dmgMod = 1.1f;
						break;
					case 27:
						_mnaMod = 0.85f;
						break;
					case 28:
						_mnaMod = 0.85f;
						_dmgMod = 1.15f;
						_kncMod = 1.05f;
						break;
					case 29:
						_mnaMod = 1.1f;
						break;
					case 30:
						_mnaMod = 1.2f;
						_dmgMod = 0.9f;
						break;
					case 31:
						_kncMod = 0.9f;
						_dmgMod = 0.9f;
						break;
					case 32:
						_mnaMod = 1.15f;
						_dmgMod = 1.1f;
						break;
					case 33:
						_mnaMod = 1.1f;
						_kncMod = 1.1f;
						_useMod = 0.9f;
						break;
					case 34:
						_mnaMod = 0.9f;
						_kncMod = 1.1f;
						_useMod = 1.1f;
						_dmgMod = 1.1f;
						break;
					case 35:
						_mnaMod = 1.2f;
						_dmgMod = 1.15f;
						_kncMod = 1.15f;
						break;
					case 36:
						_ctlMod = 3;
						break;
					case 37:
						_dmgMod = 1.1f;
						_ctlMod = 3;
						_kncMod = 1.1f;
						break;
					case 38:
						_kncMod = 1.15f;
						break;
					case 39:
						_dmgMod = 0.7f;
						_kncMod = 0.8f;
						break;
					case 40:
						_dmgMod = 0.85f;
						break;
					case 41:
						_kncMod = 0.85f;
						_dmgMod = 0.9f;
						break;
					case 42:
						_useMod = 0.9f;
						break;
					case 43:
						_dmgMod = 1.1f;
						_useMod = 0.9f;
						break;
					case 44:
						_useMod = 0.9f;
						_ctlMod = 3;
						break;
					case 45:
						_useMod = 0.95f;
						break;
					case 46:
						_ctlMod = 3;
						_useMod = 0.94f;
						_dmgMod = 1.07f;
						break;
					case 47:
						_useMod = 1.15f;
						break;
					case 48:
						_useMod = 1.2f;
						break;
					case 49:
						_useMod = 1.08f;
						break;
					case 50:
						_dmgMod = 0.8f;
						_useMod = 1.15f;
						break;
					case 51:
						_kncMod = 0.9f;
						_useMod = 0.9f;
						_dmgMod = 1.05f;
						_ctlMod = 2;
						break;
					case 52:
						_mnaMod = 0.9f;
						_dmgMod = 0.9f;
						_useMod = 0.9f;
						break;
					case 53:
						_dmgMod = 1.1f;
						break;
					case 54:
						_kncMod = 1.15f;
						break;
					case 55:
						_kncMod = 1.15f;
						_dmgMod = 1.05f;
						break;
					case 56:
						_kncMod = 0.8f;
						break;
					case 57:
						_kncMod = 0.9f;
						_dmgMod = 1.18f;
						break;
					case 58:
						_useMod = 0.85f;
						_dmgMod = 0.85f;
						break;
					case 59:
						_kncMod = 1.15f;
						_dmgMod = 1.15f;
						_ctlMod = 5;
						break;
					case 60:
						_dmgMod = 1.15f;
						_ctlMod = 5;
						break;
					case 61:
						_ctlMod = 5;
						break;
					case 81:
						_kncMod = 1.15f;
						_dmgMod = 1.15f;
						_ctlMod = 5;
						_useMod = 0.9f;
						_sclMod = 1.1f;
						break;
					case 82:
						_kncMod = 1.15f;
						_dmgMod = 1.15f;
						_ctlMod = 5;
						_useMod = 0.9f;
						_shtMod = 1.1f;
						break;
					case 83:
						_kncMod = 1.15f;
						_dmgMod = 1.15f;
						_ctlMod = 5;
						_useMod = 0.9f;
						_mnaMod = 0.9f;
						break;
				}

				var dmg = _dmgMod != 1f && Math.Round((double)((float)damage * _dmgMod)) == (double)damage;
				var ani = _useMod != 1f && Math.Round((double)((float)UseAnimation * _useMod)) == (double)UseAnimation;
				var mna = _mnaMod != 1f && Math.Round((double)((float)Mana * _mnaMod)) == (double)Mana;
				var knc = _kncMod != 1f && KnockBack == 0f;
				var pfx = prefix == -2 && _pre == 0;

				if (dmg || ani || mna || knc || pfx)
				{
					_pre = -1;
					valsNotSet = true;
				}
			}

			damage = (int)Math.Round((double)((float)damage * _dmgMod));
			UseAnimation = (int)Math.Round((double)((float)UseAnimation * _useMod));
			UseTime = (int)Math.Round((double)((float)UseTime * _useMod));
			ReUseDelay = (int)Math.Round((double)((float)ReUseDelay * _useMod));
			Mana = (int)Math.Round((double)((float)Mana * _mnaMod));
			KnockBack *= _kncMod;
			scale *= _sclMod;
			ShootSpeed *= _shtMod;
			Critical += _ctlMod;

			float _curMod = 1f * _dmgMod * (2f - _useMod) * (2f - _mnaMod) * _sclMod * _kncMod * _shtMod * (1f + (float)Critical * 0.02f);
			if (_pre == 62 || _pre == 69 || _pre == 73 || _pre == 77)
				_curMod *= 1.05f;
			if (_pre == 63 || _pre == 70 || _pre == 74 || _pre == 78 || _pre == 67)
				_curMod *= 1.1f;
			if (_pre == 64 || _pre == 71 || _pre == 75 || _pre == 79 || _pre == 66)
				_curMod *= 1.15f;
			if (_pre == 65 || _pre == 72 || _pre == 76 || _pre == 80 || _pre == 68)
				_curMod *= 1.2f;


			if ((double)_curMod >= 1.2)
				Rare += 2;
			else if ((double)_curMod >= 1.05)
				Rare++;
			else if ((double)_curMod <= 0.8)
				Rare -= 2;
			else if ((double)_curMod <= 0.95)
				Rare--;

			if (Rare < -1)
				Rare = -1;
			if (Rare > 6)
				Rare = 6;

			_curMod *= _curMod;
			Value = (int)((float)Value * _curMod);
			Prefix = (byte)_pre;
			return true;
		}

		public string AffixName()
		{
			string title = String.Empty;

			switch (Prefix)
			{
				case 1:
					title = "Large";
					break;
				case 2:
					title = "Massive";
					break;
				case 3:
					title = "Dangerous";
					break;
				case 4:
					title = "Savage";
					break;
				case 5:
					title = "Sharp";
					break;
				case 6:
					title = "Pointy";
					break;
				case 7:
					title = "Tiny";
					break;
				case 8:
					title = "Terrible";
					break;
				case 9:
					title = "Small";
					break;
				case 10:
					title = "Dull";
					break;
				case 11:
					title = "Unhappy";
					break;
				case 12:
					title = "Bulky";
					break;
				case 13:
					title = "Shameful";
					break;
				case 14:
					title = "Heavy";
					break;
				case 15:
					title = "Light";
					break;
				case 16:
					title = "Sighted";
					break;
				case 17:
					title = "Rapid";
					break;
				case 18:
					title = "Hasty";
					break;
				case 19:
					title = "Intimidating";
					break;
				case 20:
					title = "Deadly";
					break;
				case 21:
					title = "Staunch";
					break;
				case 22:
					title = "Awful";
					break;
				case 23:
					title = "Lethargic";
					break;
				case 24:
					title = "Awkward";
					break;
				case 25:
					title = "Powerful";
					break;
				case 58:
					title = "Frenzying";
					break;
				case 26:
					title = "Mystic";
					break;
				case 27:
					title = "Adept";
					break;
				case 28:
					title = "Masterful";
					break;
				case 29:
					title = "Inept";
					break;
				case 30:
					title = "Ignorant";
					break;
				case 31:
					title = "Deranged";
					break;
				case 32:
					title = "Intense";
					break;
				case 33:
					title = "Taboo";
					break;
				case 34:
					title = "Celestial";
					break;
				case 35:
					title = "Furious";
					break;
				case 52:
					title = "Manic";
					break;
				case 36:
					title = "Keen";
					break;
				case 37:
					title = "Superior";
					break;
				case 38:
					title = "Forceful";
					break;
				case 53:
					title = "Hurtful";
					break;
				case 54:
					title = "Strong";
					break;
				case 55:
					title = "Unpleasant";
					break;
				case 39:
					title = "Broken";
					break;
				case 40:
					title = "Damaged";
					break;
				case 56:
					title = "Weak";
					break;
				case 41:
					title = "Shoddy";
					break;
				case 57:
					title = "Ruthless";
					break;
				case 42:
					title = "Quick";
					break;
				case 43:
					title = "Deadly";
					break;
				case 44:
					title = "Agile";
					break;
				case 45:
					title = "Nimble";
					break;
				case 46:
					title = "Murderous";
					break;
				case 47:
					title = "Slow";
					break;
				case 48:
					title = "Sluggish";
					break;
				case 49:
					title = "Lazy";
					break;
				case 50:
					title = "Annoying";
					break;
				case 51:
					title = "Nasty";
					break;
				case 59:
					title = "Godly";
					break;
				case 60:
					title = "Demonic";
					break;
				case 61:
					title = "Zealous";
					break;
				case 62:
					title = "Hard";
					break;
				case 63:
					title = "Guarding";
					break;
				case 64:
					title = "Armored";
					break;
				case 65:
					title = "Warding";
					break;
				case 66:
					title = "Arcane";
					break;
				case 67:
					title = "Precise";
					break;
				case 68:
					title = "Lucky";
					break;
				case 69:
					title = "Jagged";
					break;
				case 70:
					title = "Spiked";
					break;
				case 71:
					title = "Angry";
					break;
				case 72:
					title = "Menacing";
					break;
				case 73:
					title = "Brisk";
					break;
				case 74:
					title = "Fleeting";
					break;
				case 75:
					title = "Hasty";
					break;
				case 76:
					title = "Quick";
					break;
				case 77:
					title = "Wild";
					break;
				case 78:
					title = "Rash";
					break;
				case 79:
					title = "Intrepid";
					break;
				case 80:
					title = "Violent";
					break;
				case 81:
					title = "Legendary";
					break;
				case 82:
					title = "Unreal";
					break;
				case 83:
					title = "Mythical";
					break;
			}

			return String.Format("{0} {1}", title, Name).Trim();
		}

		public static bool MechSpawn(float x, float y, int type)
		{
			int items = 0;
			int X = 0;
			int Y = 0;
			for (int i = 0; i < 200; i++)
			{
				if (Main.item[i].Active && Main.item[i].Type == type)
				{
					items++;
					Vector2 vector = new Vector2(x, y);
					float itemX = Main.item[i].Position.X - vector.X;
					float itemY = Main.item[i].Position.Y - vector.Y;
					float sRoot = (float)Math.Sqrt((double)(itemX * itemX + itemY * itemY));

					if (sRoot < 300f)
						X++;
					if (sRoot < 800f)
						Y++;
				}
			}
			return X < 3 && Y < 6 && items < 10;
		}

		public static Item netDefaults(int type)
		{
			if (type < 0)
			{
				switch (type)
				{
					case -1:
						return Registries.Item.Create("Gold Pickaxe");
					case -2:
						return Registries.Item.Create("Gold Broadsword");
					case -3:
						return Registries.Item.Create("Gold Shortsword");
					case -4:
						return Registries.Item.Create("Gold Axe");
					case -5:
						return Registries.Item.Create("Gold Hammer");
					case -6:
						return Registries.Item.Create("Gold Bow");
					case -7:
						return Registries.Item.Create("Silver Pickaxe");
					case -8:
						return Registries.Item.Create("Silver Broadsword");
					case -9:
						return Registries.Item.Create("Silver Shortsword");
					case -10:
						return Registries.Item.Create("Silver Axe");
					case -11:
						return Registries.Item.Create("Silver Hammer");
					case -12:
						return Registries.Item.Create("Silver Bow");
					case -13:
						return Registries.Item.Create("Copper Pickaxe");
					case -14:
						return Registries.Item.Create("Copper Broadsword");
					case -15:
						return Registries.Item.Create("Copper Shortsword");
					case -16:
						return Registries.Item.Create("Copper Axe");
					case -17:
						return Registries.Item.Create("Copper Hammer");
					case -18:
						return Registries.Item.Create("Copper Bow");
					case -19:
						return Registries.Item.Create("Blue Phasesaber");
					case -20:
						return Registries.Item.Create("Red Phasesaber");
					case -21:
						return Registries.Item.Create("Green Phasesaber");
					case -22:
						return Registries.Item.Create("Purple Phasesaber");
					case -23:
						return Registries.Item.Create("White Phasesaber");
					case -24:
						return Registries.Item.Create("Yellow Phasesaber");
				}

				ProgramLog.Log("Attempt to set default to an unknown negative ID in netDefaults.");
			}

			return Registries.Item.Create(type);
		}

		/// <summary>
		/// Finds the names of the cobalt armor based on the current release version of Terraria
		/// </summary>
		/// <param name="oldName">Previous release name of item</param>
		/// <param name="release">Release version</param>
		/// <returns>Currently used name for item</returns>
		public static string VersionName(string oldName, int release)
		{
			if (release <= 4)
			{
				switch (oldName)
				{
					case "Cobalt Helmet":
						return "Jungle Hat";
					case "Cobalt Breastplate":
						return "Jungle Shirt";
					case "Cobalt Greaves":
						return "Jungle Pants";
				}
			}
			else if (release <= 13)
			{
				if (oldName == "Jungle Rose")
				{
					return "Jungle Spores";
				}
			}
			else if (release <= 20)
			{
				switch (oldName)
				{
					case "Gills potion":
						return "Gills Potion";
					case "Thorn Chakrum":
						return "Thorn Chakram";
					case "Ball 'O Hurt":
						return "Ball O' Hurt";
				}
			}
			return oldName;
		}

		/// <summary>
		/// Updates specified item's condition
		/// </summary>
		/// <param name="TileRefs">Reference to the ITile method, For usage between Sandbox and Realtime</param>
		/// <param name="i">Item index</param>
		public void UpdateItem(Func<Int32, Int32, ITile> TileRefs, int i)
		{
			if (TileRefs == null)
				TileRefs = TileCollection.ITileAt;

			if (this.Active)
			{
				float addVelocity = 0.1f;
				float maxVelocity = 7f;
				Vector2 value = this.Velocity * 0.5f;
				if (this.Wet)
				{
					maxVelocity = 5f;
					addVelocity = 0.08f;
				}
				if (this.OwnTime > 0)
					this.OwnTime--;
				else
					this.OwnIgnore = -1;
				if (this.KeepTime > 0)
					this.KeepTime--;

				if (!this.BeingGrabbed)
				{
					this.Velocity.Y = this.Velocity.Y + addVelocity;

					if (this.Velocity.Y > maxVelocity)
						this.Velocity.Y = maxVelocity;

					this.Velocity.X = this.Velocity.X * 0.95f;

					if ((double)this.Velocity.X < 0.1 && (double)this.Velocity.X > -0.1)
						this.Velocity.X = 0f;

					this.LavaWet = Collision.LavaCollision(this.Position, this.Width, this.Height);
					if (Collision.WetCollision(this.Position, this.Width, this.Height))
					{
						if (!this.Wet)
						{
							if (this.WetCount == 0)
							{
								this.WetCount = 20;
							}
							this.Wet = true;
						}
					}
					else if (this.Wet)
					{
						this.Wet = false;
					}
					if (!this.Wet)
					{
						this.LavaWet = false;
					}
					if (this.WetCount > 0)
					{
						this.WetCount -= 1;
					}
					if (this.Wet)
					{
						Vector2 vector = this.Velocity;
						this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.Width, this.Height, false, false);
						if (this.Velocity.X != vector.X)
						{
							value.X = this.Velocity.X;
						}
						if (this.Velocity.Y != vector.Y)
						{
							value.Y = this.Velocity.Y;
						}
					}
					else
					{
						this.Velocity = Collision.TileCollision(this.Position, this.Velocity, this.Width, this.Height, false, false);
					}

					if (this.LavaWet)
					{
						if (this.Type == 267)
						{
							Active = false;
							Type = 0;
							Name = String.Empty;
							Stack = 0;
							for (int l = 0; l < NPC.MAX_NPCS; l++)
							{
								if (Main.npcs[l].Active && Main.npcs[l].type == NPCType.N22_GUIDE)
								{
									if (NPC.SpawnWallOfFlesh(TileRefs, Position) == SpawnFlags.SUMMONED)
										if (Main.npcs[l].StrikeNPC(World.Sender, 9999, 10f, -Main.npcs[l].direction))
										{
											NetMessage.SendData(28, -1, -1, String.Empty, l, 9999f, 10f, (float)(-(float)Main.npcs[l].direction), 0);
											break;
										}
								}
							}
							NetMessage.SendData(21);
						}
					}
					else
					{
						if (this.Owner == Main.myPlayer && this.LavaWet && this.Type != 312 && this.Type != 318 && this.Type != 173 && this.Type != 174 && this.Type != 175 && this.Rare == 0)
						{
							if (this.Type == 267)
							{
								var player = Main.players[this.Owner];
								var ctx = new HookContext
								{
									Sender = player
								};

								var args = new HookArgs.PlayerTriggeredEvent
								{
									Type = WorldEventType.BOSS
								};

								HookPoints.PlayerTriggeredEvent.Invoke(ref ctx, ref args);

								if (ctx.CheckForKick())
									return;
								else if (ctx.Result != HookResult.IGNORE)
								{
									ProgramLog.Users.Log("{0} @ {1}: Wall Of Flesh triggered by {2}.", player.IPAddress, this.Owner, player.Name);

									for (int l = 0; l < NPC.MAX_NPCS; l++)
									{
										if (Main.npcs[l].Active && Main.npcs[l].type == NPCType.N22_GUIDE)
										{
											if (NPC.SpawnWallOfFlesh(TileRefs, Position) == SpawnFlags.SUMMONED)
											{
												if (Main.npcs[l].StrikeNPC(World.Sender, 9999, 10f, -Main.npcs[l].direction))
												{
													NetMessage.SendData(28, -1, -1, String.Empty, l, 9999f, 10f, (float)(-(float)Main.npcs[l].direction), 0);
													break;
												}
											}
										}
									}
								}
							}
							this.Active = false;
							this.Type = 0;
							this.Name = String.Empty;
							this.Stack = 0;
							NetMessage.SendData(21, -1, -1, String.Empty, i);
						}
						if (this.Type == 75 && Main.dayTime)
						{
							this.Active = false;
							this.Type = 0;
							this.Stack = 0;
							NetMessage.SendData(21, -1, -1, "", i);
						}
					}
				}
				else
				{
					this.BeingGrabbed = false;
				}
				if (this.SpawnTime < 2147483646)
				{
					this.SpawnTime++;
				}
				if (this.Owner != Main.myPlayer)
				{
					this.Release++;
					if (this.Release >= 300)
					{
						this.Release = 0;
						NetMessage.SendData(39, this.Owner, -1, "", i);
					}
				}
				if (this.Wet)
				{
					this.Position += value;
				}
				else
				{
					this.Position += this.Velocity;
				}
				if (this.NoGrabDelay > 0)
				{
					this.NoGrabDelay--;
				}
			}
		}

		/// <summary>
		/// Creates new item
		/// </summary>
		/// <param name="X">X coordinate of new item location</param>
		/// <param name="Y">Y coordinate of new item location</param>
		/// <param name="Width">Width of new item</param>
		/// <param name="Height">Height of new item</param>
		/// <param name="type">New item type</param>
		/// <param name="stack">How big of a stack to create. Default 1</param>
		/// <param name="noBroadcast">Whether to broadcast item creation or not. Default false</param>
		/// <param name="pfix">Prefix of the new item</param>
		/// <param name="NetID">New NetID</param>
		/// <returns>New item index value</returns>
		public static int NewItem(int X, int Y, int Width, int Height, int type, int stack = 1, bool noBroadcast = false, int pfix = 0, int NetID = 255)
		{
			if (WorldModify.gen)
				return 0;

			int itemIndex = 200;
			for (int i = 0; i < 200; i++)
			{
				if (!Main.item[i].Active)
				{
					itemIndex = i;
					break;
				}
			}

			if (itemIndex == 200)
			{
				int lastSpawned = 0;
				for (int j = 0; j < 200; j++)
				{
					if (Main.item[j].SpawnTime > lastSpawned)
					{
						lastSpawned = Main.item[j].SpawnTime;
						itemIndex = j;
					}
				}
			}

			if (Main.rand == null)
				Main.rand = new Random();

			Main.item[itemIndex] = Registries.Item.Create(type, stack);
			Main.item[itemIndex].SetPrefix(pfix);
			Main.item[itemIndex].Position.X = (float)(X + Width / 2 - Main.item[itemIndex].Width / 2);
			Main.item[itemIndex].Position.Y = (float)(Y + Height / 2 - Main.item[itemIndex].Height / 2);
			Main.item[itemIndex].Wet = Collision.WetCollision(Main.item[itemIndex].Position, Main.item[itemIndex].Width, Main.item[itemIndex].Height);
			Main.item[itemIndex].Velocity.X = (float)Main.rand.Next(-20, 21) * 0.1f;
			Main.item[itemIndex].Velocity.Y = (float)Main.rand.Next(-30, -10) * 0.1f;
			Main.item[itemIndex].SpawnTime = 0;

			if (NetID != 255 && NetID != type)
				Main.item[itemIndex].NetID = NetID;

			if (!noBroadcast)
			{
				NetMessage.SendData(21, -1, -1, String.Empty, itemIndex);
				Main.item[itemIndex].FindOwner(itemIndex);
			}

			return itemIndex;
		}

		/// <summary>
		/// Finds a new owner for the specified item
		/// </summary>
		/// <param name="whoAmI">Item index</param>
		public void FindOwner(int whoAmI)
		{
			if (this.KeepTime > 0)
				return;

			int currentOwner = this.Owner;
			this.Owner = 255;
			float leastDistance = -1f;
			int count = 0;
			foreach (Player player in Main.players)
			{
				if (this.OwnIgnore != count && player.Active && player.ItemSpace(Main.item[whoAmI]))
				{
					float distance = Math.Abs(player.Position.X + (float)(player.Width / 2) - this.Position.X - (float)(this.Width / 2)) + Math.Abs(player.Position.Y + (float)(player.Height / 2) - this.Position.Y - (float)this.Height);
					if (distance < (float)(Main.screenWidth / 2 + Main.screenHeight / 2) && (leastDistance == -1f || distance < leastDistance))
					{
						leastDistance = distance;
						this.Owner = count;
					}
				}
				count++;
			}

			if (this.Owner != currentOwner && ((currentOwner == 255) || !Main.players[currentOwner].Active))
			{
				NetMessage.SendData(21, -1, -1, "", whoAmI);
				if (this.Active)
				{
					NetMessage.SendData(22, -1, -1, "", whoAmI);
				}
			}
		}

		/// <summary>
		/// Clones the item
		/// </summary>
		/// <returns>Cloned object</returns>
		public override object Clone()
		{
			return base.MemberwiseClone();
		}

		/// <summary>
		/// Finds out if current item is the same as another kind of item
		/// </summary>
		/// <param name="compareItem">Item to compare this item to</param>
		/// <returns>True if the same, false if not</returns>
		public bool IsTheSameAs(Item compareItem)
		{
			return this.Name == compareItem.Name;
		}

		public int ReUseDelay;

		public int Critical { get; set; }

		public bool Accessory { get; set; }
	}
}
