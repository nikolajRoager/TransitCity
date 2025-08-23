namespace TransitLibrary.Map;

public class Connection
{
    public int Id { get; set; }
    
    public Node From { get; private set; }
    public Node To { get; private set; }
    
    public bool Road { get; set; }
    public bool Rail { get; set; }
    public bool PedestrianPath { get; set; }
    public bool BicyclePath { get; set; }

    /// <summary>
    /// If this is a road, what type is it, controls speed limit
    /// </summary>
    public enum RoadType
    {
        Motorvej=0,
        Motortraffikvej=1,
        Landevej=2,
        Byvej=3,
        LangsomZone=4,
        Gaagade=5,
    }
    
    public RoadType Type { get; set; }
    
    public Connection(bool road, RoadType roadType , bool rail, bool pedestrianPath, bool bicyclePath, Node from, Node to)
    {
        Road = road;
        Rail = rail;
        PedestrianPath = pedestrianPath;
        BicyclePath = bicyclePath;
        
        Type = roadType;
        
        From = from;
        To = to;
        From.Neighbours.Add(To, this);
        To.Neighbours.Add(From, this);
    }
}