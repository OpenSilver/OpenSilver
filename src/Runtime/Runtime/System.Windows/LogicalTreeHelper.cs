
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

using System.Collections;
using System.Diagnostics;
using OpenSilver.Internal;
using OpenSilver.Internal.Controls;

namespace System.Windows;

/// <summary>
/// Provides static helper methods for querying objects in the logical tree.
/// </summary>
public static class LogicalTreeHelper
{
    /// <summary>
    /// Attempts to find and return an object that has the specified name. The search
    /// starts from the specified object and continues into subnodes of the logical tree.
    /// </summary>
    /// <param name="logicalTreeNode">
    /// The object to start searching from.
    /// </param>
    /// <param name="elementName">
    /// The name of the object to find.
    /// </param>
    /// <returns>
    /// The object with the matching name, if one is found; returns null if no matching
    /// name was found in the logical tree.
    /// </returns>
    public static DependencyObject FindLogicalNode(DependencyObject logicalTreeNode, string elementName)
    {
        if (logicalTreeNode is null)
        {
            throw new ArgumentNullException(nameof(logicalTreeNode));
        }

        if (elementName is null)
        {
            throw new ArgumentNullException(nameof(elementName));
        }

        if (elementName.Length == 0)
        {
            throw new ArgumentException(Strings.StringEmpty, nameof(elementName));
        }

        DependencyObject namedElement = null;
        DependencyObject childNode;

        // Check given node against named element.
        if (logicalTreeNode is IInternalFrameworkElement selfNode)
        {
            if (selfNode.Name == elementName)
            {
                namedElement = logicalTreeNode;
            }
        }

        if (namedElement is null)
        {
            // Nope, the given node isn't it.  See if we can check children.

            // If we can enumerate, check the children.
            if (GetLogicalChildren(logicalTreeNode) is IEnumerator childEnumerator)
            {
                childEnumerator.Reset();

                while (namedElement is null && childEnumerator.MoveNext())
                {
                    childNode = childEnumerator.Current as DependencyObject;

                    if (childNode is not null)
                    {
                        namedElement = FindLogicalNode(childNode, elementName);
                    }
                }
            }
        }

        // Return what we can find - may be null.
        return namedElement;
    }

    /// <summary>
    /// Returns the collection of immediate child objects of the specified object, by
    /// processing the logical tree.
    /// </summary>
    /// <param name="current">
    /// The object from which to start processing the logical tree.
    /// </param>
    /// <returns>
    /// The enumerable collection of immediate child objects from the logical tree of
    /// the specified object.
    /// </returns>
    public static IEnumerable GetChildren(DependencyObject current)
    {
        if (current is null)
        {
            throw new ArgumentNullException(nameof(current));
        }

        if (current is IInternalFrameworkElement fe)
        {
            return EnumeratorWrapper.Create(fe.LogicalChildren);
        }

        return EnumeratorWrapper.Empty;
    }

    /// <summary>
    /// Returns the collection of immediate child objects of the specified <see cref="FrameworkElement"/>
    /// by processing the logical tree.
    /// </summary>
    /// <param name="current">
    /// The object from which to start processing the logical tree.
    /// </param>
    /// <returns>
    /// The enumerable collection of immediate child objects starting from current in the logical tree.
    /// </returns>
    public static IEnumerable GetChildren(FrameworkElement current)
    {
        if (current is null)
        {
            throw new ArgumentNullException(nameof(current));
        }

        return EnumeratorWrapper.Create(current.LogicalChildren);
    }

    /// <summary>
    /// Returns the parent object of the specified object by processing the logical tree.
    /// </summary>
    /// <param name="current">
    /// The object to find the parent object for.
    /// </param>
    /// <returns>
    /// The requested parent object.
    /// </returns>
    public static DependencyObject GetParent(DependencyObject current)
    {
        if (current is null)
        {
            throw new ArgumentNullException(nameof(current));
        }

        if (current is IInternalFrameworkElement fe)
        {
            return fe.Parent;
        }

        return null;
    }

    /// <summary>
    /// Returns the parent object of the specified object by processing the logical tree.
    /// </summary>
    /// <param name="current">
    /// The object to find the parent object for.
    /// </param>
    /// <returns>
    /// The requested parent object.
    /// </returns>
    public static DependencyObject GetParent(FrameworkElement current)
    {
        if (current is null)
        {
            throw new ArgumentNullException(nameof(current));
        }

        return current.Parent;
    }

    private static IEnumerator GetLogicalChildren(DependencyObject current)
    {
        if (current is IInternalFrameworkElement fe)
        {
            return fe.LogicalChildren;
        }

        return EmptyEnumerator.Instance;
    }

    private class EnumeratorWrapper : IEnumerable
    {
        private readonly IEnumerator _enumerator;

        internal static EnumeratorWrapper Create(IEnumerator enumerator)
        {
            if (enumerator is null)
            {
                return Empty;
            }
            else
            {
                return new EnumeratorWrapper(enumerator);
            }
        }

        internal static EnumeratorWrapper Empty { get; } = new EnumeratorWrapper(EmptyEnumerator.Instance);

        private EnumeratorWrapper(IEnumerator enumerator)
        {
            Debug.Assert(enumerator is not null);
            _enumerator = enumerator;
        }

        IEnumerator IEnumerable.GetEnumerator() => _enumerator;
    }
}
