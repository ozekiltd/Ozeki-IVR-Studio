using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using GongSolutions.Wpf.DragDrop;

namespace IVRStudio.Views
{
    /// <summary>
    /// Interaction logic for WndPopup.xaml
    /// </summary>
    public partial class WndPopup : Window
    {
        private  TreeViewDropHandler dropHandler;
        private DropInfo dropInfo;

        public WndPopup(DropInfo dropInfo, TreeViewDropHandler dropTarget)
        {
            InitializeComponent();
            this.dropInfo = dropInfo;
            this.dropHandler = dropTarget;
        }

        private void Grid_MouseLeave_1(object sender, MouseEventArgs e)
        {
            this.Close();
        }

        private void btnAddAsChild_Click(object sender, RoutedEventArgs e)
        {
            dropHandler.DoDrop(dropInfo);
            this.Close();
        }

        private void btnReorder_Click(object sender, RoutedEventArgs e)
        {
            dropHandler.DropToListContainer(dropInfo);
            this.Close();
        }

        private void btnCancelOp_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
