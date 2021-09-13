
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
using System.Reflection;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace OpenSilver.Internal.Controls
{
    internal interface ITextBoxViewHost
    {
        TextBoxView View { get; }

        void AttachView(TextBoxView view);

        void DetachView();
    }

    internal class TextBoxViewHost_ContentControl : ITextBoxViewHost
    {
        private readonly ContentControl _host;
        private TextBoxView _view;

        internal TextBoxViewHost_ContentControl(ContentControl host)
        {
            _host = host;
        }

        TextBoxView ITextBoxViewHost.View => _view;

        void ITextBoxViewHost.AttachView(TextBoxView view)
        {
            _view = view;
            _host.Content = view;
        }

        void ITextBoxViewHost.DetachView()
        {
            _view = null;
            _host.Content = null;
        }
    }

    internal class TextBoxViewHost_Border : ITextBoxViewHost
    {
        private readonly Border _host;
        private TextBoxView _view;

        internal TextBoxViewHost_Border(Border host)
        {
            _host = host;
        }

        TextBoxView ITextBoxViewHost.View => _view;

        void ITextBoxViewHost.AttachView(TextBoxView view)
        {
            _view = view;
            _host.Child = view;
        }

        void ITextBoxViewHost.DetachView()
        {
            _view = null;
            _host.Child = null;
        }
    }

    internal class TextBoxViewHost_ContentPresenter : ITextBoxViewHost
    {
        private readonly ContentPresenter _host;
        private TextBoxView _view;

        internal TextBoxViewHost_ContentPresenter(ContentPresenter host)
        {
            _host = host;
        }

        TextBoxView ITextBoxViewHost.View => _view;

        void ITextBoxViewHost.AttachView(TextBoxView view)
        {
            _view = view;
            _host.Content = view;
        }

        void ITextBoxViewHost.DetachView()
        {
            _view = null;
            _host.Content = null;
        }
    }

    internal class TextBoxViewHost_UserControl : ITextBoxViewHost
    {
        private readonly UserControl _host;
        private TextBoxView _view;

        internal TextBoxViewHost_UserControl(UserControl host)
        {
            _host = host;
        }

        TextBoxView ITextBoxViewHost.View => _view;

        void ITextBoxViewHost.AttachView(TextBoxView view)
        {
            _view = view;
            _host.Content = view;
        }

        void ITextBoxViewHost.DetachView()
        {
            _view = null;
            _host.Content = null;
        }
    }

    internal class TextBoxViewHost_Panel : ITextBoxViewHost
    {
        private readonly Panel _host;
        private TextBoxView _view;

        internal TextBoxViewHost_Panel(Panel host)
        {
            _host = host;
        }

        TextBoxView ITextBoxViewHost.View => _view;

        void ITextBoxViewHost.AttachView(TextBoxView view)
        {
            _view = view;
            _host.Children.Add(view);
        }

        void ITextBoxViewHost.DetachView()
        {
            TextBoxView view = _view;
            _view = null;
            _host.Children.Remove(view);
        }
    }

    internal class TextBoxViewHost_ItemsControl : ITextBoxViewHost
    {
        private readonly ItemsControl _host;
        private TextBoxView _view;

        internal TextBoxViewHost_ItemsControl(ItemsControl host)
        {
            _host = host;
        }

        TextBoxView ITextBoxViewHost.View => _view;

        void ITextBoxViewHost.AttachView(TextBoxView view)
        {
            if (_host.HasItems)
            {
                throw new InvalidOperationException();
            }

            _view = view;
            _host.Items.Add(view);
        }

        void ITextBoxViewHost.DetachView()
        {
            TextBoxView view = _view;
            _view = null;
            _host.Items.Remove(view);
        }
    }

    internal class TextBoxViewHost_ContentProperty : ITextBoxViewHost
    {
        private readonly FrameworkElement _host;
        private readonly string _contentPropertyName;
        private TextBoxView _view;
        private bool _isInitialized;
        private MethodInfo _setMethod;

        internal TextBoxViewHost_ContentProperty(FrameworkElement host, string contentPropertyName)
        {
            _host = host;
            _contentPropertyName = contentPropertyName;
        }

        TextBoxView ITextBoxViewHost.View => _view;

        void ITextBoxViewHost.AttachView(TextBoxView view)
        {
            EnsureInitialized();

            _view = view;
            if (_setMethod != null)
            {
                _setMethod.Invoke(_host, new object[1] { view });
            }
        }

        void ITextBoxViewHost.DetachView()
        {
            EnsureInitialized();

            _view = null;
            if (_setMethod != null)
            {
                _setMethod.Invoke(_host, new object[1] { null });
            }
        }

        private void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                PropertyInfo property;
                for (Type type = _host.GetType(); type != null; type = type.BaseType)
                {
                    property = type.GetProperty(
                       _contentPropertyName,
                       BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly
                    );

                    if (property != null)
                    {
                        _setMethod = property.GetSetMethod();
                        break;
                    }
                }

                _isInitialized = true;
            }
        }
    }
}
