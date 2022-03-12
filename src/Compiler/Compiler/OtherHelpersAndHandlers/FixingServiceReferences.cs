

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
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DotNetForHtml5.Compiler
{
    internal static class FixingServiceReferences
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

        private static void UpdateBracketsCount(char currentChar, ref int bracketsCount, ref int squaredBracketsCount)
        {
            if (currentChar == '{')
            {
                bracketsCount++;
            }
            else if (currentChar == '}')
            {
                bracketsCount--;
            }
            else if (currentChar == '[')
            {
                squaredBracketsCount++;
            }
            else if (currentChar == ']')
            {
                squaredBracketsCount--;
            }
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
            string clientBaseToken = string.IsNullOrEmpty(clientBaseForcedToken) ? "base.Channel" : clientBaseForcedToken;
            endpointCode = string.IsNullOrEmpty(endpointCode) ? "this.INTERNAL_RemoteAddressAsString" : endpointCode;
            // TIP: to read the regex below, start from the "clientBaseToken" part, and go backwards. You can use the website http://regexr.com/ to help read it - Please note that for that website to work perfectly, you need to remove the C#-only capture group names ?<GROUP_NAME>
            Regex regexMethodIdentifier = new Regex(@"(?<RETURN_TYPE>([\[\]\w\.><]|\s*,\s*)+)\s+\S+\((?<METHOD_PARAMETERS_DEFINITIONS>[^\)]*)\)\s*{(?<METHOD_BODY_TO_REPLACE>[^\{]*" + clientBaseToken.Replace(".", @"\.") + @".(?<METHOD_NAME>\w+)\((?<REQUEST_PARAMETER_NAME>( *\w*,?)*)\)[^\}]*)}", RegexOptions.Compiled);

            StringBuilder sb = new StringBuilder();
            StringBuilder currentBlock = new StringBuilder();
            StringBuilder currentAttribute = new StringBuilder();
            int numberOfOpenedBrackets = 0;
            int previousNumberOfOpenedBrackets = 0;
            int charIndex = 0;
            int indexOfBeginningOfBlock = 0;
            int numberOfOpenedSquaredBrackets = 0;
            int previousNumberOfOpenedSquaredBrackets = 0;

            // used to find the interface in "System.ServiceModel.ClientBase<...>" or
            // "System.ServiceModel.DuplexClientBase<...>" so we don't need to it every
            // time we call FixBlock().
            ISubstringFinderCharByChar clientBaseInterfaceFinder = new ArrayOfSubstringFinderCharByChar(
                SupportedClientBaseTypes.Keys.Select(s => $"{s}<").ToArray());

            bool isClientBaseInterfaceFound = false;
            string clientBaseInterfaceName = "";

            wasAnythingFixed = false;
            bool isCharAppend = false;

            foreach (char c in inputText)
            {
                previousNumberOfOpenedBrackets = numberOfOpenedBrackets;
                previousNumberOfOpenedSquaredBrackets = numberOfOpenedSquaredBrackets;
                UpdateBracketsCount(c, ref numberOfOpenedBrackets, ref numberOfOpenedSquaredBrackets);

                if (previousNumberOfOpenedSquaredBrackets == 0)
                {
                    if (numberOfOpenedSquaredBrackets == 1)
                    {
                        currentAttribute.Append(c);
                        isCharAppend = true;
                    }
                }
                else if (previousNumberOfOpenedSquaredBrackets == 1)
                {
                    currentAttribute.Append(c);
                    if (numberOfOpenedSquaredBrackets == 0)
                    {
                        if (numberOfOpenedBrackets < 2)
                        {
                            sb.Append(currentAttribute.ToString());
                        }
                        else
                        {
                            currentBlock.Append(currentAttribute.ToString());
                        }
                        currentAttribute.Clear();
                    }
                    isCharAppend = true;
                }
                else if (previousNumberOfOpenedSquaredBrackets == 2 && numberOfOpenedSquaredBrackets == 1)
                {
                    // I believe this can only happen when the service has a method that takes as parameter or returns an array of a type. The type can be custom or string for example, it makes no difference.
                    // The line looks like this: [System.ServiceModel.ServiceKnownTypeAttribute(typeof(TestCshtml5WCF.ServiceReference1.ToDoItem[]))]
                    //                                                                                                           We arrive here: ↑
                    // we add the character to the attribute as is.
                    // Note: we have to do this  because otherwise, the ']' character will be added to currentBlock in this loop, and in the next loop, we will enter the "else if" above, which will put
                    //       currentAttribute in CurrentBlock, thus resulting in something like: "][System.ServiceModel.ServiceKnownTypeAttribute(typeof(TestCshtml5WCF.ServiceReference1.ToDoItem[))]"
                    //                                                                      this: ↑  ---------------------------------------------------------- should be before this parenthesis: ↑
                    currentAttribute.Append(c);
                    isCharAppend = true;
                }
                if (!isCharAppend)
                {
                    // it means we are not in a type yet, so we can add the character safely
                    if (previousNumberOfOpenedBrackets < 2)
                    {
                        // We are not in a type yet, so we try to locate the interface in System.ServiceModel.ClientBase<...>, 
                        if (numberOfOpenedBrackets == 1)
                        {
                            if (clientBaseInterfaceFinder.IsMatch)
                            {
                                if (!isClientBaseInterfaceFound)
                                {
                                    if (c != '>')
                                    {
                                        clientBaseInterfaceName += c;
                                    }
                                    else
                                    {
                                        isClientBaseInterfaceFound = true;
                                    }
                                }
                            }
                            else
                            {
                                clientBaseInterfaceFinder.MoveNext(c);
                            }
                        }
                        // this means we just entered in a type
                        else if (numberOfOpenedBrackets == 2)
                        {
                            indexOfBeginningOfBlock = charIndex + 1;
                        }
                        sb.Append(c);
                    }
                    // this means we are in a class, so we need to identify every block of text containing a method.
                    else if (previousNumberOfOpenedBrackets == 2)
                    {
                        // the type ended, we can append the currentBlock to the finalText and wait to enter the next type.
                        if (numberOfOpenedBrackets == 1)
                        {
                            currentBlock.Append(c);
                            sb.Append(currentBlock.ToString());
                            currentBlock.Clear();
                            // reset ClientBase interface in case it is not the same for every class (todo: make sure it can be the case, otherwise don't reset).
                            clientBaseInterfaceFinder.Reset();
                            clientBaseInterfaceName = "";
                            isClientBaseInterfaceFound = false;
                        }
                        else
                        {
                            // it marks the end of a block of code that we are not interested in (we look for something ending with a '}'), 
                            // so we just append the currentBlock to the finalText and reset the value of currentBlock afterward.
                            // it could be a field, or an event with no body.
                            if (c == ';')
                            {
                                currentBlock.Append(c);
                                sb.AppendLine(currentBlock.ToString());
                                currentBlock.Clear();
                                indexOfBeginningOfBlock = charIndex + 1;
                            }
                            else
                            {
                                currentBlock.Append(c);
                            }
                        }
                    }
                    // here we are inside a method, so we keep add the characters to currentBlock until we reach the end of the method.
                    else if (previousNumberOfOpenedBrackets > 2)
                    {
                        // it means we are not in the method anymore, so we can use the regex on the currentBlock to see if we have a match.
                        if (numberOfOpenedBrackets == 2)
                        {
                            currentBlock.Append(c);
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
                            indexOfBeginningOfBlock = charIndex + 1;
                        }
                        else
                        {
                            currentBlock.Append(c);
                        }
                    }
                }
                charIndex++;
                isCharAppend = false;
            }
            string finalText = sb.ToString();

            //extract the ServiceKnownTypesAttribute that are assigned to the Methods of the interfaces, to assign them to the interface itself (our version of JSIL is unable to get the custom Attributes from interfaces' methods, but not from the interface's type itself.
            ExtractServiceKnownTypeAttributesToInterfaceItself(ref finalText);

            // Replace the inheritance to ClientBase with an inheritance to CSHTML5_ClientBase 
            // and DuplexClientBase with an inheritance to CSHTML5_DuplexClientBase
            foreach (var pair in SupportedClientBaseTypes)
            {
                finalText = finalText.Replace($"{pair.Key}<", $"{pair.Value}<");
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
                    if (l.Contains("new System.ServiceModel.BasicHttpBinding()"))
                    {
                        res.Add("#if OPENSILVER");
                        res.Add("return new System.ServiceModel.Channels.CustomBinding();");
                        res.Add("#endif");
                    }

                    res.Add(l);

                    return res;
                }));
        }

        private static bool FixBlock(ref string block, string interfaceType, /*int indexOfBeginningOfBlock,*/string inputText, Regex regex, string detectedToken, string endpointCode, string soapVersion)
        {
            bool wasAnythingFixed = false;
            Match match = regex.Match(block);
            if (match.Success)
            {
                string returnType = match.Groups["RETURN_TYPE"].Value;
                string methodBodyToReplace = match.Groups["METHOD_BODY_TO_REPLACE"].Value;
                string methodParametersDefinitions = match.Groups["METHOD_PARAMETERS_DEFINITIONS"].Value;
                string methodName = match.Groups["METHOD_NAME"].Value;
                string requestParameterName = match.Groups["REQUEST_PARAMETER_NAME"].Value;

                // Check the method type:
                MethodType methodType;
                if (returnType == "System.IAsyncResult")
                {
                    methodType = MethodType.AsyncBegin;
                }
                else if (requestParameterName.Contains("result")) // AsyncEnd
                {
                    if (returnType == "void")
                        methodType = MethodType.AsyncEndWithoutReturnType;
                    else
                        methodType = MethodType.AsyncEndWithReturnType;
                }
                else if (returnType.Contains("Task<") && returnType.Contains(">"))
                {
                    methodType = MethodType.AsyncWithReturnType;
                }
                else if (returnType == "Task" || returnType == "System.Threading.Tasks.Task") //Note: I don't know if we can meet something like "Threading.Tasks.Task" or other variant, I don't expect we can.
                {
                    methodType = MethodType.AsyncWithoutReturnType;
                }
                else if (returnType == "void")
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
                    int indexOfLowerThan = returnType.IndexOf('<');
                    int indexOfGreaterThan = returnType.LastIndexOf('>');
                    returnType = returnType.Substring(indexOfLowerThan + 1, indexOfGreaterThan - indexOfLowerThan - 1);
                }

                string originalCode;
                bool thereAreParameters = true;
                if (string.IsNullOrWhiteSpace(requestParameterName))
                {
                    thereAreParameters = false;
                    requestParameterName = "null";
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
                Dictionary<string, string> outParamDefinations = new Dictionary<string, string>();
                if (!string.IsNullOrWhiteSpace(methodParametersDefinitions))
                {
                    string[] splittedMethodParametersDefinition = SplitStringTakingAccountOfBrackets(methodParametersDefinitions, ',');
                    foreach (string parameterDefinition in splittedMethodParametersDefinition)
                    {
                        string paramDefinition = parameterDefinition.Trim();
                        bool isOutParam = false;
                        if (paramDefinition.StartsWith("out "))
                        {
                            paramDefinition = paramDefinition.Remove(0, 4);
                            isOutParam = true;
                        }
                        string[] splittedParamDefinition = SplitStringTakingAccountOfBrackets(paramDefinition, ' ');
                        //Note on the line above: very improbable scenario that would lead to problems: if the guy writes Paramtype1\r\nParamName (see it as a new line, not the actual characters)
                        bool isParameterTypeGotten = false;
                        string parameterTypeAsString = null;
                        string parameterName = null;
                        foreach (string paramDefPart in splittedParamDefinition)
                        {
                            if (!string.IsNullOrWhiteSpace(paramDefPart))
                            {
                                if (!isParameterTypeGotten)
                                {
                                    parameterTypeAsString = paramDefPart.Trim();
                                    isParameterTypeGotten = true;
                                }
                                else
                                {
                                    parameterName = paramDefPart.Trim();
                                }
                            }
                        }

                        if (isOutParam)
                        {
                            outParamDefinations.Add(parameterName, parameterTypeAsString);
                        }
                        parameterNamesToTheirDefinitions.Add(parameterName, string.Format(@"{0}", parameterName));
                    }
                }
                //check the amount of parameters and adapt the body replacement:
                string[] splittedParameters = requestParameterName.Split(',');
                string newBody = "";
                if (thereAreParameters)
                {
                    string parametersDictionaryDefinition = "var data = new global::System.Collections.Generic.Dictionary<string, object>() {";
                    bool isFirst = true;
                    foreach (string paramName in splittedParameters)
                    {
                        string trimmedParamName = paramName.Trim();
                        bool isOutParam = false;
                        if (paramName.StartsWith("out "))
                        {
                            isOutParam = true;
                            trimmedParamName = trimmedParamName.Remove(0, 4);
                        }

                        if (trimmedParamName == "null") continue;

                        if (!isFirst)
                        {
                            parametersDictionaryDefinition += ", ";
                        }

                        string parameterDefinition = parameterNamesToTheirDefinitions.ContainsKey(trimmedParamName) ?
                                                     parameterNamesToTheirDefinitions[trimmedParamName] :
                                                     "null";

                        // Note: Can the params names be different from trimmedParam and the one 
                        // in parameterNamesToTheirDefinitions? (probably not if properly trimmed and all)
                        parametersDictionaryDefinition +=
                            string.Format("{{ \"{0}\", {1} }}",
                                          trimmedParamName,
                                          isOutParam ? "null" : parameterDefinition);
                        isFirst = false;
                    }
                    parametersDictionaryDefinition += "};" + Environment.NewLine;

                    string outParamString = String.Empty;
                    if (outParamDefinations.Count > 0)
                    {
                        foreach (var defination in outParamDefinations)
                        {
                            outParamString += String.Format(Environment.NewLine + "{0} = data[\"{0}\"] as {1};", defination.Key, defination.Value);
                        }
                    }

                    string bodyFormat = @"
            {4}{6}System.ServiceModel.INTERNAL_WebMethodsCaller.{8}CallWebMethod{0}{7}<{1}{2}>({9}, ""{3}"", data, ""{10}"");{11}"; 
                    if ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType)) 
                    {
                        bodyFormat = @"
            {4}            var webMethodResult = System.ServiceModel.INTERNAL_WebMethodsCaller.{8}CallWebMethod{0}{7}<{1}{2}>({9}, ""{3}"", data, ""{10}"");{11}
            return webMethodResult;
        ";
                    }
                    newBody = string.Format(

    bodyFormat,
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType) ? "Async" : string.Empty),
     ((methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? returnType + ", " : ""),
     interfaceType,
     GetMethodName(methodName, methodType),
     parametersDictionaryDefinition,
     originalCode,
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? "return " : ""),
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.NotAsyncWithoutReturnType || methodType == MethodType.AsyncEndWithoutReturnType) ? "_WithoutReturnValue" : ""),
     ((methodType == MethodType.AsyncBegin ? "Begin" : "") + (methodType == MethodType.AsyncEndWithoutReturnType || methodType == MethodType.AsyncEndWithReturnType ? "End" : "")),
     endpointCode,
     soapVersion, outParamString
     );
                }
                else //case where there are no parameters
                {

                    // Generate the body replacement:
                    newBody = string.Format(
    @"
            {6}System.ServiceModel.INTERNAL_WebMethodsCaller.{8}CallWebMethod{0}{7}
                <{1}{2}>({9}, ""{3}"", {4}, ""{10}"");
",
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType) ? "Async" : string.Empty),
     ((methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? returnType + ", " : ""),
     interfaceType,
     GetMethodName(methodName, methodType),
     requestParameterName,
     originalCode,
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? "return " : ""),
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
                if (c == '<')
                {
                    openedBrackets++;
                    currentParameter += c;
                }
                else if (c == '>')
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
            int offset = 0;
            //first we get the whole interface definition:
            Regex regex = new Regex(@"public interface [^\}]*\}"); //this reads as "find matches that start with "public interface", followed by any character different than '}' any amount of times, then '}'".

            //for each interface:
            foreach (Match match in regex.Matches(inputText))
            {
                string stringToAdd = string.Empty;
                //We want to find the occurences of [System.ServiceModel.ServiceKnownTypeAttribute(...)]
                Regex internalRegex = new Regex(@"\[System.ServiceModel.ServiceKnownTypeAttribute\(.*\)\]");
                foreach (Match internalMatch in internalRegex.Matches(match.Value))
                {
                    //we keep the ServiceKnownTypeAttributes.
                    stringToAdd += internalMatch.Value + Environment.NewLine;
                }

                //we insert the ServiceKnownTypeAttribute before the interface.
                inputText = inputText.Insert(match.Index + offset, stringToAdd);
                offset += stringToAdd.Length;
            }
        }
    }
}
