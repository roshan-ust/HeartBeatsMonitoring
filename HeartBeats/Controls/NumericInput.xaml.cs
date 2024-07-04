using System;
using System.Windows;
using System.Windows.Controls;

namespace HeartBeats.Controls
{
    public partial class NumericInput : UserControl
    {
        public NumericInput()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
            "Value", typeof(int), typeof(NumericInput), new FrameworkPropertyMetadata(0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnValueChanged, CoerceValue));

        public int Value
        {
            get { return (int)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericInput)d;
            control.CoerceValue(ValueProperty);
        }

        private static object CoerceValue(DependencyObject d, object baseValue)
        {
            var control = (NumericInput)d;
            var value = (int)baseValue;

            if (value < control.MinValue)
                return control.MinValue;
            if (value > control.MaxValue)
                return control.MaxValue;

            return value;
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
            "MinValue", typeof(int), typeof(NumericInput), new PropertyMetadata(int.MinValue, OnMinValueChanged));

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        private static void OnMinValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericInput)d;
            control.CoerceValue(ValueProperty);
        }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
            "MaxValue", typeof(int), typeof(NumericInput), new PropertyMetadata(int.MaxValue, OnMaxValueChanged));

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        private static void OnMaxValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericInput)d;
            control.CoerceValue(ValueProperty);
        }

        private void IncreaseValue(object sender, RoutedEventArgs e)
        {
            Value++;
        }

        private void DecreaseValue(object sender, RoutedEventArgs e)
        {
            Value--;
        }

        private void NumericOnlyInput(object sender, System.Windows.Input.TextCompositionEventArgs e)
        {
            foreach (char c in e.Text)
            {
                if (!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void NumericTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!int.TryParse(NumericTextBox.Text, out int newValue))
                NumericTextBox.Text = Value.ToString();
            else
                Value = newValue;
        }

        private void NumericTextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Up)
                IncreaseValue(sender, e);
            else if (e.Key == System.Windows.Input.Key.Down)
                DecreaseValue(sender, e);
        }
    }
}
