using Mono.Cecil;
using Mono.Cecil.Cil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DotNetForHtml5.Compiler
{
    public class CheckingThatNoUnsupportedMethodIsCalled
    {
        bool _initialized = false;
        Dictionary<string, HashSet<string>> _supportedMscorlibMethods = new Dictionary<string, HashSet<string>>();
        Dictionary<string, Dictionary<string, string>> _unsupportedMscorlibMethodsWithExplanation = new Dictionary<string, Dictionary<string, string>>();
        const string ExplanationWhenUnsupported = "The method \"{0}\" is not yet supported. You can see the list of supported methods at: http://www.cshtml5.com/links/what-is-supported.aspx  - You can learn how to implement missing methods at: http://www.cshtml5.com/mscorlib.aspx - For assistance, please send an email to: support@cshtml5.com";
        const string ErrorLocationString = "  (Error location: \"{0}\" in assembly \"{1}\")";

        public static List<Tuple<string, List<string>>> GetSupportedMethods()
        {
#if !STATICALLY_DEFINED_LIST_OF_SUPPORTED_TYPES

            List<Tuple<string, List<string>>> result = null;

            // Load the XML file located in the Compiler directory:
            XDocument xdoc;
            try
            {
                var xmlFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "SupportedElements.xml");

                if (!File.Exists(xmlFilePath)) // We also look in the parent folder because we may be in a subfolder of the Compiler folder (such as the "SLMigration" folder).
                    xmlFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\SupportedElements.xml");

                xdoc = XDocument.Load(xmlFilePath);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load the file that contains the list of supported methods (SupportedElements.xml). Please contact support at support@cshtml5.com" + Environment.NewLine + Environment.NewLine + ex.ToString());
            }

            // Query the document:
            try
            {
                result = (from type in xdoc.Root.Descendants("Type")
                          select new Tuple<string, List<string>>(
                              type.Attribute("Name").Value,
                              (from member in type.Elements("Member")
                               select member.Attribute("Name").Value).ToList())).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Error while parsing the file that contains the list of supported methods. Please contact support at support@cshtml5.com" + Environment.NewLine + Environment.NewLine + ex.ToString());
            }

#else
            var result = new List<Tuple<string, List<string>>>()
            {
                //---------
                // IMPORTANT NOTE: The types defined below must never contain the "+" sign of nested types: only the local type name must be used, otherwise the code in "translator.MemberCanBeSkipped += ..." does not function properly.
                //---------

                //todo: add the namespace in addition to the name of the types.

                // METHODS NOT TESTED:
                new Tuple<string, List<string>>("Activator", new List<string>() { "CreateInstance" }),
                new Tuple<string, List<string>>("Array", new List<string>() { "Copy", "Resize", "Sort" }),
                new Tuple<string, List<string>>("ArrayList", new List<string>() { ".ctor", "Add", "AddRange", "AsReadOnly", "BinarySearch", "Clear", "ClearItems", "Contains", "CopyTo", "Exists", "ForEach", "Find", "FindAll", "FindIndex", "get_Capacity", "get_Count", "get_Item", "GetEnumerator", "Insert", "InsertItem", "IndexOf", "InsertRange", "Reverse", "Remove", "RemoveAll", "RemoveAt", "RemoveItem", "RemoveRange", "set_Item", "SetItem", "Sort", "ToArray", "TrueForAll" }),
                new Tuple<string, List<string>>("ASCIIEncoding", new List<string>() {  }),
                new Tuple<string, List<string>>("AssemblyName", new List<string>() { ".ctor", "get_Flags", "get_FullName", "get_Name", "get_Version", "GetAssemblyName", "set_Flags", "set_Name", "set_Version", "toString" }),
                new Tuple<string, List<string>>("AsyncTaskMethodBuilder", new List<string>() { ".ctor", "AwaitOnCompleted", "AwaitUnsafeOnCompleted", "Create", "get_Task", "SetException", "SetResult", "Start" }),
                new Tuple<string, List<string>>("AsyncTaskMethodBuilder`1", new List<string>() { ".ctor", "AwaitOnCompleted", "AwaitUnsafeOnCompleted", "Create", "get_Task", "SetException", "SetResult", "Start" }),
                new Tuple<string, List<string>>("AsyncVoidMethodBuilder", new List<string>() { ".ctor", "AwaitOnCompleted", "AwaitUnsafeOnCompleted", "Create", "SetException", "SetResult", "Start" }),
                new Tuple<string, List<string>>("BinaryReader", new List<string>() { ".ctor", "Close", "Dispose", "get_BaseStream", "PeekChar", "Read", "Read7BitEncodedInt", "ReadBoolean", "ReadByte", "ReadBytes", "ReadChar", "ReadChars", "ReadDouble", "ReadInt16", "ReadInt32", "ReadInt64", "ReadSByte", "ReadSingle", "ReadString", "ReadUInt16", "ReadUInt32", "ReadUInt64" }),
                new Tuple<string, List<string>>("BinaryWriter", new List<string>() { ".ctor", "Dispose", "Flush", "get_BaseStream", "Seek", "Write", "Write7BitEncodedInt" }),
                new Tuple<string, List<string>>("BitConverter", new List<string>() { "GetBytes", "ToBoolean", "ToDouble", "ToInt16", "ToInt32", "ToInt64", "ToSingle", "ToUInt16", "ToUInt32", "ToUInt64" }),
                new Tuple<string, List<string>>("Buffer", new List<string>() { "BlockCopy", "ByteLength", "GetByte" }),
                new Tuple<string, List<string>>("Byte", new List<string>() {  }),
                //new Tuple<string, List<string>>("Capture", new List<string>() { "get_Length", "get_Value", "toString" }),
                new Tuple<string, List<string>>("Char", new List<string>() { "IsControl", "IsDigit", "IsLetter", "IsLetterOrDigit", "IsSurrogate", "IsWhiteSpace" }),
                new Tuple<string, List<string>>("Collection`1", new List<string>() { ".ctor", "Add", "Clear", "ClearItems", "Contains", "CopyTo", "GetEnumerator", "get_Count", "get_Item", "IndexOf", "Insert", "InsertItem", "Remove", "RemoveAt", "RemoveItem", "SetItem" }),
                new Tuple<string, List<string>>("Comparer`1", new List<string>() { "Compare", "get_Default" }),
                new Tuple<string, List<string>>("Comparison`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Console", new List<string>() {  }),
                new Tuple<string, List<string>>("ConstantExpression", new List<string>() { ".ctor", "Make" }),
                new Tuple<string, List<string>>("ConstructorInfo", new List<string>() { "GetParameters", "Invoke" }),
                new Tuple<string, List<string>>("Convert", new List<string>() { "ChangeType", "FromBase64String", "ToBase64String" }),
                new Tuple<string, List<string>>("CultureInfo", new List<string>() { ".ctor", "Clone", "get_CurrentCulture", "get_CurrentUICulture", "get_InvariantCulture", "get_Name", "get_TwoLetterISOLanguageName", "get_UseUserOverride", "GetCultureByName", "GetCultureInfo", "GetCultureInfoByIetfLanguageTag" }),
                new Tuple<string, List<string>>("DateTime", new List<string>() { ".ctor", "AddDays", "AddHours", "AddMilliseconds", "AddMinutes", "AddSeconds", "get_Date", "get_Day", "get_DayOfWeek", "get_Hour", "get_Kind", "get_Minute", "get_Month", "get_Now", "get_Second", "get_TimeOfDay", "get_UtcNow", "get_Year", "op_Equality", "op_GreaterThan", "op_GreaterThanOrEqual", "op_Inequality", "op_LessThan", "op_LessThanOrEqual", "op_Subtraction", "ToLocalTime", "ToLongTimeString", "ToShortDateString", "ToShortTimeString", "ToUniversalTime" }),
                new Tuple<string, List<string>>("Debug", new List<string>() { "Assert", "Write", "WriteLine" }),
                new Tuple<string, List<string>>("Directory", new List<string>() { "CreateDirectory", "Exists", "GetDirectories", "GetFiles" }),
                new Tuple<string, List<string>>("DirectoryInfo", new List<string>() { ".ctor", "Create", "toString" }),
                new Tuple<string, List<string>>("Double", new List<string>() {  }),
                new Tuple<string, List<string>>("Encoding", new List<string>() { "get_ASCII", "get_BigEndianUnicode", "get_Unicode", "get_UTF7", "get_UTF8", "GetByteCount", "GetBytes", "GetCharCount", "GetChars", "GetString" }),
                new Tuple<string, List<string>>("Enumerable", new List<string>() { "Any", "AsEnumerable", "Cast", "Contains", "Count", "ElementAt", "ElementAtOrDefault", "Empty", "First", "FirstOrDefault", "OfType", "OrderBy", "Range", "Select", "SelectMany", "Sum", "ToArray", "ToList", "Where" }),
                new Tuple<string, List<string>>("Environment", new List<string>() { "get_CurrentManagedThreadId", "get_NewLine", "get_TickCount", "GetFolderPath" }),
                new Tuple<string, List<string>>("EventArgs", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("EventInfo", new List<string>() { "AddEventHandler", "get_EventType", "GetAddMethod", "GetRemoveMethod", "RemoveEventHandler", "toString" }),
                new Tuple<string, List<string>>("Exception", new List<string>() { ".ctor", "get_InnerException", "get_Message", "get_StackTrace", "toString" }),
                new Tuple<string, List<string>>("Expression", new List<string>() { "Constant", "Equal", "Lambda", "Parameter" }),
                new Tuple<string, List<string>>("Expression`1", new List<string>() {  }),
                new Tuple<string, List<string>>("FieldInfo", new List<string>() { "get_FieldType", "get_IsInitOnly", "get_IsLiteral", "GetRawConstantValue", "GetValue", "op_Equality", "op_Inequality", "SetValue" }),
                new Tuple<string, List<string>>("File", new List<string>() { "AppendText", "Copy", "Create", "CreateText", "Delete", "Exists", "Open", "OpenRead", "OpenWrite", "ReadAllBytes", "ReadAllText" }),
                new Tuple<string, List<string>>("FileInfo", new List<string>() { ".ctor", "Create", "Delete", "Open", "OpenRead", "OpenText", "OpenWrite", "toString" }),
                new Tuple<string, List<string>>("FileNotFoundException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("FileStream", new List<string>() { ".ctor", "get_Name" }),
                new Tuple<string, List<string>>("FileSystemInfo", new List<string>() { ".ctor", "Delete", "get_Exists", "get_Extension", "get_FullName", "get_Name", "Refresh" }),
                new Tuple<string, List<string>>("FormatException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("GC", new List<string>() { "GetTotalMemory", "IsServerGC" }),
                new Tuple<string, List<string>>("GCHandle", new List<string>() { "AddrOfPinnedObject", "Alloc", "Free" }),
                new Tuple<string, List<string>>("Group", new List<string>() { "get_Success" }),
                new Tuple<string, List<string>>("GroupCollection", new List<string>() { "get_Count", "GetEnumerator" }),
               // new Tuple<string, List<string>>("HashSet`1", new List<string>() { ".ctor", "Add", "Clear", "Contains", "get_Count", "GetEnumerator", "Remove" }),
                new Tuple<string, List<string>>("Hashtable", new List<string>() { ".ctor", "Add", "set_Item" }),
                new Tuple<string, List<string>>("ICollection", new List<string>() { "get_Count" }),
                new Tuple<string, List<string>>("IEqualityComparer`1", new List<string>() { "Equals" }),
                new Tuple<string, List<string>>("IEquatable`1", new List<string>() { "Equals" }),
                new Tuple<string, List<string>>("Int16", new List<string>() {  }),
                new Tuple<string, List<string>>("Int64", new List<string>() { "FromInt32", "FromNumber", "op_Division", "op_GreaterThan", "op_GreaterThanOrEqual", "op_LessThan", "op_LessThanOrEqual", "op_Modulus", "op_RightShift", "op_UnaryNegation", "ToInt32", "ToNumber", "toString", "ToUInt64" }),
                new Tuple<string, List<string>>("IntPtr", new List<string>() { ".ctor", "op_Addition", "op_Equality", "op_Inequality", "ToInt32", "ToInt64" }),
                new Tuple<string, List<string>>("InvalidCastException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("InvalidOperationException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("IsolatedStorageFile", new List<string>() { ".ctor", "CreateFile", "FileExists", "GetUserStoreForAssembly", "OpenFile" }),
                new Tuple<string, List<string>>("LinkedList`1", new List<string>() { ".ctor", "AddAfter", "AddBefore", "AddFirst", "AddLast", "Clear", "get_Count", "get_First", "get_Last", "Remove", "RemoveFirst", "RemoveLast" }),
                new Tuple<string, List<string>>("LinkedListNode`1", new List<string>() { ".ctor", "get_List", "get_Next", "get_Previous", "get_Value", "set_Value" }),
                //new Tuple<string, List<string>>("ManualResetEventSlim", new List<string>() { ".ctor", "Set", "Wait" }),
                new Tuple<string, List<string>>("Marshal", new List<string>() { "OffsetOf", "SizeOf" }),
                new Tuple<string, List<string>>("Match", new List<string>() { "get_Groups" }),
                new Tuple<string, List<string>>("MatchCollection", new List<string>() { "get_Count", "get_Item", "GetEnumerator", "GetMatch" }),
                new Tuple<string, List<string>>("Math", new List<string>() { "Atan2", "IEEERemainder", "Round", "Sign" }),
                new Tuple<string, List<string>>("MemoryStream", new List<string>() { ".ctor", "GetBuffer", "ToArray" }),
                new Tuple<string, List<string>>("MethodBase", new List<string>() { "get_ContainsGenericParameters", "get_IsGenericMethod", "get_IsGenericMethodDefinition", "GetParameters", "GetParameterTypes", "Invoke", "op_Equality", "op_Inequality", "toString" }),
                new Tuple<string, List<string>>("MethodInfo", new List<string>() { "get_ContainsGenericParameters", "get_IsGenericMethod", "get_IsGenericMethodDefinition", "get_ReturnType", "GetParameters", "Invoke", "MakeGenericMethod", "op_Equality", "op_Inequality" }),
                new Tuple<string, List<string>>("Monitor", new List<string>() { "Enter", "Exit" }),
                new Tuple<string, List<string>>("MulticastDelegate", new List<string>() { "GetInvocationList" }),
                new Tuple<string, List<string>>("Nullable", new List<string>() { "GetUnderlyingType" }),
                new Tuple<string, List<string>>("Object", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("ParameterExpression", new List<string>() { "get_IsByRef", "get_Type", "Make" }),
                new Tuple<string, List<string>>("ParameterInfo", new List<string>() { "get_Name", "get_ParameterType", "get_Position", "GetCustomAttributes", "toString" }),
                new Tuple<string, List<string>>("Path", new List<string>() { "Combine", "GetDirectoryName", "GetExtension", "GetFileName", "GetFileNameWithoutExtension", "GetFullPath", "GetInvalidPathChars", "GetInvalidFileNameChars", "IsPathRooted" }),
                new Tuple<string, List<string>>("PropertyChangedEventArgs", new List<string>() { ".ctor", "get_PropertyName" }),
                new Tuple<string, List<string>>("PropertyInfo", new List<string>() { "get_CanRead", "get_CanWrite", "get_PropertyType", "GetAccessors", "GetGetMethod", "GetIndexParameters", "GetSetMethod" }),
                new Tuple<string, List<string>>("Queue`1", new List<string>() { ".ctor", "Clear", "Dequeue", "Enqueue", "get_Count", "GetEnumerator" }),
                new Tuple<string, List<string>>("ReadOnlyCollection`1", new List<string>() {  }),
                new Tuple<string, List<string>>("Regex", new List<string>() { ".ctor", "IsMatch", "Matches", "Replace" }),
                new Tuple<string, List<string>>("ResourceManager", new List<string>() { ".ctor", "GetObject", "GetResourceSet", "GetStream", "GetString" }),
                new Tuple<string, List<string>>("ResourceSet", new List<string>() { "Close", "Dispose", "GetObject", "GetString" }),
                new Tuple<string, List<string>>("SByte", new List<string>() {  }),
                new Tuple<string, List<string>>("Single", new List<string>() {  }),
                new Tuple<string, List<string>>("Stack`1", new List<string>() { ".ctor", "Clear", "get_Count", "GetEnumerator", "Peek", "Pop", "Push" }),
                new Tuple<string, List<string>>("StackFrame", new List<string>() { ".ctor", "GetMethod" }),
                new Tuple<string, List<string>>("StackTrace", new List<string>() { ".ctor", "CaptureStackTrace", "GetFrame" }),
                new Tuple<string, List<string>>("Stopwatch", new List<string>() { ".ctor", "get_Elapsed", "get_ElapsedMilliseconds", "get_ElapsedTicks", "get_IsRunning", "Reset", "Restart", "Start", "StartNew", "Stop" }),
                new Tuple<string, List<string>>("Stream", new List<string>() { "$PeekByte", "Close", "CopyTo", "Dispose", "get_CanRead", "get_CanSeek", "get_Length", "get_Position", "Read", "ReadByte", "Seek", "set_Position", "Write", "WriteByte" }),
                new Tuple<string, List<string>>("StreamReader", new List<string>() { ".ctor", "Dispose", "get_EndOfStream", "ReadLine", "ReadToEnd" }),
                new Tuple<string, List<string>>("StringBuilder", new List<string>() { ".ctor", "Append", "AppendFormat", "AppendLine", "Clear", "get_Chars", "get_Length", "Replace", "set_Length", "toString" }),
                new Tuple<string, List<string>>("StringReader", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("StringWriter", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("SystemException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Task", new List<string>() { ".ctor", "ContinueWith", "Delay", "get_Exception", "get_Factory", "get_IsCompleted", "get_Status", "GetAwaiter" }),
                new Tuple<string, List<string>>("Task`1", new List<string>() { ".ctor", "ContinueWith", "get_Factory", "get_Result", "GetAwaiter" }),
                new Tuple<string, List<string>>("TaskAwaiter", new List<string>() { ".ctor", "get_IsCompleted", "GetResult", "OnCompleted" }),
                new Tuple<string, List<string>>("TaskAwaiter`1", new List<string>() { ".ctor", "get_IsCompleted", "GetResult", "OnCompleted" }),
                new Tuple<string, List<string>>("TaskCompletionSource`1", new List<string>() { ".ctor", "get_Task", "SetResult", "TrySetCanceled", "TrySetException", "TrySetResult" }),
                new Tuple<string, List<string>>("TaskFactory", new List<string>() { "StartNew" }),
                new Tuple<string, List<string>>("TaskFactory`1", new List<string>() { "StartNew" }),
                new Tuple<string, List<string>>("TextReader", new List<string>() { "Dispose", "ReadToEnd" }), // Note: JSIL does not implement "TextReader.ReadToEnd" but we need to have it here because "StreamReader.ReadToEnd" is implemented, and Mono.Cecil reports it as "TextReader.ReadToEnd".
                new Tuple<string, List<string>>("Thread", new List<string>() { ".cctor2", "get_CurrentCulture", "get_CurrentThread", "get_CurrentUICulture", "get_ManagedThreadId" }),
                new Tuple<string, List<string>>("TimeSpan", new List<string>() { ".ctor", "FromDays", "FromHours", "FromMilliseconds", "FromMinutes", "FromSeconds", "FromTicks", "get_Days", "get_Hours", "get_Milliseconds", "get_Minutes", "get_Seconds", "get_Ticks", "get_TotalDays", "get_TotalHours", "get_TotalMilliseconds", "get_TotalMinutes", "get_TotalSeconds", "op_Addition", "op_Equality", "op_GreaterThan", "op_GreaterThanOrEqual", "op_Inequality", "op_LessThan", "op_LessThanOrEqual", "op_Subtraction", "op_UnaryNegation", "Parse", "toString" }),
                new Tuple<string, List<string>>("Trace", new List<string>() { "TraceError", "TraceInformation", "TraceWarning", "WriteLine" }),
                new Tuple<string, List<string>>("Type", new List<string>() { "get_Assembly", "get_AssemblyQualifiedName", "get_BaseType", "get_ContainsGenericParameters", "get_DeclaringType", "get_FullName", "get_GenericTypeArguments", "get_IsArray", "get_IsByRef", "get_IsEnum", "get_IsGenericParameter", "get_IsGenericType", "get_IsGenericTypeDefinition", "get_IsInterface", "get_IsPublic", "get_IsSpecialName", "get_IsStatic", "get_IsValueType", "get_Name", "get_Namespace", "GetConstructor", "GetConstructors", "GetCustomAttributes", "GetElementType", "GetEvents", "GetField", "GetFields", "GetGenericArguments", "GetGenericTypeDefinition", "GetInterfaces", "GetMember", "GetMembers", "GetMethod", "GetMethods", "GetProperties", "GetProperty", "IsAssignableFrom", "IsSubclassOf", "MakeGenericType", "op_Equality", "op_Inequality", "toString" }),
                new Tuple<string, List<string>>("UInt16", new List<string>() {  }),
                new Tuple<string, List<string>>("UInt32", new List<string>() {  }),
                new Tuple<string, List<string>>("UInt64", new List<string>() { "Clone", "FromInt32", "FromNumber", "Object.Equals", "op_Division", "op_GreaterThan", "op_GreaterThanOrEqual", "op_LessThan", "op_LessThanOrEqual", "op_Modulus", "op_RightShift", "ToHex", "ToInt64", "ToNumber", "toString", "ToUInt32" }), //todo: verify that "Object.Equals" is correct
                new Tuple<string, List<string>>("UIntPtr", new List<string>() { ".ctor", "ToUInt32", "ToUInt64" }),
                new Tuple<string, List<string>>("UnicodeEncoding", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("UTF8Encoding", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("WeakReference", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("XmlConvert", new List<string>() { "ToBoolean", "ToByte", "ToDouble", "ToInt16", "ToInt32", "ToInt64", "ToSingle", "ToUInt16", "ToUInt32", "ToUInt64" }),
                new Tuple<string, List<string>>("XmlNameTable", new List<string>() { ".ctor", "Add", "Get" }),
                new Tuple<string, List<string>>("XmlQualifiedName", new List<string>() { ".ctor", "get_Name", "get_Namespace", "op_Equality", "op_Inequality" }),
                new Tuple<string, List<string>>("XmlReader", new List<string>() { "Create", "Dispose", "get_AdvanceCount", "get_AttributeCount", "get_IsEmptyElement", "get_Item", "get_LocalName", "get_Name", "get_NamespaceURI", "get_NameTable", "get_NodeType", "get_Value", "GetAttribute", "IsStartElement", "MoveToContent", "MoveToElement", "MoveToFirstAttribute", "MoveToNextAttribute", "Read", "ReadContentAsString", "ReadElementContentAsString", "ReadElementString", "ReadEndElement", "ReadStartElement", "ReadString", "ReadToFollowing", "ReadToNextSibling", "Skip" }),
                new Tuple<string, List<string>>("XmlSerializationReader", new List<string>() { "CheckReaderCount", "get_Reader", "get_ReaderCount", "GetXsiType", "Init", "ReadEndElement", "ReadNull", "ReadSerializable" }),
                new Tuple<string, List<string>>("XmlSerializer", new List<string>() { ".ctor", "Deserialize", "Serialize" }),
                new Tuple<string, List<string>>("XmlWriter", new List<string>() { "Close", "Create", "Dispose", "Flush", "WriteAttributeString", "WriteElementString", "WriteEndDocument", "WriteEndElement", "WriteStartDocument", "WriteStartElement", "WriteString" }),

                // "RAWMETHODS" NOT TESTED:
                new Tuple<string, List<string>>("Array", new List<string>() { "CheckType", "Of" }),
                new Tuple<string, List<string>>("AsyncTaskMethodBuilder", new List<string>() { "get_TaskSource" }),
                new Tuple<string, List<string>>("AsyncTaskMethodBuilder`1", new List<string>() { "get_TaskSource" }),
                new Tuple<string, List<string>>("Boolean", new List<string>() { "CheckType" }),
                new Tuple<string, List<string>>("Byte", new List<string>() { "CheckType" }),
                new Tuple<string, List<string>>("Char", new List<string>() { "CheckType" }),
                new Tuple<string, List<string>>("Console", new List<string>() { "Write", "WriteLine" }),
                new Tuple<string, List<string>>("DateTime", new List<string>() { }),
                new Tuple<string, List<string>>("Delegate", new List<string>() { "Invoke" }),
                new Tuple<string, List<string>>("Double", new List<string>() { "CheckType" }),
                new Tuple<string, List<string>>("Enum", new List<string>() { "CheckType" }),
                new Tuple<string, List<string>>("Enumerator", new List<string>() { }),
                new Tuple<string, List<string>>("Int16", new List<string>() { "CheckType" }),
                new Tuple<string, List<string>>("Int32", new List<string>() { "CheckType" }),
                new Tuple<string, List<string>>("Math", new List<string>() { "Exp", "Max", "Min" }),
                new Tuple<string, List<string>>("MethodBase", new List<string>() { "InitResolvedSignature" }),
                new Tuple<string, List<string>>("MulticastDelegate", new List<string>() { "Invoke" }),
                new Tuple<string, List<string>>("Nullable`1", new List<string>() {".ctor", "CheckType", "get_Value" }),
                new Tuple<string, List<string>>("Object", new List<string>() { "__Initialize__", "CheckType" }),
                new Tuple<string, List<string>>("SByte", new List<string>() { "CheckType" }),
                new Tuple<string, List<string>>("Single", new List<string>() { "CheckType" }),
                new Tuple<string, List<string>>("String", new List<string>() { ".cctor2", "CheckType" }),
                new Tuple<string, List<string>>("UInt16", new List<string>() { "CheckType" }),
                new Tuple<string, List<string>>("UInt32", new List<string>() { "CheckType" }),
                new Tuple<string, List<string>>("UnicodeEncoding", new List<string>() { "$decode", "$encode" }),
                new Tuple<string, List<string>>("UTF8Encoding", new List<string>() { "$decode", "$encode" }),
                new Tuple<string, List<string>>("XmlSerializer", new List<string>() { "DeserializeInternal", "GetContractClass", "MakeSerializationReader", "MakeSerializationWriter", "MakeSerializer", "SerializeInternal", "XmlReaderFromStream", "XmlWriterForStream" }),


                // FROM THE JSIL PROXIES:
                new Tuple<string, List<string>>("Array", new List<string>() { ".ctor", "get_Length", "get_LongLength", "CreateInstance", "Set", "Get", "GetValue", "SetValue", "IndexOf", "Clone", "Clear", "CopyTo", "GetEnumerator" }),
                new Tuple<string, List<string>>("Console", new List<string>() { ".ctor", "WriteLine", "Write" }),
                new Tuple<string, List<string>>("GCHandle", new List<string>() { ".ctor", "Alloc" }),
                new Tuple<string, List<string>>("TimeSpan", new List<string>() { ".ctor", "op_Addition", "op_Subtraction" }),
                new Tuple<string, List<string>>("DateTime", new List<string>() { ".ctor", "op_Subtraction" }),
                new Tuple<string, List<string>>("Type", new List<string>() { ".ctor", "IsEquivalentTo" }),
                new Tuple<string, List<string>>("Math", new List<string>() { ".ctor", "Min", "Max", "Abs", "Sqrt", "Cos", "Sin", "Acos", "Asin", "Tan", "Atan", "Atan2", "Log", "Log10", "Round", "Floor", "Truncate", "Ceiling", "Pow" }),
                new Tuple<string, List<string>>("SByte", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Int16", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Int32", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Byte", new List<string>() { ".ctor", "CompareTo" }),
                new Tuple<string, List<string>>("UInt16", new List<string>() { ".ctor", "CompareTo" }),
                new Tuple<string, List<string>>("UInt32", new List<string>() { ".ctor", "CompareTo" }),
                new Tuple<string, List<string>>("Single", new List<string>() { ".ctor", "IsNaN", "CompareTo" }),
                new Tuple<string, List<string>>("Double", new List<string>() { ".ctor", "IsNaN", "CompareTo" }),
                new Tuple<string, List<string>>("Decimal", new List<string>() { ".ctor", "IsNaN", "CompareTo" }),
                new Tuple<string, List<string>>("UInt64", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Int64", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Object", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Boolean", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("String", new List<string>() { ".ctor", "Format", "Concat", "Split", "Join", "get_Length", "get_Chars", "op_Equality", "op_Inequality", "ToLower", "ToLowerInvariant", "ToUpper", "ToUpperInvariant", "Normalize", "StartsWith", "EndsWith", "Trim", "CompareTo", "IndexOf", "LastIndexOf", "IndexOfAny", "LastIndexOfAny", "Substring", "Replace", "ToCharArray", "Contains", "PadLeft", "PadRight", "Remove", "CopyTo" }),
                new Tuple<string, List<string>>("Task", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Task`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("TaskCompletionSource`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("TaskFactory", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("TaskFactory`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Exception", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("SystemException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("InvalidCastException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("InvalidOperationException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("FileNotFoundException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("FormatException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Console", new List<string>() { }),
                new Tuple<string, List<string>>("EventArgs", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("PropertyChangedEventArgs", new List<string>() { ".ctor", "get_PropertyName" }),
                new Tuple<string, List<string>>("Debug", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Thread", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("List`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("ArrayList", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Collection`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("ReadOnlyCollection`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Stack`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Queue`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Interlocked", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Monitor", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Random", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Math", new List<string>() { }),
                new Tuple<string, List<string>>("Environment", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Dictionary`2", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("KeyValuePair`2", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Nullable", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Nullable`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("XmlSerializer", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("StackTrace", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("StackFrame", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Enum", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Activator", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Stopwatch", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("GC", new List<string>() { ".ctor" }),
                //new Tuple<string, List<string>>("HashSet`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Convert", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("BitConverter", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("LinkedList`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("LinkedListNode`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Comparer`1", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("WeakReference", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Trace", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("TimeSpan", new List<string>() { "FromMilliseconds", "FromSeconds", "FromMinutes", "FromHours", "FromDays", "FromTicks", "op_Addition", "op_Equality", "op_GreaterThan", "op_GreaterThanOrEqual", "op_Inequality", "op_LessThan", "op_LessThanOrEqual", "op_Subtraction", "op_UnaryNegation", "get_Days", "get_Hours", "get_Milliseconds", "get_Minutes", "get_Seconds", "get_Ticks", "get_TotalMilliseconds", "get_TotalSeconds", "get_TotalMinutes", "get_TotalHours", "get_TotalDays", "Parse" }),
                new Tuple<string, List<string>>("DateTime", new List<string>() { "get_Now", "get_UtcNow" }),
                new Tuple<string, List<string>>("Enumerable", new List<string>() { ".ctor", "Any", "AsEnumerable", "Count", "First", "FirstOrDefault", "Select", "ToArray", "Contains", "Cast", "ToList", "ElementAt", "ElementAtOrDefault", "OfType", "Where", "Range", "Sum", "SelectMany", "Empty" }),
                new Tuple<string, List<string>>("Expression", new List<string>() { ".ctor", "Constant", "Lambda", "Parameter", "Equal" }),
                new Tuple<string, List<string>>("ConstantExpression", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("ParameterExpression", new List<string>() { ".ctor", "get_Type", "get_IsByRef", "get_NodeType", "get_CanReduce", "Reduce", "ReduceAndCheck", "ReduceExtensions" }),
                new Tuple<string, List<string>>("LambdaExpression", new List<string>() { ".ctor", "Compile", "get_NodeType", "get_Type", "get_CanReduce", "Reduce", "ReduceAndCheck", "ReduceExtensions" }),
                new Tuple<string, List<string>>("Expression`1", new List<string>() { ".ctor", "Compile", "get_NodeType", "get_Type", "get_CanReduce", "Reduce", "ReduceAndCheck", "ReduceExtensions" }),
                new Tuple<string, List<string>>("ResourceManager", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("ResourceSet", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("CultureInfo", new List<string>() { ".ctor", "Clone", "get_Name", "get_TwoLetterISOLanguageName", "get_UseUserOverride", "GetCultureInfo", "GetCultureInfoByIetfLanguageTag", "get_CurrentUICulture", "get_InvariantCulture", "get_CurrentCulture" }),
                new Tuple<string, List<string>>("String", new List<string>() { }),
                new Tuple<string, List<string>>("Encoding", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("ASCIIEncoding", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("UTF8Encoding", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("UnicodeEncoding", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("StringBuilder", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Regex", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("MatchCollection", new List<string>() { ".ctor" }),
                //new Tuple<string, List<string>>("Capture", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Group", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Match", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("GroupCollection", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("ParameterInfo", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("EventInfo", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("File", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Path", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Stream", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("FileStream", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("MemoryStream", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("BinaryWriter", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("BinaryReader", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("StreamReader", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("TextReader", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("FileSystemInfo", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("DirectoryInfo", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("FileInfo", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Directory", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Marshal", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("GCHandle", new List<string>() { }),
                new Tuple<string, List<string>>("Buffer", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("XmlSerializationReader", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("XmlQualifiedName", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("XmlReader", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("XmlNameTable", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("XmlConvert", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("XmlWriter", new List<string>() { ".ctor" }),


                // TESTED:
                new Tuple<string, List<string>>("Action", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Action`1", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Action`2", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Action`3", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Action`4", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Action`5", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Action`6", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Action`7", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Action`8", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("ArgumentException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("ArgumentNullException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Assembly", new List<string>() { "get_Location", "GetName" }),
                new Tuple<string, List<string>>("Attribute", new List<string>() { ".ctor", "GetCustomAttribute" }),
                new Tuple<string, List<string>>("Boolean", new List<string>() { "Parse", "TryParse" }),
                new Tuple<string, List<string>>("Char", new List<string>() { "IsUpper", "IsLower" }),
                new Tuple<string, List<string>>("Convert", new List<string>() { "ToBoolean", "ToChar", "ToByte", "ToInt16", "ToUInt16", "ToInt32", "ToUInt32", "ToInt64", "ToUInt64", "ToSingle", "ToDouble", "ToDecimal", "ToType" }),
                new Tuple<string, List<string>>("DateTime", new List<string>() { "Parse", "TryParse" }),
                new Tuple<string, List<string>>("Delegate", new List<string>() { "Combine", "CreateDelegate", "get_Method", "get_Target", "GetInvocationList", "op_Equality", "op_Inequality", "Remove" }),
                new Tuple<string, List<string>>("Dictionary`2", new List<string>() { ".ctor", "Add", "Clear", "ContainsKey", "get_Count", "get_Item", "get_Keys", "get_Values", "GetEnumerator", "Remove", "set_Item", "TryGetValue" }),
                new Tuple<string, List<string>>("Double", new List<string>() { "Parse", "TryParse" }),
                new Tuple<string, List<string>>("Enum", new List<string>() { ".ctor", "GetNames", "GetValues", "Object.Equals", "Parse", "ToInt32", "ToInt64", "ToObject", "TryParse" }), //todo: verify that "Object.Equals" is correct
                new Tuple<string, List<string>>("Enumerator", new List<string>() { ".ctor", "Dispose", "get_Current", "MoveNext", "Reset" }),
                new Tuple<string, List<string>>("EqualityComparer`1", new List<string>() { "get_Default" }),
                new Tuple<string, List<string>>("EventHandler", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("EventHandler`1", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Func`1", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Func`2", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Func`3", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Func`4", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Func`5", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Func`6", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Func`7", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Func`8", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Func`9", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("Guid", new List<string>() { ".ctor", "NewGuid", "op_Equality", "op_Inequality", "Parse" }),
                new Tuple<string, List<string>>("ICollection", new List<string>() { "CopyTo", "get_Count", "get_IsSynchronized" }),
                new Tuple<string, List<string>>("ICollection`1", new List<string>() { "Add", "Clear", "Contains", "CopyTo", "get_Count", "get_IsReadOnly" }),
                new Tuple<string, List<string>>("IComparable", new List<string>() { "CompareTo" }),
                new Tuple<string, List<string>>("IComparable`1", new List<string>() { "CompareTo" }),
                new Tuple<string, List<string>>("IComparer`1", new List<string>() { "Compare" }),
                new Tuple<string, List<string>>("IDictionary", new List<string>() { "Add", "Clear", "Contains", "get_Count", "get_Item", "get_Keys", "get_Values", "GetEnumerator", "Remove", "set_Item" }),
                new Tuple<string, List<string>>("IDictionary`2", new List<string>() { "Add", "Clear", "ContainsKey", "get_Count", "get_Item", "get_Keys", "get_Values", "GetEnumerator", "Remove", "set_Item", "TryGetValue" }),
                new Tuple<string, List<string>>("IDisposable", new List<string>() { "Dispose" }),
                new Tuple<string, List<string>>("IEnumerable", new List<string>() { "GetEnumerator" }),
                new Tuple<string, List<string>>("IEnumerable`1", new List<string>() { "GetEnumerator" }),
                new Tuple<string, List<string>>("IEnumerator", new List<string>() { "get_Current", "MoveNext" }),
                new Tuple<string, List<string>>("IEnumerator`1", new List<string>() { "get_Current" }),
                new Tuple<string, List<string>>("IList", new List<string>() { "Add", "Clear", "get_Count", "get_IsFixedSize", "Remove" }),
                new Tuple<string, List<string>>("IList`1", new List<string>() { "get_Item", "IndexOf", "Insert", "RemoveAt", "set_Item" }),
                new Tuple<string, List<string>>("INotifyCompletion", new List<string>() { "OnCompleted"}), // Required by async/await.
                new Tuple<string, List<string>>("Int16", new List<string>() { "Parse"}),
                new Tuple<string, List<string>>("Int32", new List<string>() { "Parse", "TryParse" }),
                new Tuple<string, List<string>>("Int64", new List<string>() { "Parse" }),
                new Tuple<string, List<string>>("Interlocked", new List<string>() { "CompareExchange" }),
                new Tuple<string, List<string>>("IReflect", new List<string>() { "get_UnderlyingSystemType", "GetField", "GetFields", "GetMember", "GetMembers", "GetMethod", "GetMethods", "GetProperties", "GetProperty" }),
                new Tuple<string, List<string>>("KeyCollection", new List<string>() { ".ctor", "get_Count", "GetEnumerator" }),
                new Tuple<string, List<string>>("KeyValuePair`2", new List<string>() { ".ctor", "get_Key", "get_Value", "toString" }),
                new Tuple<string, List<string>>("List`1", new List<string>() { ".ctor", "Add", "AddRange", "AsReadOnly", "BinarySearch", "Clear", "ClearItems", "Contains", "CopyTo", "Exists", "ForEach", "Find", "FindAll", "FindIndex", "get_Capacity", "get_Count", "get_IsReadOnly", "get_Item", "GetEnumerator", "IndexOf", "Insert", "InsertItem", "InsertRange", "Reverse", "Remove", "RemoveAll", "RemoveAt", "RemoveItem", "RemoveRange", "set_Item", "SetItem", "Sort", "ToArray", "TrueForAll" }),
                new Tuple<string, List<string>>("Math", new List<string>() { "Floor" }),
                new Tuple<string, List<string>>("MemberInfo", new List<string>() { "get_DeclaringType", "get_Name", "GetCustomAttributes" }),
                new Tuple<string, List<string>>("NotImplementedException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("NotSupportedException", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("Nullable`1", new List<string>() { "get_HasValue", "GetValue", "GetValueOrDefault" }),
                new Tuple<string, List<string>>("Path", new List<string>() { "Combine", "GetDirectoryName", "GetExtension", "GetFileName", "GetFileNameWithoutExtension", "GetFullPath" }),
                new Tuple<string, List<string>>("Predicate`1", new List<string>() { ".ctor", "Invoke" }),
                new Tuple<string, List<string>>("PropertyInfo", new List<string>() { "get_CanRead", "get_CanWrite", "get_PropertyType", "GetAccessors", "GetGetMethod", "GetIndexParameters", "GetSetMethod", "GetValue", "op_Equality", "op_Inequality", "SetValue" }),
                new Tuple<string, List<string>>("Random", new List<string>() { ".ctor", "Next", "NextDouble" }),
                new Tuple<string, List<string>>("RuntimeHelpers", new List<string>() { "InitializeArray" }),
                new Tuple<string, List<string>>("Stream", new List<string>() { "$PeekByte", "Close", "CopyTo", "Dispose", "get_CanRead", "get_CanSeek", "get_Length", "get_Position", "Read", "ReadByte", "Seek", "set_Position", "Write", "WriteByte" }),
                new Tuple<string, List<string>>("StreamWriter", new List<string>() { ".ctor" }),
                new Tuple<string, List<string>>("String", new List<string>() { ".ctor", "Compare", "Concat", "Contains", "CopyTo", "EndsWith", "Format", "get_Chars", "get_Length", "IndexOf", "IndexOfAny", "IsNullOrEmpty", "IsNullOrWhiteSpace", "LastIndexOfAny", "op_Equality", "PadLeft", "PadRight", "Remove", "Replace", "Split", "StartsWith", "Substring", "ToLower", "Trim" }),
                new Tuple<string, List<string>>("TextWriter", new List<string>() { "Write", "Flush" }),
                //new Tuple<string, List<string>>("Tuple`2", new List<string>() { ".ctor", "get_Item1", "get_Item2" }),
                //new Tuple<string, List<string>>("Tuple`3", new List<string>() { ".ctor", "get_Item1", "get_Item2", "get_Item3" }),
                //new Tuple<string, List<string>>("Tuple`4", new List<string>() { ".ctor", "get_Item1", "get_Item2", "get_Item3", "get_Item4" }),
                //new Tuple<string, List<string>>("Tuple`5", new List<string>() { ".ctor", "get_Item1", "get_Item2", "get_Item3", "get_Item4", "get_Item5" }),
                //new Tuple<string, List<string>>("Tuple`6", new List<string>() { ".ctor", "get_Item1", "get_Item2", "get_Item3", "get_Item4", "get_Item5", "get_Item6" }),
                //new Tuple<string, List<string>>("Tuple`7", new List<string>() { ".ctor", "get_Item1", "get_Item2", "get_Item3", "get_Item4", "get_Item5", "get_Item6", "get_Item7" }),
                new Tuple<string, List<string>>("Type", new List<string>() { "get_BaseType", "get_IsValueType", "GetProperty", "GetTypeFromHandle", "op_Inequality" }),
                new Tuple<string, List<string>>("UInt16", new List<string>() { "TryParse" }),
                new Tuple<string, List<string>>("ValueCollection", new List<string>() { ".ctor", "get_Count", "GetEnumerator" }),
            };

            // Add to each class the methods inherited from "Object":
            foreach (Tuple<string, List<string>> classAndMethods in result)
            {
                List<string> methods = classAndMethods.Item2;
                methods.Add("Equals");
                methods.Add("GetHashCode");
                methods.Add("GetType");
                methods.Add("MemberwiseClone");
                methods.Add("ReferenceEquals");
                methods.Add("ToString");
            }

#endif

            return result;
        }


        static List<Tuple<string, List<Tuple<string, string>>>> GetExplicitlyUnsupportedMethodsWithExplanation()
        {
            string messageForHashSet = "The \"HashSet\" class is not yet supported: please use the equivalent \"HashSet2\" class instead (you will be able to revert back to using the original HashSet class as soon as it becomes supported).";
            string messageForLazyClass = "The \"Lazy\" class is not yet supported: please use the equivalent \"Lazy2\" class instead (you will be able to revert back to using the original Lazy class as soon as it becomes supported).";

            return new List<Tuple<string, List<Tuple<string, string>>>>()
            {
                new Tuple<string, List<Tuple<string,string>>>("Thread", new List<Tuple<string,string>>()
                {
                    new Tuple<string,string>("Sleep","The method \"Thread.Sleep\" is not supported due to the single-threaded nature of JavaScript running in the web browser. Please consider using \"DispatcherTimer\" instead.")
                }),
                new Tuple<string, List<Tuple<string,string>>>("Task", new List<Tuple<string,string>>()
                {
                    new Tuple<string,string>("Run","The method \"Task.Run\" is not supported due to the single-threaded nature of JavaScript running in the web browser. A possible workaround may be to use the \"DispatcherTimer\" class or the \"Dispatcher.BeginInvoke\" method."),
                    new Tuple<string,string>("Wait","The method \"Task.Wait\" is not supported due to the single-threaded nature of JavaScript running in the web browser. A possible workaround may be to use the \"DispatcherTimer\" class or the \"Dispatcher.BeginInvoke\" method."),
                    new Tuple<string,string>("WaitAll","The method \"Task.WaitAll\" is not supported due to the single-threaded nature of JavaScript running in the web browser. A possible workaround may be to use the \"DispatcherTimer\" class or the \"Dispatcher.BeginInvoke\" method."),
                    new Tuple<string,string>("WaitAny","The method \"Task.WaitAny\" is not supported due to the single-threaded nature of JavaScript running in the web browser. A possible workaround may be to use the \"DispatcherTimer\" class or the \"Dispatcher.BeginInvoke\" method.")
                }),
                new Tuple<string, List<Tuple<string,string>>>("Timer", new List<Tuple<string,string>>()
                {
                    new Tuple<string,string>(".ctor","The \"Timer\" class is not yet supported: please use the \"DispatcherTimer\" class instead.")
                }),
                new Tuple<string, List<Tuple<string,string>>>("HashSet`1", new List<Tuple<string,string>>()
                {
                    new Tuple<string,string>(".ctor",messageForHashSet),
                    new Tuple<string,string>("Add",messageForHashSet),
                    new Tuple<string,string>("Clear",messageForHashSet),
                    new Tuple<string,string>("Contains",messageForHashSet),
                    new Tuple<string,string>("get_Count",messageForHashSet),
                    new Tuple<string,string>("GetEnumerator",messageForHashSet),
                    new Tuple<string,string>("Remove",messageForHashSet)
                }),
                //new Tuple<string, List<Tuple<string,string>>>("ResourceManager", new List<Tuple<string,string>>()
                //{
                //    new Tuple<string,string>(".ctor","Resource files (.resx) are not yet supported. Please vote for this feature at cshtml5.uservoice.com, or request it by posting on the forums at forums.cshtml5.com or by sending an email to support@cshtml5.com")
                //}),
                new Tuple<string, List<Tuple<string,string>>>("Guid", new List<Tuple<string,string>>()
                {
                    new Tuple<string,string>("TryParse","The method \"Guid.TryParse\" is not yet supported. Please use \"Guid.Parse\" instead."),
                    new Tuple<string,string>("TryParseExact","The method \"Guid.TryParseExact\" is not yet supported. Please use \"Guid.Parse\" instead."),
                    new Tuple<string,string>("ParseExact","The method \"Guid.ParseExact\" is not yet supported. Please use \"Guid.Parse\" instead.")
                }),
                new Tuple<string, List<Tuple<string,string>>>("DateTime", new List<Tuple<string,string>>()
                {
                    new Tuple<string,string>("TryParseExact","The method \"DateTime.TryParseExact\" is not yet supported. Please use \"DateTime.TryParse\" instead."),
                    new Tuple<string,string>("ParseExact","The method \"DateTime.ParseExact\" is not yet supported. Please use \"DateTime.Parse\" instead.")
                }),
                new Tuple<string, List<Tuple<string,string>>>("IsolatedStorageFile", new List<Tuple<string,string>>()
                {
                    new Tuple<string,string>("GetUserStoreForApplication","The method \"IsolatedStorageFile.GetUserStoreForApplication\" is not supported: please use \"IsolatedStorageFile.GetUserStoreForAssembly\" instead."),
                    new Tuple<string,string>("GetUserStoreForDomain","The method \"IsolatedStorageFile.GetUserStoreForDomain\" is not yet supported: please use \"IsolatedStorageFile.GetUserStoreForAssembly\" instead."),
                    new Tuple<string,string>("GetUserStoreForSite","The method \"IsolatedStorageFile.GetUserStoreForSite\" is not yet supported: please use \"IsolatedStorageFile.GetUserStoreForAssembly\" instead.")
                }),
                new Tuple<string, List<Tuple<string,string>>>("Delegate", new List<Tuple<string,string>>()
                {
                    new Tuple<string,string>("DynamicInvoke","The method \"Delegate.DynamicInvoke\" is not yet supported. Please use \"Delegate.Invoke\" instead."),
                }),
                new Tuple<string, List<Tuple<string,string>>>("Lazy`1", new List<Tuple<string,string>>()
                {
                    new Tuple<string,string>(".ctor", messageForLazyClass),
                    new Tuple<string,string>("get_Value", messageForLazyClass),
                }),
            };
        }

        public void InitializeListOfSupportedMethods(IEnumerable<string> pathOfJsilProxyAssemblies, ILogger logger)
        {

            // We commented the section below since the Beta 3 because we manually added the items to the explicit list at the beginning of this file.
            // We keep the section below to help update that list.
#if CODE_TO_GENERATE_LIST_OF_SUPPORTED_METHODS_BY_ANALYZING_PROXIES
            string temp = "";

            // Analyze the "Proxy" assemblies of JSIL to determine the list of supported system methods:
            foreach (string proxyAssemblyPath in pathOfJsilProxyAssemblies)
            {
                logger.WriteMessage("Analyzing proxy: " + proxyAssemblyPath);

                Assembly assembly = Assembly.LoadFrom(proxyAssemblyPath);

                foreach (Type type in assembly.DefinedTypes)
                {
                    foreach (JSProxy jsProxyAttribute in type.GetCustomAttributes(typeof(JSProxy)))
                    {
                        if (jsProxyAttribute.Types != null)
                        {
                            foreach (Type proxiedType in jsProxyAttribute.Types)
                            {
                                List<string> tempMethods = new List<string>();
                                HashSet<string> referenceToListOfSupportedMethods = GetReferenceToListOfSupportedMethodsForAGivenTypeName(proxiedType.Name);
                                foreach (MethodInfo method in type.GetMethods())
                                {
                                    if (!tempMethods.Contains("\"" + method.Name + "\""))
                                        tempMethods.Add("\"" + method.Name + "\"");

                                    referenceToListOfSupportedMethods.Add(method.Name); //todo: take more fine-grained approach by also looking at the "JSExternal" attribute and other custom attributes that the methods may have?
                                }
                                if (!referenceToListOfSupportedMethods.Contains(".ctor"))
                                {
                                    tempMethods.Insert(0, "\"" + ".ctor" + "\"");

                                    referenceToListOfSupportedMethods.Add(".ctor");
                                }

                                temp += "\r\nnew Tuple<string, List<string>>(\"" + proxiedType.Name + "\", new List<string>() { " + string.Join(", ", tempMethods) + " }),";
                            }
                        }
                        else if (jsProxyAttribute.Type != null)
                        {
                            Type proxiedType = jsProxyAttribute.Type;
                            List<string> tempMethods = new List<string>();
                            HashSet<string> referenceToListOfSupportedMethods = GetReferenceToListOfSupportedMethodsForAGivenTypeName(proxiedType.Name);
                            foreach (MethodInfo method in type.GetMethods())
                            {
                                if (!tempMethods.Contains("\"" + method.Name + "\""))
                                    tempMethods.Add("\"" + method.Name + "\"");

                                referenceToListOfSupportedMethods.Add(method.Name); //todo: take more fine-grained approach by also looking at the "JSExternal" attribute and other custom attributes that the methods may have?
                            }
                            if (!referenceToListOfSupportedMethods.Contains(".ctor"))
                            {
                                tempMethods.Insert(0, "\"" + ".ctor" + "\"");

                                referenceToListOfSupportedMethods.Add(".ctor");
                            }

                            temp += "\r\nnew Tuple<string, List<string>>(\"" + proxiedType.Name + "\", new List<string>() { " + string.Join(", ", tempMethods) + " }),";
                        }
                    }
                }
            }
#endif

            // Add additional supported methods that are not defined in the proxies (such as "Int32.Parse"):
            foreach (Tuple<string, List<string>> typeToMethods in GetSupportedMethods())
            {
                HashSet<string> typeMethods = GetReferenceToListOfSupportedMethodsForAGivenTypeName(typeToMethods.Item1);
                foreach (string methodName in typeToMethods.Item2)
                {
                    typeMethods.Add(methodName);
                }
            }

            // Initialize the list of explicitly unsupported methods with their explanation:
            foreach (Tuple<string, List<Tuple<string, string>>> typeToMethods in GetExplicitlyUnsupportedMethodsWithExplanation())
            {
                Dictionary<string, string> typeMethods = GetReferenceToListOfUnsupportedMethodsForAGivenTypeName(typeToMethods.Item1);
                foreach (Tuple<string, string> methodNameAndExplanation in typeToMethods.Item2)
                {
                    typeMethods[methodNameAndExplanation.Item1] = methodNameAndExplanation.Item2;
                }
            }

            // Generate HTML that lists the supported methods:
            //var generatedHtml = GenerateHtmlWithListOfSupportedMethods();

            // Generate XML that lists the supported methods:
            //var generatedXml = GenerateXmlWithListOfSupportedMethods();

            _initialized = true;
        }

        public static string GenerateXmlWithListOfSupportedMethods(Dictionary<string, HashSet<string>> supportedMscorlibMethods)
        {
            //-------------------------------
            // EXAMPLE OF OUTPUT STRUCTURE:
            //-------------------------------
            //      <Compiler>
            //        <SupportedElements>
            //          <Assembly Name="mscorlib">
            //            <Type Name="Math">
            //               <Member Name="Max"/>
            //               <Member Name="Min"/>
            //            </Type>
            //          <Assembly/>
            //        </SupportedElements>
            //      </Compiler>


            XDocument xdoc = new XDocument();
            XElement compiler = new XElement("Compiler");
            xdoc.Add(compiler);
            XElement supportedElements = new XElement("SupportedElements");
            compiler.Add(supportedElements);
            XElement assembly = new XElement("Assembly");
            assembly.Add(new XAttribute("Name", "mscorlib"));
            supportedElements.Add(assembly);

            // Sort the types by name:
            List<KeyValuePair<string, HashSet<string>>> types = supportedMscorlibMethods.ToList();
            types.Sort((x, y) => (x.Key.CompareTo(y.Key)));

            foreach (KeyValuePair<string, HashSet<string>> typeToMethods in types)
            {
                // Add the type name:
                string typeName = typeToMethods.Key;
                XElement type = new XElement("Type");
                type.Add(new XAttribute("Name", typeName));
                assembly.Add(type);

                // Sort the members by name:
                List<string> members = typeToMethods.Value.ToList();
                members.Sort();

                // Add the members:
                foreach (string memberName in members)
                {
                    XElement member = new XElement("Member");
                    member.Add(new XAttribute("Name", memberName));
                    type.Add(member);
                }
            }

            var result = xdoc.ToString();

            return result;
        }

        public string GenerateHtmlWithListOfSupportedMethods()
        {
            var stringBuilder = new StringBuilder();

            var lines = new List<string>();

            foreach (KeyValuePair<string, HashSet<string>> typeToMethods in _supportedMscorlibMethods)
            {
                string typeName = typeToMethods.Key
                        .Replace("`1", "&lt;T&gt;")
                        .Replace("`2", "&lt;T1,T2&gt;")
                        .Replace("`3", "&lt;T1,T2,T3&gt;")
                        .Replace("`4", "&lt;T1,T2,T3,T4&gt;")
                        .Replace("`5", "&lt;T1,T2,T3,T4,T5&gt;")
                        .Replace("`6", "&lt;T1,T2,T3,T4,T5,T6&gt;")
                        .Replace("`7", "&lt;T1,T2,T3,T4,T5,T6,T7&gt;")
                        .Replace("`8", "&lt;T1,T2,T3,T4,T5,T6,T7,T8&gt;")
                        .Replace("`9", "&lt;T1,T2,T3,T4,T5,T6,T7,T8,T9&gt;");
                IEnumerable<string> methods = typeToMethods.Value;
                IEnumerable<string> methodsProcessed = methods
                    .Where(x => !x.StartsWith("$") && !x.StartsWith("set_") && x != ".ctor" && x != ".cctor" && x != ".cctor2" && x != "Equals" && x != "GetHashCode" && x != "GetType" && x != "ToString" && x != "__CopyMembers__")
                    .Select(x => (x.StartsWith("get_") ? x.Substring(4) : x)
                        .Replace("`1", "&lt;T&gt;")
                        .Replace("`2", "&lt;T1,T2&gt;")
                        .Replace("`3", "&lt;T1,T2,T3&gt;")
                        .Replace("`4", "&lt;T1,T2,T3,T4&gt;")
                        .Replace("`5", "&lt;T1,T2,T3,T4,T5&gt;")
                        .Replace("`6", "&lt;T1,T2,T3,T4,T5,T6&gt;")
                        .Replace("`7", "&lt;T1,T2,T3,T4,T5,T6,T7&gt;")
                        .Replace("`8", "&lt;T1,T2,T3,T4,T5,T6,T7,T8&gt;")
                        .Replace("`9", "&lt;T1,T2,T3,T4,T5,T6,T7,T8,T9&gt;"))
                    .OrderBy(x => x);
                lines.Add(string.Format("<li><strong>{0}</strong>{1}{2}</li>", typeName, methodsProcessed.Count() > 0 ? ": " : "", string.Join(", ", methodsProcessed)));
            }

            var result = string.Join("\r\n", lines.OrderBy(x => x));

            return result;
        }

        internal void AddAdditionalSupportedMethods(string additionalSupportedMethods)
        {
            if (!string.IsNullOrEmpty(additionalSupportedMethods))
            {
                string[] additionalSupportedMethodsAsArray = additionalSupportedMethods.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string supportedMethod in additionalSupportedMethodsAsArray)
                {
                    int positionOfFirstDot = supportedMethod.IndexOf('.'); // Note: we use the first occurance of the dot instead of the last occurance so as to handle scenarios like "EventHandler`1..ctor", where ".ctor" is the name of the method.
                    if (positionOfFirstDot >= 0)
                    {
                        string methodName = supportedMethod.Substring(positionOfFirstDot + 1);
                        string className = supportedMethod.Substring(0, positionOfFirstDot);
                        if (methodName != "" && className != "")
                        {
                            HashSet<string> typeMethods = GetReferenceToListOfSupportedMethodsForAGivenTypeName(className);
                            if (!typeMethods.Contains(methodName))
                                typeMethods.Add(methodName);
                        }
                    }
                }
            }
        }

        internal bool IsTypeSupported(string typeName)
        {
            return _supportedMscorlibMethods.ContainsKey(typeName);
        }

        internal bool IsMethodSupported(string typeName, string methodName)
        {
            return _supportedMscorlibMethods.ContainsKey(typeName)
                && _supportedMscorlibMethods[typeName].Contains(methodName);
        }

        HashSet<string> DELETEME = new HashSet<string>();

        public void Check(AssemblyDefinition[] assembliesDefinitions, string nameOfAssembliesThatDoNotContainUserCode, string activationAppPath, string commaSeparatedSystemAssembliesThatCanContainUnsupportedMethods, HashSet<string> flags, Action<UnsupportedMethodInfo> whatToDoWhenNotSupportedMethodFound)
        {
            //todo-performance: optimize this method (only check the right assemblies, only check some types, etc.)

            if (!_initialized)
                throw new Exception("You must call the \"InitializeListOfSupportedMethods()\" method before calling this one.");

            HashSet<string> errorsAlreadyRaised = new HashSet<string>(); // This prevents raising multiple times the same error.
            HashSet<string> activatedFeatures = new HashSet<string>(); // Temporary cache for performance in the loop.
            HashSet<string> systemAssembliesThatCanContainUnsupportedMethods = new HashSet<string>(commaSeparatedSystemAssembliesThatCanContainUnsupportedMethods.Split(','));

            foreach (AssemblyDefinition userAssembly in GetAllUserAssemblies(assembliesDefinitions, nameOfAssembliesThatDoNotContainUserCode))
            {
#if REQUIRE_ACTIVATION_FOR_USING_CERTAIN_METHODS
                bool isThisProjectTheSampleShowcaseApp = ActivationHelpers.IsThisProjectTheSampleShowcaseApp(userAssembly);
#endif
                string userAssemblyName = userAssembly.Name.Name;

                foreach (MethodDefinition userMethod in GetAllMethodsDefinedInAssembly(userAssembly))
                {
                    foreach (MemberReferenceAndCallerInformation referencedMethodAndCorrespondingInstruction in GetAllMethodsReferencedInMethod(userMethod))
                    {
                        string declaringTypeName = referencedMethodAndCorrespondingInstruction.MemberReference.DeclaringType.Name;
                        if (declaringTypeName.Contains('['))
                            declaringTypeName = declaringTypeName.Substring(0, declaringTypeName.IndexOf('['));
                        string fullMethodName = declaringTypeName + "." + referencedMethodAndCorrespondingInstruction.MemberReference.Name;
                        string methodAssemblyName = referencedMethodAndCorrespondingInstruction.MemberReference.DeclaringType.Scope.Name;

                        //if (!DELETEME.Contains(fullMethodName))
                        //{
                        //    Console.WriteLine(fullMethodName);
                        //    DELETEME.Add(fullMethodName);
                        //}

                        string explanationToDisplayInErrorsWindow;
                        string callingMethodFullName = userMethod.DeclaringType.Name + "." + userMethod.Name;
#if REQUIRE_ACTIVATION_FOR_USING_CERTAIN_METHODS
                        // Check if the method requires activation (note that we skip the verification if the project is the sample showcase app):
                        string explanationToDisplayInActivationApp;
                        string missingFeatureId;
                        int unused;
                        if (!isThisProjectTheSampleShowcaseApp && !ActivationHelpers.IsMethodAllowed(
                                                                        referencedMethodAndCorrespondingInstruction.MemberReference,
                                                                        out explanationToDisplayInErrorsWindow,
                                                                        out explanationToDisplayInActivationApp,
                                                                        out missingFeatureId))
                        {
                            List<string> featuresContainingMissingFeature = new List<string>();
                            featuresContainingMissingFeature.Add(Constants.ENTERPRISE_EDITION_FEATURE_ID); //this one contains everything
                            //todo: if we make more editions that do not contain everything (for example it contains sl migration edition but not professional edition), change the way we make the tests below.
                            //Note: for the todo above, we would also need to change the isMethod Allowed so that it can return the list of the possible editions I think.
                            if (missingFeatureId != Constants.ENTERPRISE_EDITION_FEATURE_ID)
                            {
                                featuresContainingMissingFeature.Add(Constants.SL_MIGRATION_EDITION_FEATURE_ID); //this one contains everything that does not contain enterprise
                                if (missingFeatureId != Constants.SL_MIGRATION_EDITION_FEATURE_ID)
                                {
                                    featuresContainingMissingFeature.Add(Constants.PROFESSIONAL_EDITION_FEATURE_ID); //this is the last one.
                                }
                            }
                            if (!errorsAlreadyRaised.Contains(explanationToDisplayInErrorsWindow))
                            {
                                bool missingFeatureSupported = false;

                                foreach (string missingFeature in featuresContainingMissingFeature)
                                {
                                    // Check if the user has not activated the license key for the required feature:
                                    if (!activatedFeatures.Contains(missingFeature)) // Just a temporary cache for faster performance of the loop.
                                    {
                                        // Check if the user has not activated the license key for the required feature:
                                        if (ActivationHelpers.IsFeatureEnabled(missingFeature, flags))
                                        {
                                            activatedFeatures.Add(missingFeature);
                                            missingFeatureSupported = true;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        missingFeatureSupported = true;
                                        break;
                                    }
                                }
                                if (!missingFeatureSupported)
                                {
                                    bool isInValidTrialMode = false;
                                    foreach (string missingFeature in featuresContainingMissingFeature)
                                    {
                                        if ((TrialHelpers.IsTrial(missingFeature, out unused) == TrialHelpers.TrialStatus.Running))
                                        {
                                            isInValidTrialMode = true;
                                            break;
                                        }
                                    }

                                    //show an error (a window asking for activation will also appear):
                                    whatToDoWhenNotSupportedMethodFound(
                                                new UnsupportedMethodInfo()
                                                {
                                                    ExplanationToDisplayInErrorsWindow = explanationToDisplayInErrorsWindow + string.Format(ErrorLocationString, callingMethodFullName, userAssemblyName),
                                                    RequiresMissingFeature = true,
                                                    MissingFeatureId = missingFeatureId,
                                                    MessageForMissingFeature = explanationToDisplayInActivationApp,
                                                    IsInValidTrialMode = isInValidTrialMode,
                                                    FullMethodName = fullMethodName,
                                                    CallingMethodFullName = callingMethodFullName,
                                                    CallingMethodFileNameWithPath = referencedMethodAndCorrespondingInstruction.CallerFileNameOrEmpty,
                                                    CallingMethodLineNumber = referencedMethodAndCorrespondingInstruction.CallerLineNumberOrZero,
                                                    UserAssemblyName = userAssemblyName,
                                                    MethodAssemblyName = methodAssemblyName
                                                });
                                    errorsAlreadyRaised.Add(explanationToDisplayInErrorsWindow);
                                }
                            }
                        }
                        else
#endif
                        // Methods that are not defined in system files are all supported:
                        if (systemAssembliesThatCanContainUnsupportedMethods.Contains(methodAssemblyName))
                        {
                            if (IsMethodExplicitlyUnsupportedAndWhy(referencedMethodAndCorrespondingInstruction.MemberReference, out explanationToDisplayInErrorsWindow))
                            {
                                if (!errorsAlreadyRaised.Contains(explanationToDisplayInErrorsWindow))
                                {
                                    whatToDoWhenNotSupportedMethodFound(
                                        new UnsupportedMethodInfo()
                                            {
                                                ExplanationToDisplayInErrorsWindow = explanationToDisplayInErrorsWindow + string.Format(ErrorLocationString, callingMethodFullName, userAssemblyName),
#if REQUIRE_ACTIVATION_FOR_USING_CERTAIN_METHODS
                                                RequiresMissingFeature = false,
                                                MissingFeatureId = null,
                                                MessageForMissingFeature = null,
                                                IsInValidTrialMode = false,
#endif
                                            FullMethodName = fullMethodName,
                                                CallingMethodFullName = callingMethodFullName,
                                                CallingMethodFileNameWithPath = referencedMethodAndCorrespondingInstruction.CallerFileNameOrEmpty,
                                                CallingMethodLineNumber = referencedMethodAndCorrespondingInstruction.CallerLineNumberOrZero,
                                                UserAssemblyName = userAssemblyName,
                                                MethodAssemblyName = methodAssemblyName
                                            });
                                    errorsAlreadyRaised.Add(explanationToDisplayInErrorsWindow);
                                }
                            }
                            else if (!IsMethodSupported(referencedMethodAndCorrespondingInstruction.MemberReference))
                            {
                                string explanationToDisplayInErrorsWindow2 = string.Format(ExplanationWhenUnsupported, fullMethodName) + string.Format(ErrorLocationString, callingMethodFullName, userAssemblyName);
                                if (!errorsAlreadyRaised.Contains(explanationToDisplayInErrorsWindow2))
                                {
                                    whatToDoWhenNotSupportedMethodFound(
                                        new UnsupportedMethodInfo()
                                            {
                                                ExplanationToDisplayInErrorsWindow = explanationToDisplayInErrorsWindow2,
#if REQUIRE_ACTIVATION_FOR_USING_CERTAIN_METHODS
                                                RequiresMissingFeature = false,
                                                MissingFeatureId = null,
                                                MessageForMissingFeature = null,
                                                IsInValidTrialMode = false,
#endif
                                            FullMethodName = fullMethodName,
                                                CallingMethodFullName = callingMethodFullName,
                                                CallingMethodFileNameWithPath = referencedMethodAndCorrespondingInstruction.CallerFileNameOrEmpty,
                                                CallingMethodLineNumber = referencedMethodAndCorrespondingInstruction.CallerLineNumberOrZero,
                                                UserAssemblyName = userAssemblyName,
                                                MethodAssemblyName = methodAssemblyName
                                            });
                                    errorsAlreadyRaised.Add(explanationToDisplayInErrorsWindow2);
                                    System.Diagnostics.Debug.WriteLine(fullMethodName);
                                }
                            }
                        }
                    }
                }
            }
        }

        static IEnumerable<AssemblyDefinition> GetAllUserAssemblies(AssemblyDefinition[] assembliesDefinitions, string nameOfAssembliesThatDoNotContainUserCode)
        {
            HashSet<string> listOfNamesOfAssembliesThatDoNotContainUserCode = new HashSet<string>();
            if (nameOfAssembliesThatDoNotContainUserCode != null)
            {
                string[] array = nameOfAssembliesThatDoNotContainUserCode.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string item in array)
                {
                    listOfNamesOfAssembliesThatDoNotContainUserCode.Add(item.ToLower());
                }
            }

            foreach (AssemblyDefinition assemblyDefinition in assembliesDefinitions)
            {
                // Retain only user assemblies:
                if (!listOfNamesOfAssembliesThatDoNotContainUserCode.Contains(assemblyDefinition.Name.Name.ToLower()))
                {
                    yield return assemblyDefinition;
                }
            }
        }

        static IEnumerable<MethodDefinition> GetAllMethodsDefinedInAssembly(AssemblyDefinition assemblyDefinition)
        {
            // Iterate through all the members:
            foreach (Mono.Cecil.ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
            {
                foreach (Mono.Cecil.TypeDefinition typeDefinition in moduleDefinition.Types)
                {
                    foreach (Mono.Cecil.MethodDefinition methodDefinition in typeDefinition.Methods)
                    {
                        yield return methodDefinition;
                    }
                    foreach (Mono.Cecil.PropertyDefinition propertyDefinition in typeDefinition.Properties)
                    {
                        var getMethod = propertyDefinition.GetMethod;
                        var setMethod = propertyDefinition.SetMethod;
                        if (getMethod != null)
                            yield return getMethod;
                        if (setMethod != null)
                            yield return setMethod;
                    }
                }
            }
        }

        static IEnumerable<MemberReferenceAndCallerInformation> GetAllMethodsReferencedInMethod(MethodDefinition methodDefinition)
        {
            if (methodDefinition.HasBody)
            {
                SequencePoint lastNonNullSequencePoint = null;
                foreach (Instruction instruction in methodDefinition.Body.Instructions)
                {
                    if (instruction.SequencePoint != null)
                        lastNonNullSequencePoint = instruction.SequencePoint;

                    MethodReference mRef = instruction.Operand as MethodReference;

                    if (mRef != null && mRef.DeclaringType != null)
                    {
                        var memberReferenceAndCorrespondingInstruction
                            = new MemberReferenceAndCallerInformation(mRef, lastNonNullSequencePoint);

                        yield return memberReferenceAndCorrespondingInstruction;
                    }
                }
            }
        }

        bool IsMethodExplicitlyUnsupportedAndWhy(MemberReference methodReference, out string why)
        {
            string typeName = methodReference.DeclaringType.Name;
            Dictionary<string, string> methodsAndExplanations;
            if (_unsupportedMscorlibMethodsWithExplanation.TryGetValue(typeName, out methodsAndExplanations))
            {
                string explanation;
                if (methodsAndExplanations.TryGetValue(methodReference.Name, out explanation))
                {
                    why = explanation;
                    return true;
                }
            }
            why = null;
            return false;
        }

        bool IsMethodSupported(MemberReference methodReference)
        {
            string typeName = methodReference.DeclaringType.Name;
            return (_supportedMscorlibMethods.ContainsKey(typeName)
                && _supportedMscorlibMethods[typeName].Contains(methodReference.Name));
        }

        HashSet<string> GetReferenceToListOfSupportedMethodsForAGivenTypeName(string typeName)
        {
            HashSet<string> supportedMethods;
            if (_supportedMscorlibMethods.ContainsKey(typeName))
            {
                supportedMethods = _supportedMscorlibMethods[typeName];
            }
            else
            {
                supportedMethods = new HashSet<string>();
                _supportedMscorlibMethods.Add(typeName, supportedMethods);
            }
            return supportedMethods;
        }

        Dictionary<string, string> GetReferenceToListOfUnsupportedMethodsForAGivenTypeName(string typeName)
        {
            Dictionary<string, string> unsupportedMethods;
            if (_unsupportedMscorlibMethodsWithExplanation.ContainsKey(typeName))
            {
                unsupportedMethods = _unsupportedMscorlibMethodsWithExplanation[typeName];
            }
            else
            {
                unsupportedMethods = new Dictionary<string, string>();
                _unsupportedMscorlibMethodsWithExplanation.Add(typeName, unsupportedMethods);
            }
            return unsupportedMethods;
        }

        internal void Check(AssemblyDefinition[] definitions, string nameOfAssembliesThatDoNotContainUserCode, string activationAppPath, object whatToDoWhenNotSupportedMethodFound)
        {
            throw new NotImplementedException();
        }
    }
}
