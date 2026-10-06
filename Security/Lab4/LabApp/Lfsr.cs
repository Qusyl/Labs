namespace LabApp;


public record TickStep(int Index, int[] State, int[] Taps, int Fb, int Out, int[] After, string Formula);


public class Lfsr
{
    private readonly int[] _s;
    private readonly int[] _taps;
    private int _tick;

    
    public Lfsr(int[] init, int[] tapIndices)
    {
        int n = init.Length;
        _taps = new int[n - 1];
        foreach (var k in tapIndices)
        {
            if (k < 0 || k > n - 1)
                throw new ArgumentException($"Отвод {k} вне диапазона 0..{n - 2} для регистра из {n} ячеек.");
            if (k < n - 1) _taps[k] = 1;
        }
        _s = (int[])init.Clone();
    }

   
    public string TapBits => string.Concat(_taps);

    public TickStep Next()
    {
        int n = _s.Length;
        var before = (int[])_s.Clone();
        var names = new List<string>();
        var vals = new List<int>();
        for (int k = 0; k < n - 1; k++)
            if (_taps[k] == 1) { names.Add($"r{k + 1}"); vals.Add(before[k]); }
        names.Add($"r{n}");
        vals.Add(before[n - 1]);

        int fb = vals.Aggregate(0, (a, b) => a ^ b);
        for (int i = n - 1; i > 0; i--) _s[i] = _s[i - 1];
        _s[0] = fb;

        string formula = $"{string.Join("⊕", names)} = {string.Join("⊕", vals)} = {fb}";
        return new TickStep(++_tick, before, _taps, fb, before[n - 1], (int[])_s.Clone(), formula);
    }
}
