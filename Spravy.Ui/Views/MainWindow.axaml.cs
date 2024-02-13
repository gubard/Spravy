using System;
using Avalonia;
using Avalonia.Svg;
using Spravy.Ui.Interfaces;
using SukiUI.Controls;

namespace Spravy.Ui.Views;

public partial class MainWindow : SukiWindow, IDesktopTopLevelControl
{
    public MainWindow()
    {
        InitializeComponent();
    }
}