using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RateMe.DataUtils.InterfaceCollections;
using RateMe.DataUtils.Models;
using RateMe.Parser;
using RateMe.View.UserControls;

namespace RateMe
{
    /// <summary>
    /// Логика взаимодействия для DataCollection.xaml
    /// </summary>
    public partial class DataCollection : Window
    {
        private Storyboard _ballsStoryBoard = new Storyboard();

        private static readonly int NumberOfCourses = 4;
        private static readonly int NumberOfGroups = 10;
        private static readonly IEnumerable<string> CourseNumbers = Enumerable.Range(1, NumberOfCourses).Select(i => i.ToString());
        private static readonly IEnumerable<string> GroupNumbers = Enumerable.Range(1, NumberOfGroups).Select(i => i.ToString());
        private static readonly string[] Terms = ["1", "2", "3", "4"];

        private static readonly PathGeometry LoadingBallsLoadingAnimationPath = GetLoadingBallsLoadingAnimationPathInfinity();
        private static readonly Tuple<double, double> LoadingBallsStartingCords = new(-35.0, 20.0);
        private static readonly int BallsCount = 20;
        private static readonly int BallRadius = 5;


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

                WindowBarDockPanel bar = new(this);
                WindowGrid.Children.Add(bar);

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }



        }


        private async void OnContinueClick(object sender, RoutedEventArgs e)
        {
            // Loading ("wait") starts
            WaitTextBlock.Visibility = Visibility.Visible;
            LaunchLoadingBalls();
            ContinueButton.IsEnabled = false;

            // Collected data building up
            int groupNumber = int.Parse(GroupComboBox.SelectedItem.ToString() ?? "");
            Student student = new Student(SurameTextBox.Data, NameTextBox.Data, groupNumber);

            string curriculum = CurriculumsComboBox.SelectedItem.ToString() ?? "";
            int course = int.Parse(CourseComboBox.SelectedItem.ToString() ?? "");
            int term = int.Parse(TermComboBox.SelectedItem.ToString() ?? "");

            SyllabusModel syllabus = new SyllabusModel(student, curriculum, course, term);

            // Parsing(Web - sc..)
            MainParser mainParser = new MainParser(syllabus);
            await mainParser.GetCurriculumAsync();
            await mainParser.GetSubjectsUrlsAltAsync();
            List<Subject> subjects = await mainParser.GetSubjectsDataAsync();

            //List<Subject> subs = [new Subject("Алгебра", 9, [1, 2, 3, 4], []), new Subject("Матан", 9, [1, 2, 3, 4], []), new Subject("Экономика", 3, [3, 4], [])];

            SubjectsWin subjectsWin= new(syllabus, subjects);
            subjectsWin.Show();

            Close();


        }


        private void OnWindowClick(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }


        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }


        private void LaunchLoadingBalls()
        {
            for (int i = 0; i < BallsCount; i++)
            {
                LaunchBall(i);
            }

            _ballsStoryBoard.Begin(this);
        }


        private void LaunchBall(int i)
        {
            Ellipse ball = new Ellipse();
            ball.Width = BallRadius;
            ball.Height = BallRadius;
            ball.Fill = Brushes.White;
            ball.Stroke = Brushes.White;
            ball.Name = $"Ball{i}";
            ball.Visibility = Visibility.Hidden;
            RegisterName(ball.Name, ball);

            MatrixTransform matrixTransform = new MatrixTransform();
            ball.RenderTransform = matrixTransform;
            string matrixTransformName = $"BallMatrixTransform{i}";
            RegisterName(matrixTransformName, matrixTransform);

            MatrixAnimationUsingPath matrixAnimation = new MatrixAnimationUsingPath();
            matrixAnimation.PathGeometry = LoadingBallsLoadingAnimationPath;
            matrixAnimation.Duration = TimeSpan.FromSeconds(5);
            matrixAnimation.RepeatBehavior = RepeatBehavior.Forever;
            matrixAnimation.BeginTime = TimeSpan.FromMilliseconds(250 * i);

            // Makes balls visible only when animation BeginTime passes
            EventHandler onmatrixAnimationBegin = (sender, args) => { ball.Visibility = Visibility.Visible; };
            matrixAnimation.CurrentTimeInvalidated += onmatrixAnimationBegin;

            BallsCanvas.Children.Add(ball);
            ball.SetValue(Canvas.LeftProperty, LoadingBallsStartingCords.Item1);
            ball.SetValue(Canvas.TopProperty, LoadingBallsStartingCords.Item2);

            Storyboard.SetTargetName(matrixAnimation, matrixTransformName);
            Storyboard.SetTargetProperty(matrixAnimation, new PropertyPath(MatrixTransform.MatrixProperty));

            _ballsStoryBoard.Children.Add(matrixAnimation);
        }


        private static PathGeometry GetLoadingBallsLoadingAnimationPathInfinity()
        {
            PathGeometry animationPath = new PathGeometry();
            PathFigure pFigure = new PathFigure();
            pFigure.StartPoint = new Point(-35, 20);

            PolyBezierSegment lowerBezierSegment = new PolyBezierSegment();
            lowerBezierSegment.Points.Add(new Point(-20, 2));
            lowerBezierSegment.Points.Add(new Point(-4, 10));            
            lowerBezierSegment.Points.Add(new Point(0, 20));
            lowerBezierSegment.Points.Add(new Point(4, 30));
            lowerBezierSegment.Points.Add(new Point(20, 38));
            lowerBezierSegment.Points.Add(new Point(35, 20));

            PolyBezierSegment upperBezierSegment = new PolyBezierSegment();
            lowerBezierSegment.Points.Add(new Point(20, 2));
            lowerBezierSegment.Points.Add(new Point(4, 10));
            lowerBezierSegment.Points.Add(new Point(0, 20));
            lowerBezierSegment.Points.Add(new Point(-4, 30));
            lowerBezierSegment.Points.Add(new Point(-20, 38));
            lowerBezierSegment.Points.Add(new Point(-35, 20));

            pFigure.Segments.Add(lowerBezierSegment);
            pFigure.Segments.Add(upperBezierSegment);

            animationPath.Figures.Add(pFigure);
            animationPath.Freeze();

            return animationPath;
        }
    }
}
