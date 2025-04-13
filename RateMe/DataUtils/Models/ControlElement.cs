using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Windows;

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
                OnGradesUpdated();
                NotifyPropertyChanged();
            }
        }

        public double Grade
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
        private double _weight;
        private double _grade;


        public ControlElement(string name, double weight) 
        {
            _name = name;
            _weight = weight;
            _grade = 0;
        }


        public event GradesUpdatedHandler GradesUpdated;

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
}
