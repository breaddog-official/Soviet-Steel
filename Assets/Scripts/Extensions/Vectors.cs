using System;

namespace Scripts.Extensions
{
    /// <summary>
    /// Generic vector with 2 axis
    /// </summary>
    [Serializable]
    public struct GenVector2<T>
    {
        public T x;
        public T y;

        public GenVector2(T x, T y)
        {
            this.x = x;
            this.y = y;
        }

        public readonly override bool Equals(object other)
        {
            if (other is GenVector2<T> vector)
            {
                return x.Equals(vector.x) && y.Equals(vector.y);
            }

            return false;
        }

        public readonly override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode();
        public readonly override string ToString() => $"{GetType().Name}: {x}, {y}";
    }

    /// <summary>
    /// Generic vector with 3 axis
    /// </summary>
    [Serializable]
    public struct GenVector3<T>
    {
        public T x;
        public T y;
        public T z;

        public GenVector3(T x, T y, T z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public readonly override bool Equals(object other)
        {
            if (other is GenVector3<T> vector)
            {
                return x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z);
            }

            return false;
        }

        public readonly override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode();
        public readonly override string ToString() => $"{GetType().Name}: {x}, {y}, {z}";

        public static explicit operator GenVector3<T>(GenVector2<T> vector) => new GenVector3<T>(vector.x, vector.y, default);
    }

    /// <summary>
    /// Generic vector with 4 axis
    /// </summary>
    [Serializable]
    public struct GenVector4<T>
    {
        public T x;
        public T y;
        public T z;
        public T w;

        public GenVector4(T x, T y, T z, T w)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.w = w;
        }



        public readonly override bool Equals(object other) 
        {
            if (other is GenVector4<T> vector)
            {
                return x.Equals(vector.x) && y.Equals(vector.y) && z.Equals(vector.z) && w.Equals(vector.w);
            }
            
            return false;
        }

        public readonly override int GetHashCode() => x.GetHashCode() ^ y.GetHashCode() ^ z.GetHashCode() ^ w.GetHashCode();
        public readonly override string ToString() => $"{GetType().Name}: {x}, {y}, {z}, {w}";

        public static explicit operator GenVector4<T>(GenVector2<T> vector) => new GenVector4<T>(vector.x, vector.y, default, default);
        public static explicit operator GenVector4<T>(GenVector3<T> vector) => new GenVector4<T>(vector.x, vector.y, vector.z, default);

    }
}
