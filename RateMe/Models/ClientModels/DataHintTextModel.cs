using System.ComponentModel;
using System.Windows;

namespace RateMe.Models.ClientModels;

public class DataHintTextModel : INotifyPropertyChanged
{
    public string Data
    {
        get => _data;
        set
        {
            _data = value;
            NotifyPropertyChanged(nameof(Data));
            NotifyPropertyChanged(nameof(HintVisibility));
        }
    }
        
    public Visibility HintVisibility => string.IsNullOrEmpty(Data) ? Visibility.Visible : Visibility.Hidden;

    public string Hint { get; }

    private string _data = string.Empty;

    public DataHintTextModel(string data, string hint)
    {
        Data = data;
        Hint = hint;
    }

    public DataHintTextModel(string hint) : this(string.Empty, hint)
    { }
        
    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}