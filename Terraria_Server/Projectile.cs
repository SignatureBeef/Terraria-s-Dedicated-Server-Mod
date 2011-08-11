
using System;
using Terraria_Server.Events;
using Terraria_Server.Plugin;
using Terraria_Server.Misc;
using Terraria_Server.Definitions;
using Terraria_Server.Collections;
using Terraria_Server.WorldMod;

namespace Terraria_Server
{
    /// <summary>
    /// Projectile includes things like bullets, arrows, knives, explosives, boomerangs, and possibly ball/chain, orbs, and flamelash/spells.
    /// </summary>
    public class Projectile : BaseEntity
    {
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
        public int[] playerImmune = new int[255];
        /// <summary>
        /// Miscellaneous text associated with the projectile.  Only used in hardcore deaths and sign edits?
        /// </summary>
        public String miscText = "";
        
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
        }

        /// <summary>
        /// Creates a copy of the projectile's instance
        /// </summary>
        /// <returns>Copy of the projectile instance</returns>
        public override object Clone()
        {
            var cloned = (Projectile) base.MemberwiseClone();
            cloned.ai = new float [Projectile.MAX_AI];
            Array.Copy (ai, cloned.ai, Projectile.MAX_AI);
            return cloned;
        }

        public static int NewProjectile(float X, float Y, float SpeedX, float SpeedY, int Type, int Damage, float KnockBack, int Owner = 255)
        {
            return NewProjectile(X, Y, SpeedX, SpeedY, (ProjectileType) Type, Damage, KnockBack, Owner);
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
            int num = 1000;
            for (int i = 0; i < 1000; i++)
            {
                if (!Main.projectile[i].Active)
                {
                    num = i;
                    break;
                }
            }
            if (num == 1000)
            {
                return num;
            }
            Main.projectile[num] = Registries.Projectile.Create((int)Type);
            Main.projectile[num].Position.X = X - (float)Main.projectile[num].Width * 0.5f;
            Main.projectile[num].Position.Y = Y - (float)Main.projectile[num].Height * 0.5f;
            Main.projectile[num].Owner = Owner;
            Main.projectile[num].Velocity.X = SpeedX;
            Main.projectile[num].Velocity.Y = SpeedY;
            Main.projectile[num].damage = Damage;
            Main.projectile[num].knockBack = KnockBack;
            Main.projectile[num].identity = num;
            Main.projectile[num].wet = Collision.WetCollision(Main.projectile[num].Position, Main.projectile[num].Width, Main.projectile[num].Height);
            if (Owner == Main.myPlayer)
            {
                NetMessage.SendData(27, -1, -1, "", num);
            }
            if (Owner == Main.myPlayer)
            {
                if (Type == ProjectileType.BOMB)
                {
                    Main.projectile[num].timeLeft = 180;
                }
                if (Type == ProjectileType.DYNAMITE)
                {
                    Main.projectile[num].timeLeft = 300;
                }
                if (Type == ProjectileType.GRENADE)
                {
                    Main.projectile[num].timeLeft = 180;
                }
                if (Type == ProjectileType.BOMB_STICKY)
                {
                    Main.projectile[num].timeLeft = 180;
                }
            }
            return num;
        }

		public void StatusNPC (NPC npc)
		{
			switch (this.type)
			{
				case ProjectileType.ARROW_FIRE:
					if (Main.rand.Next(3) == 0)
						npc.AddBuff(24, 180, false);
					break;
				
				case ProjectileType.BALL_OF_FIRE:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(24, 300, false);
					break;
					
				case ProjectileType.FLAMARANG:
					if (Main.rand.Next(5) == 0)
						npc.AddBuff(24, 180, false);
					break;
					
				case ProjectileType.FLAMELASH:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(24, 240, false);
					break;
					
				case ProjectileType.SUNFURY:
					if (Main.rand.Next(4) == 0)
						npc.AddBuff(24, 180, false);
					break;
					
				case ProjectileType.CHAKRUM_THORN:
					if (Main.rand.Next(5) == 0)
						npc.AddBuff(20, 420, false);
					break;
					
				case ProjectileType.KNIFE_POISONED:
					if (Main.rand.Next(2) == 0)
						npc.AddBuff(20, 600, false);
					break;
					
			}
		}
		
		public void StatusPvP (Player player)
		{
			switch (this.type)
			{
				case ProjectileType.ARROW_FIRE:
					if (Main.rand.Next(3) == 0)
						player.AddBuff(24, 180, false);
					break;
				
				case ProjectileType.BALL_OF_FIRE:
					if (Main.rand.Next(2) == 0)
						player.AddBuff(24, 300, false);
					break;
					
				case ProjectileType.FLAMARANG:
					if (Main.rand.Next(5) == 0)
						player.AddBuff(24, 180, false);
					break;
					
				case ProjectileType.FLAMELASH:
					if (Main.rand.Next(2) == 0)
						player.AddBuff(24, 240, false);
					break;
					
				case ProjectileType.SUNFURY:
					if (Main.rand.Next(4) == 0)
						player.AddBuff(24, 180, false);
					break;
					
				case ProjectileType.CHAKRUM_THORN:
					if (Main.rand.Next(5) == 0)
						player.AddBuff(20, 420, false);
					break;
					
				case ProjectileType.KNIFE_POISONED:
					if (Main.rand.Next(2) == 0)
						player.AddBuff(20, 600, false);
					break;
					
			}
		}
		
		public void StatusPlayer (Player player)
		{
			switch (this.type)
			{
				case ProjectileType.STINGER:
					if (Main.rand.Next(3) == 0)
						player.AddBuff(20, 600, true);
					break;
					
				case ProjectileType.SICKLE_DEMON:
					if (Main.rand.Next(3) == 0)
						player.AddBuff(22, 900, true);
					break;
					
			}
		}

        /// <summary>
        /// Runs damage calculation on hostile mobs and players
        /// </summary>
        public void Damage()
        {
            int playerIndex = Main.myPlayer;
            Player player = Main.players[playerIndex];

            Rectangle rectangle = new Rectangle((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height);
            if (this.friendly && this.type != ProjectileType.ORB_OF_LIGHT)
            {
                if (this.Owner == playerIndex)
                {
                    if ((this.aiStyle == 16 || this.type == ProjectileType.ARROW_HELLFIRE) && this.timeLeft <= 1)
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
								int dmg = Main.DamageVar ((float)this.damage);
								this.StatusPlayer (player);
								
								player.Hurt(dmg, this.direction, true, false, Player.getDeathMessage(this.Owner, -1, this.whoAmI, -1));
								NetMessage.SendData(26, -1, -1, Player.getDeathMessage(this.Owner, -1, this.whoAmI, -1), playerIndex, (float)this.direction, (float)dmg, 1f);
                            }
                        }
                    }
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
                            if (Main.tile.At(i, j + 1).Exists && Main.tileCut[(int)Main.tile.At(i, j).Type] && Main.tile.At(i, j + 1).Type != 78)
                            {
                                WorldModify.KillTile(i, j, false, false, false);
                                NetMessage.SendData(17, -1, -1, "", 0, (float)i, (float)j);
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
                                if (this.type == ProjectileType.POWDER_VILE && (npc.type == NPCType.N47_CORRUPT_BUNNY || npc.type == NPCType.N57_CORRUPT_GOLDFISH))
                                {
                                    flag = true;
                                }
                                else
                                {
                                    if (this.type == ProjectileType.BALL_SAND_DROP && npc.type == NPCType.N69_ANTLION)
                                    {
                                        flag = true;
                                    }
                                }
                                if (!flag && (npc.noTileCollide || !this.ownerHitCheck || Collision.CanHit(Main.players[this.Owner].Position, Main.players[this.Owner].Width, Main.players[this.Owner].Height, npc.Position, npc.Width, npc.Height)))
                                {
                                    if(npc.Intersects(rectangle))
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
                                        if (this.type == ProjectileType.ARROW_HELLFIRE && this.timeLeft > 1)
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
                                        int dmg = Main.DamageVar (this.damage);
                                        
                                        this.StatusNPC (npc);
                                        npc.StrikeNPC (dmg, this.knockBack, this.direction, crit);
                                        
                                        NetMessage.SendData(28, -1, -1, "", i, (float)dmg, this.knockBack, (float)this.direction, crit ? 1 : 0);

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
                                    if (this.type == ProjectileType.ARROW_HELLFIRE && this.timeLeft > 1)
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
                                    int dmg = Main.DamageVar (this.damage);
                                    
                                    if (!playerIt.immune) this.StatusPvP (playerIt);
                                    playerIt.Hurt (dmg, this.direction, true, false, Player.getDeathMessage(this.Owner, -1, this.whoAmI, -1), crit);
                                                                        
                                    NetMessage.SendData(26, -1, -1, Player.getDeathMessage(this.Owner, -1, this.whoAmI, -1), i, (float)this.direction, (float)dmg, 1f, crit ? 1 : 0);

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
                if (this.type == ProjectileType.POWDER_VILE)
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
                                    NPC.Transform(i, 47);
                                }
                            }
                            else
                            {
                                if (npc.type == NPCType.N55_GOLDFISH)
                                {
                                    if (npc.Intersects(rectangle))
                                    {
                                        NPC.Transform(i, 57);
                                    }
                                }
                            }
                        }
                    }
                }
            }
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
        }

        /// <summary>
        /// Updates the projectile's position, damage variables, etc.
        /// </summary>
        /// <param name="i">Projectile index</param>
        public void Update(int i)
        {
            if (this.Active)
            {
                Vector2 value = this.Velocity;
                if (this.Position.X <= Main.leftWorld || this.Position.X + (float)this.Width >= Main.rightWorld || this.Position.Y <= Main.topWorld || this.Position.Y + (float)this.Height >= Main.bottomWorld)
                {
                    this.Active = false;
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
                if (this.Owner < 255 && !Main.players[this.Owner].Active)
                {
                    this.Kill();
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
                        return;
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
                if (this.tileCollide)
                {
                    Vector2 value2 = this.Velocity;
                    bool flag3 = true;
                    if (this.type == ProjectileType.STARFURY || this.type == ProjectileType.FALLEN_STAR || this.type == ProjectileType.BALL_OF_FIRE || this.type == ProjectileType.HOOK || this.type == ProjectileType.BALL_SAND_DROP || this.type == ProjectileType.BALL_MUD || this.type == ProjectileType.BALL_ASH)
                    {
                        flag3 = false;
                    }
                    if (this.aiStyle == 10)
                    {
                        if (this.type == ProjectileType.BALL_SAND_GUN || (this.type == ProjectileType.BALL_SAND_DROP && this.ai[0] == 2f))
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
                    if (value2 != this.Velocity)
                    {
                        if (this.type == ProjectileType.SHOT_METEOR)
                        {
                            if (this.penetrate > 1)
                            {
                                Collision.HitTiles(this.Position, this.Velocity, this.Width, this.Height);
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
                                this.Kill();
                            }
                        }
                        else
                        {
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
										Collision.HitTiles(this.Position, this.Velocity, this.Width, this.Height);
									}
								}
								else if (this.aiStyle == 3 || this.aiStyle == 13)
                                {
                                    Collision.HitTiles(this.Position, this.Velocity, this.Width, this.Height);
                                    if (this.type == ProjectileType.CHAKRUM_THORN)
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
                                    if (this.aiStyle == 8)
                                    {
                                        this.ai[0] += 1f;
                                        if (this.ai[0] >= 5f)
                                        {
                                            this.Position += this.Velocity;
                                            this.Kill();
                                        }
                                        else
                                        {
                                            if (this.type == ProjectileType.BALL_MUSKET && this.Velocity.Y > 4f)
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
                                            if (this.type == ProjectileType.GLOWSTICK)
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
                                                    if (this.type == ProjectileType.DYNAMITE)
                                                    {
                                                        this.Velocity.X = this.Velocity.X * 0.8f;
                                                    }
                                                }
                                                if (this.Velocity.Y != value2.Y && (double)value2.Y > 0.7)
                                                {
                                                    this.Velocity.Y = value2.Y * -0.4f;
                                                    if (this.type == ProjectileType.DYNAMITE)
                                                    {
                                                        this.Velocity.Y = this.Velocity.Y * 0.8f;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                this.Position += this.Velocity;
                                                this.Kill();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (this.type == ProjectileType.VILETHORN || this.type == ProjectileType.VILETHORN_B)
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
                this.Damage();
                this.timeLeft--;
                if (this.timeLeft <= 0)
                {
                    this.Kill();
                }
                if (this.penetrate == 0)
                {
                    this.Kill();
                }
                if (this.Active && this.netUpdate && this.Owner == Main.myPlayer)
                {
                    NetMessage.SendData(27, -1, -1, "", i);
                }
                if (this.Active && this.maxUpdates > 0)
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

        /// <summary>
        /// Moves the projectile according to the projectile's motion parameters, or AI
        /// </summary>
        public void AI()
        {
            if (this.aiStyle == 1)
            {
                if (this.type == ProjectileType.LASER_GREEN || this.type == ProjectileType.BALL_MUSKET || this.type == ProjectileType.SHOT_METEOR)
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
                if (this.type != ProjectileType.ARROW_JESTER && this.type != ProjectileType.BALL_MUSKET && this.type != ProjectileType.LASER_GREEN && this.type != ProjectileType.SHOT_METEOR && this.type != ProjectileType.FEATHER_HARPY && this.type != ProjectileType.STINGER)
                {
                    this.ai[0] += 1f;
                }
                if (this.ai[0] >= 15f)
                {
                    this.ai[0] = 15f;
                    this.Velocity.Y = this.Velocity.Y + 0.1f;
                }
                this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 1.57f;
                if (this.Velocity.Y > 16f)
                {
                    this.Velocity.Y = 16f;
                    return;
                }
            }
            else
            {
                if (this.aiStyle == 2)
                {
                    this.rotation += (Math.Abs(this.Velocity.X) + Math.Abs(this.Velocity.Y)) * 0.03f * (float)this.direction;
                    this.ai[0] += 1f;
                    if (this.ai[0] >= 20f)
                    {
                        this.Velocity.Y = this.Velocity.Y + 0.4f;
                        this.Velocity.X = this.Velocity.X * 0.97f;
                    }
                    else
                    {
                        if (this.type == ProjectileType.KNIFE_THROWING || this.type == ProjectileType.KNIFE_POISONED)
                        {
                            this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 1.57f;
                        }
                    }
                    if (this.Velocity.Y > 16f)
                    {
                        this.Velocity.Y = 16f;
                    }
                    if (this.type == ProjectileType.KNIFE_POISONED && Main.rand.Next(20) == 0)
                    {
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
                            float num4 = 9f;
                            float num5 = 0.4f;
                            if (this.type == ProjectileType.FLAMARANG)
                            {
                                num4 = 13f;
                                num5 = 0.6f;
                            }
                            else
                            {
                                if (this.type == ProjectileType.CHAKRUM_THORN)
                                {
                                    num4 = 15f;
                                    num5 = 0.8f;
                                }
                            }
                            Vector2 vector = new Vector2(this.Position.X + (float)this.Width * 0.5f, this.Position.Y + (float)this.Height * 0.5f);
                            float num6 = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2) - vector.X;
                            float num7 = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].Height / 2) - vector.Y;
                            float num8 = (float)Math.Sqrt((double)(num6 * num6 + num7 * num7));
                            num8 = num4 / num8;
                            num6 *= num8;
                            num7 *= num8;
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
                                if(Main.players[Owner].Intersects(rectangle))
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
                                if (this.type == ProjectileType.VILETHORN && Main.myPlayer == this.Owner)
                                {
                                    int num9 = (int)this.type;
                                    if (this.ai[1] >= 6f)
                                    {
                                        num9++;
                                    }
                                    int num10 = Projectile.NewProjectile(this.Position.X + this.Velocity.X + (float)(this.Width / 2), this.Position.Y + this.Velocity.Y + (float)(this.Height / 2), this.Velocity.X, this.Velocity.Y, (ProjectileType)Enum.ToObject(typeof(ProjectileType),num9), this.damage, this.knockBack, this.Owner);
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
                                this.Kill();
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (this.aiStyle == 5)
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
							if (this.ai[1] == 1f)
							{
								this.light = 0.9f;
							}
                        }
                        else
                        {
                            if (this.aiStyle == 6)
                            {
                                this.Velocity *= 0.95f;
                                this.ai[0] += 1f;
                                if (this.ai[0] == 180f)
                                {
                                    this.Kill();
                                }
                                if (this.ai[1] == 0f)
                                {
                                    this.ai[1] = 1f;
                                }
                                if (this.type == ProjectileType.POWDER_PURIFICATION)
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
                                            if (this.Position.X + (float)this.Width > vector2.X && this.Position.X < vector2.X + 16f && this.Position.Y + (float)this.Height > vector2.Y && this.Position.Y < vector2.Y + 16f && Main.myPlayer == this.Owner && Main.tile.At(l, m).Active)
                                            {
                                                if (Main.tile.At(l, m).Type == 23)
                                                {
                                                    Main.tile.At(l, m).SetType (2);
                                                    WorldModify.SquareTileFrame(l, m, true);
                                                }
                                                if (Main.tile.At(l, m).Type == 25)
                                                {
                                                    Main.tile.At(l, m).SetType (1);
                                                    WorldModify.SquareTileFrame(l, m, true);
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
                                    if (Main.players[this.Owner].dead)
                                    {
                                        this.Kill();
                                        return;
                                    }
                                    Vector2 vector3 = new Vector2(this.Position.X + (float)this.Width * 0.5f, this.Position.Y + (float)this.Height * 0.5f);
                                    float num15 = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2) - vector3.X;
                                    float num16 = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].Height / 2) - vector3.Y;
                                    float num17 = (float)Math.Sqrt((double)(num15 * num15 + num16 * num16));
                                    this.rotation = (float)Math.Atan2((double)num16, (double)num15) - 1.57f;
                                    if (this.ai[0] == 0f)
                                    {
                                        if ((num17 > 300f && this.type == ProjectileType.HOOK) || (num17 > 400f && this.type == ProjectileType.WHIP_IVY))
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
                                                if (this.Position.X + (float)this.Width > vector4.X && this.Position.X < vector4.X + 16f && this.Position.Y + (float)this.Height > vector4.Y && this.Position.Y < vector4.Y + 16f && Main.tile.At(n, num22).Active && Main.tileSolid[(int)Main.tile.At(n, num22).Type])
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
                                                            Main.projectile[num24].Kill();
                                                        }
                                                    }
                                                    WorldModify.KillTile(n, num22, true, true, false);
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
                                        if (this.type == ProjectileType.WHIP_IVY)
                                        {
                                            num27 = 15f;
                                        }
                                        if (num17 < 24f)
                                        {
                                            this.Kill();
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
                                                if (this.Position.X + (float)(this.Width / 2) > vector5.X && this.Position.X + (float)(this.Width / 2) < vector5.X + 16f && this.Position.Y + (float)(this.Height / 2) > vector5.Y && this.Position.Y + (float)(this.Height / 2) < vector5.Y + 16f && Main.tile.At(num32, num33).Active && Main.tileSolid[(int)Main.tile.At(num32, num33).Type])
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
                                else
                                {
                                    if (this.aiStyle == 8)
                                    {
                                        if (this.type != ProjectileType.BOLT_WATER)
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
                                    else
                                    {
                                        if (this.aiStyle == 9)
                                        {
                                            if (Main.myPlayer == this.Owner && this.ai[0] == 0f)
                                            {
                                                // client-code, not updated from 1.0.6
                                                if (!Main.players[this.Owner].channel)
                                                {
                                                    this.Kill();
                                                }
                                            }
                                            if (this.type == ProjectileType.FLAMELASH)
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
                                        else
                                        {
                                            if (this.aiStyle == 10)
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
                                                    if (this.type == ProjectileType.BALL_SAND_GUN)
                                                    {
                                                        this.ai[1] += 1f;
                                                        if (this.ai[1] >= 15f)
                                                        {
                                                            this.ai[1] = 15f;
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
                                            else
                                            {
                                                if (this.aiStyle == 11)
                                                {
                                                    this.rotation += 0.02f;
                                                    if (Main.myPlayer == this.Owner)
                                                    {
														if (Main.players[this.Owner].lightOrb)
														{
															this.timeLeft = 2;
														}
                                                        if (Main.players[this.Owner].dead)
                                                        {
                                                            this.Kill();
                                                            return;
                                                        }
                                                        float num51 = 2.5f;
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
                                                        if (num54 > 64f)
                                                        {
                                                            num54 = num51 / num54;
                                                            num52 *= num54;
                                                            num53 *= num54;
                                                            if (num52 != this.Velocity.X || num53 != this.Velocity.Y)
                                                            {
                                                                this.netUpdate = true;
                                                            }
                                                            this.Velocity.X = num52;
                                                            this.Velocity.Y = num53;
                                                            return;
                                                        }
                                                        if (this.Velocity.X != 0f || this.Velocity.Y != 0f)
                                                        {
                                                            this.netUpdate = true;
                                                        }
                                                        this.Velocity.X = 0f;
                                                        this.Velocity.Y = 0f;
                                                        return;
                                                    }
                                                }
                                                else
                                                {
                                                    if (this.aiStyle == 12)
                                                    {
                                                        this.scale -= 0.04f;
                                                        if (this.scale <= 0f)
                                                        {
                                                            this.Kill();
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
                                                    if (this.aiStyle == 13)
                                                    {
                                                        if (Main.players[this.Owner].dead)
                                                        {
                                                            this.Kill();
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
                                                                    this.Kill();
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
                                                    else
                                                    {
                                                        if (this.aiStyle == 14)
                                                        {
                                                            if (this.type == ProjectileType.GLOWSTICK_STICKY)
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
                                                                            if ((Main.tile.At(num70, num71).Active) && (Main.tileSolid[Main.tile.At(num70, num71).Type] || (Main.tileSolidTop[Main.tile.At(num70, num71).Type] && (Main.tile.At(num70, num71).FrameY == 0))))
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
                                                            return;
                                                        }
                                                        if (this.aiStyle == 15)
                                                        {
                                                            if (Main.players[this.Owner].dead)
                                                            {
                                                                this.Kill();
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
                                                                this.tileCollide = true;
                                                                if (num75 > 160f)
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
																	Math.Abs(num73);
																	Math.Abs(num74);
																	if (this.ai[1] == 1f)
																	{
																		this.tileCollide = false;
																	}
																	if (!owner.channel || num75 > 300f || !this.tileCollide)
																	{
																		this.ai[1] = 1f;
																		if (this.tileCollide)
																		{
																			this.netUpdate = true;
																		}
																		this.tileCollide = false;
																		if (num75 < 20f)
																		{
																			this.Kill();
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
                                                        else
                                                        {
                                                            if (this.aiStyle == 16)
                                                            {
                                                                if (this.type == ProjectileType.BOMB_STICKY)
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
                                                                                if (Main.tile.At(num76, num77).Active && (Main.tileSolid[(int)Main.tile.At(num76, num77).Type] || (Main.tileSolidTop[(int)Main.tile.At(num76, num77).Type] && Main.tile.At(num76, num77).FrameY == 0)))
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
                                                                if (this.Owner == Main.myPlayer && this.timeLeft <= 3)
                                                                {
                                                                    this.ai[1] = 0f;
                                                                    this.alpha = 255;
                                                                    if (this.type == ProjectileType.BOMB || this.type == ProjectileType.BOMB_STICKY)
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
                                                                        if (this.type == ProjectileType.DYNAMITE)
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
                                                                            if (this.type == ProjectileType.GRENADE)
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
                                                                    if (this.type != ProjectileType.GRENADE && Main.rand.Next(4) == 0)
                                                                    {
                                                                        if (this.type != ProjectileType.GRENADE)
                                                                        {
                                                                            this.damage = 0;
                                                                        }
                                                                    }
                                                                }
                                                                this.ai[0] += 1f;
                                                                if ((this.type == ProjectileType.GRENADE && this.ai[0] > 10f) || (this.type != ProjectileType.GRENADE && this.ai[0] > 5f))
                                                                {
                                                                    this.ai[0] = 10f;
                                                                    if (this.Velocity.Y == 0f && this.Velocity.X != 0f)
                                                                    {
                                                                        this.Velocity.X = this.Velocity.X * 0.97f;
                                                                        if (this.type == ProjectileType.DYNAMITE)
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
                                                            if (this.aiStyle == 17)
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
                                                                    if (Main.tile.At(num78, num79).Exists && !Main.tile.At(num78, num79).Active)
                                                                    {
                                                                        WorldModify.PlaceTile(num78, num79, 85, false, false, -1, 0);
                                                                        if (Main.tile.At(num78, num79).Active)
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
                                                                                //Program.server.PluginManager.processHook(Hooks.PLAYER_EDITSIGN, playerEvent);
                                                                                //if (playerEvent.Cancelled)
                                                                                //{
                                                                                //    return;
                                                                                //}

                                                                                Sign.TextSign(num80, this.miscText);
                                                                            }
                                                                            this.Kill();
                                                                            return;
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                if (this.aiStyle == 18)
                                                                {
                                                                    if (this.ai[1] == 0f && this.type == ProjectileType.SICKLE_DEMON)
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
                                                                if (this.aiStyle == 19)
                                                                {
                                                                    this.direction = Main.players[this.Owner].direction;
                                                                    Main.players[this.Owner].heldProj = this.whoAmI;
                                                                    Main.players[this.Owner].itemTime = Main.players[this.Owner].itemAnimation;
                                                                    this.Position.X = Main.players[this.Owner].Position.X + (float)(Main.players[this.Owner].Width / 2) - (float)(this.Width / 2);
                                                                    this.Position.Y = Main.players[this.Owner].Position.Y + (float)(Main.players[this.Owner].Height / 2) - (float)(this.Height / 2);
                                                                    if (this.type == ProjectileType.LANCE_DARK)
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
                                                                        if (this.type == ProjectileType.TRIDENT)
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
                                                                        else
                                                                        {
                                                                            if (this.type == ProjectileType.SPEAR)
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
                                                                        }
                                                                    }
                                                                    this.Position += this.Velocity * this.ai[0];
                                                                    if (Main.players[this.Owner].itemAnimation == 0)
                                                                    {
                                                                        this.Kill();
                                                                    }
                                                                    this.rotation = (float)Math.Atan2((double)this.Velocity.Y, (double)this.Velocity.X) + 2.355f;
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

        /// <summary>
        /// Destroys the projectile
        /// </summary>
        public void Kill()
        {
            if (!this.Active)
            {
                return;
            }
            this.timeLeft = 0;
            switch(this.type)
            {
                case ProjectileType.BALL_MUSKET:
                case ProjectileType.LASER_GREEN:
                case ProjectileType.SHOT_METEOR:
                    {
                        Collision.HitTiles(this.Position, this.Velocity, this.Width, this.Height);
                    }
                    break;
                    
                case ProjectileType.BOMB:
                case ProjectileType.GRENADE:
                case ProjectileType.BOMB_STICKY:
                    {
                        this.Position.X = this.Position.X + (float)(this.Width / 2);
                        this.Position.Y = this.Position.Y + (float)(this.Height / 2);
                        this.Width = 22;
                        this.Height = 22;
                        this.Position.X = this.Position.X - (float)(this.Width / 2);
                        this.Position.Y = this.Position.Y - (float)(this.Height / 2);
                    }
                    break;
                    
                case ProjectileType.DYNAMITE:
                    {
                        this.Position.X = this.Position.X + (float)(this.Width / 2);
                        this.Position.Y = this.Position.Y + (float)(this.Height / 2);
                        this.Width = 200;
                        this.Height = 200;
                        this.Position.X = this.Position.X - (float)(this.Width / 2);
                        this.Position.Y = this.Position.Y - (float)(this.Height / 2);
                        
                        this.Position.X = this.Position.X + (float)(this.Width / 2);
                        this.Position.Y = this.Position.Y + (float)(this.Height / 2);
                        this.Width = 10;
                        this.Height = 10;
                        this.Position.X = this.Position.X - (float)(this.Width / 2);
                        this.Position.Y = this.Position.Y - (float)(this.Height / 2);
                    }
                    break;
                    
                case ProjectileType.ARROW_HELLFIRE:
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
                            this.Damage();
                        }
                    }
                    break;
                    
                default:
                    break;
            }
            if (this.Owner == Main.myPlayer)
            {
                if (this.type == ProjectileType.BOMB || this.type == ProjectileType.DYNAMITE || this.type == ProjectileType.BOMB_STICKY)
                {
                    int num38 = 3;
                    if (this.type == ProjectileType.DYNAMITE)
                    {
                        num38 = 7;
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
                            if (num47 < (double)num38 && Main.tile.At(num43, num44).Exists && Main.tile.At(num43, num44).Wall == 0)
                            {
                                flag = true;
                                break;
                            }
                        }
                    }
                    for (int num48 = num39; num48 <= num40; num48++)
                    {
                        for (int num49 = num41; num49 <= num42; num49++)
                        {
                            float num50 = Math.Abs((float)num48 - this.Position.X / 16f);
                            float num51 = Math.Abs((float)num49 - this.Position.Y / 16f);
                            double num52 = Math.Sqrt((double)(num50 * num50 + num51 * num51));
                            if (num52 < (double)num38)
                            {
                                bool flag2 = true;
                                if (Main.tile.At(num48, num49).Active)
                                {
                                    flag2 = false;
                                    if (this.type == ProjectileType.BOMB || this.type == ProjectileType.BOMB_STICKY)
                                    {
                                        if (!Main.tileSolid[(int)Main.tile.At(num48, num49).Type] || Main.tileSolidTop[(int)Main.tile.At(num48, num49).Type] || Main.tile.At(num48, num49).Type == 0 || Main.tile.At(num48, num49).Type == 1 || Main.tile.At(num48, num49).Type == 2 || Main.tile.At(num48, num49).Type == 23 || Main.tile.At(num48, num49).Type == 30 || Main.tile.At(num48, num49).Type == 40 || Main.tile.At(num48, num49).Type == 6 || Main.tile.At(num48, num49).Type == 7 || Main.tile.At(num48, num49).Type == 8 || Main.tile.At(num48, num49).Type == 9 || Main.tile.At(num48, num49).Type == 10 || Main.tile.At(num48, num49).Type == 53 || Main.tile.At(num48, num49).Type == 54 || Main.tile.At(num48, num49).Type == 57 || Main.tile.At(num48, num49).Type == 59 || Main.tile.At(num48, num49).Type == 60 || Main.tile.At(num48, num49).Type == 63 || Main.tile.At(num48, num49).Type == 64 || Main.tile.At(num48, num49).Type == 65 || Main.tile.At(num48, num49).Type == 66 || Main.tile.At(num48, num49).Type == 67 || Main.tile.At(num48, num49).Type == 68 || Main.tile.At(num48, num49).Type == 70 || Main.tile.At(num48, num49).Type == 37)
                                        {
                                            flag2 = true;
                                        }
                                    }
                                    else
                                    {
                                        if (this.type == ProjectileType.DYNAMITE)
                                        {
                                            flag2 = true;
                                        }
                                    }
                                    if (Main.tileDungeon[(int)Main.tile.At(num48, num49).Type] || Main.tile.At(num48, num49).Type == 26 || Main.tile.At(num48, num49).Type == 58 || Main.tile.At(num48, num49).Type == 21)
                                    {
                                        flag2 = false;
                                    }
                                    if (flag2)
                                    {
                                        WorldModify.KillTile(num48, num49, false, false, false);
                                    }
                                }
                                if (flag2 && Main.tile.At(num48, num49).Exists && Main.tile.At(num48, num49).Wall > 0 && flag)
                                {
                                    WorldModify.KillWall(num48, num49, false);
                                }
                            }
                        }
                    }
                }
                
                NetMessage.SendData(29, -1, -1, "", this.identity, (float)this.Owner);

                int num53 = -1;
                if (this.aiStyle == 10)
                {
                    int num54 = (int)(this.Position.X + (float)(this.Width / 2)) / 16;
                    int num55 = (int)(this.Position.Y + (float)(this.Width / 2)) / 16;
                    int num56 = 0;
                    int num57 = 2;
                    if (this.type == ProjectileType.BALL_SAND_DROP)
                    {
                        num56 = 53;
                        num57 = 0;
                    }
                    if (this.type == ProjectileType.BALL_SAND_GUN)
                    {
                        num56 = 53;
                        num57 = 0;
                    }
                    else
                    {
                        if (this.type == ProjectileType.BALL_MUD)
                        {
                            num56 = 59;
                            num57 = 176;
                        }
                        else
                        {
                            if (this.type == ProjectileType.BALL_ASH)
                            {
                                num56 = 57;
                                num57 = 172;
                            }
                        }
                    }
                    if (!Main.tile.At(num54, num55).Active)
                    {
                        WorldModify.PlaceTile(num54, num55, num56, false, true, -1, 0);
                        if (Main.tile.At(num54, num55).Active && (int)Main.tile.At(num54, num55).Type == num56)
                        {
                            NetMessage.SendData(17, -1, -1, "", 1, (float)num54, (float)num55, (float)num56);
                        }
                        else
                        {
                            if (num57 > 0)
                            {
                                num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, num57, 1, false);
                            }
                        }
                    }
                    else
                    {
                        if (num57 > 0)
                        {
                            num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, num57, 1, false);
                        }
                    }
                }
                if (this.type == ProjectileType.ARROW_WOODEN && Main.rand.Next(2) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 40, 1, false);
                }
                if (this.type == ProjectileType.ARROW_FIRE && Main.rand.Next(2) == 0)
                {
                    if (Main.rand.Next(3) == 0)
                    {
                        num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 41, 1, false);
                    }
                    else
                    {
                        num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 40, 1, false);
                    }
                }
                if (this.type == ProjectileType.GLOWSTICK && Main.rand.Next(3) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 282, 1, false);
                }
                if (this.type == ProjectileType.GLOWSTICK_STICKY && Main.rand.Next(3) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 286, 1, false);
                }
                if (this.type == ProjectileType.KNIFE_THROWING && Main.rand.Next(2) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 279, 1, false);
                }
                if (this.type == ProjectileType.KNIFE_POISONED && Main.rand.Next(2) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 287, 1, false);
                }
                if (this.type == ProjectileType.SHURIKEN && Main.rand.Next(2) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 42, 1, false);
                }
                if (this.type == ProjectileType.ARROW_UNHOLY && Main.rand.Next(2) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 47, 1, false);
                }
                if (this.type == ProjectileType.FALLEN_STAR && this.damage > 100)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 75, 1, false);
                }
                if (this.type == ProjectileType.BONE && Main.rand.Next(2) == 0)
                {
                    num53 = Item.NewItem((int)this.Position.X, (int)this.Position.Y, this.Width, this.Height, 154, 1, false);
                }
            }
            this.Active = false;
        }

        /// <summary>
        /// Creates a new color by combining newColor with the projectiles alpha value
        /// </summary>
        /// <param name="newColor">New color to combine</param>
        /// <returns>Combined color</returns>
        public Color GetAlpha(Color newColor)
        {
            int r;
            int g;
            int b;
            if (this.type == ProjectileType.STARFURY || this.type == ProjectileType.BALL_OF_FIRE || this.type == ProjectileType.FLAMELASH || this.type == ProjectileType.GLOWSTICK || this.type == ProjectileType.GLOWSTICK_STICKY)
            {
                r = (int)newColor.R - this.alpha / 3;
                g = (int)newColor.G - this.alpha / 3;
                b = (int)newColor.B - this.alpha / 3;
            }
            else
            {
                if (this.type == ProjectileType.MISSILE_MAGIC || this.type == ProjectileType.ORB_OF_LIGHT || this.type == ProjectileType.SICKLE_DEMON || this.type == ProjectileType.SCYTHE_DEMON)
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
