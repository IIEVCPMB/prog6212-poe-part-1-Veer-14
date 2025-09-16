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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Prog6212___POE.Pages
{
    /// <summary>
    /// Interaction logic for HomePage.xaml
    /// </summary>
    public partial class HomePage : Page
    {
        private Frame _mainFrame;

        public HomePage(Frame mainFrame)
        {
            InitializeComponent();
            _mainFrame = mainFrame;
        }

        private void LecturerBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _mainFrame.Navigate(new LecturerDashboard(_mainFrame));
        }

        private void CoordinatorBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            _mainFrame.Navigate(new CoordinatorDashboard(_mainFrame));
        }

        private void ManagerBtn_Click(object sender, System.Windows.RoutedEventArgs e)
        {

            _mainFrame.Navigate(new ManagerDashboard(_mainFrame));
        }
        private void RegisterBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Register(_mainFrame));
        }
        private void LoginBtn_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new Login(_mainFrame));
        }
    }
}
