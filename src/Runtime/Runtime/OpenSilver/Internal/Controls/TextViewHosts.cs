
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
using System.Windows;
using System.Windows.Controls;

namespace OpenSilver.Internal.Controls;

internal interface ITextViewHost<T>
    where T : FrameworkElement
{
    T View { get; }

    void AttachView(T view);

    void DetachView();
}

internal static class TextViewHostProvider
{
    internal static ITextViewHost<T> From<T>(FrameworkElement contentElement)
        where T : FrameworkElement
    {
        return contentElement switch
        {
            ContentControl cc => new TextViewHost_ContentControl<T>(cc),
            ContentPresenter cp => new TextViewHost_ContentPresenter<T>(cp),
            Border border => new TextViewHost_Border<T>(border),
            UserControl uc => new TextViewHost_UserControl<T>(uc),
            Panel panel => new TextViewHost_Panel<T>(panel),
            ItemsControl ic => new TextViewHost_ItemsControl<T>(ic),
            _ when IsContentPropertyHost(contentElement, out string contentPropertyName) => new TextViewHost_ContentProperty<T>(contentElement, contentPropertyName),
            _ => null,
        };
    }

    private static bool IsContentPropertyHost(FrameworkElement host, out string contentPropertyName)
    {
        ContentPropertyAttribute contentProp = (ContentPropertyAttribute)host
            .GetType()
            .GetCustomAttributes(typeof(ContentPropertyAttribute), true)
            .FirstOrDefault();

        if (contentProp != null)
        {
            contentPropertyName = contentProp.Name;
            return true;
        }

        contentPropertyName = null;
        return false;
    }

    private sealed class TextViewHost_ContentControl<T> : ITextViewHost<T> where T : FrameworkElement
    {
        private readonly ContentControl _host;
        private T _view;

        internal TextViewHost_ContentControl(ContentControl host)
        {
            _host = host;
        }

        T ITextViewHost<T>.View => _view;

        void ITextViewHost<T>.AttachView(T view)
        {
            _view = view;
            _host.Content = view;
        }

        void ITextViewHost<T>.DetachView()
        {
            _view = null;
            _host.Content = null;
        }
    }

    private sealed class TextViewHost_Border<T> : ITextViewHost<T> where T : FrameworkElement
    {
        private readonly Border _host;
        private T _view;

        internal TextViewHost_Border(Border host)
        {
            _host = host;
        }

        T ITextViewHost<T>.View => _view;

        void ITextViewHost<T>.AttachView(T view)
        {
            _view = view;
            _host.Child = view;
        }

        void ITextViewHost<T>.DetachView()
        {
            _view = null;
            _host.Child = null;
        }
    }

    private sealed class TextViewHost_ContentPresenter<T> : ITextViewHost<T> where T : FrameworkElement
    {
        private readonly ContentPresenter _host;
        private T _view;

        internal TextViewHost_ContentPresenter(ContentPresenter host)
        {
            _host = host;
        }

        T ITextViewHost<T>.View => _view;

        void ITextViewHost<T>.AttachView(T view)
        {
            _view = view;
            _host.Content = view;
        }

        void ITextViewHost<T>.DetachView()
        {
            _view = null;
            _host.Content = null;
        }
    }

    private sealed class TextViewHost_UserControl<T> : ITextViewHost<T> where T : FrameworkElement
    {
        private readonly UserControl _host;
        private T _view;

        internal TextViewHost_UserControl(UserControl host)
        {
            _host = host;
        }

        T ITextViewHost<T>.View => _view;

        void ITextViewHost<T>.AttachView(T view)
        {
            _view = view;
            _host.Content = view;
        }

        void ITextViewHost<T>.DetachView()
        {
            _view = null;
            _host.Content = null;
        }
    }

    private sealed class TextViewHost_Panel<T> : ITextViewHost<T> where T : FrameworkElement
    {
        private readonly Panel _host;
        private T _view;

        internal TextViewHost_Panel(Panel host)
        {
            _host = host;
        }

        T ITextViewHost<T>.View => _view;

        void ITextViewHost<T>.AttachView(T view)
        {
            _view = view;
            _host.Children.Add(view);
        }

        void ITextViewHost<T>.DetachView()
        {
            T view = _view;
            _view = null;
            _host.Children.Remove(view);
        }
    }

    private sealed class TextViewHost_ItemsControl<T> : ITextViewHost<T> where T : FrameworkElement
    {
        private readonly ItemsControl _host;
        private T _view;

        internal TextViewHost_ItemsControl(ItemsControl host)
        {
            _host = host;
        }

        T ITextViewHost<T>.View => _view;

        void ITextViewHost<T>.AttachView(T view)
        {
            if (_host.HasItems)
            {
                throw new InvalidOperationException();
            }

            _view = view;
            _host.Items.Add(view);
        }

        void ITextViewHost<T>.DetachView()
        {
            T view = _view;
            _view = null;
            _host.Items.Remove(view);
        }
    }

    private sealed class TextViewHost_ContentProperty<T> : ITextViewHost<T> where T : FrameworkElement
    {
        private readonly FrameworkElement _host;
        private readonly string _contentPropertyName;
        private T _view;
        private bool _isInitialized;
        private MethodInfo _setMethod;

        internal TextViewHost_ContentProperty(FrameworkElement host, string contentPropertyName)
        {
            _host = host;
            _contentPropertyName = contentPropertyName;
        }

        T ITextViewHost<T>.View => _view;

        void ITextViewHost<T>.AttachView(T view)
        {
            EnsureInitialized();

            _view = view;
            if (_setMethod != null)
            {
                _setMethod.Invoke(_host, new object[1] { view });
            }
        }

        void ITextViewHost<T>.DetachView()
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
