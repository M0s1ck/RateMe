using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace RateMe.DataUtils.Models
{
    public class Subject : INotifyPropertyChanged
    {
        public string Name { get; }
        public int Credits { get; }
        public int[] Modules { get; }
        public bool IsNis { get; }

        public Visibility ListVisibility
        {
            get => _visibility;
            set
            {
                _visibility = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        // Temp
        public ObservableCollection<ControlElement> FormulaObj1 { get; } = [new ControlElement("Кр1", 0.2), new ControlElement("Кр2", 0.3)];

        public Formula FormulaObj { get; set; }

        private bool _isSelected;
        private Visibility _visibility;
        private Dictionary<string, string> _assFormulas;



        public Subject(string name, int credits, int[] modules, Dictionary<string,string> assFormulas)
        {
            Name = name;
            Credits = credits;
            Modules = modules;
            _assFormulas = assFormulas;
            IsNis = Name.ToLower().Contains("научно-исследовательский семинар");
            _isSelected = true;
            _visibility = Visibility.Visible;
        }
        

        public event PropertyChangedEventHandler? PropertyChanged;


        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void SetFormula(int module)
        {
            foreach ((string name, string val) in _assFormulas)
            {
                if (name.Contains($"{module}st") || name.Contains($"{module}nd") || name.Contains($"{module}rd") || name.Contains($"{module}th"))
                {
                    try
                    {
                        FormulaObj = new Formula(val);
                    }
                    catch (ArgumentOutOfRangeException e)
                    {
                        Console.WriteLine(e);
                    }

                    break;
                }
            }
        }


        public override string ToString()
        {
            return $"{Name} Модули: {string.Join(' ', Modules)}";
        }
    }
}
