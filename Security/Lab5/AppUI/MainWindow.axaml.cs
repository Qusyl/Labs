using System;
using System.Collections.Generic;
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Source;
using Source.FileSystem;
using AesStep = Source.AesStep.AesStep;

namespace AppUI;

public partial class MainWindow : Window
{
    private const string MonoFont =
        "Consolas, Cascadia Mono, Menlo, DejaVu Sans Mono, monospace";

    
    private const bool KeyFileIsRowMajor = true;

    private class BlockData
    {
        public int Number { get; init; }
        public byte[] Data { get; init; } = [];
        public List<AesStep> Steps { get; init; } = [];
    }

    private readonly List<BlockData> _blocks = [];

    private int _currentBlock = -1;
    private int _currentStep = -1;


    public MainWindow()
    {
        InitializeComponent();

        ShowEmptyMatrix();
        UpdateBlockButtons();
        UpdateStepButtons();

        try
        {
            KeyInput.Text = BitConverter.ToString(LoadKey()).Replace("-", "");
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }


    private byte[] LoadKey()
    {
        byte[] fileKey = FileLoader.LoadKey();

        if (fileKey.Length != 16)
            throw new ArgumentException("Ключ в файле должен содержать 16 байт.");

        if (!KeyFileIsRowMajor)
            return fileKey;

        byte[] key = new byte[16];

        for (int row = 0; row < 4; row++)
            for (int col = 0; col < 4; col++)
                key[col * 4 + row] = fileKey[row * 4 + col];

        return key;
    }


  

    private void EncryptButton_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            string text = TextInput.Text ?? "";

            if (string.IsNullOrWhiteSpace(text))
            {
                ShowError("Введите текст.");
                return;
            }

            byte[] key = LoadKey();
            KeyInput.Text = BitConverter.ToString(key).Replace("-", "");

            byte[] textBytes = Encoding.UTF8.GetBytes(text);
            byte[] paddedText = ApplyPkcs7Padding(textBytes);

            _blocks.Clear();

            int blockCount = paddedText.Length / 16;
            var aes = new Aes();

            for (int blockIndex = 0; blockIndex < blockCount; blockIndex++)
            {
                byte[] block = new byte[16];
                Array.Copy(paddedText, blockIndex * 16, block, 0, 16);

                List<AesStep> steps = aes.Encrypt(block, key);

                _blocks.Add(new BlockData
                {
                    Number = blockIndex + 1,
                    Data = block,
                    Steps = steps
                });
            }

            _currentBlock = 0;
            _currentStep = 0;

            ShowStatus(
                $"UTF-8: {textBytes.Length} байт → " +
                $"{_blocks.Count} блок(ов) по 16 байт");

            ShowCurrentStep();
            UpdateBlockButtons();
        }
        catch (Exception ex)
        {
            ShowError(ex.Message);
        }
    }


    private byte[] ApplyPkcs7Padding(byte[] data)
    {
        int paddingSize = 16 - (data.Length % 16);

        byte[] result = new byte[data.Length + paddingSize];
        Array.Copy(data, result, data.Length);

        for (int i = data.Length; i < result.Length; i++)
            result[i] = (byte)paddingSize;

        return result;
    }


    private void NextStepButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!HasData())
            return;

        if (_currentStep >= _blocks[_currentBlock].Steps.Count - 1)
            return;

        _currentStep++;
        ShowCurrentStep();
    }


    private void PreviousStepButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!HasData() || _currentStep <= 0)
            return;

        _currentStep--;
        ShowCurrentStep();
    }


    private void NextBlockButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!HasData() || _currentBlock >= _blocks.Count - 1)
            return;

        _currentBlock++;
        _currentStep = 0;

        ShowCurrentStep();
        UpdateBlockButtons();
    }


    private void PreviousBlockButton_Click(object? sender, RoutedEventArgs e)
    {
        if (!HasData() || _currentBlock <= 0)
            return;

        _currentBlock--;
        _currentStep = 0;

        ShowCurrentStep();
        UpdateBlockButtons();
    }


 
    private void ShowCurrentStep()
    {
        if (!HasData())
            return;

        BlockData block = _blocks[_currentBlock];

        if (_currentStep < 0 || _currentStep >= block.Steps.Count)
            return;

        AesStep step = block.Steps[_currentStep];

        BlockTitle.Text = $"Блок {_currentBlock + 1} из {_blocks.Count}";
        StepTitle.Text = step.Name;
        StepDescription.Text = step.Description;

        ShowMatrices(step);
        ShowDetails(step);
        UpdateProgress();
        UpdateStepList();
        UpdateStepButtons();
    }


    private void ShowDetails(AesStep step)
    {
        DetailsText.Text = step.Explanation;
        TraceText.Text = string.Join(Environment.NewLine, step.Trace);
        DetailsScroll.ScrollToHome();
    }


    
    private void ShowEmptyMatrix()
    {
        MatricesPanel.Children.Clear();
        MatricesPanel.Children.Add(BuildMatrix("State", null));
    }


    private void ShowMatrices(AesStep step)
    {
        MatricesPanel.Children.Clear();

        if (step.Before != null)
        {
            MatricesPanel.Children.Add(BuildMatrix(step.BeforeLabel, step.Before));

            if (step.Operand != null)
            {
                MatricesPanel.Children.Add(CreateSymbol(step.Operator));
                MatricesPanel.Children.Add(BuildMatrix(step.OperandLabel, step.Operand));
                MatricesPanel.Children.Add(CreateSymbol("="));
            }
            else
            {
                MatricesPanel.Children.Add(CreateSymbol("→"));
            }
        }

       
        MatricesPanel.Children.Add(BuildMatrix(step.AfterLabel, step.State, step.Before));
    }


    private Control BuildMatrix(string caption, byte[,]? m, byte[,]? compareWith = null)
    {
        var grid = new Grid
        {
            Width = 220,
            Height = 220,
            RowDefinitions = new RowDefinitions("*,*,*,*"),
            ColumnDefinitions = new ColumnDefinitions("*,*,*,*")
        };

        for (int r = 0; r < 4; r++)
        {
            for (int c = 0; c < 4; c++)
            {
                bool changed = m != null && compareWith != null && m[r, c] != compareWith[r, c];

                Border cell = CreateMatrixCell(
                    m == null ? "--" : m[r, c].ToString("X2"),
                    changed);

                Grid.SetRow(cell, r);
                Grid.SetColumn(cell, c);
                grid.Children.Add(cell);
            }
        }

        var title = new TextBlock
        {
            Text = caption,
            Foreground = new SolidColorBrush(Color.Parse("#AAAAAA")),
            HorizontalAlignment = HorizontalAlignment.Center,
            Margin = new Avalonia.Thickness(0, 0, 0, 8)
        };

        var panel = new StackPanel();
        panel.Children.Add(title);
        panel.Children.Add(grid);
        return panel;
    }


    private static TextBlock CreateSymbol(string symbol) => new()
    {
        Text = symbol,
        FontSize = 36,
        Foreground = Brushes.White,
        VerticalAlignment = VerticalAlignment.Center,
        Margin = new Avalonia.Thickness(0, 24, 0, 0)
    };


    private Border CreateMatrixCell(string value, bool highlighted = false)
    {
        var text = new TextBlock
        {
            Text = value,
            FontSize = 22,
            Foreground = Brushes.White,
            FontFamily = new FontFamily(MonoFont),
            FontWeight = FontWeight.Bold,
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };

        return new Border
        {
            BorderBrush = new SolidColorBrush(Color.Parse(highlighted ? "#B8A9FF" : "#555555")),
            BorderThickness = new Avalonia.Thickness(highlighted ? 2 : 1),
            Background = new SolidColorBrush(Color.Parse(highlighted ? "#5B4BC4" : "#303030")),
            Child = text,
            Margin = new Avalonia.Thickness(2),
            CornerRadius = new Avalonia.CornerRadius(5)
        };
    }



    private void UpdateStepList()
    {
        StepsPanel.Children.Clear();

        if (!HasData())
            return;

        BlockData block = _blocks[_currentBlock];

        for (int i = 0; i < block.Steps.Count; i++)
        {
            AesStep step = block.Steps[i];
            bool isCurrent = i == _currentStep;

            string prefix = i < _currentStep ? "✓ "
                          : isCurrent ? "● "
                          : "  ";

            TextBlock text = new()
            {
                Text = $"{prefix}{i + 1}. {step.Name}",
                FontSize = 14,
                TextWrapping = TextWrapping.Wrap,
                Foreground = new SolidColorBrush(Color.Parse("#EDEDF7")),
                VerticalAlignment = VerticalAlignment.Center
            };

            Border border = new()
            {
                Padding = new Avalonia.Thickness(10),
                CornerRadius = new Avalonia.CornerRadius(6),
                Background = new SolidColorBrush(
                    Color.Parse(isCurrent ? "#9898EA" : "#50508A")),
                Child = text
            };

            StepsPanel.Children.Add(border);
        }
    }


    private void UpdateProgress()
    {
        if (!HasData())
        {
            ProgressText.Text = "0 / 0";
            return;
        }

        ProgressText.Text =
            $"Шаг {_currentStep + 1} / {_blocks[_currentBlock].Steps.Count}";
    }


    private void UpdateBlockButtons()
    {
        bool hasBlocks = _blocks.Count > 0;

        PreviousBlockButton.IsEnabled = hasBlocks && _currentBlock > 0;
        NextBlockButton.IsEnabled = hasBlocks && _currentBlock < _blocks.Count - 1;

        BlockProgressText.Text = hasBlocks
            ? $"Блок {_currentBlock + 1} / {_blocks.Count}"
            : "Блок 0 / 0";
    }


    private void UpdateStepButtons()
    {
        if (!HasData())
        {
            PreviousStepButton.IsEnabled = false;
            NextStepButton.IsEnabled = false;
            return;
        }

        PreviousStepButton.IsEnabled = _currentStep > 0;
        NextStepButton.IsEnabled = _currentStep < _blocks[_currentBlock].Steps.Count - 1;
    }


    private bool HasData()
    {
        return _blocks.Count > 0 &&
               _currentBlock >= 0 &&
               _currentBlock < _blocks.Count;
    }


 

    private void ShowStatus(string message, bool isError = false)
    {
        InputStatus.Text = isError ? $"Ошибка: {message}" : message;
        InputStatus.Foreground = new SolidColorBrush(
            Color.Parse(isError ? "#F48771" : "#AAAAAA"));
    }


    private void ShowError(string message) => ShowStatus(message, true);
}