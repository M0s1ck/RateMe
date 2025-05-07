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
                bool good = decimal.TryParse(w, out decimal weight);
                if (good && element != null)
                {
                    element.Weight = weight;
                }
            }
        }

        private void OnGradeChanged(object sender, TextChangedEventArgs e)
        {
            if (sender != null)
            {
                ControlElement? element = ((FrameworkElement)sender).DataContext as ControlElement;
                string g = ((TextBox)sender).Text;
                bool good = decimal.TryParse(g, out decimal grade);
                if (good && element != null)
                {
                    element.Grade = grade;
                    ((TextBox)sender).Text = grade == 0 ? "0": grade.ToString();
                }
            }
        }
    }
}
