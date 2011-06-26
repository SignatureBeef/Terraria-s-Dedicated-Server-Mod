namespace Terraria_Server.Commands
{
    public class Sender
    {
        public Server getServer()
        {
            return Program.server;
        }

        public bool Op { get; set; }

        public string getName()
        {
            if (this is Player)
            {
                return ((Player)this).name;
            }
            return "CONSOLE";
        }

        public void sendMessage(string Message, int A = 255, float R = 255f, float G = 0f, float B = 0f)
        {
            if (this is Player)
            {
                NetMessage.SendData((int)Packet.PLAYER_CHAT, ((Player)this).whoAmi, -1, Message, A, R, G, B);
            }
            else
            {
                Program.tConsole.WriteLine(Message);
            }
        }

    }
}
