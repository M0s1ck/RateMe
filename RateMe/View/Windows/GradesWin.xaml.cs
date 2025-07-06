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
    private static readonly int NameColId = 0; 
    private static readonly int ElemsColId = 1; 
    private static readonly int CreditsColId = 2; 
    
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
        
        Loaded += (_, _) => AddHeaderBar(WindowGrid);
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
        if (_userService.IsUserAvailable)
        {
            ProfileWin accWin = new(_userService);
            accWin.Show();
            return;
        }
        
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

    
    // Arrow navigation management
    
    private void ArrowEscapeHandler(object sender, ArrowEscapeEventArgs e)
    {
        ElementsTable table = (ElementsTable)sender;
        Subject sub = (Subject)table.DataContext;
        int row = _subjects.IndexOf(sub);

        if (e.Key is Key.Left or Key.Right)
        {
            SetFocusToSide(e.Key, row);
            return;
        }
        
        decimal rel = (decimal)e.PanelIndex / e.TotalPanelCount + 0.5m / e.TotalPanelCount;

        if (e.Key == Key.Down && row != _subjects.Count - 1 && _subjects[row+1].FormulaObj.Count != 0)
        {
            SetFocusNextLevel(row + 1, rel, ElementsTable.NameBoxId);
        }
        else if (e.Key == Key.Up && row != 0 && _subjects[row-1].FormulaObj.Count != 0)
        {
            SetFocusNextLevel(row - 1, rel, ElementsTable.GradeBoxId);
        }
    }

    private void SetFocusNextLevel(int row, decimal rel, int boxId)
    {
        DependencyObject cell = GetGridCell(row, ElemsColId);
        DependencyObject unGrid = GetUnGrid(cell);
        int newId = GetNewPanelId(unGrid, rel);
        StackPanel panel = GetPanel(unGrid, newId);
        TextBox nameBox = (TextBox)panel.Children[boxId];
        nameBox.Focus();
    }

    private void SetFocusToSide(Key key, int row)
    {
        switch (key)
        {
            case Key.Left: SetFocusToDataGridTextBox(row, NameColId); return;
            case Key.Right: SetFocusToDataGridTextBox(row, CreditsColId); return;
            default: return;
        }
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

    private DependencyObject GetGridCell(int row, int col)
    {
        UIElement myRow = _uiRows![row];
        DependencyObject cBorder = VisualTreeHelper.GetChild(myRow, 0);
        DependencyObject csGrid = VisualTreeHelper.GetChild(cBorder, 0);
        DependencyObject cCellsPres = VisualTreeHelper.GetChild(csGrid, 0);
        DependencyObject ciPres = VisualTreeHelper.GetChild(cCellsPres, 0);
        DependencyObject cpanel = VisualTreeHelper.GetChild(ciPres, 0);
        DependencyObject cGridCell = VisualTreeHelper.GetChild(cpanel, col);
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
        int newInd = (int)Math.Floor(rel * panelsCnt);
        return newInd >= 0 ? newInd : 0;
    }

    private StackPanel GetPanel(DependencyObject unGrid, int newInd)
    {
        DependencyObject p = VisualTreeHelper.GetChild(unGrid, newInd);
        DependencyObject ccb = VisualTreeHelper.GetChild(p, 0);
        DependencyObject panel = VisualTreeHelper.GetChild(ccb, 0);
        return (StackPanel)panel;
    }

    private void SetFocusToDataGridTextBox(int row, int col)
    {
        GradesDataGrid.SelectedIndex = row;
        GradesDataGrid.CurrentCell = new DataGridCellInfo(GradesDataGrid.Items[row], GradesDataGrid.Columns[col]);
        GradesDataGrid.BeginEdit();

        DependencyObject cell = GetGridCell(row, col);
        
        GradesDataGrid.Dispatcher.InvokeAsync(() =>
        {
            DependencyObject bord = VisualTreeHelper.GetChild(cell, 0);
            DependencyObject cp = VisualTreeHelper.GetChild(bord, 0);
            TextBox textBox = (TextBox)VisualTreeHelper.GetChild(cp, 0);
            textBox.Focus();
            textBox.CaretIndex = textBox.Text.Length;
        });
    }

    private void SideBoxKeyDown(object sender, KeyEventArgs e)
    {
        TextBox box = (TextBox)sender;
        Subject subj = (Subject)box.DataContext;
        int row = _subjects.IndexOf(subj);
        int col = -1;

        switch (box.Tag.ToString())
        {
            case "NameColumn": col = NameColId; break;
            case "CreditsColumn": col = CreditsColId; break;
        }

        if (col != -1)
        {
            SideBoxKeyPressed(box, e.Key, row, col);
        }
    }

    private void SideBoxKeyPressed(TextBox box, Key key, int row, int col)
    {
        if (key == Key.Down && row != _subjects.Count - 1)
        {
            SetFocusToDataGridTextBox(row + 1, col);
            return;
        }
        
        if (key == Key.Up && row != 0)
        {
            SetFocusToDataGridTextBox(row - 1, col);
            return;
        }
        
        Subject subj = (Subject)box.DataContext;

        if (subj.FormulaObj.Count == 0)
        {
            return;
        }

        bool isEscapingNameCol = col == NameColId && key == Key.Right && box.CaretIndex == box.Text.Length;
        bool isEscapingCreditsCol = col == CreditsColId && key == Key.Left && box.CaretIndex == 0;
        
        if (isEscapingNameCol || isEscapingCreditsCol)
        {
            SetFocusOnElemsFromSide(row, col);
        }
    }
    
    private void SetFocusOnElemsFromSide(int row, int col)
    {
        DependencyObject cell = GetGridCell(row, ElemsColId);
        UniformGrid unGrid = (UniformGrid)GetUnGrid(cell);
        int panelId = col == NameColId ? 0 : unGrid.Children.Count - 1;
        StackPanel panel = GetPanel(unGrid, panelId);
        TextBox weightBox = (TextBox)panel.Children[1];
        weightBox.Focus();
    }
}