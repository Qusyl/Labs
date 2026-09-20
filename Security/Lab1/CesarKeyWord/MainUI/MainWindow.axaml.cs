
namespace MainUI;

using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MainUI.Commons;
using Source.Commons;
using Source.Encryptor;
using System;

using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Numerics;

public partial class MainWindow : Window
{
    private ObservableCollection<string> _stages = new();

    private int[] _square;

    private int _currentStep = 0;

    private int _initialKey = 0;
    private bool _useManualKey = false;

    private int _userManualKey = 1;

    private bool _initialized = false;

    private MainEncryptor _encryptor = new MainEncryptor();

    private StageMode _currentMode;
    public MainWindow()
    {
        SetInitialKey();
        InitializeComponent();
        InitButtons();
        SetBasedInput();
        StagesList.ItemsSource = _stages;
        _square = FileLoader.LoadSquare();
        RenderMatrix();
        RenderCesarAlphabetOffset();
        _initialized = true;
    }
    private void SetBasedInput()
    {
        var path = Path.Combine(FilePath.PathInputs(), "Input.txt");

        var fileContent = File.ReadAllText(path);

        EncodedWord.Text = fileContent;
       
    }
    
    
   
    private void SetEncryptedInput()
    {
        var path = Path.Combine(FilePath.PathResults(), "Encrypted.txt");
        var fileContent = File.ReadAllText(path);
        EncodedWord.Text = fileContent;
       
    }
    private void InitButtons()
    {
        NextButton.Click += OnNextMove;
        BackButton.Click += OnBackMove;
        Modes.SelectionChanged += OnModeChange;
        UseKey.IsCheckedChanged += OnUserKeyUse;
        CesarKey.ValueChanged += OnUserKeyChanged;
    }

    private void OnUserKeyChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        var numeric = sender as NumericUpDown;
        if (_useManualKey == true)
        {
            _userManualKey = (int)numeric?.Value!;
            RenderCesarAlphabetOffset();
        }
        Reset();
        PreProcessed();
        
    }

    
    private void OnUserKeyUse(object? sender, RoutedEventArgs e)
    {
        _useManualKey = UseKey.IsChecked == true;
        if (_useManualKey)
        {
            _userManualKey = (int)(CesarKey.Value ?? 1);
        }
  
        Reset();
        PreProcessed();
        
        OnUpdateUI();
    }

    private void OnModeChange(object? sender, SelectionChangedEventArgs e)
    {
        if (!_initialized) return;
        if (Modes.SelectedItem is ComboBoxItem selected)
        {
            var selectedChice = selected.Content?.ToString();
            
            var selectedMode = selectedChice switch
            {
                "Закодировать" => StageMode.Encrypt,
                "Расшифровать" => StageMode.Decrypt,
                _ => throw new NotSupportedException("Нет такой операции"),
            };

            ChangeMode(selectedMode);
            Reset();

            if(selectedMode is StageMode.Encrypt)
            {
                SetBasedInput();
            }
            if(selectedMode is StageMode.Decrypt)
            {
                
                SetEncryptedInput();
            }
            PreProcessed();

            OnUpdateUI();
        }
    }
    private void ChangeMode(StageMode mode) => _currentMode = mode;
    private void OnBackMove(object? sender, RoutedEventArgs e)
    {
        if(_currentStep > 0)
        {
            _currentStep--;
        }

        OnUpdateUI();
    }

   
    private void OnNextMove(object? sender, RoutedEventArgs e) {
        if(_stages.Count == 0)
        {
            PreProcessed();
          
        }
        if(_currentStep < _stages.Count - 1)
        {
            _currentStep++;
        }
        OnUpdateUI();
    }

    private void PreProcessed()
    {
        int? key = _useManualKey ? _userManualKey : _initialKey;
        if (_currentMode is StageMode.Encrypt)
        {
            
            foreach (var stage in _encryptor.Encrypt(key))
            {
                _stages.Add(stage);
            }
        }
        if(_currentMode is StageMode.Decrypt)
        {
            foreach (var stage in _encryptor.Decrypt(key))
            {
                _stages.Add(stage);
            }
        }
    }

    private void Reset()
    {
        _stages.Clear();
        _currentStep = 0;
       
        EncodedWord.Text = "";
    }
    private void OnUpdateUI()
    {

        if (_stages.Count == 0)
        {
            return;
        }

        if (_currentStep < 0)
        {
            _currentStep = 0;
        }

        if (_currentStep >= _stages.Count)
        {
            _currentStep = _stages.Count - 1;
        }

        EncodedWord.Text = _stages[_currentStep];


    }

    private void RenderCesarAlphabetOffset()
    {
        var key = _useManualKey ? _userManualKey : _initialKey;
        (string alphabet, string alphabetOffset) alphabetWithOffset = CesarAlphabetOffset.ShowAlphabet(key);
        char[] primaryAlphabet = alphabetWithOffset.alphabet.ToArray();
        char[] withOffset = alphabetWithOffset.alphabetOffset.ToArray();
        Alphabet.Children.Clear();
        var brush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        for(int i =0; i < alphabetWithOffset.alphabet.Length; i++)
        {
            var alp = $"{primaryAlphabet[i]} -> {withOffset[i]}";
            Alphabet.Children.Add(
            new TextBlock
            {
                Text = alp,
                FontSize = 18,
                Foreground = brush,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
            }
            );
        }
    }
    private void RenderMatrix()
    {
        MatrixSquare.Children.Clear();
        var brush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
        foreach (var v in _square)
        {
            MatrixSquare.Children.Add(
                new TextBlock
                {
                    Text = v.ToString(),
                    FontSize = 18,
                    Foreground = brush,
                    HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                    VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                });
        }
    }

    private void SetInitialKey()
    {
        _initialKey =  FileLoader.LoadKey();
    }

}