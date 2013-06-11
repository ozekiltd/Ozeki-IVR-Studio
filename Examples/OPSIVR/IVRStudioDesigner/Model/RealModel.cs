using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OPSIVRSystem;
using OPSIVRSystem.IVRMenus;

namespace IVRStudio.Model
{
    class RealModel : ModelBase, IModel
    {

        private ProjectStore projectStore;

        public RealModel()
        {
            projectStore= new ProjectStore();
        }


        public IVRProject LoadProject(string path)
        {
            return projectStore.LoadProject(path);
        }

        public void SaveProject(IVRProject project, string path)
        {
            projectStore.SaveProject(path, project);
        }

        public List<IVRMenuElementBase> GetToolboxElements()
        {
            return ToolboxElements;
        }

    }
}
