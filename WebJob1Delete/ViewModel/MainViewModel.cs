using Microsoft.Web.WebView2.Core;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

namespace WebJob1Delete.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        private string _textInWeb = string.Empty;
        public string TextInWeb
        {
            get { return _textInWeb; }
            set
            {
                if (_textInWeb != value)
                {
                    _textInWeb = value;
                    Debug.WriteLine($"TextInWeb изменен: {TextInWeb}");
                    OnPropertyChanged(nameof(TextInWeb));
                    SendTextToWpf.RaiseCanExecuteChanged(); // Уведомляем команду о необходимости перепроверки
                }
            }
        }

        private string _textInTextBox = string.Empty;
        public string TextInTextBox
        {
            get { return _textInTextBox; }
            set
            {
                if (_textInTextBox != value)
                {
                    _textInTextBox = value;
                    OnPropertyChanged(nameof(TextInTextBox));
                }
            }
        }
        private string _textInTextBlock = string.Empty;
        public string TextInTextBlock
        {
            get { return _textInTextBlock; }
            set
            {
                if (_textInTextBlock != value) _textInTextBlock = value;
                OnPropertyChanged(nameof(TextInTextBlock));
            }
        }

        private CoreWebView2? _coreWebView;
        public CoreWebView2? CoreWebView
        {
            get { return _coreWebView; }
            set
            {
                _coreWebView = value;
                OnPropertyChanged(nameof(CoreWebView));
            }
        }

        public MainViewModel()
        {
            SendTextToWebView = new RelayCommand(TextToWebView, CanTextToWebView);
            SendTextToWpf = new RelayCommand(TextToWpf, CanTextToWpf);
        }

        private bool CanTextToWpf(object? obj)
        {
            Debug.WriteLine($"CanExecute? {TextInWeb.Length > 0}");
            //return !string.IsNullOrEmpty(TextInWeb);
            return true;
        }

        private void TextToWpf(object? obj)
        {
            TextInTextBlock = TextInWeb;
        }

        private bool CanTextToWebView(object? arg)
        {
            return !string.IsNullOrEmpty(TextInTextBox);
        }

        private async void TextToWebView(object? obj)
        {
            if (CoreWebView != null)
            {
                // Используем функцию setTextFromCSharp, определенную в HTML
                Debug.WriteLine($"Отправляю в WebView2: {TextInTextBox}");
                string script = $"setTextFromCSharp('{TextInTextBox}');";
                await CoreWebView.ExecuteScriptAsync(script);
            }
        }

        public void InitializeWebView(CoreWebView2 webView)
        {
            CoreWebView = webView;
            CoreWebView.WebMessageReceived += CoreWebView_WebMessageReceived;
        }

        private void CoreWebView_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string textFromWeb = e.WebMessageAsJson.Trim('"');
            TextInWeb = textFromWeb;
        }

        public RelayCommand SendTextToWebView { get; set; }
        public RelayCommand SendTextToWpf { get; set; }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
