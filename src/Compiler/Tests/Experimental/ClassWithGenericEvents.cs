using System;
using System.Collections.Generic;
using System.Text;

namespace Experimental
{
    public class ClassWithGenericEvents<T>
    {
        public event EventHandler<T> DefaultEventWithEventHandler;
    }
}
