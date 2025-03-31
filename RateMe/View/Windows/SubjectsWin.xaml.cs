using RateMe.DataUtils.Models;
using RateMe.View.UserControls;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace RateMe
{
    /// <summary>
    /// Логика взаимодействия для SubjectsWin.xaml
    /// </summary>
    public partial class SubjectsWin : Window
    {
        // private?
        internal ObservableCollection<Subject> SubjectsObs { get; private set; } = [];

        private List<Subject> _selectedSubjects;
        private bool _isThisModule = false;
        private bool _displayNis = true;
        private int _selectedSubjCount;

        private readonly SyllabusModel Syllabus;
        private readonly Dictionary<string, Subject> SubjectsByNames = [];
        private readonly Dictionary<string, CheckBox> SubjCheckBoxesByNames = [];

        internal SubjectsWin(SyllabusModel syllabus, List<Subject> subjects)
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            WindowGrid.Children.Add(bar);

            this.DataContext = this.SubjectsObs;

            _selectedSubjects = subjects;
            Syllabus = syllabus;
            int cnt = 1;

            foreach (Subject subject in subjects)
            {
                // Creating subject checkbox
                CheckBox subjOption = new CheckBox();
                subjOption.Name = $"subjCheckBox{cnt}";

                ScaleTransform scale = new ScaleTransform(1.5, 1.5);
                subjOption.RenderTransformOrigin = new Point(0.5, 0.5); // 1 / 1.5 ?
                subjOption.RenderTransform = scale;

                //subjOptions.Items.Add(subjOption);
                SubjectsObs.Add(subject);

                cnt++;

                SubjectsByNames[subject.Name] = subject;

                
            }

            subjOptions.ItemsSource = SubjectsObs;

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

            foreach ((string name, CheckBox subjBox) in SubjCheckBoxesByNames)
            {
                if (subjBox.IsChecked != true)
                {
                    continue;
                }

                Subject subj = SubjectsByNames[name];
                selectedSubjs.Add(subj);
            }

            // new(Syllabus, _selectedSubjs)
            // Close()

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
            foreach (CheckBox box in subjOptions.Items)
            {
                Subject subject = SubjectsByNames[(string)box.Content];

                if (!subject.Modules.Contains(Syllabus.Module))
                {
                    subject.IsSelected = false;
                    box.Visibility = Visibility.Collapsed;
                    _selectedSubjCount--;
                }
            }

            _isThisModule = true;
            UpdateCurrentCountText();
        }


        private void OnThisModuleOnlyUnchecked(object sender, RoutedEventArgs e)
        {
            _isThisModule = false;

            if (_displayNis)
            {
                foreach (CheckBox box in subjOptions.Items)
                {
                    Subject subject = SubjectsByNames[(string)box.Content];

                    if (!subject.Modules.Contains(Syllabus.Module))
                    {
                        box.IsChecked = false;
                        subjOptions.UpdateLayout();
                        box.Visibility = Visibility.Visible;
                        subjOptions.UpdateLayout();
                        box.IsChecked = false;
                        subjOptions.UpdateLayout();
                    }
                }

                return;
            }

            foreach (CheckBox box in subjOptions.Items)
            {
                Subject subject = SubjectsByNames[(string)box.Content];

                if (!subject.IsNis && !subject.Modules.Contains(Syllabus.Module))
                {
                    box.Visibility = Visibility.Visible;
                    box.IsChecked = false;
                }
            }
        }


        private void OnRemoveNisChecked(object sender, RoutedEventArgs e)
        {
            foreach (string name in SubjectsByNames.Keys)
            {
                Subject subject = SubjectsByNames[name];
                CheckBox subjBox = SubjCheckBoxesByNames[name];

                if (subject.IsNis && subjBox.Visibility == Visibility.Visible)
                {
                    subjBox.IsChecked = false;
                    subjBox.Visibility = Visibility.Collapsed;
                    _selectedSubjCount--;
                }
            }

            _displayNis = false;
            UpdateCurrentCountText();
        }


        private void OnRemoveNisUnchecked(object sender, RoutedEventArgs e)
        {

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
                subject.IsSelected = true;
            }

            _selectedSubjCount = SubjectsByNames.Count;
            UpdateCurrentCountText();
        }


        private void UpdateCurrentCountText() => selectedCountTextBlock.Text = $"Выбрано предметов: {_selectedSubjCount}";
    }
}
