using RateMe.Models.ClientModels;
using RateMe.View.UserControls;
using System.Windows;
using System.Windows.Input;
using RateMe.Models.LocalDbModels;

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

        private async void OnAddClick(object sender, MouseButtonEventArgs e)
        {
            ControlElement newElem = new ControlElement();
            _updatedSubject.FormulaObj.Add(newElem);
            _updatedSubject.LocalModel.Elements.Add(newElem.LocalModel);
            await AddedElem?.Invoke(_updatedSubject.LocalModel.SubjectId, newElem.LocalModel)!;  // Wtf is '!' ???
        }

        private async void OnRemovalClick(object sender, RoutedEventArgs e)
        {
            if (((FrameworkElement)sender).DataContext is not ControlElement elem)
            {
                return;
            }
            
            _updatedSubject.FormulaObj.Remove(elem);
            _updatedSubject.LocalModel.Elements.Remove(elem.LocalModel);
            await RemovedElem?.Invoke(elem.LocalModel)!;
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
        
        private void OnRemoveClick(object sender, MouseButtonEventArgs e)
        {
            removalButtonList.Visibility = removalButtonList.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
        }
        
        public event CancelHandler? OnCancel;
        public delegate Task CancelHandler(Subject subj);
        
        public event AddedElemHandler? AddedElem;
        public delegate Task AddedElemHandler(int subId, ControlElementLocal elem);

        public event RemovedElemHandler? RemovedElem;
        public delegate Task RemovedElemHandler(ControlElementLocal elem);
        
        private void OnMouseLeave(object sender, MouseEventArgs e) => Topmost = false;
        private void OnMouseEnter(object sender, MouseEventArgs e) => Topmost = false;
    }
}
