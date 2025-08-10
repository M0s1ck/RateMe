using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using RateMe.Models.ClientModels;
using RateMe.Models.InterfaceModels;
using RateMe.Parser;
using RateMe.Utils.LocalHelpers;

namespace RateMe.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для DataCollection.xaml
    /// </summary>
    public partial class DataCollection : BaseFullWin
    {
        private Storyboard _ballsStoryBoard = new Storyboard();

        #region staticConsts
        
        private static readonly int NumberOfCourses = 4;
        private static readonly int NumberOfGroups = 10;
        private static readonly IEnumerable<string> CourseNumbers = Enumerable.Range(1, NumberOfCourses).Select(i => i.ToString());
        private static readonly IEnumerable<string> GroupNumbers = Enumerable.Range(1, NumberOfGroups).Select(i => i.ToString());
        private static readonly string[] Terms = ["1", "2", "3", "4"];

        private static readonly PathGeometry LoadingBallsLoadingAnimationPath = GetLoadingBallsLoadingAnimationPathInfinity();
        private static readonly Tuple<double, double> LoadingBallsStartingCords = new(-35.0, 20.0);
        private static readonly int BallsCount = 20;
        private static readonly int BallRadius = 5;
        
        #endregion
        

        public DataCollection()
        {
            InitializeComponent();

            Curriculums curriculums = new Curriculums();
            Loaded += (_, _) => SetItemSources(curriculums);
            Loaded += (_, _) => AddHeaderBar(WindowGrid); 
        }


        private async void OnContinueClick(object sender, RoutedEventArgs e)
        {
            // Loading ("wait") starts
            WaitTextBlock.Visibility = Visibility.Visible;
            LaunchLoadingBalls();
            ContinueButton.IsEnabled = false;

            // Collected data building up
            
            SyllabusModel syllabus = HandleSyllabus();
            
            // Parsing(Web - sc..)
            MainParser mainParser = new MainParser(syllabus);
            await mainParser.GetCurriculumAsync();
            await mainParser.GetSubjectsUrlsAltAsync();
            List<Subject> subjects = await mainParser.GetSubjectsDataAsync();

            // Subject alg = new Subject("Алгебра1", 9, [1, 2, 3, 4], []);
            // alg.FormulaObj = new Formula("0,21∙О_(Кр-3мод)+ 0,1∙О_(Сем-2)+0,08∙О_(ИДЗ-3 и 4 мод)+ 0,21∙О_(Коллоквиум-3 и 4мод)+0,5∙О_(Экз.раб.-2)");
            //
            // Subject disc = new Subject("Discra", 9, [1, 2, 3, 4], []);
            // disc.FormulaObj = new Formula("0.09 * ДЗ 4 + 0.105 * КР 3 + 0.105 * КР 4 + 0.7 * Э 4");
            //
            // Subject hist = new Subject("History", 9, [1, 2, 3, 4], []);
            // hist.FormulaObj = new Formula("0.2 * Проектная деятельность + 0.25 * Работа с СмартЛМС + 0.3 * Семинарские занятия + 0.25 * Экзамен");
            //
            // Subject eco = new Subject("Economics", 9, [1, 2, 3, 4], []);
            // eco.FormulaObj = new Formula("0.13 * Выполнение тестов онлайн-курса + 0.29 * Контрольная работа №1 (микроэкономика) + 0.29 * Контрольная работа №2 (макроэкономика) + 0.29 * Оценка за работу на семинарах");

            //List<Subject> subjects = [alg, disc, hist, eco]; // new Subject("Алгебра11", 9, [1, 2, 3, 4], []), new Subject("научно-исследовательский семинар Матан2", 9, [1, 2, 3, 4], []), new Subject("Экономика3", 3, [3, 4], []),
                                  //new Subject("Алгебраnvsknksvnk4", 9, [1, 2, 3], []), new Subject("Матанsvmsmvlmslvmlsv5", 9, [3, 4], []), new Subject("Экономика6", 3, [3, 4], []),
                                  //new Subject("Алгебра7", 9, [1, 2, 3, 4], []), new Subject("Матан8", 9, [1, 2, 3, 4], []), new Subject("Экономика9", 3, [3, 4], []),
                                  //new Subject("Алгебра10", 9, [1, 2, 3, 4], []), new Subject("Матан,vs,v;s,;v,;s,v;,sv,s;v,sv;s,vvs;s,;,sv;s,v;s11", 9, [1, 2, 3], []), new Subject("Экономика12", 3, [3, 4], []),
                                  //new Subject("Алгебра13", 9, [1, 2, 3, 4], []), new Subject("Матан14", 9, [4], []), new Subject("Экономика15", 3, [3, 4], []),
                                  //new Subject("АлгебраNjnjnvnvjsnvjnsv vsjnvjsnvjsnv16", 9, [3, 4], []), new Subject("научно-исследовательский семинар Мааьаь", 9, [1, 2, 3, 4], []),
                                  //new Subject("Алгебраscscscscscsccsc18", 9, [1, 2], []), new Subject("научно-исследовательский семинар облака", 9, [1, 2, 3, 4], [])];



            SubjectsWin subjectsWin = new(syllabus, subjects);
            subjectsWin.Show();
            Close();            
        }

        private SyllabusModel HandleSyllabus()
        {
            int groupNumber = int.Parse(GroupComboBox.SelectedItem.ToString() ?? "");
            Student student = new Student(SurameTextBox.Data, NameTextBox.Data, groupNumber);

            string curriculum = CurriculumsComboBox.SelectedItem.ToString() ?? "";
            int course = int.Parse(CourseComboBox.SelectedItem.ToString() ?? "");
            int term = int.Parse(TermComboBox.SelectedItem.ToString() ?? "");

            SyllabusModel syllabus = new(student, curriculum, course, term);
            JsonFileHelper.SaveSyllabus(syllabus);

            return syllabus;
        }

        private void SetItemSources(Curriculums curriculums)
        {
            CurriculumsComboBox.ItemsSource = curriculums;
            CurriculumsComboBox.SelectedItem = curriculums.First();

            CourseComboBox.ItemsSource = CourseNumbers;
            CourseComboBox.SelectedItem = CourseNumbers.First();

            GroupComboBox.ItemsSource = GroupNumbers;
            GroupComboBox.SelectedItem = GroupNumbers.First();

            TermComboBox.ItemsSource = Terms;
            TermComboBox.SelectedItem = Terms.Last();
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
            EventHandler onmatrixAnimationBegin = (_, _) => { ball.Visibility = Visibility.Visible; };
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
