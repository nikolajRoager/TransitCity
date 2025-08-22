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

    public void DeleteNodeAtId(int id)
    {
        //Subtract all ids after the one we delete by 1
        for (int i = id + 1; i < Nodes.Count; i++)
            Nodes[i].Id--;
        //Remove all connections
        foreach (var (_, c) in Nodes[id].Neighbours)
        {
            DeleteConnectionAtId(c.Id);
        }
            
        Nodes.RemoveAt(id);
    }
    public void DeleteConnectionAtId(int id)
    {
        //Subtract all ids after the one we delete by 1
        for (int i = id + 1; i < Connections.Count; i++)
            Connections[i].Id--;
        //Remove from all nodes
        Connections[id].From.Neighbours.Remove(Connections[id].To);
        Connections[id].To.Neighbours.Remove(Connections[id].From);
        Connections.RemoveAt(id);
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