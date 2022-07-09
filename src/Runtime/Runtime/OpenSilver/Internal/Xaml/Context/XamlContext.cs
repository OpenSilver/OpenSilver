
/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Collections.Generic;

#if MIGRATION
using System.Windows;
using System.Windows.Markup;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
#endif

namespace OpenSilver.Internal.Xaml.Context
{
    public class XamlContext
    {
        private readonly XamlContextStack _stack;
        private object _rootInstance;
        private Lazy<INameResolver> _nameResolver;

        internal XamlContext()
        {
            _stack = new XamlContextStack();
            SavedDepth = 0;
        }

        internal XamlContext(XamlContext ctx)
        {
            if (ctx == null)
            {
                throw new ArgumentNullException(nameof(ctx));
            }

            _stack = ctx._stack.DeepCopy();
            SavedDepth = _stack.Depth;
        }

        internal void PushScope() => _stack.PushScope();

        internal void PopScope() => _stack.PopScope();

        internal object CurrentInstance
        {
            get => _stack.CurrentFrame.Instance;
            set => _stack.CurrentFrame.Instance = value;
        }

        internal object ParentInstance => _stack.PreviousFrame.Instance;

        internal object RootInstance
        {
            get
            {
                //evaluate if _rootInstance should just always look at _rootFrame.Instance instead of caching an instance
                if (_rootInstance == null)
                {
                    XamlObjectFrame rootFrame = GetTopFrame();
                    _rootInstance = rootFrame.Instance;
                }
                return _rootInstance;
            }
        }

        internal INameResolver NameResolver
        {
            get
            {
                if (_nameResolver == null)
                {
                    _nameResolver = new Lazy<INameResolver>(() =>
                    {
                        if (SavedDepth > 0)
                        {
                            return new TemplateNameResolver((FrameworkElement)_stack.GetFrame(SavedDepth + 1).Instance);
                        }
                        else if (RootInstance is FrameworkElement rootObject)
                        {
                            return new XamlNameResolver(rootObject);
                        }

                        return null;
                    });
                }

                return _nameResolver.Value;
            }
        }

        internal INameScope ExternalNameScope { get; set; }

        /// <summary>
        /// Total depth of the stack SavedDepth+LiveDepth
        /// </summary>
        internal int Depth => _stack.Depth;

        internal int SavedDepth { get; }

        /// <summary>
        /// The Depth of the Stack above the Saved (template) part
        /// </summary>
        internal int LiveDepth => Depth - SavedDepth;

        private XamlObjectFrame GetTopFrame()
        {
            if (_stack.Depth == 0)
            {
                return null;
            }

            return _stack.GetFrame(1);
        }

        internal IEnumerable<object> ServiceProvider_GetAllAmbientValues()
        {
            var retList = new List<object>();

            XamlObjectFrame frame = _stack.CurrentFrame;
            while (frame.Depth >= 1)
            {
                object inst = frame.Instance;
                if (inst is FrameworkElement fe)
                {
                    if (fe.HasResources)
                    {
                        retList.Add(fe.Resources);
                    }
                }
                else if (inst is ResourceDictionary)
                {
                    retList.Add(inst);
                }
                else if (inst is Application app)
                {
                    if (app.HasResources)
                    {
                        retList.Add(app.Resources);
                    }
                }

                frame = frame.Previous;
            }

            return retList;
        }
    }
}
