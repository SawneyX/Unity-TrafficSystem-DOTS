using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Collections;
    

public class ECSPathfind : MonoBehaviour
{
    //public NativeHashMap<Vector2, Node> graph = new NativeHashMap<Vector2, Node>(); //Holds all Nodes (same as in data)

    private Dictionary<Node, NodeData> data = new Dictionary<Node, NodeData>(); //Holds All Nodes -> NodeData gets added w pathfinding

    private List<NodeData> openNodes = new List<NodeData>();

    //DEBUG
    public bool debug = false;

    private List<LineRenderer> drawGraph = new List<LineRenderer>();
    public List<Vector2> nodesView = new List<Vector2>();

    public int NodesAmount = 0;
    public int NodesinPath = 0;
    //----


    Node startNode;
    Node endNode;


    Grid grid;
    StreetGenerator streetGen;
    GenTilePath pathBuilder;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        streetGen = GetComponent<StreetGenerator>();
        pathBuilder = GetComponent<GenTilePath>();

    }


    public LinkedList<Node> FindPath(Node startNode, Node endNode)
    {

        this.startNode = startNode;
        this.endNode = endNode;

        openNodes.Clear();

        NodeData currentNode = new NodeData(startNode, endNode);//NodeData.OpenStartNode(startNode, endNode);


        data[startNode] = currentNode;




        while (currentNode != null && currentNode.GetNode() != endNode)
        {

            Explore(currentNode);
            currentNode.Close();
            currentNode = RemoveBestOpenNode();


        }




        if (currentNode == null)
        {

            Debug.Log("No Path Found");


            return new LinkedList<Node>();
        }


        LinkedList<Node> path = currentNode.GetPath(new LinkedList<Node>()); //Gets Path from NodeData



        return path;

    }

    private void Explore(NodeData currentNode)
    {

        Dictionary<Node, Connection> nodesToExplore = currentNode.GetNode().GetConnections();



        foreach (KeyValuePair<Node, Connection> node in nodesToExplore)
        {
            //if (!data.ContainsKey(node.Key)) Debug.Log(node.Key.GetPosition()); //EDITED

            NodeData nData = data[node.Key];

            if (nData == null)
            {
                OpenNewNode(node.Key, currentNode, node.Value);
            }
            else if (nData.isOpen())
            {
                nData.TryDifferentPath(currentNode, node.Value);
            }

        }

       
    }

    private void OpenNewNode(Node newNode, NodeData currentNode, Connection connectingEdge)
    {

        NodeData nData = new NodeData(newNode, endNode, currentNode, connectingEdge);//NodeData.Open(newNode, endNode, currentNode, connectingEdge);
        data[newNode] = nData;
        openNodes.Add(nData);

      
    }



    private NodeData RemoveBestOpenNode()
    {
        NodeData bestNode = null;

        foreach (NodeData node in openNodes)
        {

            if (bestNode == null || node.GetScore() < bestNode.GetScore()) bestNode = node;
        }
        openNodes.Remove(bestNode);



        return bestNode;
    }




    private struct ECSPathNode {


        private Vector2 nodePos;

        private int distanceToEnd;
        private int distanceFromStart;

        private int previousNodeIndex;

        private int score;
        private bool open;


    }



    /*

    void UpdateAmoundDisplay()
    { //JUST DISPLAY

        NodesAmount = graph.Count;

    }



    //NODES


    public Node NewNode(Vector2 position)
    {

        if (NodeExist(position) == false)
        {
            Node node = new Node(position);

            graph.Add(position, node);

            data.Add(node, null);



            return node;
        }
        return graph[position];
    }




    public void AddAllNodes(Dictionary<Vector2, Node> nodes)
    {
        Debug.Log(nodes.Count);

        foreach (Node node in nodes.Values)
        {

            Debug.Log(node.GetPosition());
            NewNode(node.GetPosition());
        }

        if (debug) DrawConnectionGraph();

    }



    public void DeleteNode(Vector2 position)
    {

        if (graph.ContainsKey(position))
        {
            Node node = graph[position];

            data.Remove(node);
            graph.Remove(position);

        }

    }

    public Node GetNode(Vector2 position)
    {

        if (graph.ContainsKey(position))
        {
            return graph[position];


        }
        else return null;
    }


    public bool NodeExist(Vector2 position)
    {

        return graph.ContainsKey(position);
    }


    public void AddConnectionToNode(Vector2 positionNode, Vector2 positionDestination, Connection connection, int type)
    {

        NewNode(positionNode); //Make StartNode if it doesnt exist Already
        NewNode(positionDestination); //Make DestinationNode if it doesnt exist Already


        Node node = graph[positionNode];
        Node nodeDest = graph[positionDestination];

        connection.SetDestination(nodeDest);

        node.AddUpdateConnection(connection);  //Set connection to node object

        //Debug.Log("addedConFrom: " + node.GetPosition() + " to: " + nodeDest.GetPosition());


        UpdateAmoundDisplay();
        if (debug)
        {
            DrawConnectionGraph();
        }
        else DeleteConnectionGraph();
    }



    public void DeleteTempNodes(Node startNodeTemp, Node endNodeTemp)
    {

        if (startNodeTemp != null)
        {

            DeleteNode(startNodeTemp.GetPosition());
        }

        if (endNodeTemp != null)
        {

            if (endNodeTemp.endConnectionNode != null)
            {
                foreach (Node node in endNodeTemp.endConnectionNode)
                {

                    node.DeleteConnection(endNodeTemp.GetPosition());
                }
                endNodeTemp.endConnectionNode = null;
            }

            DeleteNode(endNodeTemp.GetPosition());

        }

        UpdateAmoundDisplay();

    }


    public void DeleteNodeData()
    {

        data.Clear();
        foreach (Node node in graph.Values)
        {

            data.Add(node, null);
        }
    }



    public void DrawConnectionGraph()
    {

        if (streetGen.loaded == false) return;

        UpdateAmoundDisplay();
        DeleteConnectionGraph();


        foreach (Node node in graph.Values)
        {


            Dictionary<Node, Connection> cons = node.GetConnections();



            foreach (KeyValuePair<Node, Connection> con in cons)
            {
                Vector2 startPos = node.GetPosition();
                Vector2 endPos = con.Value.GetDestination().GetPosition();



                LineRenderer lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
                lineRenderer.startColor = Color.red;
                lineRenderer.endColor = Color.red;
                lineRenderer.startWidth = 0.2f;
                lineRenderer.endWidth = 0.2f;
                lineRenderer.positionCount = 2;
                lineRenderer.useWorldSpace = true;

                lineRenderer.SetPosition(0, grid.WorldPositionFromGridTile(startPos));
                lineRenderer.SetPosition(1, grid.WorldPositionFromGridTile(endPos));

                drawGraph.Add(lineRenderer);
            }
        }

        foreach (Node node in data.Keys)
        {

            nodesView.Add(node.GetPosition());
        }
    }

    public void DeleteConnectionGraph()
    {

        foreach (LineRenderer lr in drawGraph)
        {

            Destroy(lr.gameObject);
        }
        drawGraph.Clear();
        nodesView.Clear();
    }



    */

}
