using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
using GalaSoft.MvvmLight.Messaging;
using OPSIVRSystem.CommonViewModel;
using OPSIVRSystem.IVRMenus;
using OPS_IVR_Studio.Utils;
using OPS_IVR_Studio.ViewModel;
using OPS_IVR_Studio.Views;
using OzCommon.View;
using OzCommon.ViewModel;
using System.Windows.Input;
using MessageBox = System.Windows.MessageBox;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;
using OpenFileDialog = Microsoft.Win32.OpenFileDialog;

namespace OPS_IVR_Studio
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WindowAPIExtensionSelector wndApiExtension;
        public MainWindow()
        {
            InitializeComponent();
        //    WindowState = WindowState.Maximized;
          //  MaxHeight = SystemParameters.PrimaryScreenHeight-28;
            Messenger.Default.Register<NotificationMessageEx>(this, NotificationMessageReceived);
            Messenger.Default.Register<NotificationMessage>(this, CommonNotificationMessageReceived);
            MainWindow_OnStateChanged(null, null);
            this.Closed += MainWindow_Closed;
        }

        void MainWindow_Closed(object sender, EventArgs e)
        {
            Messenger.Default.Unregister<NotificationMessage>(this, CommonNotificationMessageReceived);
            Messenger.Default.Unregister<NotificationMessageEx>(this, NotificationMessageReceived);
        }

        private void CommonNotificationMessageReceived(NotificationMessage notificationMessage)
        {
            if (notificationMessage.Notification == Messages.ShowAboutWindow)
            {
                
                AboutWindow aboutWindow = new AboutWindow("Ozeki IVR Studio") { DataContext = notificationMessage.Target, Owner = this };
                aboutWindow.ShowDialog();
            }
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
                    case MsgCommand.ShowWindowApiExtensionSelector:
                        if (wndApiExtension!=null)
                        {
                            wndApiExtension.Close();
                            wndApiExtension = null;
                        }
                        wndApiExtension = new WindowAPIExtensionSelector();
                        wndApiExtension.Owner = this;
                        wndApiExtension.Show();
                        break;
                    case MsgCommand.ShowLoadDialog:
                        var opendlg = new OpenFileDialog();
                        opendlg.DefaultExt = ".ozivr";
                        opendlg.Title = "Open an Ozeki IVR Studion project";
                        opendlg.Filter = "Ozeki IVR Studio project file (.ozivr)|*.ozivr";
                        if (opendlg.ShowDialog() == true)
                        {
                            ((FileOperationMessage)message.Parameters[0]).Callback(opendlg.FileName);
                        }
                        break;
                    case MsgCommand.ShowDialogQuestion:
                        DialogMessageEx dialogMessageEx = (DialogMessageEx)message.Parameters[0];

                        var result = MessageBox.Show(dialogMessageEx.Content, dialogMessageEx.Caption, MessageBoxButton.OKCancel, MessageBoxImage.Question);
                        dialogMessageEx.Callback.Invoke(result);
                        break;
                    case MsgCommand.StartWithExtension:
                        Dispatcher.BeginInvoke(new Action(() =>
                            {
                                ((VmMain) DataContext).StartEnginewith(message.Parameters[0]);
                            }));
                        break;
                }
            }
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

        private TreeViewItem CreateTreeNodeItem(VmIVRMenuElementBase menu)
        {
            TreeViewItem cur = new TreeViewItem();
            cur.Header = menu;
            return cur;
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
            MaxHeight = Screen.PrimaryScreen.WorkingArea.Size.Height;
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
            MaxHeight = Screen.PrimaryScreen.WorkingArea.Size.Height;
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
                        MaxHeight = Screen.PrimaryScreen.WorkingArea.Size.Height;
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
