// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace OpenSilver.Compiler;

internal static class Errors
{
    public const string UnexpectedTokenAfterME = "Unexpected token after end of markup extension.";
    public const string UnclosedQuote = "Unclosed quoted value.";
    public const string QuoteCharactersOutOfPlace = "Quote characters ' or \" are only allowed at the start of values.";
    public const string MalformedBracketCharacters = "BracketCharacter '{0}' does not have a corresponding opening/closing BracketCharacter.";
    public const string InvalidClosingBracketCharacers = "Encountered a closing BracketCharacter '{0}' without a corresponding opening BracketCharacter.";
    public const string MalformedPropertyName = "Cannot parse the malformed property name '{0}'.";
    public const string UnexpectedToken = "Unexpected token '{0}' in rule: '{1}', in '{2}'.";
    public const string MissingComma1 = "Unexpected equals sign '=' following '{0}'. Check for a missing comma separator.";
    public const string MissingComma2 = "Unexpected equals sign '=' following '{0}'='{1}'. Check for a missing comma separator.";
    public const string WhitespaceAfterME = "White space is not allowed after end of markup extension.";
    public const string TooManyPositionalArguments = "Multiple positional arguments are not supported.";
}
