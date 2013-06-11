using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IVRStudio.Util;
using OPSIVRSystem;
using OPSIVRSystem.IVRMenus;

namespace IVRStudio.Model
{
    class MockModel : ModelBase, IModel
    {

      
        public MockModel()
        {
            Init();
        }


        private void Init()
        {}

        public void DeleteMenuElement(IVRMenuElementBase element)
        {

        }

        public IVRProject LoadProject(string path)
        {
            IVRProject pr= new IVRProject();
            pr.Name = "Sample project";
            pr.MenuList=new List<IVRMenuElementBase>();
            pr.MenuList.Add(new IVRMenuElementInfoReader() {Id="0",ParentId = "", Name = "root"});
            pr.MenuList.Add(new IVRMenuElementVoiceMessageRecorder() { Id = "1", ParentId = "0", Name = "subtree1" });
            pr.MenuList.Add(new IVRMenuElementInfoReader() { Id = "2", ParentId = "0", Name = "subtree2" });
            pr.MenuList.Add(new IVRMenuElementInfoReader() { Id = "3", ParentId = "0", Name = "subtree3", NarratorType = NarratorType.FilePlayback });
            pr.MenuList.Add(new IVRMenuElementInfoReader() { Id = "4", ParentId = "2", Name = "subtree2.1", NarratorType = NarratorType.FilePlayback });
            pr.MenuList.Add(new IVRMenuElementInfoReader() { Id = "5", ParentId = "2", Name = "subtree2.2", NarratorType = NarratorType.FilePlayback });
            pr.MenuList.Add(new IVRMenuElementCallTransfer() { Id = "6", ParentId = "5", Name = "subtree2.2.1" });
            
            return pr;
        }

        public void SaveProject(IVRProject project, string path)
        {
            
        }

        public List<IVRMenuElementBase> GetToolboxElements()
        {
            return base.ToolboxElements;
        }
    }
}
