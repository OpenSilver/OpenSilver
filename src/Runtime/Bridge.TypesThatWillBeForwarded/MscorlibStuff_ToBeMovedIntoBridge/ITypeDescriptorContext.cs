namespace System.ComponentModel
{
    public interface ITypeDescriptorContext
    {
        /// <devdoc>
        ///    <para>Gets the container with the set of objects for this formatter.</para>
        /// </devdoc>
        //IContainer Container { get; }

        /// <devdoc>
        ///    <para>Gets the instance that is invoking the method on the formatter object.</para>
        /// </devdoc>
        object Instance { get; }

        /// <devdoc>
        ///      Retrieves the PropertyDescriptor that is surfacing the given context item.
        /// </devdoc>
        //PropertyDescriptor PropertyDescriptor { get; }

        /// <devdoc>
        ///    <para>Gets a value indicating whether this object can be changed.</para>
        /// </devdoc>
        bool OnComponentChanging();

        /// <devdoc>
        /// <para>Raises the <see cref='System.ComponentModel.Design.IComponentChangeService.ComponentChanged'/>
        /// event.</para>
        /// </devdoc>
        void OnComponentChanged();
    }
}
