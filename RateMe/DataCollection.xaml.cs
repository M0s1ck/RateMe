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
using RateMe.DataUtils.InterfaceCollections;

namespace RateMe
{
    /// <summary>
    /// Логика взаимодействия для DataCollection.xaml
    /// </summary>
    public partial class DataCollection : Window
    {
        public DataCollection()
        {
            try
            {
                InitializeComponent();
                Curriculums curriculums = new Curriculums();
                CurriculumsComboBox.ItemsSource = curriculums;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
           
        }

        private void OnWindowClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show($"{NameTextBox.Data} {SurameTextBox.Data} {CurriculumsComboBox.Text}");
            Keyboard.ClearFocus();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
