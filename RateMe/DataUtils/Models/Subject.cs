using System.ComponentModel;
using System.Windows;
using RateMe.DataUtils.LocalDbModels;

namespace RateMe.DataUtils.Models
{
    /// <summary>
    /// The main model for the desktop app.
    /// Holds and updates model for local db.
    /// </summary>
    public class Subject : INotifyPropertyChanged
    {
        public int[] Modules { get; } = [];
        public bool IsNis { get; }
        public Formula FormulaObj { get; set; }
        public SubjectLocal LocalModel { get; set; } 

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

        public decimal Score
        {
            get => _score;
            set
            {
                _score = value;
                NotifyPropertyChanged();
            }
        }


        private string _name = string.Empty;
        private int _credits;
        private bool _isSelected;
        private decimal _score;
        private Visibility _visibility;
        private readonly Dictionary<string, string> _assFormulas = [];


        public Subject()
        {
            _name = string.Empty;
            FormulaObj = [];
            LocalModel = new() {Name = this.Name };
        }
        
        // From hse site. 
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
            FormulaObj = [];
            LocalModel = new SubjectLocal { Name = this.Name, Credits = this.Credits, Elements = [] };
        }
        
        // From local db. 
        public Subject(SubjectLocal localSubj)
        {
            Name = localSubj.Name;
            Credits = localSubj.Credits;
            FormulaObj = [];

            foreach (ControlElementLocal elemLocal in localSubj.Elements)
            {
                ControlElement elem = new ControlElement(elemLocal);
                FormulaObj.Add(elem);
                elem.GradesUpdated += UpdateScore;
            }
            
            UpdateScore();
            LocalModel = localSubj;
        }

        public void UpdateLocalModel()
        {
            LocalModel.Name = Name;
            LocalModel.Credits = Credits;

            foreach (ControlElement elem in FormulaObj)
            {
                elem.UpdateLocalModel();
            }
        }

        public void UpdateScore()
        {
            decimal score = 0;

            foreach (ControlElement elem in FormulaObj)
            {
                score += elem.Weight * elem.Grade;
            }

            Score = score;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetFormula(int module)
        {
            _ = SetFormulaForModule(module) || SetFormulaForModule(module+1) || SetFormulaForModule(module+2) || SetFormulaForModule(module+3)
                || SetFormulaForModule(module - 1) || SetFormulaForModule(module - 2) || SetFormulaForModule(module - 3);
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

            if (FormulaObj.Count == 0)
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
            return $"{Name} subject";
        }
    }
}
