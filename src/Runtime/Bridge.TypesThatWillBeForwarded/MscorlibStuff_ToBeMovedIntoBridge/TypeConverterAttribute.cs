namespace System.ComponentModel
{
    [AttributeUsage(AttributeTargets.All)]
    public class TypeConverterAttribute : Attribute
    {
        public static readonly TypeConverterAttribute Default = new TypeConverterAttribute();

        public TypeConverterAttribute()
        {
            ConverterTypeName = string.Empty;
        }

        public TypeConverterAttribute(Type type)
        {
            ConverterTypeName = type.AssemblyQualifiedName;
        }

        public TypeConverterAttribute(string typeName)
        {
            ConverterTypeName = typeName;
        }

        public string ConverterTypeName { get; set; }

        public override bool Equals(object obj)
        {
            return obj is TypeConverterAttribute other;
        }

        public override int GetHashCode()
        {
            return ConverterTypeName.GetHashCode();
        }
    }
}
