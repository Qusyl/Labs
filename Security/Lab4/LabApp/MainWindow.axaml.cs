using Avalonia.Controls;
using Avalonia.Interactivity;
using Source;
using Source.FileSystem;

namespace LabApp;

public record DiagramItem(string Title, TickStep Step, string Tail = "")
{
    public string Description =>
        $"биты связи: {string.Concat(Step.Taps)};  {Step.Formula};  выход = {Step.Out};  " +
        $"состояние после: {string.Concat(Step.After)}{Tail}";
}

public partial class MainWindow : Window
{
    private GammaCipher? _cipher;

    public MainWindow()
    {
        InitializeComponent();
        try { _cipher = new GammaCipher(); }
        catch (Exception ex) { ResultText.Text = "Не удалось загрузить алфавит: " + ex.Message; }
    }

    private void Encrypt_Click(object? sender, RoutedEventArgs e)
    {
        if (_cipher == null) return;
        try
        {
            var word = WordBox.Text ?? "";
            if (word.Length == 0) { ResultText.Text = "Введите слово."; return; }

            var bits = _cipher.TextToBits(word);
            var r5 = new Lfsr(FileLoader.LoadReg5(), FileLoader.LoadReg5Taps());
            var r17 = new Lfsr(FileLoader.LoadReg17(), FileLoader.LoadReg17Taps());

            var gamma = new int[bits.Length];
            var items = new List<DiagramItem>();
            for (int i = 0; i < bits.Length; i++)
            {
               
                    var a = r5.Next();


                int parity = a.State.Aggregate(0, (x, y) => x ^ y);
                int shifts = 1 + parity;

                    var subs = new List<TickStep>();
                    for (int s = 0; s < shifts; s++) subs.Add(r17.Next());

                    gamma[i] = subs[^1].Out;

                    if (i < gamma.Length)
                    {
                         string sumFormula =
        $";  сумма разрядов {string.Concat(a.State)} = {string.Join("⊕", a.State)} = {parity}";

    items.Add(new(
        $"Регистр 5, такт {i + 1}: сумма разрядов = {parity}, регистр 17 сдвигается на {shifts}",
        a,
        sumFormula));

    for (int s = 0; s < shifts; s++)
        items.Add(new($"Регистр 17, такт {i + 1}, сдвиг {s + 1} из {shifts}", subs[s],
                      s == shifts - 1 ? $"   ← бит гаммы = {gamma[i]}" : ""));
                    }
            }

            var cipherBits = _cipher.Xor(bits, gamma);
            FileSaver.SaveCipherText(_cipher.BitsToText(cipherBits));
            ResultText.Text =
                $"Биты текста: {string.Concat(bits)}\n" +
                $"Биты гаммы:  {string.Concat(gamma)}\n" +
                $"Биты шифра:  {string.Concat(cipherBits)}\n" +
                $"Шифртекст:   {_cipher.BitsToText(cipherBits)}";
            Diagrams.ItemsSource = items;
            _cipher.Decrypt(cipherBits, gamma);
        }
        catch (Exception ex) { ResultText.Text = "Ошибка: " + ex.Message; }
    }
}
