
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

using OpenSilver.Internal;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Markup;

namespace System.Windows.Data;

/// <summary>
/// Implements a markup extension that describes the location of the binding source relative to the 
/// position of the binding target.
/// </summary>
[ContentProperty(nameof(Mode))]
[MarkupExtensionReturnType(typeof(RelativeSource))]
public class RelativeSource : MarkupExtension, ISupportInitialize
{
    private RelativeSourceMode _mode;
    private int _ancestorLevel = -1; // while -1, indicates _mode has not been set
    private Type _ancestorType = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="RelativeSource"/> class.
    /// </summary>
    public RelativeSource()
    {
        // default mode to FindAncestor so that setting Type and Level would be OK
        _mode = RelativeSourceMode.FindAncestor;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelativeSource"/> class with an initial mode.
    /// </summary>
    /// <param name="mode">
    /// One of the <see cref="RelativeSourceMode"/> values.
    /// </param>
    public RelativeSource(RelativeSourceMode mode)
    {
        InitializeMode(mode);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelativeSource"/> class with an initial mode and 
    /// additional tree-walking qualifiers for finding the desired relative source.
    /// </summary>
    /// <param name="mode">
    /// One of the <see cref="RelativeSourceMode"/> values. For this signature to be relevant, this 
    /// should be <see cref="RelativeSourceMode.FindAncestor"/>.
    /// </param>
    /// <param name="ancestorType">
    /// The <see cref="Type"/> of ancestor to look for.
    /// </param>
    /// <param name="ancestorLevel">
    /// The ordinal position of the desired ancestor among all ancestors of the given type.
    /// </param>
    public RelativeSource(RelativeSourceMode mode, Type ancestorType, int ancestorLevel)
    {
        InitializeMode(mode);
        AncestorType = ancestorType;
        AncestorLevel = ancestorLevel;
    }

    /// <summary>
    /// Gets a static value that is used to return a <see cref="RelativeSource"/> constructed for the 
    /// <see cref="RelativeSourceMode.Self"/> mode.
    /// </summary>
    /// <returns>
    /// A static <see cref="RelativeSource"/>.
    /// </returns>
    public static RelativeSource Self { get; } = new RelativeSource(RelativeSourceMode.Self);

    /// <summary>
    /// Gets a static value that is used to return a <see cref="RelativeSource"/> constructed for the 
    /// <see cref="RelativeSourceMode.TemplatedParent"/> mode.
    /// </summary>
    /// <returns>
    /// A static <see cref="RelativeSource"/>.
    /// </returns>
    public static RelativeSource TemplatedParent { get; } = new RelativeSource(RelativeSourceMode.TemplatedParent);

    /// <summary>
    /// Gets or sets a <see cref="RelativeSourceMode"/> value that describes the location of the 
    /// binding source relative to the position of the binding target.
    /// </summary>
    /// <returns>
    /// One of the <see cref="RelativeSourceMode"/> values. The default value is null.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// This property is immutable after initialization. Instead of changing the <see cref="Mode"/> on 
    /// this instance, create a new <see cref="RelativeSource"/> or use a different static instance.
    /// </exception>
    public RelativeSourceMode Mode
    {
        get => _mode;
        set
        {
            if (IsUninitialized)
            {
                InitializeMode(value);
            }
            else if (value != _mode) // mode changes are not allowed
            {
                throw new InvalidOperationException(Strings.RelativeSourceModeIsImmutable);
            }
        }
    }

    /// <summary>
    /// Gets or sets the level of ancestor to look for, in <see cref="RelativeSourceMode.FindAncestor"/>
    /// mode. Use 1 to indicate the one nearest to the binding target element.
    /// </summary>
    /// <returns>
    /// The ancestor level. Use 1 to indicate the one nearest to the binding target element.
    /// </returns>
    public int AncestorLevel
    {
        get => _ancestorLevel;
        set
        {
            Debug.Assert(!IsUninitialized || _mode == RelativeSourceMode.FindAncestor);

            if (_mode != RelativeSourceMode.FindAncestor)
            {
                // in all other modes, AncestorLevel should not get set to a non-zero value
                if (value != 0)
                {
                    throw new InvalidOperationException(Strings.RelativeSourceNotInFindAncestorMode);
                }
            }
            else if (value < 1)
            {
                throw new ArgumentOutOfRangeException(Strings.RelativeSourceInvalidAncestorLevel);
            }
            else
            {
                _ancestorLevel = value;
            }
        }
    }

    /// <summary>
    /// Gets or sets the type of ancestor to look for.
    /// </summary>
    /// <returns>
    /// The type of ancestor. The default value is null.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The <see cref="RelativeSource"/> is not in the <see cref="RelativeSourceMode.FindAncestor"/> mode.
    /// </exception>
    public Type AncestorType
    {
        get => _ancestorType;
        set
        {
            if (IsUninitialized)
            {
                Debug.Assert(_mode == RelativeSourceMode.FindAncestor);
                AncestorLevel = 1;  // lock the mode and set default level
            }

            if (_mode != RelativeSourceMode.FindAncestor)
            {
                // in all other modes, AncestorType should not get set to a non-null value
                if (value is not null)
                {
                    throw new InvalidOperationException(Strings.RelativeSourceNotInFindAncestorMode);
                }
            }
            else
            {
                _ancestorType = value;
            }
        }
    }

    /// <summary>
    /// Returns an object that should be set as the value on the target object's property for this markup
    /// extension. For <see cref="RelativeSource"/>, this is another <see cref="RelativeSource"/>, using 
    /// the appropriate source for the specified mode.
    /// </summary>
    /// <param name="serviceProvider">
    /// An object that can provide services for the markup extension. In this implementation, this parameter 
    /// can be null.
    /// </param>
    /// <returns>
    /// Another <see cref="RelativeSource"/>.
    /// </returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return _mode switch
        {
            RelativeSourceMode.Self => Self,
            RelativeSourceMode.TemplatedParent => TemplatedParent,
            _ => this,
        };
    }

    void ISupportInitialize.BeginInit() { }

    void ISupportInitialize.EndInit()
    {
        if (IsUninitialized)
        {
            throw new InvalidOperationException(Strings.RelativeSourceNeedsMode);
        }

        if (_mode == RelativeSourceMode.FindAncestor && AncestorType is null)
        {
            throw new InvalidOperationException(Strings.RelativeSourceNeedsAncestorType);
        }
    }

    private bool IsUninitialized => _ancestorLevel == -1;

    private void InitializeMode(RelativeSourceMode mode)
    {
        Debug.Assert(IsUninitialized);

        if (mode == RelativeSourceMode.FindAncestor)
        {
            // default level
            _ancestorLevel = 1;
            _mode = mode;
        }
        else if (mode == RelativeSourceMode.Self || mode == RelativeSourceMode.TemplatedParent)
        {
            _ancestorLevel = 0;
            _mode = mode;
        }
        else
        {
            throw new ArgumentException(Strings.RelativeSourceModeInvalid, nameof(mode));
        }
    }
}