using System;
using Avalonia;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using TransitCity.Services;
using TransitLibrary.Graph;

namespace TransitCity.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{

    public float CenterX { get; set; }
    public float CenterY {get; set; }


    public float Scale { get; set; } = 1.0f;
    
    
    public MapImageGenerator Generator { get; private set; } = new();

    private Graph _mapGraph;

    public MainWindowViewModel()
    {
        _mapGraph = new Graph();
    }
    
    public Bitmap MapBitmap
    { 
        get
        {
            Console.WriteLine("CALL PNG CONVERTER");
            var pngStream = Generator.AsPngImage(_mapGraph,CenterX, CenterY,Scale);
            return new Bitmap(pngStream);
        }
    }

    [RelayCommand]
    public void MoveMapLeft()
    {
        Console.WriteLine("Click left button");
        CenterX -= 100/Scale;
        OnPropertyChanged(nameof(CenterX));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    public void MoveMapRight()
    {
        Console.WriteLine("Click right button");
        CenterX += 100/Scale;
        OnPropertyChanged(nameof(CenterX));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    public void MoveMapUp()
    {
        Console.WriteLine("Click up button");
        CenterY -= 100/Scale;
        OnPropertyChanged(nameof(CenterY));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    public void MoveMapDown()
    {
        Console.WriteLine("Click down button");
        CenterY += 100/Scale;
        OnPropertyChanged(nameof(CenterY));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    public void MoveMapIn()
    {
        Console.WriteLine("Click zoom in button");
        Scale *= 2;
        OnPropertyChanged(nameof(Scale));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    public void MoveMapOut()
    {
        Console.WriteLine("Click zoom out button");
        Scale *= 0.5f;
        OnPropertyChanged(nameof(Scale));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    private void ImageClicked(Point imagePoint)
    {
        Console.WriteLine($"Click image at {imagePoint.X},{imagePoint.Y}");
    }

}