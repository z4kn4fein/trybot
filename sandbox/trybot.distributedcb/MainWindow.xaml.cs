namespace Trybot.DistributedCB
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.Loaded += (s, e) => this.DataContext = new MainViewModel();
            this.InitializeComponent();
        }

    }
}
