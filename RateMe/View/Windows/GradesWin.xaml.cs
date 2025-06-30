using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using RateMe.Models.ClientModels;
using RateMe.Models.JsonModels;
using RateMe.Models.LocalDbModels;
using System.Net.Http;
using System.Net.Sockets;
using RateMe.Services;

namespace RateMe.View.Windows;

/// <summary>
/// Логика взаимодействия для Grades.xaml
/// </summary>
public partial class GradesWin : BaseFullWin
{
    private ObservableCollection<Subject> _subjects = [];
    private SyllabusModel _syllabus;
        
    private readonly SubjectsService _subjectsService;
    private readonly ElementsService _elementsService;
    
    
    /// <summary>
    /// Default constructor
    /// </summary>
    public GradesWin(SyllabusModel syllabus)
    {
        InitializeComponent();
            
        _subjectsService = new SubjectsService(_subjects);
        _elementsService = new ElementsService(_subjects);
        
        _syllabus = syllabus;
        grades.ItemsSource = _subjects;
        
        Loaded += (_, _) => AddHeaderBar(windowGrid);
        Loaded += async (_, _) => await LoadSubjectsFromLocalDb();
    }
    
    
    /// <summary>
    /// After pud subjects selection
    /// </summary>
    public GradesWin(SyllabusModel syllabus, List<Subject> subjectsFromPud) : this(syllabus)
    {
        Loaded += async (_, _) => await LoadSubjectsFromPud(subjectsFromPud);
    }

    
    private async void OnSaveAndQuitClick(object sender, RoutedEventArgs e)
    {
        _subjectsService.RetainSubjectsToUpdate();
        _elementsService.RetainElemsToUpdate();
        
        await _subjectsService.UpdateAllLocals();
        
        // Remote requests to add, update, delete etc. 
        await UpdateRemote();
        Close();
    }
    

    private async Task UpdateRemote()
    {
        try
        {
            await _subjectsService.SubjectsOverallRemoteUpdate();
            await _elementsService.ElementsOverallRemoteUpdate();
        }
        catch (HttpRequestException ex)
        {
            Type? exType = ex.InnerException?.GetType(); // TODO: make more appealing?
            MessageBox.Show(exType == typeof(SocketException) ? "Похоже сервер не отвечает(" : ex.ToString());
        }
    }
    

    private async Task LoadSubjectsFromLocalDb()
    {
        List<SubjectLocal> subjectLocals = await _subjectsService.GetAllLocals();
            
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
    
        
    private async Task LoadSubjectsFromPud(List<Subject> subjectsFrommPud)
    {
        foreach (Subject subject in subjectsFrommPud)
        {
            _subjects.Add(subject);
        }

        await _subjectsService.AddLocals(subjectsFrommPud);
    } 
    
        
    private async void OnAddSubject(object sender, MouseButtonEventArgs e)
    {
        Subject subject = new();
        _subjects.Add(subject);
        
        await _subjectsService.AddLocal(subject.LocalModel);
            
        SubjectEditWin subjWin = new SubjectEditWin(subject);
        subjWin.OnCancel += RemoveSubject;
        subjWin.AddedElem += _elementsService.AddLocal;
        subjWin.RemovedElem += _elementsService.RemoveLocal;
        subjWin.Show();
        subjWin.Activate();
    }

    
    private void OnEditGearClick(object sender, MouseButtonEventArgs e)
    {
        Subject? subject = ((FrameworkElement)sender).DataContext as Subject;

        if (subject == null)
        {
            MessageBox.Show("Null Subject");
            return;
        }

        SubjectEditWin subjWin = new SubjectEditWin(subject);
        subjWin.AddedElem += _elementsService.AddLocal;
        subjWin.RemovedElem += _elementsService.RemoveLocal;
        subjWin.Show();
        subjWin.Activate();
    }
    
        
    private void OnTrashBinClick(object sender, RoutedEventArgs e)
    {
        Subject? subject = ((FrameworkElement)sender).DataContext as Subject;
            
        if (subject == null)
        {
            return; 
        }

        RemoveSubjectWin win = new(subject);
        win.RemovalAgreed += RemoveSubject;
        win.Show();
    }

    
    private async Task RemoveSubject(Subject subject)
    {
        _subjects.Remove(subject);
        await _subjectsService.RemoveLocal(subject.LocalModel);
    }

    
    private void OnAccountClick(object sender, RoutedEventArgs e)
    {
        UserService userService = new UserService();
        userService.SignUpSignInStart += _subjectsService.UpdateAllLocals;
        userService.SignUpSuccess += _subjectsService.SubjectsOverallRemoteUpdate;
        
        AuthWin authWin = new(userService);
        authWin.Show();
    }


    private void OnRedoClick(object sender, RoutedEventArgs e)
    {
        RedoWin redo = new RedoWin();
        redo.RedoAgreed += Redo;
        redo.Show();
    }

    
    private async Task Redo(bool withSave)
    {
        if (withSave)
        {
            await _subjectsService.UpdateAllLocals();
        }
        else
        {
            await _subjectsService.RemoveLocals(_subjects);
        }
        
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