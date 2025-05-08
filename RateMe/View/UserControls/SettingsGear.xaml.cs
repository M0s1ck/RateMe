using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace RateMe.View.UserControls
{
    /// <summary>
    /// Логика взаимодействия для SettingsGear.xaml
    /// </summary>
    public partial class SettingsGear : UserControl
    {
        private readonly Storyboard _storyboard = new();
        private bool _touched = false;

        public SettingsGear()
        {
            InitializeComponent();

            int xCenter = 30;
            int yCenter = 30;
            int r = 9;
            int vertShift = 24;

            topCirc.Width = r * 2;
            topCirc.Height = r * 2;
            Canvas.SetLeft(topCirc, xCenter - r);
            Canvas.SetTop(topCirc, yCenter - vertShift - r);

            bottomCirc.Width = r * 2;
            bottomCirc.Height = r * 2;
            Canvas.SetLeft(bottomCirc, xCenter - r);
            Canvas.SetTop(bottomCirc, yCenter + vertShift - r);


            double tg30 = Math.Sqrt(3) / 3;

            int topSideXCatet = 21; //15
            int topSideYCatet = (int)Math.Round(topSideXCatet * tg30);
            int topSideCanvY = (yCenter - topSideYCatet) - r;

            topRightCirc.Width = r * 2;
            topRightCirc.Height = r * 2;
            int topRightCanvX = (xCenter + topSideXCatet) - r;

            Canvas.SetLeft(topRightCirc, topRightCanvX);
            Canvas.SetTop(topRightCirc, topSideCanvY);

            topLeftCirc.Width = r * 2;
            topLeftCirc.Height = r * 2;
            int topLeftCanvX = (xCenter - topSideXCatet) - r;

            Canvas.SetLeft(topLeftCirc, topLeftCanvX);
            Canvas.SetTop(topLeftCirc, topSideCanvY);


            int bottomSideXCatet = 21; //15
            int bottomSideYCatet = (int)Math.Round(bottomSideXCatet * tg30);
            int bottomSideCanvY = (yCenter + bottomSideYCatet) - r;

            bottomRightCirc.Width = r * 2;
            bottomRightCirc.Height = r * 2;
            int bottomRightCanvX = (xCenter + bottomSideXCatet) - r;

            Canvas.SetLeft(bottomRightCirc, bottomRightCanvX);
            Canvas.SetTop(bottomRightCirc, bottomSideCanvY);

            bottomLeftCirc.Width = r * 2;
            bottomLeftCirc.Height = r * 2;
            int bottomLeftCanvX = (xCenter - bottomSideXCatet) - r;

            Canvas.SetLeft(bottomLeftCirc, bottomLeftCanvX);
            Canvas.SetTop(bottomLeftCirc, bottomSideCanvY);

            SetSpinAnimation();
        }


        private void SpinGearWhenEnter(object sender, MouseEventArgs e)
        {
            if (_touched)
            {
                _storyboard.Resume(this);
                return;
            }

            _storyboard.Begin(this, true);
            _touched = true;
        }

        private void StopGearWhenLeave(object sender, MouseEventArgs e)
        {
            _storyboard.Pause(this);
        }


        private void SetSpinAnimation()
        {
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.Duration = TimeSpan.FromSeconds(0.5);
            doubleAnimation.From = 0;
            doubleAnimation.To = 60;
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            string rotateTransName = "RotateTransName";
            RegisterName(rotateTransName, canvRotateTransform);

            Storyboard.SetTargetName(doubleAnimation, rotateTransName);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath(RotateTransform.AngleProperty));
            _storyboard.Children.Add(doubleAnimation);
        }
    }
}
