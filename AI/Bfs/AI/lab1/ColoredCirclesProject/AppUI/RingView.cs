using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Media;

namespace AppUI
{
    public sealed record RingView(Point Center, double Radius, IBrush Brush, bool Clockwise);
}