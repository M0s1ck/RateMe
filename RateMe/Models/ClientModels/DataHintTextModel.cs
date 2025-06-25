using System.ComponentModel;
using System.Windows;

namespace RateMe.Models.ClientModels
{
    public class DataHintTextModel : INotifyPropertyChanged
    {
        public string Data
        {
            get => _data;
            set
            {
                //MessageBox.Show($"Being set with value={value}");
                _data = value;

                if (value == string.Empty)
                {
                    HintVisibility = Visibility.Visible;
                }
                else
                {
                    HintVisibility = Visibility.Hidden;
                }
                NotifyPropertyChanged();
            }
        }

        public Visibility HintVisibility
        {
            get => _hintVisibility;
            set
            {
                _hintVisibility = value;
                NotifyPropertyChanged();
            }
        }

        public string Hint { get; }

        private string _data = string.Empty;
        private Visibility _hintVisibility = Visibility.Visible;


        public DataHintTextModel(string data, string hint, Visibility hintVisibility=Visibility.Visible)
        {
            Data = data;
            Hint = hint;
            HintVisibility = hintVisibility;
        }

        public DataHintTextModel(string hint) : this(string.Empty, hint, Visibility.Visible)
        { }
        
        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
