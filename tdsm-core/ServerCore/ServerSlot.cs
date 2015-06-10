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
        public static void Kick(this tdsm.api.Callbacks.IAPISocket sock, string reason)
        {
            (sock as ServerSlot).Kick(reason);
        }

        public static SlotState State(this tdsm.api.Callbacks.IAPISocket sock)
        {
            return (sock as ServerSlot).state;
        }

        public static void SetState(this tdsm.api.Callbacks.IAPISocket sock, SlotState state)
        {
            (sock as ServerSlot).state = state;
        }
    }

    public class ServerSlot : tdsm.api.Callbacks.IAPISocket
    {
        public volatile ClientConnection conn;

        //The base.state will always be zero.
        //Luckily how the sockets works this ServerSlot class is only ever used by the core.
        //The core will never use base.state
        public SlotState state
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
        //public volatile string remoteAddress;
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

        public override void SpamUpdate()
        {
            if (!Netplay.spamCheck)
            {
                this.spamProjectile = 0f;
                this.spamDelBlock = 0f;
                this.spamAddBlock = 0f;
                this.spamWater = 0f;
                return;
            }
            if (this.spamProjectile > this.spamProjectileMax)
            {
                NewNetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Projectile spam");
            }
            if (this.spamAddBlock > this.spamAddBlockMax)
            {
                NewNetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Add tile spam");
            }
            if (this.spamDelBlock > this.spamDelBlockMax)
            {
                NewNetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Remove tile spam");
            }
            if (this.spamWater > this.spamWaterMax)
            {
                NewNetMessage.BootPlayer(this.whoAmI, "Cheating attempt detected: Liquid spam");
            }
            this.spamProjectile -= 0.4f;
            if (this.spamProjectile < 0f)
            {
                this.spamProjectile = 0f;
            }
            this.spamAddBlock -= 0.3f;
            if (this.spamAddBlock < 0f)
            {
                this.spamAddBlock = 0f;
            }
            this.spamDelBlock -= 5f;
            if (this.spamDelBlock < 0f)
            {
                this.spamDelBlock = 0f;
            }
            this.spamWater -= 0.2f;
            if (this.spamWater < 0f)
            {
                this.spamWater = 0f;
            }
        }

        public override void SpamClear()
        {
            this.spamProjectile = 0f;
            this.spamAddBlock = 0f;
            this.spamDelBlock = 0f;
            this.spamWater = 0f;
        }

        public override void Reset()
        {
            if (tileSection != null && tileSection.GetLength(0) >= Main.maxSectionsX && tileSection.GetLength(1) >= Main.maxSectionsY)
            {
                Array.Clear(tileSection, 0, tileSection.GetLength(0) * tileSection.GetLength(1));
            }
            else
            {
                tileSection = new bool[Main.maxSectionsX, Main.maxSectionsY];
            }

            var oldPlayer = Main.player[this.whoAmI];
            if (oldPlayer != null && state != SlotState.VACANT)
            {
                NewNetMessage.OnPlayerLeft(oldPlayer, this, announced);
            }
            announced = false;
            this.remoteAddress = "<unknown>";

            if (this.whoAmI < 255)
            {
                Main.player[this.whoAmI] = new Player();
            }

            this.statusCount = 0;
            this.statusMax = 0;
            this.statusText2 = String.Empty;
            this.statusText = String.Empty;
            this.name = "Anonymous";
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

        public override bool IsPlaying()
        {
            return state == SlotState.PLAYING;
        }

        public override bool CanSendWater()
        {
            return state >= SlotState.SENDING_TILES && Connected;
        }

        public override string RemoteAddress()
        {
            return conn.RemoteAddress;
        }
    }
}
