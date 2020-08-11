

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
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TypeScriptDefToCSharp.Model
{
    public interface IDeclaration
    {
        string Name { get; set; }
        Declaration.Container<Declaration> Super { get; set; }

        string FullName(string sep);
        string SkippedFullName(string sep, int count);
        TContainer FirstAncestor<TContainer>()
            where TContainer : Declaration.Container<Declaration>;
    }

    public abstract class Declaration : IDeclaration
    {
        public interface Container<out T> : IDeclaration, IEnumerable<T>
            where T : Declaration
        {
            void AddContent(XElement content, TypeScriptDefContext context);
        }

        public string Name { get; set; }
        public Container<Declaration> Super { get; set; }

        public Declaration() { }

        public Declaration(Container<Declaration> super)
        {
            this.Super = super;
        }

        public string FullName(string sep)
        {
            string res = this.Name;
            if (this.Super != null)
                res = this.Super.FullName(sep) + sep + res;
            return res;
        }

        public string SkippedFullName(string sep, int count)
        {
            return string.Join(sep, this.FullName(sep).Split(sep.ToCharArray()).Skip(count));
        }

        public TContainer FirstAncestor<TContainer>()
            where TContainer : Container<Declaration>
        {
            if (this.Super == null)
                return default(TContainer);
            else if (this.Super is TContainer)
                return (TContainer)this.Super;
            else
                return this.Super.FirstAncestor<TContainer>();
        }

        public abstract override string ToString();

        public virtual void Export(TypeScriptDefContext context)
        {
            throw new NotImplementedException();
        }
    }
}
