using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Warpdeck.UI.Common;

namespace Warpdeck.UI.Views;

public partial class ColorPickerDialog : Window
{
    private double _hue = 0;
    private double _sat = 1;
    private double _val = 1;
    private bool _isUpdatingUi = false;
    private bool _isDraggingSv = false;

    public string SelectedColorHex { get; private set; } = "#0070F3";
    public string InitialColorHex { get; }

    public ColorPickerDialog(string initialHex = "#0070F3")
    {
        InitializeComponent();

        InitialColorHex = ColorHelper.NormalizeHex(string.IsNullOrWhiteSpace(initialHex) ? "#0070F3" : initialHex);
        SelectedColorHex = InitialColorHex;

        PopulatePresets();
        PopulateRecentColors();

        Loaded += ColorPickerDialog_Loaded;
    }

    private void ColorPickerDialog_Loaded(object sender, RoutedEventArgs e)
    {
        var initialColor = ColorHelper.ParseHex(InitialColorHex, Color.FromRgb(0, 112, 243));
        OriginalPreviewBorder.Background = new SolidColorBrush(initialColor);

        SetColorFromRgb(initialColor.R, initialColor.G, initialColor.B);
    }

    public static bool ShowDialog(Window owner, string initialHex, out string selectedHex)
    {
        var dlg = new ColorPickerDialog(initialHex)
        {
            Owner = owner
        };

        if (dlg.ShowDialog() == true)
        {
            selectedHex = dlg.SelectedColorHex;
            ColorHelper.AddRecentColor(selectedHex);
            return true;
        }

        selectedHex = initialHex;
        return false;
    }

    private void PopulatePresets()
    {
        PresetsWrapPanel.Children.Clear();
        foreach (var hex in ColorHelper.ExpandedPresetColors)
        {
            var btn = CreateColorSwatchButton(hex);
            PresetsWrapPanel.Children.Add(btn);
        }
    }

    private void PopulateRecentColors()
    {
        RecentWrapPanel.Children.Clear();
        foreach (var hex in ColorHelper.RecentCustomColors)
        {
            var btn = CreateColorSwatchButton(hex);
            RecentWrapPanel.Children.Add(btn);
        }
    }

    private Button CreateColorSwatchButton(string hex)
    {
        var c = ColorHelper.ParseHex(hex, Colors.Black);
        var brush = new SolidColorBrush(c);
        var isLight = (c.R * 0.299 + c.G * 0.587 + c.B * 0.114) > 160;

        var btn = new Button
        {
            Width = 24,
            Height = 24,
            Margin = new Thickness(2),
            Background = brush,
            BorderBrush = new SolidColorBrush(Color.FromArgb(80, 255, 255, 255)),
            BorderThickness = new Thickness(1),
            Cursor = Cursors.Hand,
            ToolTip = hex,
            Tag = hex
        };

        btn.Click += (s, e) =>
        {
            if (s is Button b && b.Tag is string colorHex)
            {
                var clr = ColorHelper.ParseHex(colorHex, Colors.Black);
                SetColorFromRgb(clr.R, clr.G, clr.B);
            }
        };

        return btn;
    }

    private void SetColorFromRgb(byte r, byte g, byte b)
    {
        var (h, s, v) = ColorHelper.RgbToHsv(r, g, b);
        _hue = h;
        _sat = s;
        _val = v;

        UpdateUiFromHsv();
    }

    private void UpdateUiFromHsv()
    {
        if (_isUpdatingUi) return;
        _isUpdatingUi = true;

        try
        {
            var (r, g, b) = ColorHelper.HsvToRgb(_hue, _sat, _val);
            var color = Color.FromRgb(r, g, b);
            var hex = ColorHelper.ToHex(color);
            SelectedColorHex = hex;

            // Update SV Box Background to Pure Hue
            var (hr, hg, hb) = ColorHelper.HsvToRgb(_hue, 1, 1);
            SvBox.Background = new SolidColorBrush(Color.FromRgb(hr, hg, hb));

            // Update Crosshair position
            double svWidth = SvBox.ActualWidth > 0 ? SvBox.ActualWidth : 228;
            double svHeight = SvBox.ActualHeight > 0 ? SvBox.ActualHeight : 198;

            Canvas.SetLeft(Crosshair, _sat * svWidth);
            Canvas.SetTop(Crosshair, (1.0 - _val) * svHeight);

            // Update Hue Slider
            HueSlider.Value = _hue;

            // Update Preview
            NewPreviewBorder.Background = new SolidColorBrush(color);

            // Update Hex TextBox
            HexTextBox.Text = hex;

            // Update RGB Sliders & TextBoxes
            RedSlider.Value = r;
            GreenSlider.Value = g;
            BlueSlider.Value = b;

            RedTextBox.Text = r.ToString();
            GreenTextBox.Text = g.ToString();
            BlueTextBox.Text = b.ToString();
        }
        finally
        {
            _isUpdatingUi = false;
        }
    }

    private void SvBox_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        _isDraggingSv = true;
        SvBox.CaptureMouse();
        UpdateSvFromMouse(e.GetPosition(SvBox));
    }

    private void SvBox_MouseMove(object sender, MouseEventArgs e)
    {
        if (_isDraggingSv)
        {
            UpdateSvFromMouse(e.GetPosition(SvBox));
        }
    }

    private void SvBox_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
        if (_isDraggingSv)
        {
            _isDraggingSv = false;
            SvBox.ReleaseMouseCapture();
        }
    }

    private void UpdateSvFromMouse(Point pos)
    {
        double width = SvBox.ActualWidth > 0 ? SvBox.ActualWidth : 228;
        double height = SvBox.ActualHeight > 0 ? SvBox.ActualHeight : 198;

        _sat = Math.Clamp(pos.X / width, 0.0, 1.0);
        _val = Math.Clamp(1.0 - (pos.Y / height), 0.0, 1.0);

        UpdateUiFromHsv();
    }

    private void HueSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_isUpdatingUi) return;
        _hue = HueSlider.Value;
        UpdateUiFromHsv();
    }

    private void RgbSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
    {
        if (_isUpdatingUi) return;
        byte r = (byte)Math.Clamp(Math.Round(RedSlider.Value), 0, 255);
        byte g = (byte)Math.Clamp(Math.Round(GreenSlider.Value), 0, 255);
        byte b = (byte)Math.Clamp(Math.Round(BlueSlider.Value), 0, 255);

        SetColorFromRgb(r, g, b);
    }

    private void RgbTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isUpdatingUi) return;

        if (byte.TryParse(RedTextBox.Text, out var r) &&
            byte.TryParse(GreenTextBox.Text, out var g) &&
            byte.TryParse(BlueTextBox.Text, out var b))
        {
            SetColorFromRgb(r, g, b);
        }
    }

    private void HexTextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_isUpdatingUi) return;

        var text = HexTextBox.Text?.Trim();
        if (string.IsNullOrWhiteSpace(text)) return;

        var color = ColorHelper.ParseHex(text, Colors.Transparent);
        if (color.A != 0 || text.Equals("#00000000", StringComparison.OrdinalIgnoreCase))
        {
            SetColorFromRgb(color.R, color.G, color.B);
        }
    }

    private void Select_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
        Close();
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
        Close();
    }
}
