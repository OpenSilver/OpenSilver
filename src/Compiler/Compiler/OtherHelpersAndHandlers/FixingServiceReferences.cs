using System;
using System.Collections.Generic;
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
        internal class SubStringFinderCharByChar
        {
            private string SubString { get; set; }

            private int CurrentIndex { get; set; }

            internal bool IsMatch { get; private set; }

            private int _matchStartIndex;
            internal int MatchStartIndex
            {
                get
                {
                    if (!IsMatch)
                    {
                        return -1;
                    }
                    else
                    {
                        return _matchStartIndex;
                    }
                }
            }

            internal SubStringFinderCharByChar(string subString)
            {
                _matchStartIndex = -1;
                SubString = subString;
                CurrentIndex = 0;
                IsMatch = false;
            }

            internal void Reset()
            {
                CurrentIndex = 0;
                IsMatch = false;
                _matchStartIndex = -1;
            }

            internal void CheckNextChar(char currentChar, int charIndexInBaseString)
            {
                if (!IsMatch)
                {
                    if (SubString[CurrentIndex] == currentChar)
                    {
                        if (CurrentIndex == 0)
                        {
                            _matchStartIndex = charIndexInBaseString;
                        }
                        if (++CurrentIndex == SubString.Length)
                        {
                            IsMatch = true;
                        }
                    }
                    else
                    {
                        Reset();
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



        private static string FixAttributeConstructor(string attributeAsString, string classNameToReplace, string classNameReplacement, out bool wasAnythingFixed)
        {
            string fixedAttribute = attributeAsString + Environment.NewLine;
            wasAnythingFixed = true;
            string[] splittedMatch = attributeAsString.Split('"');
            int splittedMatchLength = splittedMatch.Length;
            for (int i = 0; i < splittedMatchLength; ++i)
            {
                string currentSplit = splittedMatch[i];

                if (i == 0)
                {
                    //we replace DataContractAttribute with DataContractAttribute2 because we need it to serve as an intermediate to avoid exceptions caused by our version of DataContractAttribute and the C# version (we have added parameters to the constructor because JSIL is currently unable to handle attributes with properties set outside the constructor for some reason).
                    currentSplit = currentSplit.Replace(classNameToReplace, classNameReplacement);
                }
                if (i % 2 == 0)
                {
                    //we are in the "pair" matches, which means outside of the quotation marks:
                    //we replace the '=' with ':'
                    fixedAttribute += currentSplit.Replace('=', ':');
                }
                else
                {
                    //we are inside the quotation marks, so we add the string as is:
                    fixedAttribute += "\"" + currentSplit + "\""; //adding back the quotation marks that have been removed by the Split
                }
            }
            return fixedAttribute;
        }

        private static string FixAttributeConstructor(string attributeAsString, out bool wasAnythingFixed)
        {
            int indexOfDataContractAttribute = attributeAsString.IndexOf("DataContractAttribute");
            int indexOfServiceContractAttribute = attributeAsString.IndexOf("ServiceContractAttribute");
            if (indexOfDataContractAttribute == -1 && indexOfServiceContractAttribute == -1)
            {
                wasAnythingFixed = false;
                return attributeAsString;
            }
            else if (indexOfDataContractAttribute > -1 && indexOfServiceContractAttribute == -1)
            {
                return FixAttributeConstructor(attributeAsString, "DataContractAttribute", "DataContract2Attribute", out wasAnythingFixed);
            }
            else if (indexOfServiceContractAttribute > -1 && indexOfDataContractAttribute == -1)
            {
                return FixAttributeConstructor(attributeAsString, "ServiceContractAttribute", "ServiceContract2Attribute", out wasAnythingFixed);
            }
            else
            {
                if (indexOfDataContractAttribute < indexOfServiceContractAttribute)
                {
                    return FixAttributeConstructor(attributeAsString, "DataContractAttribute", "DataContract2Attribute", out wasAnythingFixed);
                }
                else
                {
                    return FixAttributeConstructor(attributeAsString, "ServiceContractAttribute", "ServiceContract2Attribute", out wasAnythingFixed);
                }
            }
        }

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

            // used to find the interface in "System.ServiceModel.ClientBase<...>", so we don't need to it every time we call FixBlock().
            SubStringFinderCharByChar clientBaseInterfaceFinder = new SubStringFinderCharByChar("System.ServiceModel.ClientBase<");
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
                        bool wasAttributeFixed;
                        string fixedAttributeAsString = FixAttributeConstructor(currentAttribute.ToString(), out wasAnythingFixed);
                        if (wasAnythingFixed)
                        {
                            wasAnythingFixed = true;
                        }
                        if (numberOfOpenedBrackets < 2)
                        {
                            sb.Append(fixedAttributeAsString);
                        }
                        else
                        {
                            currentBlock.Append(fixedAttributeAsString);
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
                                clientBaseInterfaceFinder.CheckNextChar(c, charIndex);
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
                                if (FixBlock(ref currentBlockAsString, string.IsNullOrEmpty(clientBaseForcedInterfaceName) ? clientBaseInterfaceName : clientBaseForcedInterfaceName, inputText, regexMethodIdentifier, clientBaseToken, endpointCode, soapVersion))
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

            // Fix the ability to retrieve the SOAPAction name at runtime (read comments inside the following method):
            AddActionPathInConstructor(ref finalText);

            //extract the ServiceKnownTypesAttribute that are assigned to the Methods of the interfaces, to assign them to the interface itself (our version of JSIL is unable to get the custom Attributes from interfaces' methods, but not from the interface's type itself.
            ExtractServiceKnownTypeAttributesToInterfaceItself(ref finalText);


            //Replace the inheritance to ClientBase with an inheritance to CSHTML5_ClientBase
            finalText = finalText.Replace("System.ServiceModel.ClientBase<", "System.ServiceModel.CSHTML5_ClientBase<");

            return finalText;

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
                else if (returnType == "Task")
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
                    originalCode = $"{detectedToken}.{methodName}()";
                }
                else
                {
                    originalCode = $"{detectedToken}.{methodName}({requestParameterName})";
                }

                //Make a dictionary to know the parameters from the name:
                //todo-perf: this and the "check the amount of parameters and adapt the body replacement:" part may be a bit redundant since they're both about the parameters of the method so we might be able to make the compilation slightly faster by changing this (probably unnoticeable).
                Dictionary<string, string> parameterNamesToTheirDefinitions = new Dictionary<string, string>();
                if (!string.IsNullOrWhiteSpace(methodParametersDefinitions))
                {
                    string[] splittedMethodParametersDefinition = SplitStringTakingAccountOfBrackets(methodParametersDefinitions, ',');
                    foreach (string parameterDefinition in splittedMethodParametersDefinition)
                    {
                        string paramDefinition = parameterDefinition.Trim();
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

                        parameterNamesToTheirDefinitions.Add(parameterName, String.Format(@"new global::System.Tuple<global::System.Type, object>(typeof({0}), {1})", parameterTypeAsString, parameterName));
                    }
                }
                //check the amount of parameters and adapt the body replacement:
                string[] splittedParameters = requestParameterName.Split(',');
                string newBody = "";
                if (thereAreParameters)
                {
                    string parametersDictionaryDefinition = "new global::System.Collections.Generic.Dictionary<string, global::System.Tuple<global::System.Type, object>>() {";
                    bool isFirst = true;
                    foreach (string paramName in splittedParameters)
                    {
                        string trimmedParamName = paramName.Trim();

                        if (trimmedParamName == "null") continue;

                        if (!isFirst)
                        {
                            parametersDictionaryDefinition += ", ";
                        }

                        string parameterDefinition = parameterNamesToTheirDefinitions.ContainsKey(trimmedParamName) ?
                                                     parameterNamesToTheirDefinitions[trimmedParamName] :
                                                     "new global::System.Tuple<global::System.Type, object>(typeof(object), null)";

                        parametersDictionaryDefinition += $"{{\"{trimmedParamName}\", {parameterDefinition}}}"; //Note: Can the params names be different from trimmedParam and the one in parameterNamesToTheirDefinitions? (probably not if properly trimmed and all)
                        isFirst = false;
                    }
                    parametersDictionaryDefinition += "}";

                    newBody = string.Format(

    @"
            {6}System.ServiceModel.INTERNAL_WebMethodsCaller.{8}CallWebMethod{0}{7}
                <{1}{2}>({9}, ""{3}"", {4}, ""{10}"");
",
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType) ? "Async" : string.Empty),
     ((methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? returnType + ", " : ""),
     interfaceType,
     methodName,
     parametersDictionaryDefinition,
     originalCode,
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? "return " : ""),
     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.NotAsyncWithoutReturnType || methodType == MethodType.AsyncEndWithoutReturnType) ? "_WithoutReturnValue" : ""),
     ((methodType == MethodType.AsyncBegin ? "Begin" : "") + (methodType == MethodType.AsyncEndWithoutReturnType || methodType == MethodType.AsyncEndWithReturnType ? "End" : "")),
     endpointCode,
     soapVersion
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
     methodName,
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



        // old version
//        internal static string Fix(string inputText, out bool wasAnythingFixed)
//        {
//            // TIP: to read the regex below, start from the "base\.Channel" part, and go backwards. You can use the website http://regexr.com/ to help read it - Please note that for that website to work perfectly, you need to remove the C#-only capture group names ?<GROUP_NAME>

//            Regex regex = new Regex(@"(?<RETURN_TYPE>([\[\]\w\.><]|\s*,\s*)+)\s+\S+\((?<METHOD_PARAMETERS_DEFINITIONS>[^\)]*)\)\s*{(?<METHOD_BODY_TO_REPLACE>[^\{]*base\.Channel.(?<METHOD_NAME>\w+)\((?<REQUEST_PARAMETER_NAME>( *\w*,?)*)\)[^\}]*)}");
//            //Regex regex = new Regex(@"(?<RETURN_TYPE>[\[\]\w\.><]+)\s+\S+\((?<METHOD_PARAMETERS_DEFINITIONS>[^\)]*)\)\s*{(?<METHOD_BODY_TO_REPLACE>[^\{]*base\.Channel.(?<METHOD_NAME>\w+)\((?<REQUEST_PARAMETER_NAME>( *\w*,?)*)\)[^\}]*)}");

//            //*****************************************************
//            //
//            // SAMPLE INPUT IN CAASE OF SIMPLE METHOD:
//            //
//            //     public int GetCurrentYear() {
//            //         return base.Channel.GetCurrentYear();
//            //     }
//            //     
//            //     public System.Threading.Tasks.Task<int> GetCurrentYearAsync() {
//            //         return base.Channel.GetCurrentYearAsync();
//            //     }
//            //
//            //
//            // SAMPLE INPUT IN CASE OF A METHOD THAT USES "REQUEST" AND "RESULT" WRAPPER CLASSES (those classes are auto-generated thanks to the "XmlSerializerFormat" attribute server-side):
//            //
//            // Here is a sample input string:
//            //      
//            //     [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
//            //     DotNetForHtml5.Showcase.ServiceReference1.DeleteToDoResponse DotNetForHtml5.Showcase.ServiceReference1.IService1.DeleteToDo(DotNetForHtml5.Showcase.ServiceReference1.DeleteToDoRequest request) {
//            //         return base.Channel.DeleteToDo(request);
//            //     }
//            //*****************************************************

//            int offset = 0;
//            int count = 0;

//            foreach (Match match in regex.Matches(inputText))
//            {
//                count++;

//                string returnType = match.Groups["RETURN_TYPE"].Value;
//                string methodBodyToReplace = match.Groups["METHOD_BODY_TO_REPLACE"].Value;
//                string methodParametersDefinitions = match.Groups["METHOD_PARAMETERS_DEFINITIONS"].Value;
//                string methodName = match.Groups["METHOD_NAME"].Value;
//                string requestParameterName = match.Groups["REQUEST_PARAMETER_NAME"].Value;

//                // Check the method type:
//                MethodType methodType;
//                if (returnType == "System.IAsyncResult")
//                {
//                    methodType = MethodType.AsyncBegin;
//                }
//                else if (requestParameterName.Contains("result")) // AsyncEnd
//                {
//                    if (returnType == "void")
//                        methodType = MethodType.AsyncEndWithoutReturnType;
//                    else
//                        methodType = MethodType.AsyncEndWithReturnType;
//                }
//                else if (returnType.Contains("System.Threading.Tasks.Task<") && returnType.Contains(">"))
//                {
//                    methodType = MethodType.AsyncWithReturnType;
//                }
//                else if (returnType == "System.Threading.Tasks.Task")
//                {
//                    methodType = MethodType.AsyncWithoutReturnType;
//                }
//                else if (returnType == "void")
//                {
//                    methodType = MethodType.NotAsyncWithoutReturnType;
//                }
//                else
//                {
//                    methodType = MethodType.NotAsyncWithReturnType;
//                }

//                // Check if the method is async:
//                if (methodType == MethodType.AsyncWithReturnType)
//                {
//                    int indexOfLowerThan = returnType.IndexOf('<');
//                    int indexOfGreaterThan = returnType.LastIndexOf('>');
//                    returnType = returnType.Substring(indexOfLowerThan + 1, indexOfGreaterThan - indexOfLowerThan - 1);
//                }

//                // Find out the name of the current interface by looking for the "T" in ClientBase<T>:
//                int indexWhereMatchWasFound = match.Index + offset;
//                const string clientBase = "System.ServiceModel.ClientBase<";
//                int indexOfClientBase = inputText.LastIndexOf(clientBase, indexWhereMatchWasFound);
//                if (indexOfClientBase < 0)
//                    continue;
//                int startIndex = indexOfClientBase + clientBase.Length;
//                int endIndex = inputText.IndexOf('>', startIndex);
//                if (endIndex < 0 || endIndex <= startIndex)
//                    continue;
//                string interfaceType = inputText.Substring(startIndex, endIndex - startIndex);

//                // If the web method takes no parameters, we pass "null":
//                string originalCode;
//                bool thereAreParameters = true;
//                if (string.IsNullOrWhiteSpace(requestParameterName))
//                {
//                    thereAreParameters = false;
//                    requestParameterName = "null";
//                    originalCode = string.Format(@"base.Channel.{0}();", methodName);
//                }
//                else
//                {
//                    originalCode = string.Format(@"base.Channel.{0}({1});", methodName, requestParameterName);
//                }

//                //Make a dictionary to know the parameters from the name:
//                //todo-perf: this and the "check the amount of parameters and adapt the body replacement:" part may be a bit redundant since they're both about the parameters of the method so we might be able to make the compilation slightly faster by changing this (probably unnoticeable).
//                Dictionary<string, string> parameterNamesToTheirDefinitions = new Dictionary<string, string>();
//                if (!string.IsNullOrWhiteSpace(methodParametersDefinitions))
//                {
//                    string[] splittedMethodParametersDefinition = SplitStringTakingAccountOfBrackets(methodParametersDefinitions, ',');
//                    foreach (string parameterDefinition in splittedMethodParametersDefinition)
//                    {
//                        string paramDefinition = parameterDefinition.Trim();
//                        string[] splittedParamDefinition = SplitStringTakingAccountOfBrackets(paramDefinition, ' ');
//                        //Note on the line above: very improbable scenario that would lead to problems: if the guy writes Paramtype1\r\nParamName (see it as a new line, not the actual characters)
//                        bool isParameterTypeGotten = false;
//                        string parameterTypeAsString = null;
//                        string parameterName = null;
//                        foreach (string paramDefPart in splittedParamDefinition)
//                        {
//                            if (!string.IsNullOrWhiteSpace(paramDefPart))
//                            {
//                                if (!isParameterTypeGotten)
//                                {
//                                    parameterTypeAsString = paramDefPart.Trim();
//                                    isParameterTypeGotten = true;
//                                }
//                                else
//                                {
//                                    parameterName = paramDefPart.Trim();
//                                }
//                            }
//                        }

//                        parameterNamesToTheirDefinitions.Add(parameterName, String.Format(@"new global::System.Tuple<global::System.Type, object>(typeof({0}), {1})", parameterTypeAsString, parameterName));
//                    }
//                }

//                //check the amount of parameters and adapt the body replacement:
//                string[] splittedParameters = requestParameterName.Split(',');
//                string newBody = "";
//                if (thereAreParameters)
//                {
//                    string parametersDictionaryDefinition = "new global::System.Collections.Generic.Dictionary<string, global::System.Tuple<global::System.Type, object>>() {";
//                    bool isFirst = true;
//                    foreach (string param in splittedParameters)
//                    {
//                        string trimmedParam = param.Trim();
//                        if (!isFirst)
//                        {
//                            parametersDictionaryDefinition += ", ";
//                        }
//                        parametersDictionaryDefinition += string.Format(@"{{""{0}"", {1}}} ", trimmedParam, parameterNamesToTheirDefinitions[trimmedParam]); //Note: Can the params names be different from trimmedParam and the one in parameterNamesToTheirDefinitions? (probably not if properly trimmed and all)
//                        isFirst = false;
//                    }
//                    parametersDictionaryDefinition += "}";

//                    newBody = string.Format(

//    @"
//            {6}System.ServiceModel.INTERNAL_WebMethodsCaller.{8}CallWebMethod{0}{7}
//                <{1}{2}>(this, ""{3}"", {4});
//",
//     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType) ? "Async" : string.Empty),
//     ((methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? returnType + ", " : ""),
//     interfaceType,
//     methodName,
//     parametersDictionaryDefinition,
//     originalCode,
//     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? "return " : ""),
//     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.NotAsyncWithoutReturnType || methodType == MethodType.AsyncEndWithoutReturnType) ? "_WithoutReturnValue" : ""),
//     ((methodType == MethodType.AsyncBegin ? "Begin" : "") + (methodType == MethodType.AsyncEndWithoutReturnType || methodType == MethodType.AsyncEndWithReturnType ? "End" : ""))
//     );
//                }
//                else //case where there are no parameters
//                {

//                    // Generate the body replacement:
//                    newBody = string.Format(
//    @"
//            {6}System.ServiceModel.INTERNAL_WebMethodsCaller.{8}CallWebMethod{0}{7}
//                <{1}{2}>(this, ""{3}"", {4});
//",
//     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType) ? "Async" : string.Empty),
//     ((methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? returnType + ", " : ""),
//     interfaceType,
//     methodName,
//     requestParameterName,
//     originalCode,
//     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.AsyncWithReturnType || methodType == MethodType.NotAsyncWithReturnType || methodType == MethodType.AsyncBegin || methodType == MethodType.AsyncEndWithReturnType) ? "return " : ""),
//     ((methodType == MethodType.AsyncWithoutReturnType || methodType == MethodType.NotAsyncWithoutReturnType || methodType == MethodType.AsyncEndWithoutReturnType) ? "_WithoutReturnValue" : ""),
//     ((methodType == MethodType.AsyncBegin ? "Begin" : "") + (methodType == MethodType.AsyncEndWithoutReturnType || methodType == MethodType.AsyncEndWithReturnType ? "End" : ""))
//     );
//                }
//                inputText = inputText.Replace(methodBodyToReplace, newBody);
//                offset += (newBody.Length - methodBodyToReplace.Length);

//                //sb.AppendLine("");
//                //sb.AppendLine("RETURN_TYPE:" + returnType);
//                //sb.AppendLine("INTERFACE_TYPE:" + interfaceType);
//                //sb.AppendLine("METHOD_BODY_TO_REPLACE:" + methodBodyToReplace);
//                //sb.AppendLine("METHOD_NAME:" + methodName);
//                //sb.AppendLine("REQUEST_PARAMETER_NAME:" + requestParameterName);
//            }


//            //Now we try to fix the Definition of the DataContractAttributes for the types:
//            //looking for something that looks like:
//            //      [System.Runtime.Serialization.DataContractAttribute(Name="ToDoItem", Namespace="http://schemas.datacontract.org/2004/07/WcfService1")]
//            //we want to replace it with:
//            //      [System.Runtime.Serialization.DataContractAttribute(Name:"ToDoItem", Namespace:"http://schemas.datacontract.org/2004/07/WcfService1")]
//            //(basically replace the '=' outside of the quotation marks with ':').

//            //todo: it is possible to have an unwanted match (in the middle of a string for example), if/once we can use a multiline RegexOption use it here and add '^' at the beginning of the regex.


//            string regexString = @"\[System\.Runtime\.Serialization\.DataContractAttribute\(.*\)\]";
//            count += ReplaceAttributeConstructor(ref inputText, regexString, "DataContractAttribute", "DataContract2Attribute");

//            regexString = @"\[System\.ServiceModel\.ServiceContractAttribute\(.*\)\]";
//            count += ReplaceAttributeConstructor(ref inputText, regexString, "ServiceContractAttribute", "ServiceContract2Attribute");

//            // Fix the ability to retrieve the SOAPAction name at runtime (read comments inside the following method):
//            AddActionPathInConstructor(ref inputText);

//            //extract the ServiceKnownTypesAttribute that are assigned to the Methods of the interfaces, to assign them to the interface itself (our version of JSIL is unable to get the custom Attributes from interfaces' methods, but not from the interface's type itself.
//            ExtractServiceKnownTypeAttributesToInterfaceItself(ref inputText);


//            //Replace the inheritance to ClientBase with an inheritance to CSHTML5_ClientBase
//            inputText = inputText.Replace("System.ServiceModel.ClientBase<", "System.ServiceModel.CSHTML5_ClientBase<");
//            //wasAnythingFixed = (count > 0);
//            wasAnythingFixed = true; //I think we will always have a class that inherits ClientBase
//            return inputText;
//        }

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

        static private void ExtractServiceKnownTypeAttributesToInterfaceItself(ref string inputText)
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

        /// <summary>
        /// In the SOAPAction header sent to the server, we need to pass the same SOAPAction located
        /// in Reference.cs. We do so by reading the "Action" property of the "OperationContractAttribute"
        /// located in the methods of the interface (eg. IService1). Due to the fact that JSIL is currently
        /// unable to read attributes of methods of interfaces, we move this information to the
        /// "ServiceContract2Attribute" declared on the interface itself (rather than an interface method).
        /// </summary>
        /// <param name="inputText">The content of the Reference.cs file</param>
        static private void AddActionPathInConstructor(ref string inputText)
        {
            string Action = "";
            int offset = 0;
            Regex regex = new Regex(@"\[System\.ServiceModel\.ServiceContract2Attribute\(.*""");

            // we find all the potential candidates to add a path
            foreach (Match match in regex.Matches(inputText))
            {
                // we replace only if the next thing to be implemented is an interface
                Regex regex2 = new Regex(@"public .*");

                if (regex2.Match(inputText, match.Index).Value.Split(' ')[1] == "interface") // [0] is the word Public, [1] can be interface, class...
                {
                    //if it's the case, we find the first declaration of OperationContractAttribute
                    Regex regex3 = new Regex(@"System\.ServiceModel\.OperationContractAttribute");

                    Match matchAttribute = regex3.Match(inputText, match.Index);

                    // then the definition of the Action attribute in it 
                    Regex regex4 = new Regex(@"Action");
                    Match matchAction = regex4.Match(inputText, matchAttribute.Index);

                    // then we find the index of the next two " that define the path in string format
                    int StartAction = inputText.IndexOf('"', matchAction.Index) + 1; // +1 to remove "
                    int EndAction = inputText.IndexOf('"', StartAction);

                    Action = inputText.Substring(StartAction, EndAction - StartAction); // get something like : http://tempuri.org/IService1/GetToDos now we want to remove the method name

                    Action = Action.Substring(0, Action.LastIndexOf('/') + 1); // now we have the right path : http://tempuri.org/IService1/


                    // the index to place the path
                    int endMatch = match.Index + offset + match.Length;

                    inputText = inputText.Insert(endMatch, ",SOAPActionPrefix:\"" + Action + "\""); // add someting like: ,SOAPActionPrefix="http://tempuri.org/IService1/"
                    offset += (",SOAPActionPrefix:\"" + Action + "\"").Length;
                }
            }
        }

        static private int ReplaceAttributeConstructor(ref string inputText, string regexString, string classNameToReplace, string classNameReplacement)
        {
            int offset = 0;
            int count = 0;
            Regex regex2 = new Regex(regexString);
            List<string> matchesMet = new List<string>();
            foreach (Match match in regex2.Matches(inputText))
            {
                ++count;

                string resultMatch = "";
                //we split the string at the position of the '"' characters in order to avoid replacing the '=' that are inside the strings.
                string exactMatch = match.Value;
                if (!matchesMet.Contains(exactMatch)) //this is to avoid replacing the attribute multiple times, which would lead to having several times the additional attribute.
                {
                    matchesMet.Add(exactMatch);
                    string[] splittedMatch = exactMatch.Split('"');
                    int splittedMatchLength = splittedMatch.Length;
                    for (int i = 0; i < splittedMatchLength; ++i)
                    {
                        string currentSplit = splittedMatch[i];

                        if (i == 0)
                        {
                            //we replace DataContractAttribute with DataContractAttribute2 because we need it to serve as an intermediate to avoid exceptions caused by our version of DataContractAttribute and the C# version (we have added parameters to the constructor because JSIL is currently unable to handle attributes with properties set outside the constructor for some reason).
                            currentSplit = currentSplit.Replace(classNameToReplace, classNameReplacement);
                        }
                        if (i % 2 == 0)
                        {
                            //we are in the "pair" matches, which means outside of the quotation marks:
                            //we replace the '=' with ':'
                            resultMatch += currentSplit.Replace('=', ':');
                        }
                        else
                        {
                            //we are inside the quotation marks, so we add the string as is:
                            resultMatch += "\"" + currentSplit + "\""; //adding back the quotation marks that have been removed by the Split
                        }
                    }

                    resultMatch += Environment.NewLine + exactMatch;

                    //replacing the match:
                    inputText = inputText.Replace(exactMatch, resultMatch);
                    offset += (resultMatch.Length - exactMatch.Length);
                }
            }
            return count;
        }
    }
}
