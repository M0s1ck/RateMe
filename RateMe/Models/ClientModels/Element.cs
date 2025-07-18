using System.ComponentModel;
using System.Windows;
using RateMe.Models.LocalDbModels;

namespace RateMe.Models.ClientModels;

public class Element : INotifyPropertyChanged
{
    public ElementLocal LocalModel { get; }

    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            NotifyPropertyChanged();
        }
    }

    public decimal Weight
    {
        get => _weight;
        set
        {
            _weight = value;
            OnGradesUpdated();
            NotifyPropertyChanged();
        }
    }

    public decimal Grade
    {
        get => _grade;
        set
        {
            _grade = value;
            OnGradesUpdated();
            NotifyPropertyChanged();
        }
    }

    public CornerRadius ViewBorderRadius { get; set; } = new CornerRadius();
    public Thickness ViewBorderThickness { get; set; } = new Thickness(1, 2, 1, 2);

    private string _name;
    private decimal _weight;
    private decimal _grade;

    public Element()
    {
        _name = "Элемент контроля";
        _weight = 0;
        _grade = 0;
        LocalModel = new ElementLocal { Name = _name, Weight = _weight, Grade = _grade };
    }

    public Element(string name, decimal weight)
    {
        _name = name;
        _weight = weight;
        _grade = 0;
        LocalModel = new ElementLocal { Name = _name, Weight = _weight, Grade = _grade };
    }

    public Element(ElementLocal elemLocal)
    {
        _name = elemLocal.Name;
        _weight = elemLocal.Weight;
        _grade = elemLocal.Grade;
        LocalModel = elemLocal;
    }

    public Element(Element other)
    {
        _name = other.Name;
        _name = other.Name;
        _weight = other.Weight;
        _grade = other.Grade;
        LocalModel = other.LocalModel;
    }

    public void UpdateLocalModel()
    {
        LocalModel.Name = Name;
        LocalModel.Grade = Grade;
        LocalModel.Weight = Weight;
    }

    public event GradesUpdatedHandler? GradesUpdated;

    public delegate void GradesUpdatedHandler();

    private void OnGradesUpdated()
    {
        GradesUpdated?.Invoke();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void NotifyPropertyChanged(string propertyName = "")
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}