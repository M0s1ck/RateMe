using RateMe.DataUtils.Models;
using RateMe.View.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

namespace RateMe.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для Grades.xaml
    /// </summary>
    public partial class GradesWin : Window
    {
        internal ObservableCollection<Subject> Subjects { get; private set; } = [];
        private SyllabusModel _syllabus;

        public GradesWin(SyllabusModel syllabus, List<Subject> subjects)
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            windowGrid.Children.Add(bar);

            foreach (Subject subject in subjects)
            {
                Subjects.Add(subject);
            }

            _syllabus = syllabus;

            grades.ItemsSource = Subjects;

        }


        private void OnWindowClick(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }


        private void OnEditGearClick(object sender, MouseButtonEventArgs e)
        {
            Subject? subject = ((FrameworkElement)sender)?.DataContext as Subject;

            if (subject == null)
            {
                MessageBox.Show("Null Subject");
                return;
            }

            // MessageBox.Show(subject.Name);
            
            SubjectEditWin subjWin = new SubjectEditWin(subject);
            subjWin.Show();
            subjWin.Activate();
        }
    }
}
