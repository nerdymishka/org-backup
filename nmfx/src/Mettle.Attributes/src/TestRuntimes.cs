// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System;

namespace Mettle
{
    [Flags]
    public enum TestRuntimes
    {
        Any = ~0,
        CoreCLR = 1,
        Mono = 2,
    }
}