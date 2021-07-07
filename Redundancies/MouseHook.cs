using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Timers;
using System.Windows;
using System.Windows.Threading;

public class MouseHook
{
    private DoubleClickChecker checker = new DoubleClickChecker();
    private readonly LowLevelMouseProc _proc;
    private IntPtr _hookID = IntPtr.Zero;

    public MouseHook(EventHandler MouseAction)
    {
        //this.MouseAction = MouseAction;
        _proc = HookCallback;
        _hookID = SetHook(_proc);
    }

    //public event EventHandler MouseAction;


    private IntPtr SetHook(LowLevelMouseProc proc)
    {
        using (Process curProcess = Process.GetCurrentProcess())
        using (ProcessModule curModule = curProcess.MainModule)
        {
            return SetWindowsHookEx(WH_MOUSE_LL, proc, IntPtr.Zero, 0);
        }
    }

    private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

    [Flags]
    private enum MouseEventFlags
    {
        LEFTDOWN = 0x00000002,
        LEFTUP = 0x00000004,
        MIDDLEDOWN = 0x00000020,
        MIDDLEUP = 0x00000040,
        MOVE = 0x00000001,
        ABSOLUTE = 0x00008000,
        RIGHTDOWN = 0x00000008,
        RIGHTUP = 0x00000010
    }
    private enum MouseMessages
    {
        WM_LBUTTONDOWN = 0x0201,
        WM_LBUTTONUP = 0x0202,
        WM_MOUSEMOVE = 0x0200,
        WM_MOUSEWHEEL = 0x020A,
        WM_RBUTTONDOWN = 0x0204,
        WM_RBUTTONUP = 0x0205
    }

    private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && MouseMessages.WM_LBUTTONDOWN == (MouseMessages)wParam)
        {
            MSLLHOOKSTRUCT hookStruct = (MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(MSLLHOOKSTRUCT));
            if (checker.Check())
            {
                Console.WriteLine("Check: true");
                TextSelectionReader tsr = new TextSelectionReader();
                string text = tsr.TryGetSelectedTextFromActiveControl();
                Console.WriteLine(text);
                //MouseAction(null, new EventArgs());
            }
            else
            {
                Console.WriteLine("Check: false");
            }
        }
        return CallNextHookEx(_hookID, nCode, wParam, lParam);
    }

    private const int WH_MOUSE_LL = 14;

    [StructLayout(LayoutKind.Sequential)]
    private struct POINT
    {
        public int x;
        public int y;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public POINT pt;
        public uint mouseData;
        public uint flags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [DllImport("user32.dll")]
    static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr SetWindowsHookEx(int idHook,
        LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
        IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr GetModuleHandle(string lpModuleName);


    private class DoubleClickChecker
    {
        private bool state = false;
       // private Timer timer = new Timer();
        private DispatcherTimer timer = new DispatcherTimer();
        private double TimerDelay = (double)GetDoubleClickTime();

        public DoubleClickChecker()
        {
            //timer.Elapsed += Reset;
            //timer.Interval = TimerDelay;

            timer.Tick += new EventHandler(Reset);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 1000);
        }

        public bool Check()
        {
            Console.WriteLine(state);
            if (state == false)
            {
                Console.WriteLine("Start");
                state = true;
                timer.Start();
                return false;
            }
            else
            {
                if (timer.IsEnabled)
                {
                    Console.WriteLine("Stop");
                    state = false;
                    timer.Stop();
                    return true;
                } else { Console.WriteLine("Is not enabled"); } 
            }
            return false;
        }

        private void Reset(object sender, EventArgs e)
        {
            Console.WriteLine("Reset");
            timer.Stop();
            state = false;
        }
        
        //private void Reset(object sender, ElapsedEventArgs e)
        //{
        //    timer.Stop();
        //    state = false;
        //}

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern int GetDoubleClickTime();
    }

}