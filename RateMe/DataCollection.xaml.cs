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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using RateMe.DataUtils;
using RateMe.DataUtils.InterfaceCollections;
using static System.Net.Mime.MediaTypeNames;

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
        private static readonly string[] Terms = ["1", "2"];
        private static readonly int BallsCount = 6;
        private static readonly int BallRadius = 5;
        private static readonly int BallsCircleRadius = 20;
        private static readonly double BallsCircleArc = Math.PI * 4.0 / 5.0;

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

            //MessageBox.Show(syllabus.ToString());

            WaitTextBlock.Visibility = Visibility.Visible;
            CreateLoadingBalls();
            // System.Windows.Application.Current.Dispatcher.Invoke(() => AnimateLoadingBalls(BallsCanvas));
        }

        private void CreateLoadingBalls()
        {
            NameScope.SetNameScope(this, new NameScope());
            
            PathGeometry infPath = GetAnimationPathGeometry();

            double xCenter = 0;
            double yCenter = BallsCircleRadius;
            double angStep = BallsCircleArc / BallsCount;

            // 1!!!
            for (int i = 0; i < 1; i++)
            {
                Ellipse ball = new Ellipse();
                ball.Name = $"Ball{i+1}";
                RegisterName(ball.Name, ball); //<Ellipse x:Name="Ball" HorizontalAlignment="Center" VerticalAlignment="Center" Height="7"  Width="7" Stroke="White" Fill="White"/>
                ball.Width = BallRadius;
                ball.Height = BallRadius;
                ball.Fill = Brushes.White;
                ball.Stroke = Brushes.White;

                double yChange = BallsCircleRadius * Math.Cos(angStep * i);
                double xChange = BallsCircleRadius * Math.Sin(angStep * i);
                
                double x = xCenter + xChange;
                double y = yCenter - yChange;

                MatrixTransform matrixTransform = new MatrixTransform();
                ball.RenderTransform = matrixTransform;
                RegisterName($"BallMatrixTransform{i+1}", matrixTransform);

                MatrixAnimationUsingPath matrixAnimation = new MatrixAnimationUsingPath();
                matrixAnimation.PathGeometry = infPath;
                matrixAnimation.Duration = TimeSpan.FromSeconds(5);
                matrixAnimation.RepeatBehavior = RepeatBehavior.Forever;

                BallsCanvas.Children.Add(ball);
                ball.SetValue(Canvas.LeftProperty, -35.0);
                ball.SetValue(Canvas.TopProperty, 20.0);

                Storyboard.SetTargetName(matrixAnimation, $"BallMatrixTransform{i+1}");
                Storyboard.SetTargetProperty(matrixAnimation, new PropertyPath(MatrixTransform.MatrixProperty));

                _ballsStoryBoard.Children.Add(matrixAnimation);

                ball.Loaded += delegate (object sender, RoutedEventArgs e)
                {
                    _ballsStoryBoard.Begin(this);
                };
            }


            Ellipse ball1 = new Ellipse(); 
            ball1.Width = BallRadius + 5;
            ball1.Height = BallRadius + 5;
            ball1.Fill = Brushes.White;
            ball1.Stroke = Brushes.White;

            //MatrixTransform matrixTransform = new MatrixTransform();
            //ball1.RenderTransform = matrixTransform;
            //RegisterName("BallMatrixTransform", matrixTransform);

            MainGrid.Children.Add(ball1);

            PathGeometry animationPath = new PathGeometry();
            PathFigure pFigure = new PathFigure();
            pFigure.StartPoint = new Point(10, 100);

            PolyBezierSegment pBezierSegment = new PolyBezierSegment([], true);
            pBezierSegment.Points.Add(new Point(35, 0));
            pBezierSegment.Points.Add(new Point(135, 0));
            pBezierSegment.Points.Add(new Point(160, 100));
            pBezierSegment.Points.Add(new Point(180, 190));
            pBezierSegment.Points.Add(new Point(285, 200));
            pBezierSegment.Points.Add(new Point(310, 100));

            ArcSegment rlArcSegment = new ArcSegment(new Point(10, 100), new Size(50, 50), Math.PI, true, SweepDirection.Counterclockwise, false);
            ArcSegment ruArcSegment = new ArcSegment(new Point(10, 100), new Size(50, 50), Math.PI, true, SweepDirection.Counterclockwise, false);

            pFigure.Segments.Add(pBezierSegment);
            pFigure.Segments.Add(rlArcSegment);

            animationPath.Figures.Add(pFigure);
            animationPath.Freeze();

            //MatrixAnimationUsingPath matrixAnimation = new MatrixAnimationUsingPath();
            //matrixAnimation.PathGeometry = animationPath;
            //matrixAnimation.Duration = TimeSpan.FromSeconds(5);
            //matrixAnimation.RepeatBehavior = RepeatBehavior.Forever;

            //Storyboard.SetTargetName(matrixAnimation, "BallMatrixTransform");
            //Storyboard.SetTargetProperty(matrixAnimation, new PropertyPath(MatrixTransform.MatrixProperty));

            //_ballsStoryBoard.Children.Add(matrixAnimation);

            ball1.Loaded += delegate (object sender, RoutedEventArgs e)
            {
                _ballsStoryBoard.Begin(this);
            };
        }


        private PathGeometry GetAnimationPathGeometry()
        {
            PathGeometry animationPath = new PathGeometry();
            PathFigure pFigure = new PathFigure();
            pFigure.StartPoint = new Point(-35, 20);

            PolyBezierSegment lowerBezierSegment = new PolyBezierSegment();
            lowerBezierSegment.Points.Add(new Point(-20, 5));
            lowerBezierSegment.Points.Add(new Point(-4, 10));            
            lowerBezierSegment.Points.Add(new Point(0, 20));
            lowerBezierSegment.Points.Add(new Point(4, 30));
            lowerBezierSegment.Points.Add(new Point(20, 35));
            lowerBezierSegment.Points.Add(new Point(35, 20));

            PolyBezierSegment upperBezierSegment = new PolyBezierSegment();
            lowerBezierSegment.Points.Add(new Point(20, 5));
            lowerBezierSegment.Points.Add(new Point(4, 10));
            lowerBezierSegment.Points.Add(new Point(-4, 30));
            lowerBezierSegment.Points.Add(new Point(-20, 35));
            lowerBezierSegment.Points.Add(new Point(-35, 20));

            ArcSegment rlArcSegment = new ArcSegment(new Point(35, 20), new Size(15, 13), Math.PI * 0.45, false, SweepDirection.Counterclockwise, false);
            ArcSegment ruArcSegment = new ArcSegment(new Point(20, 5), new Size(15, 13), Math.PI * 0.45, false, SweepDirection.Counterclockwise, false);

            //PolyBezierSegment upperBezierSegment = new PolyBezierSegment();
            //lowerBezierSegment.Points.Add(new Point(35, 0));
            //lowerBezierSegment.Points.Add(new Point(0, 20));
            //lowerBezierSegment.Points.Add(new Point(10, 35));

            //ArcSegment arcSegment = new ArcSegment(new Point(10, 100), new Size(50, 50), Math.PI / 4, true, SweepDirection.Counterclockwise, false);

            pFigure.Segments.Add(lowerBezierSegment);
            pFigure.Segments.Add(upperBezierSegment);


            animationPath.Figures.Add(pFigure);
            animationPath.Freeze();

            return animationPath;
        }
    }
}
