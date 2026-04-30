// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Generic;

namespace OpenSilver.Compiler;

internal sealed class XamlParserContext
{
    private readonly Stack<Frame> _frames = [];

    public BracketModeParseParameters CurrentBracketModeParseParameters
    {
        get => _frames.Peek().BracketModeParseParameters;
        set => _frames.Peek().BracketModeParseParameters = value;
    }

    public int CurrentArgCount
    {
        get => _frames.Peek().CtorArgCount;
        set => _frames.Peek().CtorArgCount = value;
    }

    public string CurrentMember
    {
        get => _frames.Peek().Member;
        set => _frames.Peek().Member = value;
    }

    public string CurrentType
    {
        get => _frames.Peek().Descriptor.Name;
        set => _frames.Peek().Descriptor.Name = value;
    }

    public MarkupExtensionDescriptor CurrentDescriptor => _frames.Peek().Descriptor;

    public void PushScope() => _frames.Push(new Frame());

    public MarkupExtensionDescriptor PopScope() => _frames.Pop().Descriptor;

    private sealed class Frame
    {
        public MarkupExtensionDescriptor Descriptor = new();

        public BracketModeParseParameters BracketModeParseParameters;
        public int CtorArgCount;
        public string Member;
    }
}
