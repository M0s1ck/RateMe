using RateMe.Models.ClientModels;
using RateMe.View.UserControls;
using System.Windows;
using System.Windows.Input;

namespace RateMe.View.Windows
{
    /// <summary>
    /// Window for editing the picked subject.
    /// </summary>
    public partial class SubjectEditWin : BaseFullWin
    {
        private Subject _theSubject;
        private Subject _updatedSubject;
        private DataHintTextModel _subjectNameTextModel;

        public SubjectEditWin(Subject subject)
        {
            InitializeComponent();
            
            _theSubject = subject;
            _updatedSubject = new Subject(subject);
            _subjectNameTextModel = new DataHintTextModel(_updatedSubject.Name, "Название предмета");

            DataContext = _updatedSubject;
            gradesTable.DataContext = _updatedSubject;
            subjTetx2.DataContext = _subjectNameTextModel;
            
            Loaded += (_, _) => OnLoaded();
            Loaded += (_, _) => AddHeaderBar(windowGrid); 
        }

        private void OnSaveClick(object sender, RoutedEventArgs e)
        {
            _theSubject.Name = _subjectNameTextModel.Data;
            _theSubject.Credits = _updatedSubject.Credits;
            _theSubject.FormulaObj.Clear();
            _theSubject.Score = 0;

            foreach (ControlElement elem in _updatedSubject.FormulaObj)
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
            _updatedSubject.FormulaObj.Add(newElem);
            _updatedSubject.LocalModel.Elements.Add(newElem.LocalModel);
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
                _updatedSubject.FormulaObj.Remove(elem);
                _updatedSubject.LocalModel.Elements.Remove(elem.LocalModel);
            }
        }

        private void OnLoaded()
        {
            Topmost = true;
            MinusButton.vertBar.Visibility = Visibility.Hidden;
        }

        private void OnCancelClick(object sender, RoutedEventArgs e)
        {
            OnCancel?.Invoke(_theSubject);
            Close();
        }

        public event CancelHandler? OnCancel;
        
        public delegate void CancelHandler(Subject subj);
        
        private void OnMouseLeave(object sender, MouseEventArgs e) => Topmost = false;

        private void OnMouseEnter(object sender, MouseEventArgs e) => Topmost = false;
    }
}
