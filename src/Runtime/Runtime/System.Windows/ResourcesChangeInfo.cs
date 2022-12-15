
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

#if MIGRATION
namespace System.Windows
#else
namespace Windows.UI.Xaml
#endif
{
    /// <summary>
    ///     This is the data that is passed through the DescendentsWalker
    ///     during a resources change tree-walk.
    /// </summary>
    internal struct ResourcesChangeInfo
    {
        /// <summary>
        ///     This constructor is used for notifying changes to individual
        ///     entries in a ResourceDictionary
        /// </summary>
        internal ResourcesChangeInfo(object key)
        {
            _oldDictionary = null;
            _newDictionary = null;
            _key = key;
            _flags = 0;
        }

        /// <summary>
        ///     This constructor is used for notifying changes in Application.Resources,
        ///     FE.Resources, ResourceDictionary.EndInit
        /// </summary>
        internal ResourcesChangeInfo(ResourceDictionary oldDictionary, ResourceDictionary newDictionary)
        {
            _oldDictionary = oldDictionary;
            _newDictionary = newDictionary;
            _key = null;
            _flags = 0;
        }

        /// <summary>
        ///     This is a static accessor for a ResourcesChangeInfo that is used
        ///     for any ResourceDictionary operations that we aren't able to provide
        ///     the precise 'key that changed' information
        /// </summary>
        internal static ResourcesChangeInfo CatastrophicDictionaryChangeInfo
            => new() { IsCatastrophicDictionaryChange = true };

        // This flag is used to indicate that a theme change has occured
        internal bool IsThemeChange
        {
            get => ReadPrivateFlag(PrivateFlags.IsThemeChange);
            set => WritePrivateFlag(PrivateFlags.IsThemeChange, value);
        }

        // This flag is used to indicate that a tree change has occured
        internal bool IsTreeChange
        {
            get => ReadPrivateFlag(PrivateFlags.IsTreeChange);
            set => WritePrivateFlag(PrivateFlags.IsTreeChange, value);
        }

        // This flag is used to indicate that a style has changed
        internal bool IsStyleResourcesChange
        {
            get => ReadPrivateFlag(PrivateFlags.IsStyleResourceChange);
            set => WritePrivateFlag(PrivateFlags.IsStyleResourceChange, value);
        }

        // This flag is used to indicate that this resource change was triggered from a Template change
        internal bool IsTemplateResourcesChange
        {
            get => ReadPrivateFlag(PrivateFlags.IsTemplateResourceChange);
            set => WritePrivateFlag(PrivateFlags.IsTemplateResourceChange, value);
        }

        // This flag is used to indicate that a system color or settings change has occured
        internal bool IsSysColorsOrSettingsChange
        {
            get => ReadPrivateFlag(PrivateFlags.IsSysColorsOrSettingsChange);
            set => WritePrivateFlag(PrivateFlags.IsSysColorsOrSettingsChange, value);
        }

        // This flag is used to indicate that a catastrophic dictionary change has occured
        internal bool IsCatastrophicDictionaryChange
        {
            get => ReadPrivateFlag(PrivateFlags.IsCatastrophicDictionaryChange);
            set => WritePrivateFlag(PrivateFlags.IsCatastrophicDictionaryChange, value);
        }

        // This flag is used to indicate that an implicit data template change has occured
        internal bool IsImplicitDataTemplateChange
        {
            get => ReadPrivateFlag(PrivateFlags.IsImplicitDataTemplateChange);
            set => WritePrivateFlag(PrivateFlags.IsImplicitDataTemplateChange, value);
        }

        // This flag is used to indicate if the current operation is an effective add operation
        internal bool IsResourceAddOperation => _key != null || _newDictionary != null;

        internal object Key => _key;

        internal ResourceDictionary NewDictionary => _newDictionary;

        internal ResourceDictionary OldDictionary => _oldDictionary;

        // determine whether this change affects implicit data templates
        internal void SetIsImplicitDataTemplateChange()
        {
            bool isImplicitDataTemplateChange = IsCatastrophicDictionaryChange || _key is DataTemplateKey;

            if (!isImplicitDataTemplateChange && _oldDictionary != null)
            {
                if (_oldDictionary.HasImplicitDataTemplates)
                {
                    isImplicitDataTemplateChange = true;
                }
            }

            if (!isImplicitDataTemplateChange && _newDictionary != null)
            {
                if (_newDictionary.HasImplicitDataTemplates)
                {
                    isImplicitDataTemplateChange = true;
                }
            }

            IsImplicitDataTemplateChange = isImplicitDataTemplateChange;
        }

        private void WritePrivateFlag(PrivateFlags bit, bool value)
        {
            if (value)
            {
                _flags |= bit;
            }
            else
            {
                _flags &= ~bit;
            }
        }

        private bool ReadPrivateFlag(PrivateFlags bit) => (_flags & bit) != 0;

        private enum PrivateFlags : byte
        {
            IsThemeChange = 0x01,
            IsTreeChange = 0x02,
            IsStyleResourceChange = 0x04,
            IsTemplateResourceChange = 0x08,
            IsSysColorsOrSettingsChange = 0x10,
            IsCatastrophicDictionaryChange = 0x20,
            IsImplicitDataTemplateChange = 0x40,
        }

        private ResourceDictionary _oldDictionary;
        private ResourceDictionary _newDictionary;
        private object _key;
        private PrivateFlags _flags;
    }
}