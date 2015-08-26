using System;
using System.Net.Sockets;
using tdsm.core.Messages.Out;
using Terraria;

namespace tdsm.core.ServerCore
{
    [Flags]
    public enum SlotState : int
    {
        NONE = 0,

        SHUTDOWN = 1,        // the client's socket is being shut down unconditionally
        KICK = 2,            // the client is being kicked, disconnect him after sending him all remaining data

        VACANT = 4,           //                this socket has no client and is available
        CONNECTED = 8,        // previously 0,  the client socket has been accepted
        SERVER_AUTH = 16,     //           -1,  the client has been asked for a server password
        ACCEPTED = 32,        //            1,  the client has successfully authenticated
        PLAYER_AUTH = 64,     //                the client has been asked for a character password
        QUEUED = 128,         //                the client is waiting for a free slot
        ASSIGNING_SLOT = 256,     //                the client has been assigned a slot after waiting
        SENDING_WORLD = 512,  //            2,  the client requested world info
        SENDING_TILES = 1024, //            3,  the client requested tiles
        PLAYING = 2048,       //            10

        // composites
        DISCONNECTING = KICK | SHUTDOWN,
        // before a slot is assigned (whoAmI is invalid)
        UNASSIGNED = KICK | SHUTDOWN | VACANT | CONNECTED | SERVER_AUTH | ACCEPTED | PLAYER_AUTH | QUEUED,

        ALL = 4095,
    }

    public static class SlotStateExtensions
    {
        public static bool DisconnectInProgress(this SlotState state)
        {
            return (state & SlotState.DISCONNECTING) != 0;
        }
    }

    public static class IAPISocketExtensions
    {
        public static void Kick(this Terraria.RemoteClient sock, string reason)
        {
            (sock as ServerSlot).Kick(reason);
        }

        public static SlotState State(this Terraria.RemoteClient sock)
        {
            return (sock as ServerSlot).state;
        }

        public static void SetState(this Terraria.RemoteClient sock, SlotState state)
        {
            (sock as ServerSlot).state = state;
        }

        public static bool IsPlaying(this Terraria.RemoteClient sock)
        {
            return (sock as ServerSlot).state == SlotState.PLAYING;
        }

        public static bool CanSendWater(this Terraria.RemoteClient sock)
        {
            return (sock as ServerSlot).state >= SlotState.SENDING_TILES && (sock as ServerSlot).Connected;
        }

        public static string RemoteAddress(this Terraria.RemoteClient sock)
        {
            return (sock as ServerSlot).remoteAddress;
        }
    }

    public class ServerSlot : Terraria.RemoteClient
    {
        public volatile ClientConnection conn;

        //The base.state will always be zero.
        //Luckily how the sockets works this ServerSlot class is only ever used by the core.
        //The core will never use base.state
        public new SlotState state
        {
            get { return conn == null ? SlotState.VACANT : conn.State; }
            set
            {
                if (value == SlotState.VACANT)
                    conn = null;
                else
                    conn.State = value;
            }
        }

        //public int whoAmI;
        //public string statusText2;
        //public int statusCount;
        //public int statusMax;
        public volatile string remoteAddress;
        //public bool[,] tileSection;
        //public string statusText = String.Empty;
        //public bool announced;
        //public string name = "Anonymous";
        //public string oldName = String.Empty;
        //public float spamProjectile;
        //public float spamAddBlock;
        //public float spamDelBlock;
        //public float spamWater;
        //public float spamProjectileMax = 100f;
        //public float spamAddBlockMax = 100f;
        //public float spamDelBlockMax = 500f;
        //public float spamWaterMax = 50f;

        public bool Connected
        {
            get
            {
                try
                {
                    return (conn != null && conn.Active); //(socket != null && socket.Connected);
                }
                catch (SocketException)
                {
                    return false;
                }
                catch (ObjectDisposedException)
                {
                    return false;
                }
            }
        }

        public ServerSlot()
        {
            state = SlotState.VACANT;
        }

        public new void SpamUpdate()
        {
            if (!Netplay.spamCheck)
            {
                this.SpamProjectile = 0f;
                this.SpamDeleteBlock = 0f;
                this.SpamAddBlock = 0f;
                this.SpamWater = 0f;
                return;
            }
            if (this.SpamProjectile > this.SpamProjectileMax && Entry.EnableCheatProtection)
            {
                NewNetMessage.BootPlayer(this.Id, "Cheating attempt detected: Projectile spam");
            }
            if (this.SpamAddBlock > this.SpamAddBlockMax && Entry.EnableCheatProtection)
            {
                NewNetMessage.BootPlayer(this.Id, "Cheating attempt detected: Add tile spam");
            }
            if (this.SpamDeleteBlock > this.SpamDeleteBlockMax && Entry.EnableCheatProtection)
            {
                NewNetMessage.BootPlayer(this.Id, "Cheating attempt detected: Remove tile spam");
            }
            if (this.SpamWater > this.SpamWaterMax && Entry.EnableCheatProtection)
            {
                NewNetMessage.BootPlayer(this.Id, "Cheating attempt detected: Liquid spam");
            }
            this.SpamProjectile -= 0.4f;
            if (this.SpamProjectile < 0f)
            {
                this.SpamProjectile = 0f;
            }
            this.SpamAddBlock -= 0.3f;
            if (this.SpamAddBlock < 0f)
            {
                this.SpamAddBlock = 0f;
            }
            this.SpamDeleteBlock -= 5f;
            if (this.SpamDeleteBlock < 0f)
            {
                this.SpamDeleteBlock = 0f;
            }
            this.SpamWater -= 0.2f;
            if (this.SpamWater < 0f)
            {
                this.SpamWater = 0f;
            }
        }

        public new bool SectionRange(int size, int firstX, int firstY)
        {
            for (var i = 0; i < 4; i++)
            {
                var x = firstX;
                var y = firstY;
                if (i == 1) x += size;

                if (i == 2) y += size;
                if (i == 3)
                {
                    x += size;
                    y += size;
                }

                var sectionX = Netplay.GetSectionX(x);
                var sectionY = Netplay.GetSectionY(y);
                if (this.TileSections[sectionX, sectionY])
                    return true;
            }
            return false;
        }

        public new void SpamClear()
        {
            this.SpamProjectile = 0f;
            this.SpamAddBlock = 0f;
            this.SpamDeleteBlock = 0f;
            this.SpamWater = 0f;
        }

        public new void Reset()
        {
            if (TileSections != null && TileSections.GetLength(0) >= Main.maxSectionsX && TileSections.GetLength(1) >= Main.maxSectionsY)
            {
                Array.Clear(TileSections, 0, TileSections.GetLength(0) * TileSections.GetLength(1));
            }
            else
            {
                TileSections = new bool[Main.maxSectionsX, Main.maxSectionsY];
            }

            var oldPlayer = Main.player[this.Id];
            if (oldPlayer != null && state != SlotState.VACANT)
            {
                NewNetMessage.OnPlayerLeft(oldPlayer, this, IsAnnouncementCompleted);
            }
            IsAnnouncementCompleted = false;
            this.remoteAddress = "<unknown>";

            if (this.Id < 255)
            {
                Main.player[this.Id] = new Player();
            }

            this.StatusCount = 0;
            this.StatusMax = 0;
            this.StatusText2 = String.Empty;
            this.StatusText = String.Empty;
            this.Name = "Anonymous";
            this.conn = null;

            this.SpamClear();

            conn = null;
        }

        public void Kick(string reason)
        {
            if (state == SlotState.VACANT) return;

            conn.Kick(reason);
        }

        public void Send(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentException("Data to send cannot be null");
            }

            if (conn == null) return;

            conn.Send(data);
        }

        public void Send(byte[] data, int offset, int length)
        {
            if (data == null)
            {
                throw new ArgumentException("Data to send cannot be null");
            }

            if (conn == null) return;

            conn.CopyAndSend(new ArraySegment<byte>(data, offset, length));
        }

//        public override bool IsPlaying()
//        {
//            return state == SlotState.PLAYING;
//        }
//
//        public override bool CanSendWater()
//        {
//            return state >= SlotState.SENDING_TILES && Connected;
//        }
//
//        public override string RemoteAddress()
//        {
//            return conn.RemoteAddress;
//        }
    }
}
