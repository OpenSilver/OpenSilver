
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

namespace OpenSilver.Internal;

internal static class Obsoletions
{
    public const string ObsoleteMemberMessage = "Deprecated. It will be removed in a future release.";

    internal const string LegacyFormatterImplMessage = "This API supports obsolete formatter-based serialization. It should not be called or extended by application code.";
    internal const string LegacyFormatterImplDiagId = "SYSLIB0051";
}
