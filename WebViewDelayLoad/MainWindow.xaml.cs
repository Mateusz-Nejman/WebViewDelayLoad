using CefSharp;
using CefSharp.OffScreen;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace WebViewDelayLoad
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly ChromiumWebBrowser webBrowser;
        const int delay = 1000;
        const string htmlLink = @"https://www.tradingview.com/";
        
        Task task = null;
        bool isTaskStarted = false;
        public MainWindow()
        {
            InitializeComponent();

            var settings = new CefSettings()
            {
                //By default CefSharp will use an in-memory cache, you need to specify a Cache Folder to persist data
                CachePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CefSharp\\Cache")
            };

            Cef.Initialize(settings, performDependencyCheck: true, browserProcessHandler: null);
            webBrowser = new ChromiumWebBrowser(htmlLink);
            webBrowser.LoadingStateChanged += WebBrowser_LoadingStateChanged;

            

        }

        private void WebBrowser_LoadingStateChanged(object sender, LoadingStateChangedEventArgs e)
        {
            if(!e.IsLoading)
            {
                if(!isTaskStarted)
                {
                    isTaskStarted = true;
                    task = new Task(() => {
                        Thread.Sleep(delay);
                        Dispatcher.Invoke(async () => {
                            string text = await webBrowser.GetBrowser().MainFrame.GetSourceAsync();
                            textBoxSource.Text = text;


                        });
                    });
                    task.Start();
                }
            }
        }
    }
}
