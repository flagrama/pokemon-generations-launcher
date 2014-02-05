using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using PokeGen.ViewModel;

namespace PokeGen.View
{
    public partial class MainWindow
    {
        public MainWindow() {
            InitializeComponent();
        }

        private void TitleImage_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void ForumImage_MouseEnter(object sender, MouseEventArgs e) {
            ForumImage.Opacity = 0.75;
        }

        private void ForumImage_MouseLeave(object sender, MouseEventArgs e) {
            ForumImage.Opacity = 1;
        }

        private void ViewAll_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            Process.Start("http://www.moddb.com/games/pokemon-generations/news");
        }

        private void ViewAll_MouseEnter(object sender, MouseEventArgs e) {
            var newBrush = new SolidColorBrush {Color = Color.FromArgb(255, 255, 206, 0)};
            ViewAll.Foreground = newBrush;
        }

        private void ViewAll_MouseLeave(object sender, MouseEventArgs e) {
            var newBrush = new SolidColorBrush {Color = Color.FromArgb(255, 255, 255, 255)};
            ViewAll.Foreground = newBrush;
        }

        private void NewsItem1_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ((MainWindowViewModel) this.DataContext).NewsItem1Command.Execute(null);
        }

        private void NewsItem2_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ((MainWindowViewModel) this.DataContext).NewsItem2Command.Execute(null);
        }

        private void NewsItem3_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ((MainWindowViewModel) this.DataContext).NewsItem3Command.Execute(null);
        }

        private void NewsItem1_MouseEnter(object sender, MouseEventArgs e) {
            var newBrush = new SolidColorBrush {Color = Color.FromArgb(255, 255, 206, 0)};
            NewsItem1.Foreground = newBrush;
        }

        private void NewsItem1_MouseLeave(object sender, MouseEventArgs e) {
            var newBrush = new SolidColorBrush {Color = Color.FromArgb(255, 72, 151, 208)};
            NewsItem1.Foreground = newBrush;
        }

        private void NewsItem2_MouseEnter(object sender, MouseEventArgs e) {
            var newBrush = new SolidColorBrush {Color = Color.FromArgb(255, 255, 206, 0)};
            NewsItem2.Foreground = newBrush;
        }

        private void NewsItem2_MouseLeave(object sender, MouseEventArgs e) {
            var newBrush = new SolidColorBrush {Color = Color.FromArgb(255, 72, 151, 208)};
            NewsItem2.Foreground = newBrush;
        }

        private void NewsItem3_MouseEnter(object sender, MouseEventArgs e) {
            var newBrush = new SolidColorBrush {Color = Color.FromArgb(255, 255, 206, 0)};
            NewsItem3.Foreground = newBrush;
        }

        private void NewsItem3_MouseLeave(object sender, MouseEventArgs e) {
            var newBrush = new SolidColorBrush {Color = Color.FromArgb(255, 72, 151, 208)};
            NewsItem3.Foreground = newBrush;
        }

        private void ModDbImage_MouseEnter(object sender, MouseEventArgs e) {
            ModDbImage.Opacity = 0.75;
        }

        private void ModDbImage_MouseLeave(object sender, MouseEventArgs e) {
            ModDbImage.Opacity = 1;
        }

        private void TwitterImage_MouseEnter(object sender, MouseEventArgs e) {
            TwitterImage.Opacity = 0.75;
        }

        private void TwitterImage_MouseLeave(object sender, MouseEventArgs e) {
            TwitterImage.Opacity = 1;
        }

        private void mdbImg1_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ((MainWindowViewModel) this.DataContext).NewsPic1Command.Execute(null);
        }

        private void mdbImg2_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ((MainWindowViewModel) this.DataContext).NewsPic2Command.Execute(null);
        }

        private void mdbImg3_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ((MainWindowViewModel) this.DataContext).NewsPic3Command.Execute(null);
        }

        private void mdbImg1_MouseEnter(object sender, MouseEventArgs e) {
            mdbImg1.Opacity = 0.75;
        }

        private void mdbImg1_MouseLeave(object sender, MouseEventArgs e) {
            mdbImg1.Opacity = 1;
        }

        private void mdbImg2_MouseEnter(object sender, MouseEventArgs e) {
            mdbImg2.Opacity = 0.75;
        }

        private void mdbImg2_MouseLeave(object sender, MouseEventArgs e) {
            mdbImg2.Opacity = 1;
        }

        private void mdbImg3_MouseEnter(object sender, MouseEventArgs e) {
            mdbImg3.Opacity = 0.75;
        }

        private void mdbImg3_MouseLeave(object sender, MouseEventArgs e) {
            mdbImg3.Opacity = 1;
        }

        private void PathLabel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            ((MainWindowViewModel) this.DataContext).MovePathCommand.Execute(null);
        }
    }
}