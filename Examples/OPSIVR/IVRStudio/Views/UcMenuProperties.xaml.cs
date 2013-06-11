using System.Windows.Controls;
using OPSIVRSystem.CommonViewModel;
using VmProperties = OPS_IVR_Studio.ViewModel.VmProperties;

namespace OPS_IVR_Studio.Views
{
    /// <summary>
    /// Interaction logic for UcMenuProperties.xaml
    /// </summary>
    public partial class UcMenuProperties : UserControl
    {
        public UcMenuProperties()
        {
            InitializeComponent();
        }

        public UcMenuProperties(VmIVRMenuElementBase menu)
        {
            DataContext = new VmProperties(menu);
            InitializeComponent();
        }
    }
}
