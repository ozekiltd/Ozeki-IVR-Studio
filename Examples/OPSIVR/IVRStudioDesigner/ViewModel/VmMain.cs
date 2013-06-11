using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using IVRStudio.Model;
using IVRStudio.Util;
using IVRStudio.Views;
using OPSIVRSystem.CommonViewModel;
using OPSIVRSystem.IVRMenus;
using OPSIVRSystem.Utils;

namespace IVRStudio.ViewModel
{
    class VmMain : ViewModelBase
    {
        public VmMain()
        {
           // model = new MockModel();
            model = new RealModel();

            IVRToolboxElementList = new ObservableCollection<VmIVRMenuElementBase>(ConverterMenu.GetMenuViewModels(model.GetToolboxElements()));
            ToolboxSource = CollectionViewSource.GetDefaultView(IVRToolboxElementList);

            Init();
        }

        private IModel model;
    

        public VmIVRProject CurrentProject
        {
            get { return _currentProject; }
            set
            {
                _currentProject = value; RaisePropertyChanged(() => CurrentProject);
                Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowMain, MsgCommand.PopulateTreeView, CurrentProject.MenuList));
            }
        }

        private UserControl _ucPropertiesPart;

        public ICollectionView ToolboxSource { get; set; }

        public RelayCommand<TreeViewItem> CommandDeleteMenu { get; private set; }
        public RelayCommand<object> CommandNewProject { get; private set; }
        public RelayCommand<object> CommandLoadProject { get; private set; }
        public RelayCommand<object> CommandSaveProject { get; private set; }
        public RelayCommand<object> CommandSaveAsProject { get; private set; }
        public RelayCommand<object> CommandTreeViewItemChanged { get; private set; }

        private ObservableCollection<IVRMenuElementBase> _treeSource1;
        private VmIVRProject _currentProject;

        public UserControl UcPropertiesPart
        {
            get { return _ucPropertiesPart; }
            set
            {
                _ucPropertiesPart = value;
                RaisePropertyChanged(() => UcPropertiesPart);
            }
        }

        private void Init()
        {
            CurrentProject = new VmIVRProject();
            CurrentProject.IVRMenuRoot = new VmIVRMenuElementInfoReader();
            CommandDeleteMenu = new RelayCommand<TreeViewItem>(DeleteIvrMenu);
            CommandTreeViewItemChanged = new RelayCommand<object>(TreeViewMenuItemChanged);
            CommandNewProject = new RelayCommand<object>(CreateNewProject);
            CommandLoadProject = new RelayCommand<object>(LoadProject);
            CommandSaveProject = new RelayCommand<object>(SaveProject);
            CommandSaveAsProject = new RelayCommand<object>(SaveAsProject);
            UcPropertiesPart = new UcTipView();
        }

        #region Create/Load/Save project


        private void CreateNewProject(object currentMenuTree)
        {
                Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowMain, MsgCommand.ShowSaveQuestion, DialogMessageEx.CreateQuestionBox("Do you wish to save current project before creating a new project?", (result =>
                   {
                        if (result == MessageBoxResult.Yes)
                        {
                            SaveProject(currentMenuTree);
                        }
                    }))));

            Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowMain, MsgCommand.ShowWindowNewProject));
        }


        private List<VmIVRMenuElementBase> UpdateCurrentProject(object currentMenuTree)
        {
            List<VmIVRMenuElementBase> reslist = new List<VmIVRMenuElementBase>();
            if (currentMenuTree is ItemCollection && ((ItemCollection)currentMenuTree).Count > 0)
            {
                BuildMenuListFromHierarchy((TreeViewItem)((ItemCollection)currentMenuTree).GetItemAt(0), null, reslist);
                CurrentProject.MenuList = reslist;
                Debug.WriteLine(reslist.ToString());
            }
            return reslist;
        }

        private void SaveProject(object currentMenuTree)
        {
            if (string.IsNullOrEmpty(CurrentProject.SavePath))
                SaveAsProject(currentMenuTree);
            else
            {
                if (UpdateCurrentProject(currentMenuTree).Count > 0)
                {
                    SaveToFile(CurrentProject);
                }
            }
        }

        private void SaveAsProject(object currentMenuTree)
        {
            if (UpdateCurrentProject(currentMenuTree).Count > 0)
            {
                Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowMain,
                                                                 MsgCommand.ShowSavedialog,
                                                                 new FileOperationMessage((resPath) =>
                                                                     {
                                                                         CurrentProject.SavePath = resPath;
                                                                         SaveToFile(CurrentProject);
                                                                     }), CurrentProject.Name));
            }
        }


        private void SaveToFile( VmIVRProject pr)
        {
            model.SaveProject(pr.GetModelProject(), pr.SavePath);

        }

        private void LoadProject(object currentTree)
        {
            Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowMain, MsgCommand.ShowSaveQuestion, DialogMessageEx.CreateQuestionBox("Do you wish to save current project before loads an other project?", (result =>
            {
                if (result == MessageBoxResult.Yes)
                {
                    SaveProject(currentTree);
                }
            }))));

            Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowMain, MsgCommand.ShowLoadDialog, new FileOperationMessage((
                resPath) =>
                {
                    CurrentProject = new VmIVRProject(model.LoadProject(resPath));
                    CurrentProject.SavePath = resPath;
                })));
        }


        #endregion

        private void DeleteIvrMenu(TreeViewItem menuElement)
        {
            Debug.WriteLine("Delete menu called");
            if (menuElement != null)
            {
                Messenger.Default.Send(new NotificationMessageEx(MsgDestination.WindowMain, MsgCommand.DeleteSelectedItem, menuElement));
            }

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
        
        private ObservableCollection<VmIVRMenuElementBase> IVRToolboxElementList { get; set; }

        private void BuildMenuListFromHierarchy(TreeViewItem node, TreeViewItem parent, List<VmIVRMenuElementBase> resList)
        {
            if (parent != null)
            {
                VmIVRMenuElementBase menuParent = ((VmIVRMenuElementBase)parent.Header).GetAClone();
                VmIVRMenuElementBase child = ((VmIVRMenuElementBase)node.Header).GetAClone();
                menuParent.ChildMenus.Add(child);
                child.ParentId = menuParent.Id;
                child.Parent = menuParent;
                resList.Add(child);
            }
            else
            {
                resList.Add((VmIVRMenuElementBase)node.Header);
            }
            foreach (TreeViewItem child in node.Items)
            {
                BuildMenuListFromHierarchy(child, node, resList); //<-- recursive
            }
        }



    }
}
