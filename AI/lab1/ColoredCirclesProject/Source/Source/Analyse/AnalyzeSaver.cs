using System.Text;
namespace Source.Analyse
{
    public static class AnalyzeSaver
    {
        public static void SaveAnalyse(AnalyseReport report)
        {
            var sb = new StringBuilder();
            sb.Append($"Число итераций: {report.Iterations}\n");
            sb.Append($"Число (max) открытых узлов: {report.MaxOpenSize}\n");
            sb.Append($"Число (max) всех хранимых в памяти узлов: {report.MaxTotalSize}\n");
            sb.Append($"Текущее число открытых узлов ( на момент завершения поиска ): {report.CurrentOpenSize}\n");
           

            File.WriteAllText("Analyse/report.txt", sb.ToString());
        }
    }
}