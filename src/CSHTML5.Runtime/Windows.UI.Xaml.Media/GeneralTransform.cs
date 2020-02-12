
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



using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if !MIGRATION
using Windows.Foundation;
#endif

#if MIGRATION
namespace System.Windows.Media
#else
namespace Windows.UI.Xaml.Media
#endif
{
    /// <summary>
    /// Provides generalized transformation support for objects, such as points and
    /// rectangles.
    /// </summary>
    public abstract partial class GeneralTransform : DependencyObject
    {
        internal UIElement INTERNAL_parent;


        /// <summary>
        /// Provides base class initialization behavior for GeneralTransform-derived
        /// classes.
        /// </summary>
        protected GeneralTransform()
        {

        }

        // Must be implemented by the concrete class:
        protected abstract Point INTERNAL_TransformPoint(Point point);

        //
        // Summary:
        //     Transforms the specified point and returns the result.
        //
        // Parameters:
        //   point:
        //     The point to transform.  
        //
        // Returns:
        //     The result of transforming point.
        /// <summary>
        /// Transforms the specified point and returns the result.
        /// </summary>
        /// <param name="point">The point to transform.</param>
        /// <returns>The result of transforming point.</returns>
        public Point TransformPoint(Point point)
        {
            return INTERNAL_TransformPoint(point);
        }

        //---- FOR SILVERLIGHT BACKWARD COMPATIBILITY: ----
        /// <summary>
        /// Transforms the specified point and returns the result.
        /// </summary>
        /// <param name="point">The point to transform.</param>
        /// <returns>The result of transforming point.</returns>
        public Point Transform(Point point)
        {
            // (For Silverlight backward compatibility only)

            return TransformPoint(point);
        }


        // Returns:
        //     An inverse of this instance, if possible; otherwise null.
        //// <summary>
        //// Gets the inverse transformation of this GeneralTransform, if possible.
        //// </summary>
        //public GeneralTransform Inverse { get; } 


        // Returns:
        //     The value that should be returned as Inverse by the GeneralTransform.
        //// <summary>
        //// Implements the behavior for return value of Inverse in a derived or custom
        //// GeneralTransform.
        //// </summary>
        //protected virtual GeneralTransform InverseCore { get; }



        // Summary:
        //     Transforms the specified bounding box and returns an axis-aligned bounding
        //     box that is exactly large enough to contain it.
        //
        // Parameters:
        //   rect:
        //     The bounding box to transform.
        //
        // Returns:
        //     The smallest axis-aligned bounding box possible that contains the transformed
        //     rect.
        //public Rect TransformBounds(Rect rect);




        //
        // Summary:
        //     Provides the means to override the TransformBounds behavior in a derived
        //     transform class.
        //
        // Parameters:
        //   rect:
        //     The bounding box to transform.
        //
        // Returns:
        //     The smallest axis-aligned bounding box possible that contains the transformed
        //     rect.
        //protected virtual Rect TransformBoundsCore(Rect rect);




        //
        // Summary:
        //     Attempts to transform the specified point and returns a value that indicates
        //     whether the transformation was successful.
        //
        // Parameters:
        //   inPoint:
        //     The point to transform.
        //
        //   outPoint:
        //     The result of transforming inPoint.
        //
        // Returns:
        //     True if inPoint was transformed; otherwise, false.
        //public bool TryTransform(Point inPoint, out Point outPoint);



        //
        // Summary:
        //     Provides the means to override the TryTransform behavior in a derived transform
        //     class.
        //
        // Parameters:
        //   inPoint:
        //     The point to transform.
        //
        //   outPoint:
        //     The result of transforming inPoint.
        //
        // Returns:
        //     True if inPoint was transformed; otherwise, false.
        //protected virtual bool TryTransformCore(Point inPoint, out Point outPoint);

#if WORKINPROGRESS
        public abstract GeneralTransform Inverse { get; }

        public abstract Rect TransformBounds(Rect rect);

        public abstract bool TryTransform(Point inPoint, out Point outPoint);
#endif
    }
}
