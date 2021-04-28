#if WORKINPROGRESS

#if !MIGRATION
using System;
#endif

#if MIGRATION
using System.ComponentModel;
using System.Diagnostics;

namespace System.Windows
#else
namespace Windows.Foundation
#endif
{
    [DebuggerNonUserCode]
    public static class EventHandlerExtensions
    {
        public static void Raise(this EventHandler handler, object sender)
        {
            if (handler != null)
            {
                handler(sender, EventArgs.Empty);
            }
        }

        public static void Raise(this EventHandler handler, object sender, EventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public static void Raise<T>(this EventHandler<T> handler, object sender, T e) where T : EventArgs
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }

        public static void Raise(this PropertyChangedEventHandler handler, object sender, string propertyName)
        {
            handler.Raise(sender, new PropertyChangedEventArgs(propertyName));
        }

        public static void Raise(this PropertyChangedEventHandler handler, object sender, PropertyChangedEventArgs e)
        {
            if (handler != null)
            {
                handler(sender, e);
            }
        }
    }
}
#endif