using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Interactivity;
using Source.Encoder;
using Source.FileSystem;
using System;
using System.Threading;
using System.Threading.Tasks;
using Source;

namespace AppUI;

public partial class MainWindow : Window
{
    private readonly PolybEncoder _encoder;

    private readonly CryptoAnalyzator _crAnalyzer;
    private readonly char[] _table;

    private const int Columns = 7;
    private const int Shift = 7;
    private const int StepDelayMs = 2000;

    private static readonly IBrush SourceHighlight = Brushes.LightBlue;
    private static readonly IBrush TargetHighlight = Brushes.LightGreen;

    private CancellationTokenSource? _animationCts;

    public MainWindow()
    {
        InitializeComponent();

        _encoder = new PolybEncoder();
        _crAnalyzer = new CryptoAnalyzator();
        _table = FileLoader.LoadTable();

        BuildTable();

        MessageBox.TextChanged += MessageBox_TextChanged;
    }

    private void BuildTable()
    {
        PolybiusGrid.ColumnDefinitions.Clear();
        PolybiusGrid.RowDefinitions.Clear();
        PolybiusGrid.Children.Clear();

        int rows = (_table.Length + Columns - 1) / Columns;

        for (int column = 0; column < Columns; column++)
            PolybiusGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

        for (int row = 0; row < rows; row++)
            PolybiusGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        for (int i = 0; i < _table.Length; i++)
        {
            int row = i / Columns;
            int column = i % Columns;

            var text = new TextBlock
            {
                Text = DisplayCharacter(_table[i]),
                FontSize = 24,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };

            var border = new Border
            {
                Width = 75,
                Height = 60,
                BorderBrush = Brushes.Gray,
                BorderThickness = new Thickness(1),
                Child = text,
                Tag = i
            };

            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);

            PolybiusGrid.Children.Add(border);
        }
    }

    private static string DisplayCharacter(char character)
    {
        return character == ' ' ? "␠" : character.ToString();
    }

    private void MessageBox_TextChanged(object? sender, TextChangedEventArgs e)
    {
        if (_animationCts != null)
            return;

        ShowCurrentCharacter();
    }

    private void ShowCurrentCharacter()
    {
        ClearHighlights();

        string message = MessageBox.Text ?? "";

        if (message.Length == 0)
        {
            TransitionText.Text = "Введите символ для просмотра смещения";
            return;
        }

        char character = message[0];
        int index = Array.IndexOf(_table, character);

        if (index == -1)
        {
            TransitionText.Text =
                $"Символ «{DisplayCharacter(character)}» отсутствует в таблице";
            return;
        }

        int newIndex = (index + Shift) % _table.Length;

        HighlightCell(index, SourceHighlight);
        HighlightCell(newIndex, TargetHighlight);

        char encrypted = _table[newIndex];

        
    }

    private void HighlightCell(int index, IBrush brush)
    {
        foreach (var child in PolybiusGrid.Children)
        {
            if (child is Border border &&
                border.Tag is int cellIndex &&
                cellIndex == index)
            {
                border.Background = brush;
            }
        }
    }

    private void ClearHighlights()
    {
        foreach (var child in PolybiusGrid.Children)
        {
            if (child is Border border)
            {
                border.Background = Brushes.Transparent;
            }
        }
    }

    private async void Encrypt_Click(object? sender, RoutedEventArgs e)
    {
        string message = MessageBox.Text ?? "";

        if (string.IsNullOrEmpty(message))
            return;

        _animationCts?.Cancel();
        _animationCts = new CancellationTokenSource();
        var token = _animationCts.Token;

        var button = sender as Button;
        if (button != null) button.IsEnabled = false;
        MessageBox.IsEnabled = false;

        try
        {
            string encrypted = _encoder.Encrypt(message);

            _crAnalyzer.Scan();

            await AnimateTransformAsync(message, isEncrypt: true, token);

            ResultBox.Text = encrypted;
            TransitionText.Text = "Шифрование завершено";
            ClearHighlights();
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            if (button != null) button.IsEnabled = true;
            MessageBox.IsEnabled = true;
            _animationCts = null;
        }
    }

    private async void Decrypt_Click(object? sender, RoutedEventArgs e)
    {
        string message = FileLoader.LoadEncrypted();

        if (string.IsNullOrEmpty(message))
            return;

        _animationCts?.Cancel();
        _animationCts = new CancellationTokenSource();
        var token = _animationCts.Token;

        var button = sender as Button;
        if (button != null) button.IsEnabled = false;
        MessageBox.IsEnabled = false;

        try
        {
            string decrypted = _encoder.Decrypt(message);

            await AnimateTransformAsync(message, isEncrypt: false, token);

            ResultBox.Text = decrypted;
            TransitionText.Text = "Дешифрование завершено";
            ClearHighlights();
        }
        catch (OperationCanceledException)
        {
        }
        finally
        {
            if (button != null) button.IsEnabled = true;
            MessageBox.IsEnabled = true;
            _animationCts = null;
        }
    }

    private async Task AnimateTransformAsync(string source, bool isEncrypt, CancellationToken token)
    {
        var revealed = new char[source.Length];

        for (int i = 0; i < source.Length; i++)
        {
            token.ThrowIfCancellationRequested();

            ClearHighlights();

            char character = source[i];
            int index = Array.IndexOf(_table, character);

            if (index == -1)
            {
                revealed[i] = character;
                TransitionText.Text =
                    $"[{i + 1}/{source.Length}] «{DisplayCharacter(character)}» — нет в таблице, копируется как есть";
            }
            else
            {
                int newIndex = isEncrypt
                    ? (index + Shift) % _table.Length
                    : ((index - Shift) % _table.Length + _table.Length) % _table.Length;

                char result = _table[newIndex];

                HighlightCell(index, SourceHighlight);
                HighlightCell(newIndex, TargetHighlight);

                revealed[i] = result;

                
               
            }

            ResultBox.Text = new string(revealed, 0, i + 1);

            await Task.Delay(StepDelayMs, token);
        }
    }

    private void Clear_Click(object? sender, RoutedEventArgs e)
    {
        _animationCts?.Cancel();
        _animationCts = null;

        MessageBox.Text = "";
        ResultBox.Text = "";

        ClearHighlights();

        TransitionText.Text = "Введите символ для просмотра смещения";
    }
}