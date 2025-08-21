using System.IO;
using SkiaSharp;
using TransitLibrary.Graph;

namespace TransitCity.Services;

public class MapImageGenerator
{
    private static readonly int Width = 1400;
    private static readonly int Height= 900;
    public static readonly float NodeRadius = 10;

    public static float XGlobalToImageSpace(float xGlobal, float centerX, float scale)
    {
        return (xGlobal - centerX)*scale + Width * 0.5f;
    }
    
    public static float YGlobalToImageSpace(float yGlobal, float centerY,float scale)
    {
        return (yGlobal - centerY)*scale + Height* 0.5f;
    }

    public static float XImageToGlobalSpace(float xImage, float centerX, float scale)
    {
        return (xImage - Width * 0.5f) / scale + centerX;
    }

    public static float YImageToGlobalSpace(float yImage, float centerY, float scale)
    {
        return (yImage - Height * 0.5f) / scale + centerY;
    }
    
    
    
    public MemoryStream AsPngImage(Graph graph,float centerX, float centerY,float scale, int selectedNode, int selectedConnection)
    {
        
        var image = new SKImageInfo(Width,Height);

        SKPaint unselectedNodeColorNoParking = new SKPaint
        {
            Color = SKColors.MidnightBlue,
            IsAntialias = true
        };

        SKPaint selectedNodeColorNoParking = new SKPaint
        {
            Color = SKColors.Blue,
            IsAntialias = true
        };
        
        SKPaint unselectedNodeColorParking = new SKPaint
        {
            Color = SKColors.BlueViolet,
            IsAntialias = true
        };
        SKPaint selectedNodeColorParking = new SKPaint
        {
            Color = SKColors.Purple,
            IsAntialias = true
        };

        SKPaint yardstick= new SKPaint
        {
            Color = SKColors.Red,
            IsAntialias = true,
            StrokeWidth = 3
        };

        //Thick red line
        SKPaint[] roadColor = [
        new SKPaint
        {//Motorvej
            Color = SKColors.Red,
            IsAntialias = true,
            StrokeWidth = 18
        },
        new SKPaint
        {//Motortraffikvej
            Color = SKColors.Red,
            IsAntialias = true,
            StrokeWidth = 16
        },
        new SKPaint
        {//Landevej
            Color = SKColors.Red,
            IsAntialias = true,
            StrokeWidth = 14
        },
        new SKPaint
        {//byvej
            Color = SKColors.Red,
            IsAntialias = true,
            StrokeWidth = 12
        },
        new SKPaint
        {//Langsom Zone
            Color = SKColors.Red,
            IsAntialias = true,
            StrokeWidth = 10
        },
        new SKPaint
        {//Gågade
            Color = SKColors.Red,
            IsAntialias = true,
            StrokeWidth = 8
        },
        
        ];
        
        //Dashed, thick black line
        SKPaint railColor = new SKPaint
        {
            Color = SKColors.Black,
            IsAntialias = true,
            StrokeWidth = 16,
            PathEffect = SKPathEffect.CreateDash([4,8],0)
        };
        //Dot-dashed green line
        SKPaint pedestrianColor = new SKPaint
        {
            Color = SKColors.Green,
            IsAntialias = true,
            StrokeWidth = 4,
            PathEffect = SKPathEffect.CreateDash([4,2,2,2],0)
        };
        //Dotted yellow line
        SKPaint bicycleColor = new SKPaint
        {
            Color = SKColors.Yellow,
            IsAntialias = true,
            StrokeWidth = 4,
            PathEffect = SKPathEffect.CreateDash([4,4],0)
        };


        
        
        using var bitmap = new SKBitmap(image);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear(SKColors.LightGreen);
            
            foreach (var node in graph.Nodes)
            {
                //The node is selected, if it is selected, or any of its neighbours are selected
                bool isSelected = (node.Id == selectedNode);
                if (!isSelected && selectedConnection!=-1)
                    foreach (var (_, con) in node.Neighbours)
                    {
                        if (con.Id == selectedConnection)
                            isSelected = true;
                    }
                canvas.DrawCircle(
                    XGlobalToImageSpace((float)node.X,centerX,scale),
                    YGlobalToImageSpace((float)node.Y,centerY,scale),
                    NodeRadius, isSelected?(node.IsPublicParkingLot?selectedNodeColorParking:selectedNodeColorNoParking):(node.IsPublicParkingLot?unselectedNodeColorParking:unselectedNodeColorNoParking));
            }
            foreach (var connection in graph.Connections)
            {
                if (connection.Road)
                    canvas.DrawLine(XGlobalToImageSpace((float)connection.From.X,centerX,scale), YGlobalToImageSpace((float)connection.From.Y,centerY,scale), XGlobalToImageSpace((float)connection.To.X,centerX,scale), YGlobalToImageSpace((float)connection.To.Y,centerY,scale),roadColor[(int)connection.Type]);
                if (connection.Rail)
                    canvas.DrawLine(XGlobalToImageSpace((float)connection.From.X,centerX,scale), YGlobalToImageSpace((float)connection.From.Y,centerY,scale), XGlobalToImageSpace((float)connection.To.X,centerX,scale), YGlobalToImageSpace((float)connection.To.Y,centerY,scale),railColor);
                if (connection.BicyclePath)
                    canvas.DrawLine(XGlobalToImageSpace((float)connection.From.X,centerX,scale), YGlobalToImageSpace((float)connection.From.Y,centerY,scale), XGlobalToImageSpace((float)connection.To.X,centerX,scale), YGlobalToImageSpace((float)connection.To.Y,centerY,scale),bicycleColor);
                if (connection.PedestrianPath)
                    canvas.DrawLine(XGlobalToImageSpace((float)connection.From.X,centerX,scale), YGlobalToImageSpace((float)connection.From.Y,centerY,scale), XGlobalToImageSpace((float)connection.To.X,centerX,scale), YGlobalToImageSpace((float)connection.To.Y,centerY,scale),pedestrianColor);
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