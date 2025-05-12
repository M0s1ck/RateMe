using RateMe.Models.ClientModels;
using System.Windows;
using System.Windows.Controls;

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
                if (element != null)
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
                    ((TextBox)sender).Text = grade == 0 ? "0" : grade.ToString();
                }
            }
        }
    }
}
