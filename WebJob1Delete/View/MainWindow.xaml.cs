using System.Windows; 
using System.IO; 
using WebJob1Delete.ViewModel; 
using Microsoft.Web.WebView2.Core;


namespace WebJob1Delete
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
            Window_Loaded();

            WebViewControl.CoreWebView2InitializationCompleted += WebViewControl_CoreWebView2InitializationCompleted;
        }

        private void WebViewControl_CoreWebView2InitializationCompleted(object? sender, CoreWebView2InitializationCompletedEventArgs e)
        {
            // Передаем CoreWebView2 в ViewModel
            if (WebViewControl.CoreWebView2 != null)
            {
                var viewModel = (MainViewModel)DataContext;
                viewModel.InitializeWebView(WebViewControl.CoreWebView2);
            }
        }

        private async void Window_Loaded()
        {
            #region Поиск файла в проекте
            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string htmlPath = Path.Combine(appDirectory, "Web", "WebWindow.html");

            if (!Path.Exists(htmlPath))
            {
                MessageBox.Show($"Файл не найден: {htmlPath}");
                return;
            }
            #endregion

            WebViewControl.Source = new Uri(htmlPath);
            await WebViewControl.EnsureCoreWebView2Async();
        }
    }
}