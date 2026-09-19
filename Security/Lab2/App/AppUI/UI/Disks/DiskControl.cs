using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Media;

namespace UI.Disks
{
    public class DiskControl : Control
    {
        public override void Render(DrawingContext context)
    {
        base.Render(context);

        var center = new Avalonia.Point(300, 300);

        var pen = new Pen(Brushes.Black, 4);

        context.DrawEllipse(
            Brushes.Yellow,
            pen,
            center,
            300,
            300);

            context.DrawEllipse(
                Brushes.LightGoldenrodYellow,
                pen,
                center,
                250,
                250);

            context.DrawEllipse(
                Brushes.White,
                pen,
                center,
                200,
                200
            );
    }
    }
}