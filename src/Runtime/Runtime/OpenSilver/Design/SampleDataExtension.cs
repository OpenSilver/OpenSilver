

/*===================================================================================
* 
*   Copyright (c) Userware/OpenSilver.net
*      
*   This file is part of the OpenSilver Runtime (https://opensilver.net), which is
*   licensed under the MIT license: https://opensource.org/licenses/MIT
*   
*   As stated in the MIT license, "the above copyright notice and this permission
*   notice shall be included in all copies or substantial portions of the Software."
*  
\*====================================================================================*/



using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace OpenSilver.Design;

/// <summary>
/// Provides design-time data.
/// </summary>
/// https://learn.microsoft.com/en-us/visualstudio/xaml-tools/xaml-design-time-sample-data
public class SampleDataExtension : MarkupExtension
{
    /// <summary>
    /// The number of items to generate. Default is 5.
    /// </summary>
    public int ItemCount { get; set; } = 5;

    /// <summary>
    /// Provides the value for the data context.
    /// </summary>
    /// <param name="serviceProvider">The service provider.</param>
    /// <returns>The generated sample data.</returns>
    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        var items = new List<SampleData>();
        var random = new Random();

        for (int i = 1; i <= ItemCount; i++)
        {
            items.Add(new SampleData
            {
                SampleInt = i,
                SampleStringA = $"Sample String A - {i}",
                SampleStringB = $"Sample String B - {i}",
                SampleBool = random.Next(2) == 0
            });
        }

        return items;
    }

    private class SampleData
    {
        public int SampleInt { get; set; }
        public string SampleStringA { get; set; }
        public string SampleStringB { get; set; }
        public bool SampleBool { get; set; }

        public override string ToString() => $"Sample Item {SampleInt}";
    }
}
