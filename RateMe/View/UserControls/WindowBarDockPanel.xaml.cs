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
    /// Логика взаимодействия для WindowBarDockPanel.xaml
    /// </summary>
    public partial class WindowBarDockPanel : UserControl
    {
        private readonly Window _window;

        public WindowBarDockPanel(Window window)
        {
            InitializeComponent();
            _window = window;
        }


        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            _window.Close();
        }

        private void OnExpandButtonClick(object sender, RoutedEventArgs e)
        {
            if (_window.WindowState == WindowState.Normal)
            {
                _window.WindowState = WindowState.Maximized;
            }
            else if (_window.WindowState == WindowState.Maximized)
            {
                _window.WindowState = WindowState.Normal;
            }
        }


        private void OnWrapButtonClick(object sender, RoutedEventArgs e)
        {
            _window.WindowState = WindowState.Minimized;
            
        }


        private void OnButtonEnter(object sender, MouseEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            Button button = (Button)sender;

            button.Background = new SolidColorBrush(Color.FromRgb(0x00, 0x00, 0xd2));
            button.BorderBrush = Brushes.White;
        }

        
        private void OnButtonLeave(object sender, MouseEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            Button button = (Button)sender;

            button.Background = Brushes.Transparent;
            button.BorderBrush = Brushes.Transparent;
        }
    }
}
