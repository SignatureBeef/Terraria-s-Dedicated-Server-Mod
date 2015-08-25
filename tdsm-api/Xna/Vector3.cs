using System;
using System.ComponentModel;
using System.Globalization;
namespace Microsoft.Xna.Framework
{
    [Serializable]
    public struct Vector3 : IEquatable<Vector3>
    {
        public float X;
        public float Y;
        public float Z;
        private static Vector3 _zero = default(Vector3);
        private static Vector3 _one = new Vector3(1f, 1f, 1f);
        private static Vector3 _unitX = new Vector3(1f, 0f, 0f);
        private static Vector3 _unitY = new Vector3(0f, 1f, 0f);
        private static Vector3 _unitZ = new Vector3(0f, 0f, 1f);
        private static Vector3 _up = new Vector3(0f, 1f, 0f);
        private static Vector3 _down = new Vector3(0f, -1f, 0f);
        private static Vector3 _right = new Vector3(1f, 0f, 0f);
        private static Vector3 _left = new Vector3(-1f, 0f, 0f);
        private static Vector3 _forward = new Vector3(0f, 0f, -1f);
        private static Vector3 _backward = new Vector3(0f, 0f, 1f);
        public static Vector3 Zero
        {
            get
            {
                return Vector3._zero;
            }
        }
        public static Vector3 One
        {
            get
            {
                return Vector3._one;
            }
        }
        public static Vector3 UnitX
        {
            get
            {
                return Vector3._unitX;
            }
        }
        public static Vector3 UnitY
        {
            get
            {
                return Vector3._unitY;
            }
        }
        public static Vector3 UnitZ
        {
            get
            {
                return Vector3._unitZ;
            }
        }
        public static Vector3 Up
        {
            get
            {
                return Vector3._up;
            }
        }
        public static Vector3 Down
        {
            get
            {
                return Vector3._down;
            }
        }
        public static Vector3 Right
        {
            get
            {
                return Vector3._right;
            }
        }
        public static Vector3 Left
        {
            get
            {
                return Vector3._left;
            }
        }
        public static Vector3 Forward
        {
            get
            {
                return Vector3._forward;
            }
        }
        public static Vector3 Backward
        {
            get
            {
                return Vector3._backward;
            }
        }
        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
        public Vector3(float value)
        {
            this.Z = value;
            this.Y = value;
            this.X = value;
        }
        public Vector3(Vector2 value, float z)
        {
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
        }
        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "{{X:{0} Y:{1} Z:{2}}}", new object[]
			{
				this.X.ToString(currentCulture),
				this.Y.ToString(currentCulture),
				this.Z.ToString(currentCulture)
			});
        }
        public bool Equals(Vector3 other)
        {
            return this.X == other.X && this.Y == other.Y && this.Z == other.Z;
        }
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is Vector3)
            {
                result = this.Equals((Vector3)obj);
            }
            return result;
        }
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode() + this.Z.GetHashCode();
        }
        public float Length()
        {
            float num = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            return (float)Math.Sqrt((double)num);
        }
        public float LengthSquared()
        {
            return this.X * this.X + this.Y * this.Y + this.Z * this.Z;
        }
        public static float Distance(Vector3 value1, Vector3 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            return (float)Math.Sqrt((double)num4);
        }
        public static void Distance(ref Vector3 value1, ref Vector3 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            float num4 = num * num + num2 * num2 + num3 * num3;
            result = (float)Math.Sqrt((double)num4);
        }
        public static float DistanceSquared(Vector3 value1, Vector3 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            return num * num + num2 * num2 + num3 * num3;
        }
        public static void DistanceSquared(ref Vector3 value1, ref Vector3 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = value1.Z - value2.Z;
            result = num * num + num2 * num2 + num3 * num3;
        }
        public static float Dot(Vector3 vector1, Vector3 vector2)
        {
            return vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }
        public static void Dot(ref Vector3 vector1, ref Vector3 vector2, out float result)
        {
            result = vector1.X * vector2.X + vector1.Y * vector2.Y + vector1.Z * vector2.Z;
        }
        public void Normalize()
        {
            float num = this.X * this.X + this.Y * this.Y + this.Z * this.Z;
            float num2 = 1f / (float)Math.Sqrt((double)num);
            this.X *= num2;
            this.Y *= num2;
            this.Z *= num2;
        }
        public static Vector3 Normalize(Vector3 value)
        {
            float num = value.X * value.X + value.Y * value.Y + value.Z * value.Z;
            float num2 = 1f / (float)Math.Sqrt((double)num);
            Vector3 result;
            result.X = value.X * num2;
            result.Y = value.Y * num2;
            result.Z = value.Z * num2;
            return result;
        }
        public static void Normalize(ref Vector3 value, out Vector3 result)
        {
            float num = value.X * value.X + value.Y * value.Y + value.Z * value.Z;
            float num2 = 1f / (float)Math.Sqrt((double)num);
            result.X = value.X * num2;
            result.Y = value.Y * num2;
            result.Z = value.Z * num2;
        }
        public static Vector3 Cross(Vector3 vector1, Vector3 vector2)
        {
            Vector3 result;
            result.X = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            result.Y = vector1.Z * vector2.X - vector1.X * vector2.Z;
            result.Z = vector1.X * vector2.Y - vector1.Y * vector2.X;
            return result;
        }
        public static void Cross(ref Vector3 vector1, ref Vector3 vector2, out Vector3 result)
        {
            float x = vector1.Y * vector2.Z - vector1.Z * vector2.Y;
            float y = vector1.Z * vector2.X - vector1.X * vector2.Z;
            float z = vector1.X * vector2.Y - vector1.Y * vector2.X;
            result.X = x;
            result.Y = y;
            result.Z = z;
        }
        public static Vector3 Reflect(Vector3 vector, Vector3 normal)
        {
            float num = vector.X * normal.X + vector.Y * normal.Y + vector.Z * normal.Z;
            Vector3 result;
            result.X = vector.X - 2f * num * normal.X;
            result.Y = vector.Y - 2f * num * normal.Y;
            result.Z = vector.Z - 2f * num * normal.Z;
            return result;
        }
        public static void Reflect(ref Vector3 vector, ref Vector3 normal, out Vector3 result)
        {
            float num = vector.X * normal.X + vector.Y * normal.Y + vector.Z * normal.Z;
            result.X = vector.X - 2f * num * normal.X;
            result.Y = vector.Y - 2f * num * normal.Y;
            result.Z = vector.Z - 2f * num * normal.Z;
        }
        public static Vector3 Min(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = ((value1.X < value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
            result.Z = ((value1.Z < value2.Z) ? value1.Z : value2.Z);
            return result;
        }
        public static void Min(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = ((value1.X < value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
            result.Z = ((value1.Z < value2.Z) ? value1.Z : value2.Z);
        }
        public static Vector3 Max(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = ((value1.X > value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
            result.Z = ((value1.Z > value2.Z) ? value1.Z : value2.Z);
            return result;
        }
        public static void Max(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = ((value1.X > value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
            result.Z = ((value1.Z > value2.Z) ? value1.Z : value2.Z);
        }
        public static Vector3 Clamp(Vector3 value1, Vector3 min, Vector3 max)
        {
            float num = value1.X;
            num = ((num > max.X) ? max.X : num);
            num = ((num < min.X) ? min.X : num);
            float num2 = value1.Y;
            num2 = ((num2 > max.Y) ? max.Y : num2);
            num2 = ((num2 < min.Y) ? min.Y : num2);
            float num3 = value1.Z;
            num3 = ((num3 > max.Z) ? max.Z : num3);
            num3 = ((num3 < min.Z) ? min.Z : num3);
            Vector3 result;
            result.X = num;
            result.Y = num2;
            result.Z = num3;
            return result;
        }
        public static void Clamp(ref Vector3 value1, ref Vector3 min, ref Vector3 max, out Vector3 result)
        {
            float num = value1.X;
            num = ((num > max.X) ? max.X : num);
            num = ((num < min.X) ? min.X : num);
            float num2 = value1.Y;
            num2 = ((num2 > max.Y) ? max.Y : num2);
            num2 = ((num2 < min.Y) ? min.Y : num2);
            float num3 = value1.Z;
            num3 = ((num3 > max.Z) ? max.Z : num3);
            num3 = ((num3 < min.Z) ? min.Z : num3);
            result.X = num;
            result.Y = num2;
            result.Z = num3;
        }
        public static Vector3 Lerp(Vector3 value1, Vector3 value2, float amount)
        {
            Vector3 result;
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
            return result;
        }
        public static void Lerp(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
        {
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
        }
        public static Vector3 Barycentric(Vector3 value1, Vector3 value2, Vector3 value3, float amount1, float amount2)
        {
            Vector3 result;
            result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
            result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
            result.Z = value1.Z + amount1 * (value2.Z - value1.Z) + amount2 * (value3.Z - value1.Z);
            return result;
        }
        public static void Barycentric(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, float amount1, float amount2, out Vector3 result)
        {
            result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
            result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
            result.Z = value1.Z + amount1 * (value2.Z - value1.Z) + amount2 * (value3.Z - value1.Z);
        }
        public static Vector3 SmoothStep(Vector3 value1, Vector3 value2, float amount)
        {
            amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
            amount = amount * amount * (3f - 2f * amount);
            Vector3 result;
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
            return result;
        }
        public static void SmoothStep(ref Vector3 value1, ref Vector3 value2, float amount, out Vector3 result)
        {
            amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
            amount = amount * amount * (3f - 2f * amount);
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            result.Z = value1.Z + (value2.Z - value1.Z) * amount;
        }
        public static Vector3 CatmullRom(Vector3 value1, Vector3 value2, Vector3 value3, Vector3 value4, float amount)
        {
            float num = amount * amount;
            float num2 = amount * num;
            Vector3 result;
            result.X = 0.5f * (2f * value2.X + (-value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4f * value3.X - value4.X) * num + (-value1.X + 3f * value2.X - 3f * value3.X + value4.X) * num2);
            result.Y = 0.5f * (2f * value2.Y + (-value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4f * value3.Y - value4.Y) * num + (-value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * num2);
            result.Z = 0.5f * (2f * value2.Z + (-value1.Z + value3.Z) * amount + (2f * value1.Z - 5f * value2.Z + 4f * value3.Z - value4.Z) * num + (-value1.Z + 3f * value2.Z - 3f * value3.Z + value4.Z) * num2);
            return result;
        }
        public static void CatmullRom(ref Vector3 value1, ref Vector3 value2, ref Vector3 value3, ref Vector3 value4, float amount, out Vector3 result)
        {
            float num = amount * amount;
            float num2 = amount * num;
            result.X = 0.5f * (2f * value2.X + (-value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4f * value3.X - value4.X) * num + (-value1.X + 3f * value2.X - 3f * value3.X + value4.X) * num2);
            result.Y = 0.5f * (2f * value2.Y + (-value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4f * value3.Y - value4.Y) * num + (-value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * num2);
            result.Z = 0.5f * (2f * value2.Z + (-value1.Z + value3.Z) * amount + (2f * value1.Z - 5f * value2.Z + 4f * value3.Z - value4.Z) * num + (-value1.Z + 3f * value2.Z - 3f * value3.Z + value4.Z) * num2);
        }
        public static Vector3 Hermite(Vector3 value1, Vector3 tangent1, Vector3 value2, Vector3 tangent2, float amount)
        {
            float num = amount * amount;
            float num2 = amount * num;
            float num3 = 2f * num2 - 3f * num + 1f;
            float num4 = -2f * num2 + 3f * num;
            float num5 = num2 - 2f * num + amount;
            float num6 = num2 - num;
            Vector3 result;
            result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
            result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
            result.Z = value1.Z * num3 + value2.Z * num4 + tangent1.Z * num5 + tangent2.Z * num6;
            return result;
        }
        public static void Hermite(ref Vector3 value1, ref Vector3 tangent1, ref Vector3 value2, ref Vector3 tangent2, float amount, out Vector3 result)
        {
            float num = amount * amount;
            float num2 = amount * num;
            float num3 = 2f * num2 - 3f * num + 1f;
            float num4 = -2f * num2 + 3f * num;
            float num5 = num2 - 2f * num + amount;
            float num6 = num2 - num;
            result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
            result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
            result.Z = value1.Z * num3 + value2.Z * num4 + tangent1.Z * num5 + tangent2.Z * num6;
        }
        public static Vector3 Transform(Vector3 position, Matrix matrix)
        {
            float x = position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41;
            float y = position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42;
            float z = position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43;
            Vector3 result;
            result.X = x;
            result.Y = y;
            result.Z = z;
            return result;
        }
        public static void Transform(ref Vector3 position, ref Matrix matrix, out Vector3 result)
        {
            float x = position.X * matrix.M11 + position.Y * matrix.M21 + position.Z * matrix.M31 + matrix.M41;
            float y = position.X * matrix.M12 + position.Y * matrix.M22 + position.Z * matrix.M32 + matrix.M42;
            float z = position.X * matrix.M13 + position.Y * matrix.M23 + position.Z * matrix.M33 + matrix.M43;
            result.X = x;
            result.Y = y;
            result.Z = z;
        }
        public static Vector3 TransformNormal(Vector3 normal, Matrix matrix)
        {
            float x = normal.X * matrix.M11 + normal.Y * matrix.M21 + normal.Z * matrix.M31;
            float y = normal.X * matrix.M12 + normal.Y * matrix.M22 + normal.Z * matrix.M32;
            float z = normal.X * matrix.M13 + normal.Y * matrix.M23 + normal.Z * matrix.M33;
            Vector3 result;
            result.X = x;
            result.Y = y;
            result.Z = z;
            return result;
        }
        public static void TransformNormal(ref Vector3 normal, ref Matrix matrix, out Vector3 result)
        {
            float x = normal.X * matrix.M11 + normal.Y * matrix.M21 + normal.Z * matrix.M31;
            float y = normal.X * matrix.M12 + normal.Y * matrix.M22 + normal.Z * matrix.M32;
            float z = normal.X * matrix.M13 + normal.Y * matrix.M23 + normal.Z * matrix.M33;
            result.X = x;
            result.Y = y;
            result.Z = z;
        
        }

        public static void Transform(Vector3[] sourceArray, ref Matrix matrix, Vector3[] destinationArray)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException("sourceArray");
            }
            if (destinationArray == null)
            {
                throw new ArgumentNullException("destinationArray");
            }
            if (destinationArray.Length < sourceArray.Length)
            {
                throw new ArgumentException("NotEnoughTargetSize");
            }
            for (int i = 0; i < sourceArray.Length; i++)
            {
                float x = sourceArray[i].X;
                float y = sourceArray[i].Y;
                float z = sourceArray[i].Z;
                destinationArray[i].X = x * matrix.M11 + y * matrix.M21 + z * matrix.M31 + matrix.M41;
                destinationArray[i].Y = x * matrix.M12 + y * matrix.M22 + z * matrix.M32 + matrix.M42;
                destinationArray[i].Z = x * matrix.M13 + y * matrix.M23 + z * matrix.M33 + matrix.M43;
            }
        }
        public static void Transform(Vector3[] sourceArray, int sourceIndex, ref Matrix matrix, Vector3[] destinationArray, int destinationIndex, int length)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException("sourceArray");
            }
            if (destinationArray == null)
            {
                throw new ArgumentNullException("destinationArray");
            }
            if ((long)sourceArray.Length < (long)sourceIndex + (long)length)
            {
                throw new ArgumentException("NotEnoughSourceSize");
            }
            if ((long)destinationArray.Length < (long)destinationIndex + (long)length)
            {
                throw new ArgumentException("NotEnoughTargetSize");
            }
            while (length > 0)
            {
                float x = sourceArray[sourceIndex].X;
                float y = sourceArray[sourceIndex].Y;
                float z = sourceArray[sourceIndex].Z;
                destinationArray[destinationIndex].X = x * matrix.M11 + y * matrix.M21 + z * matrix.M31 + matrix.M41;
                destinationArray[destinationIndex].Y = x * matrix.M12 + y * matrix.M22 + z * matrix.M32 + matrix.M42;
                destinationArray[destinationIndex].Z = x * matrix.M13 + y * matrix.M23 + z * matrix.M33 + matrix.M43;
                sourceIndex++;
                destinationIndex++;
                length--;
            }
        }
        public static void TransformNormal(Vector3[] sourceArray, ref Matrix matrix, Vector3[] destinationArray)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException("sourceArray");
            }
            if (destinationArray == null)
            {
                throw new ArgumentNullException("destinationArray");
            }
            if (destinationArray.Length < sourceArray.Length)
            {
                throw new ArgumentException("NotEnoughTargetSize");
            }
            for (int i = 0; i < sourceArray.Length; i++)
            {
                float x = sourceArray[i].X;
                float y = sourceArray[i].Y;
                float z = sourceArray[i].Z;
                destinationArray[i].X = x * matrix.M11 + y * matrix.M21 + z * matrix.M31;
                destinationArray[i].Y = x * matrix.M12 + y * matrix.M22 + z * matrix.M32;
                destinationArray[i].Z = x * matrix.M13 + y * matrix.M23 + z * matrix.M33;
            }
        }
        public static void TransformNormal(Vector3[] sourceArray, int sourceIndex, ref Matrix matrix, Vector3[] destinationArray, int destinationIndex, int length)
        {
            if (sourceArray == null)
            {
                throw new ArgumentNullException("sourceArray");
            }
            if (destinationArray == null)
            {
                throw new ArgumentNullException("destinationArray");
            }
            if ((long)sourceArray.Length < (long)sourceIndex + (long)length)
            {
                throw new ArgumentException("NotEnoughSourceSize");
            }
            if ((long)destinationArray.Length < (long)destinationIndex + (long)length)
            {
                throw new ArgumentException("NotEnoughTargetSize");
            }
            while (length > 0)
            {
                float x = sourceArray[sourceIndex].X;
                float y = sourceArray[sourceIndex].Y;
                float z = sourceArray[sourceIndex].Z;
                destinationArray[destinationIndex].X = x * matrix.M11 + y * matrix.M21 + z * matrix.M31;
                destinationArray[destinationIndex].Y = x * matrix.M12 + y * matrix.M22 + z * matrix.M32;
                destinationArray[destinationIndex].Z = x * matrix.M13 + y * matrix.M23 + z * matrix.M33;
                sourceIndex++;
                destinationIndex++;
                length--;
            }
        }

        public static Vector3 Negate(Vector3 value)
        {
            Vector3 result;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            return result;
        }
        public static void Negate(ref Vector3 value, out Vector3 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
        }
        public static Vector3 Add(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
            return result;
        }
        public static void Add(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
        }
        public static Vector3 Subtract(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
            return result;
        }
        public static void Subtract(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
        }
        public static Vector3 Multiply(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
            return result;
        }
        public static void Multiply(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
        }
        public static Vector3 Multiply(Vector3 value1, float scaleFactor)
        {
            Vector3 result;
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
            return result;
        }
        public static void Multiply(ref Vector3 value1, float scaleFactor, out Vector3 result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            result.Z = value1.Z * scaleFactor;
        }
        public static Vector3 Divide(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
            return result;
        }
        public static void Divide(ref Vector3 value1, ref Vector3 value2, out Vector3 result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
        }
        public static Vector3 Divide(Vector3 value1, float value2)
        {
            float num = 1f / value2;
            Vector3 result;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            result.Z = value1.Z * num;
            return result;
        }
        public static void Divide(ref Vector3 value1, float value2, out Vector3 result)
        {
            float num = 1f / value2;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            result.Z = value1.Z * num;
        }
        public static Vector3 operator -(Vector3 value)
        {
            Vector3 result;
            result.X = -value.X;
            result.Y = -value.Y;
            result.Z = -value.Z;
            return result;
        }
        public static bool operator ==(Vector3 value1, Vector3 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y && value1.Z == value2.Z;
        }
        public static bool operator !=(Vector3 value1, Vector3 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y || value1.Z != value2.Z;
        }
        public static Vector3 operator +(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            result.Z = value1.Z + value2.Z;
            return result;
        }
        public static Vector3 operator -(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            result.Z = value1.Z - value2.Z;
            return result;
        }
        public static Vector3 operator *(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            result.Z = value1.Z * value2.Z;
            return result;
        }
        public static Vector3 operator *(Vector3 value, float scaleFactor)
        {
            Vector3 result;
            result.X = value.X * scaleFactor;
            result.Y = value.Y * scaleFactor;
            result.Z = value.Z * scaleFactor;
            return result;
        }
        public static Vector3 operator *(float scaleFactor, Vector3 value)
        {
            Vector3 result;
            result.X = value.X * scaleFactor;
            result.Y = value.Y * scaleFactor;
            result.Z = value.Z * scaleFactor;
            return result;
        }
        public static Vector3 operator /(Vector3 value1, Vector3 value2)
        {
            Vector3 result;
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            result.Z = value1.Z / value2.Z;
            return result;
        }
        public static Vector3 operator /(Vector3 value, float divider)
        {
            float num = 1f / divider;
            Vector3 result;
            result.X = value.X * num;
            result.Y = value.Y * num;
            result.Z = value.Z * num;
            return result;
        }
    }
}
