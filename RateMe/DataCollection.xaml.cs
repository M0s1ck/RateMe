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
using RateMe.DataUtils;
using RateMe.DataUtils.InterfaceCollections;

namespace RateMe
{
    /// <summary>
    /// Логика взаимодействия для DataCollection.xaml
    /// </summary>
    public partial class DataCollection : Window
    {
        private static readonly int NumberOfCourses = 4;
        private static readonly int NumberOfGroups = 10;
        private static readonly IEnumerable<string> CourseNumbers = Enumerable.Range(1, NumberOfCourses).Select(i => i.ToString());
        private static readonly IEnumerable<string> GroupNumbers = Enumerable.Range(1, NumberOfGroups).Select(i => i.ToString());
        private static readonly string[] Terms = ["1", "2"];

        public DataCollection()
        {
            try
            {
                InitializeComponent();

                Curriculums curriculums = new Curriculums();
                CurriculumsComboBox.ItemsSource = curriculums;
                CurriculumsComboBox.SelectedItem = curriculums.First();

                CourseComboBox.ItemsSource = CourseNumbers;
                CourseComboBox.SelectedItem = CourseNumbers.First();

                GroupComboBox.ItemsSource = GroupNumbers;
                GroupComboBox.SelectedItem = GroupNumbers.First();

                TermComboBox.ItemsSource = Terms;
                TermComboBox.SelectedItem = Terms.Last();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
           
        }

        private void OnWindowClick(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show($"{NameTextBox.Data} {SurameTextBox.Data} {CurriculumsComboBox.Text} {CourseComboBox.SelectedValue} {CourseComboBox.SelectedValue} {TermComboBox.SelectedValue}");
            Keyboard.ClearFocus();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void OnContinueClick(object sender, RoutedEventArgs e)
        {
            int groupNumber = int.Parse(GroupComboBox.SelectedItem.ToString() ?? "");
            Student student = new Student(SurameTextBox.Data, NameTextBox.Data, groupNumber);

            string curriculum = CurriculumsComboBox.SelectedItem.ToString() ?? "";
            int course = int.Parse(CourseComboBox.SelectedItem.ToString() ?? "");
            int term = int.Parse(TermComboBox.SelectedItem.ToString() ?? "");

            SyllabusModel syllabus = new SyllabusModel(student, curriculum, course, term);

            MessageBox.Show(syllabus.ToString());
        }
    }
}
