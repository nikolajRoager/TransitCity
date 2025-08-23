    using System.Xml.Linq;

    namespace TransitLibrary.Map;

public class Graph
{
    public List<Node> Nodes { get; private set; }  = new();
    public List<Connection> Connections { get; private set; } = new();

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

    /// <summary>
    /// Load graph data from this xml file, only commits changes to memory if successful, may throw exceptions if not
    /// </summary>
    /// <param name="fileName"></param>
    public void Load(string fileName)
    {
        //We will only commit the lists to memory if all steps of the loading process works
        var newNodes = new List<Node>();
        var newConnections = new List<Connection>();
        
        Console.WriteLine($"Trying to load graph from {fileName}");
        XDocument doc = XDocument.Load(fileName);
        
        XElement? root = doc.Root;
        
        if (root == null)
            throw new Exception("Could not load graph, root element is null");
        
        //Load each node and all its data
        foreach (var node in root.Elements("Node"))
        {
            int id = int.Parse(node?.Attribute("Id")?.Value ?? "0");
            double x = double.Parse(node?.Attribute("X")?.Value ?? "0");
            double y = double.Parse(node?.Attribute("Y")?.Value ?? "0");
            bool isPublicParkingLot = bool.Parse(node?.Attribute("IsPublicParkingLot")?.Value ?? "False");
            string nodeTypeStr = node?.Attribute("IntersectionType")?.Value ?? "None";
            Node.IntersectionType nodeType;
            switch (nodeTypeStr)
            {
                default:
                case "None":
                    nodeType=Node.IntersectionType.None;
                    break;
                case "TrafficLight":
                    nodeType=Node.IntersectionType.TrafficLight;
                    break;
                case "Roundabout":
                    nodeType=Node.IntersectionType.Roundabout;
                    break;
            }
            var newNode = new Node (id, x, y, isPublicParkingLot, nodeType);
            newNodes.Add(newNode);   
        }
        //Load each connection, and connect the nodes
        foreach (var connectionNode in root.Elements("Connection"))
        {
            
            int id = int.Parse(connectionNode?.Attribute("Id")?.Value ?? "0");
            int from = int.Parse(connectionNode?.Attribute("From")?.Value ?? "0");
            int to = int.Parse(connectionNode?.Attribute("To")?.Value ?? "0");
            bool road = bool.Parse(connectionNode?.Attribute("Road")?.Value ?? "False");
            bool rail = bool.Parse(connectionNode?.Attribute("Rail")?.Value ?? "False");
            bool bicyclePath = bool.Parse(connectionNode?.Attribute("BicyclePath")?.Value ?? "False");
            bool pedestrianPath = bool.Parse(connectionNode?.Attribute("PedestrianPath")?.Value ?? "False");
            string typeStr = connectionNode?.Attribute("Type")?.Value ?? "Byvej";
                
            var type = Connection.RoadType.Byvej;
            switch (typeStr)
            {
                case "Motorvej":
                    type = Connection.RoadType.Motorvej;
                    break;
                case "Motortraffikvej":
                    type = Connection.RoadType.Motortraffikvej;
                    break;
                case "Landevej":
                    type = Connection.RoadType.Landevej;
                    break;
                default:
                case "Byvej":
                    type = Connection.RoadType.Byvej;
                    break;  
                case "LangsomZone":
                    type = Connection.RoadType.LangsomZone;
                    break;  
                case "Gaagade":
                    type = Connection.RoadType.Gaagade;
                    break;  
            }
            var newConnection = new Connection(road,type,rail,bicyclePath,pedestrianPath,newNodes[from],newNodes[to]);
            newConnection.Id = id;
            newConnections.Add(newConnection);
        }
        
        //If we got here, it worked, commit changes
        Nodes=newNodes;
        Connections=newConnections;
    }
    
    
    public void Save(string fileName)
    {
        XElement root = new XElement("Graph");

        foreach (var nodeElement in Nodes.Select(node => new XElement("Node",
                     new XAttribute("Id", node.Id),
                     new XAttribute("X",node.X),
                     new XAttribute("Y",node.Y),
                     new XAttribute("IsPublicParkingLot",node.IsPublicParkingLot),
                     new XAttribute("IntersectionType",node.Type)
                 )))
        {
            root.Add(nodeElement);
        }

        foreach (var connectionElement in Connections.Select(connection => new XElement("Connection",
                     new XAttribute("Id", connection.Id),
                     new XAttribute("From",connection.From.Id),
                     new XAttribute("To",connection.To.Id),
                     new XAttribute("Road",connection.Road),
                     new XAttribute("Rail",connection.Rail),
                     new XAttribute("BicyclePath",connection.BicyclePath),
                     new XAttribute("PedestrianPath",connection.PedestrianPath),
                     new XAttribute("Type", connection.Type)
                 )))
        {
            root.Add(connectionElement);
        }
        
        var doc = new XDocument(new XDeclaration("1.0","utf-8","yes"),root);
        
        doc.Save(fileName);
    }
}