using System;
using System.Windows;
using GalaSoft.MvvmLight.Messaging;

namespace IVRStudio.Util
{
    class DialogMessageEx : DialogMessage
    {
        public DialogMessageEx( string content)
            : base(content, param => { })
        {
            Icon = MessageBoxImage.None;
            Button = MessageBoxButton.OK;
            Caption = string.Empty;
        }

        public DialogMessageEx( string content, Action<MessageBoxResult> callback)
            : base(content, callback)
        {
            Icon = MessageBoxImage.None;
            Button = MessageBoxButton.OK;
            Caption = string.Empty;
        }


        public static DialogMessageEx CreateQuestionBox(string content, Action<MessageBoxResult> callback)
        {
            DialogMessageEx res =
                new DialogMessageEx(content, callback)
                    {
                        Icon = MessageBoxImage.Question,
                        Button = MessageBoxButton.YesNo,
                        Caption = string.Empty
                    };
            
            return res;
        }

    }
}
