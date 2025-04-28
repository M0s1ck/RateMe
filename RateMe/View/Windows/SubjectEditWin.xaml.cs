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
        public SubjectEditWin(Subject subject)
        {
            InitializeComponent();
            WindowBarDockPanel bar = new(this);
            windowGrid.Children.Add(bar);


            Topmost = true;
            DataContext = subject;

            DataHintTextModel subjectNameTextModel = new DataHintTextModel(subject.Name, "Название предмета", Visibility.Visible);
            subjTetx2.DataContext = subjectNameTextModel;

            gradesTable.DataContext = subject;
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
