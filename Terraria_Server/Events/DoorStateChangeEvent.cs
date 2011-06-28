using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Plugin;

namespace Terraria_Server.Events
{
    public class DoorStateChangeEvent : Event
    {
        private int x;
        private int y;
        private int direction;
        private bool opened;
        private DoorOpener opener;

        public bool isOpened()
        {
            return opened;
        }

        public void setOpened(bool Opened)
        {
            opened = Opened;
        }


        public int getX()
        {
            return x;
        }

        public void setX(int X)
        {
            x = X;
        }

        public int getY()
        {
            return y;
        }

        public Vector2 getVector()
        {
            return new Vector2(x, y);
        }

        public void setVector(Vector2 Vector)
        {
            x = (int)Vector.X;
            y = (int)Vector.Y;
        }

        public void setY(int Y)
        {
            y = Y;
        }

        public int getDirection()
        {
            return direction;
        }

        public void setDirection(int Direction)
        {
            direction = Direction;
        }

        public DoorOpener getOpener()
        {
            return opener;
        }

        public void setOpener(DoorOpener Opener)
        {
            opener = Opener;
        }

    }
}
