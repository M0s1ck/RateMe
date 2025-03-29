using RateMe.DataUtils.Models;
using RateMe.View.UserControls;
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
using System.Windows.Shapes;

namespace RateMe
{
    /// <summary>
    /// Логика взаимодействия для SubjectsWin.xaml
    /// </summary>
    public partial class SubjectsWin : Window
    {
        private readonly SyllabusModel Syllabus;
        private readonly List<Subject> Subjects; 

        internal SubjectsWin(SyllabusModel syllabus, List<Subject> subjects)
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            WindowGrid.Children.Add(bar);

            Subjects = subjects;
            Syllabus = syllabus;

            foreach (Subject subject in Subjects)
            {
                CheckBox subjOption = new CheckBox();
                subjOption.Content = subject.Name;
                subjOption.Foreground = Brushes.White;
                subjOption.HorizontalContentAlignment = HorizontalAlignment.Left;
                subjOption.VerticalContentAlignment = VerticalAlignment.Top;
                subjOption.FontSize = 12.5;
                subjOption.FontFamily = new FontFamily("Bahnschrift SemiCondensed");

                ScaleTransform scale = new ScaleTransform(1.5, 1.5);
                subjOption.RenderTransformOrigin = new Point(0.5, 0.5); // 1 / 1.5 ?
                subjOption.RenderTransform = scale;

                subjOptions.Items.Add(subjOption);

            }
        }


        private void OnWindowClick(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }


    }
}
