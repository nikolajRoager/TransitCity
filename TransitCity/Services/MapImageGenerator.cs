using System.IO;
using SkiaSharp;
using TransitLibrary.Graph;

namespace TransitCity.Services;

public class MapImageGenerator
{
    private static readonly int Width = 1400;
    private static readonly int Height= 900;

    public static float XToImageSpace(float x, float centerX, float scale)
    {
        return (x - centerX)*scale + Width * 0.5f;
    }

    public static float YToImageSpace(float y, float centerY,float scale)
    {
        return (y - centerY)*scale + Height* 0.5f;
    }
    
    public MemoryStream AsPngImage(Graph graph,float centerX, float centerY,float scale)
    {
        
        var image = new SKImageInfo(Width,Height);

        SKPaint unselectedNode = new SKPaint
        {
            Color = SKColors.MidnightBlue,
            IsAntialias = true
        };

        SKPaint yardstick= new SKPaint
        {
            Color = SKColors.Red,
            IsAntialias = true,
            StrokeWidth = 3
        };
        
        float nodeRadius = 5;
        
        
        
        using var bitmap = new SKBitmap(image);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColors.LightGreen);
            
            


            foreach (var node in graph.Nodes)
            {
                canvas.DrawCircle(
                    XToImageSpace((float)node.X,centerX,scale),
                    YToImageSpace((float)node.Y,centerY,scale),
                    nodeRadius, unselectedNode);
            }
            
            //Draw a line we will use as a yardstick
            canvas.DrawLine(50,50,250,50,yardstick);
            
            
            //Stupid hack: convert image to Png, then convert png back to avalonia bitmap
            //This is slower than making a dedicated Skiashap Avalonia control, but way easier, and more flexible, as we can also send the output of this over the internet, or save it to a file.
            var imageStream = new MemoryStream();
            bitmap.Encode(imageStream, SKEncodedImageFormat.Png, 50);
            imageStream.Position = 0;
        
            return imageStream;
        }
    }
}