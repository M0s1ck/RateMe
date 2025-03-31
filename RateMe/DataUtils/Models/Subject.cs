using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateMe.DataUtils.Models
{
    public class Subject : INotifyPropertyChanged
    {
        public string Name { get; }
        public int Credits { get; }
        public int[] Modules { get; }
        public bool IsNis { get; }
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isSelected;
        private Dictionary<string, string> _assFormulas;


        public Subject(string name, int credits, int[] modules, Dictionary<string,string> assFormulas)
        {
            Name = name;
            Credits = credits;
            Modules = modules;
            _assFormulas = assFormulas;
            IsNis = Name.ToLower().Contains("научно-исследовательский семинар");
            _isSelected = true;
        }
        

        public event PropertyChangedEventHandler? PropertyChanged;


        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public override string ToString()
        {
            return $"{Name} Модули: {string.Join(' ', Modules)}";
        }
    }
}
