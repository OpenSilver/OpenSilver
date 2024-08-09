
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

using System.Diagnostics;

namespace System.Windows;

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
        OldDictionary = null;
        NewDictionary = null;
        Key = key;
        _flags = 0;
    }

    /// <summary>
    ///     This constructor is used for notifying changes in Application.Resources,
    ///     FE.Resources, ResourceDictionary.EndInit
    /// </summary>
    internal ResourcesChangeInfo(ResourceDictionary oldDictionary, ResourceDictionary newDictionary)
    {
        OldDictionary = oldDictionary;
        NewDictionary = newDictionary;
        Key = null;
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
    internal bool IsResourceAddOperation => Key != null || NewDictionary != null;

    internal object Key { get; }

    internal ResourceDictionary NewDictionary { get; }

    internal ResourceDictionary OldDictionary { get; }

    // Says if either the old or the new dictionaries contain the given key
    internal bool Contains(object key, bool isImplicitStyleKey)
    {
        if (IsTreeChange || IsCatastrophicDictionaryChange)
        {
            return true;
        }
        else if (IsThemeChange || IsSysColorsOrSettingsChange)
        {
            // Implicit Styles are not fetched from the Themes.
            // So we do not need to respond to theme changes.
            // This is a performance optimization.

            return !isImplicitStyleKey;
        }

        Debug.Assert(OldDictionary != null || NewDictionary != null || Key != null, "Must have a dictionary or a key that has changed");

        if (Key != null && Equals(Key, key))
        {
            return true;
        }

        if (OldDictionary != null && OldDictionary.Contains(key))
        {
            return true;
        }

        if (NewDictionary != null && NewDictionary.Contains(key))
        {
            return true;
        }

        return false;
    }

    // determine whether this change affects implicit data templates
    internal void SetIsImplicitDataTemplateChange()
    {
        bool isImplicitDataTemplateChange = IsCatastrophicDictionaryChange || Key is DataTemplateKey;

        if (!isImplicitDataTemplateChange && OldDictionary != null)
        {
            if (OldDictionary.HasImplicitDataTemplates)
            {
                isImplicitDataTemplateChange = true;
            }
        }

        if (!isImplicitDataTemplateChange && NewDictionary != null)
        {
            if (NewDictionary.HasImplicitDataTemplates)
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

    private PrivateFlags _flags;
}