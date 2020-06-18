using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal static class RemovalOfUnsupportedTypesFromCorlib
    {
        //---------
        // NOTE: Here, nested type names must contain the "+" sign.
        //---------

        //Note: the following list was created by listing the types declared in the JSIL bootstrap libraries:
        static string SupportedTypesTakenFromJsilBootstrapLibraries = @"Action,Action`1,Action`2,Action`3,Activator,ArithmeticException,array,Array,ArrayList,ASCIIEncoding,Assembly,AsyncTaskMethodBuilder,AsyncTaskMethodBuilder`1,AsyncVoidMethodBuilder,BinaryReader,BinaryWriter,BindingFlags,BitConverter,Boolean,Buffer,Byte,Capture,Char,CheckType,Collection`1,Comparer`1,Console,ConstantExpression,ConstructorInfo,Convert,CultureInfo,DateTime,DateTimeKind,Debug,Delegate,Dictionary`2,Dictionary`2+Enumerator,Dictionary`2+KeyCollection,Dictionary`2+KeyCollection+Enumerator,Dictionary`2+ValueCollection,Dictionary`2+ValueCollection+Enumerator,Directory,DirectoryInfo,Double,Encoding,Enum,Enumerable,Environment,EventArgs,EventInfo,Exception,Expression,Expression`1,FieldInfo,File,FileInfo,FileMode,FileNotFoundException,FileStream,FileSystemInfo,FormatException,Func`1,Func`2,Func`3,Func`4,GC,GCHandle,Group,GroupCollection,HashSet`1,HashSet`1+Enumerator,IAsyncResult,IAsyncStateMachine,ICollection,ICollection`1,IComparer,IComparer`1,IConvertible,IDictionary,IDictionary`2,IDictionaryEnumerator,IDisposable,IEnumerable,IEnumerable`1,IEnumerator,IEnumerator`1,IEquatable`1,IFormatProvider,IList,IList`1,INotifyCompletion,Int16,Int32,int64,Int64,Interlocked,IntPtr,InvalidCastException,InvalidOperationException,IOException,IPackedArray`1,IReadOnlyCollection`1,IReadOnlyList`1,ITuple,KeyValuePair`2,LambdaExpression,LinkedList`1,LinkedListNode`1,List`1,ManualResetEventSlim,Marshal,MarshalByRefObject,Match,MatchCollection,Math,MemberDescriptor,MemberInfo,MemberTypes,MemoryStream,MessageBox,MethodBase,MethodInfo,Monitor,MulticastDelegate,Nullable,Nullable`1,NumberStyles,Object,ParameterExpression,ParameterInfo,Path,Pointer,Predicate`1,PropertyChangedEventArgs,PropertyInfo,Queue`1,Random,ReadOnlyCollection`1,Reference,Regex,RegexOptions,ResourceManager,ResourceSet,SByte,SeekOrigin,Single,Stack`1,StackFrame,StackTrace,Stopwatch,Stream,StreamReader,String,StringBuilder,StringComparison,SystemException,Task,Task`1,TaskAwaiter,TaskAwaiter`1,TaskCompletionSource`1,TaskFactory,TaskFactory`1,TaskStatus,TextReader,Thread,TimeSpan,toString,Trace,Tuple,Type,TypeCode,TypeConverter,UInt16,UInt32,UInt64,UIntPtr,UnicodeEncoding,UTF8Encoding,UTF8Encoding+UTF8EncodingSealed,ValueType,VirtualDirectory,VirtualFile,VirtualJunction,VirtualVolume,WeakReference,XmlConvert,XmlNameTable,XmlNodeType,XmlQualifiedName,XmlReader,XmlSerializationReader,XmlSerializer,XmlWriter";

        //Note: the following list was created by listing the types used by the fields of the supported types in Mscorlib. We then manually added some types to support additional features (such as async/await):
        static string AdditionalSupportedTypes = @"AppDomain,AsyncCallback,AsyncMethodBuilderCore,AssemblyHashAlgorithm,AssemblyNameFlags,AssemblyVersionCompatibility,Binder,bucket,Calendar,CharEnumerator,CancellationToken,CancellationTokenSource,CodeAccessPermission,CodePageDataItem,CompareInfo,CompilerGeneratedAttribute,ConcurrentDictionary`2,ConditionalWeakTable`2,ConditionalWeakTable`2+CreateValueCallback,ConditionalWeakTable`2+Entry,Console+ControlCHooker,CriticalFinalizerObject,CultureData,ConsoleCancelEventHandler,Context,ContextCallback,Task+ContingentProperties,ControlCHooker,ResourceManager+CultureNameResourceSetPair,DateTimeFormatInfo,DayOfWeek,DebuggerBrowsableAttribute,DebuggerBrowsableState,DebuggerDisplayAttribute,DebuggerStepThroughAttribute,Decoder,DecoderFallback,Dictionary`2+Entry,DynamicPropertyHolder,Encoder,EncoderFallback,Environment+ResourceHelper,EventWaitHandle,ExceptionDispatchInfo,ExecutionContext,FileIOPermission,GCHandleCookieTable,GCHandleType,Guid,Hashtable,Hex,IComparable`1,IContextProperty,IEqualityComparer,IEqualityComparer`1,IMessageSink,Win32Native+.InputRecord,IResourceGroveler,IResourceReader,IsolatedStorage,IsolatedStorageFile,IsolatedStorageFilePermission,IsolatedStorageFileStream,IsolatedStoragePermission,IsolatedStorageScope,LocalDataStoreHolder,LocalDataStoreMgr,ManualResetEvent,StreamWriter+MdaHelper,NumberFormatInfo,Environment+OSName,OnDeserializedAttribute,OnDeserializingAttribute,OnSerializedAttribute,OnSerializingAttribute,OperatingSystem,OptionalFieldAttribute,ParameterAttributes,ParameterModifier,PermissionSet,PRIExceptionInfo,RegionInfo,ReliabilityContractAttribute,ResourceHelper,RuntimeAssembly,RuntimeType,SafeFileHandle,SafeHandle,SafeHandleZeroOrMinusOneIsInvalid,SafeIsolatedStorageFileHandle,SafeSerializationManager,SemaphoreSlim,SecurityPermission,SendOrPostCallback,SerializableAttribute,SerializationException,SerializationInfo,Stack,StackGuard,Stream+ReadWriteParameters,Stream+ReadWriteTask,StreamingContext,StreamingContextStates,StrongNameKeyPair,SynchronizationContext,SynchronizationContextProperties,TaskContinuationOptions,TaskCreationOptions,TaskScheduler,TextInfo,TimerCallback,UltimateResourceFallbackLocation,UnobservedTaskExceptionEventArgs,Version,Void*,VoidTaskResult,WaitCallback,WaitHandle,Win32Native+FILE_TIME,Win32Native+InputRecord,Win32Native+KeyEventRecord,Win32Native+WIN32_FILE_ATTRIBUTE_DATA,WindowsRuntimeResourceManagerBase,WinRTSynchronizationContextFactoryBase";
        static HashSet<string> SupportedTypesHashSet;

        static RemovalOfUnsupportedTypesFromCorlib()
        {
            SupportedTypesHashSet = new HashSet<string>(SupportedTypesTakenFromJsilBootstrapLibraries.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries));
            foreach (string item in AdditionalSupportedTypes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                SupportedTypesHashSet.Add(item.Trim());   
        }

        public static bool IsTypeSupported(string typeName)
        {
            return (SupportedTypesHashSet.Contains(typeName));
        }
    }
}
