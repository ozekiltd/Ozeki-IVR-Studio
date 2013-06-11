using System;
using System.IO;
using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using OPSIVRSystem;
using OPSIVRSystem.CommonViewModel;
using OPSIVRSystem.IVRMenus;
using OPSIVRSystem.Utils;
using OPSSDK;
using OPS_IVR_Studio.Utils;
using OPS_IVR_Studio.Views;
using OzCommon.Model;
using OzCommon.ViewModel;

namespace OPS_IVR_Studio.ViewModel
{
    class VmMain : CommonMainViewModel<BaseModel> 
    {
        private IIVREngine ivrEngine;
        private string _logText;
       
        public VmMain()
        {
            Init();
        }

        public RelayCommand CommandStart { get; private set; }
        public RelayCommand CommandStop { get; private set; }
        public RelayCommand CommandOpenIVRProject { get; private set; }
        public RelayCommand<object> CommandTreeViewItemChanged { get; private set; }

        private IVRProject _currentProject;
        private string _loadedProjectPath;

        public IVRProject CurrentProject
        {
            get { return _currentProject; }
            set { _currentProject = value; RaisePropertyChanged(() => CurrentProject); }
        }

        public string LogText
        {
            get { return _logText; }
            set { _logText = value; RaisePropertyChanged(()=>LogText);}
        }

        public string LoadedProjectPath
        {
            get { return _loadedProjectPath; }
            set { _loadedProjectPath = value; RaisePropertyChanged(()=>LoadedProjectPath);}
        }

        private UserControl _ucPropertiesPart;
        private bool _isRunning;

        public UserControl UcPropertiesPart
        {
            get { return _ucPropertiesPart; }
            set
            {
                _ucPropertiesPart = value;
                RaisePropertyChanged(() => UcPropertiesPart);
            }
        }

        public bool IsRunning
        {
            get { return _isRunning; }
            set 
            {
                _isRunning = value;
                RaisePropertyChanged(() => IsRunning);
            }
        }

        private void Init()
        {
            LogText += "Successfully login to Ozeki Phone System XE" + Environment.NewLine;
     //       LogText = "";
            ivrEngine = new IVREngineOPSSDK();
            CommandStart = new RelayCommand(ShowApiExtensionPopup);
            CommandStop = new RelayCommand(StopIVREngine);
            CommandOpenIVRProject = new RelayCommand(OpenIVRProject);
            CommandTreeViewItemChanged = new RelayCommand<object>(TreeViewMenuItemChanged);
            ivrEngine.NotifyAction += ivrEngine_NotifyAction;
            UcPropertiesPart = new UcTipView();
        }

        private void TreeViewMenuItemChanged(object obj)
        {
            if (obj == null)
            {
                UcPropertiesPart = new UcTipView();
            }
            else if (obj is IVRMenuElementBase)
            {
                UcPropertiesPart = new UcMenuProperties((VmIVRMenuElementBase)obj);
            }
            else if (obj is TreeViewItem)
            {
                UcPropertiesPart = new UcMenuProperties((VmIVRMenuElementBase)((TreeViewItem)obj).Header);
            }
        }

        private void OpenIVRProject()
        {
            Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowMain, MsgCommand.ShowLoadDialog, new FileOperationMessage((
                resPath) =>
            {
                CurrentProject = new ProjectStore().LoadProject(resPath);
                LoadedProjectPath = resPath;
                LogEvent("IVR project Loaded from " + LoadedProjectPath);
                Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowMain, MsgCommand.PopulateTreeView, ConverterMenu.GetMenuViewModels(CurrentProject.MenuList)));
            })));
        }

 
        void ivrEngine_NotifyAction(object sender, OPSIVRSystem.Utils.GenericEventArgs<string> e)
        {
            LogEvent(e.Item);
        }

        private void LogEvent(string message)
        {
            LogText += message + Environment.NewLine;
        }

        private void ShowApiExtensionPopup()
        {
            if (!File.Exists(LoadedProjectPath))
            {
                OpenIVRProject();   
            }
            if (usedExtension!= null)
            {
                StartEnginewith(usedExtension);
                return;
            }
            Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowMain, MsgCommand.ShowWindowApiExtensionSelector));
        }

        private void StopIVREngine()
        {
            ivrEngine.Stop();
            IsRunning = false;
        }

        private IAPIExtension usedExtension;

        public void StartEnginewith(object extension )
        {
            usedExtension = (IAPIExtension) extension;
            ivrEngine.Start(LoadedProjectPath, usedExtension);
            IsRunning = true;
        }



    }
}
