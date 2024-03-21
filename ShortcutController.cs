using Newtonsoft.Json;
using System.Runtime.InteropServices;

namespace SpotiHotKey
{
    public class OnShortcutSetArgs : EventArgs
    {
        public string Shortcut { get; private set; }

        public OnShortcutSetArgs(string shortcut = "")
        {
            Shortcut = shortcut;
        }
    }

    public class ShortcutController
    {
        private IntPtr hookID = IntPtr.Zero;
        private NativeMethods.LowLevelKeyboardProc proc;
        private List<Keys> currentKeys = new List<Keys>();
        private List<Keys> shortcutKeys = new List<Keys>();
        private bool settingShortcut = false;

        public delegate void OnShortcutHandler(object source, OnShortcutSetArgs args);
        public event OnShortcutHandler OnShortcutSetEvent;
        public event OnShortcutHandler OnShortcutCallEvent;

        public ShortcutController()
        {
            proc = HookCallback;
            if (hookID == IntPtr.Zero)
            {
                hookID = SetHook(proc);
            }
            shortcutKeys = ConfigManager.ShortcutKeys;
        }

        public void SetShortcut()
        {
            settingShortcut = true;
        }
        public string GetShortcut()
        {
            return string.Join(" + ", shortcutKeys);
        }

        private IntPtr SetHook(NativeMethods.LowLevelKeyboardProc proc)
        {
            using (var curProcess = System.Diagnostics.Process.GetCurrentProcess())
            using (var curModule = curProcess.MainModule)
            {
                return NativeMethods.SetWindowsHookEx(NativeMethods.WH_KEYBOARD_LL, proc, NativeMethods.GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                if (wParam == (IntPtr)NativeMethods.WM_KEYDOWN)
                {
                    if (settingShortcut && !currentKeys.Contains(key))
                    {
                        currentKeys.Add(key);
                        return (IntPtr)1; // Block other apps to receive this event
                    }
                    else if (!currentKeys.Contains(key))
                    {
                        currentKeys.Add(key);
                    }
                }
                else if (wParam == (IntPtr)NativeMethods.WM_KEYUP)
                {
                    if (settingShortcut)
                    {
                        shortcutKeys = new List<Keys>(currentKeys); // save shortcut
                        ConfigManager.ShortcutKeys = shortcutKeys;
                        settingShortcut = false;
                        currentKeys.Clear();
                        if (OnShortcutSetEvent != null)
                        {
                            OnShortcutSetEvent(this, new OnShortcutSetArgs(string.Join(" + ", shortcutKeys))); // Raise the event
                        }
                        return (IntPtr)1;
                    }
                    else
                    {
                        if (shortcutKeys.SequenceEqual(shortcutKeys.Intersect(currentKeys)))
                        {
                            currentKeys.Clear();
                            OnShortcutCallEvent(this, new OnShortcutSetArgs());
                        }
                        currentKeys.Clear();
                    }
                }
            }
            return NativeMethods.CallNextHookEx(hookID, nCode, wParam, lParam);
        }

        ~ShortcutController()
        {
            if (hookID != IntPtr.Zero)
            {
                NativeMethods.UnhookWindowsHookEx(hookID);
            }
        }
    }
}
