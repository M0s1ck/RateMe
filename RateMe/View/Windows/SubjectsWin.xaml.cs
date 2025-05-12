using RateMe.Models.ClientModels;
using RateMe.View.UserControls;
using RateMe.View.Windows;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace RateMe
{
    /// <summary>
    /// Логика взаимодействия для SubjectsWin.xaml
    /// </summary>
    public partial class SubjectsWin : Window
    {
        internal ObservableCollection<Subject> SubjectsObs { get; private set; } = [];

        private bool _isThisModule = false;
        private bool _displayNis = true;
        private int _selectedSubjCount;

        private readonly SyllabusModel Syllabus;
        private readonly List<Subject> AllSubjects;

        internal SubjectsWin(SyllabusModel syllabus, List<Subject> subjects)
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            WindowGrid.Children.Add(bar);


            foreach (Subject subject in subjects)
            {
                SubjectsObs.Add(subject);
            }

            subjOptions.ItemsSource = SubjectsObs;
            Syllabus = syllabus;
            AllSubjects = subjects;

            _selectedSubjCount = subjects.Count;
            selectedCountTextBlock.Text = $"Выбрано предметов: {_selectedSubjCount}";
            thisModuleOnly.Content = $"Только указанный модуль ({Syllabus.Module})";
        }


        private void OnWindowClick(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }


        private void OnContinueClick(object sender, RoutedEventArgs e)
        {
            List<Subject> selectedSubjs = [];

            foreach (Subject subject in SubjectsObs)
            {
                if (subject.IsSelected)
                {
                    subject.SetFormula(Syllabus.Module);
                    selectedSubjs.Add(subject);
                }
            }

            GradesWin gradesWin = new(Syllabus, selectedSubjs);
            gradesWin.Show();
            Close();

        }


        private void OnSubjBoxChecked(object sender, RoutedEventArgs e)
        {
            _selectedSubjCount++;
            UpdateCurrentCountText();
        }


        private void OnSubjBoxUnchecked(object sender, RoutedEventArgs e)
        {
            _selectedSubjCount--;
            UpdateCurrentCountText();
        }


        private void OnThisModuleOnlyChecked(object sender, RoutedEventArgs e)
        {
            foreach (Subject subject in AllSubjects)
            {
                if (!subject.Modules.Contains(Syllabus.Module))
                {
                    subject.IsSelected = false;
                    SubjectsObs.Remove(subject);
                }
            }

            _selectedSubjCount = SubjectsObs.Where(subj => subj.IsSelected).Count();
            _isThisModule = true;
            UpdateCurrentCountText();
        }


        private void OnThisModuleOnlyUnchecked(object sender, RoutedEventArgs e)
        {
            _isThisModule = false;

            if (_displayNis)
            {
                for (int i = 0; i < AllSubjects.Count; i++)
                {
                    if (!AllSubjects[i].Modules.Contains(Syllabus.Module))
                    {
                        SubjectsObs.Insert(i, AllSubjects[i]);
                    }
                }
                
                return;
            }

            int cntNoNis = 0;

            foreach (Subject subject in AllSubjects)
            {
                if (!subject.IsNis && !subject.Modules.Contains(Syllabus.Module))
                {
                    SubjectsObs.Insert(cntNoNis, subject);
                }

                if (!subject.IsNis)
                {
                    cntNoNis++;
                }
            }
        }


        private void OnRemoveNisChecked(object sender, RoutedEventArgs e)
        {
            foreach (Subject subject in AllSubjects)
            {
                if (subject.IsNis)
                {
                    subject.IsSelected = false;
                    SubjectsObs.Remove(subject);
                }
            }

            _selectedSubjCount = SubjectsObs.Where(subj => subj.IsSelected).Count();
            _displayNis = false;
            UpdateCurrentCountText();
        }


        private void OnRemoveNisUnchecked(object sender, RoutedEventArgs e)
        {
            _displayNis = true;

            if (!_isThisModule)
            {
                for (int i = 0; i < AllSubjects.Count; i++)
                {
                    Subject subject = AllSubjects[i];

                    if (subject.IsNis)
                    {
                        SubjectsObs.Insert(i, subject);
                    }
                }

                return;
            }

            int cntModule = 0;

            foreach (Subject subject in AllSubjects)
            {
                if (subject.IsNis && subject.Modules.Contains(Syllabus.Module))
                {
                    SubjectsObs.Insert(cntModule, subject);
                }

                if (subject.Modules.Contains(Syllabus.Module))
                {
                    cntModule++;
                }
            }
        }


        private void OnclearChoicesChecked(object sender, RoutedEventArgs e)
        {
            foreach (Subject subject in SubjectsObs)
            {
                subject.IsSelected = false;
            }

            _selectedSubjCount = 0;
            UpdateCurrentCountText();
        }


        private void OnclearChoicesUnchecked(object sender, RoutedEventArgs e)
        {
            foreach (Subject subject in SubjectsObs)
            {
                if (subject.ListVisibility == Visibility.Visible)
                {
                    subject.IsSelected = true;
                }
            }

            _selectedSubjCount = SubjectsObs.Where(subj => subj.IsSelected).Count();
            UpdateCurrentCountText();
        }


        private void UpdateCurrentCountText() => selectedCountTextBlock.Text = $"Выбрано предметов: {_selectedSubjCount}";
    }
}
