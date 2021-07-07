using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace SewoTranslator.Behavior
{
    public sealed class CaretIndexBehavior : Behavior<UIElement>
    {
        private TextBox textBox;

        protected override void OnAttached()
        {
            base.OnAttached();

            textBox = AssociatedObject as TextBox;

            if (textBox == null)
            {
                return;
            }
            
            textBox.GotKeyboardFocus += SetCaretIndex;
        }
        
        protected override void OnDetaching()
        {
            if (textBox == null)
            {
                return;
            }
            textBox.GotKeyboardFocus -= SetCaretIndex;
            base.OnDetaching();
        }

        private void SetCaretIndex(object sender, RoutedEventArgs routedEventArgs)
        {
            textBox.CaretIndex = textBox.Text.Length;
        }
    }
}
