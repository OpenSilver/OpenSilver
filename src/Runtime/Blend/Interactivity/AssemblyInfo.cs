// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// -------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Resources;
using System.Windows.Markup;

[assembly: XmlnsPrefix(@"http://schemas.microsoft.com/expression/2010/interactivity", "i")]
[assembly: XmlnsDefinition(@"http://schemas.microsoft.com/expression/2010/interactivity", "System.Windows.Interactivity")]

[assembly: SuppressMessage("Microsoft.Performance", "CA1824:MarkAssembliesWithNeutralResourcesLanguage")]
[assembly: NeutralResourcesLanguage("en", UltimateResourceFallbackLocation.MainAssembly)]