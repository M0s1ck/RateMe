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
using System.Net.Http;
using System.Net.Sockets;

namespace RateMe.View.Windows;

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
        
    // Default
    public GradesWin(SyllabusModel syllabus)
    {
        InitializeComponent();
        WindowBarDockPanel bar = new(this);
        windowGrid.Children.Add(bar);
            
        _subjectsService = new SubjectsService(new SubjectsClient());
        _syllabus = syllabus;
        grades.ItemsSource = _subjects;

        Loaded += async (_, _) => await LoadSubjectsFromLocalDb();
    }
        
    // After pud subjects selection
    public GradesWin(SyllabusModel syllabus, List<Subject> subjectsFromPud) : this(syllabus)
    {
        Loaded += (_, _) => LoadSubjectsFromPud(subjectsFromPud);
    }

    private async void OnSaveAndQuitClick(object sender, RoutedEventArgs e)
    {
        foreach (Subject subject in _subjects)
        {
            subject.UpdateLocalModel();
        }

        await _localDb.SaveChangesAsync();
        // Remote requests to add, update, delete etc. 
        await UpdateRemote();
        Close();
    }

    private async Task UpdateRemote()
    {
        try
        {
            await  _subjectsService.SubjectsOverallRemoteUpdate(_subjectsToAdd, _subjKeysToRemove);
        }
        catch (HttpRequestException ex)
        {
            Type? exType = ex.InnerException?.GetType(); // TODO: make more appealing?
            MessageBox.Show(exType == typeof(SocketException) ? "Похоже сервер не отвечает(" : ex.ToString());
        }
    }

    private async Task LoadSubjectsFromLocalDb()
    {
        List<SubjectLocal> subjectLocals = await _localDb.Subjects.Include(s => s.Elements).ToListAsync();
            
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
        
    private void LoadSubjectsFromPud(List<Subject> subjectsFrommPud)
    {
        foreach (Subject subject in subjectsFrommPud)
        {
            _subjects.Add(subject);
            subject.LocalModel = new SubjectLocal { Name = subject.Name, Credits = subject.Credits, Elements = [] };

            foreach (ControlElement elem in subject.FormulaObj)
            {
                subject.LocalModel.Elements.Add(elem.LocalModel);
            }

            _localDb.Add(subject.LocalModel);
        }
            
        grades.ItemsSource = _subjects;
    } 
        
    private async void OnAddSubject(object sender, MouseButtonEventArgs e)
    {
        Subject subject = new();
        _subjects.Add(subject);
        _localDb.Add(subject.LocalModel);
        await _localDb.SaveChangesAsync();
        _subjectsToAdd[subject.LocalModel.SubjectId] = subject;
            
        SubjectEditWin subjWin = new SubjectEditWin(subject);
        subjWin.OnCancel += RemoveSubject;
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
        
    private void OnWindowClick(object sender, MouseButtonEventArgs e)
    {
        Keyboard.ClearFocus();
    }
}