using System.Windows.Controls;
using IVRStudio.Util;
using Microsoft.Win32;
using OPSIVRSystem.CommonViewModel;
using VmProperties = IVRStudio.ViewModel.VmProperties;

namespace IVRStudio.Views
{
    /// <summary>
    /// Interaction logic for UcMenuProperties.xaml
    /// </summary>
    public partial class UcMenuProperties : UserControl
    {
        public UcMenuProperties()
        {
            DataContext = new VmProperties();
            InitializeComponent();
        }

        public UcMenuProperties(VmIVRMenuElementBase menu)
        {
            DataContext = new VmProperties(menu);
            InitializeComponent();
        }

        private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
        {
            var opendlg = new OpenFileDialog();
            opendlg.DefaultExt = ".wav";
            opendlg.Title = "Open an Ozeki IVR Studion project";
            opendlg.Filter = "Wave audio file|*.wav";
            if (opendlg.ShowDialog() == true)
            {
                ((VmProperties)DataContext).SetAudioFilePath(opendlg.FileName);
            }
        }
    }
}
