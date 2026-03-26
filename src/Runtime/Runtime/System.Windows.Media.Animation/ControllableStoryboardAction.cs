
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
using System.Windows.Markup;
using OpenSilver.Internal;

namespace System.Windows.Media.Animation;

/// <summary>
/// Manipulates a <see cref="Storyboard"/> that has been applied by a <see cref="BeginStoryboard"/> action.
/// </summary>
public abstract class ControllableStoryboardAction : TriggerAction
{
    private string _beginStoryboardName;

    internal ControllableStoryboardAction() { }

    /// <summary>
    /// Gets or sets the <see cref="BeginStoryboard.Name"/> of the <see cref="BeginStoryboard"/> that began the <see cref="Storyboard"/> 
    /// you want to interactively control.
    /// </summary>
    /// <returns>
    /// The <see cref="BeginStoryboard.Name"/> of the <see cref="BeginStoryboard"/> that began the <see cref="Storyboard"/> you want to 
    /// interactively control. The default value is null.
    /// </returns>
    public string BeginStoryboardName
    {
        get { return _beginStoryboardName; }
        set
        {
            if (IsSealed)
            {
                throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, nameof(ControllableStoryboardAction)));
            }

            _beginStoryboardName = value;
        }
    }

    internal sealed override void Invoke(IFrameworkElement fe, INameScope namescope)
    {
        Debug.Assert(fe is not null, "Invoke needs an object as starting point");

        Invoke(fe, GetStoryboard(fe, namescope));
    }

    internal sealed override void Invoke(IFrameworkElement fe)
    {
        Debug.Assert(fe is not null, "Invoke needs an object as starting point");

        Invoke(fe, GetStoryboard(fe, null));
    }

    internal virtual void Invoke(IFrameworkElement containingFE, Storyboard storyboard) { }

    // Find a Storyboard object for this StoryboardAction to act on, using the
    //  given BeginStoryboardName to find a BeginStoryboard instance and use
    //  its Storyboard object reference.
    private Storyboard GetStoryboard(IFrameworkElement fe, INameScope namescope)
    {
        if (BeginStoryboardName is null)
        {
            throw new InvalidOperationException(Strings.Storyboard_BeginStoryboardNameRequired);
        }

        BeginStoryboard keyedBeginStoryboard = ResolveBeginStoryboardName(BeginStoryboardName, fe, namescope);

        Storyboard storyboard = keyedBeginStoryboard.Storyboard ??
            throw new InvalidOperationException(string.Format(Strings.Storyboard_BeginStoryboardNoStoryboard, BeginStoryboardName));

        return storyboard;
    }

    private static BeginStoryboard ResolveBeginStoryboardName(string targetName, IFrameworkElement fe, INameScope nameScope)
    {
        object namedObject;

        if (nameScope is not null)
        {
            namedObject = nameScope.FindName(targetName);
        }
        else if (fe is not null)
        {
            namedObject = fe.FindName(targetName);
        }
        else
        {
            throw new InvalidOperationException(string.Format(Strings.Storyboard_NoNameScope, targetName));
        }

        if (namedObject is null)
        {
            throw new InvalidOperationException(
                string.Format(Strings.Storyboard_NameNotFound, targetName, fe.GetType().ToString()));
        }

        if (namedObject is not BeginStoryboard beginStoryboard)
        {
            throw new InvalidOperationException(string.Format(Strings.Storyboard_BeginStoryboardNameNotFound, targetName));
        }

        return beginStoryboard;
    }
}
