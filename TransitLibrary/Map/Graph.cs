namespace TransitLibrary.Graph;

public class Graph
{
    public List<Node> Nodes;

    public Graph()
    {
        Nodes = new List<Node>()
        {
            new Node(0, -40, -40),
            new Node(0, -20, -20),
            new Node(0, 0, 0),
            new Node(0, 20, 20),
            new Node(0, 40, 40),
        };
    }
}