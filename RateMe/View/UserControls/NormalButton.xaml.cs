using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RateMe.View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для NormalButton.xaml
    /// </summary>
    public partial class NormalButton : UserControl
    {
        public static readonly DependencyProperty BackgroundProp =
            DependencyProperty.Register(nameof(Back), typeof(Brush), typeof(NormalButton), new PropertyMetadata(Brushes.DarkSlateBlue));

        public static readonly DependencyProperty HoverBackgroundProp =
            DependencyProperty.Register(nameof(BackHover), typeof(Brush), typeof(NormalButton), new PropertyMetadata(Brushes.BlueViolet));

        public static readonly DependencyProperty ContentProp =
            DependencyProperty.Register(nameof(TheContent), typeof(string), typeof(NormalButton), new PropertyMetadata("Hello"));

        public static readonly DependencyProperty WidthProp =
            DependencyProperty.Register(nameof(TheWidth), typeof(int), typeof(NormalButton), new PropertyMetadata(120));

        public static readonly DependencyProperty HeightProp =
            DependencyProperty.Register(nameof(TheHeight), typeof(int), typeof(NormalButton), new PropertyMetadata(40));

        public static readonly DependencyProperty FontSizeProp =
            DependencyProperty.Register(nameof(TheFontSize), typeof(double), typeof(NormalButton), new PropertyMetadata(17.0));

        public static readonly DependencyProperty CornerRadiusProp =
            DependencyProperty.Register(nameof(TheCornerRadius), typeof(CornerRadius), typeof(NormalButton), new PropertyMetadata(new CornerRadius(4)));

        public static readonly DependencyProperty BorderThickProp =
            DependencyProperty.Register(nameof(BorderThick), typeof(Thickness), typeof(NormalButton), new PropertyMetadata(new Thickness(2)));

        public static readonly DependencyProperty MarginProp =
            DependencyProperty.Register(nameof(TheMargin), typeof(Thickness), typeof(NormalButton), new PropertyMetadata(new Thickness(0)));


        public static readonly RoutedEvent WhenClickedEvent = 
            EventManager.RegisterRoutedEvent(nameof(WhenClicked), RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(NormalButton));

        public event RoutedEventHandler? WhenClicked;


        public Brush Back
        {
            get => (Brush)GetValue(BackgroundProp);
            set => SetValue(BackgroundProp, value);
        }

        public Brush BackHover
        {
            get => (Brush)GetValue(HoverBackgroundProp);
            set => SetValue(HoverBackgroundProp, value);
        }

        public string TheContent
        {
            get => (string)GetValue(ContentProp);
            set => SetValue(ContentProp, value);
        }

        public int TheWidth
        {
            get => (int)GetValue(WidthProp);
            set => SetValue(WidthProp, value);
        }

        public int TheHeight
        {
            get => (int)GetValue(HeightProp);
            set => SetValue(HeightProp, value);
        }

        public double TheFontSize
        {
            get => (double)GetValue(FontSizeProp);
            set => SetValue(FontSizeProp, value);
        }

        public CornerRadius TheCornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProp);
            set => SetValue(CornerRadiusProp, value);
        }

        public Thickness BorderThick
        {
            get => (Thickness)GetValue(BorderThickProp);
            set => SetValue(BorderThickProp, value);
        }
        public Thickness TheMargin
        {
            get => (Thickness)GetValue(MarginProp);
            set => SetValue(MarginProp, value);
        }


        public NormalButton()
        {
            InitializeComponent();
        }

        private void OnClicked(object sender, RoutedEventArgs e)
        {
            RaiseEvent(new RoutedEventArgs(WhenClickedEvent));
        }
    }
}
