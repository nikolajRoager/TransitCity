using System;
using System.Collections.Generic;
using System.Diagnostics;
using Avalonia;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.Input;
using TransitCity.Services;
using TransitLibrary.Graph;

namespace TransitCity.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{

    public List<string> ConnectionTypes { get; private set; } = [
    "Motorvej",
    "Motortraffikvej",
    "Landevej",
    "Byvej",
    "Langsom Zone",
    "Gågade"];
    
    private string _selectedConnectionType="Byvej";

    public string SelectedConnectionType
    {
        get => _selectedConnectionType;
        set
        {
            _selectedConnectionType = value;
        }
    }
    
    
    private bool _connectionIsRoad { get; set; } = true;
    private bool _connectionIsRail { get; set; } = false;
    private bool _connectionIsPedestrian { get; set; } = true;
    private bool _connectionIsBicycle { get; set; } = true;

    public bool ConnectionIsRoad
    {
        get => _connectionIsRoad;
        set
        {
            _connectionIsRoad = value;
            //If there is a selected connection, update it
            if (SelectedConnection != -1)
                _mapGraph.Connections[SelectedConnection].Road=_connectionIsRoad;
            OnPropertyChanged(nameof(MapBitmap));
        }
    }

    public bool ConnectionIsRail
    {
        get => _connectionIsRail;
        set
        {
            _connectionIsRail = value;
            //If there is a selected connection, update it
            if (SelectedConnection != -1)
                _mapGraph.Connections[SelectedConnection].Rail=_connectionIsRail;
            OnPropertyChanged(nameof(MapBitmap));
        }
    }

    public bool ConnectionIsPedestrian
    {
        get => _connectionIsPedestrian;
        set
        {
            _connectionIsPedestrian = value;
            //If there is a selected connection, update it
            if (SelectedConnection != -1)
                _mapGraph.Connections[SelectedConnection].PedestrianPath=_connectionIsPedestrian;
            OnPropertyChanged(nameof(MapBitmap));
        }
    }

    public bool ConnectionIsBicycle
    {
        get => _connectionIsBicycle;
        set
        {
            _connectionIsBicycle = value;
            //If there is a selected connection, update it
            if (SelectedConnection != -1)
                _mapGraph.Connections[SelectedConnection].BicyclePath=_connectionIsBicycle;
            OnPropertyChanged(nameof(MapBitmap));
        }
    }
    

    public float CenterX { get; set; }
    public float CenterY {get; set; }

    public float Scale { get; set; } = 1.0f;

    public int SelectedNode { get; set; } = -1;
    public int SelectedConnection { get; set; } = -1;
    
    public string YardstickText {get; set; } ="Yardstick: 200m";
    
    
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
            Debug.WriteLine("CALL PNG CONVERTER");
            var pngStream = Generator.AsPngImage(_mapGraph,CenterX, CenterY,Scale,SelectedNode,SelectedConnection);
            return new Bitmap(pngStream);
        }
    }

    [RelayCommand]
    public void MoveMapLeft()
    {
        CenterX -= 100/Scale;
        OnPropertyChanged(nameof(CenterX));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    public void MoveMapRight()
    {
        CenterX += 100/Scale;
        OnPropertyChanged(nameof(CenterX));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    public void MoveMapUp()
    {
        CenterY -= 100/Scale;
        OnPropertyChanged(nameof(CenterY));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    public void MoveMapDown()
    {
        CenterY += 100/Scale;
        OnPropertyChanged(nameof(CenterY));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    public void MoveMapIn()
    {
        Scale *= 2;
        OnPropertyChanged(nameof(Scale));
        OnPropertyChanged(nameof(MapBitmap));
        YardstickText = $"Yardstick: {200/Scale}m";
        OnPropertyChanged(nameof(YardstickText));
    }

    [RelayCommand]
    public void MoveMapOut()
    {
        Scale *= 0.5f;
        OnPropertyChanged(nameof(Scale));
        OnPropertyChanged(nameof(MapBitmap));
        YardstickText = $"Yardstick: {200/Scale}m";
        OnPropertyChanged(nameof(YardstickText));
    }

    [RelayCommand]
    private void ImageClicked_AddNode(Point imagePoint)
    {
        var globalX = MapImageGenerator.XImageToGlobalSpace((float)imagePoint.X, CenterX, Scale);
        var globalY = MapImageGenerator.YImageToGlobalSpace((float)imagePoint.Y, CenterY, Scale);
        
        _mapGraph.AddNode(new Node(0,globalX,globalY));
        OnPropertyChanged(nameof(MapBitmap));
    }


    [RelayCommand]
    private void ImageClicked_SelectNode(Point imagePoint)
    {
        SelectedConnection = -1;
        //Select a new node by right-clicking, if another node is already s     elected we will create a connection and unselect both instead
        int otherNode = -1;
        
        foreach (var node in _mapGraph.Nodes)
        {
            var imageX = MapImageGenerator.XGlobalToImageSpace((float)node.X, CenterX, Scale);
            var imageY = MapImageGenerator.YGlobalToImageSpace((float)node.Y, CenterY, Scale);
            
            float dist2 = (float)(Math.Pow(imageX-imagePoint.X,2)+Math.Pow(imageY-imagePoint.Y,2));

            if (dist2 < MapImageGenerator.NodeRadius * MapImageGenerator.NodeRadius)
            {
                otherNode = node.Id;
            }
        }

        if (otherNode != -1)
        {
            //This is the first selection, select it
            if (SelectedNode == -1)
            {
                SelectedNode = otherNode;
            }
            //This is the second selection, either make a new connection, or select the existing one
            else
            {
                //Do nothing if clicked the same node
                if (SelectedNode == otherNode)
                {
                }
                //Check if there is an existing connection
                else if (_mapGraph.Nodes[SelectedNode].Neighbours.ContainsKey(_mapGraph.Nodes[otherNode]))
                {
                    var connection = _mapGraph.Nodes[SelectedNode].Neighbours[_mapGraph.Nodes[otherNode]];
                    SelectedConnection = connection.Id;
                    
                    //Make sure the selected settings match the selected connection
                    _connectionIsBicycle = connection.BicyclePath;
                    _connectionIsPedestrian = connection.PedestrianPath;
                    _connectionIsRail = connection.Rail;
                    _connectionIsRoad = connection.Road;

                    switch (connection.Type)
                    {
                        case Connection.RoadType.Motorvej:
                            _selectedConnectionType = "Motorvej";
                            break;
                        case Connection.RoadType.Motortraffikvej:
                            _selectedConnectionType = "Motortraffikvej";
                            break;
                        case Connection.RoadType.Landevej:
                            _selectedConnectionType = "Landevej";
                            break;
                        default:
                        case Connection.RoadType.Byvej:
                            _selectedConnectionType = "Byvej";
                            break;
                        case Connection.RoadType.LangsomZone:
                            _selectedConnectionType = "Langsom Zone";
                            break;
                        case Connection.RoadType.Gaagade:
                            _selectedConnectionType = "Gågade";
                            break;
                    }
                    
                    OnPropertyChanged(nameof(ConnectionIsBicycle));
                    OnPropertyChanged(nameof(ConnectionIsPedestrian));
                    OnPropertyChanged(nameof(ConnectionIsRail));
                    OnPropertyChanged(nameof(ConnectionIsRoad));
                    OnPropertyChanged(nameof(SelectedConnectionType));
                    
                    Debug.WriteLine($"Select existing connection {SelectedConnection}");
                }
                else
                {
                    Debug.WriteLine("create new connection");

                    Connection.RoadType NewRoadType;
                    
                    switch (_selectedConnectionType)
                    {
                        case "Motorvej":
                            NewRoadType = Connection.RoadType.Motorvej;
                            break;
                        case "Motortraffikvej":
                            NewRoadType = Connection.RoadType.Motortraffikvej;
                            break;
                        case "Landevej":
                            NewRoadType = Connection.RoadType.Landevej;
                            break;
                        default:
                        case "Byvej":
                            NewRoadType = Connection.RoadType.Byvej;
                            break;
                        case "Langsom Zone":
                            NewRoadType = Connection.RoadType.LangsomZone;
                            break;
                        case "Gågade":
                            NewRoadType = Connection.RoadType.Gaagade;
                            break;
                    }
                    
                    _mapGraph.AddConnection(new Connection(ConnectionIsRoad, NewRoadType,ConnectionIsRail,ConnectionIsPedestrian,ConnectionIsBicycle,_mapGraph.Nodes[SelectedNode],_mapGraph.Nodes[otherNode]));
                }
                
                //Deselect either way
                SelectedNode = -1;
            }
        }
        else
        {
            //You clicked somewhere else, unselect everything
            SelectedNode = -1;
        }
        
        OnPropertyChanged(nameof(MapBitmap));
    }
}