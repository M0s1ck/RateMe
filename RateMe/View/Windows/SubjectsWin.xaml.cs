using RateMe.DataUtils.Models;
using RateMe.View.UserControls;
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
        private List<Subject> _selectedSubjects;
        private bool _isThisModule = false;
        private bool _displayNis = true;
        private int _currentSubjCount;

        private readonly SyllabusModel Syllabus;
        private readonly Dictionary<string, Subject> SubjectsByNames = [];
        private readonly Dictionary<string, CheckBox> SubjCheckBoxesByNames = [];

        internal SubjectsWin(SyllabusModel syllabus, List<Subject> subjects)
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            WindowGrid.Children.Add(bar);

            _selectedSubjects = subjects;
            Syllabus = syllabus;
            int cnt = 1;

            foreach (Subject subject in subjects)
            {
                // Creating subject checkbox
                CheckBox subjOption = new CheckBox();
                subjOption.Name = $"subjCheckBox{cnt}";
                subjOption.Content = subject.Name;
                subjOption.Foreground = Brushes.White;
                subjOption.HorizontalContentAlignment = HorizontalAlignment.Left;
                subjOption.VerticalContentAlignment = VerticalAlignment.Top;
                subjOption.FontSize = 12.5;
                subjOption.FontFamily = new FontFamily("Bahnschrift SemiCondensed");
                subjOption.IsChecked = true;

                ScaleTransform scale = new ScaleTransform(1.5, 1.5);
                subjOption.RenderTransformOrigin = new Point(0.5, 0.5); // 1 / 1.5 ?
                subjOption.RenderTransform = scale;

                subjOptions.Items.Add(subjOption);

                SubjCheckBoxesByNames[subject.Name] = (CheckBox)subjOptions.Items[cnt - 1];
                cnt++;

                SubjectsByNames[subject.Name] = subject;
            }

            _currentSubjCount = subjects.Count;
            selectedCountTextBlock.Text = $"Выбрано предметов: {_currentSubjCount}";
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
            CheckBox box = (CheckBox)sender;
            string name = (string)box.Content;
            Subject subject = SubjectsByNames[name];
            _selectedSubjects.Add(subject);

            _currentSubjCount++;
            UpdateCurrentCountText();
        }


        private void OnSubjBoxUnchecked(object sender, RoutedEventArgs e)
        {
            CheckBox box = (CheckBox)sender;
            string name = (string)box.Content;
            Subject subject = SubjectsByNames[name];
            _selectedSubjects.Remove(subject);

            foreach (CheckBox realBox in subjOptions.Items)
            {
                if ((string)realBox.Content == name)
                {
                    realBox.IsChecked = false;
                }
            }

                _currentSubjCount--;
            UpdateCurrentCountText();
        }


        private void OnThisModuleOnlyChecked(object sender, RoutedEventArgs e)
        {
            foreach (CheckBox box in subjOptions.Items)
            {
                Subject subject = SubjectsByNames[(string)box.Content];

                if (!subject.Modules.Contains(Syllabus.Module))
                {
                    _selectedSubjects.Remove(subject);
                    box.IsChecked = false;
                    box.Visibility = Visibility.Collapsed;
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
                    _currentSubjCount--;
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
            foreach (ListBoxItem item in subjOptions.Items)
            {
                CheckBox box = (CheckBox)item.Content;
                box.IsChecked = false;
            }

            subjOptions.UpdateLayout();

            MainGrid.UpdateLayout();
            UpdateLayout();

            _selectedSubjects.Clear();
            UpdateCurrentCountText();
        }


        private void OnclearChoicesUnchecked(object sender, RoutedEventArgs e)
        {

        }


        private void UpdateCurrentCountText() => selectedCountTextBlock.Text = $"Выбрано предметов: {_selectedSubjects.Count}";
    }
}
