
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

using System.Windows.Markup;

namespace System.Windows.Media.Animation;

/// <summary>
/// A trigger action that begins a <see cref="Animation.Storyboard"/> and
/// distributes its animations to their targeted objects and properties.
/// </summary>
[ContentProperty(nameof(Storyboard))]
public sealed class BeginStoryboard : TriggerAction
{
    // Silverlight prevents the name property from being set to null. To work around this limitation,
    // we override the metadata of the name property to replace the coercion callback.
    static BeginStoryboard()
    {
        FrameworkElement.NameProperty.OverrideMetadata(
            typeof(BeginStoryboard),
            new PropertyMetadata(string.Empty, null, CoerceName));
    }

    private static object CoerceName(DependencyObject d, object baseValue) => baseValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="BeginStoryboard"/> class.
    /// </summary>
    public BeginStoryboard() { }

    /// <summary>
    /// Identifies the <see cref="Storyboard"/> dependency property.
    /// </summary>
    public static readonly DependencyProperty StoryboardProperty =
        DependencyProperty.Register(
            nameof(Storyboard),
            typeof(Storyboard),
            typeof(BeginStoryboard),
            null);

    /// <summary>
    /// Gets or sets the <see cref="Animation.Storyboard"/> that this <see cref="BeginStoryboard"/>
    /// starts.
    /// </summary>
    /// <returns>
    /// The <see cref="Animation.Storyboard"/> that the <see cref="BeginStoryboard"/>
    /// starts. The default is null.
    /// </returns>
    public Storyboard Storyboard
    {
        get => (Storyboard)GetValue(StoryboardProperty);
        set => SetValueInternal(StoryboardProperty, value);
    }

    /// <summary>
    /// Gets or sets the name of the <see cref="BeginStoryboard"/> object. By naming the 
    /// <see cref="BeginStoryboard"/> object, the <see cref="Storyboard"/> can be controlled 
    /// after it is started.
    /// </summary>
    /// <returns>
    /// The name of the <see cref="BeginStoryboard"/>. The default is null.
    /// </returns>
    public string Name
    {
        get => (string)GetValue(FrameworkElement.NameProperty);
        set => SetValueInternal(FrameworkElement.NameProperty, value);
    }

    internal override void Invoke(IFrameworkElement fe)
    {
        if (Storyboard is Storyboard storyboard)
        {
            if (fe is FrameworkElement frameworkElement)
            {
                storyboard.Begin(frameworkElement, Name is not null);
            }
            else
            {
                storyboard.Begin();
            }
        }
    }
}
