namespace TransitLibrary.Graph;

public class Node
{
    /// <summary>
    /// The ID in the list of all nodes, is different from the number in the address, as this never repeats
    /// </summary>
    public int Id;
    
    /// <summary>
    /// Location on the x-axis in meters
    /// </summary>
    public double X;
    
    /// <summary>
    /// Location on the y-axis in meters
    /// </summary>
    public double Y;
    
    
    public Dictionary<Node, Connection> Neighbours { get; private set; }

    public Node(int id, double x, double y)
    {
        Id = id;
        X = x;
        Y = y;
        
        //Will be set by the connections when added
        Neighbours = new Dictionary<Node, Connection>();
    }
}