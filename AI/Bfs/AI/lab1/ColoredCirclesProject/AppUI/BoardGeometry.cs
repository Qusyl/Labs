using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;
using Avalonia;

namespace AppUI
{
    public static class BoardGeometry
{
    public static Point[] Build(
        IReadOnlyDictionary<string, int[]> topology,
        IReadOnlyDictionary<string, RingView> rings,
        int count)
    {
        var pts = new Point?[count];

        // 1. Общие позиции: точки пересечения соседних кругов
        var names = topology.Keys.OrderBy(n => rings[n].Center.X).ToList();
        for (int i = 0; i < names.Count; i++)
        for (int j = i + 1; j < names.Count; j++)
        {
            string a = names[i], b = names[j];
            int[] shared = topology[a].Intersect(topology[b]).ToArray();
            if (shared.Length == 0) continue;
            if (shared.Length != 2)
                throw new InvalidOperationException(
                    $"{a} и {b} делят {shared.Length} позиций, у двух окружностей максимум 2 общие точки");

            var (top, bottom) = Intersect(rings[a], rings[b]);

            // Левое кольцо a: пересечения у него на правой стороне.
            // В порядке списка "первая" из двух общих позиций идёт по ходу кольца.
            // По часовой это верхняя точка, против часовой нижняя.
            int[] list = topology[a];
            int x = shared[0], y = shared[1];
            int first = Next(list, x) == y ? x
                      : Next(list, y) == x ? y
                      : throw new InvalidOperationException(
                            $"В кольце {a} общие позиции {x} и {y} должны идти подряд");
            int second = first == x ? y : x;

            bool firstIsTop = rings[a].Clockwise;
            pts[first] = firstIsTop ? top : bottom;
            pts[second] = firstIsTop ? bottom : top;
        }

        // 2. Частные позиции: равномерно по дуге между фиксированными
        foreach (var (name, list) in topology)
        {
            var ring = rings[name];
            int n = list.Length;

            int s = Array.FindIndex(list, p => pts[p] is not null);
            if (s < 0)
            {
                pts[list[0]] = OnCircle(ring, -Math.PI / 2);
                s = 0;
            }

            double sign = ring.Clockwise ? 1 : -1;
            var run = new List<int>();
            int prev = list[s];

            for (int i = 1; i <= n; i++)
            {
                int p = list[(s + i) % n];
                if (pts[p] is null) { run.Add(p); continue; }

                double a0 = Angle(ring.Center, pts[prev]!.Value);
                double a1 = Angle(ring.Center, pts[p]!.Value);
                double delta = Norm(sign * (a1 - a0));

                for (int k = 0; k < run.Count; k++)
                    pts[run[k]] = OnCircle(ring, a0 + sign * delta * (k + 1) / (run.Count + 1));

                run.Clear();
                prev = p;
            }
        }

        for (int i = 0; i < count; i++)
            if (pts[i] is null)
                throw new InvalidOperationException($"Позиция {i} не входит ни в одно кольцо");

        return pts.Select(p => p!.Value).ToArray();
    }

    public static double Angle(Point center, Point p) =>
        Math.Atan2(p.Y - center.Y, p.X - center.X);

    /// <summary>Приводит угол в диапазон (0, 2π].</summary>
    public static double Norm(double x)
    {
        x %= 2 * Math.PI;
        if (x <= 1e-9) x += 2 * Math.PI;
        return x;
    }

    private static int Next(int[] list, int value) =>
        list[(Array.IndexOf(list, value) + 1) % list.Length];

    private static Point OnCircle(RingView r, double angle) =>
        new(r.Center.X + r.Radius * Math.Cos(angle),
            r.Center.Y + r.Radius * Math.Sin(angle));

    private static (Point top, Point bottom) Intersect(RingView a, RingView b)
    {
        double dx = b.Center.X - a.Center.X, dy = b.Center.Y - a.Center.Y;
        double d = Math.Sqrt(dx * dx + dy * dy);

        if (d >= a.Radius + b.Radius || d <= Math.Abs(a.Radius - b.Radius))
            throw new InvalidOperationException("Окружности не пересекаются в двух точках");

        double l = (a.Radius * a.Radius - b.Radius * b.Radius + d * d) / (2 * d);
        double h = Math.Sqrt(a.Radius * a.Radius - l * l);
        double mx = a.Center.X + l * dx / d, my = a.Center.Y + l * dy / d;

        var p1 = new Point(mx + h * dy / d, my - h * dx / d);
        var p2 = new Point(mx - h * dy / d, my + h * dx / d);
        return p1.Y < p2.Y ? (p1, p2) : (p2, p1);
    }
}
}