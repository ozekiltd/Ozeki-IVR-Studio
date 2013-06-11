using System.Collections.Generic;
using OPSIVRSystem;
using OPSIVRSystem.IVRMenus;

namespace IVRStudio.Model
{
    interface IModel
    {
        IVRProject LoadProject(string path);
        void SaveProject(IVRProject project, string path);
        List<IVRMenuElementBase> GetToolboxElements();
    }
}
