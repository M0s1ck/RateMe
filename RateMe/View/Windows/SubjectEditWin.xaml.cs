using RateMe.Models.ClientModels;
using System.Windows;
using System.Windows.Input;
using RateMe.Models.LocalDbModels;
using RateMe.Services.Interfaces;

namespace RateMe.View.Windows;

/// <summary>
/// Window for editing the picked subject.
/// </summary>
public partial class SubjectEditWin : BaseFullWin
{
    private Subject _theSubject;
    private readonly ILocalElemService _elemService;
    
    private Subject _updatedSubject;
    private DataHintTextModel _subjectNameTextModel;
    private int SubId => _updatedSubject.LocalModel.SubjectId;
    
    private HashSet<ElementLocal> _addedElems = [];
    private HashSet<ElementLocal> _removedElems = [];
    
    public event CancelAsyncHandler? OnCancel;
    public delegate Task CancelAsyncHandler(Subject subj);

    
    public SubjectEditWin(Subject subject, ILocalElemService elemService)
    {
        InitializeComponent();
            
        _theSubject = subject;
        _elemService = elemService;
            
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

        foreach (Element elem in _updatedSubject.FormulaObj)
        {
            elem.GradesUpdated += _theSubject.UpdateScore;
            _theSubject.FormulaObj.Add(elem);
            _theSubject.Score += elem.Weight * elem.Grade;
        }

        Close();
    }

    
    private async void OnAddClick(object sender, MouseButtonEventArgs e)
    {
        Element newElem = new Element();
        _updatedSubject.FormulaObj.Add(newElem);
        _updatedSubject.LocalModel.Elements.Add(newElem.LocalModel);
        
        await _elemService.AddLocal(SubId, newElem.LocalModel);
        
        _addedElems.Add(newElem.LocalModel);
        _removedElems.Remove(newElem.LocalModel);
    }
    
    private async void OnRemovalClick(object sender, RoutedEventArgs e)
    {
        if (((FrameworkElement)sender).DataContext is not Element elem)
        {
            return;
        }
            
        _updatedSubject.FormulaObj.Remove(elem);
        _updatedSubject.LocalModel.Elements.Remove(elem.LocalModel);
        
        await _elemService.RemoveLocal(elem.LocalModel);
        
        _addedElems.Remove(elem.LocalModel);
        _removedElems.Add(elem.LocalModel);
    }

    private async void OnCancelClick(object sender, RoutedEventArgs e)
    {
        await _elemService.RemoveLocals(SubId, _addedElems);
        await _elemService.AddLocals(SubId, _removedElems);
        
        if (OnCancel != null)
        {
            await OnCancel.Invoke(_theSubject);
        }
        
        Close();
    }
        
    private void OnRemoveClick(object sender, MouseButtonEventArgs e)
    {
        removalButtonList.Visibility = removalButtonList.Visibility == Visibility.Hidden ? Visibility.Visible : Visibility.Hidden;
    }
    
    private void OnLoaded()
    {
        Topmost = true;
        MinusButton.vertBar.Visibility = Visibility.Hidden;
    }
        
    private void OnMouseLeave(object sender, MouseEventArgs e) => Topmost = false;
    private void OnMouseEnter(object sender, MouseEventArgs e) => Topmost = false;
}