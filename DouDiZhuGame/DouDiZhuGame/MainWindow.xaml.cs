using System.Windows;

namespace DouDiZhuGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            Width = SystemParameters.WorkArea.Width / 1.2;
            Height = SystemParameters.WorkArea.Height / 1.2;
            
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MyGameTable.StartDispatched();
            MyBtnStart.Visibility = Visibility.Collapsed;
        }
    }
}
