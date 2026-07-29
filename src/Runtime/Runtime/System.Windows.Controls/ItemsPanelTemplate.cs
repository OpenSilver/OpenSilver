
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

namespace System.Windows.Controls;

/// <summary>
/// Specifies the panel that the <see cref="ItemsPresenter"/> creates for the layout of 
/// the items of an <see cref="ItemsControl"/>.
/// </summary>
public class ItemsPanelTemplate : FrameworkTemplate
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ItemsPanelTemplate"/> class.
    /// </summary>
    public ItemsPanelTemplate() { }

    internal override Type TargetTypeInternal => typeof(ItemsPresenter);

    /// <summary>
    /// Checks that the templated parent is a non-null <see cref="ItemsPresenter"/> object.
    /// </summary>
    /// <param name="templatedParent">
    /// The element this template is applied to. This must be an <see cref="ItemsPresenter"/> object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="templatedParent"/> is null.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="templatedParent"/> is not an <see cref="ItemsPresenter"/>.
    /// </exception>
    protected override void ValidateTemplatedParent(FrameworkElement templatedParent)
    {
        // Must have a non-null feTemplatedParent
        ArgumentNullException.ThrowIfNull(templatedParent);

        // A ItemsPanelTemplate must be applied to an ItemsPresenter
        if (templatedParent is not ItemsPresenter)
        {
            throw new ArgumentException(
                string.Format(Strings.TemplateTargetTypeMismatch, nameof(ItemsPresenter), templatedParent.GetType().Name));
        }
    }
}
