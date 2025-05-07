using System.Windows;
using System.Windows.Controls;

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

        private string _data = string.Empty;

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
