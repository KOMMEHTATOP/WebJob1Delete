using Microsoft.Web.WebView2.Core;
using System.ComponentModel;
using System.Diagnostics;

namespace WebJob1Delete.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region Отвлекающий мусор
        public event PropertyChangedEventHandler? PropertyChanged;

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

        public RelayCommand SendTextToWebView { get; set; }
        public RelayCommand SendTextToWpf { get; set; }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public MainViewModel()
        {
            SendTextToWebView = new RelayCommand(TextToWebView, CanTextToWebView);
            SendTextToWpf = new RelayCommand(TextToWpf, CanTextToWpf);
        }
        #endregion

        #region Свойства класса

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
                if (_textInTextBlock != value)
                {
                    _textInTextBlock = value;
                    OnPropertyChanged(nameof(TextInTextBlock));
                }
            }
        }

        private string _newTextInWebView = string.Empty;

        public string NewTextInWebView
        {
            get { return _newTextInWebView; }
            set
            {
                if (_newTextInWebView != value)
                {
                    _newTextInWebView = value;
                    OnPropertyChanged(nameof(NewTextInWebView));
                    SendTextToWpf.RaiseCanExecuteChanged();
                }
            }
        }

        #endregion

        #region SendTextToWebView
        private bool CanTextToWebView(object? arg)
        {
            return !string.IsNullOrEmpty(TextInTextBox);
        }

        private async void TextToWebView(object? obj)
        {
            if (CoreWebView != null)
            {
                string script = $"setTextFromCSharp('{TextInTextBox}');";
                await CoreWebView.ExecuteScriptAsync(script);
            }
        }


        #endregion

        #region SendTextToWpf
        private bool CanTextToWpf(object? obj)
        {
            return !string.IsNullOrEmpty(NewTextInWebView);
        }

        private void TextToWpf(object? obj)
        {
            TextInTextBlock = NewTextInWebView;
        }

        #endregion

        public void InitializeWebView(CoreWebView2 webView)
        {
            CoreWebView = webView;
            CoreWebView.WebMessageReceived += CoreWebView_WebMessageReceived;
        }

        private void CoreWebView_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            string textFromWeb = e.WebMessageAsJson.Trim('"');
            NewTextInWebView = textFromWeb;
        }
    }
}
