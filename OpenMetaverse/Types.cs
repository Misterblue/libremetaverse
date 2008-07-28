/*
 * Copyright (c) 2006-2008, openmetaverse.org
 * All rights reserved.
 *
 * - Redistribution and use in source and binary forms, with or without 
 *   modification, are permitted provided that the following conditions are met:
 *
 * - Redistributions of source code must retain the above copyright notice, this
 *   list of conditions and the following disclaimer.
 * - Neither the name of the openmetaverse.org nor the names 
 *   of its contributors may be used to endorse or promote products derived from
 *   this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
 * AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
 * ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
 * SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
 * INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
 * CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
 * ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
 * POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.Collections.Generic;
using OpenMetaverse.StructuredData;

namespace OpenMetaverse
{
    /// <summary>
    /// A 128-bit Universally Unique Identifier, used throughout the Second
    /// Life networking protocol
    /// </summary>
    [Serializable]
    public struct UUID : IComparable
    {
        /// <summary>The System.Guid object this struct wraps around</summary>
        public Guid Guid;

        #region Constructors

        /// <summary>
        /// Constructor that takes a string UUID representation
        /// </summary>
        /// <param name="val">A string representation of a UUID, case 
        /// insensitive and can either be hyphenated or non-hyphenated</param>
        /// <example>LLUUID("11f8aa9c-b071-4242-836b-13b7abe0d489")</example>
        public UUID(string val)
        {
            if (String.IsNullOrEmpty(val))
                Guid = new Guid();
            else
                Guid = new Guid(val);
        }

        /// <summary>
        /// Constructor that takes a System.Guid object
        /// </summary>
        /// <param name="val">A Guid object that contains the unique identifier
        /// to be represented by this LLUUID</param>
        public UUID(Guid val)
        {
            Guid = val;
        }

        /// <summary>
        /// Constructor that takes a byte array containing a UUID
        /// </summary>
        /// <param name="source">Byte array containing a 16 byte UUID</param>
        /// <param name="pos">Beginning offset in the array</param>
        public UUID(byte[] source, int pos)
        {
            Guid = UUID.Zero.Guid;
            FromBytes(source, pos);
        }

        /// <summary>
        /// Constructor that takes an unsigned 64-bit unsigned integer to 
        /// convert to a UUID
        /// </summary>
        /// <param name="val">64-bit unsigned integer to convert to a UUID</param>
        public UUID(ulong val)
        {
            Guid = new Guid(0, 0, 0, BitConverter.GetBytes(val));
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="val">UUID to copy</param>
        public UUID(UUID val)
        {
            Guid = val.Guid;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// IComparable.CompareTo implementation.
        /// </summary>
        public int CompareTo(object obj)
        {
            if (obj is UUID)
            {
                UUID ID = (UUID)obj;
                return this.Guid.CompareTo(ID.Guid);
            }

            throw new ArgumentException("object is not a LLUUID");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="pos"></param>
        public void FromBytes(byte[] source, int pos)
        {
            Guid = new Guid(
                (source[pos + 0] << 24) | (source[pos + 1] << 16) | (source[pos + 2] << 8) | source[pos + 3],
                (short)((source[pos + 4] << 8) | source[pos + 5]),
                (short)((source[pos + 6] << 8) | source[pos + 7]),
                source[pos + 8], source[pos + 9], source[pos + 10], source[pos + 11],
                source[pos + 12], source[pos + 13], source[pos + 14], source[pos + 15]);
        }

        /// <summary>
        /// Returns a copy of the raw bytes for this UUID
        /// </summary>
        /// <returns>A 16 byte array containing this UUID</returns>
        public byte[] GetBytes()
        {
            byte[] bytes = Guid.ToByteArray();
            byte[] output = new byte[16];
            output[0] = bytes[3];
            output[1] = bytes[2];
            output[2] = bytes[1];
            output[3] = bytes[0];
            output[4] = bytes[5];
            output[5] = bytes[4];
            output[6] = bytes[7];
            output[7] = bytes[6];
            Buffer.BlockCopy(bytes, 8, output, 8, 8);

            return output;
        }

		/// <summary>
		/// Calculate an LLCRC (cyclic redundancy check) for this LLUUID
		/// </summary>
		/// <returns>The CRC checksum for this LLUUID</returns>
		public uint CRC() 
		{
			uint retval = 0;
            byte[] bytes = GetBytes();

            retval += (uint)((bytes[ 3] << 24) + (bytes[ 2] << 16) + (bytes[ 1] << 8) + bytes[ 0]);
            retval += (uint)((bytes[ 7] << 24) + (bytes[ 6] << 16) + (bytes[ 5] << 8) + bytes[ 4]);
            retval += (uint)((bytes[11] << 24) + (bytes[10] << 16) + (bytes[ 9] << 8) + bytes[ 8]);
            retval += (uint)((bytes[15] << 24) + (bytes[14] << 16) + (bytes[13] << 8) + bytes[12]);

			return retval;
		}

        /// <summary>
        /// Create a 64-bit integer representation of the first half of this UUID
        /// </summary>
        /// <returns>An integer created from the first eight bytes of this UUID</returns>
        public ulong GetULong()
        {
            return Helpers.BytesToUInt64(Guid.ToByteArray());
        }

        #endregion Public Methods

        #region Static Methods

        /// <summary>
        /// Generate a LLUUID from a string
        /// </summary>
        /// <param name="val">A string representation of a UUID, case 
        /// insensitive and can either be hyphenated or non-hyphenated</param>
        /// <example>LLUUID.Parse("11f8aa9c-b071-4242-836b-13b7abe0d489")</example>
        public static UUID Parse(string val)
        {
            return new UUID(val);
        }

        /// <summary>
        /// Generate a LLUUID from a string
        /// </summary>
        /// <param name="val">A string representation of a UUID, case 
        /// insensitive and can either be hyphenated or non-hyphenated</param>
        /// <param name="result">Will contain the parsed UUID if successful,
        /// otherwise null</param>
        /// <returns>True if the string was successfully parse, otherwise false</returns>
        /// <example>LLUUID.TryParse("11f8aa9c-b071-4242-836b-13b7abe0d489", result)</example>
        public static bool TryParse(string val, out UUID result)
        {
            try
            {
                result = Parse(val);
                return true;
            }
            catch (Exception)
            {
                result = UUID.Zero;
                return false;
            }
        }

        /// <summary>
        /// Combine two UUIDs together by taking the MD5 hash of a byte array
        /// containing both UUIDs
        /// </summary>
        /// <param name="first">First LLUUID to combine</param>
        /// <param name="second">Second LLUUID to combine</param>
        /// <returns>The UUID product of the combination</returns>
        public static UUID Combine(UUID first, UUID second)
        {
            // Construct the buffer that MD5ed
            byte[] input = new byte[32];
            Buffer.BlockCopy(first.GetBytes(), 0, input, 0, 16);
            Buffer.BlockCopy(second.GetBytes(), 0, input, 16, 16);

            return new UUID(Helpers.MD5Builder.ComputeHash(input), 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static UUID Random()
		{
			return new UUID(Guid.NewGuid());
        }

        #endregion Static Methods

        #region Overrides

        /// <summary>
        /// Return a hash code for this UUID, used by .NET for hash tables
        /// </summary>
        /// <returns>An integer composed of all the UUID bytes XORed together</returns>
		public override int GetHashCode()
		{
            return Guid.GetHashCode();
		}

        /// <summary>
        /// Comparison function
        /// </summary>
        /// <param name="o">An object to compare to this UUID</param>
        /// <returns>False if the object is not an LLUUID, true if it is and
        /// byte for byte identical to this</returns>
		public override bool Equals(object o)
		{
			if (!(o is UUID)) return false;

			UUID uuid = (UUID)o;
            return Guid == uuid.Guid;
        }

        /// <summary>
        /// Get a hyphenated string representation of this UUID
        /// </summary>
        /// <returns>A string representation of this UUID, lowercase and 
        /// with hyphens</returns>
        /// <example>11f8aa9c-b071-4242-836b-13b7abe0d489</example>
        public override string ToString()
        {
            return Guid.ToString();
        }

        #endregion Overrides

        #region Operators

        /// <summary>
        /// Equals operator
        /// </summary>
        /// <param name="lhs">First LLUUID for comparison</param>
        /// <param name="rhs">Second LLUUID for comparison</param>
        /// <returns>True if the UUIDs are byte for byte equal, otherwise false</returns>
		public static bool operator==(UUID lhs, UUID rhs)
		{
            return lhs.Guid == rhs.Guid;
		}

        /// <summary>
        /// Not equals operator
        /// </summary>
        /// <param name="lhs">First LLUUID for comparison</param>
        /// <param name="rhs">Second LLUUID for comparison</param>
        /// <returns>True if the UUIDs are not equal, otherwise true</returns>
		public static bool operator!=(UUID lhs, UUID rhs)
		{
			return !(lhs == rhs);
		}

        /// <summary>
        /// XOR operator
        /// </summary>
        /// <param name="lhs">First LLUUID</param>
        /// <param name="rhs">Second LLUUID</param>
        /// <returns>A UUID that is a XOR combination of the two input UUIDs</returns>
        public static UUID operator ^(UUID lhs, UUID rhs)
        {
            byte[] lhsbytes = lhs.GetBytes();
            byte[] rhsbytes = rhs.GetBytes();
            byte[] output = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                output[i] = (byte)(lhsbytes[i] ^ rhsbytes[i]);
            }

            return new UUID(output, 0);
        }

        /// <summary>
        /// String typecasting operator
        /// </summary>
        /// <param name="val">A UUID in string form. Case insensitive, 
        /// hyphenated or non-hyphenated</param>
        /// <returns>A UUID built from the string representation</returns>
        public static implicit operator UUID(string val)
		{
			return new UUID(val);
        }

        #endregion Operators

        /// <summary>An LLUUID with a value of all zeroes</summary>
        public static readonly UUID Zero = new UUID();
	}

    /// <summary>
    /// A two-dimensional vector with floating-point values
    /// </summary>
    [Serializable]
    public struct Vector2
    {
        /// <summary>X value</summary>
        public float X;
        /// <summary>Y value</summary>
        public float Y;

        // Used for little to big endian conversion on big endian architectures
        private byte[] conversionBuffer;

        #region Public Methods

        /// <summary>
        /// Builds a vector from a byte array
        /// </summary>
        /// <param name="byteArray">Byte array containing two four-byte floats</param>
        /// <param name="pos">Beginning position in the byte array</param>
        public void FromBytes(byte[] byteArray, int pos)
        {
            if (!BitConverter.IsLittleEndian)
            {
                // Big endian architecture
                if (conversionBuffer == null)
                    conversionBuffer = new byte[8];

                Buffer.BlockCopy(byteArray, pos, conversionBuffer, 0, 8);

                Array.Reverse(conversionBuffer, 0, 4);
                Array.Reverse(conversionBuffer, 4, 4);

                X = BitConverter.ToSingle(conversionBuffer, 0);
                Y = BitConverter.ToSingle(conversionBuffer, 4);
            }
            else
            {
                // Little endian architecture
                X = BitConverter.ToSingle(byteArray, pos);
                Y = BitConverter.ToSingle(byteArray, pos + 4);
            }
        }

        /// <summary>
        /// Returns the raw bytes for this vector
        /// </summary>
        /// <returns>An eight-byte array containing X and Y</returns>
        public byte[] GetBytes()
        {
            byte[] byteArray = new byte[8];

            Buffer.BlockCopy(BitConverter.GetBytes(X), 0, byteArray, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Y), 0, byteArray, 4, 4);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteArray, 0, 4);
                Array.Reverse(byteArray, 4, 4);
            }

            return byteArray;
        }

        #endregion Public Methods

        #region Constructors

        /// <summary>
        /// Constructor, builds a vector for individual float values
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
		public Vector2(float x, float y)
		{
            conversionBuffer = null;
			X = x;
			Y = y;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="vector">Vector to copy</param>
        public Vector2(Vector2 vector)
        {
            conversionBuffer = null;
            X = vector.X;
            Y = vector.Y;
        }

        #endregion Constructors

        #region Overrides

        /// <summary>
        /// A hash of the vector, used by .NET for hash tables
        /// </summary>
        /// <returns>The hashes of the individual components XORed together</returns>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            if (!(o is Vector2)) return false;

            Vector2 vector = (Vector2)o;

            return (X == vector.X && Y == vector.Y);
        }

        /// <summary>
        /// Get a formatted string representation of the vector
        /// </summary>
        /// <returns>A string representation of the vector</returns>
        public override string ToString()
        {
            return String.Format(Helpers.EnUsCulture, "<{0}, {1}>", X, Y);
        }

        #endregion Overrides

        #region Operators

        public static bool operator ==(Vector2 lhs, Vector2 rhs)
        {
            return (lhs.X == rhs.X && lhs.Y == rhs.Y);
        }

        public static bool operator !=(Vector2 lhs, Vector2 rhs)
        {
            return !(lhs == rhs);
        }

        public static Vector2 operator +(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.X + rhs.X, lhs.Y + rhs.Y);
        }

        public static Vector2 operator -(Vector2 vec)
        {
            return new Vector2(-vec.X, -vec.Y);
        }

        public static Vector2 operator -(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.X - rhs.X, lhs.Y - rhs.Y);
        }

        public static Vector2 operator *(Vector2 vec, float val)
        {
            return new Vector2(vec.X * val, vec.Y * val);
        }

        public static Vector2 operator *(float val, Vector2 vec)
        {
            return new Vector2(vec.X * val, vec.Y * val);
        }

        public static Vector2 operator *(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.X * rhs.X, lhs.Y * rhs.Y);
        }

        public static Vector2 operator /(Vector2 lhs, Vector2 rhs)
        {
            return new Vector2(lhs.X / rhs.X, lhs.Y / rhs.Y);
        }

        public static Vector2 operator /(Vector2 vec, float val)
        {
            return new Vector2(vec.X / val, vec.Y / val);
        }

        #endregion Operators

        /// <summary>An LLVector2 with a value of 0,0,0</summary>
        public readonly static Vector2 Zero = new Vector2();
    }

    /// <summary>
    /// A three-dimensional vector with floating-point values
    /// </summary>
    [Serializable]
	public struct Vector3
	{
        /// <summary>X value</summary>
        public float X;
		/// <summary>Y value</summary>
        public float Y;
        /// <summary>Z value</summary>
        public float Z;

        // Used for little to big endian conversion on big endian architectures
        private byte[] conversionBuffer;

        #region Constructors

        /// <summary>
        /// Constructor, builds a vector from a byte array
        /// </summary>
        /// <param name="byteArray">Byte array containing three four-byte floats</param>
        /// <param name="pos">Beginning position in the byte array</param>
		public Vector3(byte[] byteArray, int pos)
		{
            conversionBuffer = null;
            X = Y = Z = 0;
            FromBytes(byteArray, pos);
        }

        /// <summary>
        /// Constructor, builds a vector for individual float values
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="z">Z value</param>
		public Vector3(float x, float y, float z)
		{
            conversionBuffer = null;
			X = x;
			Y = y;
			Z = z;
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="vector">Vector to copy</param>
        public Vector3(Vector3 vector)
        {
            conversionBuffer = null;
            X = (float)vector.X;
            Y = (float)vector.Y;
            Z = (float)vector.Z;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Test if this vector is composed of all finite numbers
        /// </summary>
        public bool IsFinite()
        {
            if (Helpers.IsFinite(X) && Helpers.IsFinite(Y) && Helpers.IsFinite(Z))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Builds a vector from a byte array
        /// </summary>
        /// <param name="byteArray">Byte array containing a 12 byte vector</param>
        /// <param name="pos">Beginning position in the byte array</param>
        public void FromBytes(byte[] byteArray, int pos)
        {
            if (!BitConverter.IsLittleEndian)
            {
                // Big endian architecture
                if (conversionBuffer == null)
                    conversionBuffer = new byte[12];

                Buffer.BlockCopy(byteArray, pos, conversionBuffer, 0, 12);

                Array.Reverse(conversionBuffer, 0, 4);
                Array.Reverse(conversionBuffer, 4, 4);
                Array.Reverse(conversionBuffer, 8, 4);

                X = BitConverter.ToSingle(conversionBuffer, 0);
                Y = BitConverter.ToSingle(conversionBuffer, 4);
                Z = BitConverter.ToSingle(conversionBuffer, 8);
            }
            else
            {
                // Little endian architecture
                X = BitConverter.ToSingle(byteArray, pos);
                Y = BitConverter.ToSingle(byteArray, pos + 4);
                Z = BitConverter.ToSingle(byteArray, pos + 8);
            }
        }

        /// <summary>
        /// Returns the raw bytes for this vector
        /// </summary>
        /// <returns>A 12 byte array containing X, Y, and Z</returns>
		public byte[] GetBytes()
		{
			byte[] byteArray = new byte[12];

            Buffer.BlockCopy(BitConverter.GetBytes(X), 0, byteArray, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Y), 0, byteArray, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Z), 0, byteArray, 8, 4);

			if(!BitConverter.IsLittleEndian) {
				Array.Reverse(byteArray, 0, 4);
				Array.Reverse(byteArray, 4, 4);
				Array.Reverse(byteArray, 8, 4);
			}

			return byteArray;
        }

        public LLSD ToLLSD()
        {
            LLSDArray array = new LLSDArray();
            array.Add(LLSD.FromReal(X));
            array.Add(LLSD.FromReal(Y));
            array.Add(LLSD.FromReal(Z));
            return array;
        }

        public void FromLLSD(LLSD llsd)
        {
            if (llsd.Type == LLSDType.Array)
            {
                LLSDArray array = (LLSDArray)llsd;

                if (array.Count == 3)
                {
                    X = (float)array[0].AsReal();
                    Y = (float)array[1].AsReal();
                    Z = (float)array[2].AsReal();

                    return;
                }
            }

            this = Vector3.Zero;
        }

        //FIXME: Need comprehensive testing for this
        // <summary>
        // Assumes this vector represents euler rotations along the X, Y, and
        // Z axis and creates a quaternion representation of the rotations
        // </summary>
        // <returns>A quaternion representation of the euler rotations</returns>
        //public LLQuaternion ToQuaternion()
        //{
        //    LLMatrix3 rotMat = new LLMatrix3(X, Y, Z);
        //    rotMat = LLMatrix3.Orthogonalize(rotMat);

        //    LLQuaternion quat = rotMat.ToQuaternion();
        //    quat = LLQuaternion.Norm(quat);

        //    return quat;
        //}

        #endregion Public Methods

        #region Static Methods

        /// <summary>
        /// Calculate the magnitude of the supplied vector
        /// </summary>
        public static float Mag(Vector3 v)
        {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }

        /// <summary>
        /// Calculate the squared magnitude of the supplied vector
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public static float MagSquared(Vector3 v)
        {
            return v.X * v.X + v.Y * v.Y + v.Z * v.Z;
        }

        /// <summary>
        /// Returns a normalized version of the supplied vector
        /// </summary>
        /// <param name="vector">The vector to normalize</param>
        /// <returns>A normalized version of the vector</returns>
        public static Vector3 Norm(Vector3 vector)
        {
            float mag = Mag(vector);
            return new Vector3(vector.X / mag, vector.Y / mag, vector.Z / mag);
        }

        /// <summary>
        /// Return the cross product of two vectors
        /// </summary>
        /// <param name="v1">First vector</param>
        /// <param name="v2">Second vector</param>
        /// <returns>Cross product of first and second vector</returns>
        public static Vector3 Cross(Vector3 v1, Vector3 v2)
        {
            return new Vector3
            (
                v1.Y * v2.Z - v1.Z * v2.Y,
                v1.Z * v2.X - v1.X * v2.Z,
                v1.X * v2.Y - v1.Y * v2.X
            );
        }

        /// <summary>
        /// Returns the dot product of two vectors
        /// </summary>
        public static float Dot(Vector3 v1, Vector3 v2)
        {
            return (v1.X * v2.X) + (v1.Y * v2.Y) + (v1.Z * v2.Z);
        }

        /// <summary>
        /// Calculates the distance between two vectors
        /// </summary>
        public static float Dist(Vector3 pointA, Vector3 pointB)
        {
            float xd = pointB.X - pointA.X;
            float yd = pointB.Y - pointA.Y;
            float zd = pointB.Z - pointA.Z;
            return (float)Math.Sqrt(xd * xd + yd * yd + zd * zd);
        }

        public static Vector3 Rot(Vector3 vector, Quaternion rotation)
        {
            return vector * rotation;
        }

        public static Vector3 Rot(Vector3 vector, Matrix3 rotation)
        {
            return vector * rotation;
        }

        /// <summary>
        /// Calculate the rotation between two vectors
        /// </summary>
        /// <param name="a">Directional vector, such as 1,0,0 for the forward face</param>
        /// <param name="b">Target vector - normalize first with VecNorm</param>
        public static Quaternion RotBetween(Vector3 a, Vector3 b)
        {
            //A and B should both be normalized

            float dotProduct = Dot(a, b);
            Vector3 crossProduct = Cross(a, b);
            float magProduct = Mag(a) * Mag(b);
            double angle = Math.Acos(dotProduct / magProduct);
            Vector3 axis = Norm(crossProduct);
            float s = (float)Math.Sin(angle / 2);

            return new Quaternion(
                axis.X * s,
                axis.Y * s,
                axis.Z * s,
                (float)Math.Cos(angle / 2));
        }

        public static Vector3 Transform(Vector3 vector, Matrix3 matrix)
        {
            // Operates "from the right" on row vector
            return new Vector3(
                vector.X * matrix.M11 + vector.Y * matrix.M21 + vector.Z * matrix.M31,
                vector.X * matrix.M12 + vector.Y * matrix.M22 + vector.Z * matrix.M32,
                vector.X * matrix.M13 + vector.Y * matrix.M23 + vector.Z * matrix.M33
            );
        }

        /// <summary>
        /// Generate an LLVector3 from a string
        /// </summary>
        /// <param name="val">A string representation of a 3D vector, enclosed 
        /// in arrow brackets and separated by commas</param>
        public static Vector3 Parse(string val)
        {
            char[] splitChar = { ',' };
            string[] split = val.Replace("<", String.Empty).Replace(">", String.Empty).Split(splitChar);
            return new Vector3(
                float.Parse(split[0].Trim(), Helpers.EnUsCulture),
                float.Parse(split[1].Trim(), Helpers.EnUsCulture),
                float.Parse(split[2].Trim(), Helpers.EnUsCulture));
        }

        public static bool TryParse(string val, out Vector3 result)
        {
            try
            {
                result = Parse(val);
                return true;
            }
            catch (Exception)
            {
                result = new Vector3();
                return false;
            }
        }

        #endregion Static Methods

        #region Overrides

        /// <summary>
        /// A hash of the vector, used by .NET for hash tables
        /// </summary>
        /// <returns>The hashes of the individual components XORed together</returns>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
		public override bool Equals(object o)
		{
			if (!(o is Vector3)) return false;

			Vector3 vector = (Vector3)o;

			return (X == vector.X && Y == vector.Y && Z == vector.Z);
        }

        /// <summary>
        /// Get a formatted string representation of the vector
        /// </summary>
        /// <returns>A string representation of the vector</returns>
        public override string ToString()
        {
            return String.Format(Helpers.EnUsCulture, "<{0}, {1}, {2}>", X, Y, Z);
        }

        #endregion Overrides

        #region Operators

		public static bool operator==(Vector3 lhs, Vector3 rhs)
		{
			return (lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z);
		}

		public static bool operator!=(Vector3 lhs, Vector3 rhs)
		{
            return !(lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z);
		}

        public static Vector3 operator +(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        public static Vector3 operator -(Vector3 vec)
        {
            return new Vector3(-vec.X, -vec.Y, -vec.Z);
        }

        public static Vector3 operator -(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.X - rhs.X,lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public static Vector3 operator *(Vector3 vec, float val)
        {
            return new Vector3(vec.X * val, vec.Y * val, vec.Z * val);
        }

        public static Vector3 operator *(float val, Vector3 vec)
        {
            return new Vector3(vec.X * val, vec.Y * val, vec.Z * val);
        }

        public static Vector3 operator *(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z);
        }

        public static Vector3 operator *(Vector3 vec, Quaternion rot)
        {
            float rw = -rot.X * vec.X - rot.Y * vec.Y - rot.Z * vec.Z;
            float rx =  rot.W * vec.X + rot.Y * vec.Z - rot.Z * vec.Y;
            float ry =  rot.W * vec.Y + rot.Z * vec.X - rot.X * vec.Z;
            float rz =  rot.W * vec.Z + rot.X * vec.Y - rot.Y * vec.X;

            float nx = -rw * rot.X + rx * rot.W - ry * rot.Z + rz * rot.Y;
            float ny = -rw * rot.Y + ry * rot.W - rz * rot.X + rx * rot.Z;
            float nz = -rw * rot.Z + rz * rot.W - rx * rot.Y + ry * rot.X;

            return new Vector3(nx, ny, nz);
        }

        public static Vector3 operator *(Vector3 vector, Matrix3 matrix)
        {
            return Vector3.Transform(vector, matrix);
        }

        public static Vector3 operator /(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(lhs.X / rhs.X, lhs.Y / rhs.Y, lhs.Z / rhs.Z);
        }

        public static Vector3 operator /(Vector3 vec, float val)
        {
            return new Vector3(vec.X / val, vec.Y / val, vec.Z / val);
        }

        public static Vector3 operator %(Vector3 lhs, Vector3 rhs)
        {
            return new Vector3(
                lhs.Y * rhs.Z - rhs.Y * lhs.Z,
                lhs.Z * rhs.X - rhs.Z * lhs.X,
                lhs.X * rhs.Y - rhs.X * lhs.Y);
        }

        #endregion Operators

        /// <summary>An LLVector3 with a value of 0,0,0</summary>
        public readonly static Vector3 Zero = new Vector3();
        /// <summary>A unit vector facing up (Z axis)</summary>
        public readonly static Vector3 Up = new Vector3(0f, 0f, 1f);
        /// <summary>A unit vector facing forward (X axis)</summary>
        public readonly static Vector3 Fwd = new Vector3(1f, 0f, 0f);
        /// <summary>A unit vector facing left (Y axis)</summary>
        public readonly static Vector3 Left = new Vector3(0f, 1f, 0f);
	}

    /// <summary>
    /// A double-precision three-dimensional vector
    /// </summary>
	[Serializable]
    public struct Vector3d
	{
        /// <summary>X value</summary>
        public double X;
        /// <summary>Y value</summary>
        public double Y;
        /// <summary>Z value</summary>
        public double Z;

        // Used for little to big endian conversion on big endian architectures
        private byte[] conversionBuffer;

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
		public Vector3d(double x, double y, double z)
		{
            conversionBuffer = null;
			X = x;
			Y = y;
			Z = z;
		}

        /// <summary>
        /// Create a double precision vector from a float vector
        /// </summary>
        /// <param name="llv3"></param>
        public Vector3d(Vector3 llv3)
        {
            conversionBuffer = null;
            X = llv3.X;
            Y = llv3.Y;
            Z = llv3.Z;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="pos"></param>
		public Vector3d(byte[] byteArray, int pos)
		{
            conversionBuffer = null;
            X = Y = Z = 0;
            FromBytes(byteArray, pos);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="vector">Vector to copy</param>
        public Vector3d(Vector3d vector)
        {
            conversionBuffer = null;
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="pos"></param>
        public void FromBytes(byte[] byteArray, int pos)
        {
            if (!BitConverter.IsLittleEndian)
            {
                // Big endian architecture
                if (conversionBuffer == null)
                    conversionBuffer = new byte[24];

                Buffer.BlockCopy(byteArray, pos, conversionBuffer, 0, 24);

                Array.Reverse(conversionBuffer, 0, 8);
                Array.Reverse(conversionBuffer, 8, 8);
                Array.Reverse(conversionBuffer, 16, 8);

                X = BitConverter.ToDouble(conversionBuffer, 0);
                Y = BitConverter.ToDouble(conversionBuffer, 8);
                Z = BitConverter.ToDouble(conversionBuffer, 16);
            }
            else
            {
                // Little endian architecture
                X = BitConverter.ToDouble(byteArray, pos);
                Y = BitConverter.ToDouble(byteArray, pos + 8);
                Z = BitConverter.ToDouble(byteArray, pos + 16);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            byte[] byteArray = new byte[24];

            Buffer.BlockCopy(BitConverter.GetBytes(X), 0, byteArray, 0, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(Y), 0, byteArray, 8, 8);
            Buffer.BlockCopy(BitConverter.GetBytes(Z), 0, byteArray, 16, 8);

            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(byteArray, 0, 8);
                Array.Reverse(byteArray, 8, 8);
                Array.Reverse(byteArray, 16, 8);
            }

            return byteArray;
        }

        public LLSD ToLLSD()
        {
            LLSDArray array = new LLSDArray();
            array.Add(LLSD.FromReal(X));
            array.Add(LLSD.FromReal(Y));
            array.Add(LLSD.FromReal(Z));
            return array;
        }

        public void FromLLSD(LLSD llsd)
        {
            if (llsd.Type == LLSDType.Array)
            {
                LLSDArray array = (LLSDArray)llsd;

                if (array.Count == 3)
                {
                    X = array[0].AsReal();
                    Y = array[1].AsReal();
                    Z = array[2].AsReal();

                    return;
                }
            }

            this = Vector3d.Zero;
        }

        #endregion Public Methods

        #region Static Methods

        /// <summary>
        /// Calculates the distance between two vectors
        /// </summary>
        public static double Dist(Vector3d pointA, Vector3d pointB)
        {
            double xd = pointB.X - pointA.X;
            double yd = pointB.Y - pointA.Y;
            double zd = pointB.Z - pointA.Z;
            return Math.Sqrt(xd * xd + yd * yd + zd * zd);
        }

        /// <summary>
        /// Generate an LLVector3d from a string
        /// </summary>
        /// <param name="val">A string representation of a 3D vector, enclosed 
        /// in arrow brackets and separated by commas</param>
        public static Vector3d Parse(string val)
        {
            char[] splitChar = { ',' };
            string[] split = val.Replace("<", String.Empty).Replace(">", String.Empty).Split(splitChar);
            return new Vector3d(
                double.Parse(split[0].Trim(), Helpers.EnUsCulture),
                double.Parse(split[1].Trim(), Helpers.EnUsCulture),
                double.Parse(split[2].Trim(), Helpers.EnUsCulture));
        }

        public static bool TryParse(string val, out Vector3d result)
        {
            try
            {
                result = Parse(val);
                return true;
            }
            catch (Exception)
            {
                result = new Vector3d();
                return false;
            }
        }

        #endregion Static Methods

        #region Overrides

        /// <summary>
        /// A hash of the vector, used by .NET for hash tables
        /// </summary>
        /// <returns>The hashes of the individual components XORed together</returns>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            if (!(o is Vector3d)) return false;

            Vector3d vector = (Vector3d)o;

            return (X == vector.X && Y == vector.Y && Z == vector.Z);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("<{0}, {1}, {2}>", X, Y, Z);
        }

        #endregion Overrides

        #region Operators

        public static bool operator ==(Vector3d lhs, Vector3d rhs)
        {
            return (lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z);
        }

        public static bool operator !=(Vector3d lhs, Vector3d rhs)
        {
            return !(lhs == rhs);
        }

        public static Vector3d operator +(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z);
        }

        public static Vector3d operator -(Vector3d vec)
        {
            return new Vector3d(-vec.X, -vec.Y, -vec.Z);
        }

        public static Vector3d operator -(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z);
        }

        public static Vector3d operator *(Vector3d vec, float val)
        {
            return new Vector3d(vec.X * val, vec.Y * val, vec.Z * val);
        }

        public static Vector3d operator *(double val, Vector3d vec)
        {
            return new Vector3d(vec.X * val, vec.Y * val, vec.Z * val);
        }

        public static Vector3d operator *(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z);
        }

        public static Vector3d operator /(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(lhs.X / rhs.X, lhs.Y / rhs.Y, lhs.Z / rhs.Z);
        }

        public static Vector3d operator /(Vector3d vec, double val)
        {
            return new Vector3d(vec.X / val, vec.Y / val, vec.Z / val);
        }

        public static Vector3d operator %(Vector3d lhs, Vector3d rhs)
        {
            return new Vector3d(
                lhs.Y * rhs.Z - rhs.Y * lhs.Z,
                lhs.Z * rhs.X - rhs.Z * lhs.X,
                lhs.X * rhs.Y - rhs.X * lhs.Y);
        }

        #endregion Operators

        /// <summary>An LLVector3d with a value of 0,0,0</summary>
        public static readonly Vector3d Zero = new Vector3d();
	}

    /// <summary>
    /// A four-dimensional vector
    /// </summary>
	[Serializable]
    public struct Vector4
	{
        /// <summary></summary>
        public float X;
        /// <summary></summary>
        public float Y;
        /// <summary></summary>
        public float Z;
        /// <summary></summary>
        public float S;

        // Used for little to big endian conversion on big endian architectures
        private byte[] conversionBuffer;

        #region Constructors

        /// <summary>
        /// Constructor, sets the vector members according to parameters
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="z">Z value</param>
        /// <param name="s">S value</param>
        public Vector4(float x, float y, float z, float s)
        {
            conversionBuffer = null;
            X = x;
            Y = y;
            Z = z;
            S = s;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="pos"></param>
		public Vector4(byte[] byteArray, int pos)
		{
            conversionBuffer = null;
            X = Y = Z = S = 0;
            FromBytes(byteArray, pos);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="vector">Vector to copy</param>
        public Vector4(Vector4 vector)
        {
            conversionBuffer = null;
            X = vector.X;
            Y = vector.Y;
            Z = vector.Z;
            S = vector.S;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="pos"></param>
        public void FromBytes(byte[] byteArray, int pos)
        {
            if (!BitConverter.IsLittleEndian)
            {
                // Big endian architecture
                if (conversionBuffer == null)
                    conversionBuffer = new byte[16];

                Buffer.BlockCopy(byteArray, pos, conversionBuffer, 0, 16);

                Array.Reverse(conversionBuffer, 0, 4);
                Array.Reverse(conversionBuffer, 4, 4);
                Array.Reverse(conversionBuffer, 8, 4);
                Array.Reverse(conversionBuffer, 12, 4);

                X = BitConverter.ToSingle(conversionBuffer, 0);
                Y = BitConverter.ToSingle(conversionBuffer, 4);
                Z = BitConverter.ToSingle(conversionBuffer, 8);
                S = BitConverter.ToSingle(conversionBuffer, 12);
            }
            else
            {
                // Little endian architecture
                X = BitConverter.ToSingle(byteArray, pos);
                Y = BitConverter.ToSingle(byteArray, pos + 4);
                Z = BitConverter.ToSingle(byteArray, pos + 8);
                S = BitConverter.ToSingle(byteArray, pos + 12);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public byte[] GetBytes()
		{
			byte[] byteArray = new byte[16];

            Buffer.BlockCopy(BitConverter.GetBytes(X), 0, byteArray, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Y), 0, byteArray, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(Z), 0, byteArray, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(S), 0, byteArray, 12, 4);

			if(!BitConverter.IsLittleEndian)
            {
				Array.Reverse(byteArray, 0, 4);
				Array.Reverse(byteArray, 4, 4);
				Array.Reverse(byteArray, 8, 4);
				Array.Reverse(byteArray, 12, 4);
			}

			return byteArray;
        }

        public LLSD ToLLSD()
        {
            LLSDArray array = new LLSDArray();
            array.Add(LLSD.FromReal(X));
            array.Add(LLSD.FromReal(Y));
            array.Add(LLSD.FromReal(Z));
            array.Add(LLSD.FromReal(S));
            return array;
        }

        public void FromLLSD(LLSD llsd)
        {
            if (llsd.Type == LLSDType.Array)
            {
                LLSDArray array = (LLSDArray)llsd;

                if (array.Count == 4)
                {
                    X = (float)array[0].AsReal();
                    Y = (float)array[1].AsReal();
                    Z = (float)array[2].AsReal();
                    S = (float)array[3].AsReal();

                    return;
                }
            }

            this = Vector4.Zero;
        }

        #endregion Public Methods

        #region Static Methods

        public static Vector4 Parse(string val)
        {
            char[] splitChar = { ',' };
            string[] split = val.Replace("<", String.Empty).Replace(">", String.Empty).Split(splitChar);
            return new Vector4(
                float.Parse(split[0].Trim(), Helpers.EnUsCulture),
                float.Parse(split[1].Trim(), Helpers.EnUsCulture),
                float.Parse(split[2].Trim(), Helpers.EnUsCulture),
                float.Parse(split[3].Trim(), Helpers.EnUsCulture));
        }

        public static bool TryParse(string val, out Vector4 result)
        {
            try
            {
                result = Parse(val);
                return true;
            }
            catch (Exception)
            {
                result = new Vector4();
                return false;
            }
        }

        #endregion Static Methods

        #region Overrides

        /// <summary>
        /// A hash of the vector, used by .NET for hash tables
        /// </summary>
        /// <returns>The hashes of the individual components XORed together</returns>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ S.GetHashCode());
        }
         /// <summary>
         /// 
         /// </summary>
         /// <param name="o"></param>
         /// <returns></returns>
        public override bool Equals(object o)
        {
            if (!(o is Vector4)) return false;

            Vector4 vector = (Vector4)o;
            return (X == vector.X && Y == vector.Y && Z == vector.Z && S == vector.S);
        }
        /// <summary>        
        /// 
        /// </summary>
        /// <returns></returns>
		public override string ToString()
		{
            return String.Format("<{0}, {1}, {2}, {3}>", X, Y, Z, S);
        }

        #endregion Overrides

        #region Operators

        public static bool operator ==(Vector4 lhs, Vector4 rhs)
        {
            return (lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z && lhs.S == rhs.S);
        }

        public static bool operator !=(Vector4 lhs, Vector4 rhs)
        {
            return !(lhs == rhs);
        }

        public static Vector4 operator +(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(lhs.X + rhs.X, lhs.Y + rhs.Y, lhs.Z + rhs.Z, lhs.S + rhs.S);
        }

        public static Vector4 operator -(Vector4 vec)
        {
            return new Vector4(-vec.X, -vec.Y, -vec.Z, -vec.S);
        }

        public static Vector4 operator -(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(lhs.X - rhs.X, lhs.Y - rhs.Y, lhs.Z - rhs.Z, lhs.S - rhs.S);
        }

        public static Vector4 operator *(Vector4 vec, float val)
        {
            return new Vector4(vec.X * val, vec.Y * val, vec.Z * val, vec.S * val);
        }

        public static Vector4 operator *(float val, Vector4 vec)
        {
            return new Vector4(vec.X * val, vec.Y * val, vec.Z * val, vec.S * val);
        }

        public static Vector4 operator *(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(lhs.X * rhs.X, lhs.Y * rhs.Y, lhs.Z * rhs.Z, lhs.S * rhs.S);
        }

        public static Vector4 operator /(Vector4 lhs, Vector4 rhs)
        {
            return new Vector4(lhs.X / rhs.X, lhs.Y / rhs.Y, lhs.Z / rhs.Z, lhs.S / rhs.S);
        }

        public static Vector4 operator /(Vector4 vec, float val)
        {
            return new Vector4(vec.X / val, vec.Y / val, vec.Z / val, vec.S / val);
        }

        #endregion Operators

        /// <summary>An LLVector4 with a value of 0,0,0,0</summary>
        public readonly static Vector4 Zero = new Vector4();
	}

    /// <summary>
    /// An 8-bit color structure including an alpha channel
    /// </summary>
    [Serializable]
    public struct Color4
    {
        /// <summary>Red</summary>
        public float R;
        /// <summary>Green</summary>
        public float G;
        /// <summary>Blue</summary>
        public float B;
        /// <summary>Alpha</summary>
        public float A;

        #region Constructors

        /// <summary>
        /// 
        /// </summary>
        /// <param name="r"></param>
        /// <param name="g"></param>
        /// <param name="b"></param>
        /// <param name="a"></param>
        public Color4(byte r, byte g, byte b, byte a)
        {
            const float quanta = 1.0f / 255.0f;

            R = (float)r * quanta;
            G = (float)g * quanta;
            B = (float)b * quanta;
            A = (float)a * quanta;
        }

        public Color4(float r, float g, float b, float a)
        {
            if (r > 1f || g > 1f || b > 1f || a > 1f)
                Logger.Log(
                    String.Format("Attempting to initialize LLColor with out of range values <{0},{1},{2},{3}>",
                    r, g, b, a), Helpers.LogLevel.Warning);

            // Valid range is from 0.0 to 1.0
            R = Helpers.Clamp(r, 0f, 1f);
            G = Helpers.Clamp(g, 0f, 1f);
            B = Helpers.Clamp(b, 0f, 1f);
            A = Helpers.Clamp(a, 0f, 1f);
        }

        public Color4(byte[] byteArray, int pos, bool inverted)
        {
            R = G = B = A = 0f;
            FromBytes(byteArray, pos, inverted);
        }

        public Color4(byte[] byteArray, int pos, bool inverted, bool alphaInverted)
        {
            R = G = B = A = 0f;
            FromBytes(byteArray, pos, inverted, alphaInverted);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="color">Color to copy</param>
        public Color4(Color4 color)
        {
            R = color.R;
            G = color.G;
            B = color.B;
            A = color.A;
        }

        #endregion Constructors

        #region Public Methods

        public void FromBytes(byte[] byteArray, int pos, bool inverted)
        {
            const float quanta = 1.0f / 255.0f;

            if (inverted)
            {
                R = (float)(255 - byteArray[pos]) * quanta;
                G = (float)(255 - byteArray[pos + 1]) * quanta;
                B = (float)(255 - byteArray[pos + 2]) * quanta;
                A = (float)(255 - byteArray[pos + 3]) * quanta;
            }
            else
            {
                R = (float)byteArray[pos] * quanta;
                G = (float)byteArray[pos + 1] * quanta;
                B = (float)byteArray[pos + 2] * quanta;
                A = (float)byteArray[pos + 3] * quanta;
            }
        }

        public void FromBytes(byte[] byteArray, int pos, bool inverted, bool alphaInverted)
        {
            FromBytes(byteArray, pos, inverted);

            if (alphaInverted)
                A = 1.0f - A;
        }

        public byte[] GetBytes()
        {
            return GetBytes(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes(bool inverted)
        {
            byte[] byteArray = new byte[4];

            byteArray[0] = Helpers.FloatToByte(R, 0f, 1f);
            byteArray[1] = Helpers.FloatToByte(G, 0f, 1f);
            byteArray[2] = Helpers.FloatToByte(B, 0f, 1f);
            byteArray[3] = Helpers.FloatToByte(A, 0f, 1f);

            if (inverted)
            {
                byteArray[0] = (byte)(255 - byteArray[0]);
                byteArray[1] = (byte)(255 - byteArray[1]);
                byteArray[2] = (byte)(255 - byteArray[2]);
                byteArray[3] = (byte)(255 - byteArray[3]);
            }

            return byteArray;
        }

        public byte[] GetFloatBytes()
        {
            byte[] bytes = new byte[16];
            Buffer.BlockCopy(BitConverter.GetBytes(R), 0, bytes, 0, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(G), 0, bytes, 4, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(B), 0, bytes, 8, 4);
            Buffer.BlockCopy(BitConverter.GetBytes(A), 0, bytes, 12, 4);
            return bytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public LLSD ToLLSD()
        {
            LLSDArray array = new LLSDArray();
            array.Add(LLSD.FromReal(R));
            array.Add(LLSD.FromReal(G));
            array.Add(LLSD.FromReal(B));
            array.Add(LLSD.FromReal(A));
            return array;
        }

        public void FromLLSD(LLSD llsd)
        {
            if (llsd.Type == LLSDType.Array)
            {
                LLSDArray array = (LLSDArray)llsd;

                if (array.Count == 4)
                {
                    R = (float)array[0].AsReal();
                    G = (float)array[1].AsReal();
                    B = (float)array[2].AsReal();
                    A = (float)array[3].AsReal();

                    return;
                }
            }

            this = Color4.Black;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string ToStringRGB()
        {
            return String.Format("<{0}, {1}, {2}>", R, G, B);
        }

        #endregion Public Methods

        #region Static Methods

        #endregion Static Methods

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("<{0}, {1}, {2}, {3}>", R, G, B, A);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is Color4)
            {
                Color4 c = (Color4)obj;
                return (R == c.R) && (G == c.G) && (B == c.B) && (A == c.A);
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return R.GetHashCode() ^ G.GetHashCode() ^ B.GetHashCode() ^ A.GetHashCode();
        }

        #endregion Overrides

        #region Operators

        /// <summary>
        /// Comparison operator
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator ==(Color4 lhs, Color4 rhs)
        {
            // Return true if the fields match:
            return lhs.R == rhs.R && lhs.G == rhs.G && lhs.B == rhs.B && lhs.A == rhs.A;
        }

        /// <summary>
        /// Not comparison operator
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static bool operator !=(Color4 lhs, Color4 rhs)
        {
            return !(lhs == rhs);
        }

        #endregion Operators

        /// <summary>An LLColor with zero RGB values and full alpha</summary>
        public readonly static Color4 Black = new Color4(0f, 0f, 0f, 1f);
    }

    /// <summary>
    /// A quaternion, used for rotations
    /// </summary>
	[Serializable]
    public struct Quaternion
	{
        /// <summary>X value</summary>
        public float X;
        /// <summary>Y value</summary>
        public float Y;
        /// <summary>Z value</summary>
        public float Z;
        /// <summary>W value</summary>
        public float W;

        // Used for little to big endian conversion on big endian architectures
        private byte[] conversionBuffer;

        #region Properties

        /// <summary>
        /// Returns the conjugate (spatial inverse) of this quaternion
        /// </summary>
        public Quaternion Conjugate
        {
            get
            {
                return new Quaternion(-this.X, -this.Y, -this.Z, this.W);
            }
        }

        #endregion Properties

        #region Constructors

        /// <summary>
        /// Constructor, builds a quaternion object from a byte array
        /// </summary>
        /// <param name="byteArray">Byte array containing four four-byte floats</param>
        /// <param name="pos">Offset in the byte array to start reading at</param>
        /// <param name="normalized">Whether the source data is normalized or
        /// not. If this is true 12 bytes will be read, otherwise 16 bytes will
        /// be read.</param>
        public Quaternion(byte[] byteArray, int pos, bool normalized)
        {
            conversionBuffer = null;
            X = Y = Z = W = 0;
            FromBytes(byteArray, pos, normalized);
        }

        /// <summary>
        /// Build a quaternion from normalized float values
        /// </summary>
        /// <param name="x">X value from -1.0 to 1.0</param>
        /// <param name="y">Y value from -1.0 to 1.0</param>
        /// <param name="z">Z value from -1.0 to 1.0</param>
        public Quaternion(float x, float y, float z)
        {
            conversionBuffer = null;
            X = x;
            Y = y;
            Z = z;

            float xyzsum = 1 - X * X - Y * Y - Z * Z;
            W = (xyzsum > 0) ? (float)Math.Sqrt(xyzsum) : 0;
        }

        /// <summary>
        /// Build a quaternion from individual float values
        /// </summary>
        /// <param name="x">X value</param>
        /// <param name="y">Y value</param>
        /// <param name="z">Z value</param>
        /// <param name="w">W value</param>
        public Quaternion(float x, float y, float z, float w)
        {
            conversionBuffer = null;
            X = x;
            Y = y;
            Z = z;
            W = w;
        }

        /// <summary>
        /// Build a quaternion from an angle and a vector
        /// </summary>
        /// <param name="angle">Angle value</param>
        /// <param name="vec">Vector value</param>
        public Quaternion(float angle, Vector3 vec)
        {
            conversionBuffer = null;
            X = Y = Z = W = 0f;
            SetQuaternion(angle, vec);
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="quaternion">Quaternion to copy</param>
        public Quaternion(Quaternion quaternion)
        {
            conversionBuffer = null;
            X = quaternion.X;
            Y = quaternion.Y;
            Z = quaternion.Z;
            W = quaternion.W;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Builds a quaternion object from a byte array
        /// </summary>
        /// <param name="byteArray">The source byte array</param>
        /// <param name="pos">Offset in the byte array to start reading at</param>
        /// <param name="normalized">Whether the source data is normalized or
        /// not. If this is true 12 bytes will be read, otherwise 16 bytes will
        /// be read.</param>
        public void FromBytes(byte[] byteArray, int pos, bool normalized)
        {
            if (!normalized)
            {
                if (!BitConverter.IsLittleEndian)
                {
                    // Big endian architecture
                    if (conversionBuffer == null)
                        conversionBuffer = new byte[16];

                    Buffer.BlockCopy(byteArray, pos, conversionBuffer, 0, 16);

                    Array.Reverse(conversionBuffer, 0, 4);
                    Array.Reverse(conversionBuffer, 4, 4);
                    Array.Reverse(conversionBuffer, 8, 4);
                    Array.Reverse(conversionBuffer, 12, 4);

                    X = BitConverter.ToSingle(conversionBuffer, 0);
                    Y = BitConverter.ToSingle(conversionBuffer, 4);
                    Z = BitConverter.ToSingle(conversionBuffer, 8);
                    W = BitConverter.ToSingle(conversionBuffer, 12);
                }
                else
                {
                    // Little endian architecture
                    X = BitConverter.ToSingle(byteArray, pos);
                    Y = BitConverter.ToSingle(byteArray, pos + 4);
                    Z = BitConverter.ToSingle(byteArray, pos + 8);
                    W = BitConverter.ToSingle(byteArray, pos + 12);
                }
            }
            else
            {
                if (!BitConverter.IsLittleEndian)
                {
                    // Big endian architecture
                    if (conversionBuffer == null)
                        conversionBuffer = new byte[16];

                    Buffer.BlockCopy(byteArray, pos, conversionBuffer, 0, 12);

                    Array.Reverse(conversionBuffer, 0, 4);
                    Array.Reverse(conversionBuffer, 4, 4);
                    Array.Reverse(conversionBuffer, 8, 4);

                    X = BitConverter.ToSingle(conversionBuffer, 0);
                    Y = BitConverter.ToSingle(conversionBuffer, 4);
                    Z = BitConverter.ToSingle(conversionBuffer, 8);
                }
                else
                {
                    // Little endian architecture
                    X = BitConverter.ToSingle(byteArray, pos);
                    Y = BitConverter.ToSingle(byteArray, pos + 4);
                    Z = BitConverter.ToSingle(byteArray, pos + 8);
                }

                float xyzsum = 1 - X * X - Y * Y - Z * Z;
                W = (xyzsum > 0) ? (float)Math.Sqrt(xyzsum) : 0;
            }
        }

        /// <summary>
        /// Normalize this quaternion and serialize it to a byte array
        /// </summary>
        /// <returns>A 12 byte array containing normalized X, Y, and Z floating
        /// point values in order using little endian byte ordering</returns>
        public byte[] GetBytes()
        {
            byte[] bytes = new byte[12];
            float norm;

            norm = (float)Math.Sqrt(X * X + Y * Y + Z * Z + W * W);

            if (norm != 0)
            {
                norm = 1 / norm;

                float x, y, z;
                if (W >= 0) {
                    x = X; y = Y; z = Z;
                } else {
                    x = -X; y = -Y; z = -Z;
                }

                Buffer.BlockCopy(BitConverter.GetBytes(norm * x), 0, bytes, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(norm * y), 0, bytes, 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(norm * z), 0, bytes, 8, 4);

                if (!BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes, 0, 4);
                    Array.Reverse(bytes, 4, 4);
                    Array.Reverse(bytes, 8, 4);
                }
            }
            else
            {
                throw new Exception("Quaternion " + this.ToString() + " normalized to zero");
            }

            return bytes;
        }

        public LLSD ToLLSD()
        {
            LLSDArray array = new LLSDArray();
            array.Add(LLSD.FromReal(X));
            array.Add(LLSD.FromReal(Y));
            array.Add(LLSD.FromReal(Z));
            array.Add(LLSD.FromReal(W));
            return array;
        }

        public void FromLLSD(LLSD llsd)
        {
            if (llsd.Type == LLSDType.Array)
            {
                LLSDArray array = (LLSDArray)llsd;

                if (array.Count == 4)
                {
                    X = (float)array[0].AsReal();
                    Y = (float)array[1].AsReal();
                    Z = (float)array[2].AsReal();
                    W = (float)array[3].AsReal();

                    return;
                }
            }

            this = Quaternion.Identity;
        }

        //FIXME: Need comprehensive testing for this
        //public void GetEulerAngles(out float roll, out float pitch, out float yaw)
        //{
        //    LLMatrix3 rotMat = new LLMatrix3(this);
        //    rotMat = LLMatrix3.Orthogonalize(rotMat);
        //    rotMat.GetEulerAngles(out roll, out pitch, out yaw);
        //}

        //FIXME: Need comprehensive testing for this
        /// <summary>
        /// Returns the inverse matrix from a quaternion, or the correct
        /// matrix if the quaternion is inverse
        /// </summary>
        /// <returns>A matrix representation of this quaternion</returns>
        public Matrix3 ToMatrix()
        {
            Matrix3 m;
            float xx, xy, xz, xw, yy, yz, yw, zz, zw;

            xx = X * X;
            xy = X * Y;
            xz = X * Z;
            xw = X * W;

            yy = Y * Y;
            yz = Y * Z;
            yw = Y * W;

            zz = Z * Z;
            zw = Z * W;

            m.M11 = 1f - 2f * (yy + zz);
            m.M12 = 2f * (xy + zw);
            m.M13 = 2f * (xz - yw);

            m.M21 = 2f * (xy - zw);
            m.M22 = 1f - 2f * (xx + zz);
            m.M23 = 2f * (yz + xw);

            m.M31 = 2f * (xz + yw);
            m.M32 = 2f * (yz - xw);
            m.M33 = 1f - 2f * (xx + yy);

            return m;
        }

        public void SetQuaternion(float angle, float x, float y, float z)
        {
            Vector3 vec = new Vector3(x, y, z);
            SetQuaternion(angle, vec);
        }

        public void SetQuaternion(float angle, Vector3 vec)
        {
            vec = Vector3.Norm(vec);

            angle *= 0.5f;
            float c = (float)Math.Cos(angle);
            float s = (float)Math.Sin(angle);

            X = vec.X * s;
            Y = vec.Y * s;
            Z = vec.Z * s;
            W = c;

            this = Quaternion.Norm(this);
        }

        #endregion Public Methods

        #region Static Methods

        /// <summary>
        /// Creates a quaternion from a euler vector
        /// </summary>
        /// <param name="euler">Vector representation of the euler angle</param>
        /// <returns>Quaternion representation of Euler angle</returns>
        public static Quaternion FromEuler(Vector3 euler)
        {
            double atCos = Math.Cos(euler.X / 2);
            double atSin = Math.Sin(euler.X / 2);
            double leftCos = Math.Cos(euler.Y / 2);
            double leftSin = Math.Sin(euler.Y / 2);
            double upCos = Math.Cos(euler.Z / 2);
            double upSin = Math.Sin(euler.Z / 2);
            double atLeftCos = atCos * leftCos;
            double atLeftSin = atSin * leftSin;
            return new Quaternion(
                (float)(atSin * leftCos * upCos + atCos * leftSin * upSin),
                (float)(atCos * leftSin * upCos - atSin * leftCos * upSin),
                (float)(atLeftCos * upSin + atLeftSin * upCos),
                (float)(atLeftCos * upCos - atLeftSin * upSin)
            );
        }

        /// <summary>
        /// Calculate the magnitude of the supplied quaternion
        /// </summary>
        public static float Mag(Quaternion q)
        {
            return (float)Math.Sqrt(q.W * q.W + q.X * q.X + q.Y * q.Y + q.Z * q.Z);
        }

        /// <summary>
        /// Returns a normalized version of the supplied quaternion
        /// </summary>
        /// <param name="q">The quaternion to normalize</param>
        /// <returns>A normalized version of the quaternion</returns>
        public static Quaternion Norm(Quaternion q)
        {
            const float MAG_THRESHOLD = 0.0000001f;
            float mag = (float)Math.Sqrt(q.X * q.X + q.Y * q.Y + q.Z * q.Z + q.W * q.W);

            if (mag > MAG_THRESHOLD)
            {
                float oomag = 1.0f / mag;
                q.X *= oomag;
                q.Y *= oomag;
                q.Z *= oomag;
                q.W *= oomag;
            }
            else
            {
                q.X = 0.0f;
                q.Y = 0.0f;
                q.Z = 0.0f;
                q.W = 1.0f;
            }

            return q;
        }

        public static Quaternion Parse(string val)
        {
            char[] splitChar = { ',' };
            string[] split = val.Replace("<", String.Empty).Replace(">", String.Empty).Split(splitChar);
            if (split.Length == 3)
            {
                return new Quaternion(
                    float.Parse(split[0].Trim(), Helpers.EnUsCulture),
                    float.Parse(split[1].Trim(), Helpers.EnUsCulture),
                    float.Parse(split[2].Trim(), Helpers.EnUsCulture));
            }
            else
            {
                return new Quaternion(
                    float.Parse(split[0].Trim(), Helpers.EnUsCulture),
                    float.Parse(split[1].Trim(), Helpers.EnUsCulture),
                    float.Parse(split[2].Trim(), Helpers.EnUsCulture),
                    float.Parse(split[3].Trim(), Helpers.EnUsCulture));
            }
        }

        public static bool TryParse(string val, out Quaternion result)
        {
            try
            {
                result = Parse(val);
                return true;
            }
            catch (Exception)
            {
                result = new Quaternion();
                return false;
            }
        }

        #endregion Static Methods

        #region Overrides

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return (X.GetHashCode() ^ Y.GetHashCode() ^ Z.GetHashCode() ^ W.GetHashCode());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public override bool Equals(object o)
        {
            if (!(o is Quaternion)) return false;

            Quaternion quaternion = (Quaternion)o;

            return X == quaternion.X && Y == quaternion.Y && Z == quaternion.Z && W == quaternion.W;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "<" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ", " + W.ToString() + ">";
        }

        #endregion Overrides

        #region Operators

        public static bool operator ==(Quaternion lhs, Quaternion rhs)
        {
            // Return true if the fields match:
            return lhs.X == rhs.X && lhs.Y == rhs.Y && lhs.Z == rhs.Z && lhs.W == rhs.W;
        }

        public static bool operator !=(Quaternion lhs, Quaternion rhs)
        {
            return !(lhs == rhs);
        }

        /// <summary>
        /// Performs quaternion multiplication
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Quaternion operator *(Quaternion lhs, Quaternion rhs)
        {
            Quaternion ret = new Quaternion(
                (lhs.W * rhs.X) + (lhs.X * rhs.W) + (lhs.Y * rhs.Z) - (lhs.Z * rhs.Y),
                (lhs.W * rhs.Y) - (lhs.X * rhs.Z) + (lhs.Y * rhs.W) + (lhs.Z * rhs.X),
                (lhs.W * rhs.Z) + (lhs.X * rhs.Y) - (lhs.Y * rhs.X) + (lhs.Z * rhs.W),
                (lhs.W * rhs.W) - (lhs.X * rhs.X) - (lhs.Y * rhs.Y) - (lhs.Z * rhs.Z)
            );

            return ret;
        }

        /// <summary>
        /// Division operator (multiply by the conjugate)
        /// </summary>
        /// <param name="lhs"></param>
        /// <param name="rhs"></param>
        /// <returns></returns>
        public static Quaternion operator /(Quaternion lhs, Quaternion rhs)
        {
            return lhs * rhs.Conjugate;
        }

        #endregion Operators

        /// <summary>An LLQuaternion with a value of 0,0,0,1</summary>
        public readonly static Quaternion Identity = new Quaternion(0f, 0f, 0f, 1f);
	}

    /// <summary>
    /// A 3x3 matrix
    /// </summary>
    [Serializable]
    public struct Matrix3
    {
        public float M11, M12, M13;
        public float M21, M22, M23;
        public float M31, M32, M33;

        #region Properties

        public float Trace
        {
            get
            {
                return M11 + M22 + M33;
            }
        }

        public float Determinant
        {
            get
            {
                return M11 * M22 * M33 + M12 * M23 * M31 + M13 * M21 * M32 - 
                       M13 * M22 * M31 - M11 * M23 * M32 - M12 * M21 * M33;
            }
        }

        #endregion Properties

        #region Constructors

        public Matrix3(float m11, float m12, float m13, float m21, float m22, float m23, float m31, float m32, float m33)
        {
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        public Matrix3(Quaternion q)
        {
            this = q.ToMatrix();
        }

        //FIXME:
        //public LLMatrix3(float roll, float pitch, float yaw)
        //{
        //    M11 = M12 = M13 = M21 = M22 = M23 = M31 = M32 = M33 = 0f;
        //    FromEulers(roll, pitch, yaw);
        //}

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="m">Matrix to copy</param>
        public Matrix3(Matrix3 m)
        {
            M11 = m.M11;
            M12 = m.M12;
            M13 = m.M13;
            M21 = m.M21;
            M22 = m.M22;
            M23 = m.M23;
            M31 = m.M31;
            M32 = m.M32;
            M33 = m.M33;
        }

        #endregion Constructors

        #region Public Methods

        // FIXME: Need comprehensive testing for this
        //public void FromEulers(float roll, float pitch, float yaw)
        //{
        //    float cx, sx, cy, sy, cz, sz;
        //    float cxsy, sxsy;

        //    cx = (float)Math.Cos(roll);
        //    sx = (float)Math.Sin(roll);
        //    cy = (float)Math.Cos(pitch);
        //    sy = (float)Math.Sin(pitch);
        //    cz = (float)Math.Cos(yaw);
        //    sz = (float)Math.Sin(yaw);

        //    cxsy = cx * sy;
        //    sxsy = sx * sy;

        //    M11 = cy * cz;
        //    M21 = -cy * sz;
        //    M31 = sy;
        //    M12 = sxsy * cz + cx * sz;
        //    M22 = -sxsy * sz + cx * cz;
        //    M32 = -sx * cy;
        //    M13 = -cxsy * cz + sx * sz;
        //    M23 = cxsy * sz + sx * cz;
        //    M33 = cx * cy;
        //}

        public void GetEulerAngles(out float roll, out float pitch, out float yaw)
        {
            // From the Matrix and Quaternion FAQ: http://www.j3d.org/matrix_faq/matrfaq_latest.html

            double angleX, angleY, angleZ;
            double cx, cy, cz; // cosines
            double sx, sz; // sines

            angleY = Math.Asin(Helpers.Clamp(M31, -1f, 1f));
            cy = Math.Cos(angleY);

            if (Math.Abs(cy) > 0.005f)
            {
                // No gimbal lock
                cx = M33 / cy;
                sx = (-M32) / cy;

                angleX = (float)Math.Atan2(sx, cx);

                cz = M11 / cy;
                sz = (-M21) / cy;

                angleZ = (float)Math.Atan2(sz, cz);
            }
            else
            {
                // Gimbal lock
                angleX = 0;
                
                cz = M22;
                sz = M12;

                angleZ = Math.Atan2(sz, cz);
            }

            roll = (float)angleX;
            pitch = (float)angleY;
            yaw = (float)angleZ;
        }

        public Quaternion ToQuaternion()
        {
            Quaternion quat = new Quaternion();
            float tr, s;
            float[] q = new float[4];
            int i, j, k;
            int[] nxt = new int[] { 1, 2, 0 };

            tr = this[0, 0] + this[1, 1] + this[2, 2];

            // Check the diagonal
            if (tr > 0f)
            {
                s = (float)Math.Sqrt(tr + 1f);
                quat.W = s / 2f;
                
                s = 0.5f / s;
                quat.X = (this[1, 2] - this[2, 1]) * s;
                quat.Y = (this[2, 0] - this[0, 2]) * s;
                quat.Z = (this[0, 1] - this[1, 0]) * s;
            }
            else
            {
                // Diagonal is negative
                i = 0;
                if (this[1, 1] > this[0, 0])
                    i = 1;
                if (this[2, 2] > this[i, i])
                    i = 2;

                j = nxt[i];
                k = nxt[j];

                s = (float)Math.Sqrt((this[i, i] - (this[j, j] + this[k, k])) + 1f);
                q[i] = s * 0.5f;

                if (s != 0f)
                    s = 0.5f / s;

                q[3] = (this[j, k] - this[k, j]) * s;
                q[j] = (this[i, j] + this[j, i]) * s;
                q[k] = (this[i, k] + this[k, i]) * s;

                quat.X = q[0];
                quat.Y = q[1];
                quat.Z = q[2];
                quat.W = q[3];
            }

            return quat;
        }

        #endregion Public Methods

        #region Static Methods

        public static Matrix3 Add(Matrix3 left, Matrix3 right)
        {
            return new Matrix3(
                left.M11 + right.M11, left.M12 + right.M12, left.M13 + right.M13,
                left.M21 + right.M21, left.M22 + right.M22, left.M23 + right.M23,
                left.M31 + right.M31, left.M32 + right.M32, left.M33 + right.M33
            );
        }

        public static Matrix3 Add(Matrix3 matrix, float scalar)
        {
            return new Matrix3(
                matrix.M11 + scalar, matrix.M12 + scalar, matrix.M13 + scalar,
                matrix.M21 + scalar, matrix.M22 + scalar, matrix.M23 + scalar,
                matrix.M31 + scalar, matrix.M32 + scalar, matrix.M33 + scalar
            );
        }

        public static Matrix3 Subtract(Matrix3 left, Matrix3 right)
        {
            return new Matrix3(
                left.M11 - right.M11, left.M12 - right.M12, left.M13 - right.M13,
                left.M21 - right.M21, left.M22 - right.M22, left.M23 - right.M23,
                left.M31 - right.M31, left.M32 - right.M32, left.M33 - right.M33
                );
        }

        public static Matrix3 Subtract(Matrix3 matrix, float scalar)
        {
            return new Matrix3(
                matrix.M11 - scalar, matrix.M12 - scalar, matrix.M13 - scalar,
                matrix.M21 - scalar, matrix.M22 - scalar, matrix.M23 - scalar,
                matrix.M31 - scalar, matrix.M32 - scalar, matrix.M33 - scalar
                );
        }

        public static Matrix3 Multiply(Matrix3 left, Matrix3 right)
        {
            return new Matrix3(
                left.M11 * right.M11 + left.M12 * right.M21 + left.M13 * right.M31,
                left.M11 * right.M12 + left.M12 * right.M22 + left.M13 * right.M32,
                left.M11 * right.M13 + left.M12 * right.M23 + left.M13 * right.M33,

                left.M21 * right.M11 + left.M22 * right.M21 + left.M23 * right.M31,
                left.M21 * right.M12 + left.M22 * right.M22 + left.M23 * right.M32,
                left.M21 * right.M13 + left.M22 * right.M23 + left.M23 * right.M33,

                left.M31 * right.M11 + left.M32 * right.M21 + left.M33 * right.M31,
                left.M31 * right.M12 + left.M32 * right.M22 + left.M33 * right.M32,
                left.M31 * right.M13 + left.M32 * right.M23 + left.M33 * right.M33
            );
        }

        /// <summary>
        /// Transposes a matrix
        /// </summary>
        /// <param name="m">Matrix to transpose</param>
        /// <returns>Transposed matrix</returns>
        public static Matrix3 Transpose(Matrix3 m)
        {
            Helpers.Swap<float>(ref m.M12, ref m.M21);
            Helpers.Swap<float>(ref m.M13, ref m.M31);
            Helpers.Swap<float>(ref m.M23, ref m.M32);

            return m;
        }

        public static Matrix3 Orthogonalize(Matrix3 m)
        {
            Vector3 xAxis = m[0];
            Vector3 yAxis = m[1];
            Vector3 zAxis = m[2];

            xAxis = Vector3.Norm(xAxis);
            yAxis -= xAxis * (xAxis * yAxis);
            yAxis = Vector3.Norm(yAxis);
            zAxis = Vector3.Cross(xAxis, yAxis);

            m[0] = xAxis;
            m[1] = yAxis;
            m[2] = zAxis;

            return m;
        }

        #endregion Static Methods

        #region Overrides

        public override int GetHashCode()
        {
            return
                M11.GetHashCode() ^ M12.GetHashCode() ^ M13.GetHashCode() ^
                M21.GetHashCode() ^ M22.GetHashCode() ^ M23.GetHashCode() ^
                M31.GetHashCode() ^ M32.GetHashCode() ^ M33.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj is Matrix3)
            {
                Matrix3 m = (Matrix3)obj;
                return
                    (M11 == m.M11) && (M12 == m.M12) && (M13 == m.M13) &&
                    (M21 == m.M21) && (M22 == m.M22) && (M23 == m.M23) &&
                    (M31 == m.M31) && (M32 == m.M32) && (M33 == m.M33);
            }
            return false;
        }

        public override string ToString()
        {
            return string.Format("[{0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}]",
                M11, M12, M13, M21, M22, M23, M31, M32, M33);
        }

        #endregion Overrides

        #region Operators

        public static bool operator ==(Matrix3 left, Matrix3 right)
        {
            return ValueType.Equals(left, right);
        }

        public static bool operator !=(Matrix3 left, Matrix3 right)
        {
            return !ValueType.Equals(left, right);
        }

        public static Matrix3 operator +(Matrix3 left, Matrix3 right)
        {
            return Matrix3.Add(left, right);
        }

        public static Matrix3 operator +(Matrix3 matrix, float scalar)
        {
            return Matrix3.Add(matrix, scalar);
        }

        public static Matrix3 operator +(float scalar, Matrix3 matrix)
        {
            return Matrix3.Add(matrix, scalar);
        }

        public static Matrix3 operator -(Matrix3 left, Matrix3 right)
        {
            return Matrix3.Subtract(left, right); ;
        }

        public static Matrix3 operator -(Matrix3 matrix, float scalar)
        {
            return Matrix3.Subtract(matrix, scalar);
        }

        public static Matrix3 operator *(Matrix3 left, Matrix3 right)
        {
            return Matrix3.Multiply(left, right); ;
        }

        public Vector3 this[int row]
        {
            get
            {
                switch (row)
                {
                    case 0:
                        return new Vector3(M11, M12, M13);
                    case 1:
                        return new Vector3(M21, M22, M23);
                    case 2:
                        return new Vector3(M31, M32, M33);
                    default:
                        throw new IndexOutOfRangeException("LLMatrix3 row index must be from 0-2");
                }
            }
            set
            {
                switch (row)
                {
                    case 0:
                        M11 = value.X;
                        M12 = value.Y;
                        M13 = value.Z;
                        break;
                    case 1:
                        M21 = value.X;
                        M22 = value.Y;
                        M23 = value.Z;
                        break;
                    case 2:
                        M31 = value.X;
                        M32 = value.Y;
                        M33 = value.Z;
                        break;
                    default:
                        throw new IndexOutOfRangeException("LLMatrix3 row index must be from 0-2");
                }
            }
        }

        public float this[int row, int column]
        {
            get
            {
                switch (row)
                {
                    case 0:
                        switch (column)
                        {
                            case 0:
                                return M11;
                            case 1:
                                return M12;
                            case 2:
                                return M13;
                            default:
                                throw new IndexOutOfRangeException("LLMatrix3 row and column values must be from 0-2");
                        }
                    case 1:
                        switch (column)
                        {
                            case 0:
                                return M21;
                            case 1:
                                return M22;
                            case 2:
                                return M23;
                            default:
                                throw new IndexOutOfRangeException("LLMatrix3 row and column values must be from 0-2");
                        }
                    case 2:
                        switch (column)
                        {
                            case 0:
                                return M31;
                            case 1:
                                return M32;
                            case 2:
                                return M33;
                            default:
                                throw new IndexOutOfRangeException("LLMatrix3 row and column values must be from 0-2");
                        }
                    default:
                        throw new IndexOutOfRangeException("LLMatrix3 row and column values must be from 0-2");
                }
            }
            set
            {
                //FIXME:
                throw new NotImplementedException();
            }
        }

        #endregion Operators

        /// <summary>A 3x3 matrix set to all zeroes</summary>
        public static readonly Matrix3 Zero = new Matrix3();
        /// <summary>A 3x3 identity matrix</summary>
        public static readonly Matrix3 Identity = new Matrix3(
            1f, 0f, 0f,
            0f, 1f, 0f,
            0f, 0f, 1f
        );
    }
}