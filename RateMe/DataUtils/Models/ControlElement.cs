using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace RateMe.DataUtils.Models
{
    public class ControlElement : INotifyPropertyChanged
    {
        public string Name 
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }


        public double Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                NotifyPropertyChanged();
            }
        }

        public double Grade
        {
            get => _grade;
            set
            {
                _grade = value;
                NotifyPropertyChanged();
            }
        }

        private string _name;
        private double _weight;
        private double _grade;


        public ControlElement(string name, double weight) 
        {
            _name = name;
            _weight = weight;
            _grade = 0;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        private void NotifyPropertyChanged(string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
