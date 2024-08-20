namespace WorkRestTool
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.IO;
    using System.Linq;
    using System.Text.Json;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private string savePath = $@"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\{nameof(WorkRestTool)}\save.json";

        public App()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            Dispatcher.UnhandledException += (s, e) =>
            {
                MessageBox.Show(e.Exception.ToString());
            };

            try
            {
                if (File.Exists(savePath))
                {
                    Save.Instance = JsonSerializer.Deserialize<Save>(File.ReadAllText(savePath, Encoding.UTF8));
                }
            }
            catch { }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            if (Save.Instance != null)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(savePath));
                File.WriteAllText(savePath, JsonSerializer.Serialize(Save.Instance));
            }
        }
    }
}
