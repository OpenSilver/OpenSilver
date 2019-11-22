using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows.Data
#else
namespace Windows.UI.Xaml.Data
#endif
{
    class IndexedPropertyPathNode : PropertyPathNode
    {
        //Note: limitations of this class (that I could think of)
        //      - If the source object's type defines this[...] in different ways, it will not work (see comments in the GetMethod method in this class).
        //      - If it defines this[..] only with multiple parameters, it will not work
        //      - We will not know when the element changes outside of this class because we cannot have a listener on a method's call (so it the developper calls theSourceItem[XXX] = val, nothing will happen in this class).
        //
        //  The first two limitations are due to the fact that neither Type.GetMethod(string name, BindingFlags bindingAttr, Type[] parameterTypes) nor MethodInfo.ParameterTypes are implemented in Bridge so we cannot know the amount and the type of the parameters of the method.
        //  For now, I have decided to only support one parameter, otherwise it would become much more complicated (index comes as a string so we would need to check whether there are commas, how many, and I think it could still be considered as only one parameter: a string with commas in it).

        //Note: The BindsDirectlyToSource from the StandardPropertyPathNode class would always be considered true in this class.
        string _index = null;
        MethodInfo _setMethod = null;
        MethodInfo _getMethod = null;
        internal IndexedPropertyPathNode(string index)
        {
            _index = index;
        }

        internal override void SetValue(object value)
        {
            if (_setMethod != null)
            {
                bool attemptParsedIndex = false;
                int parsedIndex = -1;
                if (int.TryParse(_index, out parsedIndex))
                {
                    attemptParsedIndex = true;
                }
                if (!TryInvokeMethod(parsedIndex, value)) //we attempt to invoke the method with an int as parameter but there is no guarantee that what is expected is an int so we keep the option of a string as a Backup.
                {
                    TryInvokeMethod(_index, value);
                }
            }
        }

        bool TryInvokeMethod(object index, object value)
        {
            try
            {
                _setMethod.Invoke(this.Source, _index, value);
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal override void UpdateValue()
        {
            if (_getMethod != null)
                this.UpdateValueAndIsBroken(_getMethod.Invoke(Source, _index), CheckIsBroken(true));
        }

        internal override void OnSourceChanged(object oldvalue, object newValue)
        {
            DependencyObject oldSource = null;
            DependencyObject newSource = null;
            if (oldvalue is DependencyObject)
            {
                oldSource = (DependencyObject)oldvalue;
            }
            if (newValue is DependencyObject)
            {
                newSource = (DependencyObject)newValue;
            }

            //todo: (?) find out how to have a listener here since it is a method and not a DependencyProperty (get_Item and set_Item). I guess it would be nice to be able to attach to calls on set_item and handle it from there.

            //var listener = _dependencyPropertyListener;
            //if (listener != null)
            //{
            //    listener.Detach();
            //    _dependencyPropertyListener = listener = null;
            //}

            if (Source == null)
                return;

            if (newSource != null)
            {
                Type sourceType = newSource.GetType();
                //todo: try/catch around the following line because there can be errors (for example if the source type defines both: public int this[int index] and public int this[int index1, int index2])

                _getMethod = GetMethod(sourceType, "get_Item");
                _setMethod = GetMethod(sourceType, "set_Item");
            }
        }

        MethodInfo GetMethod(Type sourceType, string methodName)
        {
            try
            {
                //Note: returns null if not found. 
                return sourceType.GetMethod(methodName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static); //This will throw an ambiguous thingy exception if this[...] is defined more than once in the type.
            }
            catch //todo: make it so that there is no need for the try/catch (so fix the comments in the catch.
            {
                //we can have a System.Reflection.AmbiguousMatchException here if the type has more than one implementation for this[..]
                //for example:
                //      -public int this[int index]
                //      -public int this[int index1, int index2]

                //Note: We cannot use Type.GetMethod(string name, BindingFlags bindingAttr, Type[] parameterTypes) because it does not exist in Bridge yet. (we could probably find out which one is the correct one using the format of _index).
                return null;
            }
        }

    }
}
