#if WORKINPROGRESS
namespace System.ServiceModel.Security
{
    //
    // Summary:
    //     Represents a client credential based on user name and password.
    public sealed partial class UserNamePasswordClientCredential
    {
        //
        // Summary:
        //     Gets or sets the password.
        //
        // Returns:
        //     The password.
        public string Password { get; set; }
        //
        // Summary:
        //     Gets or sets the user name.
        //
        // Returns:
        //     The user name.
        public string UserName { get; set; }
    }
}
#endif