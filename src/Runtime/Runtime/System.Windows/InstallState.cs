namespace System.Windows
{
	/// <summary>Defines constants that indicate the installation state of an application that is configured to run outside the browser.</summary>
	public enum InstallState
	{
		/// <summary>The application has not been installed to run outside the browser.</summary>
		NotInstalled,
		/// <summary>The application is in the process of being installed to run outside the browser. </summary>
		Installing,
		/// <summary>The application has been installed to run outside the browser.</summary>
		Installed,
		/// <summary>The application could not be installed to run outside the browser. </summary>
		InstallFailed
	}
}
