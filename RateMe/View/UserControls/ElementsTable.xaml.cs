using System.Collections.ObjectModel;
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

    #region StaticConsts
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
            ((TextBox)sender).Text = grade == 0 ? "0" : grade.ToString();
        }
    }
        
    
    private void OnKeyPressed(object sender, KeyEventArgs e)
    {
        Key key = e.Key;
        TextBox box = (TextBox)sender;
        StackPanel panel = (StackPanel)box.Parent;

        if (IsArrowEscaping(key, box.Name))
        {
            int boxIndex = panel.Children.IndexOf(box);
            int totalCount = panel.Children.Count;
            ArrowEscapeEventArgs ae = new(key, boxIndex, totalCount);
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
            SetFocusRight(box, panel);
        } 
        else if (key == Key.Left && box.CaretIndex == 0)
        {
            SetFocusLeft(box, panel);
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

    private void SetFocusRight(TextBox box, StackPanel panel)
    {
        ObservableCollection<Element> elems = (ObservableCollection<Element>)Elems.ItemsSource;
        Element currentElem = (Element)panel.DataContext;
        int ind = elems.IndexOf(currentElem);

        if (ind == -1 || ind == elems.Count - 1)
        {
            return;
        }

        int nextIndex = ind + 1;
        SetFocusInVisualTreeByInd(box, nextIndex);
    }
    
    private void SetFocusLeft(TextBox box, StackPanel panel)
    {
        ObservableCollection<Element> elems = (ObservableCollection<Element>)Elems.ItemsSource;
        Element currentElem = (Element)panel.DataContext;
        int ind = elems.IndexOf(currentElem);

        if (ind is -1 or 0)
        {
            return;
        }

        int nextIndex = ind - 1;
        SetFocusInVisualTreeByInd(box, nextIndex);
    }

    private void SetFocusInVisualTreeByInd(TextBox box, int nextIndex)
    {
        ContentPresenter? container = Elems.ItemContainerGenerator.ContainerFromIndex(nextIndex) as ContentPresenter;
        DependencyObject group = VisualTreeHelper.GetChild(container!, 0);
        StackPanel nextPanel = (StackPanel)VisualTreeHelper.GetChild(group!, 0);

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

    private bool IsArrowEscaping(Key key, string boxName)
    {
        return key == Key.Down && boxName == GradeBoxName || key == Key.Up && boxName == NameBoxName;
    }
}


public class ArrowEscapeEventArgs
{
    public Key Key { get; }
    public int BoxIndex { get; }
    public int TotalBoxCount { get; }
    
    public ArrowEscapeEventArgs(Key key, int boxIndex, int total)
    {
        Key = key;
        BoxIndex = boxIndex;
        TotalBoxCount = total;
    }
} 