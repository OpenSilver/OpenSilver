using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace TypeScriptDefToCSharp.Model
{
    public class Generic : TSType
    {
        public string Name { get; set; }
        public List<TSType> TypeList { get; set; }
        public Generic()
        {
            this.TypeList = new List<TSType>();
        }

        public Generic(XElement elem, TypeScriptDefContext context)
            : this()
        {
            this.Name = elem.Element("generic").Attribute("text").Value;

            if (elem.Element("type") != null)
            {
                var types = Tool.NewType(elem.Element("type"), context);

                if (types is UnionTypeModel)
                    this.TypeList = ((UnionTypeModel)types).Types;
                else
                    this.TypeList.Add((TSType)types);
            }
        }

        public override string ToString()
        {
            if (this.TypeList.Any())
            {
                string param = "";

                for (int i = 0; i < this.TypeList.Count; ++i)
                {
                    if (i > 0)
                        param += ", ";
                    param += this.TypeList[i].ToString();
                }
                return this.Name + "<" + param + ">";
            }
            else
                return this.Name;
        }

        public string New(string jsObj)
        {
            return "JSObject.FromJavaScriptInstance<" + this.ToString() + ">(" + jsObj + ")";
        }
    }
}
