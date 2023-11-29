﻿namespace Stott.Security.Optimizely.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

internal static class StringExtensions
{
    internal static IList<string> SplitByComma(this string? value)
    {
        return value?.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? new List<string>(0);
    }
}
