using CodexAssistant.ViewModels;
using System.Windows;

namespace CodexAssistant.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var viewModel = DataContext as MainWindowViewModel;
            viewModel.LogRichTextBox = LogRichTextBox;
        }
    }
}
