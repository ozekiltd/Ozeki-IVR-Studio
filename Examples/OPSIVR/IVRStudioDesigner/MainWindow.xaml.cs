using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using GalaSoft.MvvmLight.Messaging;
using IVRStudio.Model;
using IVRStudio.Util;
using IVRStudio.ViewModel;
using IVRStudio.Views;
using OPSIVRSystem.CommonViewModel;
using OPSIVRSystem.IVRMenus;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;
using TreeView = System.Windows.Controls.TreeView;

namespace IVRStudio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
	/// 
	/// 
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindow_OnStateChanged(null, null);
            Messenger.Default.Register<NotificationMessageEx>(this, NotificationMessageReceived);
        }

        private void NotificationMessageReceived(NotificationMessageEx message)
        {
            if (message.Notification == MsgDestination.WindowMain) // Csak akkor foglalkozunk az özenettel, ha nekünk szól
            {
                switch (message.Command)
                {
                    case MsgCommand.PopulateTreeView:
                        MenuTreeView.Items.Clear();
                        PopulateTreeView((List<VmIVRMenuElementBase>)message.Parameters[0], IVRMenuElementBase.RootIdentifier, null);
                        if (MenuTreeView.Items.Count > 0 && MenuTreeView.Items.GetItemAt(0) is TreeViewItem)
                        {
                            TreeViewItem root = (TreeViewItem)MenuTreeView.Items.GetItemAt(0);
                            root.IsExpanded = true;
                        }
                        break;
                    case MsgCommand.UpdateTreeDatas:
                        UpdateTreeDatas();
                        Debug.WriteLine(resList.ToString());
                        break;
                    case MsgCommand.DeleteSelectedItem:
                        if (MessageBox.Show("Are you sure you want to delete the selected menu?", "Delete menu", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            DeleteSelectedItem((TreeViewItem)message.Parameters[0]);
                        }
                        break;
                    case MsgCommand.ShowSavedialog:
                        var dlg = new SaveFileDialog();
                        dlg.DefaultExt = ".ozivr";
                        dlg.Title = "Save Ozeki IVR Studio project";
                        dlg.FileName = (string) message.Parameters[1];
                        dlg.Filter = "Ozeki IVR Studio project file (.ozivr)|*.ozivr";
                        if (dlg.ShowDialog() == true)
                        {
                            ((FileOperationMessage)message.Parameters[0]).Callback(dlg.FileName);
                        }
                        break;
                    case MsgCommand.ShowLoadDialog:
                        var opendlg= new OpenFileDialog();
                        opendlg.DefaultExt = ".ozivr";
                        opendlg.Title = "Open an Ozeki IVR Studion project";
                        opendlg.Filter = "Ozeki IVR Studio project file (.ozivr)|*.ozivr";
                        if (opendlg.ShowDialog() == true)
                        {
                            ((FileOperationMessage)message.Parameters[0]).Callback(opendlg.FileName);
                        }
                        break;
                        case MsgCommand.ShowSaveQuestion:
                          DialogMessageEx dialogMessageEx = (DialogMessageEx)message.Parameters[0];

                        var result = MessageBox.Show(dialogMessageEx.Content, dialogMessageEx.Caption, MessageBoxButton.YesNo, MessageBoxImage.Question);
                        dialogMessageEx.Callback.Invoke(result);
                        break;
                        case MsgCommand.ShowWindowNewProject:
                        WindowNewProject wndNewProject=new WindowNewProject();
                        wndNewProject.Owner = this;
                        wndNewProject.ShowDialog();
                        break;
                        case MsgCommand.UpdateProject:
                        ((VmMain)this.DataContext).CurrentProject = (VmIVRProject)message.Parameters[0];
                        break;
                }
            }
        }

        private void DeleteSelectedItem(TreeViewItem treeViewItem)
        {
            if (treeViewItem.Parent == null)
            {
                MenuTreeView.Items.Remove(treeViewItem);
            }
            else
            {
                if (treeViewItem.Parent is TreeView)
                {
                    MenuTreeView.Items.Remove(treeViewItem);
                }
                else
                    ((TreeViewItem)treeViewItem.Parent).Items.Remove(treeViewItem);
            }

        }


        public void AppendLog(String text)
        {
            txtLog.Text += text + Environment.NewLine;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(MenuTreeView.Items.ToString());
        }


        private void PopulateTreeView(List<VmIVRMenuElementBase> treeViewList, string parentId, TreeViewItem parentNode)
        {
            var filteredItems = treeViewList.Where(item =>
                                        item.ParentId == parentId);

            TreeViewItem childNode;
            foreach (var i in filteredItems.ToList())
            {
                childNode = CreateTreeNodeItem(i);
                if (parentNode == null)
                    MenuTreeView.Items.Add(childNode);
                else
                    parentNode.Items.Add(childNode);

                PopulateTreeView(treeViewList, i.Id, childNode);
            }
        }

        private void UpdateTreeDatas()
        {
            if (MenuTreeView.Items.Count > 0)
            {
                printNode((TreeViewItem)MenuTreeView.Items.GetItemAt(0), null);
            }
        }



        private TreeViewItem CreateTreeNodeItem(VmIVRMenuElementBase menu)
        {
            TreeViewItem cur = new TreeViewItem();
            cur.Header = menu;
            return cur;
        }

        List<VmIVRMenuElementBase> resList = new List<VmIVRMenuElementBase>();

        private void printNode(TreeViewItem node, TreeViewItem parent)
        {
            // printTitle(node.title)
            Debug.WriteLine(node.ToString());
            if (parent != null)
            {
                VmIVRMenuElementBase menuParent = (VmIVRMenuElementBase)parent.Header;
                VmIVRMenuElementBase child = (VmIVRMenuElementBase)node.Header;
                menuParent.ChildMenus.Add(child);
                child.ParentId = menuParent.Id;
                child.Parent = menuParent;
                resList.Add(child);
            }
            else
            {
                resList = new List<VmIVRMenuElementBase>();
                resList.Add((VmIVRMenuElementBase)node.Header);
            }
            foreach (TreeViewItem child in node.Items)
            {
                printNode(child, node); //<-- recursive
            }
        }


        #region WindowState Events

        public void ShowWindow()
        {
            Activate();
            ShowInTaskbar = true;
            Show();
            WindowState = WindowState.Normal;
            Focus();
        }

        private void WindowsIcon_Click(object sender, EventArgs e)
        {
            WindowIconMenu.IsOpen = true;
        }

        private void MaximizeButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Maximized;
            MaxHeight = Screen.PrimaryScreen.WorkingArea.Size.Height + 9;
        }

        private void ChangeViewButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        private void MinimizeButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Maximized;
            MaxHeight = Screen.PrimaryScreen.WorkingArea.Size.Height + 9;
        }

        private void MinimizeWindow_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void RestoreWindowSize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Normal;
        }

        void MainWindow_OnStateChanged(object sender, EventArgs e)
        {
            switch (WindowState)
            {
                case WindowState.Maximized:
                    MaximizeButton.Visibility = Visibility.Collapsed;
                    ChangeViewButton.Visibility = Visibility.Visible;
                    break;
                case WindowState.Normal:
                    MaximizeButton.Visibility = Visibility.Visible;
                    ChangeViewButton.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        void DragableGrid_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;

            var mouseX = e.GetPosition(this).X;
            var width = RestoreBounds.Width;
            var x = mouseX - width / 2;

            if (x < 0) x = 0;
            else if (x + width > SystemParameters.PrimaryScreenWidth)
                x = SystemParameters.PrimaryScreenWidth - width;

            WindowState = WindowState.Normal;
            Left = x;
            Top = 0;

            DragMove();
        }

        void DragableGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount >= 2)
                SwitchState();
            else if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void SwitchState()
        {
            switch (WindowState)
            {
                case WindowState.Normal:
                    {
                        WindowState = WindowState.Maximized;
                        MaxHeight = Screen.PrimaryScreen.WorkingArea.Size.Height + 9;
                        break;
                    }
                case WindowState.Maximized:
                    {
                        WindowState = WindowState.Normal;
                        break;
                    }
            }
        }
        #endregion

        #region Closings
        void Exit_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void CloseButtonMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Close();
        }

        private void CloseWindow_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region Mouse Enter and Leave coloring

        private void MinimizeButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            MinimizeButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        private void MinizeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            MinimizeButton.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void MaximizeButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            MaximizeButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        private void MaximizeButton_MouseLeave(object sender, MouseEventArgs e)
        {
            MaximizeButton.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void ChangeViewButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            ChangeViewButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        private void ChangeViewButton_MouseLeave(object sender, MouseEventArgs e)
        {
            ChangeViewButton.Foreground = new SolidColorBrush(Colors.Gray);
        }

        private void CloseButton_OnMouseEnter(object sender, MouseEventArgs e)
        {
            CloseButton.Foreground = new SolidColorBrush(Color.FromRgb(188, 216, 188));
        }

        private void CloseButton_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseButton.Foreground = new SolidColorBrush(Colors.Gray);
        }
        #endregion

    }
}
