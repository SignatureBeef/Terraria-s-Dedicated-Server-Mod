namespace tdsm.api
{
    public interface IPlayerConnection
    {
        bool DisconnectInProgress();

        void Kick(string reason, bool announce = true);

        bool HasConnected();

        void Flush();
    }
}

