using Microsoft.CSharp.RuntimeBinder;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Versioning;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Permissions;
using System.Security;
//using System.Xml;
//using System.Xml.Serialization;




[assembly: TypeForwardedToAttribute(typeof(Action))]
[assembly: TypeForwardedToAttribute(typeof(Action<>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,,,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,,,,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,,,,,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,,,,,,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,,,,,,,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Action<,,,,,,,,,,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Activator))]
[assembly: TypeForwardedToAttribute(typeof(AggregateException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(AmbiguousMatchException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(AppDomain))] //NewInBridgeNotInJSIL
[assembly: TypeForwardedToAttribute(typeof(ApplicationException))] //NewInBridgeNotInJSIL
[assembly: TypeForwardedToAttribute(typeof(ArgumentException))]
[assembly: TypeForwardedToAttribute(typeof(ArgumentNullException))]
[assembly: TypeForwardedToAttribute(typeof(ArgumentOutOfRangeException))]
[assembly: TypeForwardedToAttribute(typeof(ArithmeticException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Array))]
//[assembly: TypeForwardedToAttribute(typeof(ArrayExtensions))]  // Generation Added //Error message "a type forwarder is specified for it"
[assembly: TypeForwardedToAttribute(typeof(ArrayList))]
[assembly: TypeForwardedToAttribute(typeof(ArraySegment<>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ArrayTypeMismatchException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ASCIIEncoding))]
[assembly: TypeForwardedToAttribute(typeof(Assembly))]
[assembly: TypeForwardedToAttribute(typeof(AssemblyCompanyAttribute))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(AssemblyConfigurationAttribute))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(AssemblyCopyrightAttribute))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(AssemblyCultureAttribute))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(AssemblyDescriptionAttribute))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(AssemblyFileVersionAttribute))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(AssemblyInformationalVersionAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(AssemblyName))]
[assembly: TypeForwardedToAttribute(typeof(AssemblyProductAttribute))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(AssemblyTitleAttribute))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(AssemblyTrademarkAttribute))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(AssemblyVersionAttribute))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(AssociationAttribute))]
[assembly: TypeForwardedToAttribute(typeof(AsyncCallback))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(AsyncTaskMethodBuilder))]
[assembly: TypeForwardedToAttribute(typeof(AsyncTaskMethodBuilder<>))]
[assembly: TypeForwardedToAttribute(typeof(AsyncVoidMethodBuilder))]
[assembly: TypeForwardedToAttribute(typeof(Attribute))]
[assembly: TypeForwardedToAttribute(typeof(AttributeTargets))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(AttributeUsageAttribute))] //AddedForXamlCompilerSupport
[assembly: TypeForwardedToAttribute(typeof(Base64FormattingOptions))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(BestFitMappingAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(BinaryExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(BinaryReader))]
[assembly: TypeForwardedToAttribute(typeof(BinaryWriter))]
//[assembly: TypeForwardedToAttribute(typeof(Binder))] //removed, I think it is the global::Microsoft.CSharp.RuntimeBinder.Binder))] below
[assembly: TypeForwardedToAttribute(typeof(global::Microsoft.CSharp.RuntimeBinder.Binder))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(BindingFlags))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(BitArray))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(BitConverter))]
[assembly: TypeForwardedToAttribute(typeof(BlockExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Boolean))]
[assembly: TypeForwardedToAttribute(typeof(BrowsableAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(Buffer))]
[assembly: TypeForwardedToAttribute(typeof(BufferedStream))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Byte))]
[assembly: TypeForwardedToAttribute(typeof(Calendar))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CalendarAlgorithmType))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CalendarWeekRule))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CallerFilePathAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CallerLineNumberAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CallerMemberNameAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CallingConvention))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CallingConventions))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CallSite))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(CallSite<>))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(CallSiteBinder))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(CancellationToken))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(CancellationTokenRegistration))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CancellationTokenSource))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Capture))] //NewInBridgeNotInJSIL
[assembly: TypeForwardedToAttribute(typeof(CaptureCollection))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CatchBlock))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Char))]
[assembly: TypeForwardedToAttribute(typeof(CharEnumerator))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CharSet))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ClientWebSocket))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ClientWebSocketOptions))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CLSCompliantAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Collection<>))]
[assembly: TypeForwardedToAttribute(typeof(CollectionDataContractAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(ColumnAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CompareAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Comparer<>))]
[assembly: TypeForwardedToAttribute(typeof(Comparison<>))]
[assembly: TypeForwardedToAttribute(typeof(CompilerGeneratedAttribute))] //Added on 2018.11.10 to support "Strings.Designer.cs" (for the .RESX files)
[assembly: TypeForwardedToAttribute(typeof(ComplexTypeAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ComVisibleAttribute))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(ConcurrencyCheckAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ConditionalAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ConditionalExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Console))]
[assembly: TypeForwardedToAttribute(typeof(ConstantExpression))]
[assembly: TypeForwardedToAttribute(typeof(ConstructorInfo))]
[assembly: TypeForwardedToAttribute(typeof(Contract))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractAbbreviatorAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractArgumentValidatorAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractClassAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractClassForAttribute))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(ContractException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractFailedEventArgs))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractFailureKind))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractInvariantMethodAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractNamespaceAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractOptionAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractPublicPropertyNameAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractReferenceAssemblyAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractRuntimeIgnoredAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ContractVerificationAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Convert))]
[assembly: TypeForwardedToAttribute(typeof(Converter<,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CreditCardAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CSharpArgumentInfo))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(CSharpArgumentInfoFlags))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(CSharpBinderFlags))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(CultureInfo))]
[assembly: TypeForwardedToAttribute(typeof(CultureNotFoundException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(CustomValidationAttribute))]
[assembly: TypeForwardedToAttribute(typeof(DatabaseGeneratedAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DatabaseGeneratedOption))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DataContractAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(DataMemberAttribute))]
[assembly: TypeForwardedToAttribute(typeof(DataType))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DataTypeAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DateTime))]
[assembly: TypeForwardedToAttribute(typeof(DateTimeFormatInfo))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(DateTimeKind))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(DateTimeOffset))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DateTimeStyles))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DaylightTime))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DayOfWeek))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(DBNull))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(Debug))]
[assembly: TypeForwardedToAttribute(typeof(Debugger))]
[assembly: TypeForwardedToAttribute(typeof(DebuggerBrowsableAttribute))]
[assembly: TypeForwardedToAttribute(typeof(DebuggerBrowsableState))]
[assembly: TypeForwardedToAttribute(typeof(DebuggerDisplayAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DebuggerHiddenAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DebuggerNonUserCodeAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(DebuggerStepperBoundaryAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DebuggerStepThroughAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(DebuggerTypeProxyAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DebuggerVisualizerAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Decimal))]
//[assembly: TypeForwardedToAttribute(typeof(DecimalConfig))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DecimalConstantAttribute))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(DecimalFormatConfig))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Decoder))]
[assembly: TypeForwardedToAttribute(typeof(DecoderFallbackBuffer))]
[assembly: TypeForwardedToAttribute(typeof(DefaultCharSetAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DefaultDllImportSearchPathsAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DefaultExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DefaultMemberAttribute))] // Added on 2018.11.10
[assembly: TypeForwardedToAttribute(typeof(DefaultValueAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Delegate))]
[assembly: TypeForwardedToAttribute(typeof(Dictionary<,>))]
[assembly: TypeForwardedToAttribute(typeof(DictionaryEntry))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Directory))]
[assembly: TypeForwardedToAttribute(typeof(DirectoryInfo))]
[assembly: TypeForwardedToAttribute(typeof(DisplayAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DisplayColumnAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DisplayFormatAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DivideByZeroException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DllImportAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DllImportSearchPath))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Double))]
//[assembly: TypeForwardedToAttribute(typeof(DummyTypeUsedToAddAttributeToDefaultValueTypeConstructor))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DynamicAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(DynamicExpression))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(DynamicExpressionType))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(DynamicIndexExpression))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(DynamicInvocationExpression))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(DynamicMemberExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(DynamicMetaObject))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(DynamicMetaObjectBinder))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(DynamicObject))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(EditableAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(EditorBrowsableAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(EditorBrowsableState))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(ElementInit))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(EmailAddressAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Encoding))]
[assembly: TypeForwardedToAttribute(typeof(EncodingInfo))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(EndOfStreamException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Enum))]
[assembly: TypeForwardedToAttribute(typeof(EnumDataTypeAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Enumerable))]
//[assembly: TypeForwardedToAttribute(typeof(EnumerableInstance<>))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(Enumerator))]
[assembly: TypeForwardedToAttribute(typeof(EnumMemberAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Environment))]
[assembly: TypeForwardedToAttribute(typeof(EnvironmentVariableTarget))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(EqualityComparer<>))]
[assembly: TypeForwardedToAttribute(typeof(EventArgs))]
[assembly: TypeForwardedToAttribute(typeof(EventHandler))]
[assembly: TypeForwardedToAttribute(typeof(EventHandler<>))]
[assembly: TypeForwardedToAttribute(typeof(EventInfo))]
[assembly: TypeForwardedToAttribute(typeof(Exception))]
[assembly: TypeForwardedToAttribute(typeof(Expression))]
[assembly: TypeForwardedToAttribute(typeof(Expression<>))]
[assembly: TypeForwardedToAttribute(typeof(ExpressionType))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(ExtensionAttribute))] //AddedForXamlCompilerSupport
[assembly: TypeForwardedToAttribute(typeof(ExternalException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(FieldInfo))]
[assembly: TypeForwardedToAttribute(typeof(FieldOffsetAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(File))]
[assembly: TypeForwardedToAttribute(typeof(FileExtensionsAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(FileInfo))]
[assembly: TypeForwardedToAttribute(typeof(FileMode))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(FileNotFoundException))]
[assembly: TypeForwardedToAttribute(typeof(FileOptions))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(FileShare))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(FileStream))]
[assembly: TypeForwardedToAttribute(typeof(FileSystemInfo))]
[assembly: TypeForwardedToAttribute(typeof(FilterUIHintAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(FlagsAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(ForeignKeyAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(FormatException))]
//[assembly: TypeForwardedToAttribute(typeof(FormattableString))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(FormattableStringFactory))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Func<>))]
[assembly: TypeForwardedToAttribute(typeof(Func<,>))]
[assembly: TypeForwardedToAttribute(typeof(Func<,,>))]
[assembly: TypeForwardedToAttribute(typeof(Func<,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,,,,,,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,,,,,,,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,,,,,,,,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,,,,,,,,,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,,,,,,,,,,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,,,,,,,,,,,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,,,,,,,,,,,,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Func<,,,,,,,,,,,,,,,,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(GC))]
[assembly: TypeForwardedToAttribute(typeof(GCHandle))]
[assembly: TypeForwardedToAttribute(typeof(GetMemberBinder))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(GotoExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(GotoExpressionKind))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Group))]
[assembly: TypeForwardedToAttribute(typeof(GroupCollection))]
//[assembly: TypeForwardedToAttribute(typeof(Grouping<,>))]  // Generation Added //Error message "a type forwarder is specified for it"
[assembly: TypeForwardedToAttribute(typeof(Guid))]
[assembly: TypeForwardedToAttribute(typeof(GuidAttribute))] //AddedForXamlCompilerSupport
[assembly: TypeForwardedToAttribute(typeof(HandleRef))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(HashSet<>))] //NewInBridgeNotInJSIL
[assembly: TypeForwardedToAttribute(typeof(Hashtable))]
[assembly: TypeForwardedToAttribute(typeof(HostProtectionAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(HostProtectionResource))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IAsyncResult))]
[assembly: TypeForwardedToAttribute(typeof(IAsyncStateMachine))] //NewInBridgeNotInJSIL
[assembly: TypeForwardedToAttribute(typeof(ICloneable))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ICollection))]
[assembly: TypeForwardedToAttribute(typeof(ICollection<>))]
[assembly: TypeForwardedToAttribute(typeof(IComparable))]
[assembly: TypeForwardedToAttribute(typeof(IComparable<>))]
[assembly: TypeForwardedToAttribute(typeof(IComparer))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(IComparer<>))]
[assembly: TypeForwardedToAttribute(typeof(IConvertible))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(ICriticalNotifyCompletion))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(ICustomAttributeProvider))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IDeserializationCallback))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IDictionary))]
[assembly: TypeForwardedToAttribute(typeof(IDictionary<,>))]
[assembly: TypeForwardedToAttribute(typeof(IDictionaryEnumerator))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(IDisposable))]
[assembly: TypeForwardedToAttribute(typeof(IDynamicMetaObjectProvider))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(IEnumerable))]
[assembly: TypeForwardedToAttribute(typeof(IEnumerable<>))]
[assembly: TypeForwardedToAttribute(typeof(IEnumerator))]
[assembly: TypeForwardedToAttribute(typeof(IEnumerator<>))]
[assembly: TypeForwardedToAttribute(typeof(IEqualityComparer))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IEqualityComparer<>))]
[assembly: TypeForwardedToAttribute(typeof(IEquatable<>))]
[assembly: TypeForwardedToAttribute(typeof(IFormatProvider))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(IFormattable))] //Added on 2018.11.15 for #828
[assembly: TypeForwardedToAttribute(typeof(IFormatterConverter))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IgnoreDataMemberAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IGrouping<,>))] //NewInBridgeNotInJSIL
[assembly: TypeForwardedToAttribute(typeof(IList))]
[assembly: TypeForwardedToAttribute(typeof(IList<>))]
[assembly: TypeForwardedToAttribute(typeof(ILookup<,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(InAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IndexerNameAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IndexExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IndexOutOfRangeException))]
[assembly: TypeForwardedToAttribute(typeof(INotifyCompletion))]
[assembly: TypeForwardedToAttribute(typeof(INotifyPropertyChanged))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(Int16))]
[assembly: TypeForwardedToAttribute(typeof(Int32))]
[assembly: TypeForwardedToAttribute(typeof(Int64))]
[assembly: TypeForwardedToAttribute(typeof(Interlocked))]
[assembly: TypeForwardedToAttribute(typeof(InternalsVisibleToAttribute))] //Added
[assembly: TypeForwardedToAttribute(typeof(IntPtr))]
[assembly: TypeForwardedToAttribute(typeof(InvalidCastException))]
[assembly: TypeForwardedToAttribute(typeof(InvalidDataContractException))]
[assembly: TypeForwardedToAttribute(typeof(InvalidFilterCriteriaException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(InvalidOperationException))]
[assembly: TypeForwardedToAttribute(typeof(InvalidProgramException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(InversePropertyAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(InvocationExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(InvokeMemberBinder))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(IObjectReference))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IOException))] //NewInBridgeNotInJSIL
[assembly: TypeForwardedToAttribute(typeof(IOrderedEnumerable<>))] //NewInBridgeNotInJSIL
//[assembly: TypeForwardedToAttribute(typeof(IPromise))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IReadOnlyCollection<>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IReadOnlyDictionary<,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IReadOnlyList<>))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(IReflect))]
[assembly: TypeForwardedToAttribute(typeof(IResourceReader))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ISafeSerializationData))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ISerializable))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(ISerializationSurrogateProvider))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IServiceProvider))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ISet<>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IsolatedStorageFile))]
[assembly: TypeForwardedToAttribute(typeof(IStructuralComparable))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IStructuralEquatable))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IsVolatile))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(IValidatableObject))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(KeyAttribute))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(KeyCollection))]
[assembly: TypeForwardedToAttribute(typeof(KeyNotFoundException))]
[assembly: TypeForwardedToAttribute(typeof(KeyValuePair<,>))]
[assembly: TypeForwardedToAttribute(typeof(KnownTypeAttribute))]
[assembly: TypeForwardedToAttribute(typeof(LabelExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(LabelTarget))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(LambdaExpression))]
[assembly: TypeForwardedToAttribute(typeof(LayoutKind))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(LinkedList<>))]
[assembly: TypeForwardedToAttribute(typeof(LinkedListNode<>))]
[assembly: TypeForwardedToAttribute(typeof(List<>))]
[assembly: TypeForwardedToAttribute(typeof(ListInitExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Lookup<,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(LoopExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Marshal))]
[assembly: TypeForwardedToAttribute(typeof(Match))]
[assembly: TypeForwardedToAttribute(typeof(MatchCollection))]
[assembly: TypeForwardedToAttribute(typeof(MatchEvaluator))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Math))]
[assembly: TypeForwardedToAttribute(typeof(MaxLengthAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MemberAssignment))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MemberBinding))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MemberBindingType))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MemberExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MemberInfo))]
[assembly: TypeForwardedToAttribute(typeof(MemberInitExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MemberListBinding))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MemberMemberBinding))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MemberTypes))]
[assembly: TypeForwardedToAttribute(typeof(MemoryStream))]
[assembly: TypeForwardedToAttribute(typeof(MethodBase))]
[assembly: TypeForwardedToAttribute(typeof(MethodCallExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MethodCodeType))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MethodImplAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MethodImplAttributes))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MethodImplOptions))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MethodInfo))]
[assembly: TypeForwardedToAttribute(typeof(MidpointRounding))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MinLengthAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MissingManifestResourceException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MissingMethodException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(MissingSatelliteAssemblyException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Module))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Monitor))]
[assembly: TypeForwardedToAttribute(typeof(MulticastDelegate))]
[assembly: TypeForwardedToAttribute(typeof(NeutralResourcesLanguageAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(NewArrayExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(NewExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(NonSerializedAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(NotImplementedException))]
[assembly: TypeForwardedToAttribute(typeof(NotMappedAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(NotSupportedException))]
[assembly: TypeForwardedToAttribute(typeof(Nullable))]
[assembly: TypeForwardedToAttribute(typeof(Nullable<>))]
[assembly: TypeForwardedToAttribute(typeof(NullReferenceException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(NumberFormatInfo))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(Object))]
[assembly: TypeForwardedToAttribute(typeof(ObjectDisposedException))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(ObjectExtensions))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ObsoleteAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(OnDeserializedAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(OnDeserializingAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(OnSerializedAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(OnSerializingAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(OperationCanceledException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(OptionalFieldAttribute))] //Added on 2018.11.01 to support WCF with Bridge
//[assembly: TypeForwardedToAttribute(typeof(OrderedEnumerable<>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(OutAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(OutOfMemoryException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(OverflowException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ParamArrayAttribute))] //NewInBridgeNotInJSIL
[assembly: TypeForwardedToAttribute(typeof(ParameterExpression))]
[assembly: TypeForwardedToAttribute(typeof(ParameterInfo))]
[assembly: TypeForwardedToAttribute(typeof(ParameterModifier))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Path))]
[assembly: TypeForwardedToAttribute(typeof(PhoneAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Predicate<>))]
//[assembly: TypeForwardedToAttribute(typeof(PromiseException))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(PromiseExtensions))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(PropertyChangedEventArgs))] //RequiredToRun //
[assembly: TypeForwardedToAttribute(typeof(PropertyChangedEventHandler))] //RequiredToRun //
[assembly: TypeForwardedToAttribute(typeof(PropertyInfo))]
[assembly: TypeForwardedToAttribute(typeof(PureAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Queue<>))]
[assembly: TypeForwardedToAttribute(typeof(Random))]
[assembly: TypeForwardedToAttribute(typeof(RangeAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(RankException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ReadOnlyCollection<>))]
[assembly: TypeForwardedToAttribute(typeof(Regex))]
[assembly: TypeForwardedToAttribute(typeof(RegexMatchTimeoutException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(RegexOptions))]
[assembly: TypeForwardedToAttribute(typeof(RegularExpressionAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(RequiredAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ResourceManager))]
[assembly: TypeForwardedToAttribute(typeof(ResourceSet))]
[assembly: TypeForwardedToAttribute(typeof(RuntimeFieldHandle))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(RuntimeHelpers))]
[assembly: TypeForwardedToAttribute(typeof(RuntimeMethodHandle))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(RuntimeTypeHandle))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(SatelliteContractVersionAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SByte))]
[assembly: TypeForwardedToAttribute(typeof(ScaffoldColumnAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SecurityAction))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SecurityCriticalAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SecurityCriticalScope))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SecurityException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SecuritySafeCriticalAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SeekOrigin))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SendOrPostCallback))]
[assembly: TypeForwardedToAttribute(typeof(SerializableAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(SerializationEntry))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SerializationException))]
[assembly: TypeForwardedToAttribute(typeof(SerializationInfoEnumerator))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SetMemberBinder))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(Single))]
[assembly: TypeForwardedToAttribute(typeof(SortVersion))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Stack<>))]
[assembly: TypeForwardedToAttribute(typeof(StackFrame))]
[assembly: TypeForwardedToAttribute(typeof(StackTrace))]
[assembly: TypeForwardedToAttribute(typeof(Stopwatch))]
[assembly: TypeForwardedToAttribute(typeof(Stream))]
[assembly: TypeForwardedToAttribute(typeof(StreamingContext))]
[assembly: TypeForwardedToAttribute(typeof(StreamingContextStates))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(StreamReader))]
[assembly: TypeForwardedToAttribute(typeof(StreamWriter))]
[assembly: TypeForwardedToAttribute(typeof(String))]
[assembly: TypeForwardedToAttribute(typeof(StringBuilder))]
[assembly: TypeForwardedToAttribute(typeof(StringComparer))]
[assembly: TypeForwardedToAttribute(typeof(StringComparison))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(StringLengthAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(StringReader))]
[assembly: TypeForwardedToAttribute(typeof(StringSplitOptions))] //NewInBridgeNotInJSIL
[assembly: TypeForwardedToAttribute(typeof(StringWriter))]
[assembly: TypeForwardedToAttribute(typeof(StructLayoutAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SuppressMessageAttribute))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(SwitchCase))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SwitchExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(SystemException))]
[assembly: TypeForwardedToAttribute(typeof(TableAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(TargetFrameworkAttribute))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(Task))]
[assembly: TypeForwardedToAttribute(typeof(Task<>))]
[assembly: TypeForwardedToAttribute(typeof(TaskAwaiter))]
[assembly: TypeForwardedToAttribute(typeof(TaskAwaiter<>))]
[assembly: TypeForwardedToAttribute(typeof(TaskCanceledException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(TaskCompletionSource<>))]
[assembly: TypeForwardedToAttribute(typeof(TaskFactory))]
[assembly: TypeForwardedToAttribute(typeof(TaskFactory<>))]
[assembly: TypeForwardedToAttribute(typeof(TaskSchedulerException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(TaskStatus))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(TextInfo))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(TextReader))]
[assembly: TypeForwardedToAttribute(typeof(TextWriter))]
[assembly: TypeForwardedToAttribute(typeof(Thread))]
[assembly: TypeForwardedToAttribute(typeof(ThreadStaticAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Timeout))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(TimeoutException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Timer))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(TimerCallback))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(TimeSpan))]
[assembly: TypeForwardedToAttribute(typeof(TimestampAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Trace))]
[assembly: TypeForwardedToAttribute(typeof(TryExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Tuple))]
[assembly: TypeForwardedToAttribute(typeof(Tuple<>))]
[assembly: TypeForwardedToAttribute(typeof(Tuple<,>))]
[assembly: TypeForwardedToAttribute(typeof(Tuple<,,>))]
[assembly: TypeForwardedToAttribute(typeof(Tuple<,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Tuple<,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Tuple<,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Tuple<,,,,,,>))]
[assembly: TypeForwardedToAttribute(typeof(Tuple<,,,,,,,>))]
//[assembly: TypeForwardedToAttribute(typeof(TupleElementNamesAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(Type))]
[assembly: TypeForwardedToAttribute(typeof(TypeAttributes))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(TypeBinaryExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(TypeCode))] //RequiredToRun
[assembly: TypeForwardedToAttribute(typeof(TypeFilter))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(TypeForwardedFromAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(UIHintAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(UInt16))]
[assembly: TypeForwardedToAttribute(typeof(UInt32))]
[assembly: TypeForwardedToAttribute(typeof(UInt64))]
[assembly: TypeForwardedToAttribute(typeof(UIntPtr))]
[assembly: TypeForwardedToAttribute(typeof(UltimateResourceFallbackLocation))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(UnaryExpression))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(UnauthorizedAccessException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(UnhandledExceptionEventArgs))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(UnhandledExceptionEventHandler))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(UnicodeCategory))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(UnicodeEncoding))]
[assembly: TypeForwardedToAttribute(typeof(Uri))] //NewInBridgeNotInJSIL
[assembly: TypeForwardedToAttribute(typeof(UriKind))] //NewInBridgeNotInJSIL, Added to Bridge by Userware
[assembly: TypeForwardedToAttribute(typeof(UrlAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(UTF32Encoding))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(UTF7Encoding))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(UTF8Decoder))]
[assembly: TypeForwardedToAttribute(typeof(UTF8Encoding))]
[assembly: TypeForwardedToAttribute(typeof(ValidationAttribute))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ValidationContext))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ValidationException))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ValidationResult))] //AddedForSimulatorSupport
[assembly: TypeForwardedToAttribute(typeof(Validator))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(ValueCollection))]
//[assembly: TypeForwardedToAttribute(typeof(ValueTuple))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(ValueTuple<>))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(ValueTuple<,>))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(ValueTuple<,,>))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(ValueTuple<,,,>))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(ValueTuple<,,,,>))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(ValueTuple<,,,,,>))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(ValueTuple<,,,,,,>))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(ValueTuple<,,,,,,,>))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(ValueType))] //NewInBridgeNotInJSIL
[assembly: TypeForwardedToAttribute(typeof(Version))]
[assembly: TypeForwardedToAttribute(typeof(void))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(WaitHandle))] //RequiredToRun //
[assembly: TypeForwardedToAttribute(typeof(WeakReference))]
[assembly: TypeForwardedToAttribute(typeof(WebSocketCloseStatus))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(WebSocketMessageType))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(WebSocketReceiveResult))] // Generation Added
[assembly: TypeForwardedToAttribute(typeof(WebSocketState))] // Generation Added
//[assembly: TypeForwardedToAttribute(typeof(XmlConvert))]
//[assembly: TypeForwardedToAttribute(typeof(XmlNameTable))]
//[assembly: TypeForwardedToAttribute(typeof(XmlQualifiedName))]
//[assembly: TypeForwardedToAttribute(typeof(XmlReader))]
//[assembly: TypeForwardedToAttribute(typeof(XmlSerializationReader))]
//[assembly: TypeForwardedToAttribute(typeof(XmlSerializer))]
//[assembly: TypeForwardedToAttribute(typeof(XmlWriter))]
