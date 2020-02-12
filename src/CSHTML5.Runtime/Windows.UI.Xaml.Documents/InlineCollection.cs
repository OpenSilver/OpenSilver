using CSHTML5.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

#if MIGRATION
namespace System.Windows.Documents
#else
namespace Windows.UI.Xaml.Documents
#endif
{
    public partial class InlineCollection : TextElementCollection<Inline>, IList
    {
        #region Constructor
        internal InlineCollection(DependencyObject owner) : base(owner)
        {

        }
        #endregion

        #region Public Methods
        public void Add(string text)
        {
            this.AddInternal(text);
        }
        #endregion

        #region Internal Properties
        internal Inline FirstElement
        {
            get
            {
                if (this.Count == 0)
                {
                    return null; //note: maybe throw an exception ?
                }
                return (Inline)this[0];
            }
        }
        #endregion

        #region Internal Methods
        private void AddInternal(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException("text");
            }
            Run run = new Run()
            {
                Text = text
            };
            this.Add(run);
        }
        #endregion
    }
}
