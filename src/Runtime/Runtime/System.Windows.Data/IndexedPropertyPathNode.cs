

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


using DotNetForHtml5.Core;
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
    internal partial class IndexedPropertyPathNode : PropertyPathNode
    {
        //Note: limitations of this class (that I could think of)
        //      - If the source object's type defines this[...] in different ways, it will not work (see comments in the GetMethod method in this class).
        //      - If it defines this[..] only with multiple parameters, it will not work
        //      - We will not know when the element changes outside of this class because we cannot have a listener on a method's call (so it the developper calls theSourceItem[XXX] = val, nothing will happen in this class).
        //
        //  The first two limitations are due to the fact that neither Type.GetMethod(string name, BindingFlags bindingAttr, Type[] parameterTypes) nor MethodInfo.ParameterTypes are implemented in Bridge so we cannot know the amount and the type of the parameters of the method.
        //  For now, I have decided to only support one parameter, otherwise it would become much more complicated (index comes as a string so we would need to check whether there are commas, how many, and I think it could still be considered as only one parameter: a string with commas in it).

        //Note: The BindsDirectlyToSource from the StandardPropertyPathNode class would always be considered true in this class.
        readonly string _index = null;
        object _parsedIndex = null;
        MethodInfo _setMethod = null;
        MethodInfo _getMethod = null;

        internal IndexedPropertyPathNode(string index)
        {
            this._index = index;
        }

        internal override void SetValue(object value)
        {
            if (this._setMethod != null)
            {
                this.TryInvokeSetMethod(this._parsedIndex, value);
            }
        }

        private bool TryInvokeSetMethod(object index, object value)
        {
            try
            {
                this._setMethod.Invoke(this.Source, new object[] { index, value });
                return true;
            }
            catch
            {
                return false;
            }
        }

        internal override void UpdateValue()
        {
            if (this._getMethod != null)
            {
                object result;
                if (this.TryInvokeGetMethod(this._parsedIndex, out result))
                {
                    this.UpdateValueAndIsBroken(result, this.CheckIsBroken(true));
                }
                // else should we do something to say it is broken ?
            }
        }

        private bool TryInvokeGetMethod(object index, out object result)
        {
            try
            {
                result = this._getMethod.Invoke(this.Source, new object[] { index });
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
            if (this.Source != null)
            {
                if (newValue != null)
                {
                    this.GetIndexerInfo(newValue.GetType());
                }
            }
        }

        private PropertyInfo GetIndexer(Type type)
        {
            PropertyInfo singleParameterIndexer = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).FirstOrDefault(property =>
            {
                return property.GetMethod != null && property.GetIndexParameters().Length == 1;
            });
            if (singleParameterIndexer == null)
            {
                throw new NotSupportedException("Indexers with multiple parameters are not supported yet for Binding.");
            }
            return singleParameterIndexer;
        }

        private void GetIndexerInfo(Type sourceType)
        {
            PropertyInfo indexer = this.GetIndexer(sourceType);
            this.PrepareIndexForIndexer(indexer);
            if (indexer != null)
            {
                this._getMethod = indexer.GetMethod;
                this._setMethod = indexer.SetMethod;
            }
            else
            {
                this._getMethod = null;
                this._setMethod = null;
            }
        }

        private void PrepareIndexForIndexer(PropertyInfo indexer)
        {
            Type indexType = indexer.GetIndexParameters().First().ParameterType;
            try
            {
                this._parsedIndex = Convert.ChangeType(this._index, indexType);
            }
            catch (Exception)
            {
                if (TypeFromStringConverters.CanTypeBeConverted(indexType))
                {
                    this._parsedIndex = TypeFromStringConverters.ConvertFromInvariantString(indexType, this._index); //todo: maybe in try / catch ?
                }
                else
                {
                    this._parsedIndex = this._index; //todo: maybe null ?
                }
            }
        }
    }
}
