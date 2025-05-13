using Microsoft.AspNetCore.Components.Web;

namespace OpenSilver.Simulator.BlazorSupport
{
    /// <inheritdoc />
    internal class JSComponentConfiguration : IJSComponentConfiguration
    {
        /// <inheritdoc />
        public JSComponentConfigurationStore JSComponents { get; } = new();
    }
}
