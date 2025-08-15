using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using TransitCity.ViewModels;

namespace TransitCity.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void OnMapPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        
        var image = sender as Image;
        //We will also need to talk to the pixels of the bitmap to scale this correctly
        if (image?.Source is Bitmap bitmap)
        {
            var position = e.GetPosition(image);
            
            var point = e.GetCurrentPoint(image);
            if (point.Properties.IsLeftButtonPressed)
            {
                Console.WriteLine("LEFT, SELECTPRESS");
            }
            else if (point.Properties.IsRightButtonPressed)
            {
                Console.WriteLine("RIGHT, MOVEPRESS");
            }
            
            //To rescale the point, get the widths before and after scaling
            double renderedWidth = image.Bounds.Width;
            double renderedHeight= image.Bounds.Height;
            double imageWidth = bitmap.PixelSize.Width;
            double imageHeight = bitmap.PixelSize.Height;
            
            Console.WriteLine($"Got width {renderedWidth} and height {renderedHeight}");
            
            //Scale back to image pixels
            double x =position.X * imageWidth / renderedWidth;
            double y = position.Y * imageHeight / renderedHeight;
            var newPoint = new Point(x, y);
            
            Console.WriteLine($"Got point at {newPoint.X},{newPoint.Y}");
            //Pass on this new point to the view model
            if (DataContext is MainWindowViewModel vm)
            {
                vm.ImageClickedCommand.Execute(newPoint);
            }
        }
    }
}