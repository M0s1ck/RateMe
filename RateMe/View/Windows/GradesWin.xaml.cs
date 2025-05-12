using RateMe.View.UserControls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using RateMe.Models.ClientModels;
using RateMe.Models.JsonModels;
using RateMe.Models.LocalDbModels;

namespace RateMe.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для Grades.xaml
    /// </summary>
    public partial class GradesWin : Window
    {
        private ObservableCollection<Subject> Subjects { get; } = [];
        private SyllabusModel _syllabus;

        private readonly SubjectsContext localDb = new SubjectsContext();

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

                localDb.Add(subject.LocalModel);
            }

            _syllabus = syllabus;
            grades.ItemsSource = Subjects;
        }

        public GradesWin(SyllabusModel syllabus) : this(syllabus, []) 
        { }


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

            await localDb.SaveChangesAsync();
            Close();
        }

        private void LoadSubjectsFromLocalDb()
        {
            List <SubjectLocal> subjectLocals = localDb.Subjects.Include(s => s.Elements).ToList();
            
            foreach (SubjectLocal subjLocal in subjectLocals)
            {
                Subject subj = new(subjLocal);
                Subjects.Add(subj);
            }
            
            // Log to config
            Config config = JsonModelsHandler.GetConfig();
            config.IsSubjectsLoaded = true;
            JsonModelsHandler.SaveConfig(config);
        }
        
        private void OnAddSubject(object sender, MouseButtonEventArgs e)
        {
            Subject subject = new();
            Subjects.Add(subject);
            localDb.Add(subject.LocalModel);
            
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
            localDb.Remove(subject.LocalModel);
        }


        private void OnAccountClick(object sender, RoutedEventArgs e)
        {
            AuthWin authWin = new();
            authWin.Show();
        }


        private void OnRedoClick(object sender, RoutedEventArgs e)
        {
            RedoWin redo = new RedoWin();
            redo.RedoAgreed += Redo;
            redo.Show();
        }

        private async Task Redo(bool withSave)
        {
            if (!withSave)
            {
                foreach (Subject subject in Subjects)
                {
                    Subjects.Remove(subject);
                    localDb.Remove(subject.LocalModel);
                }
            }
            else
            {
                foreach (Subject subject in Subjects)
                {
                    subject.UpdateLocalModel();
                }
            }

            await localDb.SaveChangesAsync();
            Close();
            DataCollection dataWin = new();
            dataWin.Show();
            
            // Log to config
            Config config = JsonModelsHandler.GetConfig();
            config.IsSubjectsLoaded = false;
            JsonModelsHandler.SaveConfig(config);
        }

        private void OnInfoClick(object sender,RoutedEventArgs e)
        {
            InfoWin infoWin = new();
            infoWin.Show();
        }
    }
}
