using System.Collections.Generic;
using System.ComponentModel;
using GalaSoft.MvvmLight;
using IVRStudio.Util;
using OPSIVRSystem;
using OPSIVRSystem.CommonViewModel;
using OPSIVRSystem.Config;
using OPSIVRSystem.Utils;

namespace IVRStudio.ViewModel
{
    class VmIVRProject : ViewModelBase , IDataErrorInfo
    {
        public VmIVRProject()
        {
            Name = "Untiltled IVR project";
            MenuList = new List<VmIVRMenuElementBase>();
        }

        //public VmIVRProject(IVRProjectConfig mProject)
        //{
        //    Name = mProject.Name;
        //    MenuList = ConverterMenu.GetMenuViewModels(mProject.MenuList);
        //  //  IVRMenuRoot = ConverterMenu.GetMenuViewModel(mProject.IVRMenuRoot);

        //}
        public VmIVRProject(IVRProject mProject)
        {
            Name = mProject.Name;
            MenuList = ConverterMenu.GetMenuViewModels(mProject.MenuList);
            IVRMenuRoot = ConverterMenu.GetMenuViewModel(mProject.IVRMenuRoot);

        }
        private VmIVRMenuElementBase _ivrMenuRoot;

        public List<VmIVRMenuElementBase> MenuList { get; set; }

        public VmIVRMenuElementBase IVRMenuRoot
        {
            get
            {
                if (_ivrMenuRoot == null)
                {
                    _ivrMenuRoot = MenuTreeBuilder.BuildTreeAndGetRoots(MenuList);
                }
                return _ivrMenuRoot;
            }
            set { _ivrMenuRoot = value; }
        }


        private string _name;
        public string Name
        {
            get { return _name; }
            set
            {
                if (string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value))
                {
                    Error = "Ivr project name cannot be an empty string.";
                }
                else
                {
                    Error = "";
                }
                _name = value;
                RaisePropertyChanged(()=>Name);
            }
        }

        public string SavePath { get; set; }


        public string this[string columnName]
        {
            get { return Error; }
        }

        public string Error { get; private set; }


        public IVRProject GetModelProject()
        {
            var pr = new IVRProject();
            pr.Name = Name;
            pr.MenuList = ConverterMenu.ConvertToMenuModels(MenuList);
            pr.IVRMenuRoot = IVRMenuRoot.GetModel();
            return pr;
        }

     
    }
}
