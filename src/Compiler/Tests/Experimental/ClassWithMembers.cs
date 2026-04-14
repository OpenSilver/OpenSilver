using System;

namespace Experimental
{
    public class BaseClassWithMembers
    {
        public int InheritedPublicInstanceField;

        public int InheritedPublicInstanceProperty { get; set; }

#pragma warning disable CS0067 // Event is never used
        public event EventHandler InheritedPublicInstanceEvent;
#pragma warning restore CS0067

        public void InheritedPublicInstanceMethod() { }
    }

    public class ClassWithMembers : BaseClassWithMembers
    {
        public int PublicInstanceField;
        public static int PublicStaticField;

#pragma warning disable CS0649 // Field is never assigned to
        internal int NonPublicInstanceField;
        internal static int NonPublicStaticField;
#pragma warning restore CS0649

        public int PublicInstanceProperty { get; set; }
        public static int PublicStaticProperty { get; set; }
        internal int NonPublicInstanceProperty { get; set; }
        internal static int NonPublicStaticProperty { get; set; }

#pragma warning disable CS0067 // Event is never used
        public event EventHandler PublicInstanceEvent;
        public static event EventHandler PublicStaticEvent;
        internal event EventHandler NonPublicInstanceEvent;
        internal static event EventHandler NonPublicStaticEvent;
#pragma warning restore CS0067

        public void PublicInstanceMethod() { }
        public static void PublicStaticMethod() { }
        internal void NonPublicInstanceMethod() { }
        internal static void NonPublicStaticMethod() { }
    }
}
