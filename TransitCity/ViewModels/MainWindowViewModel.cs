using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using CommunityToolkit.Mvvm.Input;
using TransitCity.Services;
using TransitLibrary.Map;

namespace TransitCity.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{

    private Node.IntersectionType _newNodeType=Node.IntersectionType.None;

    private string _selectedNodeType = "None";
    public string SelectedNodeType
    {
        get => _selectedNodeType;
        set
        {
            _selectedNodeType= value;
            switch (_selectedNodeType)
            {
                default:
                case "None":
                    _newNodeType=Node.IntersectionType.None;
                    break;
                case "Traffic Lights":
                    _newNodeType=Node.IntersectionType.TrafficLight;
                    break;
                case "Roundabout":
                    _newNodeType=Node.IntersectionType.Roundabout;
                    break;
            }

            if (_selectedNode != -1)
            {
                _mapGraph.Nodes[_selectedNode].Type=_newNodeType;
                OnPropertyChanged(nameof(MapBitmap));
            }
        }
    }
    public List<string> NodeTypes { get; private set; } = [
        "None",
        "Traffic Lights",
        "Roundabout",
    ];
    
    
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

            if (_selectedConnection != -1)
            {
                
                Connection.RoadType newRoadType;
                    
                switch (_selectedConnectionType)
                {
                    case "Motorvej":
                        newRoadType = Connection.RoadType.Motorvej;
                        break;
                    case "Motortraffikvej":
                        newRoadType = Connection.RoadType.Motortraffikvej;
                        break;
                    case "Landevej":
                        newRoadType = Connection.RoadType.Landevej;
                        break;
                    default:
                    case "Byvej":
                        newRoadType = Connection.RoadType.Byvej;
                        break;
                    case "Langsom Zone":
                        newRoadType = Connection.RoadType.LangsomZone;
                        break;
                    case "Gågade":
                        newRoadType = Connection.RoadType.Gaagade;
                        break;
                }
                _mapGraph.Connections[_selectedConnection].Type= newRoadType;
            }
            OnPropertyChanged(nameof(MapBitmap));
            
        }
    }
    
    
    private bool _connectionIsRoad = true;
    private bool _connectionIsRail = false;
    private bool _connectionIsPedestrian = true;
    private bool _connectionIsBicycle = true;

    public bool ConnectionIsRoad
    {
        get => _connectionIsRoad;
        set
        {
            _connectionIsRoad = value;
            //If there is a selected connection, update it
            if (_selectedConnection != -1)
                _mapGraph.Connections[_selectedConnection].Road=_connectionIsRoad;
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
            if (_selectedConnection != -1)
                _mapGraph.Connections[_selectedConnection].Rail=_connectionIsRail;
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
            if (_selectedConnection != -1)
                _mapGraph.Connections[_selectedConnection].PedestrianPath=_connectionIsPedestrian;
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
            if (_selectedConnection != -1)
                _mapGraph.Connections[_selectedConnection].BicyclePath=_connectionIsBicycle;
            OnPropertyChanged(nameof(MapBitmap));
        }
    }

    private bool _nodeIsParkingLot;

    public bool NodeIsParkingLot
    {
        get => _nodeIsParkingLot;
        set
        {
            _nodeIsParkingLot = value;
            if (_selectedNode!= -1)
                _mapGraph.Nodes[_selectedNode].IsPublicParkingLot=_nodeIsParkingLot;
            OnPropertyChanged(nameof(MapBitmap));
        }
    }
    

    public float CenterX { get; set; }
    public float CenterY {get; set; }

    public float Scale { get; set; } = 1.0f;

    private int _selectedNode = -1;
    private int _selectedConnection = -1;
    
    public string YardstickText {get; set; } ="Yardstick: 200m";


    private readonly MapImageGenerator _generator;

    private Graph _mapGraph;

    private string _saveFileName = "save";

    public string SaveFileName
    {
        get=>_saveFileName;
        set
        {
            _saveFileName = value;
            
            //Make sure this can be used as a save file name, this may not be displayed instantly, but as long as it is updated behind the scenes I don't care
            
            //Get invalid characters in a filename
            var invalidChars = Path.GetInvalidFileNameChars();
            //Fix it
            foreach (var c in invalidChars)
                _saveFileName = _saveFileName.Replace(c, '_');
            //I especially don't like spaces
            _saveFileName = _saveFileName.Replace(' ', '_');
        }
    }
    
    public MainWindowViewModel()
    {
        
        Stream mapStream = AssetLoader.Open(new Uri("avares://TransitCity/Assets/map.png"));
        _mapGraph = new Graph();
        _generator = new MapImageGenerator(mapStream);
    }
    
    public Bitmap MapBitmap
    { 
        get
        {
            Debug.WriteLine("CALL PNG CONVERTER");
            var pngStream = _generator.AsPngImage(_mapGraph,CenterX, CenterY,Scale,_selectedNode,_selectedConnection);
            return new Bitmap(pngStream);
        }
    }

    [RelayCommand]
    private void MoveMapLeft()
    {
        CenterX -= 100/Scale;
        OnPropertyChanged(nameof(CenterX));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    private void MoveMapRight()
    {
        CenterX += 100/Scale;
        OnPropertyChanged(nameof(CenterX));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    private void MoveMapUp()
    {
        CenterY -= 100/Scale;
        OnPropertyChanged(nameof(CenterY));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    private void MoveMapDown()
    {
        CenterY += 100/Scale;
        OnPropertyChanged(nameof(CenterY));
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    private void MoveMapIn()
    {
        Scale *= 2;
        OnPropertyChanged(nameof(Scale));
        OnPropertyChanged(nameof(MapBitmap));
        YardstickText = $"Yardstick: {200/Scale}m";
        OnPropertyChanged(nameof(YardstickText));
    }

    [RelayCommand]
    private void MoveMapOut()
    {
        Scale *= 0.5f;
        OnPropertyChanged(nameof(Scale));
        OnPropertyChanged(nameof(MapBitmap));
        YardstickText = $"Yardstick: {200/Scale}m";
        OnPropertyChanged(nameof(YardstickText));
    }

    [RelayCommand]
    private void Save()
    {
        //Get windows appdata, or linux .local, or mac application support
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string myAppDataFolder = Path.Combine(appDataPath, "TransitCity");
        Directory.CreateDirectory(myAppDataFolder);
        
        string outputPath = Path.ChangeExtension(Path.Combine(myAppDataFolder, _saveFileName),".xml");
        
        Console.WriteLine($"Saving graph as {outputPath}");
        _mapGraph.Save(outputPath);
    }

    [RelayCommand]
    private void Load()
    {
        //Get windows appdata, or linux .local, or mac application support
        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        string myAppDataFolder = Path.Combine(appDataPath, "TransitCity");
        Directory.CreateDirectory(myAppDataFolder);
        
        string outputPath = Path.ChangeExtension(Path.Combine(myAppDataFolder, _saveFileName),".xml");
        
        Console.WriteLine($"Loading graph as {outputPath}");
        try
        {
            _mapGraph.Load(outputPath);
        }
        catch (Exception ex)
        {
            //Didn't work... since the loading fails safe, there is no need to clean up the map
            Console.WriteLine($"Could not load graph, error: {ex.Message}");
        }
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    private void DeleteSelected()
    {
        if (_selectedNode != -1)
        {
            _mapGraph.DeleteNodeAtId(_selectedNode);
            _selectedNode = -1;
        }

        if (_selectedConnection != -1)
        {
            _mapGraph.DeleteConnectionAtId(_selectedConnection);
            _selectedConnection = -1;
        }
        OnPropertyChanged(nameof(MapBitmap));
    }

    [RelayCommand]
    private void ImageClicked_AddNode(Point imagePoint)
    {
        var globalX = MapImageGenerator.XImageToGlobalSpace((float)imagePoint.X, CenterX, Scale);
        var globalY = MapImageGenerator.YImageToGlobalSpace((float)imagePoint.Y, CenterY, Scale);
        
        _mapGraph.AddNode(new Node(0,globalX,globalY,_nodeIsParkingLot,_newNodeType));
        OnPropertyChanged(nameof(MapBitmap));
    }


    [RelayCommand]
    private void ImageClicked_SelectNode(Point imagePoint)
    {
        _selectedConnection = -1;
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
            if (_selectedNode == -1)
            {
                _selectedNode = otherNode;
            }
            //This is the second selection, either make a new connection, or select the existing one
            else
            {
                //Do nothing if clicked the same node
                if (_selectedNode == otherNode)
                {
                }
                //Check if there is an existing connection
                else if (_mapGraph.Nodes[_selectedNode].Neighbours.ContainsKey(_mapGraph.Nodes[otherNode]))
                {
                    var connection = _mapGraph.Nodes[_selectedNode].Neighbours[_mapGraph.Nodes[otherNode]];
                    _selectedConnection = connection.Id;
                    
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
                    
                    Debug.WriteLine($"Select existing connection {_selectedConnection}");
                }
                else
                {
                    Debug.WriteLine("create new connection");

                    Connection.RoadType newRoadType;
                    
                    switch (_selectedConnectionType)
                    {
                        case "Motorvej":
                            newRoadType = Connection.RoadType.Motorvej;
                            break;
                        case "Motortraffikvej":
                            newRoadType = Connection.RoadType.Motortraffikvej;
                            break;
                        case "Landevej":
                            newRoadType = Connection.RoadType.Landevej;
                            break;
                        default:
                        case "Byvej":
                            newRoadType = Connection.RoadType.Byvej;
                            break;
                        case "Langsom Zone":
                            newRoadType = Connection.RoadType.LangsomZone;
                            break;
                        case "Gågade":
                            newRoadType = Connection.RoadType.Gaagade;
                            break;
                    }
                    
                    _mapGraph.AddConnection(new Connection(ConnectionIsRoad, newRoadType,ConnectionIsRail,ConnectionIsPedestrian,ConnectionIsBicycle,_mapGraph.Nodes[_selectedNode],_mapGraph.Nodes[otherNode]));
                }
                
                //Deselect either way
                _selectedNode = -1;
            }
        }
        else
        {
            //You clicked somewhere else, unselect everything
            _selectedNode = -1;
        }
        
        OnPropertyChanged(nameof(MapBitmap));
    }
}