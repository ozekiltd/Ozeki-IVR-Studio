using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using GongSolutions.Wpf.DragDrop.Utilities;
using IVRStudio;
using IVRStudio.GDragDrop;
using IVRStudio.GDragDrop.Utilities;
using IVRStudio.Model;
using IVRStudio.ViewModel;
using IVRStudio.Views;
using OPSIVRSystem.CommonViewModel;


namespace GongSolutions.Wpf.DragDrop
{
    public class TreeViewDropHandler : IDropTarget
    {
        public virtual void DragOver(DropInfo dropInfo)
        {
            if (CanAcceptData(dropInfo))
            {
                try
                {
                    dropInfo.Effects = dropInfo.DragInfo.VisualSource is TreeView ? DragDropEffects.Move : DragDropEffects.Copy;
                    //                  Debug.WriteLine("is Tree: " + (dropInfo.DragInfo.VisualSource is TreeView).ToString());
                }
                catch (Exception)
                {
                    dropInfo.Effects = DragDropEffects.Copy;
                }

                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            }
        }
        /// <summary>
        /// Megkeresi van e root item és visszaajda ha van
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        private static bool HasARootItem(UIElement uiElement, out TreeViewItem root)
        {
            try
            {
                if (uiElement is TreeView)
                {
                    root = (TreeViewItem)((TreeView)uiElement).ItemContainerGenerator.ContainerFromIndex(0);

                    return true;
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.ToString());
            }
            root = null;
            return false;
        }

        public  void DoDrop(DropInfo dropInfo)
        {
            TreeViewItem parentViewItem = null;

            if (dropInfo.VisualTargetItem == null)// ha a treeview üres területére húzták meg kell keresni a root itemet
            {
                HasARootItem(dropInfo.VisualTarget, out parentViewItem);
            }
            else
            {
                parentViewItem = GetNearestContainer(dropInfo.VisualTargetItem as UIElement);
                if (parentViewItem == null)
                {
                    Debug.WriteLine("GetNearestContainer nem ér semmit");
                }
            }

            if (parentViewItem != null)
            {
                //           Debug.WriteLine("jee nem null " + parentViewItem.ToString());
                if (parentViewItem.Items.Count == 9)
                {
                    MessageBox.Show("You are reached the maximum number of child elements.");
                    return;
                }
                foreach (object o in ExtractData(dropInfo.Data))
                {
                    //parentViewItem.Items.Add(Activator.CreateInstance(o.GetType()));
                    AddRemoveOperation(dropInfo, parentViewItem, o);
                    parentViewItem.IsExpanded = true;
                }
            }
            else
            {
                DropToListContainer(dropInfo);
            }
        }


        public  void DropToListContainer(DropInfo dropInfo)
        {
            int insertIndex = dropInfo.InsertIndex;
            IList destinationList = GetList(dropInfo.TargetCollection);
            IEnumerable data = ExtractData(dropInfo.Data);

            if (dropInfo.DragInfo.VisualSource == dropInfo.VisualTarget)
            {
                IList sourceList = GetList(dropInfo.DragInfo.SourceCollection);
                object data1 = null;
                foreach (object o in data)
                {
                    int index = sourceList.IndexOf(o);

                    if (index != -1)
                    {
                        data1 = sourceList[index];
                        sourceList.RemoveAt(index);

                        if (sourceList == destinationList && index < insertIndex)
                        {
                            --insertIndex;
                        }
                    }
                }
                
                destinationList.Insert(insertIndex++,data1);
                MaintainTouchToneKeys(destinationList);
            }
            else
                foreach (object o in data)//ez csak a root hozzaadaskor fog lefutni
                {
                    destinationList.Insert(insertIndex++, CreateNewNode(Activator.CreateInstance(o.GetType()), 0));
                }
        }

        private static void MaintainTouchToneKeys(IList destinationList)
        {
            byte counter=1;
            foreach (TreeViewItem treeview in destinationList)
            {
                ((VmIVRMenuElementBase) treeview.Header).TouchToneKey = counter++.ToString();
            }
        }

        public virtual void Drop(DropInfo dropInfo)
        {
            if (dropInfo.DragInfo.VisualSourceItem is TreeViewItem && dropInfo.VisualTargetItem is TreeViewItem)
            {
            //    DoDrop(dropInfo);

                ShowOperationChooserPopup(dropInfo);
            }
            else
            {
                DoDrop(dropInfo);
            }
        }



        protected static bool CanAcceptData(DropInfo dropInfo)
        {
            try
            {
                if (dropInfo.DragInfo.VisualSourceItem is TreeViewItem && dropInfo.VisualTargetItem is TreeViewItem)
                {
                    Debug.WriteLine("mindkettő  is TreeViewItem");

                    return !IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
                }
            }
            catch (Exception)
            {
                return false;
            }

            if (dropInfo.DragInfo.SourceCollection == dropInfo.TargetCollection)
            {
                return GetList(dropInfo.TargetCollection) != null;
            }
            else if (dropInfo.DragInfo.SourceCollection is ItemCollection)
            //  else if (dropInfo.DragInfo.SourceCollection is ListCollecitonView )
            {
                //   Debug.WriteLine("CanAcceptData - dropInfo.DragInfo.SourceCollection is ItemCollection: false");
                return false;
            }
            else
            {
                if (TestCompatibleTypes(dropInfo.TargetCollection, dropInfo.Data))
                {
                    //      Debug.WriteLine("CanAcceptData - TestCompatibleTypes: " + !IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem));
                    return !IsChildOf(dropInfo.VisualTargetItem, dropInfo.DragInfo.VisualSourceItem);
                }
                else
                {
                    //             Debug.WriteLine("CanAcceptData: false");
                    return true;
                }
            }
        }

        protected static IEnumerable ExtractData(object data)
        {
            if (data is IEnumerable && !(data is string))
            {
                return (IEnumerable)data;
            }
            else
            {
                return Enumerable.Repeat(data, 1);
            }
        }

        protected static IList GetList(IEnumerable enumerable)
        {
            if (enumerable is ICollectionView)
            {
                return ((ICollectionView)enumerable).SourceCollection as IList;
            }
            else
            {
                return enumerable as IList;
            }
        }

        protected static bool IsChildOf(UIElement targetItem, UIElement sourceItem)
        {
            ItemsControl parent = ItemsControl.ItemsControlFromItemContainer(targetItem);

            while (parent != null)
            {
                if (parent == sourceItem)
                {
                    return true;
                }

                parent = ItemsControl.ItemsControlFromItemContainer(parent);
            }

            return false;
        }

        protected static bool TestCompatibleTypes(IEnumerable target, object data)
        {
            TypeFilter filter = (t, o) =>
            {
                return (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            };

            var enumerableInterfaces = target.GetType().FindInterfaces(filter, null);
            var enumerableTypes = from i in enumerableInterfaces select i.GetGenericArguments().Single();

            if (enumerableTypes.Count() > 0)
            {
                Type dataType = TypeUtilities.GetCommonBaseClass(ExtractData(data));
                return enumerableTypes.Any(t => t.IsAssignableFrom(dataType));
            }
            else
            {
                return target is IList;
            }
        }

        private static TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }

        private static void AddRemoveOperation(DropInfo dropInfo, TreeViewItem target, object ob)
        {

            //dropeffect is not consistent in all state, so:
            //   Debug.WriteLine(dropInfo.DragInfo.VisualSource.ToString());
            if (dropInfo.DragInfo.VisualSource is TreeView)
            {
                if (dropInfo.DragInfo.VisualSourceItem == target)
                    return;
                AddChild((TreeViewItem)dropInfo.DragInfo.VisualSourceItem, target);
                RemoveTreeViewItem((TreeView)dropInfo.DragInfo.VisualSource, (TreeViewItem)dropInfo.DragInfo.VisualSourceItem, dropInfo.DragInfo.SourceItem);

            }
            else
            {

                object newdata = Activator.CreateInstance(ob.GetType());
                target.Items.Add(CreateNewNode(newdata, target.Items.Count));
            }
        }


        private static TreeViewItem CreateNewNode(object data, int listcount)
        {
            TreeViewItem res = new TreeViewItem();
            if (data is VmIVRMenuElementBase)
            {
                res.Header = data;
                ((VmIVRMenuElementBase)data).TouchToneKey = (listcount + 1).ToString();
            }
            else
            {
                res.Header = ((TreeViewItem)data).Header;
            }

            return res;
        }

        public static void AddChild(TreeViewItem _sourceItem, TreeViewItem _targetItem)
        {
            try
            {
                if (_sourceItem == _targetItem)
                    return;
                // add item in target TreeViewItem 
                TreeViewItem item1 = new TreeViewItem();
                item1.Header = _sourceItem.Header;
                ((VmIVRMenuElementBase)item1.Header).TouchToneKey = (_targetItem.Items.Count + 1).ToString();
                _targetItem.Items.Add(item1);

                //        Debug.WriteLine(_sourceItem.Header.GetType());
                foreach (var item in _sourceItem.Items)
                {
                    TreeViewItem child = item as TreeViewItem;
                    if (child == null)
                    {
                        child = _sourceItem.ItemContainerGenerator.ContainerFromItem(item) as TreeViewItem;
                    }
                    AddChild(child, item1);
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Error at Addchild: " + e.ToString());
            }
        }

        private static void RemoveTreeViewItem(TreeView treeView, TreeViewItem visualtargetItem, object targetdata)
        {
            try
            {
                //            Debug.WriteLine((VisualTreeHelper.GetParent(visualtargetItem).ToString()));
                TreeViewItem parent = FindVisualParent<TreeViewItem>(visualtargetItem);
                if (parent != null)
                {
                    parent.Items.Remove(targetdata);
                    MaintainTouchToneKeys(parent.Items);
                }
                else
                {
                    treeView.Items.Remove(targetdata);
                    MaintainTouchToneKeys(treeView.Items);
                }
            }
            catch (Exception e)
            {
            }
        }

        private static void MaintainTouchToneKeys(ItemCollection collection)
        {
            for (int i = 0; i < collection.Count; i++)
            {
                ((VmIVRMenuElementBase)((TreeViewItem)collection[i]).Header).TouchToneKey = (i + 1).ToString();
            }

        }

        static TObject FindVisualParent<TObject>(UIElement child) where TObject : UIElement
        {
            if (child == null)
            {
                return null;
            }

            UIElement parent = VisualTreeHelper.GetParent(child) as UIElement;

            while (parent != null)
            {
                TObject found = parent as TObject;
                if (found != null)
                {
                    return found;
                }
                else
                {
                    parent = VisualTreeHelper.GetParent(parent) as UIElement;
                }
            }

            return null;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct W32Point
        {
            public int X;
            public int Y;
        }


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref W32Point pt);

     
        private void ShowOperationChooserPopup(DropInfo dropInfo)
        {
          //  Window wnd = FindVisualParent<Window>(dropInfo.VisualTargetItem);
          //  if (wnd != null)
            {
            //    MainWindow main = (MainWindow)wnd;
               
                WndPopup popup = new WndPopup(dropInfo, this);
                Mouse.GetPosition(dropInfo.VisualTargetItem);
                W32Point p=new W32Point();
                GetCursorPos(ref p);
                popup.Left = p.X;
                popup.Top = p.Y;
                popup.Topmost = true;
                popup.ShowDialog();  
            }
        }

    }
}
