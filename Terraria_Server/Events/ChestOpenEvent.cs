﻿namespace Terraria_Server.Events
{
    public class ChestOpenEvent : Event
    {
        private int chestID;
        public int getChestID()
        {
            return chestID;
        }
        public void setChestID(int ChestID)
        {
            chestID = ChestID;
        }
    }
}
