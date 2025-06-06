﻿using System.ComponentModel;
using System.Windows;
using RateMe.Models.LocalDbModels;

namespace RateMe.Models.ClientModels
{
    public class ControlElement : INotifyPropertyChanged
    {
        public ControlElementLocal LocalModel { get; set; }

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

        public ControlElement()
        {
            _name = "Элемент контроля";
            _weight = 0;
            _grade = 0;
            LocalModel = new ControlElementLocal { Name = _name, Weight = _weight, Grade = _grade };
        }

        public ControlElement(string name, decimal weight)
        {
            _name = name;
            _weight = weight;
            _grade = 0;
            LocalModel = new ControlElementLocal { Name = _name, Weight = _weight, Grade = _grade };
        }

        public ControlElement(ControlElementLocal elemLocal)
        {
            _name = elemLocal.Name;
            _weight = elemLocal.Weight;
            _grade = elemLocal.Grade;
            LocalModel = elemLocal;
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
}
