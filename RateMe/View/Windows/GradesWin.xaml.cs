using RateMe.View.UserControls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Microsoft.EntityFrameworkCore;
using RateMe.Api.Clients;
using RateMe.Api.Services;
using RateMe.Models.ClientModels;
using RateMe.Models.JsonModels;
using RateMe.Models.LocalDbModels;
using RateMe.Repositories;

namespace RateMe.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для Grades.xaml
    /// </summary>
    public partial class GradesWin : Window
    {
        private ObservableCollection<Subject> _subjects = [];
        private Dictionary<int, Subject> _subjectsToAdd = [];
        private List<int> _subjKeysToRemove = []; 
        private SyllabusModel _syllabus;
        
        private readonly SubjectsService _subjectsService;
        private readonly SubjectsContext _localDb = new SubjectsContext();

        public GradesWin(SyllabusModel syllabus, List<Subject> subjects)
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            windowGrid.Children.Add(bar);

            LoadSubjectsFromLocalDb();

            foreach (Subject subject in subjects)
            {
                _subjects.Add(subject);
                subject.LocalModel = new SubjectLocal { Name = subject.Name, Credits = subject.Credits, Elements = [] };

                foreach (ControlElement elem in subject.FormulaObj)
                {
                    subject.LocalModel.Elements.Add(elem.LocalModel);
                }

                _localDb.Add(subject.LocalModel);
            }
            
            _subjectsService = new SubjectsService(new SubjectsClient());
            grades.ItemsSource = _subjects;
            _syllabus = syllabus;
        }

        public GradesWin(SyllabusModel syllabus) : this(syllabus, []) 
        { }
        
        private void OnWindowClick(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private async void OnSaveAndQuitClick(object sender, RoutedEventArgs e)
        {
            foreach (Subject subject in _subjects)
            {
                subject.UpdateLocalModel();
            }

            await _localDb.SaveChangesAsync();
            int userId = JsonModelsHandler.GetUserId();
            await _subjectsService.PushSubjectsByUserId(userId, _subjectsToAdd);
            await _subjectsService.RemoveSubjectsByKeys(_subjKeysToRemove);
            Close();
        }

        private void LoadSubjectsFromLocalDb()
        {
            List <SubjectLocal> subjectLocals = _localDb.Subjects.Include(s => s.Elements).ToList();
            
            foreach (SubjectLocal subjLocal in subjectLocals)
            {
                Subject subj = new(subjLocal);
                _subjects.Add(subj);
            }
            
            // Log to config
            Config config = JsonModelsHandler.GetConfig();
            config.IsSubjectsLoaded = true;
            JsonModelsHandler.SaveConfig(config);
        }
        
        private void OnAddSubject(object sender, MouseButtonEventArgs e)
        {
            Subject subject = new();
            _subjects.Add(subject);
            _localDb.Add(subject.LocalModel);
            _subjectsToAdd[subject.LocalModel.SubjectId] = subject;
            
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
            _subjects.Remove(subject);
            
            if (!_subjectsToAdd.ContainsKey(subject.LocalModel.SubjectId))
            {
                _subjKeysToRemove.Add(subject.LocalModel.RemoteId);
            }
            
            _localDb.Remove(subject.LocalModel);
            _subjectsToAdd.Remove(subject.LocalModel.SubjectId);
        }


        private async void OnAccountClick(object sender, RoutedEventArgs e)
        {
            AuthWin authWin = new(_subjectsService);
            authWin.Show();
            
            foreach (Subject subject in _subjects)
            {
                subject.UpdateLocalModel();
            }

            await _localDb.SaveChangesAsync();
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
                foreach (Subject subject in _subjects)
                {
                    _subjects.Remove(subject);
                    _localDb.Remove(subject.LocalModel);
                }
            }
            else
            {
                foreach (Subject subject in _subjects)
                {
                    subject.UpdateLocalModel();
                }
            }

            await _localDb.SaveChangesAsync();
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
