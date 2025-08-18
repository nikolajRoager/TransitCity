namespace TransitLibrary.Graph;

public class Graph
{
    public readonly List<Node> Nodes;
    public readonly List<Connection> Connections;

    public Graph()
    {
        Nodes = new List<Node>();
        Connections=new List<Connection>();
    }

    public void AddNode(Node node)
    {
        node.Id=Nodes.Count;
        Nodes.Add(node);
    }

    public void AddConnection(Connection connection)
    {
        connection.Id=Connections.Count;
        Connections.Add(connection);
    }
}