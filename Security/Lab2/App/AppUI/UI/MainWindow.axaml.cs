
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Source.Utills;
using UI.Model;
namespace AppUI;

public partial class MainWindow : Window
{
    private string _message;

    private int _step = 0;

    private int _maxStep;

    private int _manualStep = 1;

    private bool _isEncryptor = false;

    private bool _isDecryptor = false;

    private ObservableCollection<char[]> _encoding = new();

    private ObservableCollection<char[]> _currentInnerDisk = new();

    private readonly Disk _eDisk = new Disk
    {
        Radius = 300,
        CenterX = 300,
        CenterY = 300
    };
    private readonly Disk _iDisk = new Disk
    {
        Radius = 250,
        CenterX = 300,
        CenterY = 300
    };
    private char[] _externalDisk;
    private char[] _internalDisk;
    

    private Source.Encryptor.AlbertEncryptor _encryptor;
    public MainWindow()
    {
        InitializeComponent();
        _encryptor = new Source.Encryptor.AlbertEncryptor();
        StepBoxNumeric.ValueChanged += OnStepChanged;
        InitExternalDisk();
        InitBasedInternalDisk();
       
    }

    private void OnStepChanged(object? sender, NumericUpDownValueChangedEventArgs e)
    {
        var numeric = sender as NumericUpDown;

        if(numeric?.Value is decimal d)
        {
            _manualStep = (int)d;
        }
        Reset();

        if (_isDecryptor)
        {
            Decrypt();
            UpdateUI();
        }
        if (_isEncryptor)
        {
            Encrypt();
            UpdateUI();
        }
    }

    public void RenderDisks()
    {
     DiskCanvas.Children.Clear();
    for (int i = 0; i < _externalDisk.Length; i++)
    {
        double angle = 2 * Math.PI * i / _externalDisk.Length;

        Avalonia.Point outerPoint = GetPoint(
            new Avalonia.Point(_eDisk.CenterX, _eDisk.CenterY),
            _eDisk.Radius - 20,
            angle);

        Avalonia.Point innerPoint = GetPoint(
            new Avalonia.Point(_iDisk.CenterX, _iDisk.CenterY),
            _iDisk.Radius - 30,
            angle);

            CreateText(
            _externalDisk[i].ToString(),
            outerPoint);

            CreateText(
                _currentInnerDisk[_step][i].ToString(),
            innerPoint);
    }
}
    private void CreateText(string value, Avalonia.Point point)
{
    var text = new TextBlock
    {
        Text = value,
        FontSize = 20,
        Width = 30,
        Height = 30,
        TextAlignment = TextAlignment.Center
    };

    Canvas.SetLeft(text, point.X - 10);
    Canvas.SetTop(text, point.Y);

    DiskCanvas.Children.Add(text);
}

    private Avalonia.Point GetPoint(Avalonia.Point center, double radius, double angle)
    {
        double x = center.X + radius * Math.Cos(angle);
        double y = center.Y + radius * Math.Sin(angle);

        return new Avalonia.Point(x, y);
    }
    private void InitExternalDisk()
    {
        _externalDisk = _encryptor.GetExternalDisk();
    }
    private void InitBasedInternalDisk()
    {
        _internalDisk = File.ReadAllText(PathLoader.ShiftedAlphabetPath()).ToArray();
    }
    private void UpdateUI()
    {   
        _internalDisk = _currentInnerDisk[_step];
        EncodingText.Text = new string(_encoding[_step]);
        RenderDisks();
    }
   private void Reset()
{
    _step = 0;
    _maxStep = 0;

    _encoding.Clear();
    _currentInnerDisk.Clear();

    InitBasedInternalDisk();
}
   
    private void Apply_Message_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        _message = MessageTextBox.Text ?? "";

        if (_isEncryptor)
        {
            Encrypt();
        }
        if (_isDecryptor)
        {
            Decrypt();
        }
    }
    private void Next_button_click(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (_step >= _maxStep || _encoding.Count == 0)
        {
            return;
        }
        _step++;
        EncodingText.Text = new string(_encoding[_step]);
        UpdateUI();
    }
    private void Back_button_click(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        if (_encoding.Count == 0 || _step == 0)
        {
            return;
        }
         _step--;
        EncodingText.Text = new string(_encoding[_step]);
        UpdateUI();
    }

     private void Encrypt_button_click(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        Reset();
        _isEncryptor = true;
        _isDecryptor = false;
        Encrypt();
        _maxStep = _encoding.Count -1;
        UpdateUI();
    }
     private void Decrypt_button_click(object? sender, Avalonia.Interactivity.RoutedEventArgs args)
    {
        Reset();
        _isEncryptor = false;
        _isDecryptor = true;
        Decrypt();
        _maxStep = _encoding.Count - 1;
        UpdateUI();
    }

    private void Encrypt()
    {
        if (_message is null)
        {
            return;
        }
        var encrypted = _encryptor.Encrypt(_message, _manualStep);

        var saves = File.ReadAllLines(PathLoader.TempPath()).Select(line => line.ToCharArray()).ToList();

        foreach (var enc in encrypted)
        {
            _encoding.Add(enc);
        }

        foreach(var save in saves)
        {
            _currentInnerDisk.Add(save);
        }

    }
    private void Decrypt()
    {
        var encryptedPath = PathLoader.EncryptedPath();

        var encrypted = File.ReadAllText(encryptedPath)
        .Replace("\n", "")
        .Replace("\r", "");

        var decrypted = _encryptor.Decrypt(encrypted, _manualStep);
    var saves = File.ReadAllLines(PathLoader.TempPath()).Select(line => line.ToCharArray()).ToList();
        foreach (var enc in decrypted)
        {
            _encoding.Add(enc);
        }

        foreach(var save in saves)
        {
            _currentInnerDisk.Add(save);
        }
        
    }
}