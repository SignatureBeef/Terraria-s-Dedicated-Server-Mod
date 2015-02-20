using Microsoft.Xna.Framework;
using tdsm.api.Command;

namespace tdsm.api
{
    public partial class BasePlayer : Sender
    {
        public string ClientUUId { get; set; }

        public string AuthenticatedAs { get; set; }
        public string AuthenticatedBy { get; set; }

        public System.Collections.Hashtable PluginData;// = new System.Collections.Hashtable();

        public void SetAuthentication(string auth, string by)
        {
            this.AuthenticatedAs = auth;
            this.AuthenticatedBy = by;
        }

        public override string Name
        {
            get
            {
#if Full_API
                if (this is Terraria.Player)
                {
                    return ((Terraria.Player)this).name;
                }
#endif
                return "??";
            }
            protected set { }
        }

        public override void SendMessage(string message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
#if Full_API
            Terraria.NetMessage.SendData((int)Packet.PLAYER_CHAT, ((Terraria.Player)this).whoAmi, -1, message, A, R, G, B);
#endif
        }

        public void SendMessage(string message, Color color)
        {
            SendMessage(message, color.A, color.R, color.G, color.B);
        }

        public IPlayerConnection Connection;
        public string IPAddress;
        public string DisconnectReason;

#if Full_API
        /// <summary>
        /// Teleports a player to another player
        /// </summary>
        /// <param name="target"></param>
        /// <param name="style"></param>
        public void Teleport(Terraria.Player target, int style = 0)
        {
            if (this is Terraria.Player)
            {
                var plr = (Terraria.Player)this;
                plr.Teleport(target.position, style);

                Terraria.NetMessage.SendData((int)Packet.TELEPORT, -1, -1, "", 0, plr.whoAmi, target.position.X, target.position.Y, 3);
            }
        }

        /// <summary>
        /// Teleports a player to a specified location
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="style"></param>
        public void Teleport(float x, float y, int style = 0)
        {
            if (this is Terraria.Player)
            {
                var plr = (Terraria.Player)this;
                var pos = new Vector2(x, y);
                plr.Teleport(pos, style);

                Terraria.NetMessage.SendData((int)Packet.TELEPORT, -1, -1, "", 0, plr.whoAmi, pos.X, pos.Y, 3);
            }
        }

        /// <summary>
        /// Removes a player from the server
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="announce"></param>
        public void Kick(string reason, bool announce = true)
        {
            Connection.Kick(reason, announce);
        }

        /// <summary>
        /// Gives a player an item
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="stack"></param>
        /// <param name="sender"></param>
        /// <param name="netId"></param>
        /// <param name="notifyOps"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public int GiveItem(int itemId, int stack, ISender sender, int netId, bool notifyOps = true, int prefix = 0)
        {
            if (this is Terraria.Player)
            {
                var plr = (Terraria.Player)this;

                var index = Terraria.Item.NewItem((int)plr.position.X, (int)plr.position.Y, plr.width, plr.height, itemId, stack, false, prefix);

                if (netId < 0)
                    Terraria.Main.item[index].netDefaults(netId);

                if (prefix > 0) Terraria.Main.item[index].Prefix(prefix);

                if (notifyOps)
                    Tools.NotifyAllOps("Giving " + this.Name + " some " + Terraria.Main.item[index].name + " (" + itemId.ToString() + ") [" + sender.SenderName + "]", true);

                return index;
            }
            return -1;
        }
#endif
    }
}
