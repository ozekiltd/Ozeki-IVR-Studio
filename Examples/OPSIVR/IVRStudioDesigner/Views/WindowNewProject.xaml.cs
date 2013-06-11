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
using IVRStudio.Model;
using IVRStudio.Util;

namespace IVRStudio.Views
{
    /// <summary>
    /// Interaction logic for WindowNewProject.xaml
    /// </summary>
    public partial class WindowNewProject : Window
    {
        public WindowNewProject()
        {
            InitializeComponent();
            this.Closing += WindowNewProject_Closing;
            Messenger.Default.Register<NotificationMessageEx>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessageEx message)
        {
            if (message.Notification == MsgDestination.WindowNewProject)
            {
                switch (message.Command)
                {
                    case MsgCommand.Close:
                      this.Close();
                        break;
                }
            }
        }

        void WindowNewProject_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Messenger.Default.Unregister(this );
        }

       
    }
}
