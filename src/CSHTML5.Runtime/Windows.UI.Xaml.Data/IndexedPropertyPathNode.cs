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
                bool attemptParsedIndex = false; //todo-perf: keep attemptParsedIndex, parsedIndex, as well as a boolean saying whether we should use the parsedIndex. That way we will only call TryInvokeMethod once when needed and won't need to retry to parse the index.
                int parsedIndex = -1;
                if (int.TryParse(_index, out parsedIndex))
                {
                    attemptParsedIndex = true;
                }
                if (!TryInvokeSetMethod(parsedIndex, value)) //we attempt to invoke the method with an int as parameter but there is no guarantee that what is expected is an int so we keep the option of a string as a Backup.
                {
                    TryInvokeSetMethod(_index, value);
                }
            }
        }

        bool TryInvokeSetMethod(object index, object value)
        {
            try
            {
                _setMethod.Invoke(this.Source, new object[] { index, value });
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
            {
                bool getMethodInvokedSuccessfully = false;
                bool attemptParsedIndex = false; //todo-perf: keep attemptParsedIndex, parsedIndex, as well as a boolean saying whether we should use the parsedIndex. That way we will only call TryInvokeMethod once when needed and won't need to retry to parse the index.
                int parsedIndex = -1;
                object result;
                if (int.TryParse(_index, out parsedIndex))
                {
                    attemptParsedIndex = true;
                }
                getMethodInvokedSuccessfully = TryInvokeGetMethod(parsedIndex, out result);
                if (!getMethodInvokedSuccessfully) //we attempt to invoke the method with an int as parameter but there is no guarantee that what is expected is an int so we keep the option of a string as a Backup.
                {
                    getMethodInvokedSuccessfully = TryInvokeGetMethod(_index, out result);
                }

                if (getMethodInvokedSuccessfully)
                {
                    this.UpdateValueAndIsBroken(result, CheckIsBroken(true));
                }
                // else should we do something to say it is broken ?

            }
        }

        bool TryInvokeGetMethod(object index, out object result)
        {
            try
            {
                result = _getMethod.Invoke(this.Source, new object[] { index });
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        internal override void OnSourceChanged(object oldvalue, object newValue)
        {
            //todo: (?) find out how to have a listener here since it is a method and not a DependencyProperty (get_Item and set_Item). I guess it would be nice to be able to attach to calls on set_item and handle it from there.

            //var listener = _dependencyPropertyListener;
            //if (listener != null)
            //{
            //    listener.Detach();
            //    _dependencyPropertyListener = listener = null;
            //}

            if (Source == null)
                return;

            if (newValue != null)
            {
                Type sourceType = newValue.GetType();

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
