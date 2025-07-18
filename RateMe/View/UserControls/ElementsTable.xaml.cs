using System.Collections.ObjectModel;
using System.Globalization;
using RateMe.Models.ClientModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace RateMe.View.UserControls;

/// <summary>
/// Логика взаимодействия для ControlElementsTable.xaml
/// </summary>
public partial class ElementsTable : UserControl
{
    public event EventHandler<ArrowEscapeEventArgs>? ArrowEscape;
    
    private int ElemsCnt => Elems.Items.Count;
    private ObservableCollection<Element>? _elems;

    #region StaticConsts
    public static int NameBoxId { get; } = 0;
    public static int WeightBoxId { get; } = 1;
    public static int GradeBoxId { get; } = 2;
    
    private static readonly string NameBoxName = "NameBox";
    private static readonly string WeightBoxName = "WeightBox";
    private static readonly string GradeBoxName = "GradeBox";
        
    private static readonly Dictionary<string, string> BoxNamesMapDown = new()
    {
        {NameBoxName, WeightBoxName},
        {WeightBoxName, GradeBoxName}
    };
    
    private static readonly Dictionary<string, string> BoxNamesMapUp = new()
    {
        {WeightBoxName, NameBoxName},
        {GradeBoxName, WeightBoxName}
    };
    #endregion
    
    public ElementsTable()
    {
        InitializeComponent();
        Loaded += (_, _) => _elems = (ObservableCollection<Element>)Elems.ItemsSource;
    }

    private void OnNameChanged(object sender, TextChangedEventArgs e)
    {
        Element? element = ((FrameworkElement)sender).DataContext as Element;
        if (element != null)
        {
            element.Name = ((TextBox)sender).Text;
        }
    }

    private void OnWeightChanged(object sender, TextChangedEventArgs e) // TODO: add bounds? 
    {
        Element? element = ((FrameworkElement)sender).DataContext as Element;
        string w = ((TextBox)sender).Text;
        bool good = decimal.TryParse(w, out decimal weight);
        if (good && element != null)
        {
            element.Weight = weight;
        }
    }

    private void OnGradeChanged(object sender, TextChangedEventArgs e)
    {
        Element? element = ((FrameworkElement)sender).DataContext as Element;
        string g = ((TextBox)sender).Text;
        bool good = decimal.TryParse(g, out decimal grade);
        if (good && element != null)
        {
            element.Grade = grade;
            ((TextBox)sender).Text = grade == 0 ? "0" : grade.ToString(CultureInfo.InvariantCulture);
        }
    }
        
    
    private void OnKeyPressed(object sender, KeyEventArgs e)
    {
        Key key = e.Key;
        TextBox box = (TextBox)sender;
        StackPanel panel = (StackPanel)box.Parent;
        int panelId = GetPanelId(panel);

        if (IsArrowEscaping(key, box, panelId))
        {
            ArrowEscapeEventArgs ae = new(key, panelId, ElemsCnt);
            ArrowEscape?.Invoke(this, ae);
        }
            
        if (key == Key.Down)
        {
            SetFocusDown(box, panel);
        }
        else if (key == Key.Up)
        {
            SetFocusUp(box, panel);
        } 
        else if (key == Key.Right && box.CaretIndex == box.Text.Length)
        { 
            SetFocusRight(box, panelId);
        } 
        else if (key == Key.Left && box.CaretIndex == 0)
        {
            SetFocusLeft(box, panelId);
        }
    }

    
    private void SetFocusDown(TextBox box, StackPanel panel)
    {
        if (box.Name == GradeBoxName)
        {
            return;
        }
        
        string nextName = BoxNamesMapDown[box.Name];
        TextBox? next = FindByNameInPanel(nextName, panel);
        next!.Focus();
    }

    private void SetFocusUp(TextBox box, StackPanel panel)
    {
        if (box.Name == NameBoxName)
        {
            return;
        }
        
        string nextName = BoxNamesMapUp[box.Name];
        TextBox? next = FindByNameInPanel(nextName, panel);
        next!.Focus();
    }

    private void SetFocusRight(TextBox box, int panelId)
    {
        if (panelId == -1 || panelId == ElemsCnt - 1)
        {
            return;
        }

        int nextIndex = panelId + 1;
        SetFocusInVisualTreeByInd(box, nextIndex);
    }
    
    private void SetFocusLeft(TextBox box, int panelId)
    {
        if (panelId is -1 or 0)
        {
            return;
        }

        int nextIndex = panelId - 1;
        SetFocusInVisualTreeByInd(box, nextIndex);
    }

    private void SetFocusInVisualTreeByInd(TextBox box, int nextIndex)
    {
        ContentPresenter? container = Elems.ItemContainerGenerator.ContainerFromIndex(nextIndex) as ContentPresenter;
        DependencyObject group = VisualTreeHelper.GetChild(container!, 0);
        StackPanel nextPanel = (StackPanel)VisualTreeHelper.GetChild(group, 0);

        TextBox? nextBox = FindByNameInPanel(box.Name, nextPanel);
        nextBox!.Focus();
    }

    private TextBox? FindByNameInPanel(string nextName, StackPanel panel)
    {
        foreach (TextBox b in panel.Children)
        {
            if (b.Name == nextName)
            {
                return b;
            }
        }

        return null;
    }

    private int GetPanelId(StackPanel panel)
    {
        Element currentElem = (Element)panel.DataContext;
        return _elems!.IndexOf(currentElem);
    }

    private bool IsArrowEscaping(Key key, TextBox box , int panelId)
    {
        return key == Key.Down && box.Name == GradeBoxName || key == Key.Up && box.Name == NameBoxName
            || key == Key.Left && panelId == 0 && box.CaretIndex == 0 
            || key == Key.Right && panelId == ElemsCnt - 1 && box.CaretIndex == box.Text.Length;
    }
}


public class ArrowEscapeEventArgs
{
    public Key Key { get; }
    public int PanelIndex { get; }
    public int TotalPanelCount { get; }
    
    public ArrowEscapeEventArgs(Key key, int panelIndex, int total)
    {
        Key = key;
        PanelIndex = panelIndex;
        TotalPanelCount = total;
    }
} 