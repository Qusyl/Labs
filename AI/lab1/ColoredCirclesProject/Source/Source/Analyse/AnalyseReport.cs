using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Source.Analyse
{
    public sealed record AnalyseReport(int Iterations, int MaxOpenSize, int MaxTotalSize, int CurrentOpenSize);
}