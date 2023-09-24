using System.ComponentModel;

namespace $ext_safeprojectname$.Web
{
    /// <summary>
    /// Extensions to the <see cref="User"/> class.
    /// </summary>
    public partial class User
    {
        #region Make DisplayName Bindable

        /// <summary>
        /// Override of the <c>OnPropertyChanged</c> method that generates property change notifications when <see cref="User.DisplayName"/> changes.
        /// </summary>
        /// <param name="e">The property change event args.</param>
        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);

            if (e.PropertyName == "Name" || e.PropertyName == "FriendlyName")
            {
                this.RaisePropertyChanged("DisplayName");
            }
        }

        #endregion Make DisplayName Bindable
    }
}