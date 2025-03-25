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
using System.Windows.Shapes;

namespace RateMe
{
    /// <summary>
    /// Логика взаимодействия для DataCollection.xaml
    /// </summary>
    public partial class DataCollection : Window
    {
        public DataCollection()
        {
            InitializeComponent();
        }

        private void OnWindowClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show($"{NameTextBox.Data} {SurameTextBox.Data}");
            Keyboard.ClearFocus();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
