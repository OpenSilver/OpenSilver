

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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TinyPG;

namespace TypeScriptDefToCSharp
{
    public class XMLExporter
    {
        // Used to export the XML file, should better use a string and export it at the end
        private StreamWriter Output { get; set; }
        private string _OutputFile { get; set; }
        public string OutputFile {
            get { return _OutputFile; }
            set
            {
                if (value != null)
                {
                    this.Output = File.AppendText(value);
                    this.Output.AutoFlush = true;
                }
                _OutputFile = value;
            }
        }

        private static int _total { get; set; }

        public XMLExporter(string Output)
        {
            string[] tmp = Output.Split('\\');
            // Remove the filename from the path
            tmp = tmp.Take(tmp.Count() - 1).ToArray();
            // If needed, create the destination folder
            Directory.CreateDirectory(string.Join("\\", tmp));
            // Clear the file if not empty
            File.WriteAllText(Output, string.Empty);
            this.OutputFile = Output;
        }

        // Remove some useless nodes
        public static void ClearParseNode(ParseNode Node)
        {
            int i = 0;

            while (i < Node.Nodes.Count())
            {
                switch (Node.Nodes[i].Token.Type)
                {
                    // Remove all of these tokens in the tree
                    case TokenType.DOT:
                    case TokenType.COMMA:
                    case TokenType.COLON:
                    case TokenType.SEMICOLON:
                    case TokenType.LBRACKET:
                    case TokenType.RBRACKET:
                    case TokenType.LBRACE:
                    case TokenType.RBRACE:
                    case TokenType.LPAREN:
                    case TokenType.RPAREN:
                    case TokenType.VBAR:
                    case TokenType.INFERIOR:
                    case TokenType.SUPERIOR:
                    case TokenType.FATARROW:
                    case TokenType.SIMPLEQUOTE:
                    case TokenType.DOUBLEQUOTE:
                    case TokenType.EQUALS:
                    case TokenType.EOF:
                    case TokenType.K_FUNCTION:
                    case TokenType.K_CLASS:
                    case TokenType.K_INTERFACE:
                    case TokenType.K_MODULE:
                    case TokenType.K_NAMESPACE:
                    case TokenType.K_VAR:
                    case TokenType.K_DECLARE:
                    case TokenType.K_IMPORT:
                    case TokenType.K_EXTENDS:
                    case TokenType.K_ENUM:
                    case TokenType.K_READONLY:
                        Node.Nodes.Remove(Node.Nodes[i]);
                        break;
                    case TokenType.BlockElement:
                        Node.Nodes[i] = Node.Nodes[i].Nodes[0];
                        break;
                    case TokenType.Type:
                        if (Node.Nodes[i].Nodes.Any() &&
                            Node.Nodes[i].Nodes[0].Token.Type == TokenType.DOTIDENT &&
                            Node.Nodes[i].Nodes[0].Token.Text == "void")
                        {
                            Node.Nodes.Remove(Node.Nodes[i]);
                        }
                        else
                        {
                            // Recursively clear the tree
                            ClearParseNode(Node.Nodes[i]);
                            i++;
                        }
                        break;
                    case TokenType.Variable:
                        if (Node.Nodes[i].Nodes.Any() &&
                            Node.Nodes[i].Nodes[0].Token.Type == TokenType.IDENT &&
                            Node.Nodes[i].Nodes[0].Token.Text == "static" &&
                            Node.Nodes[i].Nodes.Count() == 1)
                        {
                            Node.Nodes[i].Token.Type = TokenType.K_STATIC;
                        }
                        else
                            ClearParseNode(Node.Nodes[i]);
                        i++;
                        break;
                    case TokenType.K_IMPLEMENTS:
                        Node.Nodes[i].Token.Type = TokenType.K_EXTENDS;
                        break;
                    default:
                        // Recursively clear the tree
                        ClearParseNode(Node.Nodes[i]);
                        i++;
                        break;
                }
            }
        }

        public void PrintBasicTree(ParseTree Tree)
        {
            this.Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            PrintBasicNode(Tree.Nodes[0], 0);
            this.Output.Close();
        }

        private void PrintBasicNode(ParseNode Node, int Tab)
        {
            for (int i = 0; i < Tab; i++)
                this.Output.Write("  ");
            this.Output.Write("<" + Node.Token.Type.ToString().ToLower());
            switch (Node.Token.Type)
            {
                case TokenType.FUNCTION:
                case TokenType.GENERIC:
                    this.Output.Write(" text=\"" + ReplaceSpecialChar(Node.Token.Text.Remove(Node.Token.Text.Length - 1)) + "\"");
                    break;
                case TokenType.IDENT:
                case TokenType.DOTIDENT:
                case TokenType.DOTIDENT_WITH_ADDITIONAL_CHARS_ALLOWED:
                case TokenType._UNDETERMINED_:
                    this.Output.Write(" text=\"" + Node.Token.Text + "\"");
                    break;
                case TokenType.STRING:
                    this.Output.Write(" text=" + Node.Token.Text);
                    break;                    
                case TokenType.GENERIC_ARG:
                    this.Output.Write(" text=\"" + ReplaceSpecialChar(Node.Token.Text) + "\"");
                    break;
            }
            if (Node.Nodes.Count() == 0)
                this.Output.Write("/");
            this.Output.WriteLine(">");
            foreach (ParseNode n in Node.Nodes)
            {
                PrintBasicNode(n, Tab + 1);
            }

            if (Node.Nodes.Count() != 0)
            {
                for (int i = 0; i < Tab; i++)
                    this.Output.Write("  ");
                this.Output.WriteLine("</" + Node.Token.Type.ToString().ToLower() + ">");
            }
        }

        public void PrintXMLTree(ParseTree Tree)
        {
            PrintXMLNode(Tree, 0);
        }
        
        // Print the begin of a node (ex: "<myNode")
        private void PrintXMLNode(ParseNode Node, int Tab)
        {
            for (int i = 0; i < Tab; i++)
                Console.Write("\t");
            Console.Write("<" + CultureInfo.CurrentCulture.TextInfo.ToTitleCase(Node.Token.Type.ToString()));
        }

        // Replace '<' and '>' by '&lt;' and '&gt;' to avoid errors in XML file
        public string ReplaceSpecialChar(string s)
        {
            return s.Replace("<", "&lt;").Replace(">", "&gt;");
        }
    }
}
