using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Input;

namespace moth
{
    class AKS
    {
        public enum KeyState
        {
            JustReleased = -1,
            Up = 0,
            JustPressed = 1,
            Down = 2,
        }

        private static Dictionary<Keys, KeyState> lastFrameKeys = new Dictionary<Keys, KeyState>();
        private static Dictionary<Keys, KeyState> keys = new Dictionary<Keys, KeyState>();
        private static List<Keys> watchingKeys = new List<Keys>();

        private static KeyboardState KeyboardState
        {
            get { return Keyboard.GetState(); }
        }

        public static bool IsKeyDown(Keys key)
        {
            return AKS.KeyboardState.IsKeyDown(key);
        }

        public static bool IsKeyUp(Keys key)
        {
            return AKS.KeyboardState.IsKeyUp(key);
        }

        public static void WatchKey(Keys key)
        {
            watchingKeys.Add(key);
            keys[key] = IsKeyDown(key) ? KeyState.Down : KeyState.Up;
            lastFrameKeys[key] = IsKeyDown(key) ? KeyState.Down : KeyState.Up;
        }

        public static void Update()
        {
            foreach (var key in watchingKeys)
            {
                lastFrameKeys[key] = keys[key];
                keys[key] = IsKeyDown(key) ? KeyState.Down : KeyState.Up;

                if (keys[key] == KeyState.Up && (int)lastFrameKeys[key] >= 1)
                {
                    keys[key] = KeyState.JustReleased;
                }
                else if (keys[key] == KeyState.Down && (int)lastFrameKeys[key] <= 0)
                {
                    keys[key] = KeyState.JustPressed;
                }
            }
        }

        public static KeyState GetKeyState(Keys key)
        {
            if (watchingKeys.Contains(key))
            {
                return keys[key];
            }
            else
            {
                throw new Exception("Not watching for key: " + key.ToString());
            }
        }

        public static bool WasJustPressed(Keys key)
        {
            return GetKeyState(key) == KeyState.JustPressed;
        }

        public static int Axis(Keys negative, Keys positive)
        {
            return Convert.ToInt32(IsKeyDown(positive)) - Convert.ToInt32(IsKeyDown(negative));
        }
    }
}
