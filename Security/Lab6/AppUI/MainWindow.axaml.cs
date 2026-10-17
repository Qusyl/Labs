using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Source;

namespace AppUI;

public partial class MainWindow : Window
{
    private readonly Egsa _egsa = new();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void SignButton_Click(
        object? sender,
        RoutedEventArgs e)
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
        }
    }

    private void SendButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(SenderMessage.Text) ||
            string.IsNullOrWhiteSpace(SignatureText.Text))
        {
            SenderStatus.Text =
                "Сначала введите сообщение и создайте подпись.";
            return;
        }

       
        ReceiverMessage.Text = SenderMessage.Text;
        ReceiverSignature.Text = SignatureText.Text;

        VerifyStatus.Text = "Данные получены. Проверьте подпись.";
        SenderStatus.Text = "Сообщение и подпись переданы.";
    }

    private void VerifyButton_Click(
        object? sender,
        RoutedEventArgs e)
    {
        string message = ReceiverMessage.Text ?? string.Empty;
        string signature = ReceiverSignature.Text ?? string.Empty;

        string[] parts = signature.Split(
            (char[]?)null,
            StringSplitOptions.RemoveEmptyEntries);

        if (string.IsNullOrWhiteSpace(message) || parts.Length != 2)
        {
            VerifyStatus.Text =
                "Введите сообщение и подпись из двух чисел.";
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
            VerifyStatus.Text =
                $"Ошибка проверки: {ex.Message}";
        }
    }
}