using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DouDiZhuGame
{
    public class ButtonImage : Button
    {
        public ImageSource DefaultImage
        {
            get { return (ImageSource)GetValue(DefaultImageProperty); }
            set { SetValue(DefaultImageProperty, value); }
        }

        public static readonly DependencyProperty DefaultImageProperty =
            DependencyProperty.Register("DefaultImage", typeof(ImageSource), typeof(ButtonImage), new PropertyMetadata(null));

        public ImageSource MouseImage
        {
            get { return (ImageSource)GetValue(MouseImageProperty); }
            set { SetValue(MouseImageProperty, value); }
        }

        public static readonly DependencyProperty MouseImageProperty =
            DependencyProperty.Register("MouseImage", typeof(ImageSource), typeof(ButtonImage), new PropertyMetadata(null));

        public ImageSource PressedImage
        {
            get { return (ImageSource)GetValue(PressedImageProperty); }
            set { SetValue(PressedImageProperty, value); }
        }

        public static readonly DependencyProperty PressedImageProperty =
            DependencyProperty.Register("PressedImage", typeof(ImageSource), typeof(ButtonImage), new PropertyMetadata(null));

        static ButtonImage()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ButtonImage), new FrameworkPropertyMetadata(typeof(ButtonImage)));
        }
    }
}
