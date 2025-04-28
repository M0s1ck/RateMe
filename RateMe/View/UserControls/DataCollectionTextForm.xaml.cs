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
    /// Логика взаимодействия для DataCollectionTextForm.xaml
    /// </summary>
    public partial class DataCollectionTextForm : UserControl
    {

        private string _hint;

        public string Hint
        {
            get { return _hint; }
            set 
            {
                _hint = value;
                DataCollectionHint.Text = _hint;
            }
        }

        private string _data;

        public string Data
        {
            get { return _data; }
            set { 
                _data = value;
            }
        }

        public DataCollectionTextForm(string hint, string data)
        {
            InitializeComponent();
            _hint = hint;
            Data = data;

            DataCollectionHint.Visibility = Visibility.Hidden;
            DataCollectionTextBox.Text = data;
        }

        public DataCollectionTextForm()
        {
            InitializeComponent();
            _hint = "";
            _data = "";
        }

        private void OnDataCollectionInput(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(DataCollectionTextBox.Text))
            {
                DataCollectionHint.Visibility = Visibility.Visible;
            }
            else
            {
                DataCollectionHint.Visibility = Visibility.Hidden;
            }

            _data = DataCollectionTextBox.Text;
        }
    }
}
