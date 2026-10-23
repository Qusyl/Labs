using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Source;
 
namespace AppUI;
 
public partial class MainWindow : Window
{
    private readonly Egsa _egsa = new();
 
    public MainWindow()
    {
        InitializeComponent();
    }
 
    private void SignButton_Click(object? sender, RoutedEventArgs e)
    {
        string message = SenderMessage.Text ?? string.Empty;
 
        if (string.IsNullOrWhiteSpace(message))
        {
            SenderStatus.Text = "Введите сообщение.";
            return;
        }
 
        try
        {
            string signature = _egsa.Sign(message);
 
            SignatureText.Text = signature;
            SenderStatus.Text = "Сообщение подписано.";
        }
        catch (Exception ex)
        {
            SenderStatus.Text = $"Ошибка подписания: {ex.Message}";
            _egsa.SignLog.Add("Ошибка", ex.Message);
        }
        finally
        {
           
            SignLogText.Text = _egsa.SignLog.ToString();
        }
    }
 
    private void SendButton_Click(object? sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SenderMessage.Text) ||
            string.IsNullOrWhiteSpace(SignatureText.Text))
        {
            SenderStatus.Text = "Сначала введите сообщение и создайте подпись.";
            return;
        }
 
        ReceiverMessage.Text = SenderMessage.Text;
        ReceiverSignature.Text = SignatureText.Text;
 
       
        VerifyLogText.Text = string.Empty;
 
        VerifyStatus.Text = "Данные получены. Проверьте подпись.";
        SenderStatus.Text = "Сообщение и подпись переданы.";
    }
 
    private void VerifyButton_Click(object? sender, RoutedEventArgs e)
    {
        string message = ReceiverMessage.Text ?? string.Empty;
        string signature = ReceiverSignature.Text ?? string.Empty;
 
        string[] parts = signature.Split(
            (char[]?)null,
            StringSplitOptions.RemoveEmptyEntries);
 
        if (string.IsNullOrWhiteSpace(message) || parts.Length != 2)
        {
            VerifyStatus.Text = "Введите сообщение и подпись из двух чисел.";
            return;
        }
 
        try
        {
            bool isValid = _egsa.Verify(message, parts);
 
            VerifyStatus.Text = isValid
                ? "✓ Подпись корректна. Сообщение не изменено."
                : "✗ Подпись недействительна или сообщение изменено.";
        }
        catch (Exception ex)
        {
            VerifyStatus.Text = $"Ошибка проверки: {ex.Message}";
            _egsa.VerifyLog.Add("Ошибка", ex.Message);
        }
        finally
        {
            VerifyLogText.Text = _egsa.VerifyLog.ToString();
        }
    }
 
    private async void SaveSignLogButton_Click(object? sender, RoutedEventArgs e)
    {
        await SaveLogAsync("Сохранить журнал подписания", "sign-log.txt", SignLogText.Text, SenderStatus);
    }
 
    private async void SaveVerifyLogButton_Click(object? sender, RoutedEventArgs e)
    {
        await SaveLogAsync("Сохранить журнал проверки", "verify-log.txt", VerifyLogText.Text, VerifyStatus);
    }
 
    private async Task SaveLogAsync(string title, string fileName, string? text, TextBlock status)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            status.Text = "Журнал пуст: сначала выполните операцию.";
            return;
        }
 
        try
        {
            var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
            {
                Title = title,
                SuggestedFileName = fileName,
                DefaultExtension = "txt"
            });
 
            if (file is null)
            {
                return; 
            }
 
            await using var stream = await file.OpenWriteAsync();
            stream.SetLength(0);
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(text);
 
            status.Text = "Журнал сохранён.";
        }
        catch (Exception ex)
        {
            status.Text = $"Не удалось сохранить журнал: {ex.Message}";
        }
    }
}
 