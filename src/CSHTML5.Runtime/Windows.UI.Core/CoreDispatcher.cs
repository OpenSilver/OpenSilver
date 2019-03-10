
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  ● This code is dual-licensed (GPLv3 + Commercial). Commercial licenses can be obtained from: http://cshtml5.com
//
//  ● You are NOT allowed to:
//       – Use this code in a proprietary or closed-source project (unless you have obtained a commercial license)
//       – Mix this code with non-GPL-licensed code (such as MIT-licensed code), or distribute it under a different license
//       – Remove or modify this notice
//
//  ● Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product.
//
//===============================================================================


#if !BRIDGE
using JSIL.Meta;
#else
using Bridge;
#endif

using CSHTML5.Internal;
using DotNetForHtml5.Core;
using System;
#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Core
#endif
{
    /// <summary>
    /// Provides the Windows Runtime core event message dispatcher. Instances of
    /// this type are responsible for processing the window messages and dispatching
    /// the events to the client.
    /// </summary>
    public sealed class CoreDispatcher
    {
        static CoreDispatcher _currentDispatcher;

        internal static CoreDispatcher INTERNAL_GetCurrentDispatcher()
        {
            if (_currentDispatcher == null)
                _currentDispatcher = new CoreDispatcher();
            return _currentDispatcher;
        }

        //todo: this does not exactly correspond to the actual method --> add args to BeginInvoke?
        // Summary:
        //     Executes the specified delegate asynchronously with the specified arguments
        //     on the thread that the System.Windows.Threading.Dispatcher was created on.
        //
        // Parameters:
        //   method:
        //     The delegate to a method that takes parameters specified in args, which is
        //     pushed onto the System.Windows.Threading.Dispatcher event queue.
        //
        //   args:
        //     An array of objects to pass as arguments to the given method. Can be null.
        //
        // Returns:
        //     An object, which is returned immediately after Overload:System.Windows.Threading.Dispatcher.BeginInvoke
        //     is called, that can be used to interact with the delegate as it is pending
        //     execution in the event queue.
        /// <summary>
        /// Executes the specified delegate asynchronously on the thread that the System.Windows.Threading.Dispatcher was created on.
        /// </summary>
        /// <param name="method">
        /// The delegate to a method, which is
        /// pushed onto the System.Windows.Threading.Dispatcher event queue.
        /// </param>
        public void BeginInvoke(Action method)
        {
            if (method == null)
                throw new ArgumentNullException("method");

            BeginInvokeInternal(method);
        }

#if !BRIDGE
        [JSIL.Meta.JSReplacement("setTimeout(function(){$method();},1);")]
#else
        [Template("setTimeout(function(){ {method}(); },1)")]
#endif
        static void BeginInvokeInternal(Action method)
        {
#if CSHTML5NETSTANDARD
            //Console.WriteLine("ON BEGININVOKE CALLED");

            CSHTML5.Interop.ExecuteJavaScriptAsync("setTimeout($0, 1)",
                (Action)(() =>
                {
                    //Console.WriteLine("ON BEGININVOKE EXECUTION");

                    method();
                }));
#else
            //Simulator only. We call the JavaScript "setTimeout" to queue the action on the thread, and then we call Dispatcher.BeginInvoke(...) to ensure that it runs in the UI thread.
            //CSHTML5.Interop.ExecuteJavaScriptAsync("setTimeout($0, 1)",
            //    (Action)(() =>
            //    {
            INTERNAL_Simulator.WebControl.Dispatcher.BeginInvoke((Action)(() =>
                {
                    method();
                }));
            //}));
#endif

            // Note: the implementation below was commented because it led to issues when drawing the Shape controls: sometimes some Shape controls did not render in the Simulator because the method passed "Dispatcher.Begin()" was called too early.
            /*
            global::System.Threading.Timer timer = new global::System.Threading.Timer(
                delegate(object state)
                {
                    INTERNAL_Simulator.WebControl.Dispatcher.BeginInvoke((Action)(() =>
                    {
                        method();
                    }));
                },
                null,
                1,
                global::System.Threading.Timeout.Infinite);
             */
        }
    }
}