namespace tdsm.api
{
    public interface IPlayerConnection
    {
        bool DisconnectInProgress();

        void Kick(string reason, bool announce = true);

        //bool HasConnected();

        //void Flush();
    }

    /// <summary>
    /// Used for the vanilla connection implementation
    /// </summary>
    public class APIConnection : IPlayerConnection
    {
        public int whoAmI;

        public bool DisconnectInProgress()
        {
            return Terraria.Netplay.serverSock[whoAmI].kill;
        }

        public void Kick(string reason, bool announce = true)
        {
            Terraria.NetMessage.SendData(2, whoAmI, -1, reason, 0, 0f, 0f, 0f, 0);
        }
    }
}

