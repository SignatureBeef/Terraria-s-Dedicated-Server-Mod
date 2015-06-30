using System;

#if Full_API
using Terraria;
#endif
using Microsoft.Xna.Framework;
using tdsm.api.Command;

namespace tdsm.api.Plugin
{
    public static class HookPoints
    {
        public static readonly HookPoint<HookArgs.ConfigurationLine> ConfigurationLine;
        public static readonly HookPoint<HookArgs.StartDefaultServer> StartDefaultServer;
        //public static readonly HookPoint<HookArgs.StatusTextChanged> StatusTextChanged;
        //public static readonly HookPoint<HookArgs.UpdateServer> UpdateServer;
        public static readonly HookPoint<HookArgs.InvasionNPCSpawn> InvasionNPCSpawn;

        public static readonly HookPoint<HookArgs.NewConnection> NewConnection;
        public static readonly HookPoint<HookArgs.ServerStateChange> ServerStateChange;
        public static readonly HookPoint<HookArgs.PluginLoadRequest> PluginLoadRequest;
        //public static readonly HookPoint<HookArgs.UnkownSendPacket> UnkownSendPacket;
        //public static readonly HookPoint<HookArgs.UnkownReceivedPacket> UnkownReceivedPacket;
        public static readonly HookPoint<HookArgs.SendNetData> SendNetData; //[TODO] determine if this should be a Action<...>

        public static readonly HookPoint<HookArgs.ConnectionRequestReceived> ConnectionRequestReceived;
        public static readonly HookPoint<HookArgs.DisconnectReceived> DisconnectReceived;
        public static readonly HookPoint<HookArgs.ServerPassReceived> ServerPassReceived;
        public static readonly HookPoint<HookArgs.PlayerPassReceived> PlayerPassReceived;
        public static readonly HookPoint<HookArgs.PlayerDataReceived> PlayerDataReceived;
        public static readonly HookPoint<HookArgs.PlayerAuthenticationChanging> PlayerAuthenticationChanging;
        public static readonly HookPoint<HookArgs.PlayerAuthenticationChanged> PlayerAuthenticationChanged;
        public static readonly HookPoint<HookArgs.StateUpdateReceived> StateUpdateReceived;
        public static readonly HookPoint<HookArgs.InventoryItemReceived> InventoryItemReceived;
        public static readonly HookPoint<HookArgs.ObituaryReceived> ObituaryReceived;
        //public static readonly HookPoint<HookArgs.PlayerTeleport> PlayerTeleport;

        public static readonly HookPoint<HookArgs.PlayerWorldAlteration> PlayerWorldAlteration;

        //public static readonly HookPoint<HookArgs.DoorStateChanged> DoorStateChanged;

        public static readonly HookPoint<HookArgs.LiquidFlowReceived> LiquidFlowReceived;
        public static readonly HookPoint<HookArgs.ProjectileReceived> ProjectileReceived;
        public static readonly HookPoint<HookArgs.KillProjectileReceived> KillProjectileReceived;
        public static readonly HookPoint<HookArgs.TileSquareReceived> TileSquareReceived;

        //public static readonly HookPoint<HookArgs.Explosion> Explosion;

        public static readonly HookPoint<HookArgs.ChestBreakReceived> ChestBreakReceived;
        public static readonly HookPoint<HookArgs.ChestOpenReceived> ChestOpenReceived;

        public static readonly HookPoint<HookArgs.PvpSettingReceived> PvpSettingReceived;
        public static readonly HookPoint<HookArgs.PartySettingReceived> PartySettingReceived;

        public static readonly HookPoint<HookArgs.PlayerEnteringGame> PlayerEnteringGame;
        public static readonly HookPoint<HookArgs.PlayerEnteredGame> PlayerEnteredGame;
        public static readonly HookPoint<HookArgs.PlayerLeftGame> PlayerLeftGame;

        public static readonly HookPoint<HookArgs.SignTextSet> SignTextSet;
        public static readonly HookPoint<HookArgs.SignTextGet> SignTextGet;

        public static readonly HookPoint<HookArgs.PluginsLoaded> PluginsLoaded;
        //public static readonly HookPoint<HookArgs.WorldLoaded> WorldLoaded;

        //public static readonly HookPoint<HookArgs.PlayerHurt> PlayerHurt;
        //public static readonly HookPoint<HookArgs.NpcHurt> NpcHurt;
        //public static readonly HookPoint<HookArgs.NpcCreation> NpcCreation;
        public static readonly HookPoint<HookArgs.PlayerTriggeredEvent> PlayerTriggeredEvent;

        public static readonly HookPoint<HookArgs.PlayerChat> PlayerChat;
        public static readonly HookPoint<HookArgs.Command> Command;
        //public static readonly HookPoint<HookArgs.WorldGeneration> WorldGeneration;
        public static readonly HookPoint<HookArgs.WorldRequestMessage> WorldRequestMessage;

        public static readonly HookPoint<HookArgs.ProgramStart> ProgramStart;
        public static readonly HookPoint<HookArgs.StartCommandProcessing> StartCommandProcessing;

        public static readonly HookPoint<HookArgs.AddBan> AddBan;
        public static readonly HookPoint<HookArgs.NPCSpawn> NPCSpawn;

        //public static readonly HookPoint<HookArgs.PatchServer> PatchServer;

        static HookPoints()
        {
            //UnkownReceivedPacket = new HookPoint<HookArgs.UnkownReceivedPacket>("unkown-receive-packet");
            //UnkownSendPacket = new HookPoint<HookArgs.UnkownSendPacket>("unkown-send-packet");
            //PlayerTeleport = new HookPoint<HookArgs.PlayerTeleport>("player-teleport");
            ServerStateChange = new HookPoint<HookArgs.ServerStateChange>("server-state-change");
            NewConnection = new HookPoint<HookArgs.NewConnection>("new-connection");
            PluginLoadRequest = new HookPoint<HookArgs.PluginLoadRequest>("plugin-load-request");
            ConnectionRequestReceived = new HookPoint<HookArgs.ConnectionRequestReceived>("connection-request-received");
            DisconnectReceived = new HookPoint<HookArgs.DisconnectReceived>("disconnect-received");
            ServerPassReceived = new HookPoint<HookArgs.ServerPassReceived>("server-pass-received");
            PlayerPassReceived = new HookPoint<HookArgs.PlayerPassReceived>("player-pass-received");
            PlayerDataReceived = new HookPoint<HookArgs.PlayerDataReceived>("player-data-received");
            StateUpdateReceived = new HookPoint<HookArgs.StateUpdateReceived>("state-update-received");
            InventoryItemReceived = new HookPoint<HookArgs.InventoryItemReceived>("inventory-item-received");
            ObituaryReceived = new HookPoint<HookArgs.ObituaryReceived>("obituary-received");
            PlayerWorldAlteration = new HookPoint<HookArgs.PlayerWorldAlteration>("player-world-alteration");
            //DoorStateChanged = new HookPoint<HookArgs.DoorStateChanged>("door-state-changed");
            LiquidFlowReceived = new HookPoint<HookArgs.LiquidFlowReceived>("liquid-flow-received");
            ProjectileReceived = new HookPoint<HookArgs.ProjectileReceived>("projectile-received");
            KillProjectileReceived = new HookPoint<HookArgs.KillProjectileReceived>("kill-projectile-received");
            TileSquareReceived = new HookPoint<HookArgs.TileSquareReceived>("tile-square-received");
            ChestBreakReceived = new HookPoint<HookArgs.ChestBreakReceived>("chest-break-received");
            ChestOpenReceived = new HookPoint<HookArgs.ChestOpenReceived>("chest-open-received");
            PvpSettingReceived = new HookPoint<HookArgs.PvpSettingReceived>("pvp-setting-received");
            PartySettingReceived = new HookPoint<HookArgs.PartySettingReceived>("party-setting-received");
            PlayerEnteringGame = new HookPoint<HookArgs.PlayerEnteringGame>("player-entering-game");
            PlayerEnteredGame = new HookPoint<HookArgs.PlayerEnteredGame>("player-entered-game");
            PlayerLeftGame = new HookPoint<HookArgs.PlayerLeftGame>("player-left-game");
            PlayerAuthenticationChanging = new HookPoint<HookArgs.PlayerAuthenticationChanging>("player-auth-changing");
            PlayerAuthenticationChanged = new HookPoint<HookArgs.PlayerAuthenticationChanged>("player-auth-change");
            //Explosion = new HookPoint<HookArgs.Explosion>("explosion");
            SignTextSet = new HookPoint<HookArgs.SignTextSet>("sign-text-set");
            SignTextGet = new HookPoint<HookArgs.SignTextGet>("sign-text-get");
            PluginsLoaded = new HookPoint<HookArgs.PluginsLoaded>("plugins-loaded");
            //WorldLoaded = new HookPoint<HookArgs.WorldLoaded>("world-loaded");
            //PlayerHurt = new HookPoint<HookArgs.PlayerHurt>("player-hurt");
            //NpcHurt = new HookPoint<HookArgs.NpcHurt>("npc-hurt");
            //NpcCreation = new HookPoint<HookArgs.NpcCreation>("npc-creation");
            PlayerTriggeredEvent = new HookPoint<HookArgs.PlayerTriggeredEvent>("player-triggered-event");
            PlayerChat = new HookPoint<HookArgs.PlayerChat>("player-chat");
            Command = new HookPoint<HookArgs.Command>("command");
            //WorldGeneration = new HookPoint<HookArgs.WorldGeneration>("world-generation");
            WorldRequestMessage = new HookPoint<HookArgs.WorldRequestMessage>("world-request-message");
            StartDefaultServer = new HookPoint<HookArgs.StartDefaultServer>("start-default-server");
            //StatusTextChanged = new HookPoint<HookArgs.StatusTextChanged>("status-text-changed");
            SendNetData = new HookPoint<HookArgs.SendNetData>("netmessage-senddata");
            //UpdateServer = new HookPoint<HookArgs.UpdateServer>("update-server");
            ProgramStart = new HookPoint<HookArgs.ProgramStart>("program-start");
            StartCommandProcessing = new HookPoint<HookArgs.StartCommandProcessing>("start-command-processing");
            ConfigurationLine = new HookPoint<HookArgs.ConfigurationLine>("config-line");
            AddBan = new HookPoint<HookArgs.AddBan>("add-ban");
            NPCSpawn = new HookPoint<HookArgs.NPCSpawn>("npc-spawn");
            InvasionNPCSpawn = new HookPoint<HookArgs.InvasionNPCSpawn>("invasion-npc-spawn");

            ////Non API - but to seperate from the patcher
            //PatchServer = new HookPoint<HookArgs.PatchServer>("patch-server");
        }
    }

    public static class HookArgs
    {
        //        public struct PatchServer
        //        {
        ////            public Injector Default;
        //            public byte[] /*AssemblyDefinition*/ Terraria { get; set; }
        //            public bool IsServer { get; set; }
        //            public bool IsClient { get; set; }
        //        }

        public struct NPCSpawn
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Type { get; set; }
            public int Start { get; set; }
        }

        public struct PlayerAuthenticationChanging
        {
            public string AuthenticatedAs { get; set; }
            public string AuthenticatedBy { get; set; }
        }

        public struct PlayerAuthenticationChanged
        {
            public string AuthenticatedAs { get; set; }
            public string AuthenticatedBy { get; set; }
        }

        public struct InvasionNPCSpawn
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public struct AddBan
        {
            public string RemoteAddress { get; set; }
        }

        public struct ConfigurationLine
        {
            public string Key { get; set; }
            public string Value { get; set; }
        }

        public struct SendNetData
        {
            public int MsgType { get; set; }
            public int RemoteClient { get; set; }
            public int IgnoreClient { get; set; }
            public string Text { get; set; }
            public int Number { get; set; }
            public float Number2 { get; set; }
            public float Number3 { get; set; }
            public float Number4 { get; set; }
            public int Number5 { get; set; }
        }
        //public struct StatusTextChanged { }
        //{
        //    public string Old { get; set; }
        //    public string New { get; set; }
        //}
        public struct StartDefaultServer { }
        public struct StartCommandProcessing { }

        public struct WorldRequestMessage
        {
            public int SpawnX { get; set; }
            public int SpawnY { get; set; }
        }

        public struct UnkownReceivedPacket
        {
            public IPlayerConnection Connection { get; set; }
            public byte[] ReadBuffer { get; set; }
            public int Start { get; set; }
            public int Length { get; set; }
        }

        public struct UnkownSendPacket
        {
#if Full_API
            public NetMessage Message { get; set; }
#endif
            public int PacketId { get; set; }
            public int RemoteClient { get; set; }
            public int IgnoreClient { get; set; }
            public string Text { get; set; }
            public int Number { get; set; }
            public float Number2 { get; set; }
            public float Number3 { get; set; }
            public float Number4 { get; set; }
            public int Number5 { get; set; }
        }

        public struct WorldGeneration { }

        public struct NewConnection
        {
        }

        //public struct UpdateServer { }

        public struct ServerStateChange
        {
            public ServerState ServerChangeState { get; set; }
        }

        public struct ProgramStart
        {
            public string[] Arguments { get; set; }
        }

        public struct PlayerTeleport
        {
            public Vector2 ToLocation { get; set; }
        }

        public struct PluginLoadRequest
        {
            public string Path { get; set; }
            public BasePlugin LoadedPlugin { get; set; }
        }

        public struct ConnectionRequestReceived
        {
            public string Version { get; set; }
        }

        public struct DisconnectReceived
        {
            public string Content { get; set; }
            public string[] Lines { get; set; }
        }

        public struct ServerPassReceived
        {
            public string Password { get; set; }
        }

        public struct PlayerPassReceived
        {
            public string Password { get; set; }
        }

        public struct PlayerDataReceived
        {
            public bool IsConnecting { get; set; }
            public string Name { get; set; }

            public byte Hair { get; set; }
            public byte HairDye { get; set; }
            public byte HideVisual { get; set; }
            public bool Male { get; set; }
            public byte Difficulty { get; set; }

            public Color HairColor { get; set; }
            public Color SkinColor { get; set; }
            public Color EyeColor { get; set; }
            public Color ShirtColor { get; set; }
            public Color UndershirtColor { get; set; }
            public Color PantsColor { get; set; }
            public Color ShoeColor { get; set; }

            public bool NameChecked { get; set; }
            public bool BansChecked { get; set; }
            public bool WhitelistChecked { get; set; }

            public int Parse(byte[] buf, int at, int length)
            {
                int start = at - 2;

                Male = buf[at++] != 1;
                Hair = buf[at++];

                if (Hair >= 123)
                {
                    Hair = 0;
                }

                int len = 0;
                while (true)
                {
                    len += buf[at] & 0x7F;
                    if (buf[at++] > 127)
                        len <<= 7;
                    else break;
                }

                Name = System.Text.Encoding.UTF8.GetString(buf, at, len);
                at += len;

                HairDye = buf[at++];
                HideVisual = buf[at++];

                HairColor = ParseColor(buf, at); at += 3;
                SkinColor = ParseColor(buf, at); at += 3;
                EyeColor = ParseColor(buf, at); at += 3;
                ShirtColor = ParseColor(buf, at); at += 3;
                UndershirtColor = ParseColor(buf, at); at += 3;
                PantsColor = ParseColor(buf, at); at += 3;
                ShoeColor = ParseColor(buf, at); at += 3;

                Difficulty = buf[at++];

                //Name = System.Text.Encoding.ASCII.GetString(buf, at, length - at + start).Trim();

                return at - (start + 2);
            }

#if Full_API
            public void Apply(Player player)
            {
                player.hair = Hair;
                player.male = Male;
                player.difficulty = Difficulty;
                player.name = Name;

                player.hairColor = HairColor;
                player.skinColor = SkinColor;
                player.eyeColor = EyeColor;
                player.shirtColor = ShirtColor;
                player.underShirtColor = UndershirtColor;
                player.shoeColor = ShoeColor;
                player.pantsColor = PantsColor;
            }
#endif

            public static Color ParseColor(byte[] buf, int at)
            {
                return new Color(buf[at++], buf[at++], buf[at++]);
            }

            public bool CheckName(out string error)
            {
                error = null;
                NameChecked = true;

                if (Name.Length > 20)
                {
                    error = "Invalid name: longer than 20 characters.";
                    return false;
                }

                if (Name == "")
                {
                    error = "Invalid name: whitespace or empty.";
                    return false;
                }

                foreach (char c in Name)
                {
                    if (c < 32 || c > 126)
                    {
                        //Console.Write ((byte) c);
                        error = "Invalid name: contains non-printable characters.";
                        return false;
                    }
                    //Console.Write (c);
                }

                if (Name.Contains(" " + " "))
                {
                    error = "Invalid name: contains double spaces.";
                    return false;
                }

                return true;
            }
        }

        public struct StateUpdateReceived
        {
            public byte FlagsA { get; set; }
            public byte FlagsB { get; set; }

            public byte SelectedItemIndex { get; set; }

            public float X { get; set; }
            public float Y { get; set; }

            public float VX { get; set; }
            public float VY { get; set; }

            public bool ControlUp
            {
                get { return (FlagsA & 1) != 0; }
                set { SetFlagA(1, value); }
            }

            public bool ControlDown
            {
                get { return (FlagsA & 2) != 0; }
                set { SetFlagA(2, value); }
            }

            public bool ControlLeft
            {
                get { return (FlagsA & 4) != 0; }
                set { SetFlagA(4, value); }
            }

            public bool ControlRight
            {
                get { return (FlagsA & 8) != 0; }
                set { SetFlagA(8, value); }
            }

            public bool ControlJump
            {
                get { return (FlagsA & 16) != 0; }
                set { SetFlagA(16, value); }
            }

            public bool ControlUseItem
            {
                get { return (FlagsA & 32) != 0; }
                set { SetFlagA(32, value); }
            }

            public int Direction
            {
                get { return ((FlagsA & 64) != 0) ? 1 : -1; }
                set { SetFlagA(64, value == 1); }
            }

            public bool Pulley
            {
                get { return (FlagsB & 1) != 0; }
                set { SetFlagB(1, value); }
            }

            public byte PulleyDirection
            {
                get { return (byte)(((FlagsB & 2) != 0) ? 2 : 1); }
                set { SetFlagB(2, value == 2); }
            }

            public bool HasVelocity
            {
                get { return (FlagsB & 4) != 0; }
                set { SetFlagB(4, value); }
            }

            internal void SetFlagA(byte f, bool value)
            {
                if (value)
                    FlagsA |= f;
                else
                    FlagsA &= (byte)~f;
            }

            internal void SetFlagB(byte f, bool value)
            {
                if (value)
                    FlagsB |= f;
                else
                    FlagsB &= (byte)~f;
            }

#if Full_API
            public void ApplyKeys(Player player)
            {
                player.controlUp = ControlUp;
                player.controlDown = ControlDown;
                player.controlLeft = ControlLeft;
                player.controlRight = ControlRight;
                player.controlJump = ControlJump;
                player.controlUseItem = ControlUseItem;
            }

            public void ApplyParams(Player player)
            {
                player.selectedItem = SelectedItemIndex;
                player.direction = Direction;
                player.position = new Vector2(X, Y);

                if (HasVelocity)
                    player.velocity = new Vector2(VX, VY);

                player.pulley = Pulley;
                player.pulleyDir = PulleyDirection;
            }
#endif

            public void Parse(byte[] buf, int at)
            {
                FlagsA = buf[at++];
                FlagsB = buf[at++];

                SelectedItemIndex = buf[at++];

                X = BitConverter.ToSingle(buf, at); at += 4;
                Y = BitConverter.ToSingle(buf, at); at += 4;
                VX = BitConverter.ToSingle(buf, at); at += 4;
                VY = BitConverter.ToSingle(buf, at);
            }
        }

        public struct InventoryItemReceived
        {
            public int InventorySlot { get; set; }
            public int Amount { get; set; }
            public string Name { get; set; }
            public int Prefix { get; set; }
            public int NetID { get; set; }

#if Full_API
            public Item Item { get; set; }
#endif

            public void SetItem()
            {
#if Full_API
                Item = new Item();
                Item.netDefaults(NetID);
                Item.stack = Amount;
                Item.Prefix(Prefix);
#endif
            }
        }

        public struct ObituaryReceived
        {
            public int Direction { get; set; }
            public int Damage { get; set; }
            public byte PvpFlag { get; set; }
            public string Obituary { get; set; }
        }

        public struct PvpSettingReceived
        {
            public bool PvpFlag { get; set; }
        }

        public struct PartySettingReceived
        {
            public byte Party { get; set; }
        }

        public struct PlayerWorldAlteration
        {
            public int X { get; set; }
            public int Y { get; set; }
            public byte Action { get; set; }
            public short Type { get; set; }
            public int Style { get; set; }

            public bool TypeChecked { get; set; }

            //            public WorldMod.PlayerSandbox Sandbox { get; internal set; }

            public bool TileWasRemoved
            {
                get { return Action == 0 || Action == 4 || Action == 100; }
            }

            public bool NoItem
            {
                get { return Action == 4 || Action == 101; }
                set
                {
                    if (value)
                    {
                        if (Action == 0) Action = 4;
                        else if (Action == 100) Action = 101;
                    }
                    else
                    {
                        if (Action == 4) Action = 0;
                        else if (Action == 101) Action = 100;
                    }
                }
            }

            public bool TileWasPlaced
            {
                get { return Action == 1; }
            }

            public bool WallWasRemoved
            {
                get { return Action == 2 || Action == 100 || Action == 101; }
            }

            public bool WallWasPlaced
            {
                get { return Action == 3; }
            }

            public bool RemovalFailed
            {
                get { return Type == 1 && (Action == 0 || Action == 2 || Action == 4); }
                set { if (Action == 0 || Action == 2 || Action == 4) Type = value ? (byte)1 : (byte)0; }
            }

#if Full_API
            public Terraria.Tile Tile
            {
                get
                { return Main.tile[X, Y]; }
            }
#endif
        }

        public struct DoorStateChanged
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Direction { get; set; }

            public bool Open { get; set; }
            public bool Close
            {
                get { return !Open; }
                set { Open = !value; }
            }
        }

        public struct LiquidFlowReceived
        {
            public int X { get; set; }
            public int Y { get; set; }
            public byte Amount { get; set; }

            public bool Lava { get; set; }
            public bool Water
            {
                get { return !Lava; }
                set { Lava = !value; }
            }
        }

        public struct ProjectileReceived
        {
            public int Id { get; set; }
            public Vector2 Position { get; set; }
            public Vector2 Velocity { get; set; }
            //public float X { get; set; }
            //public float Y { get; set; }
            //public float VX { get; set; }
            //public float VY { get; set; }
            public float Knockback { get; set; }
            public int Damage { get; set; }
            public int Owner { get; set; }
            public int Type { get; set; }
            public float[] AI { get; set; }

            public float AI_0
            {
                get { return AI[0]; }
                set { AI[0] = value; }
            }

            public float AI_1
            {
                get { return AI[01]; }
                set { AI[01] = value; }
            }

            public int ExistingIndex { get; set; }

#if Full_API
            internal Projectile projectile;

            public Projectile CreateProjectile()
            {
                if (projectile != null) return projectile;

                //                var index = Projectile.ReserveSlot(Id, Owner);
                //
                //                if (index == 1000) return null;
                //
                //                projectile = Registries.Projectile.Create(TypeByte);

                //                projectile.whoAmI = index;
                //                Apply(projectile);

                return projectile;
            }

            public void Apply(Projectile projectile)
            {
                //                if (Owner < 255)
                //                    projectile.Creator = Main.player[Owner];
                projectile.identity = Id;
                projectile.owner = Owner;
                projectile.damage = Damage;
                projectile.knockBack = Knockback;
                //                projectile.position = new Vector2(X, Y);
                //                projectile.velocity = new Vector2(VX, VY);
                projectile.ai = AI;
            }

            internal void CleanupProjectile()
            {
                if (projectile != null)
                {
                    //                    Projectile.FreeSlot(projectile.identity, projectile.Owner, projectile.whoAmI);
                    projectile = null;
                }
            }

            //            public ProjectileType Type
            //            {
            //                get { return (ProjectileType)TypeByte; }
            //                set { TypeByte = (byte)value; }
            //            }

            public Projectile Current
            {
                get { return Main.projectile[Id]; }
            }
#endif
        }

        public struct KillProjectileReceived
        {
            public int Index { get; set; }
            public int Id { get; set; }
            public int Owner { get; set; }
        }

        public struct Explosion
        {
#if Full_API
            public Projectile Source { get; set; }
#endif
        }

        public struct ChestBreakReceived
        {
            public int X { get; set; }
            public int Y { get; set; }
        }

        public struct ChestOpenReceived
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int ChestIndex { get; set; }
        }

        public struct PlayerEnteringGame
        {
            public int Slot { get; set; }
        }

        public struct PlayerEnteredGame
        {
            public int Slot { get; set; }
        }

        public struct PlayerLeftGame
        {
            public int Slot { get; set; }
        }

        public struct SignTextGet
        {
            public int X { get; set; }
            public int Y { get; set; }
            public short SignIndex { get; set; }
            public string Text { get; set; }
        }

        public struct SignTextSet
        {
            public int X { get; set; }
            public int Y { get; set; }
            public short SignIndex { get; set; }
            public string Text { get; set; }
#if Full_API
            public Sign OldSign { get; set; }
#endif
        }

        public struct PluginsLoaded
        {
        }

        public struct WorldLoaded
        {
            public int Width { get; set; }
            public int Height { get; set; }
        }

        public struct PlayerHurt
        {
#if Full_API
            public Player Victim { get; internal set; }
#endif
            public int Damage { get; set; }
            public int HitDirection { get; set; }
            public bool Pvp { get; set; }
            public bool Quiet { get; set; }
            public string Obituary { get; set; }
            public bool Critical { get; set; }
        }

        public struct NpcHurt
        {
#if Full_API
            public NPC Victim { get; set; }
#endif
            public int Damage { get; set; }
            public int HitDirection { get; set; }
            public float Knockback { get; set; }
            public bool Critical { get; set; }
        }

        public struct NpcCreation
        {
            public int X { get; set; }
            public int Y { get; set; }
            public string Name { get; set; }

#if Full_API
            public NPC CreatedNpc { get; set; }
#endif
        }

        public struct PlayerTriggeredEvent
        {
            public int X { get; set; }
            public int Y { get; set; }

            public WorldEventType Type { get; set; }
            public string Name { get; set; }
        }

        public struct PlayerChat
        {
            public string Message { get; set; }
            public Color Color { get; set; }
        }

        public struct Command
        {
            public string Prefix { get; internal set; }
            public ArgumentList Arguments { get; set; }
            public string ArgumentString { get; set; }
        }

        public struct TileSquareReceived
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Size { get; set; }

            public byte[] readBuffer;
            public int start;
            public int applied;

            //            public void ForEach(object state, TileSquareForEachFunc func)
            //            {
            //                int num = start;
            //
            //                for (int x = X; x < X + Size; x++)
            //                {
            //                    for (int y = Y; y < Y + Size; y++)
            //                    {
            //                        TileData tile = Main.tile.At(x, y).Data;
            //
            //                        byte b9 = readBuffer[num++];
            //
            //                        bool wasActive = tile.Active;
            //
            //                        tile.Active = ((b9 & 1) == 1);
            //
            //                        if ((b9 & 2) == 2)
            //                        {
            //                            tile.Lighted = true;
            //                        }
            //
            //                        if (tile.Active)
            //                        {
            //                            int wasType = (int)tile.Type;
            //                            tile.Type = readBuffer[num++];
            //
            //                            if (tile.Type < Main.MAX_TILE_SETS && Main.tileFrameImportant[(int)tile.Type])
            //                            {
            //                                tile.FrameX = BitConverter.ToInt16(readBuffer, num);
            //                                num += 2;
            //                                tile.FrameY = BitConverter.ToInt16(readBuffer, num);
            //                                num += 2;
            //                            }
            //                            else if (!wasActive || (int)tile.Type != wasType)
            //                            {
            //                                tile.FrameX = -1;
            //                                tile.FrameY = -1;
            //                            }
            //                        }
            //
            //                        if ((b9 & 4) == 4)
            //                            tile.Wall = readBuffer[num++];
            //                        else
            //                            tile.Wall = 0;
            //
            //                        if ((b9 & 8) == 8)
            //                        {
            //                            tile.Liquid = readBuffer[num++];
            //                            byte b10 = readBuffer[num++];
            //                            tile.Lava = (b10 == 1);
            //                        }
            //                        else
            //                            tile.Liquid = 0;
            //
            //                        var result = func(x, y, ref tile, state);
            //                        if (result == TileSquareForEachResult.ACCEPT)
            //                        {
            //                            applied += 1;
            //                            Main.tile.At(x, y).SetData(tile);
            //                        }
            //                        else if (result == TileSquareForEachResult.BREAK)
            //                        {
            //                            return;
            //                        }
            //                    }
            //                }
            //            }
        }
    }

    public enum TileSquareForEachResult
    {
        ACCEPT,
        IGNORE,
        BREAK,
    }

    //    public delegate TileSquareForEachResult TileSquareForEachFunc(int x, int y, ref TileData tile, object state);

    public enum WorldEventType
    {
        BOSS,
        INVASION,
        SHADOW_ORB,
    }
}