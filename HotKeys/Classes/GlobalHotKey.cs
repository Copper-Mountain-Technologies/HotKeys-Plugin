using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace HotKeys
{
    public class GlobalHotKey
    {
        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        // modifiers
        public const int NOMOD = 0x0000;

        public const int ALT = 0x0001;
        public const int CTRL = 0x0002;
        public const int SHIFT = 0x0004;
        public const int WIN = 0x0008;

        // windows message id for hot key
        public const int WM_HOTKEY_MSG_ID = 0x0312;

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public int Id { get; set; }

        public string Description { get; set; }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        private int modifier;
        private int key;
        private IntPtr hWnd;

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::

        public GlobalHotKey(int modifier, Keys key, Form form)
        {
            this.modifier = modifier;
            this.key = (int)key;
            this.hWnd = form.Handle;

            Id = this.GetHashCode();
            Description = getDescription(modifier, key);
        }

        private string getDescription(int modifier, Keys key)
        {
            string returnString = "";

            // determine modifier description
            if (modifier != NOMOD)
            {
                if ((modifier & ALT) == ALT)
                {
                    returnString = returnString + "ALT" + " + ";
                }
                if ((modifier & CTRL) == CTRL)
                {
                    returnString = returnString + "CTRL" + " + ";
                }
                if ((modifier & SHIFT) == SHIFT)
                {
                    returnString = returnString + "SHIFT" + " + ";
                }
                if ((modifier & WIN) == WIN)
                {
                    returnString = returnString + "WIN" + " + ";
                }
            }

            // add key description
            returnString = returnString + key.ToString();

            return returnString;
        }

        // ------------------------------------------------------------------------------------------------------------

        public bool Register()
        {
            return RegisterHotKey(hWnd, Id, modifier, key);
        }

        // ------------------------------------------------------------------------------------------------------------

        public bool Unregiser()
        {
            return UnregisterHotKey(hWnd, Id);
        }

        // ------------------------------------------------------------------------------------------------------------

        public override int GetHashCode()
        {
            return modifier ^ key ^ hWnd.ToInt32();
        }

        // ::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
    }
}