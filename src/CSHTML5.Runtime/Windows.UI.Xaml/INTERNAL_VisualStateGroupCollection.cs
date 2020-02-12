
//===============================================================================
//
//  IMPORTANT NOTICE, PLEASE READ CAREFULLY:
//
//  => This code is licensed under the GNU General Public License (GPL v3). A copy of the license is available at:
//        https://www.gnu.org/licenses/gpl.txt
//
//  => As stated in the license text linked above, "The GNU General Public License does not permit incorporating your program into proprietary programs". It also does not permit incorporating this code into non-GPL-licensed code (such as MIT-licensed code) in such a way that results in a non-GPL-licensed work (please refer to the license text for the precise terms).
//
//  => Licenses that permit proprietary use are available at:
//        http://www.cshtml5.com
//
//  => Copyright 2019 Userware/CSHTML5. This code is part of the CSHTML5 product (cshtml5.com).
//
//===============================================================================



using CSHTML5.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    /// Do not use this class
    /// </summary>
    /// <exclude/>
    public partial class INTERNAL_VisualStateGroupCollection : IList<VisualStateGroup>
    {
        Dictionary<string, VisualStateGroup> _visualStateGroups = new Dictionary<string,VisualStateGroup>();
        Dictionary<string, VisualState> _visualStates = new Dictionary<string,VisualState>();

        internal VisualState GetVisualState(string visualStateName)
        {
            if (visualStateName != null)
            {
                if (_visualStates != null && _visualStates.ContainsKey(visualStateName))
                {
                    return _visualStates[visualStateName];
                }
                else
                {
                    // Ignore if Visual State not found:
                    return null;

                    //throw new ArgumentException(string.Format(@"VisualState with name ""{0}"" not found.", visualStateName));
                }
            }
            throw new ArgumentNullException("visualStateName");
        }

        public int IndexOf(VisualStateGroup item)
        {
            int i = 0;
            while (i < _visualStateGroups.Count)
            {
#if BRIDGE
                if (INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(_visualStateGroups).ElementAt(i) == item)
#else
                if (_visualStateGroups.Values.ElementAt(i) == item)
#endif
                {
                    return i;
                }
                ++i;
            }
            return -1;
        }

        public void Insert(int index, VisualStateGroup item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public VisualStateGroup this[int index]
        {
            get
            {
#if BRIDGE
                return INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(_visualStateGroups).ElementAt(index);
#else
                return _visualStateGroups.Values.ElementAt(index);
#endif
            }
            set
            {
                //todo: refresh the VisualStates
                throw new NotImplementedException();
                _visualStateGroups[_visualStateGroups.Keys.ElementAt(index)] = value;
            }
        }

        public void Add(VisualStateGroup item)
        {
            if (_visualStateGroups.ContainsKey(item.Name))
            {
                throw new InvalidOperationException(string.Format(@"Multiple VisualStatesGroups with the Name:""{0}"" appeared in the same scope.",item.Name));
            }
            _visualStateGroups.Add(item.Name, item);
            foreach (VisualState visualState in item.States)
            {
                if (_visualStates.ContainsKey(visualState.Name))
                {
                    throw new InvalidOperationException(string.Format(@"Multiple VisualStates with the Name:""{0}"" appeared in the same scope.", visualState.Name));
                }
                visualState.INTERNAL_Group = item;
                _visualStates.Add(visualState.Name, visualState);
            }

        }

        public void Clear()
        {
            _visualStateGroups.Clear();
            _visualStates.Clear();
        }

        public bool Contains(VisualStateGroup item)
        {
            return _visualStateGroups.ContainsValue(item);
        }

        public void CopyTo(VisualStateGroup[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return _visualStateGroups.Count; }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(VisualStateGroup item)
        {
            _visualStateGroups.Remove(item.Name);
            foreach (VisualState state in item.States)
            {
                if(_visualStates.ContainsKey(state.Name))
                {
                    _visualStates.Remove(state.Name);
                }
            }
            return true;
        }

        public IEnumerator<VisualStateGroup> GetEnumerator()
        {
#if BRIDGE
            return INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(_visualStateGroups).GetEnumerator();
#else
            return _visualStateGroups.Values.GetEnumerator();
#endif
        }

        global::System.Collections.IEnumerator global::System.Collections.IEnumerable.GetEnumerator()
        {
#if BRIDGE
            return INTERNAL_BridgeWorkarounds.GetDictionaryValues_SimulatorCompatible(_visualStateGroups).GetEnumerator();
#else
            return _visualStateGroups.Values.GetEnumerator();
#endif
        }

        internal bool ContainsVisualState(string visualStateName)
        {
            return _visualStates.ContainsKey(visualStateName);
        }
    }
}
