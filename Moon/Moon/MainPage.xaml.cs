using System;
using System.Windows;
using Microsoft.Phone.Controls;
using System.Windows.Threading;

namespace Moon
{
    public partial class MainPage : PhoneApplicationPage
    {
        private DispatcherTimer timer { get; set; }

        // Constructor
        public MainPage()
        {
            InitializeComponent();

            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Start();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (FeedBackOverlay.Visibility != System.Windows.Visibility.Visible)
            {
                NavigationService.Navigate(new Uri("/MainMenuPage.xaml", UriKind.Relative));
            }

            timer.Stop();
        }

        private void FeedBackOverlay_VisibilityChanged(object sender, EventArgs e)
        {
            if (FeedBackOverlay.Visibility == System.Windows.Visibility.Collapsed)
            {
                timer = new DispatcherTimer();
                timer.Tick += new EventHandler(timer_Tick);
                timer.Interval = TimeSpan.FromMilliseconds(500);
                timer.Start();
            }
        }
    }
}