using RateMe.DataUtils.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RateMe.View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для ControlElementsTable.xaml
    /// </summary>
    public partial class ControlElementsTable : UserControl
    {
        public ControlElementsTable()
        {
            InitializeComponent();
        }

        private void OnNameChanged(object sender, TextChangedEventArgs e)
        {
            if (sender != null)
            {
                ControlElement? element = ((FrameworkElement)sender).DataContext as ControlElement;
                element.Name = ((TextBox)sender).Text;
            }
            
        }

        private void OnWeightChanged(object sender, TextChangedEventArgs e)
        {
            if (sender != null)
            {
                ControlElement? element = ((FrameworkElement)sender).DataContext as ControlElement;
                string w = ((TextBox)sender).Text;
                bool good = double.TryParse(w, out double weight);
                if (good)
                {
                    element.Weight = double.Round(weight, 4);
                }
            }
        }

        private void OnGradeChanged(object sender, TextChangedEventArgs e)
        {
            if (sender != null)
            {
                ControlElement? element = ((FrameworkElement)sender).DataContext as ControlElement;
                string g = ((TextBox)sender).Text;
                bool good = double.TryParse(g, out double grade);
                if (good)
                {
                    element.Grade = double.Round(grade, 4);
                }
            }
        }
    }
}
