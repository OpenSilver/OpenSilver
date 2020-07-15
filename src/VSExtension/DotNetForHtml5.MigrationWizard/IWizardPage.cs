using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DotNetForHtml5.MigrationWizard
{
    interface IWizardPage
    {
        bool TryCreateNextPage(Context context, out Page nextPage);

        bool CanGoBack { get; }
    }
}
