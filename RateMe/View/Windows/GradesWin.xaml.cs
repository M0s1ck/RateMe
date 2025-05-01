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
using Microsoft.EntityFrameworkCore;
using RateMe.DataUtils.LocalDbModels;

namespace RateMe.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для Grades.xaml
    /// </summary>
    public partial class GradesWin : Window
    {
        internal ObservableCollection<Subject> Subjects { get; private set; } = [];
        private SyllabusModel _syllabus;

        private static SubjectsContext _localDb = new SubjectsContext();

        public GradesWin(SyllabusModel syllabus, List<Subject> subjects)
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            windowGrid.Children.Add(bar);

            LoadSubjectsFromLocalDb();

            foreach (Subject subject in subjects)
            {
                Subjects.Add(subject);
                subject.LocalModel = new SubjectLocal { Name = subject.Name, Credits = subject.Credits, Elements = [] };

                foreach (ControlElement elem in subject.FormulaObj)
                {
                    subject.LocalModel.Elements.Add(elem.LocalModel);
                }

                _localDb.Add(subject.LocalModel);
            }

            _syllabus = syllabus;
            grades.ItemsSource = Subjects;
        }


        private void OnWindowClick(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private async void OnSaveAndQuitClick(object sender, RoutedEventArgs e)
        {
            foreach (Subject subject in Subjects)
            {
                subject.UpdateLocalModel();
            }

            await _localDb.SaveChangesAsync();
            Close();
        }

        private void LoadSubjectsFromLocalDb()
        {
            List <SubjectLocal> subjectLocals = _localDb.Subjects.Include(s => s.Elements).ToList();
            
            foreach (SubjectLocal subjLocal in subjectLocals)
            {
                Subject subj = new(subjLocal);
                Subjects.Add(subj);
            }
        }
        
        private void OnAddSubject(object sender, MouseButtonEventArgs e)
        {
            Subject subject = new();
            Subjects.Add(subject);
            _localDb.Add(subject.LocalModel);
            
            SubjectEditWin subjWin = new SubjectEditWin(subject);
            subjWin.Show();
            subjWin.Activate();
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
        
        private void OnTrashBinClick(object sender, RoutedEventArgs e)
        {
            Subject? subject = ((FrameworkElement)sender)?.DataContext as Subject;
            
            if (subject == null)
            {
                return; 
            }

            RemoveSubjectWin win = new(subject);
            win.RemovalAgreed += RemoveSubject;
            win.Show();
        }

        private void RemoveSubject(Subject subject)
        {
            Subjects.Remove(subject);
            _localDb.Remove(subject.LocalModel);
        }

        private void OnTrashBinEnter(object sender, MouseEventArgs e)
        {
            Button trashBin = (Button)sender;
            trashBin.Height = 35;
            trashBin.Width = 35;
        }
        
        private void OnTrashBinLeave(object sender, MouseEventArgs e)
        {
            Button trashBin = (Button)sender;
            trashBin.Height = 30;
            trashBin.Width = 30;
        }
    }
}
