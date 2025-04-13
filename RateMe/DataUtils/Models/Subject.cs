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
        public int[] Modules { get; }
        public bool IsNis { get; }
        public Formula FormulaObj { get; set; }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public int Credits
        {
            get => _credits;
            set
            {
                _credits = value < 0 ? 0 : value > 15 ? 15 : value;
                NotifyPropertyChanged();
            }
        }

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

        public double Score
        {
            get => _score;
            set
            {
                _score = value;
                NotifyPropertyChanged();
            }
        }


        
        private string _name;
        private int _credits;
        private bool _isSelected;
        private double _score;
        private Visibility _visibility;
        private Dictionary<string, string> _assFormulas;


        public Subject(string name, int credits, int[] modules, Dictionary<string,string> assFormulas)
        {
            Name = name;
            _credits = credits;
            _score = 0;
            Modules = modules;
            _assFormulas = assFormulas;
            IsNis = Name.ToLower().Contains("научно-исследовательский семинар");
            _isSelected = true;
            _visibility = Visibility.Visible;
        }

        public void UpdateScore()
        {
            double score = 0;

            foreach (ControlElement elem in FormulaObj)
            {
                score += elem.Weight * elem.Grade;
            }

            Score = Math.Round(score, 2);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetFormula(int module)
        {
            bool success = SetFormulaForModule(module);
            if (success)
            {
                return;
            }

            bool success2 = SetFormulaForModule(module+1);
            if (success2)
            {
                return;
            }

            bool success3 = SetFormulaForModule(module+2);
            if (success3)
            {
                return;
            }

            bool success4 = SetFormulaForModule(module-1);
        }


        private bool SetFormulaForModule(int module)
        {
            foreach ((string name, string val) in _assFormulas)
            {
                if (FindModuleName(name, module))
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

            if (FormulaObj == null)
            {
                return false;
            }

            foreach (ControlElement elem in FormulaObj)
            {
                elem.GradesUpdated += UpdateScore;
            }

            UpdateScore();
            return true;
        }

        private static bool FindModuleName(string formulaName, int module)
        {
            return formulaName.Contains($"{module}st") || formulaName.Contains($"{module}nd") || formulaName.Contains($"{module}rd") || formulaName.Contains($"{module}th");
        }


        public override string ToString()
        {
            return $"{Name} Модули: {string.Join(' ', Modules)}";
        }
    }
}
