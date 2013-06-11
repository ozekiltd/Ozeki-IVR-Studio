using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Windows;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OzCommon.Model;
using OzCommon.Model.Mock;
using OzCommon.Utils;
using OzCommon.Utils.DialogService;
using OzCommon.View;
using OzCommon.ViewModel;


namespace OPS_IVR_Studio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private readonly SingletonApp _singletonApp;

        public App()
        {
            _singletonApp = new SingletonApp("OPSIVRStudio");
            InitDependencies();
        }

        void InitDependencies()
        {

            GalaSoft.MvvmLight.Ioc.SimpleIoc.Default.Register<IDialogService>(() => new DialogService());
            SimpleIoc.Default.Register<IUserInfoSettingsRepository>(() => new UserInfoSettingsRepository());
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Messenger.Default.Register<NotificationMessage>(this, MessageReceived);
           if (e.Args.Length != 0 && e.Args[0].ToLower() == "-mock")
                SimpleIoc.Default.Register<IClient>(() => new MockClient());
            else
               SimpleIoc.Default.Register<IClient>(() => new Client());

            _singletonApp.OnStartup(e);
            MainWindow = new LoginWindow();
            MainWindow.Show();
            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this, MessageReceived);
            base.OnExit(e);
        }

        private void MessageReceived(NotificationMessage notificationMessage)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                if (notificationMessage.Notification == Messages.NavigateToMainWindow)
                {
                    var mainWindow = new MainWindow();
                    mainWindow.Show();

                    Application.Current.MainWindow = mainWindow;
                }
            }));
        }
    }
}
