using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using RateMe.Models.ClientModels;
using RateMe.Models.LocalDbModels;
using System.Net.Http;
using System.Net.Sockets;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using RateMe.Models.JsonFileModels;
using RateMe.Services;
using RateMe.View.UserControls;

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
    private readonly UserService _userService;

    private UIElementCollection? _uiRows;
    
    /// <summary>
    /// Default constructor
    /// </summary>
    public GradesWin(SyllabusModel syllabus)
    {
        InitializeComponent();
            
        _subjectsService = new SubjectsService(_subjects);
        _elementsService = new ElementsService(_subjects);
        _userService = new UserService(_subjectsService, _elementsService);
        
        _syllabus = syllabus;
        GradesDataGrid.ItemsSource = _subjects;
        
        Loaded += (_, _) => AddHeaderBar(windowGrid);
        Loaded += async (_, _) => await LoadSubjectsFromLocalDb();
        Loaded += (_, _) => _uiRows = GetUiRows();
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
        await SaveData();
        Close();
    }

    private async Task SaveData()
    {
        if (_userService.IsUserAvailable)
        {
            _subjectsService.RetainSubjectsToUpdate();
            _elementsService.RetainElemsToUpdate();
        }
        
        await _subjectsService.UpdateAllLocals();

        if (_userService.IsUserAvailable)
        {
            await UpdateRemote();
        }

        if (!_userService.IsUserAvailable)
        {
            MessageBox.Show("No remote save for ya because u are not signed up");
        }
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
            await _subjectsService.MarkRemoteStates();
            await _elementsService.MarkRemoteStates();
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
        Config config = JsonFileModelsHelper.GetConfig();
        config.IsSubjectsLoaded = true;
        JsonFileModelsHelper.SaveConfig(config);
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
            
        SubjectEditWin subjWin = new(subject, _elementsService);
        subjWin.OnCancel += RemoveSubject;
        
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

        SubjectEditWin subjWin = new(subject, _elementsService);
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

        string question = $"Удалить предмет {subject.Name}?";
        YesNoWin win = new(question);
        win.YesButton.Click += async (_, _) => { await RemoveSubject(subject); };
        win.Show();
    }

    
    private async Task RemoveSubject(Subject subject)
    {
        _subjects.Remove(subject);
        await _subjectsService.RemoveLocal(subject.LocalModel);
    }

    
    private void OnAccountClick(object sender, RoutedEventArgs e)
    {
        AuthWin authWin = new(_userService);
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
            await SaveData();
        }
        else
        {
            await _subjectsService.ClearLocal(); // No remote remove for now
        }
        
        Close();
        
        DataCollection dataWin = new();
        dataWin.Show();
            
        // Log to config
        Config config = JsonFileModelsHelper.GetConfig();
        config.IsSubjectsLoaded = false;
        JsonFileModelsHelper.SaveConfig(config);
    }

    private async void OnInfoClick(object sender, RoutedEventArgs e)
    {
        InfoWin infoWin = new();
        // infoWin.Show();
        await _userService.SignOut();
    }

    private void ArrowEscapeHandler(object sender, ArrowEscapeEventArgs e)
    {
        ElementsTable table = (ElementsTable)sender;
        Subject sub = (Subject)table.DataContext;
        decimal rel = (decimal)e.BoxIndex / e.TotalBoxCount; // Or sub.__.Count
        int row = _subjects.IndexOf(sub);

        if (e.Key == Key.Down && row != _subjects.Count - 1)
        {
            SetFocusLower(row, rel);
        }
    }

    private void SetFocusLower(int row, decimal rel)
    {
        DependencyObject cell = GetGridCell(row + 1, 1);
        DependencyObject unGri = GetUnGrid(cell);
        int newId = GetNewPanelId(unGri, rel);
        DependencyObject panel = GetPanel(unGri, newId);
        TextBox nameBox = (TextBox)VisualTreeHelper.GetChild(panel, 0);
        nameBox.Focus();
    }
    
    private UIElementCollection GetUiRows()
    {
        DependencyObject borderChild = VisualTreeHelper.GetChild(GradesDataGrid, 0);
        DependencyObject scViewer = VisualTreeHelper.GetChild(borderChild, 0);
        DependencyObject gridChild = VisualTreeHelper.GetChild(scViewer, 0);
        DependencyObject scpr = VisualTreeHelper.GetChild(gridChild, 2);
        DependencyObject itp = VisualTreeHelper.GetChild(scpr, 0);
        DataGridRowsPresenter dgrp = (DataGridRowsPresenter)VisualTreeHelper.GetChild(itp, 0);
        return dgrp.Children;
    }

    private DependencyObject GetGridCell(int row, int cell)
    {
        UIElement myRow = _uiRows![row];
        DependencyObject cBorder = VisualTreeHelper.GetChild(myRow, 0);
        DependencyObject csGrid = VisualTreeHelper.GetChild(cBorder, 0);
        DependencyObject cCellsPres = VisualTreeHelper.GetChild(csGrid, 0);
        DependencyObject ciPres = VisualTreeHelper.GetChild(cCellsPres, 0);
        DependencyObject cpanel = VisualTreeHelper.GetChild(ciPres, 0);
        DependencyObject cGridCell = VisualTreeHelper.GetChild(cpanel, cell);
        return cGridCell;
    }

    private DependencyObject GetUnGrid(DependencyObject gridCell)
    {
        DependencyObject cBord = VisualTreeHelper.GetChild(gridCell, 0);
        DependencyObject ccPres = VisualTreeHelper.GetChild(cBord, 0);
        DependencyObject cccPres = VisualTreeHelper.GetChild(ccPres, 0);
        ElementsTable myHomie = (ElementsTable)VisualTreeHelper.GetChild(cccPres, 0);
        DependencyObject bord = VisualTreeHelper.GetChild(myHomie, 0);
        DependencyObject aaa = VisualTreeHelper.GetChild(bord, 0);
        DependencyObject grid = VisualTreeHelper.GetChild(aaa, 0);
        DependencyObject itemC = VisualTreeHelper.GetChild(grid, 0);
        DependencyObject cBord2 = VisualTreeHelper.GetChild(itemC, 0);
        DependencyObject pres = VisualTreeHelper.GetChild(cBord2, 0);
        DependencyObject unGrid = VisualTreeHelper.GetChild(pres, 0);
        return unGrid;
    }

    private int GetNewPanelId(DependencyObject unGrid, decimal rel)
    {
        int panelsCnt = VisualTreeHelper.GetChildrenCount(unGrid);
        int newInd = (int)Math.Round(rel * panelsCnt) - 1;         // TODO: fix this -1 stuff i have no braincells left
        return newInd;
    }

    private DependencyObject GetPanel(DependencyObject unGrid, int newInd)
    {
        DependencyObject p = VisualTreeHelper.GetChild(unGrid, newInd);
        DependencyObject ccb = VisualTreeHelper.GetChild(p, 0);
        DependencyObject panel = VisualTreeHelper.GetChild(ccb, 0);
        return panel;
    }
}