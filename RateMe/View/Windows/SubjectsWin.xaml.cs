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
    public partial class SubjectsWin : BaseFullWin
    {
        internal ObservableCollection<Subject> SubjectsObs { get; private set; } = [];

        private bool _isThisModule = false;
        private bool _displayNis = true;
        private int _selectedSubjCount;

        private readonly SyllabusModel _syllabus;
        private readonly List<Subject> _allSubjects;

        internal SubjectsWin(SyllabusModel syllabus, List<Subject> subjects)
        {
            InitializeComponent();
            
            foreach (Subject subject in subjects)
            {
                SubjectsObs.Add(subject);
            }

            subjOptions.ItemsSource = SubjectsObs;
            _syllabus = syllabus;
            _allSubjects = subjects;

            _selectedSubjCount = subjects.Count;
            selectedCountTextBlock.Text = $"Выбрано предметов: {_selectedSubjCount}";
            thisModuleOnly.Content = $"Только указанный модуль ({_syllabus.Module})";
            Loaded += (_, _) => AddHeaderBar(WindowGrid);
        }

        private void OnContinueClick(object sender, RoutedEventArgs e)
        {
            List<Subject> selectedSubjs = [];

            foreach (Subject subject in SubjectsObs)
            {
                if (subject.IsSelected)
                {
                    subject.SetFormula(_syllabus.Module);
                    selectedSubjs.Add(subject);
                }
            }

            GradesWin gradesWin = new(_syllabus, selectedSubjs);
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
            foreach (Subject subject in _allSubjects)
            {
                if (!subject.Modules.Contains(_syllabus.Module))
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
                for (int i = 0; i < _allSubjects.Count; i++)
                {
                    if (!_allSubjects[i].Modules.Contains(_syllabus.Module))
                    {
                        SubjectsObs.Insert(i, _allSubjects[i]);
                    }
                }
                
                return;
            }

            int cntNoNis = 0;

            foreach (Subject subject in _allSubjects)
            {
                if (!subject.IsNis && !subject.Modules.Contains(_syllabus.Module))
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
            foreach (Subject subject in _allSubjects)
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
                for (int i = 0; i < _allSubjects.Count; i++)
                {
                    Subject subject = _allSubjects[i];

                    if (subject.IsNis)
                    {
                        SubjectsObs.Insert(i, subject);
                    }
                }

                return;
            }

            int cntModule = 0;

            foreach (Subject subject in _allSubjects)
            {
                if (subject.IsNis && subject.Modules.Contains(_syllabus.Module))
                {
                    SubjectsObs.Insert(cntModule, subject);
                }

                if (subject.Modules.Contains(_syllabus.Module))
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
