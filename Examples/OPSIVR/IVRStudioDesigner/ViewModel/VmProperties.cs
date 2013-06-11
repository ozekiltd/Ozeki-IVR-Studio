using System.Windows.Controls;
using GalaSoft.MvvmLight.Command;
using IVRStudio.Views;
using OPSIVRSystem.CommonViewModel;
using OPSIVRSystem.IVRMenus;

namespace IVRStudio.ViewModel
{
    class VmProperties 
    {
        //designer miatt
        public VmProperties()
        {}

        public VmProperties(VmIVRMenuElementBase menu)
        {
            CurrentIVRMenu = menu;
        }


        public RelayCommand CommandShowAudioFileOpen { get; private set; }

        private VmIVRMenuElementBase _currentIVRMenu;
        public VmIVRMenuElementBase CurrentIVRMenu
        {
            get { return _currentIVRMenu; }
            set
            {
                _currentIVRMenu = value;
                CreateApropiatePartControl(value);
            }
        }

        private void Init()
        {
            CommandShowAudioFileOpen=new RelayCommand(ShowOpenAudioFileDialog);
        }

        private void ShowOpenAudioFileDialog()
        {
            
        }

        internal void SetAudioFilePath(string filepath)
        {
            CurrentIVRMenu.AudioFile = filepath;
        }

        internal void SetPostIntroductionAudio(string filepath)
        {
            if (CurrentIVRMenu is VmIVRMenuElementVoiceMessageRecorder)
            {
                ((VmIVRMenuElementVoiceMessageRecorder) CurrentIVRMenu).PostIntroductionAudio= filepath;
            }
        }

        private void CreateApropiatePartControl(VmIVRMenuElementBase menubase)
        {
            if (menubase is VmIVRMenuElementVoiceMessageRecorder)
            {
                UCMenuSpecificPart = new UcVoiceMessageRecorderProperties();
            }
            else if (menubase is VmIVRMenuElementCallTransfer)
            {
                UCMenuSpecificPart = new UcCallTransferProperties();
            }
        }

        public UserControl UCMenuSpecificPart { get; set; }



    }
}
