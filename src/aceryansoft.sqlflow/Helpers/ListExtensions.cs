﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace aceryansoft.sqlflow.Helpers
{
    internal static class ListExtensions
    {
        internal static List<List<T>> SplitByPacket<T>(List<T> source, int packetSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / packetSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }
    }
}
