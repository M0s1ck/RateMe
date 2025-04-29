using RateMe.DataUtils.Models;
using RateMe.View.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RateMe.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для SubjectEditWin.xaml
    /// </summary>
    public partial class SubjectEditWin : Window
    {
        Subject TheSubject { get; }

        static readonly SolidColorBrush ColorWhenEntered = new SolidColorBrush(Colors.DimGray);
        static readonly SolidColorBrush ColorWhenLeft = new SolidColorBrush(Colors.White);

        public SubjectEditWin(Subject subject)
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            windowGrid.Children.Add(bar);

            TheSubject = subject;
            Topmost = true;
            DataContext = TheSubject;

            DataHintTextModel subjectNameTextModel = new DataHintTextModel(TheSubject.Name, "Название предмета", Visibility.Visible);
            subjTetx2.DataContext = subjectNameTextModel;

            gradesTable.DataContext = TheSubject;
        }


        private void OnAddClick(object sender, MouseButtonEventArgs e)
        {
            TheSubject.FormulaObj.Add(new ControlElement());
        }

        private void OnRemoveClick(object sender, MouseButtonEventArgs e)
        {
            TheSubject.FormulaObj.Add(new ControlElement());
        }

        private void OnRemoveMouseEnter(object sender, MouseEventArgs e)
        {
            horBar2.Fill = ColorWhenEntered;
        }

        private void OnRemoveMouseLeave(object sender, MouseEventArgs e)
        {
            horBar2.Fill = ColorWhenLeft;
        }

        private void OnAddMouseEnter(object sender, MouseEventArgs e)
        {
            vertBar.Fill = ColorWhenEntered;
            horBar.Fill = ColorWhenEntered;
        }

        private void OnAddMouseLeave(object sender, MouseEventArgs e)
        {
            vertBar.Fill = ColorWhenLeft;
            horBar.Fill = ColorWhenLeft;
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
