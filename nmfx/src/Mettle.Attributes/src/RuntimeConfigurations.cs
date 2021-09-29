// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Mettle
{
    [Flags]
    public enum RuntimeConfigurations
    {
        Any = ~0,
        Checked = 1,
        Debug = 2,
        Release = 4,
    }
}