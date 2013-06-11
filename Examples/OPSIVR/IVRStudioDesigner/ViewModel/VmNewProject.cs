using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using IVRStudio.Util;

namespace IVRStudio.ViewModel
{
    class VmNewProject : ViewModelBase
    {
        private VmIVRProject _ivrProject;

        public VmIVRProject IVRProject
        {
            get { return _ivrProject; }
            set { _ivrProject = value; 
            RaisePropertyChanged(()=>IVRProject);}
        }

        public VmNewProject()
        {
            Init();
        }


        public RelayCommand CommandOk { get; private set; }
        public RelayCommand CommandCancel { get; private set; }

        private void Init()
        {
            IVRProject = new VmIVRProject();
            CommandOk = new RelayCommand(Ok);
            CommandCancel = new RelayCommand(Cancel);
        }

        private void Cancel()
        {
           Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowNewProject, MsgCommand.Close));
        }

        private void Ok()
        {
            Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowMain, MsgCommand.UpdateProject, IVRProject));
            Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowNewProject, MsgCommand.Close));
        }
    }
}
