﻿#if MIGRATION

namespace System.ServiceModel.Channels
{
    internal interface IPollingDuplexSettings
    {
        TimeSpan InactivityTimeout { get; set; }
    }
}

#endif
