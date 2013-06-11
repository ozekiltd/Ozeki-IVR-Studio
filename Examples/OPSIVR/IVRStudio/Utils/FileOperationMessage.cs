using System;

namespace OPS_IVR_Studio.Utils
{
    class FileOperationMessage
    {
        public Action<string> Callback { get; private set; }

        public FileOperationMessage(Action<string> callback)
        {
            Callback = callback;
        }
    }
}
