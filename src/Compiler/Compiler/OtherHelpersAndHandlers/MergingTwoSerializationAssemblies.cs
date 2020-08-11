

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
using System.Threading.Tasks;

namespace DotNetForHtml5.Compiler
{
    internal class MergingTwoSerializationAssemblies
    {
        public static string Merge(string file1content, string file2content)
        {
            StringBuilder mergedFile = new StringBuilder();
            StringsMergingTool stringMergingtool = new StringsMergingTool(file1content, file2content);

            //Look for the line: public class XmlSerializationWriter1 : System.Xml.Serialization.XmlSerializationWriter {
            //copy from that point to: protected override void InitCallbacks()
            mergedFile.Append(stringMergingtool.AdvanceToAfter("public class XmlSerializationWriter1 : System.Xml.Serialization.XmlSerializationWriter {"));
            mergedFile.Append(stringMergingtool.MergeTo("protected override void InitCallbacks()"));

            //Look for the line: public class XmlSerializationReader1 : System.Xml.Serialization.XmlSerializationReader {
            //copy from that point to: protected override void InitCallbacks()
            mergedFile.Append(stringMergingtool.AdvanceToAfter("public class XmlSerializationReader1 : System.Xml.Serialization.XmlSerializationReader {"));
            mergedFile.Append(stringMergingtool.MergeTo("protected override void InitCallbacks()"));

            //after the InitCallbacks() line, find the end of the method and copy until protected override void InitIDs() while removing duplicates.
            mergedFile.Append(stringMergingtool.AdvanceToEndOfBlock());
            mergedFile.Append(stringMergingtool.MergeWithoutDuplicateLinesTo("protected override void InitIDs()"));

            //Copy the content of InitIDs()
            mergedFile.Append(stringMergingtool.MergeBlockContent());

            //Copy all "public sealed" classes. (advanceToBefore("public sealed") then mergeTo("public class XmlSerializerContract")).
            mergedFile.Append(stringMergingtool.AdvanceToBefore("public sealed"));
            mergedFile.Append(stringMergingtool.MergeTo("public class XmlSerializerContract"));

            //Copy all _tmp[ lines. (advanceToBefore("_tmp[") then MergeTo("if")   -----> do this twice for the ReadMethods and WriteMethods
            mergedFile.Append(stringMergingtool.AdvanceToBefore("_tmp["));
            mergedFile.Append(stringMergingtool.MergeTo("if ("));

            mergedFile.Append(stringMergingtool.AdvanceToBefore("_tmp["));
            mergedFile.Append(stringMergingtool.MergeTo("if ("));

            //Copy all _tmp.Add lines. (advanceToBefore("_tmp.Add") then MergeTo("if") 
            mergedFile.Append(stringMergingtool.AdvanceToBefore("_tmp.Add"));
            mergedFile.Append(stringMergingtool.MergeTo("if ("));


            //Copy the content of public override System.Boolean CanSerialize(System.Type type) {
            mergedFile.Append(stringMergingtool.AdvanceToAfter("public override System.Boolean CanSerialize(System.Type type) {"));
            mergedFile.Append(stringMergingtool.MergeTo("return false"));

            //Copy the content of public override System.Xml.Serialization.XmlSerializer GetSerializer(System.Type type) {
            mergedFile.Append(stringMergingtool.AdvanceToAfter("public override System.Xml.Serialization.XmlSerializer GetSerializer(System.Type type) {"));
            mergedFile.Append(stringMergingtool.MergeTo("return null"));

            mergedFile.Append(stringMergingtool.advanceToEnd());

            return mergedFile.ToString();
        }

        public static string ModifySecondFileToAvoidCollisions(string secondFileContent)
        {
            // Modify the second file to avoid collisions:
            secondFileContent = Regex.Replace(secondFileContent, @"([^\w]Write[0-9]+)_", @"$1b_");
            secondFileContent = Regex.Replace(secondFileContent, @"([^\w]Read[0-9]+)_", @"$1b_");
            secondFileContent = Regex.Replace(secondFileContent, @"([^\w]id[0-9]+)_", @"$1b_");

            return secondFileContent;
        }

        /// <summary>
        /// This class provides methods to read and advance through 2 strings.
        /// </summary>
        class StringsMergingTool
        {
            string _source1;
            string _source2;
            int indexInSource1 = 0;
            int indexInSource2 = 0;
            public StringsMergingTool(string string1, string string2)
            {
                _source1 = string1;
                _source2 = string2;
            }

            public string AdvanceToBefore(string goal)
            {
                int formerIndex = indexInSource1;
                indexInSource1 = _source1.IndexOf(goal, indexInSource1);
                indexInSource2 = _source2.IndexOf(goal, indexInSource2);
                if (indexInSource1 == -1 || indexInSource2 == -2)
                {
                    throw new InvalidOperationException("Could not find \"" + goal + "\".");
                }
                return _source1.Substring(formerIndex, indexInSource1 - formerIndex);
            }

            public string AdvanceToAfter(string goal)
            {
                int formerIndex = indexInSource1;
                indexInSource1 = _source1.IndexOf(goal, indexInSource1);
                indexInSource2 = _source2.IndexOf(goal, indexInSource2);
                if (indexInSource1 == -1 || indexInSource2 == -2)
                {
                    throw new InvalidOperationException("Could not find \"" + goal + "\".");
                }
                indexInSource1 += goal.Length;
                indexInSource2 += goal.Length;
                return _source1.Substring(formerIndex, indexInSource1 - formerIndex);
            }

            /// <summary>
            /// This method advances to right after the end of the next block that opens ('{') (method, class, etc.). Note: we assume that no brackets can be ignored (for example if they were inside quotation marks).
            /// </summary>
            /// <returns></returns>
            public string AdvanceToEndOfBlock()
            {
                AdvanceToEndOfBlock(_source2, ref indexInSource2);
                return AdvanceToEndOfBlock(_source1, ref indexInSource1);
            }

            string AdvanceToEndOfBlock(string source, ref int indexInSource)
            {
                int currentIndex = indexInSource;
                while (currentIndex < source.Length && source[currentIndex] != '{')
                {
                    ++currentIndex;
                }
                if (currentIndex == source.Length)
                {
                    //probably throw an exception but for now we return the end of the string.
                    throw new InvalidOperationException("Could not find the next block");
                }
                else
                {
                    int amountOfOpenbrackets = 0;
                    bool endFound = false;
                    while (currentIndex < source.Length == !endFound)
                    {
                        if (source[currentIndex] == '{')
                        {
                            ++amountOfOpenbrackets;
                        }
                        else if (source[currentIndex] == '}')
                        {
                            --amountOfOpenbrackets;
                            if (amountOfOpenbrackets == 0)
                            {
                                endFound = true;
                            }
                        }
                        ++currentIndex;
                    }
                    if (!endFound)
                    {
                        throw new InvalidOperationException("Could not find the end of the block");
                    }

                    if (endFound && !(currentIndex + 1 == source.Length))
                    {
                        ++currentIndex; //so that we are 1 character after the closing bracket
                    }
                    int formerIndex = indexInSource;
                    indexInSource = currentIndex;
                    return source.Substring(formerIndex, currentIndex - formerIndex);
                }
            }

            public string advanceToEnd()
            {
                int formerIndex = indexInSource1;
                indexInSource1 = _source1.Length;
                indexInSource2 = _source1.Length;
                return _source1.Substring(formerIndex);
            }

            /// <summary>
            /// This method merges the content of the next block to open ('{')
            /// </summary>
            /// <returns></returns>
            public string MergeBlockContent()
            {
                string mergedString = AdvanceToEndOfBlock(_source1, ref indexInSource1);
                string source2String = AdvanceToEndOfBlock(_source2, ref indexInSource2);

                //we got the content of both blocks, now we need to put the content of _source2 without the brackets inside the brackets of the result on _source1
                source2String = source2String.Substring(source2String.IndexOf('{') + 1); //Note: AdvanceToEndOfBlock returns the string starting where it last stopped, so not necessarily right before the '{'.
                mergedString = mergedString.Remove(mergedString.Length - 2); //we know that we were 1 character after the '}' character.

                mergedString += "\r\n" + source2String; //Note: source2String contains the '}' we removed in the last line of code.
                return mergedString;
            }

            public string MergeTo(string goal)
            {
                int formerIndex1 = indexInSource1;
                indexInSource1 = _source1.IndexOf(goal, indexInSource1);
                int formerIndex2 = indexInSource2;
                indexInSource2 = _source2.IndexOf(goal, indexInSource2);
                if (indexInSource1 == -1 || indexInSource2 == -2)
                {
                    throw new InvalidOperationException("Could not find \"" + goal + "\".");
                }
                string merging = _source1.Substring(formerIndex1, indexInSource1 - formerIndex1) + "\r\n";
                merging += "\r\n" + _source2.Substring(formerIndex2, indexInSource2 - formerIndex2) + "\r\n";
                return merging;
            }

            internal string MergeWithoutDuplicateLinesTo(string goal)
            {
                int formerIndex1 = indexInSource1;
                indexInSource1 = _source1.IndexOf(goal, indexInSource1);
                int formerIndex2 = indexInSource2;
                indexInSource2 = _source2.IndexOf(goal, indexInSource2);
                if (indexInSource1 == -1 || indexInSource2 == -2)
                {
                    throw new InvalidOperationException("Could not find \"" + goal + "\".");
                }
                string merging = _source1.Substring(formerIndex1, indexInSource1 - formerIndex1) + "\r\n";
                string source2Result = _source2.Substring(formerIndex2, indexInSource2 - formerIndex2);
                string[] newLine = new string[] { "\r\n" };
                HashSet<string> source1Lines = new HashSet<string>(merging.Split(newLine, StringSplitOptions.RemoveEmptyEntries));
                string[] source2Lines = source2Result.Split(newLine, StringSplitOptions.RemoveEmptyEntries);
                foreach (string str in source2Lines)
                {
                    if (!source1Lines.Contains(str))
                    {
                        merging += "\r\n" + str;
                    }
                }
                merging += "\r\n";
                return merging;
            }
        }
    }
}
