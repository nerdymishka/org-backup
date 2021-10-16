// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
using System;
using System.Diagnostics.CodeAnalysis;

namespace Mettle
{
    [Flags]
    [SuppressMessage("CS4070", "S4070:", Justification = "By Design")]
    public enum TestPlatforms
    {
        Any = ~0,
        Windows = 1,
        Linux = 2,
        OSX = 4,
        FreeBSD = 8,
        NetBSD = 16,
        Illumos = 32,
        Solaris = 64,
        IOS = 128,
        TVOS = 256,
        Android = 512,
        Browser = 1024,
        MacCatalyst = 2048,
        AnyUnix = FreeBSD | Linux | NetBSD | OSX | Illumos | Solaris | IOS | TVOS | MacCatalyst | Android | Browser,
    }
}