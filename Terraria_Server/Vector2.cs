using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Terraria_Server
{
    public class Vector2
    {
        public float X = 0;
        public float Y = 0;

        public Vector2() { }

        public Vector2 (float x, float y)
        {
            X = x;
            Y = y;
        }

        public static Vector2 operator +(Vector2 value1, Vector2 value2)
        {
            if (value1 == null)
            {
                value1 = new Vector2();
            }
            if (value2 == null)
            {
                value2 = new Vector2();
            }
            Vector2 result = new Vector2();
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            return result;
        }

        public static Vector2 operator *(Vector2 value1, Vector2 value2)
        {
            Vector2 result = new Vector2();
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            return result;
        }
        public static Vector2 operator *(Vector2 value, float scaleFactor)
        {
            Vector2 result = new Vector2();
            result.X = value.X * scaleFactor;
            result.Y = value.Y * scaleFactor;
            return result;
        }
        public static Vector2 operator *(float scaleFactor, Vector2 value)
        {
            Vector2 result = new Vector2();
            result.X = value.X * scaleFactor;
            result.Y = value.Y * scaleFactor;
            return result;
        }

        public static Vector2 operator -(Vector2 value1, Vector2 value2)
        {
            Vector2 result = new Vector2();
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            return result;
        }

        /*public float getX()
        {
            return x;
        }

        public void setX(float X)
        {
            x = X;
        }

        public float getY()
        {
            return y;
        }

        public void setYfloat Y)
        {
            y = Y;
        }*/

    }

}
