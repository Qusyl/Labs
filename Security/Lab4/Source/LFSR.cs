using System.Buffers.Text;
using System.Text;
using Source.BinConverter;
using Source.FileSystem;

namespace Source;

public class LFSR
{
    private int[] _reg5 = new int[5];
    private int[] _reg17 = new int[17];

    private int[] _reg5Taps = new[] { 4, 1 };

    private int[] _reg17Taps = new[]{ 16, 2 };

   

    public LFSR()
    {
        _reg5 = FileLoader.LoadReg5();
        _reg17 = FileLoader.LoadReg17();
    }

    public int[] GenerateGamma(int length)
    {
        int[] gamma = new int[length];

        var loggerReport = new StringBuilder();
        loggerReport.Append("======== Начало генерации гаммы ========\n");

        for (int i = 0; i < length; i++)
        {
            loggerReport.Append($"-------- {i+1} бит гаммы --------\n");
            gamma[i] = NextGammaBit(loggerReport);
        }
        loggerReport.Append("======== Конец генерации гаммы ========\n");

        FileSaver.SaveGamma(gamma, loggerReport.ToString());

        return gamma;
    }
    private int NextGammaBit(StringBuilder logger)
    {
        int control = 0;

        foreach (var reg in _reg5)
        {
            control ^= reg;
        }

        logger.Append($"Сумма разрядов предшествующего регистра (reg 5): {control}\n");

        logger.Append("Такт первого региста: \n");
        Tact(_reg5, _reg5Taps, logger);
        logger.Append("Начало работы второго регистра\n");

        int tactCount = (control == 1) ? 2 : 1;

        logger.Append($"Число тактов 2 регистра = {tactCount}\n");

        int gammaBit = 0;
        for (int i = 0; i < tactCount; i++)
        {
            logger.Append($"---- {i+1} такт регистра[2] ----\n");
            gammaBit = Tact(_reg17, _reg17Taps, logger);
        }
        logger.Append($"бит гаммы на выходе: {gammaBit}\n");

        return gammaBit;
    }
    private int Tact(int[] reg, int[] taps, StringBuilder logger)
    {
        var feedBack = 0;

        foreach (var tap in taps)
        {
            feedBack ^= reg[tap];
        }
        logger.Append($"1) XOR отводов = {feedBack}\n");

        var eldBit = reg[0];

        logger.Append($"2) Старший ( выпадающий ) бит = {eldBit}\n");
        logger.Append($"3) Сдвиг разрядов регистра: \n");
        logger.Append($"Было: ");
        for (int i = 0; i < reg.Length; i++)
        {
            logger.Append($"{reg[i]}");
        }
        logger.Append(Environment.NewLine);
        logger.Append($"Стало: ");
        for (int i = 0; i < reg.Length - 1; i++)
        {
            reg[i] = reg[i + 1];
            logger.Append($"{reg[i]}");
        }
        logger.Append(Environment.NewLine);
        reg[reg.Length - 1] = feedBack;
        logger.Append($"4) Добавление бита обратной связи: {feedBack}\n");
        for (int i = 0; i < reg.Length; i++)
        {
            logger.Append($"{reg[i]} ");
        }
        logger.Append(Environment.NewLine);
        logger.Append($"---- Конец такта регистра ----\n");
        return eldBit;
    }


}
