using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Курсова_робота
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            SplashScreen splash = new SplashScreen("ReServant3Done.png");
            splash.Show(autoClose: true, topMost: true);

            System.Threading.Thread.Sleep(2000); // затримка 2 секунди

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

    }
}
