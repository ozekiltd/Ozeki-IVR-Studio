using GongSolutions.Wpf.DragDrop;

namespace IVRStudio.GDragDrop.Utilities
{
    class DataContainer
    {
   

        public MainWindow MainWindow { get; private set; }

        public string btnId { get; private set; }

        public DropInfo DropInfo { get; private set; }

        public DataContainer(MainWindow wnd, string btnidentifier, DropInfo dropInfo)
        {
            MainWindow = wnd;
            btnId = btnidentifier;
            DropInfo = dropInfo;
        }
    }
}
