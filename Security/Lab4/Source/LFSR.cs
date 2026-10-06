using System.Buffers.Text;
using System.Text;
using Source.BinConverter;
using Source.FileSystem;

namespace Source;

public class LFSR
{
    private int[] _reg5 = new int[5];
    private int[] _reg17 = new int[17];


    private readonly int[] _reg5Taps;
    private readonly int[] _reg17Taps;


    private const int DetailedLogBits = 5;


    private const int LogWidth = 78;

    public LFSR()
    {
        _reg5  = FileLoader.LoadReg5();
        _reg17 = FileLoader.LoadReg17();
        _reg5Taps = FileLoader.LoadReg5Taps();
        _reg17Taps = FileLoader.LoadReg17Taps();
    }

    public int[] GenerateGamma(int length)
    {
        int[] gamma = new int[length];

        var log = new StringBuilder();

        WriteHeader(log, "ГЕНЕРАЦИЯ ГАММЫ", length);

        log.AppendLine("Начальные состояния регистров:");
        log.AppendLine();
        AppendRegisterTable(log, "Регистр 5",  _reg5,  _reg5Taps,  highlightTaps: true);
        log.AppendLine();
        AppendRegisterTable(log, "Регистр 17", _reg17, _reg17Taps, highlightTaps: true);
        log.AppendLine();

        for (int i = 0; i < length; i++)
        {
            gamma[i] = NextGammaBit(log, i, i < DetailedLogBits);
        }

        WriteFooter(log, "ГЕНЕРАЦИЯ ГАММЫ ЗАВЕРШЕНА");

        FileSaver.SaveGamma(gamma, log.ToString());
        return gamma;
    }

    
    private static int XorAllBits(int[] reg)
    {
        int sum = 0;
        for (int i = 0; i < reg.Length; i++)
            sum ^= reg[i];
        return sum;
    }

    private int NextGammaBit(StringBuilder log, int bitIndex, bool detailed)
    {
        if (detailed)
        {
            WriteHeader(log, $"БИТ ГАММЫ #{bitIndex + 1}", bitIndex);
        }

       
        int control = XorAllBits(_reg5);

        if (detailed)
        {
            log.AppendLine();
            AppendRegisterTable(log, "Регистр 5 (до сдвига)", _reg5, _reg5Taps, highlightTaps: true);

            log.AppendLine();
            log.AppendLine($"  Сумма по модулю 2 всех разрядов R5: " +
                           $"{string.Join(" ^ ", _reg5)} = {control}");
            log.AppendLine($"  → Управляющий сигнал = {control}  " +
                           $"({(control == 1 ? "R17 сделает 2 такта" : "R17 сделает 1 такт")})");
        }

        if (detailed)
        {
            log.AppendLine();
            log.AppendLine("  ── Такт регистра 5 ──");
        }

        int r5Out = Tact(_reg5, _reg5Taps, log, detailed);

        if (detailed)
        {
            AppendRegisterTable(log, "Регистр 5 (после сдвига)", _reg5, _reg5Taps, highlightTaps: false);
            log.AppendLine($"  Выпавший бит R5: {r5Out}");
        }


        int tactCount = (control == 1) ? 2 : 1;
        int gammaBit = 0;

        if (detailed)
        {
            log.AppendLine();
            log.AppendLine($"  ── Такты регистра 17 (всего: {tactCount}) ──");
        }

        for (int t = 0; t < tactCount; t++)
        {
            if (detailed)
            {
                log.AppendLine();
                log.AppendLine($"  [Такт R17 {t + 1}/{tactCount}]");
            }

            gammaBit = Tact(_reg17, _reg17Taps, log, detailed);
        }

        if (detailed)
        {
            AppendRegisterTable(log, "Регистр 17 (после тактов)", _reg17, _reg17Taps, highlightTaps: false);
            log.AppendLine();
            log.AppendLine($"  ▶ БИТ ГАММЫ #{bitIndex + 1} = {gammaBit}");
            WriteFooter(log, $"КОНЕЦ БИТА #{bitIndex + 1}", bitIndex);
        }

        return gammaBit;
    }


    private int Tact(int[] reg, int[] taps, StringBuilder log, bool detailed)
    {
        int tapA = taps[0];
        int tapB = taps[1];

        int feedback = reg[tapA] ^ reg[tapB];
        int outBit   = reg[0];

        if (detailed)
        {
            log.AppendLine($"    Отводы: [{tapA}]={reg[tapA]}, [{tapB}]={reg[tapB]}");
            log.AppendLine($"    Обратная связь = {reg[tapA]} ^ {reg[tapB]} = {feedback}");
            log.AppendLine($"    Выпадающий бит (R[0]) = {outBit}");
            log.AppendLine();
            AppendShiftDiagram(log, reg, feedback);
        }


        for (int i = 0; i < reg.Length - 1; i++)
        {
            reg[i] = reg[i + 1];
        }
        reg[reg.Length - 1] = feedback;

        if (detailed)
        {
            log.AppendLine();
            log.AppendLine($"    Результат сдвига: {FormatBits(reg)}");
        }

        return outBit;
    }


    private static void WriteHeader(StringBuilder log, string title, int index)
    {
        string inner = $" {title} ";
        int pad = Math.Max(0, LogWidth - inner.Length - 2);
        int left  = pad / 2;
        int right = pad - left;

        log.AppendLine();
        log.AppendLine("╔" + new string('═', LogWidth - 2) + "╗");
        log.AppendLine("║" + new string(' ', left) + inner + new string(' ', right) + "║");
        log.AppendLine("╚" + new string('═', LogWidth - 2) + "╝");
    }

    private static void WriteFooter(StringBuilder log, string title, int index = -1)
    {
        string inner = $" {title} ";
        int pad = Math.Max(0, LogWidth - inner.Length - 2);
        int left  = pad / 2;
        int right = pad - left;

        log.AppendLine("┌" + new string('─', LogWidth - 2) + "┐");
        log.AppendLine("│" + new string(' ', left) + inner + new string(' ', right) + "│");
        log.AppendLine("└" + new string('─', LogWidth - 2) + "┘");
        log.AppendLine();
    }


    private static void AppendRegisterTable(
        StringBuilder log,
        string title,
        int[] reg,
        int[] taps,
        bool highlightTaps)
    {
        log.AppendLine($"  {title}:");
        log.AppendLine();


        log.Append("    idx |");
        for (int i = 0; i < reg.Length; i++)
            log.Append($" {i,3} ");
        log.AppendLine("|");


        log.Append("    ----+");
        for (int i = 0; i < reg.Length; i++)
            log.Append("----");
        log.AppendLine("+");


        log.Append("    val |");
        for (int i = 0; i < reg.Length; i++)
            log.Append($" {reg[i],3} ");
        log.AppendLine("|");


        if (highlightTaps)
        {
            log.Append("    tap |");
            for (int i = 0; i < reg.Length; i++)
            {
                bool isTap = Array.IndexOf(taps, i) >= 0;
                log.Append(isTap ? "  ^ " : "    ");
            }
            log.AppendLine("|");
        }

        log.AppendLine();
        log.AppendLine($"    Битовая строка: {FormatBits(reg)}");
    }


    private static void AppendShiftDiagram(StringBuilder log, int[] reg, int feedback)
    {
        log.AppendLine("    ┌── Диаграмма сдвига ──────────────────────────────────");
        log.AppendLine($"    │ было:  {FormatBits(reg)}   (R[0]={reg[0]} выпадает)");


        log.Append("    │ сдвиг: ");
        for (int i = 0; i < reg.Length; i++)
            log.Append(" ←  ");
        log.AppendLine();

        log.AppendLine($"    │ ОС:    {feedback}  (подаётся в последний разряд)");
        log.AppendLine("    └──────────────────────────────────────────────────────");
    }

    private static string FormatBits(int[] reg)
    {
        var sb = new StringBuilder(reg.Length + 2);
        sb.Append('[');
        for (int i = 0; i < reg.Length; i++)
        {
            sb.Append(reg[i]);
            if (i < reg.Length - 1) sb.Append(' ');
        }
        sb.Append(']');
        return sb.ToString();
    }
}