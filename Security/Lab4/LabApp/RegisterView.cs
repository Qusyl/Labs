using System.Globalization;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace LabApp;


public class RegisterView : Control
{
    public static readonly StyledProperty<TickStep?> StepProperty =
        AvaloniaProperty.Register<RegisterView, TickStep?>(nameof(Step));

    public TickStep? Step { get => GetValue(StepProperty); set => SetValue(StepProperty, value); }

    static RegisterView()
    {
        AffectsRender<RegisterView>(StepProperty);
        AffectsMeasure<RegisterView>(StepProperty);
    }

    const double L = 40, Gap = 26, H = 46, Top = 26, R = 10;
    static double CellW(int n) => n <= 8 ? 56 : 40;
    const double Bus = Top + H + 56;

    protected override Size MeasureOverride(Size available)
    {
        int n = Step?.State.Length ?? 0;
        return new Size(L + n * CellW(n) + n * Gap + 60, Bus + 30);
    }

    public override void Render(DrawingContext ctx)
    {
        var st = Step;
        if (st == null) return;
        int n = st.State.Length;
        double cw = CellW(n), midY = Top + H / 2;
        double X(int i) => L + i * (cw + Gap);

        var pen = new Pen(Brushes.Black, 1.6);
        var red = new Pen(Brushes.Crimson, 2.2);
        var dash = new Pen(Brushes.Gray, 1.6, new DashStyle(new double[] { 4, 3 }, 0));

        for (int i = 0; i < n; i++)
        {
            var rect = new Rect(X(i), Top, cw, H);
            bool last = i == n - 1;
            ctx.DrawRectangle(last ? Brushes.MistyRose : Brushes.White, last ? red : pen, rect);
            Label(ctx, st.State[i].ToString(), 20, X(i) + cw / 2, midY, Brushes.Black);

            Label(ctx, $"r{i + 1}", 11, X(i) + cw / 2, Top - 11, Brushes.Gray);
            if (i == 0 && st.State.Length >= 17 && st.Index > 1) Label(ctx, $"ВЫХОД ГАММЫ", 11, X(i) + cw / 2, Top - 11, Brushes.Black);
            if (i < n - 1) Arrow(ctx, pen, X(i) + cw, midY, X(i + 1), midY);
        }


        double outX = X(n - 1) + cw + Gap / 2;
        Arrow(ctx, pen, X(n - 1) + cw, midY, outX + Gap / 2 + 10, midY);
        ctx.DrawLine(pen, new Point(outX, midY), new Point(outX, Bus));
        Label(ctx, $"выход = {st.Out}", 12, outX + 30, midY - 14, Brushes.Crimson);

      
        ctx.DrawLine(pen, new Point(outX, Bus), new Point(L - 18, Bus));
        ctx.DrawLine(pen, new Point(L - 18, Bus), new Point(L - 18, midY));
        Arrow(ctx, pen, L - 18, midY, L, midY);

       
        for (int k = 0; k < n - 1; k++)
        {
            double x = X(k) + cw + Gap / 2;
            ctx.DrawLine(st.Taps[k] == 1 ? pen : dash, new Point(x, midY), new Point(x, Bus));
        }
        for (int k = 0; k < n - 1; k++)
            if (st.Taps[k] == 1) Xor(ctx, pen, X(k) + cw + Gap / 2, Bus);

        Label(ctx, $"обратная связь = {st.Fb}", 12, L + 70, Bus + 18, Brushes.DarkBlue);
    }

    static void Xor(DrawingContext ctx, Pen pen, double x, double y)
    {
        ctx.DrawEllipse(Brushes.White, pen, new Point(x, y), R, R);
        ctx.DrawLine(pen, new Point(x - R, y), new Point(x + R, y));
        ctx.DrawLine(pen, new Point(x, y - R), new Point(x, y + R));
    }

    static void Arrow(DrawingContext ctx, Pen pen, double x1, double y1, double x2, double y2)
    {
        ctx.DrawLine(pen, new Point(x1, y1), new Point(x2, y2));
        double dx = x2 - x1, dy = y2 - y1, len = Math.Sqrt(dx * dx + dy * dy);
        if (len < 1) return;
        double ux = dx / len, uy = dy / len, nx = -uy, ny = ux;
        ctx.DrawLine(pen, new Point(x2, y2), new Point(x2 - ux * 8 + nx * 4, y2 - uy * 8 + ny * 4));
        ctx.DrawLine(pen, new Point(x2, y2), new Point(x2 - ux * 8 - nx * 4, y2 - uy * 8 - ny * 4));
    }

    static void Label(DrawingContext ctx, string s, double size, double cx, double cy, IBrush brush)
    {
        var ft = new FormattedText(s, CultureInfo.InvariantCulture, FlowDirection.LeftToRight,
                                   Typeface.Default, size, brush);
        ctx.DrawText(ft, new Point(cx - ft.Width / 2, cy - ft.Height / 2));
    }
}
