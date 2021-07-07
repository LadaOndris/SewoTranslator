using SewoTranslator.ViewModel;
using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;

namespace SewoTranslator.View
{
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        private const int SHOW_ID = 1;
        private const int REVERSE_ID = 2;

        private const uint VK_MULTIPLY = 0x6A;

        private IntPtr _windowHandle;
        private HwndSource _source;
        
        private MainVM mainVM;

        public MainWindow(MainVM mainVM)
        {
            InitializeComponent();
            this.mainVM = mainVM;
            DataContext = mainVM;

            Rect desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width - 5;
            this.Top = desktopWorkingArea.Bottom - this.Height - 8;
            
            mainVM.Scroll += Scroll;
            mainVM.TextBoxFocused += SetTextBoxFocus;

            this.SetTextBoxFocus();
        }

        private void Scroll(object o)
        {
            listBox.UpdateLayout();
            listBox.ScrollIntoView(o);
        }

        private void SetTextBoxFocus()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Input,
                new Action(delegate ()
                {
                    this.Activate();
                    TextBox.Focus();         // Set Logical Focus
                    Keyboard.Focus(TextBox); // Set Keyboard Focus
                }));
        }

        private new void MouseDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            _windowHandle = new WindowInteropHelper(this).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, SHOW_ID, (uint)ModifierKeys.Control, VK_MULTIPLY); // CTRL + *
            RegisterHotKey(_windowHandle, REVERSE_ID, (uint)ModifierKeys.Alt, VK_MULTIPLY); // ALT + *

            SetTextBoxFocus();

            //DispatcherTimer dispatcherTimer = new DispatcherTimer();
            //dispatcherTimer.Tick += new EventHandler(MouseHandle);
            //dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 150);
            //dispatcherTimer.Start();
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            switch (msg)
            {
                case WM_HOTKEY:
                    switch (wParam.ToInt32())
                    {
                        case SHOW_ID:
                            int vkey = (((int)lParam >> 16) & 0xFFFF);
                            if (vkey == VK_MULTIPLY)
                            {
                                mainVM.HandleInput();
                                SetCaretIndex(); // needs to set after handle input because it's needed to set text to the TextBox
                            }
                            handled = true;
                            break;
                        case REVERSE_ID:
                            mainVM.ReverseVisibility();
                            handled = true;
                            break;
                    }
                    handled = true;
                    break;
            }
            return IntPtr.Zero;
        }

        private void SetCaretIndex()
        {
            this.TextBox.CaretIndex = TextBox.Text.Length;
        }

        protected override void OnClosed(EventArgs e)
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, SHOW_ID);
            UnregisterHotKey(_windowHandle, REVERSE_ID);
            base.OnClosed(e);
        }

        private void TextBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var vm = (MainVM)this.DataContext;
                vm.ShowCurrentWordInCambridgeDictCommand.Execute(null);
            }

            // stops the beeping sound after pressing alt and pressing another key afterwards
            e.Handled = e.KeyboardDevice.Modifiers == ModifierKeys.Alt; 
        }
    }
}
