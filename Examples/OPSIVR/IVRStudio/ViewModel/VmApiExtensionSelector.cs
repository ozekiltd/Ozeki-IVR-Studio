using System;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using OPS_IVR_Studio.Utils;
using OzCommon.Model;

namespace OPS_IVR_Studio.ViewModel
{
    class VmApiExtensionSelector
    {
        private IClient client;

        public VmApiExtensionSelector()
        {
            Init();
        }

        private void Init()
        {
            client = SimpleIoc.Default.GetInstance<IClient>();
            Ok = new RelayCommand<string>(GetAPiExtension);
            Cancel = new RelayCommand(CancelStarting);
        }

        public void CancelStarting()
        {
            if (Mouse.OverrideCursor != null) { return; }
            Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowApiExtension, MsgCommand.CloseWindow));
        }

        private void GetAPiExtension(string extensionName)
        {
            if (Mouse.OverrideCursor !=null) { return; }
            if (string.IsNullOrWhiteSpace(extensionName) || string.IsNullOrEmpty(extensionName))
                return;
            Mouse.OverrideCursor = Cursors.Wait;
            try
            {
                client.GetAPIExtensionAsync(extensionName, extension =>
                    {
                        App.Current.Dispatcher.BeginInvoke(new Action(() => { Mouse.OverrideCursor = null; }));
                        if (extension == null)
                            MessageBox.Show("Cannot find the given Api Extension please try another.");
                        else
                        {
                            //  Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowApiExtension, MsgCommand.CloseWaitWindow));
                            Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowApiExtension,
                                                                             MsgCommand.CloseWindow));
                            Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowMain,
                                                                             MsgCommand.StartWithExtension, extension));
                        }
                    });
            }
            catch (Exception)
            {
                Mouse.OverrideCursor = null;
            }
        }

      

        public RelayCommand<string> Ok { get; private set; }
        public RelayCommand Cancel { get; private set; }


      
    }
}
