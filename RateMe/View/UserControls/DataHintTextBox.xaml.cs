using RateMe.DataUtils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RateMe.View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для DataHintTextBox.xaml
    /// </summary>
    public partial class DataHintTextBox : UserControl
    {
        public static readonly DependencyProperty TextBoxFontSizeProp =
            DependencyProperty.Register("TextBoxFontSize", typeof(double), typeof(DataHintTextBox), new PropertyMetadata(15.0));

        public static readonly DependencyProperty TextBoxHeightProp =
            DependencyProperty.Register("TextBoxHeight", typeof(double), typeof(DataHintTextBox), new PropertyMetadata(30.0));

        public static readonly DependencyProperty TextBoxMinWidthProp =
            DependencyProperty.Register("TextBoxMinWidth", typeof(double), typeof(DataHintTextBox), new PropertyMetadata(120.0));

        public static readonly DependencyProperty TextBlockFontSizeProp =
            DependencyProperty.Register("TextBlockFontSize", typeof(double), typeof(DataHintTextBox), new PropertyMetadata(13.0));

        public double TextBoxFontSize
        {
            get => (double)GetValue(TextBoxFontSizeProp);
            set => SetValue(TextBoxFontSizeProp, value);
        }

        public double TextBoxHeight
        {
            get => (double)GetValue(TextBoxHeightProp);
            set => SetValue(TextBoxHeightProp, value);
        }

        public double TextBoxMinWidth
        {
            get => (double)GetValue(TextBoxMinWidthProp);
            set => SetValue(TextBoxMinWidthProp, value);
        }

        public double TextBlockFontSize
        {
            get => (double)GetValue(TextBlockFontSizeProp);
            set => SetValue(TextBlockFontSizeProp, value);
        }


        public DataHintTextBox()
        {
            InitializeComponent();
        }

        private void OnUpdatedText(object sender, TextChangedEventArgs e)
        {
            ((DataHintTextModel)DataContext).Data = dataTextBox.Text;
        }
    }
}
