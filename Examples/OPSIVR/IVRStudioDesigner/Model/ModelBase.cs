using System.Collections.Generic;
using OPSIVRSystem.IVRMenus;

namespace IVRStudio.Model
{
    abstract class ModelBase
    {

        protected List<IVRMenuElementBase> ToolboxElements;

        public ModelBase()
        {
            ToolboxElements = new List<IVRMenuElementBase>();
            ToolboxElements.Add(new IVRMenuElementInfoReader());
            ToolboxElements.Add(new IVRMenuElementCallTransfer());
            ToolboxElements.Add(new IVRMenuElementVoiceMessageRecorder());
        }
    }
}
