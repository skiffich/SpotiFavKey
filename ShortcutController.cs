using Newtonsoft.Json;
using System.Linq;
using System.Runtime.InteropServices;

namespace SpotiHotKey
{
    public class OnShortcutSetArgs : EventArgs
    {
        public string Shortcut { get; private set; }

        public int ShortcutIdx { get; private set; }

        public OnShortcutSetArgs(string shortcut = "", int shortcutIdx = 0)
        {
            Shortcut = shortcut;
            ShortcutIdx = shortcutIdx;
        }
    }

    public class ShortcutController
    {
        private IntPtr hookID = IntPtr.Zero;
        private NativeMethods.LowLevelKeyboardProc proc;
        private List<Keys> currentKeys = new List<Keys>();
        private List<List<Keys>> shortcutKeys = new List<List<Keys>>();
        private bool settingShortcut = false;
        private int settingShortcutIdx = 0;
        private const int MAX_SHORTCUTS = 4;

        public delegate void OnShortcutHandler(object source, OnShortcutSetArgs args);
        public event OnShortcutHandler OnShortcutSetEvent;
        public event OnShortcutHandler OnShortcutMessage;
        public event OnShortcutHandler OnShortcutCallEvent;

        public ShortcutController()
        {
            proc = HookCallback;
            if (hookID == IntPtr.Zero)
            {
                hookID = SetHook(proc);
            }
            shortcutKeys = ConfigManager.ShortcutKeys;
            for (int i = shortcutKeys.Count; i < MAX_SHORTCUTS; i++)
            {
                shortcutKeys.Add(new List<Keys>());
            }
        }

        public void SetShortcut(int ShortcutIndex = 0)
        {
            settingShortcutIdx = ShortcutIndex;
            settingShortcut = true;
        }
        public string GetShortcut(int ShortcutIndex = 0)
        {
            return string.Join(" + ", shortcutKeys[ShortcutIndex]);
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

                if (wParam == (IntPtr)NativeMethods.WM_KEYDOWN || wParam == (IntPtr)NativeMethods.WM_SYSKEYDOWN)
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
                else if (wParam == (IntPtr)NativeMethods.WM_KEYUP || wParam == (IntPtr)NativeMethods.WM_SYSKEYUP)
                {
                    if (settingShortcut)
                    {
                        if (currentKeys.Count >= 2)
                        {
                            shortcutKeys[settingShortcutIdx] = new List<Keys>(currentKeys); // save shortcut
                            ConfigManager.ShortcutKeys = shortcutKeys;
                            settingShortcut = false;
                            currentKeys.Clear();
                            if (OnShortcutSetEvent != null)
                            {
                                OnShortcutSetEvent(this, new OnShortcutSetArgs(string.Join(" + ", shortcutKeys[settingShortcutIdx]), settingShortcutIdx)); // Raise the event
                                OnShortcutMessage(this, new OnShortcutSetArgs("Success"));
                            }
                            return (IntPtr)1;
                        }
                        else
                        {
                            OnShortcutMessage(this, new OnShortcutSetArgs("Shortcut enter failed: Enter at least 2 keys"));
                            OnShortcutSetEvent(this, new OnShortcutSetArgs("No hotkey Set", settingShortcutIdx));
                            settingShortcut = false;
                            currentKeys.Clear();
                            return (IntPtr)1;
                        }    
                    }
                    else
                    {
                        //if (shortcutKeys.SequenceEqual(shortcutKeys.Intersect(currentKeys)))
                        foreach (var sk in shortcutKeys)
                        {
                            if (sk.Count > 1 && sk.SequenceEqual(sk.Intersect(currentKeys)))
                            {
                                currentKeys.Clear();
                                OnShortcutCallEvent(this, new OnShortcutSetArgs("", shortcutKeys.IndexOf(sk)));
                            }
                        }
                        currentKeys.Clear();
                        return (IntPtr)1;
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
