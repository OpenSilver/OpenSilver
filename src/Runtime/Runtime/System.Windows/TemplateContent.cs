
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

namespace System.Windows;

/// <summary>
/// Provides the base implementation for template content used by a <see cref="FrameworkTemplate"/>.
/// </summary>
public abstract class TemplateContent : ITemplateContent
{
    private FrameworkTemplate _ownerTemplate;

    /// <summary>
    /// Initializes a new instance of the <see cref="TemplateContent"/> class.
    /// </summary>
    protected TemplateContent() { }

    /// <summary>
    /// Gets a value that indicates whether the <see cref="TemplateContent"/> is read-only and cannot be changed.
    /// </summary>
    /// <returns>
    /// true if the <see cref="TemplateContent"/> is sealed; otherwise, false.
    /// </returns>
    public bool IsSealed { get; private set; }

    internal FrameworkTemplate OwnerTemplate
    {
        get => _ownerTemplate;
        set
        {
            CheckSealed();
            ArgumentNullException.ThrowIfNull(value);
            _ownerTemplate = value;
        }
    }

    /// <summary>
    /// Creates an instance of the template's visual tree for the specified owner.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the templated owner.
    /// </typeparam>
    /// <param name="owner">
    /// The element to which the template is being applied.
    /// </param>
    /// <returns>
    /// The root <see cref="IFrameworkElement"/> generated from the template content.
    /// </returns>
    public abstract IFrameworkElement LoadContent<T>(T owner) where T : DependencyObject, IFrameworkElement;

    /// <summary>
    /// Invoked after this <see cref="TemplateContent"/> becomes sealed.
    /// </summary>
    protected virtual void OnSealed() { }

    /// <summary>
    /// Throws an exception if the instance has been sealed.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the template content has been sealed and a modification is attempted.
    /// </exception>
    protected void CheckSealed()
    {
        if (IsSealed)
        {
            throw new InvalidOperationException(string.Format(Strings.CannotChangeAfterSealed, GetType().Name));
        }
    }

    internal void Seal()
    {
        if (IsSealed)
        {
            return;
        }

        IsSealed = true;
        OnSealed();
    }
}
