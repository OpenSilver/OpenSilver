
/*===================================================================================
* 
*   Copyright (c) Userware (OpenSilver.net, CSHTML5.com)
*      
*   This file is part of both the OpenSilver Compiler (https://opensilver.net), which
*   is licensed under the MIT license (https://opensource.org/licenses/MIT), and the
*   CSHTML5 Compiler (http://cshtml5.com), which is dual-licensed (MIT + commercial).
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OpenSilver.Compiler
{
    internal static class FixingServiceReferencesVB
    {
        enum MethodType
        {
            AsyncWithReturnType,
            AsyncWithoutReturnType,
            NotAsyncWithReturnType,
            NotAsyncWithoutReturnType,
            AsyncEndWithReturnType,
            AsyncEndWithoutReturnType,
            AsyncBegin
        }

        // this struct is used to find a given substring in a text when reading it character by character 
        // (in this class, we use it to find the substring "base.Channel" efficiently and the interface type in System.ServiceModel.ClientBase<...>).
        internal interface ISubstringFinderCharByChar
        {
            bool IsMatch { get; }

            void Reset();

            void MoveNext(char c);

            string Match { get; }
        }

        internal class SubstringFinderCharByChar : ISubstringFinderCharByChar
        {
            private readonly string _subString;
            private int _position;
            private bool _isMatch;

            public SubstringFinderCharByChar(string subString)
            {
                _subString = subString;
                _position = 0;
                _isMatch = false;
            }

            public bool IsMatch => _isMatch;

            public string Match => _isMatch ? _subString : throw new InvalidOperationException();

            public void Reset()
            {
                _position = 0;
                _isMatch = false;
            }

            public void MoveNext(char c)
            {
                if (_isMatch)
                {
                    return;
                }

                if (_subString[_position] == c)
                {
                    if (++_position == _subString.Length)
                    {
                        _isMatch = true;
                    }
                }
                else
                {
                    Reset();
                }
            }
        }

        internal class ArrayOfSubstringFinderCharByChar : ISubstringFinderCharByChar
        {
            private readonly ISubstringFinderCharByChar[] _subStringFinders;
            private int _matchIndex = -1;
            public ArrayOfSubstringFinderCharByChar(IList<string> subStrings)
            {
                _subStringFinders = new ISubstringFinderCharByChar[subStrings.Count];
                for (int i = 0; i < subStrings.Count; i++)
                {
                    _subStringFinders[i] = new SubstringFinderCharByChar(subStrings[i]);
                }
            }

            public bool IsMatch => _matchIndex > -1;

            public string Match => IsMatch ? _subStringFinders[_matchIndex].Match : throw new InvalidOperationException();

            public void Reset()
            {
                for (int i = 0; i < _subStringFinders.Length; i++)
                {
                    _subStringFinders[i].Reset();
                }

                _matchIndex = -1;
            }

            public void MoveNext(char c)
            {
                if (IsMatch)
                {
                    return;
                }

                for (int i = 0; i < _subStringFinders.Length; i++)
                {
                    ISubstringFinderCharByChar finder = _subStringFinders[i];
                    finder.MoveNext(c);
                    if (finder.IsMatch)
                    {
                        _matchIndex = i;
                        break;
                    }
                }
            }
        }

        private static void CheckBracket(string line, ref bool isInClass, ref bool isInheritsLine, ref bool isInSubOrFunction)
        {
            if (line.Contains("Class "))
                isInClass = true;

            if (line.Contains("End Class"))
                isInClass = false;

            if (isInClass && line.Contains("Inherits "))
                isInheritsLine = true;
            else
                isInheritsLine = false;

            if (isInClass && 
                (line.Contains("Sub ") || line.Contains("Function ")))
                isInSubOrFunction = true;

            if (isInSubOrFunction && 
                (line.Contains("End Sub") || line.Contains("End Function")))
                isInSubOrFunction = false;
        }

        private static readonly Dictionary<string, string> SupportedClientBaseTypes = 
            new Dictionary<string, string>()
            {
                ["System.ServiceModel.ClientBase"] = "System.ServiceModel.CSHTML5_ClientBase",
                ["System.ServiceModel.DuplexClientBase"] = "System.ServiceModel.CSHTML5_DuplexClientBase",
            };
        
        internal static string Fix(string inputText, string clientBaseForcedToken, string clientBaseForcedInterfaceName, string endpointCode, string soapVersion, out bool wasAnythingFixed)
        {
            soapVersion = string.IsNullOrEmpty(soapVersion) ? "1.1" : soapVersion;
            string clientBaseToken = string.IsNullOrEmpty(clientBaseForcedToken) ? "MyBase.Channel" : clientBaseForcedToken;
            endpointCode = string.IsNullOrEmpty(endpointCode) ? "Me.INTERNAL_RemoteAddressAsString" : endpointCode;
            // TIP: to read the regex below, start from the "clientBaseToken" part, and go backwards. You can use the website http://regexr.com/ to help read it - Please note that for that website to work perfectly, you need to remove the C#-only capture group names ?<GROUP_NAME>
            Regex regexMethodIdentifier = new Regex(clientBaseToken.Replace(".", @"\.") + @".(?<METHOD_NAME>\w+)\((?<REQUEST_PARAMETER_NAME>(\s*\w*\s*,\s*)*\s*\w*\s*)\)", RegexOptions.Compiled);

            StringBuilder sb = new StringBuilder();
            StringBuilder currentBlock = new StringBuilder();
            StringBuilder currentAttribute = new StringBuilder();

            // used to find the interface in "System.ServiceModel.ClientBase(Of ...)" or
            // "System.ServiceModel.DuplexClientBase(Of ...)" so we don't need to it every
            // time we call FixBlock().
            ISubstringFinderCharByChar clientBaseInterfaceFinder = new ArrayOfSubstringFinderCharByChar(
                SupportedClientBaseTypes.Keys.Select(s => $"{s}(Of").ToArray());

            string clientBaseInterfaceName = "";

            wasAnythingFixed = false;
            bool isCharAppend = false;
            bool isInClass = false;
            bool isInheritsLine = false;
            bool isInSubOrFunction = false;

            string[] lines = inputText.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None);
            foreach(string line in lines)
            {
                if (line.TrimStart().StartsWith("'"))
                {
                    sb.AppendLine(line);
                    continue;
                }

                bool previousIsInClass = isInClass;
                bool previousIsInSubOrFunction = isInSubOrFunction;
                CheckBracket(line, ref isInClass, ref isInheritsLine, ref isInSubOrFunction);

                if (isInheritsLine)
                {
                    foreach (var pair in SupportedClientBaseTypes)
                    {
                        if (line.Contains(pair.Key + "(Of "))
                        {
                            int start = line.IndexOf(pair.Key + "(Of ") + pair.Key.Length + 4;
                            int end = line.LastIndexOf(")");
                            clientBaseInterfaceName = line.Substring(start, end - start);
                        }
                    }
                }

                isCharAppend = false;
                if (!isInSubOrFunction)
                {
                    if (previousIsInSubOrFunction)
                    {
                        currentBlock.AppendLine(line);
                        string currentBlockAsString = currentBlock.ToString();
                        if (currentBlockAsString.Contains(clientBaseForcedToken))
                        {
                            if (FixBlock(
                                ref currentBlockAsString,
                                string.IsNullOrEmpty(clientBaseForcedInterfaceName) ? clientBaseInterfaceName :
                                                                                        clientBaseForcedInterfaceName,
                                inputText,
                                regexMethodIdentifier,
                                clientBaseToken,
                                endpointCode,
                                soapVersion))
                            {
                                wasAnythingFixed = true;
                            }
                        }
                        sb.Append(currentBlockAsString);
                        currentBlock.Clear();
                        isCharAppend = true;
                    }
                }
                else
                {
                    currentBlock.AppendLine(line);
                    isCharAppend = true;
                }

                if (!isCharAppend)
                {
                    sb.AppendLine(line);
                }
            }

            string finalText = sb.ToString();

            //extract the ServiceKnownTypesAttribute that are assigned to the Methods of the interfaces, to assign them to the interface itself (our version of JSIL is unable to get the custom Attributes from interfaces' methods, but not from the interface's type itself.
            ExtractServiceKnownTypeAttributesToInterfaceItself(ref finalText);

            // Replace the inheritance to ClientBase with an inheritance to CSHTML5_ClientBase 
            // and DuplexClientBase with an inheritance to CSHTML5_DuplexClientBase
            foreach (var pair in SupportedClientBaseTypes)
            {
                finalText = finalText.Replace($"{pair.Key}(", $"{pair.Value}(");
            }

            finalText = FixBasicHttpBinding(finalText);

            return finalText;
        }

        private static string FixBasicHttpBinding(string text)
        {
            return string.Join(Environment.NewLine,
                //\r\n for non-Unix platforms, or \n for Unix platforms
                text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None).SelectMany(l =>
                {
                    var res = new List<string>();
                    if (l.Contains("New System.ServiceModel.BasicHttpBinding()"))
                    {
                        res.Add("#If OPENSILVER Then");
                        res.Add("Return New System.ServiceModel.Channels.CustomBinding()");
                        res.Add("#End If");
                    }

                    res.Add(l);

                    return res;
                }));
        }

        private static bool FixBlock(ref string block, string interfaceType, /*int indexOfBeginningOfBlock,*/string inputText, Regex regex, string detectedToken, string endpointCode, string soapVersion)
        {
            string[] lines = block.Trim().Split(new[] { "\r\n", "\n"}, StringSplitOptions.None);
            if (lines.Length < 2)
            {
                return false;
            }

            int definitionLineCount = 0;
            string functionDefinition = "";
            while (true)
            {
                if (definitionLineCount > 0)
                    functionDefinition += lines[definitionLineCount].Trim();
                else
                    functionDefinition += lines[definitionLineCount];

                definitionLineCount++;

                // It can have multiple lines for variables.
                if (functionDefinition.EndsWith("_"))
                {
                    functionDefinition = functionDefinition.Remove(functionDefinition.Length - 1);
                }
                else
                    break;
            }

            string implementsDefinition = "";
            if (functionDefinition.Contains("Implements"))
            {
                int index = functionDefinition.IndexOf("Implements");
                implementsDefinition = functionDefinition.Substring(index + 11);
                functionDefinition = functionDefinition.Substring(0, index).Trim();
            }

            string pattern = @"(Function|Sub)\s+(?<METHOD_NAME>\w+)\((?<METHOD_PARAMETERS_DEFINITIONS>.*?)\)(\s+As\s+(?<RETURN_TYPE>[\w\.\(\)\s]+))?$";
            Match match = Regex.Match(functionDefinition, pattern);

            bool wasAnythingFixed = false;
            if (match.Success)
            {
                string returnType = match.Groups["RETURN_TYPE"].Value;
                string methodParametersDefinitions = match.Groups["METHOD_PARAMETERS_DEFINITIONS"].Value;
                string methodName = match.Groups["METHOD_NAME"].Value;

                if (implementsDefinition.Length > 0)
                {
                    string[] splits = implementsDefinition.Split(".".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    methodName = splits[splits.Length - 1];
                }

                string methodBodyToReplace = string.Join("\r\n", lines, definitionLineCount, lines.Length - definitionLineCount - 1);
                Match matchBody = regex.Match(methodBodyToReplace);
                string requestParameterName = matchBody.Groups["REQUEST_PARAMETER_NAME"].Value;

                if (!matchBody.Success)
                    return false;

                // Check the method type:
                MethodType methodType;
                if (returnType == "System.IAsyncResult")
                {
                    methodType = MethodType.AsyncBegin;
                }
                else if (requestParameterName.Contains("result")) // AsyncEnd
                {
                    if (returnType == "")
                        methodType = MethodType.AsyncEndWithoutReturnType;
                    else
                        methodType = MethodType.AsyncEndWithReturnType;
                }
                else if (returnType.Contains("Task(") && returnType.Contains(")"))
                {
                    methodType = MethodType.AsyncWithReturnType;
                }
                else if (returnType == "Task" || returnType == "System.Threading.Tasks.Task") //Note: I don't know if we can meet something like "Threading.Tasks.Task" or other variant, I don't expect we can.
                {
                    methodType = MethodType.AsyncWithoutReturnType;
                }
                else if (returnType == "")
                {
                    methodType = MethodType.NotAsyncWithoutReturnType;
                }
                else
                {
                    methodType = MethodType.NotAsyncWithReturnType;
                }

                // Check if the method is async:
                if (methodType == MethodType.AsyncWithReturnType)
                {
                    int indexOfLowerThan = returnType.IndexOf('(');
                    int indexOfGreaterThan = returnType.LastIndexOf(')');
                    returnType = returnType.Substring(indexOfLowerThan + 1, indexOfGreaterThan - indexOfLowerThan - 1);

                    if (returnType.StartsWith("Of"))
                    {
                        returnType = returnType.Substring(2);
                    }
                }

                string originalCode;
                bool thereAreParameters = true;
                if (string.IsNullOrWhiteSpace(requestParameterName))
                {
                    thereAreParameters = false;
                    requestParameterName = "Nothing";
                    originalCode = string.Format("{0}.{1}()", 
                                                 detectedToken, 
                                                 methodName);
                }
                else
                {
                    originalCode = string.Format("{0}.{1}({2})", 
                                                 detectedToken, 
                                                 methodName, 
                                                 requestParameterName);
                }

                //Make a dictionary to know the parameters from the name:
                //todo-perf: this and the "check the amount of parameters and adapt the body replacement:" part may be a bit redundant since they're both about the parameters of the method so we might be able to make the compilation slightly faster by changing this (probably unnoticeable).
                Dictionary<string, string> parameterNamesToTheirDefinitions = new Dictionary<string, string>();
                Dictionary<string, string> outParamDefinitions = new Dictionary<string, string>();
                if (!string.IsNullOrWhiteSpace(methodParametersDefinitions))
                {
                    string[] splittedMethodParametersDefinition = SplitStringTakingAccountOfBrackets(methodParametersDefinitions, ',');
                    foreach (string parameterDefinition in splittedMethodParametersDefinition)
                    {
                        string paramDefinition = parameterDefinition.Trim();
                        bool isOutParam = false;
                        if (paramDefinition.StartsWith("ByRef "))
                        {
                            paramDefinition = paramDefinition.Remove(0, 6).Trim();
                            // isOutParam = true;
                        }
                        else if (paramDefinition.StartsWith("ByVal "))
                        {
                            paramDefinition = paramDefinition.Remove(0, 6).Trim();
                        }

                        string[] splittedParamDefinition = paramDefinition.Split(new string[] { " As " }, StringSplitOptions.RemoveEmptyEntries);
                        //Note on the line above: very improbable scenario that would lead to problems: if the guy writes Paramtype1\r\nParamName (see it as a new line, not the actual characters)
                        string parameterName = splittedParamDefinition[0].Trim(); 
                        string parameterTypeAsString = splittedParamDefinition[1].Trim();
                        
                        if (isOutParam)
                        {
                            outParamDefinitions.Add(parameterName, parameterTypeAsString);
                        }
                        else
                        {
                            parameterNamesToTheirDefinitions.Add(parameterName, string.Format(@"{0}", parameterName));
                        }
                    }
                }
                //check the amount of parameters and adapt the body replacement:
                string[] splittedParameters = requestParameterName.Split(',');
                string newBody = "";
                if (thereAreParameters)
                {
                    string parametersDictionaryDefinition = "New Global.System.Collections.Generic.Dictionary(Of String, Object)() From {";
                    foreach (string paramName in splittedParameters)
                    {
                        string trimmedParamName = paramName.Trim();
                        bool isOutParam = false;
                        if (paramName.StartsWith("ByRef "))
                        {
                            isOutParam = true;
                            trimmedParamName = trimmedParamName.Remove(0, 6).Trim();
                        }

                        if (trimmedParamName == "Nothing") continue;

                        string parameterDefinition = parameterNamesToTheirDefinitions.ContainsKey(trimmedParamName) ?
                                                     parameterNamesToTheirDefinitions[trimmedParamName] :
                                                     "Nothing";

                        // Note: Can the params names be different from trimmedParam and the one 
                        // in parameterNamesToTheirDefinitions? (probably not if properly trimmed and all)
                        if (!isOutParam)
                        {
                            parametersDictionaryDefinition += $"{{ \"{trimmedParamName}\", {parameterDefinition} }},";
                        }
                    }
                    parametersDictionaryDefinition = parametersDictionaryDefinition.TrimEnd(',');
                    parametersDictionaryDefinition += "}";

                    newBody = string.Format(

    @"            {11}
            {6}System.ServiceModel.INTERNAL_WebMethodsCaller.{8}CallWebMethod{0}{7}({1}{2})({9}, ""{3}"", {4}, ""{10}"")",
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType) ? "Async" : string.Empty),
     ((methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? "Of "  + returnType + ", " : "Of "),
     interfaceType,
     GetMethodName(methodName, methodType),
     parametersDictionaryDefinition,
     originalCode,
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? "Return " : ""),
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.NotAsyncWithoutReturnType || methodType == MethodType.AsyncEndWithoutReturnType) ? "_WithoutReturnValue" : ""),
     ((methodType == MethodType.AsyncBegin ? "Begin" : "") + (methodType == MethodType.AsyncEndWithoutReturnType || methodType == MethodType.AsyncEndWithReturnType ? "End" : "")),
     endpointCode,
     soapVersion,
     ""//string.Join(" ", outParamDefinitions.Select(def => $"{def.Key} = default({def.Value})"))
     );
                }
                else //case where there are no parameters
                {

                    // Generate the body replacement:
                    newBody = string.Format(
    @"            {6}System.ServiceModel.INTERNAL_WebMethodsCaller.{8}CallWebMethod{0}{7}({1}{2})({9}, ""{3}"", {4}, ""{10}"")",
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType) ? "Async" : string.Empty),
     ((methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? "Of " + returnType + ", " : "Of "),
     interfaceType,
     GetMethodName(methodName, methodType),
     requestParameterName,
     originalCode,
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? "Return " : ""),
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.NotAsyncWithoutReturnType || methodType == MethodType.AsyncEndWithoutReturnType) ? "_WithoutReturnValue" : ""),
     ((methodType == MethodType.AsyncBegin ? "Begin" : "") + (methodType == MethodType.AsyncEndWithoutReturnType || methodType == MethodType.AsyncEndWithReturnType ? "End" : "")),
     endpointCode,
     soapVersion
     );
                }
                block = block.Replace(methodBodyToReplace, newBody);
                if (methodBodyToReplace != newBody)
                {
                    wasAnythingFixed = true;
                }
            }
            return wasAnythingFixed;
        }

        /// <summary>
        /// Try to find the web method name by trying to extract a substring.
        /// </summary>
        /// <param name="method"></param>
        /// <param name="prefixTokens"></param>
        /// <param name="suffixTokens"></param>
        /// <returns></returns>
        private static string GetMethodNameUnsafe(string method, string[] prefixTokens, string[] suffixTokens)
        {
            foreach (string token in prefixTokens)
            {
                if (method.StartsWith(token))
                {
                    return method.Substring(token.Length);
                }
            }

            foreach (string token in suffixTokens)
            {
                if (method.EndsWith(token))
                {
                    return method.Substring(0, method.Length - token.Length);
                }
            }

            return method;
        }

        private static string GetMethodName(string method, MethodType methodType)
        {
            switch (methodType)
            {
                case MethodType.AsyncBegin:
                    if (!method.StartsWith("Begin"))
                    {
                        return GetMethodNameUnsafe(method, 
                                                   new string[1] { "End" }, 
                                                   new string[1] { "Async" });
                    }
                    return method.Substring(5); // skips "Begin"

                case MethodType.AsyncEndWithoutReturnType:
                case MethodType.AsyncEndWithReturnType:
                    if (!method.StartsWith("End"))
                    {
                        return GetMethodNameUnsafe(method,
                                                   new string[1] { "Begin" },
                                                   new string[1] { "Async" });
                    }
                    return method.Substring(3); // skips "End"
                
                case MethodType.AsyncWithoutReturnType:
                case MethodType.AsyncWithReturnType:
                    if (!method.EndsWith("Async"))
                    {
                        return GetMethodNameUnsafe(method, 
                                                   new string[2] { "Begin", "End" }, 
                                                   new string[0]);
                    }
                    return method.Substring(0, method.Length - 5); // skips "Async"

                case MethodType.NotAsyncWithoutReturnType:
                case MethodType.NotAsyncWithReturnType:
                    return method;

                default:
                    throw new InvalidOperationException(
                        string.Format("unexpected method type : '{0}'.", methodType));
            }
        }

        private static string[] SplitStringTakingAccountOfBrackets(string parameters, char separator)
        {
            List<string> splittedParameters = new List<string>();
            int openedBrackets = 0;
            string currentParameter = "";
            foreach (char c in parameters)
            {
                if (c == '(')
                {
                    openedBrackets++;
                    currentParameter += c;
                }
                else if (c == ')')
                {
                    openedBrackets--;
                    currentParameter += c;
                }
                else if (c == separator)
                {
                    if (openedBrackets == 0)
                    {
                        splittedParameters.Add(currentParameter);
                        currentParameter = "";
                    }
                    else
                    {
                        currentParameter += c;
                    }
                }
                else
                {
                    currentParameter += c;
                }
            }
            if (!string.IsNullOrWhiteSpace(currentParameter))
            {
                splittedParameters.Add(currentParameter);
            }
            return splittedParameters.ToArray();
        }

        private static void ExtractServiceKnownTypeAttributesToInterfaceItself(ref string inputText)
        {
            // TODO TEST
            // Regex regex = new Regex(@"Public Interface\s+(\w+)\s*[\w\W]*?End Interface");
        }
    }
}
