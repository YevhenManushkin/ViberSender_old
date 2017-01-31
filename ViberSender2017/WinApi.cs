namespace ViberSender2017
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    using System.Windows.Automation;
    using System.Windows.Forms;

    internal static class WinApi
    {
        private static AutomationElement button = null;
        private static IntPtr hwnd;
        public static uint MK_LBUTTON = 1;
        private static AutomationElement number_edit = null;
        private static Random r = new Random();
        public static uint VK_BACK = 8;
        public static uint VK_CONTROL = 0x11;
        public static uint VK_LCONTROL = 0xa2;
        public static uint VK_RETURN = 13;
        public static uint WM_CHAR = 0x102;
        public static uint WM_CLEAR = 0x303;
        public static uint WM_CLOSE = 0x10;
        public static uint WM_KEYDOWN = 0x100;
        public static uint WM_KEYUP = 0x101;
        public static uint WM_LBUTTONDOWN = 0x201;
        public static uint WM_LBUTTONUP = 0x202;
        public static uint WM_MOUSEACTIVATE = 0x21;
        public static uint WM_MOUSEMOVE = 0x200;
        public static uint WM_PASTE = 770;

        public static void ClickMessage()
        {
            SendMessage(hwnd, WM_MOUSEMOVE, IntPtr.Zero, MakeLParam(0xc7, 0x1dd));
            Thread.Sleep(200);
            SendMessage(hwnd, WM_LBUTTONDOWN, (IntPtr) MK_LBUTTON, MakeLParam(0xc7, 0x1dd));
            SendMessage(hwnd, WM_LBUTTONUP, IntPtr.Zero, MakeLParam(0xc7, 0x1dd));
            Thread.Sleep(200);
        }

        public static void ClickNumber()
        {
            try
            {
                SendMessage(FindWindow("Qt5QWindowIcon", "Viber"), WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                SetForegroundWindow(hwnd);
                SendCtrlhotKey('D');
            }
            catch
            {
            }
        }

        public static void EnterNumber(string number)
        {
            while (true)
            {
                try
                {
                    InvokePattern currentPattern = number_edit.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
                    currentPattern.Invoke();
                    SendCtrlhotKey('A');
                    Clipboard.SetText(number);
                    currentPattern.Invoke();
                    SendCtrlhotKey('V');
                    Thread.Sleep(100);
                    return;
                }
                catch
                {
                    SendMessage(FindWindow("Qt5QWindowIcon", "Viber"), WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                    AutomationElement element = AutomationElement.FromHandle(FindWindow("Qt5QWindowOwnDCIcon", null));
                    ControlType byId = ControlType.LookupById(0xc354);
                    number_edit = element.FindFirst(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, byId));
                    SendCtrlhotKey('D');
                }
            }
        }

        [DllImport("user32.dll", SetLastError=true)]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);
        private static int GetTextBoxTextLength(IntPtr hTextBox)
        {
            uint msg = 14;
            return SendMessage4(hTextBox, msg, 0, 0);
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern int GetWindowTextLength(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError=true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, int dwExtraInfo);
        public static IntPtr MakeLParam(int LoWord, int HiWord) =>
            ((IntPtr)((HiWord << 0x10) | (LoWord & 0xffff)));

        [DllImport("user32.dll")]
        public static extern uint MapVirtualKey(uint uCode, uint uMapType);
        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        private static void SendCtrlhotKey(char key)
        {
            keybd_event(0x11, 0, 0, 0);
            keybd_event((byte) key, 0, 0, 0);
            keybd_event((byte) key, 0, 2, 0);
            keybd_event(0x11, 0, 2, 0);
        }

        private static void SendFile(string path)
        {
            string[] files = Directory.GetFiles(path);
            path = files[r.Next(0, files.Length)];
            Thread.Sleep(300);
            IntPtr hWnd = FindWindow("Qt5QWindowOwnDCIcon", null);
            SendMessage(hWnd, WM_MOUSEMOVE, IntPtr.Zero, MakeLParam(0x14f, 0x220));
            Thread.Sleep(100);
            SendMessage(hWnd, WM_LBUTTONDOWN, (IntPtr) MK_LBUTTON, MakeLParam(0x14f, 0x220));
            Thread.Sleep(50);
            SendMessage(hWnd, WM_LBUTTONUP, IntPtr.Zero, MakeLParam(0x14f, 0x220));
            Thread.Sleep(500);
            IntPtr parentHandle = FindWindow(null, "Отправить файл");
            if (parentHandle == IntPtr.Zero)
            {
                parentHandle = FindWindow(null, "Send a File");
            }
            while (parentHandle != IntPtr.Zero)
            {
                IntPtr ptr3 = FindWindowEx(FindWindowEx(FindWindowEx(parentHandle, IntPtr.Zero, "ComboBoxEx32", ""), IntPtr.Zero, "ComboBox", ""), IntPtr.Zero, "Edit", "");
                Clipboard.SetText(path);
                SendMessage(ptr3, WM_CLEAR, IntPtr.Zero, IntPtr.Zero);
                SendMessage(ptr3, WM_PASTE, IntPtr.Zero, IntPtr.Zero);
                SendMessage(FindWindowEx(parentHandle, IntPtr.Zero, "Button", null), 0xf5, IntPtr.Zero, IntPtr.Zero);
                Thread.Sleep(100);
                parentHandle = FindWindow(null, "Отправить файл");
                if (parentHandle == IntPtr.Zero)
                {
                    parentHandle = FindWindow(null, "Send a File");
                }
                if (parentHandle != IntPtr.Zero)
                {
                    SetForegroundWindow(parentHandle);
                    SendKeys.SendWait("~");
                }
            }
        }

        [DllImport("user32.dll", CharSet=CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", EntryPoint="SendMessage", CharSet=CharSet.Auto)]
        private static extern int SendMessage4(IntPtr hwndControl, uint Msg, int wParam, int lParam);
        public static bool SendMsg(string text, string path = null, bool flag = false)
        {
            SendMessage(FindWindow("Qt5QWindowIcon", "Viber"), WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            if (flag)
            {
                SendFile(path);
            }
            if (!string.IsNullOrEmpty(text))
            {
                Clipboard.SetText(text);
            }
            SetForegroundWindow(hwnd);
            Thread.Sleep(50);
            SendMessage(hwnd, WM_MOUSEMOVE, (IntPtr) MK_LBUTTON, MakeLParam(0x1d3, 0x222));
            SendMessage(hwnd, WM_LBUTTONDOWN, (IntPtr) MK_LBUTTON, MakeLParam(0x1d3, 0x222));
            SendMessage(hwnd, WM_LBUTTONUP, IntPtr.Zero, MakeLParam(0x1d3, 0x222));
            SendCtrlhotKey('V');
            Thread.Sleep(50);
            SendMessage(FindWindow("Qt5QWindowIcon", "Viber"), WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            SendMessage(hwnd, WM_KEYDOWN, (IntPtr) VK_RETURN, (IntPtr) ((MapVirtualKey(VK_RETURN, 0) << 0x10) | 1));
            Thread.Sleep(100);
            IntPtr hWnd = FindWindow("Qt5QWindowIcon", "Viber");
            if (hWnd != IntPtr.Zero)
            {
                SendMessage(hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                return false;
            }
            return true;
        }

        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hWnd, uint nCmdShow);
        public static bool StartWork()
        {
            if (button == null)
            {
                button = AutomationElement.FromHandle(FindWindow("Qt5QWindowOwnDCIcon", null)).FindAll(TreeScope.Children, Condition.TrueCondition)[6];
            }
            if (number_edit == null)
            {
                AutomationElement element2 = AutomationElement.FromHandle(FindWindow("Qt5QWindowOwnDCIcon", null));
                ControlType byId = ControlType.LookupById(0xc354);
                number_edit = element2.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, byId));
            }
            SendMessage(FindWindow("Qt5QWindowIcon", "Viber"), WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
            hwnd = IntPtr.Zero;
            hwnd = FindWindow("Qt5QWindowOwnDCIcon", null);
            while (hwnd == IntPtr.Zero)
            {
                hwnd = FindWindow("Qt5QWindowOwnDCIcon", null);
            }
            Thread.Sleep(500);
            int capacity = GetWindowTextLength(hwnd) + 1;
            StringBuilder lpString = new StringBuilder(capacity);
            GetWindowText(hwnd, lpString, lpString.Capacity);
            string str = lpString.ToString();
            if (lpString.ToString() == "Viber")
            {
                return false;
            }
            SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 800, 600, 0x40);
            return true;
        }
    }
}

