
using System.Collections;
using System.Collections.Generic;

#if MIGRATION
using System.Windows;
#else
using Windows.UI.Xaml;
#endif

namespace Experimental
{
    public class GenericType<K, B, T>
    {
        public T MyProperty { get; set; }

        public AnotherGenericType<AnotherGenericType<B>> PropertyWithNestedGeneric { get; set; }

        public int MyNonGenericProperty { get; set; }

        public static string MyField = "MyField";

        public List<int> ListField = new();

        public Dictionary<int, int> DictionaryField = new();

        public IList IListField = new List<string>();

        public static readonly DependencyProperty HasSomethingProperty =
            DependencyProperty.RegisterAttached(
                "HasSomething",
                typeof(T), typeof(GenericType<K, B, T>), null);

        public static T GetHasSomething(UIElement target) =>
            (T)target.GetValue(HasSomethingProperty);

        public static void SetHasSomething(UIElement target, T value) =>
            target.SetValue(HasSomethingProperty, value);

        public T MethodWithGenericReturnType()
        {
            return default;
        }

        public AnotherGenericType<T> MethodReturnsAnotherGeneric()
        {
            return new AnotherGenericType<T>();
        }
    }
}
