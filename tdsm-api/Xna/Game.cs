using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Globalization;
using tdsm.api;

namespace Microsoft.Xna.Framework
{
    public class Game
    {
        public ContentManager Content { get; set; }
        public GameWindow Window { get; set; }
        public GraphicsDevice GraphicsDevice { get; set; }
        public bool IsActive { get; set; }
        public bool IsFixedTimeStep { get; set; }
        public bool IsMouseVisible { get; set; }

        public Game()
        {
            Content = new ContentManager();
            Window = new GameWindow();
        }

        protected virtual void Initialize() { }
        protected virtual void LoadContent() { }
        protected virtual void Draw(GameTime gameTime) { }
        protected virtual void Update(GameTime gameTime) { }
        protected virtual void UnloadContent() { }

        public void Exit() { }
    }

    public class GraphicsDeviceManager
    {
        public GraphicsDeviceManager(Game game) { }

        public bool IsFullScreen { get; set; }
        public int PreferredBackBufferWidth { get; set; }
        public int PreferredBackBufferHeight { get; set; }
        public bool SynchronizeWithVerticalRetrace { get; set; }

        public void ToggleFullScreen() { }
        public void ApplyChanges() { }
    }
    public interface IPackedVector
    {
        /// <summary>Expands the packed representation into a Vector4.</summary>
        Vector4 ToVector4();
        /// <summary>Sets the packed representation from a Vector4.</summary>
        /// <param name="vector">The vector to create the packed representation from.</param>
        void PackFromVector4(Vector4 vector);
    }
    public interface IPackedVector<TPacked> : IPackedVector
    {
        /// <summary>Directly gets or sets the packed representation of the value.</summary>
        TPacked PackedValue
        {
            get;
            set;
        }
    }

    public struct Vector2
    {
        public static void Test(ref Vector2[] arr)
        {
            var col = new Vector2[0];
        }
        public static Vector2[] Array;

        /// <summary>Gets or sets the x-component of the vector.</summary>
        public float X;
        /// <summary>Gets or sets the y-component of the vector.</summary>
        public float Y;
        private static Vector2 _zero = default(Vector2);
        private static Vector2 _one = new Vector2(1f, 1f);
        private static Vector2 _unitX = new Vector2(1f, 0f);
        private static Vector2 _unitY = new Vector2(0f, 1f);
        /// <summary>Returns a Vector2 with all of its components set to zero.</summary>
        public static Vector2 Zero
        {
            get
            {
                return Vector2._zero;
            }
        }
        /// <summary>Returns a Vector2 with both of its components set to one.</summary>
        public static Vector2 One
        {
            get
            {
                return Vector2._one;
            }
        }
        /// <summary>Returns the unit vector for the x-axis.</summary>
        public static Vector2 UnitX
        {
            get
            {
                return Vector2._unitX;
            }
        }
        /// <summary>Returns the unit vector for the y-axis.</summary>
        public static Vector2 UnitY
        {
            get
            {
                return Vector2._unitY;
            }
        }
        /// <summary>Initializes a new instance of Vector2.</summary>
        /// <param name="x">Initial value for the x-component of the vector.</param>
        /// <param name="y">Initial value for the y-component of the vector.</param>
        public Vector2(float x, float y)
        {
            this.X = x;
            this.Y = y;
        }
        /// <summary>Creates a new instance of Vector2.</summary>
        /// <param name="value">Value to initialize both components to.</param>
        public Vector2(float value)
        {
            this.Y = value;
            this.X = value;
        }
        /// <summary>Retrieves a string representation of the current object.</summary>
        public override string ToString()
        {
            CultureInfo currentCulture = CultureInfo.CurrentCulture;
            return string.Format(currentCulture, "{{X:{0} Y:{1}}}", new object[]
			{
				this.X.ToString(currentCulture),
				this.Y.ToString(currentCulture)
			});
        }
        /// <summary>Determines whether the specified Object is equal to the Vector2.</summary>
        /// <param name="other">The Object to compare with the current Vector2.</param>
        public bool Equals(Vector2 other)
        {
            return this.X == other.X && this.Y == other.Y;
        }
        /// <summary>Returns a value that indicates whether the current instance is equal to a specified object.</summary>
        /// <param name="obj">Object to make the comparison with.</param>
        public override bool Equals(object obj)
        {
            bool result = false;
            if (obj is Vector2)
            {
                result = this.Equals((Vector2)obj);
            }
            return result;
        }
        /// <summary>Gets the hash code of the vector object.</summary>
        public override int GetHashCode()
        {
            return this.X.GetHashCode() + this.Y.GetHashCode();
        }
        /// <summary>Calculates the length of the vector.</summary>
        public float Length()
        {
            float num = this.X * this.X + this.Y * this.Y;
            return (float)Math.Sqrt((double)num);
        }
        /// <summary>Calculates the length of the vector squared.</summary>
        public float LengthSquared()
        {
            return this.X * this.X + this.Y * this.Y;
        }
        /// <summary>Calculates the distance between two vectors.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        public static float Distance(Vector2 value1, Vector2 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = num * num + num2 * num2;
            return (float)Math.Sqrt((double)num3);
        }
        /// <summary>Calculates the distance between two vectors.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        /// <param name="result">[OutAttribute] The distance between the vectors.</param>
        public static void Distance(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            float num3 = num * num + num2 * num2;
            result = (float)Math.Sqrt((double)num3);
        }
        /// <summary>Calculates the distance between two vectors squared.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        public static float DistanceSquared(Vector2 value1, Vector2 value2)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            return num * num + num2 * num2;
        }
        /// <summary>Calculates the distance between two vectors squared.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        /// <param name="result">[OutAttribute] The distance between the vectors squared.</param>
        public static void DistanceSquared(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            float num = value1.X - value2.X;
            float num2 = value1.Y - value2.Y;
            result = num * num + num2 * num2;
        }
        /// <summary>Calculates the dot product of two vectors. If the two vectors are unit vectors, the dot product returns a floating point value between -1 and 1 that can be used to determine some properties of the angle between two vectors. For example, it can show whether the vectors are orthogonal, parallel, or have an acute or obtuse angle between them.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        public static float Dot(Vector2 value1, Vector2 value2)
        {
            return value1.X * value2.X + value1.Y * value2.Y;
        }
        /// <summary>Calculates the dot product of two vectors and writes the result to a user-specified variable. If the two vectors are unit vectors, the dot product returns a floating point value between -1 and 1 that can be used to determine some properties of the angle between two vectors. For example, it can show whether the vectors are orthogonal, parallel, or have an acute or obtuse angle between them.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        /// <param name="result">[OutAttribute] The dot product of the two vectors.</param>
        public static void Dot(ref Vector2 value1, ref Vector2 value2, out float result)
        {
            result = value1.X * value2.X + value1.Y * value2.Y;
        }
        /// <summary>Turns the current vector into a unit vector. The result is a vector one unit in length pointing in the same direction as the original vector.</summary>
        public void Normalize()
        {
            float num = this.X * this.X + this.Y * this.Y;
            float num2 = 1f / (float)Math.Sqrt((double)num);
            this.X *= num2;
            this.Y *= num2;
        }
        /// <summary>Creates a unit vector from the specified vector. The result is a vector one unit in length pointing in the same direction as the original vector.</summary>
        /// <param name="value">Source Vector2.</param>
        public static Vector2 Normalize(Vector2 value)
        {
            float num = value.X * value.X + value.Y * value.Y;
            float num2 = 1f / (float)Math.Sqrt((double)num);
            Vector2 result;
            result.X = value.X * num2;
            result.Y = value.Y * num2;
            return result;
        }
        /// <summary>Creates a unit vector from the specified vector, writing the result to a user-specified variable. The result is a vector one unit in length pointing in the same direction as the original vector.</summary>
        /// <param name="value">Source vector.</param>
        /// <param name="result">[OutAttribute] Normalized vector.</param>
        public static void Normalize(ref Vector2 value, out Vector2 result)
        {
            float num = value.X * value.X + value.Y * value.Y;
            float num2 = 1f / (float)Math.Sqrt((double)num);
            result.X = value.X * num2;
            result.Y = value.Y * num2;
        }
        /// <summary>Determines the reflect vector of the given vector and normal.</summary>
        /// <param name="vector">Source vector.</param>
        /// <param name="normal">Normal of vector.</param>
        public static Vector2 Reflect(Vector2 vector, Vector2 normal)
        {
            float num = vector.X * normal.X + vector.Y * normal.Y;
            Vector2 result;
            result.X = vector.X - 2f * num * normal.X;
            result.Y = vector.Y - 2f * num * normal.Y;
            return result;
        }
        /// <summary>Determines the reflect vector of the given vector and normal.</summary>
        /// <param name="vector">Source vector.</param>
        /// <param name="normal">Normal of vector.</param>
        /// <param name="result">[OutAttribute] The created reflect vector.</param>
        public static void Reflect(ref Vector2 vector, ref Vector2 normal, out Vector2 result)
        {
            float num = vector.X * normal.X + vector.Y * normal.Y;
            result.X = vector.X - 2f * num * normal.X;
            result.Y = vector.Y - 2f * num * normal.Y;
        }
        /// <summary>Returns a vector that contains the lowest value from each matching pair of components.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        public static Vector2 Min(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = ((value1.X < value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
            return result;
        }
        /// <summary>Returns a vector that contains the lowest value from each matching pair of components.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        /// <param name="result">[OutAttribute] The minimized vector.</param>
        public static void Min(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = ((value1.X < value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y < value2.Y) ? value1.Y : value2.Y);
        }
        /// <summary>Returns a vector that contains the highest value from each matching pair of components.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        public static Vector2 Max(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = ((value1.X > value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
            return result;
        }
        /// <summary>Returns a vector that contains the highest value from each matching pair of components.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        /// <param name="result">[OutAttribute] The maximized vector.</param>
        public static void Max(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = ((value1.X > value2.X) ? value1.X : value2.X);
            result.Y = ((value1.Y > value2.Y) ? value1.Y : value2.Y);
        }
        /// <summary>Restricts a value to be within a specified range.</summary>
        /// <param name="value1">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        public static Vector2 Clamp(Vector2 value1, Vector2 min, Vector2 max)
        {
            float num = value1.X;
            num = ((num > max.X) ? max.X : num);
            num = ((num < min.X) ? min.X : num);
            float num2 = value1.Y;
            num2 = ((num2 > max.Y) ? max.Y : num2);
            num2 = ((num2 < min.Y) ? min.Y : num2);
            Vector2 result;
            result.X = num;
            result.Y = num2;
            return result;
        }
        /// <summary>Restricts a value to be within a specified range.</summary>
        /// <param name="value1">The value to clamp.</param>
        /// <param name="min">The minimum value.</param>
        /// <param name="max">The maximum value.</param>
        /// <param name="result">[OutAttribute] The clamped value.</param>
        public static void Clamp(ref Vector2 value1, ref Vector2 min, ref Vector2 max, out Vector2 result)
        {
            float num = value1.X;
            num = ((num > max.X) ? max.X : num);
            num = ((num < min.X) ? min.X : num);
            float num2 = value1.Y;
            num2 = ((num2 > max.Y) ? max.Y : num2);
            num2 = ((num2 < min.Y) ? min.Y : num2);
            result.X = num;
            result.Y = num2;
        }
        /// <summary>Performs a linear interpolation between two vectors.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        public static Vector2 Lerp(Vector2 value1, Vector2 value2, float amount)
        {
            Vector2 result;
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            return result;
        }
        /// <summary>Performs a linear interpolation between two vectors.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <param name="result">[OutAttribute] The result of the interpolation.</param>
        public static void Lerp(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
        {
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
        }
        /// <summary>Returns a Vector2 containing the 2D Cartesian coordinates of a point specified in barycentric (areal) coordinates relative to a 2D triangle.</summary>
        /// <param name="value1">A Vector2 containing the 2D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A Vector2 containing the 2D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A Vector2 containing the 2D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in value2).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in value3).</param>
        public static Vector2 Barycentric(Vector2 value1, Vector2 value2, Vector2 value3, float amount1, float amount2)
        {
            Vector2 result;
            result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
            result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
            return result;
        }
        /// <summary>Returns a Vector2 containing the 2D Cartesian coordinates of a point specified in barycentric (areal) coordinates relative to a 2D triangle.</summary>
        /// <param name="value1">A Vector2 containing the 2D Cartesian coordinates of vertex 1 of the triangle.</param>
        /// <param name="value2">A Vector2 containing the 2D Cartesian coordinates of vertex 2 of the triangle.</param>
        /// <param name="value3">A Vector2 containing the 2D Cartesian coordinates of vertex 3 of the triangle.</param>
        /// <param name="amount1">Barycentric coordinate b2, which expresses the weighting factor toward vertex 2 (specified in value2).</param>
        /// <param name="amount2">Barycentric coordinate b3, which expresses the weighting factor toward vertex 3 (specified in value3).</param>
        /// <param name="result">[OutAttribute] The 2D Cartesian coordinates of the specified point are placed in this Vector2 on exit.</param>
        public static void Barycentric(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, float amount1, float amount2, out Vector2 result)
        {
            result.X = value1.X + amount1 * (value2.X - value1.X) + amount2 * (value3.X - value1.X);
            result.Y = value1.Y + amount1 * (value2.Y - value1.Y) + amount2 * (value3.Y - value1.Y);
        }
        /// <summary>Interpolates between two values using a cubic equation.</summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <param name="amount">Weighting value.</param>
        public static Vector2 SmoothStep(Vector2 value1, Vector2 value2, float amount)
        {
            amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
            amount = amount * amount * (3f - 2f * amount);
            Vector2 result;
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
            return result;
        }
        /// <summary>Interpolates between two values using a cubic equation.</summary>
        /// <param name="value1">Source value.</param>
        /// <param name="value2">Source value.</param>
        /// <param name="amount">Weighting value.</param>
        /// <param name="result">[OutAttribute] The interpolated value.</param>
        public static void SmoothStep(ref Vector2 value1, ref Vector2 value2, float amount, out Vector2 result)
        {
            amount = ((amount > 1f) ? 1f : ((amount < 0f) ? 0f : amount));
            amount = amount * amount * (3f - 2f * amount);
            result.X = value1.X + (value2.X - value1.X) * amount;
            result.Y = value1.Y + (value2.Y - value1.Y) * amount;
        }
        /// <summary>Performs a Catmull-Rom interpolation using the specified positions.</summary>
        /// <param name="value1">The first position in the interpolation.</param>
        /// <param name="value2">The second position in the interpolation.</param>
        /// <param name="value3">The third position in the interpolation.</param>
        /// <param name="value4">The fourth position in the interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        public static Vector2 CatmullRom(Vector2 value1, Vector2 value2, Vector2 value3, Vector2 value4, float amount)
        {
            float num = amount * amount;
            float num2 = amount * num;
            Vector2 result;
            result.X = 0.5f * (2f * value2.X + (-value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4f * value3.X - value4.X) * num + (-value1.X + 3f * value2.X - 3f * value3.X + value4.X) * num2);
            result.Y = 0.5f * (2f * value2.Y + (-value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4f * value3.Y - value4.Y) * num + (-value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * num2);
            return result;
        }
        /// <summary>Performs a Catmull-Rom interpolation using the specified positions.</summary>
        /// <param name="value1">The first position in the interpolation.</param>
        /// <param name="value2">The second position in the interpolation.</param>
        /// <param name="value3">The third position in the interpolation.</param>
        /// <param name="value4">The fourth position in the interpolation.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <param name="result">[OutAttribute] A vector that is the result of the Catmull-Rom interpolation.</param>
        public static void CatmullRom(ref Vector2 value1, ref Vector2 value2, ref Vector2 value3, ref Vector2 value4, float amount, out Vector2 result)
        {
            float num = amount * amount;
            float num2 = amount * num;
            result.X = 0.5f * (2f * value2.X + (-value1.X + value3.X) * amount + (2f * value1.X - 5f * value2.X + 4f * value3.X - value4.X) * num + (-value1.X + 3f * value2.X - 3f * value3.X + value4.X) * num2);
            result.Y = 0.5f * (2f * value2.Y + (-value1.Y + value3.Y) * amount + (2f * value1.Y - 5f * value2.Y + 4f * value3.Y - value4.Y) * num + (-value1.Y + 3f * value2.Y - 3f * value3.Y + value4.Y) * num2);
        }
        /// <summary>Performs a Hermite spline interpolation.</summary>
        /// <param name="value1">Source position vector.</param>
        /// <param name="tangent1">Source tangent vector.</param>
        /// <param name="value2">Source position vector.</param>
        /// <param name="tangent2">Source tangent vector.</param>
        /// <param name="amount">Weighting factor.</param>
        public static Vector2 Hermite(Vector2 value1, Vector2 tangent1, Vector2 value2, Vector2 tangent2, float amount)
        {
            float num = amount * amount;
            float num2 = amount * num;
            float num3 = 2f * num2 - 3f * num + 1f;
            float num4 = -2f * num2 + 3f * num;
            float num5 = num2 - 2f * num + amount;
            float num6 = num2 - num;
            Vector2 result;
            result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
            result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
            return result;
        }
        /// <summary>Performs a Hermite spline interpolation.</summary>
        /// <param name="value1">Source position vector.</param>
        /// <param name="tangent1">Source tangent vector.</param>
        /// <param name="value2">Source position vector.</param>
        /// <param name="tangent2">Source tangent vector.</param>
        /// <param name="amount">Weighting factor.</param>
        /// <param name="result">[OutAttribute] The result of the Hermite spline interpolation.</param>
        public static void Hermite(ref Vector2 value1, ref Vector2 tangent1, ref Vector2 value2, ref Vector2 tangent2, float amount, out Vector2 result)
        {
            float num = amount * amount;
            float num2 = amount * num;
            float num3 = 2f * num2 - 3f * num + 1f;
            float num4 = -2f * num2 + 3f * num;
            float num5 = num2 - 2f * num + amount;
            float num6 = num2 - num;
            result.X = value1.X * num3 + value2.X * num4 + tangent1.X * num5 + tangent2.X * num6;
            result.Y = value1.Y * num3 + value2.Y * num4 + tangent1.Y * num5 + tangent2.Y * num6;
        }
        ///// <summary>Transforms the vector (x, y, 0, 1) by the specified matrix.</summary>
        ///// <param name="position">The source vector.</param>
        ///// <param name="matrix">The transformation matrix.</param>
        //public static Vector2 Transform(Vector2 position, Matrix matrix)
        //{
        //    float x = position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41;
        //    float y = position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42;
        //    Vector2 result;
        //    result.X = x;
        //    result.Y = y;
        //    return result;
        //}
        ///// <summary>Transforms a Vector2 by the given Matrix.</summary>
        ///// <param name="position">The source Vector2.</param>
        ///// <param name="matrix">The transformation Matrix.</param>
        ///// <param name="result">[OutAttribute] The Vector2 resulting from the transformation.</param>
        //public static void Transform(ref Vector2 position, ref Matrix matrix, out Vector2 result)
        //{
        //    float x = position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41;
        //    float y = position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42;
        //    result.X = x;
        //    result.Y = y;
        //}
        ///// <summary>Transforms a 2D vector normal by a matrix.</summary>
        ///// <param name="normal">The source vector.</param>
        ///// <param name="matrix">The transformation matrix.</param>
        //public static Vector2 TransformNormal(Vector2 normal, Matrix matrix)
        //{
        //    float x = normal.X * matrix.M11 + normal.Y * matrix.M21;
        //    float y = normal.X * matrix.M12 + normal.Y * matrix.M22;
        //    Vector2 result;
        //    result.X = x;
        //    result.Y = y;
        //    return result;
        //}
        ///// <summary>Transforms a vector normal by a matrix.</summary>
        ///// <param name="normal">The source vector.</param>
        ///// <param name="matrix">The transformation matrix.</param>
        ///// <param name="result">[OutAttribute] The Vector2 resulting from the transformation.</param>
        //public static void TransformNormal(ref Vector2 normal, ref Matrix matrix, out Vector2 result)
        //{
        //    float x = normal.X * matrix.M11 + normal.Y * matrix.M21;
        //    float y = normal.X * matrix.M12 + normal.Y * matrix.M22;
        //    result.X = x;
        //    result.Y = y;
        //}
        ///// <summary>Transforms a single Vector2, or the vector normal (x, y, 0, 0), by a specified Quaternion rotation.</summary>
        ///// <param name="value">The vector to rotate.</param>
        ///// <param name="rotation">The Quaternion rotation to apply.</param>
        //public static Vector2 Transform(Vector2 value, Quaternion rotation)
        //{
        //    float num = rotation.X + rotation.X;
        //    float num2 = rotation.Y + rotation.Y;
        //    float num3 = rotation.Z + rotation.Z;
        //    float num4 = rotation.W * num3;
        //    float num5 = rotation.X * num;
        //    float num6 = rotation.X * num2;
        //    float num7 = rotation.Y * num2;
        //    float num8 = rotation.Z * num3;
        //    float x = value.X * (1f - num7 - num8) + value.Y * (num6 - num4);
        //    float y = value.X * (num6 + num4) + value.Y * (1f - num5 - num8);
        //    Vector2 result;
        //    result.X = x;
        //    result.Y = y;
        //    return result;
        //}
        ///// <summary>Transforms a Vector2, or the vector normal (x, y, 0, 0), by a specified Quaternion rotation.</summary>
        ///// <param name="value">The vector to rotate.</param>
        ///// <param name="rotation">The Quaternion rotation to apply.</param>
        ///// <param name="result">[OutAttribute] An existing Vector2 filled in with the result of the rotation.</param>
        //public static void Transform(ref Vector2 value, ref Quaternion rotation, out Vector2 result)
        //{
        //    float num = rotation.X + rotation.X;
        //    float num2 = rotation.Y + rotation.Y;
        //    float num3 = rotation.Z + rotation.Z;
        //    float num4 = rotation.W * num3;
        //    float num5 = rotation.X * num;
        //    float num6 = rotation.X * num2;
        //    float num7 = rotation.Y * num2;
        //    float num8 = rotation.Z * num3;
        //    float x = value.X * (1f - num7 - num8) + value.Y * (num6 - num4);
        //    float y = value.X * (num6 + num4) + value.Y * (1f - num5 - num8);
        //    result.X = x;
        //    result.Y = y;
        //}
        ///// <summary>Transforms an array of Vector2s by a specified Matrix.</summary>
        ///// <param name="sourceArray">The array of Vector2s to transform.</param>
        ///// <param name="matrix">The transform Matrix to apply.</param>
        ///// <param name="destinationArray">An existing array into which the transformed Vector2s are written.</param>
        //public static void Transform(Vector2[] sourceArray, ref Matrix matrix, Vector2[] destinationArray)
        //{
        //    if (sourceArray == null)
        //    {
        //        throw new ArgumentNullException("sourceArray");
        //    }
        //    if (destinationArray == null)
        //    {
        //        throw new ArgumentNullException("destinationArray");
        //    }
        //    if (destinationArray.Length < sourceArray.Length)
        //    {
        //        throw new ArgumentException(FrameworkResources.NotEnoughTargetSize);
        //    }
        //    for (int i = 0; i < sourceArray.Length; i++)
        //    {
        //        float x = sourceArray[i].X;
        //        float y = sourceArray[i].Y;
        //        destinationArray[i].X = x * matrix.M11 + y * matrix.M21 + matrix.M41;
        //        destinationArray[i].Y = x * matrix.M12 + y * matrix.M22 + matrix.M42;
        //    }
        //}
        ///// <summary>Transforms a specified range in an array of Vector2s by a specified Matrix and places the results in a specified range in a destination array.</summary>
        ///// <param name="sourceArray">The source array.</param>
        ///// <param name="sourceIndex">The index of the first Vector2 to transform in the source array.</param>
        ///// <param name="matrix">The Matrix to transform by.</param>
        ///// <param name="destinationArray">The destination array into which the resulting Vector2s will be written.</param>
        ///// <param name="destinationIndex">The index of the position in the destination array where the first result Vector2 should be written.</param>
        ///// <param name="length">The number of Vector2s to be transformed.</param>
        //public static void Transform(Vector2[] sourceArray, int sourceIndex, ref Matrix matrix, Vector2[] destinationArray, int destinationIndex, int length)
        //{
        //    if (sourceArray == null)
        //    {
        //        throw new ArgumentNullException("sourceArray");
        //    }
        //    if (destinationArray == null)
        //    {
        //        throw new ArgumentNullException("destinationArray");
        //    }
        //    if ((long)sourceArray.Length < (long)sourceIndex + (long)length)
        //    {
        //        throw new ArgumentException(FrameworkResources.NotEnoughSourceSize);
        //    }
        //    if ((long)destinationArray.Length < (long)destinationIndex + (long)length)
        //    {
        //        throw new ArgumentException(FrameworkResources.NotEnoughTargetSize);
        //    }
        //    while (length > 0)
        //    {
        //        float x = sourceArray[sourceIndex].X;
        //        float y = sourceArray[sourceIndex].Y;
        //        destinationArray[destinationIndex].X = x * matrix.M11 + y * matrix.M21 + matrix.M41;
        //        destinationArray[destinationIndex].Y = x * matrix.M12 + y * matrix.M22 + matrix.M42;
        //        sourceIndex++;
        //        destinationIndex++;
        //        length--;
        //    }
        //}
        ///// <summary>Transforms an array of Vector2 vector normals by a specified Matrix.</summary>
        ///// <param name="sourceArray">The array of vector normals to transform.</param>
        ///// <param name="matrix">The transform Matrix to apply.</param>
        ///// <param name="destinationArray">An existing array into which the transformed vector normals are written.</param>
        //public static void TransformNormal(Vector2[] sourceArray, ref Matrix matrix, Vector2[] destinationArray)
        //{
        //    if (sourceArray == null)
        //    {
        //        throw new ArgumentNullException("sourceArray");
        //    }
        //    if (destinationArray == null)
        //    {
        //        throw new ArgumentNullException("destinationArray");
        //    }
        //    if (destinationArray.Length < sourceArray.Length)
        //    {
        //        throw new ArgumentException(FrameworkResources.NotEnoughTargetSize);
        //    }
        //    for (int i = 0; i < sourceArray.Length; i++)
        //    {
        //        float x = sourceArray[i].X;
        //        float y = sourceArray[i].Y;
        //        destinationArray[i].X = x * matrix.M11 + y * matrix.M21;
        //        destinationArray[i].Y = x * matrix.M12 + y * matrix.M22;
        //    }
        //}
        ///// <summary>Transforms a specified range in an array of Vector2 vector normals by a specified Matrix and places the results in a specified range in a destination array.</summary>
        ///// <param name="sourceArray">The source array.</param>
        ///// <param name="sourceIndex">The index of the first Vector2 to transform in the source array.</param>
        ///// <param name="matrix">The Matrix to apply.</param>
        ///// <param name="destinationArray">The destination array into which the resulting Vector2s are written.</param>
        ///// <param name="destinationIndex">The index of the position in the destination array where the first result Vector2 should be written.</param>
        ///// <param name="length">The number of vector normals to be transformed.</param>
        //public static void TransformNormal(Vector2[] sourceArray, int sourceIndex, ref Matrix matrix, Vector2[] destinationArray, int destinationIndex, int length)
        //{
        //    if (sourceArray == null)
        //    {
        //        throw new ArgumentNullException("sourceArray");
        //    }
        //    if (destinationArray == null)
        //    {
        //        throw new ArgumentNullException("destinationArray");
        //    }
        //    if ((long)sourceArray.Length < (long)sourceIndex + (long)length)
        //    {
        //        throw new ArgumentException(FrameworkResources.NotEnoughSourceSize);
        //    }
        //    if ((long)destinationArray.Length < (long)destinationIndex + (long)length)
        //    {
        //        throw new ArgumentException(FrameworkResources.NotEnoughTargetSize);
        //    }
        //    while (length > 0)
        //    {
        //        float x = sourceArray[sourceIndex].X;
        //        float y = sourceArray[sourceIndex].Y;
        //        destinationArray[destinationIndex].X = x * matrix.M11 + y * matrix.M21;
        //        destinationArray[destinationIndex].Y = x * matrix.M12 + y * matrix.M22;
        //        sourceIndex++;
        //        destinationIndex++;
        //        length--;
        //    }
        //}
        ///// <summary>Transforms an array of Vector2s by a specified Quaternion.</summary>
        ///// <param name="sourceArray">The array of Vector2s to transform.</param>
        ///// <param name="rotation">The transform Matrix to use.</param>
        ///// <param name="destinationArray">An existing array into which the transformed Vector2s are written.</param>
        //public static void Transform(Vector2[] sourceArray, ref Quaternion rotation, Vector2[] destinationArray)
        //{
        //    if (sourceArray == null)
        //    {
        //        throw new ArgumentNullException("sourceArray");
        //    }
        //    if (destinationArray == null)
        //    {
        //        throw new ArgumentNullException("destinationArray");
        //    }
        //    if (destinationArray.Length < sourceArray.Length)
        //    {
        //        throw new ArgumentException(FrameworkResources.NotEnoughTargetSize);
        //    }
        //    float num = rotation.X + rotation.X;
        //    float num2 = rotation.Y + rotation.Y;
        //    float num3 = rotation.Z + rotation.Z;
        //    float num4 = rotation.W * num3;
        //    float num5 = rotation.X * num;
        //    float num6 = rotation.X * num2;
        //    float num7 = rotation.Y * num2;
        //    float num8 = rotation.Z * num3;
        //    float num9 = 1f - num7 - num8;
        //    float num10 = num6 - num4;
        //    float num11 = num6 + num4;
        //    float num12 = 1f - num5 - num8;
        //    for (int i = 0; i < sourceArray.Length; i++)
        //    {
        //        float x = sourceArray[i].X;
        //        float y = sourceArray[i].Y;
        //        destinationArray[i].X = x * num9 + y * num10;
        //        destinationArray[i].Y = x * num11 + y * num12;
        //    }
        //}
        ///// <summary>Transforms a specified range in an array of Vector2s by a specified Quaternion and places the results in a specified range in a destination array.</summary>
        ///// <param name="sourceArray">The source array.</param>
        ///// <param name="sourceIndex">The index of the first Vector2 to transform in the source array.</param>
        ///// <param name="rotation">The Quaternion rotation to apply.</param>
        ///// <param name="destinationArray">The destination array into which the resulting Vector2s are written.</param>
        ///// <param name="destinationIndex">The index of the position in the destination array where the first result Vector2 should be written.</param>
        ///// <param name="length">The number of Vector2s to be transformed.</param>
        //public static void Transform(Vector2[] sourceArray, int sourceIndex, ref Quaternion rotation, Vector2[] destinationArray, int destinationIndex, int length)
        //{
        //    if (sourceArray == null)
        //    {
        //        throw new ArgumentNullException("sourceArray");
        //    }
        //    if (destinationArray == null)
        //    {
        //        throw new ArgumentNullException("destinationArray");
        //    }
        //    if ((long)sourceArray.Length < (long)sourceIndex + (long)length)
        //    {
        //        throw new ArgumentException(FrameworkResources.NotEnoughSourceSize);
        //    }
        //    if ((long)destinationArray.Length < (long)destinationIndex + (long)length)
        //    {
        //        throw new ArgumentException(FrameworkResources.NotEnoughTargetSize);
        //    }
        //    float num = rotation.X + rotation.X;
        //    float num2 = rotation.Y + rotation.Y;
        //    float num3 = rotation.Z + rotation.Z;
        //    float num4 = rotation.W * num3;
        //    float num5 = rotation.X * num;
        //    float num6 = rotation.X * num2;
        //    float num7 = rotation.Y * num2;
        //    float num8 = rotation.Z * num3;
        //    float num9 = 1f - num7 - num8;
        //    float num10 = num6 - num4;
        //    float num11 = num6 + num4;
        //    float num12 = 1f - num5 - num8;
        //    while (length > 0)
        //    {
        //        float x = sourceArray[sourceIndex].X;
        //        float y = sourceArray[sourceIndex].Y;
        //        destinationArray[destinationIndex].X = x * num9 + y * num10;
        //        destinationArray[destinationIndex].Y = x * num11 + y * num12;
        //        sourceIndex++;
        //        destinationIndex++;
        //        length--;
        //    }
        //}

        public static Vector2 Transform(Vector2 position, Matrix matrix)
        {
            //float x = position.X * matrix.M11 + position.Y * matrix.M21 + matrix.M41;
            //float y = position.X * matrix.M12 + position.Y * matrix.M22 + matrix.M42;
            //Vector2 result;
            //result.X = x;
            //result.Y = y;
            //return result;
            return position;
        }


        /// <summary>Returns a vector pointing in the opposite direction.</summary>
        /// <param name="value">Source vector.</param>
        public static Vector2 Negate(Vector2 value)
        {
            Vector2 result;
            result.X = -value.X;
            result.Y = -value.Y;
            return result;
        }
        /// <summary>Returns a vector pointing in the opposite direction.</summary>
        /// <param name="value">Source vector.</param>
        /// <param name="result">[OutAttribute] Vector pointing in the opposite direction.</param>
        public static void Negate(ref Vector2 value, out Vector2 result)
        {
            result.X = -value.X;
            result.Y = -value.Y;
        }
        /// <summary>Adds two vectors.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        public static Vector2 Add(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            return result;
        }
        /// <summary>Adds two vectors.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        /// <param name="result">[OutAttribute] Sum of the source vectors.</param>
        public static void Add(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
        }
        /// <summary>Subtracts a vector from a vector.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        public static Vector2 Subtract(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            return result;
        }
        /// <summary>Subtracts a vector from a vector.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        /// <param name="result">[OutAttribute] The result of the subtraction.</param>
        public static void Subtract(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
        }
        /// <summary>Multiplies the components of two vectors by each other.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        public static Vector2 Multiply(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            return result;
        }
        /// <summary>Multiplies the components of two vectors by each other.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        /// <param name="result">[OutAttribute] The result of the multiplication.</param>
        public static void Multiply(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
        }
        /// <summary>Multiplies a vector by a scalar value.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        public static Vector2 Multiply(Vector2 value1, float scaleFactor)
        {
            Vector2 result;
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
            return result;
        }
        /// <summary>Multiplies a vector by a scalar value.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <param name="result">[OutAttribute] The result of the multiplication.</param>
        public static void Multiply(ref Vector2 value1, float scaleFactor, out Vector2 result)
        {
            result.X = value1.X * scaleFactor;
            result.Y = value1.Y * scaleFactor;
        }
        /// <summary>Divides the components of a vector by the components of another vector.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Divisor vector.</param>
        public static Vector2 Divide(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            return result;
        }
        /// <summary>Divides the components of a vector by the components of another vector.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">The divisor.</param>
        /// <param name="result">[OutAttribute] The result of the division.</param>
        public static void Divide(ref Vector2 value1, ref Vector2 value2, out Vector2 result)
        {
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
        }
        /// <summary>Divides a vector by a scalar value.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="divider">The divisor.</param>
        public static Vector2 Divide(Vector2 value1, float divider)
        {
            float num = 1f / divider;
            Vector2 result;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            return result;
        }
        /// <summary>Divides a vector by a scalar value.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="divider">The divisor.</param>
        /// <param name="result">[OutAttribute] The result of the division.</param>
        public static void Divide(ref Vector2 value1, float divider, out Vector2 result)
        {
            float num = 1f / divider;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
        }
        /// <summary>Returns a vector pointing in the opposite direction.</summary>
        /// <param name="value">Source vector.</param>
        public static Vector2 operator -(Vector2 value)
        {
            Vector2 result;
            result.X = -value.X;
            result.Y = -value.Y;
            return result;
        }
        /// <summary>Tests vectors for equality.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        public static bool operator ==(Vector2 value1, Vector2 value2)
        {
            return value1.X == value2.X && value1.Y == value2.Y;
        }
        /// <summary>Tests vectors for inequality.</summary>
        /// <param name="value1">Vector to compare.</param>
        /// <param name="value2">Vector to compare.</param>
        public static bool operator !=(Vector2 value1, Vector2 value2)
        {
            return value1.X != value2.X || value1.Y != value2.Y;
        }
        /// <summary>Adds two vectors.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        public static Vector2 operator +(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X + value2.X;
            result.Y = value1.Y + value2.Y;
            return result;
        }
        /// <summary>Subtracts a vector from a vector.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">source vector.</param>
        public static Vector2 operator -(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X - value2.X;
            result.Y = value1.Y - value2.Y;
            return result;
        }
        /// <summary>Multiplies the components of two vectors by each other.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Source vector.</param>
        public static Vector2 operator *(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X * value2.X;
            result.Y = value1.Y * value2.Y;
            return result;
        }
        /// <summary>Multiplies a vector by a scalar value.</summary>
        /// <param name="value">Source vector.</param>
        /// <param name="scaleFactor">Scalar value.</param>
        public static Vector2 operator *(Vector2 value, float scaleFactor)
        {
            Vector2 result;
            result.X = value.X * scaleFactor;
            result.Y = value.Y * scaleFactor;
            return result;
        }
        /// <summary>Multiplies a vector by a scalar value.</summary>
        /// <param name="scaleFactor">Scalar value.</param>
        /// <param name="value">Source vector.</param>
        public static Vector2 operator *(float scaleFactor, Vector2 value)
        {
            Vector2 result;
            result.X = value.X * scaleFactor;
            result.Y = value.Y * scaleFactor;
            return result;
        }
        /// <summary>Divides the components of a vector by the components of another vector.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="value2">Divisor vector.</param>
        public static Vector2 operator /(Vector2 value1, Vector2 value2)
        {
            Vector2 result;
            result.X = value1.X / value2.X;
            result.Y = value1.Y / value2.Y;
            return result;
        }
        /// <summary>Divides a vector by a scalar value.</summary>
        /// <param name="value1">Source vector.</param>
        /// <param name="divider">The divisor.</param>
        public static Vector2 operator /(Vector2 value1, float divider)
        {
            float num = 1f / divider;
            Vector2 result;
            result.X = value1.X * num;
            result.Y = value1.Y * num;
            return result;
        }
    }
    public struct Vector3
    {
        public static Vector3[] Array;

        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }
    }
    public struct Vector4
    {
        public static Vector4[] Array;

        public float W;
        public float X;
        public float Y;
        public float Z;

        public Vector4(Vector2 value, float z, float w)
        {
            this.W = w;
            this.X = value.X;
            this.Y = value.Y;
            this.Z = z;
        }
    }
    public struct Rectangle
    {
        public static Rectangle[] Array;

        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Rectangle(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public bool Intersects(Rectangle value)
        {
            return value.X < this.X + this.Width && this.X < value.X + value.Width && value.Y < this.Y + this.Height && this.Y < value.Y + value.Height;
        }

        public bool Contains(Point value)
        {
            return this.X <= value.X && value.X < this.X + this.Width && this.Y <= value.Y && value.Y < this.Y + this.Height;
        }
    }
    public struct Matrix
    {
        public float M11, M12, M13, M14, M21, M22, M23, M24, M31, M32, M33, M34, M41, M42, M43, M44;

        public static Matrix CreateRotationZ(float radians)
        {
            float num = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            Matrix result;
            result.M11 = num;
            result.M12 = num2;
            result.M13 = 0f;
            result.M14 = 0f;
            result.M21 = -num2;
            result.M22 = num;
            result.M23 = 0f;
            result.M24 = 0f;
            result.M31 = 0f;
            result.M32 = 0f;
            result.M33 = 1f;
            result.M34 = 0f;
            result.M41 = 0f;
            result.M42 = 0f;
            result.M43 = 0f;
            result.M44 = 1f;
            return result;
        }

        public static Matrix CreateScale(float xScale, float yScale, float zScale)
        {
            Matrix result;
            result.M11 = xScale;
            result.M12 = 0f;
            result.M13 = 0f;
            result.M14 = 0f;
            result.M21 = 0f;
            result.M22 = yScale;
            result.M23 = 0f;
            result.M24 = 0f;
            result.M31 = 0f;
            result.M32 = 0f;
            result.M33 = zScale;
            result.M34 = 0f;
            result.M41 = 0f;
            result.M42 = 0f;
            result.M43 = 0f;
            result.M44 = 1f;
            return result;
        }

        public static Matrix operator *(Matrix matrix1, Matrix matrix2)
        {
            Matrix result;
            result.M11 = matrix1.M11 * matrix2.M11 + matrix1.M12 * matrix2.M21 + matrix1.M13 * matrix2.M31 + matrix1.M14 * matrix2.M41;
            result.M12 = matrix1.M11 * matrix2.M12 + matrix1.M12 * matrix2.M22 + matrix1.M13 * matrix2.M32 + matrix1.M14 * matrix2.M42;
            result.M13 = matrix1.M11 * matrix2.M13 + matrix1.M12 * matrix2.M23 + matrix1.M13 * matrix2.M33 + matrix1.M14 * matrix2.M43;
            result.M14 = matrix1.M11 * matrix2.M14 + matrix1.M12 * matrix2.M24 + matrix1.M13 * matrix2.M34 + matrix1.M14 * matrix2.M44;
            result.M21 = matrix1.M21 * matrix2.M11 + matrix1.M22 * matrix2.M21 + matrix1.M23 * matrix2.M31 + matrix1.M24 * matrix2.M41;
            result.M22 = matrix1.M21 * matrix2.M12 + matrix1.M22 * matrix2.M22 + matrix1.M23 * matrix2.M32 + matrix1.M24 * matrix2.M42;
            result.M23 = matrix1.M21 * matrix2.M13 + matrix1.M22 * matrix2.M23 + matrix1.M23 * matrix2.M33 + matrix1.M24 * matrix2.M43;
            result.M24 = matrix1.M21 * matrix2.M14 + matrix1.M22 * matrix2.M24 + matrix1.M23 * matrix2.M34 + matrix1.M24 * matrix2.M44;
            result.M31 = matrix1.M31 * matrix2.M11 + matrix1.M32 * matrix2.M21 + matrix1.M33 * matrix2.M31 + matrix1.M34 * matrix2.M41;
            result.M32 = matrix1.M31 * matrix2.M12 + matrix1.M32 * matrix2.M22 + matrix1.M33 * matrix2.M32 + matrix1.M34 * matrix2.M42;
            result.M33 = matrix1.M31 * matrix2.M13 + matrix1.M32 * matrix2.M23 + matrix1.M33 * matrix2.M33 + matrix1.M34 * matrix2.M43;
            result.M34 = matrix1.M31 * matrix2.M14 + matrix1.M32 * matrix2.M24 + matrix1.M33 * matrix2.M34 + matrix1.M34 * matrix2.M44;
            result.M41 = matrix1.M41 * matrix2.M11 + matrix1.M42 * matrix2.M21 + matrix1.M43 * matrix2.M31 + matrix1.M44 * matrix2.M41;
            result.M42 = matrix1.M41 * matrix2.M12 + matrix1.M42 * matrix2.M22 + matrix1.M43 * matrix2.M32 + matrix1.M44 * matrix2.M42;
            result.M43 = matrix1.M41 * matrix2.M13 + matrix1.M42 * matrix2.M23 + matrix1.M43 * matrix2.M33 + matrix1.M44 * matrix2.M43;
            result.M44 = matrix1.M41 * matrix2.M14 + matrix1.M42 * matrix2.M24 + matrix1.M43 * matrix2.M34 + matrix1.M44 * matrix2.M44;
            return result;
        }

        public static Matrix CreateTranslation(Vector3 position)
        {
            Matrix result;
            result.M11 = 1f;
            result.M12 = 0f;
            result.M13 = 0f;
            result.M14 = 0f;
            result.M21 = 0f;
            result.M22 = 1f;
            result.M23 = 0f;
            result.M24 = 0f;
            result.M31 = 0f;
            result.M32 = 0f;
            result.M33 = 1f;
            result.M34 = 0f;
            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;
            result.M44 = 1f;
            return result;
        }

    }
    public class GameTime
    {
        public GameTime() { }
    }
    public class GameWindow
    {
        public string Title
        {
            get
            { return Console.Title; }
            set
            {
                SetTitle(value);
            }
        }

        public static void SetTitle(string title)
        {
            Console.Title = title + " | TDSM Rebind build " + Globals.Build + Globals.PhaseToSuffix(Globals.BuildPhase);
        }

        public IntPtr Handle { get; set; }
        public bool AllowUserResizing { get; set; }
    }
    public struct Point
    {
        public static Point[] Array;

        public int X;
        public int Y;

        public Point(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }

    public static class MathHelper
    {
        public static float Lerp(float value1, float value2, float amount)
        {
            return value1 + (value2 - value1) * amount;
        }

        public static float SmoothStep(float value1, float value2, float amount)
        {
            float num = MathHelper.Clamp(amount, 0f, 1f);
            return MathHelper.Lerp(value1, value2, num * num * (3f - 2f * num));
        }

        public static float Clamp(float value, float min, float max)
        {
            value = ((value > max) ? max : value);
            value = ((value < min) ? min : value);
            return value;
        }
    }
}
