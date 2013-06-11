using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OPS_IVR_Studio.Config
{
    [Serializable]
    class IVRStudioSettings
    {
        public IVRStudioSettings()
        {
            
        }
        public string LastIVRProjectPath { get; set; }
        public string LastUsedExtension { get; set; }
        public bool RemebertoExtension { get; set; }

    }
}
