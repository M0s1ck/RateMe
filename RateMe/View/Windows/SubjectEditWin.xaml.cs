using RateMe.DataUtils.Models;
using RateMe.View.UserControls;
using System.Windows;
using System.Windows.Input;

namespace RateMe.View.Windows
{
    /// <summary>
    /// Window for editing the picked subject.
    /// </summary>
    public partial class SubjectEditWin : Window
    {
        private Subject _theSubject;
        public Subject UpdatedSubject { get; private set; }

        private DataHintTextModel _subjectNameTextModel;


        public SubjectEditWin(Subject subject)
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            windowGrid.Children.Add(bar);
            Topmost = true;
            MinusButton.vertBar.Visibility = Visibility.Hidden;

            _theSubject = subject;
            UpdatedSubject = new Subject(subject.Name, subject.Credits, subject.Modules, []);
            UpdatedSubject.FormulaObj = [];
            UpdatedSubject.LocalModel = subject.LocalModel;
            
            foreach (ControlElement elem in subject.FormulaObj)
            {
                ControlElement newElem = new ControlElement(elem.Name, elem.Weight);
                newElem.Grade = elem.Grade;
                newElem.LocalModel = elem.LocalModel;
                UpdatedSubject.FormulaObj.Add(newElem);
            }

            DataContext = UpdatedSubject;

            _subjectNameTextModel = new DataHintTextModel(UpdatedSubject.Name, "Название предмета", Visibility.Visible);
            subjTetx2.DataContext = _subjectNameTextModel;

            gradesTable.DataContext = UpdatedSubject;
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            _theSubject.Name = _subjectNameTextModel.Data;
            _theSubject.Credits = UpdatedSubject.Credits;
            _theSubject.FormulaObj.Clear();
            _theSubject.Score = 0;

            foreach (ControlElement elem in UpdatedSubject.FormulaObj)
            {
                elem.GradesUpdated += _theSubject.UpdateScore;
                _theSubject.FormulaObj.Add(elem);
                _theSubject.Score += elem.Weight * elem.Grade;
            }

            Close();
        }

        private void OnAddClick(object sender, MouseButtonEventArgs e)
        {
            ControlElement newElem = new ControlElement();
            UpdatedSubject.FormulaObj.Add(newElem);
            UpdatedSubject.LocalModel.Elements.Add(newElem.LocalModel);
        }

        private void OnRemoveClick(object sender, MouseButtonEventArgs e)
        {
            removalButtonList.Visibility = removalButtonList.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }

        private void OnRemovalClick(object sender, RoutedEventArgs e)
        {
            ControlElement? elem = ((FrameworkElement)sender)?.DataContext as ControlElement;

            if (elem != null)
            {
                UpdatedSubject.FormulaObj.Remove(elem);
                UpdatedSubject.LocalModel.Elements.Remove(elem.LocalModel);
            }
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OnWindowClick(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
            Topmost = false;
        }
        
        private void OnMouseLeave(object sender, MouseEventArgs e) => Topmost = false;

        private void OnMouseEnter(object sender, MouseEventArgs e) => Topmost = false;
    }
}
