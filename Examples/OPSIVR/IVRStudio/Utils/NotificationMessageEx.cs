namespace OPS_IVR_Studio.Utils
{
    class NotificationMessageEx
    {
        public NotificationMessageEx(MsgDestination notification, MsgCommand command, params object[] parameters)
        {
            Notification = notification;
            Command = command;
            Parameters = parameters;
        }

        public MsgDestination Notification { get; private set; }

        public MsgCommand Command { get; set; }

        public object[] Parameters { get; private set; }
    }
}
