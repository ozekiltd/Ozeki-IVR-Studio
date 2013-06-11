using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using OPS_IVR_Studio.Utils;
using OPS_IVR_Studio.ViewModel;
using OzCommon.View;

namespace OPS_IVR_Studio.Views
{
    /// <summary>
    /// Interaction logic for WindowAPIExtensionSelector.xaml
    /// </summary>
    public partial class WindowAPIExtensionSelector : Window
    {

        WaitWindow waitWindow;

        public WindowAPIExtensionSelector()
        {
            InitializeComponent();
            Closed += WindowAPIExtensionSelector_Closed;
            Messenger.Default.Register<NotificationMessageEx>(this, NotificationMessageReceived);
        }

        void WindowAPIExtensionSelector_Closed(object sender, EventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessageEx>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessageEx message)
        {
              Dispatcher.BeginInvoke(new Action(() =>
            {
            if (message.Notification == MsgDestination.WindowApiExtension) // Csak akkor foglalkozunk az özenettel, ha nekünk szól
            {
                switch (message.Command)
                {
                    case MsgCommand.CloseWindow:
                        this.Close();
                        break;
                    case MsgCommand.ShowDialogQuestion:
                        DialogMessageEx dialogMessageEx = (DialogMessageEx)message.Parameters[0];

                        var result = MessageBox.Show(dialogMessageEx.Content, dialogMessageEx.Caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);
                        dialogMessageEx.Callback.Invoke(result);
                        break;
                    case MsgCommand.ShowWaitWindow:
                      
                                if (waitWindow != null)
                                    waitWindow.Close();
                                waitWindow = new WaitWindow("Loading please wait...");
                                waitWindow.Closed += waitWindow_Closed;
                                waitWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                                waitWindow.Owner = this;
                                waitWindow.Show();
                          
                        break;
                    case MsgCommand.CloseWaitWindow:
                        
                        if (waitWindow != null)
                        {
                            waitWindow.Closed -= waitWindow_Closed;
                            waitWindow.Close();
                            waitWindow = null;
                        }
                          
                        break;
                }
            }
            }));
        }


        void waitWindow_Closed(object sender, EventArgs e)
        {
            ((VmApiExtensionSelector)DataContext).CancelStarting();
            waitWindow.Closed -= waitWindow_Closed;
        }
    }
}
