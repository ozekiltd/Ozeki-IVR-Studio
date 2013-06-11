using System.Windows;
using System.Windows.Controls;
using IVRStudio.ViewModel;
using Microsoft.Win32;

namespace IVRStudio
{
	/// <summary>
	/// Interaction logic for UcVoiceMessageRecorderProperties.xaml
	/// </summary>
	public partial class UcVoiceMessageRecorderProperties : UserControl
	{
		public UcVoiceMessageRecorderProperties()
		{
			this.InitializeComponent();
		}

	    private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
	    {
            var opendlg = new OpenFileDialog();
            opendlg.DefaultExt = ".wav";
            opendlg.Title = "Open an Ozeki IVR Studion project";
            opendlg.Filter = "Wave audio file|*.wav";
            if (opendlg.ShowDialog() == true)
            {
                ((VmProperties)DataContext).SetPostIntroductionAudio(opendlg.FileName);
            }
	    }
	}
}