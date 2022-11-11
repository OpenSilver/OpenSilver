
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
using System.Linq;
using System.Reflection;
using System.Windows.Markup;

#if MIGRATION
using System.Windows;
using System.Windows.Controls;
#else
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
#endif

namespace OpenSilver.Internal.Controls
{
    internal interface ITextBoxView { }

    internal interface ITextBoxViewHost<T> where T : FrameworkElement, ITextBoxView
    {
        T View { get; }

        void AttachView(T view);

        void DetachView();
    }

    internal sealed class TextBoxViewHost_ContentControl<T> : ITextBoxViewHost<T> where T : FrameworkElement, ITextBoxView
    {
        private readonly ContentControl _host;
        private T _view;

        internal TextBoxViewHost_ContentControl(ContentControl host)
        {
            _host = host;
        }

        T ITextBoxViewHost<T>.View => _view;

        void ITextBoxViewHost<T>.AttachView(T view)
        {
            _view = view;
            _host.Content = view;
        }

        void ITextBoxViewHost<T>.DetachView()
        {
            _view = null;
            _host.Content = null;
        }
    }

    internal sealed class TextBoxViewHost_Border<T> : ITextBoxViewHost<T> where T : FrameworkElement, ITextBoxView
    {
        private readonly Border _host;
        private T _view;

        internal TextBoxViewHost_Border(Border host)
        {
            _host = host;
        }

        T ITextBoxViewHost<T>.View => _view;

        void ITextBoxViewHost<T>.AttachView(T view)
        {
            _view = view;
            _host.Child = view;
        }

        void ITextBoxViewHost<T>.DetachView()
        {
            _view = null;
            _host.Child = null;
        }
    }

    internal sealed class TextBoxViewHost_ContentPresenter<T> : ITextBoxViewHost<T> where T : FrameworkElement, ITextBoxView
    {
        private readonly ContentPresenter _host;
        private T _view;

        internal TextBoxViewHost_ContentPresenter(ContentPresenter host)
        {
            _host = host;
        }

        T ITextBoxViewHost<T>.View => _view;

        void ITextBoxViewHost<T>.AttachView(T view)
        {
            _view = view;
            _host.Content = view;
        }

        void ITextBoxViewHost<T>.DetachView()
        {
            _view = null;
            _host.Content = null;
        }
    }

    internal sealed class TextBoxViewHost_UserControl<T> : ITextBoxViewHost<T> where T : FrameworkElement, ITextBoxView
    {
        private readonly UserControl _host;
        private T _view;

        internal TextBoxViewHost_UserControl(UserControl host)
        {
            _host = host;
        }

        T ITextBoxViewHost<T>.View => _view;

        void ITextBoxViewHost<T>.AttachView(T view)
        {
            _view = view;
            _host.Content = view;
        }

        void ITextBoxViewHost<T>.DetachView()
        {
            _view = null;
            _host.Content = null;
        }
    }

    internal sealed class TextBoxViewHost_Panel<T> : ITextBoxViewHost<T> where T : FrameworkElement, ITextBoxView
    {
        private readonly Panel _host;
        private T _view;

        internal TextBoxViewHost_Panel(Panel host)
        {
            _host = host;
        }

        T ITextBoxViewHost<T>.View => _view;

        void ITextBoxViewHost<T>.AttachView(T view)
        {
            _view = view;
            _host.Children.Add(view);
        }

        void ITextBoxViewHost<T>.DetachView()
        {
            T view = _view;
            _view = null;
            _host.Children.Remove(view);
        }
    }

    internal sealed class TextBoxViewHost_ItemsControl<T> : ITextBoxViewHost<T> where T : FrameworkElement, ITextBoxView
    {
        private readonly ItemsControl _host;
        private T _view;

        internal TextBoxViewHost_ItemsControl(ItemsControl host)
        {
            _host = host;
        }

        T ITextBoxViewHost<T>.View => _view;

        void ITextBoxViewHost<T>.AttachView(T view)
        {
            if (_host.HasItems)
            {
                throw new InvalidOperationException();
            }

            _view = view;
            _host.Items.Add(view);
        }

        void ITextBoxViewHost<T>.DetachView()
        {
            T view = _view;
            _view = null;
            _host.Items.Remove(view);
        }
    }

    internal sealed class TextBoxViewHost_ContentProperty<T> : ITextBoxViewHost<T> where T : FrameworkElement, ITextBoxView
    {
        private readonly FrameworkElement _host;
        private readonly string _contentPropertyName;
        private T _view;
        private bool _isInitialized;
        private MethodInfo _setMethod;

        internal TextBoxViewHost_ContentProperty(FrameworkElement host, string contentPropertyName)
        {
            _host = host;
            _contentPropertyName = contentPropertyName;
        }

        T ITextBoxViewHost<T>.View => _view;

        void ITextBoxViewHost<T>.AttachView(T view)
        {
            EnsureInitialized();

            _view = view;
            if (_setMethod != null)
            {
                _setMethod.Invoke(_host, new object[1] { view });
            }
        }

        void ITextBoxViewHost<T>.DetachView()
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
