using Mono.Cecil;
using Mono.Cecil.Cil;

namespace DotNetForHtml5.Compiler
{
    internal class MemberReferenceAndCallerInformation
    {
        public MemberReferenceAndCallerInformation(
            MemberReference memberReference,
            SequencePoint callerSequencePoint)
        {
            MemberReference = memberReference;
            CallerSequencePoint = callerSequencePoint;
        }

        /// <summary>
        /// Information about the member, such as a method of a class.
        /// </summary>
        public MemberReference MemberReference { get; private set; }

        /// <summary>
        /// Information about the place where the member is used. For example, if the member is a method, this property contains the information about the location where this method is called.
        /// </summary>
        public SequencePoint CallerSequencePoint { get; private set; }

        /// <summary>
        /// The name of the source code file where the member is used. For example, if the member is a method, this property returns the location where this method is called. Returns an empty string is no information was found.
        /// </summary>
        public string CallerFileNameOrEmpty
        {
            get
            {
                var callerSequencePoint = CallerSequencePoint;
                if (callerSequencePoint != null
                    && callerSequencePoint.Document != null)
                    return callerSequencePoint.Document.Url;
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// The line number in the source code file where the member is used. For example, if the member is a method, this property returns the location where this method is called. Returns "0" if no information was found.
        /// </summary>
        public int CallerLineNumberOrZero
        {
            get
            {
                var callerSequencePoint = CallerSequencePoint;
                if (callerSequencePoint != null)
                    return callerSequencePoint.StartLine;
                else
                    return 0;
            }
        }
    }
}
