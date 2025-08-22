namespace TransitLibrary.Graph;

public class Node
{
    /// <summary>
    /// The ID in the list of all nodes, is different from the number in the address, as this never repeats
    /// </summary>
    public int Id { get; set; }
    
    /// <summary>
    /// Location on the x-axis in meters
    /// </summary>
    public double X { get; set; }
    
    /// <summary>
    /// Location on the y-axis in meters
    /// </summary>
    public double Y { get; set; }

    /// <summary>
    /// Does this node allow public parking, should not be true if only private parking is allowed
    /// </summary>
    public bool IsPublicParkingLot { get; set; }=true;


    public enum IntersectionType
    {
        None =0,
        TrafficLight=1,
        Roundabout=2,
    }
    
    public IntersectionType Type { get; set; }
    
    public Dictionary<Node, Connection> Neighbours { get; private set; }

    public Node(int id, double x, double y, bool isPublicParkingLot, IntersectionType type)
    {
        Id = id;
        X = x;
        Y = y;
        IsPublicParkingLot = isPublicParkingLot;
        Type = type;
        
        //Will be set by the connections when added
        Neighbours = new Dictionary<Node, Connection>();
    }
}