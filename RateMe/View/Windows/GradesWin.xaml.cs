using RateMe.DataUtils.Models;
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
    /// Логика взаимодействия для Grades.xaml
    /// </summary>
    public partial class GradesWin : Window
    {
        private List<Subject> _subjects;
        private SyllabusModel _syllabus;

        public GradesWin(SyllabusModel syllabus, List<Subject> subjects)
        {
            _subjects = subjects;
            _syllabus = syllabus;
            InitializeComponent();
        }
    }
}
