using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node
{

    
    private Vector2 position;
    //public int length;
    public int pathStartDirection;
    public List<Node> endConnectionNode = null;

    private Dictionary<Vector2, Connection> connections = new Dictionary<Vector2, Connection>();  //Vector2 = destination


    

    public Node(Vector2 position) {

        
        this.position = position;
        
    }



    public Vector2 GetPosition() {

        return position;
    }


    /*
    public void AddUpdateConnection(Connection con) {

        List<Connection> connectionsList = new List<Connection>();

        if (connections.ContainsKey(con.GetDestination().GetPosition())) { //If destination already exists

            connectionsList = connections[con.GetDestination().GetPosition()];

            foreach (Connection co in connectionsList) {

                if (co.GetDirection() == con.GetDirection()) connectionsList.Remove(co);  //Remove all connections in the same direction
                break;
            }

            connectionsList.Add(con);
            return;

        }

        //Destination doesnt exist

        connectionsList.Add(con);
        connections.Add(con.GetDestination().GetPosition(), connectionsList); //Add Connection

    }
    */
    public void AddUpdateConnection(Connection con)
    {

       

        if (connections.ContainsKey(con.GetDestination().GetPosition()))
        { //If destination already exists

            Connection co = connections[con.GetDestination().GetPosition()];

            if (co.GetDirection() == con.GetDirection()) connections[con.GetDestination().GetPosition()] = con;



            return;
        }

        //Destination doesnt exist      
        connections.Add(con.GetDestination().GetPosition(), con); //Add Connection



    }


    /*
    public Connection GetConnection(Vector2 destination) {

        if (connections.ContainsKey(destination))
        {
            Connection bestConnection = connections[destination][0];  //First element in connections

            foreach (Connection co in connections[destination])
            {
                if (co.GetLength() < bestConnection.GetLength()) bestConnection = co;
                
                
            }
            return bestConnection;
        }

        else return null;

    }
    */
    public Connection GetConnection(Vector2 destination)
    {

        if (connections.ContainsKey(destination))
        {
            return connections[destination];
        }

        else return null;

    }

    public void DeleteConnection(Vector2 destination)
    {

        if (connections.ContainsKey(destination))
        {
            connections.Remove(destination);
        }

    }
    public void DeleteAllConnections()
    {

        connections.Clear();
       
    }





    public Dictionary<Node, Connection> GetConnections() {

        Dictionary<Node, Connection> allConnections = new Dictionary<Node, Connection>();

        foreach (Connection con in connections.Values) {

            allConnections.Add(con.GetDestination(), con);
        }

        return allConnections;
    }


    public int CalcDistanceToEnd(Node end) {  //Heuristic function h: here Manhatten Distance

        return (int)(Mathf.Abs(position.x - end.GetPosition().x) + Mathf.Abs(position.y - end.GetPosition().y)); //distance

    }

}
