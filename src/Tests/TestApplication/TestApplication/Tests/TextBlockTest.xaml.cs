﻿using System.Windows.Controls;

namespace TestApplication.Tests
{
    public partial class TextBlockTest : Page
    {
        public TextBlockTest()
        {
            InitializeComponent();

#if OPENSILVER
            EllipsisTrimmedCustomLayoutBorder.CustomLayout = true;
#endif
        }
    }
}
