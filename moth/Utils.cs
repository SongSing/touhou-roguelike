using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace moth
{
    static class Utils
    {
        public static float RadToDeg(float rad)
        {
            return (float)(rad * 180 / Math.PI);
        }

        public static float DegToRad(float deg)
        {
            return (float)(deg / 180.0f * Math.PI);
        }

        public static Vector2 RadToVector2(float rad)
        {
            return new Vector2((float)Math.Cos(rad), (float)Math.Sin(rad));
        }

        public static Vector2 DegToVector2(float deg)
        {
            return RadToVector2(DegToRad(deg));
        }

        public static TV GetValue<TK, TV>(this IDictionary<TK, TV> dict, TK key, TV defaultValue = default(TV))
        {
            return dict.TryGetValue(key, out TV value) ? value : defaultValue;
        }

        public static T GetValue<T>(this XmlAttributeCollection attrs, string attr, T defaultValue = default(T))
        {
            if (attrs == null || attrs[attr] == null)
            {
                return defaultValue;
            }
            else
            {
                return (T)Convert.ChangeType(attrs[attr].Value, typeof(T));
            }
        }

        public static T[] Copy<T>(this T[] array)
        {
            T[] newArray = new T[array.Length];
            array.CopyTo(newArray, 0);
            return newArray;
        }

        public static Point Rounded(this Vector2 v)
        {
            return new Point((int)Math.Round(v.X), (int)Math.Round(v.Y));
        }

        public static Point Clamped(this Point p, Point min, Point max)
        {
            p.X = Math.Max(Math.Min(p.X, max.X), min.X);
            p.Y = Math.Max(Math.Min(p.Y, max.Y), min.Y);
            return p;
        }

        public static Vector2 Clamped(this Vector2 p, Vector2 min, Vector2 max)
        {
            p.X = Math.Max(Math.Min(p.X, max.X), min.X);
            p.Y = Math.Max(Math.Min(p.Y, max.Y), min.Y);
            return p;
        }

        public static Vector2 ParseToVector(string str)
        {
            string[] parts = str.Split(',');
            return new Vector2(float.Parse(parts[0]), float.Parse(parts[1]));
        }

        public static string ParseFromVector(Vector2 vector)
        {
            return vector.X + "," + vector.Y;
        }

        public static string Capitlized(this string str)
        {
            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }
    }
}
