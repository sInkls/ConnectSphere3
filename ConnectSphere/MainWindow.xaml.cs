using System.Windows;
using System.Windows.Controls;

namespace SocialAppViewer
{
    public partial class MainWindow : Window
    {
        private readonly MainViewModel _viewModel;

        public MainWindow()
        {
            InitializeComponent();
            _viewModel = new MainViewModel();
            DataContext = _viewModel;

            AddRow.Click += (s, e) => _viewModel.AddRow();
            DeleteRow.Click += (s, e) => _viewModel.DeleteRow();
            SaveChanges.Click += (s, e) => _viewModel.SaveChanges();
            ExportJson.Click += (s, e) => _viewModel.ExportJson();
            ImportJson.Click += (s, e) => _viewModel.ImportJson();
        }
    }
}