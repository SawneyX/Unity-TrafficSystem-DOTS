using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection : MonoBehaviour
{

    private Node destination;
    private int length;
    private int direction;
    private int type;
    //public Vector2 start;
    

    public Connection(Node destination, int direction, int length, int type)
    {
        this.destination = destination;
        this.length = length;
        this.direction = direction;
        this.type = type;
        
    }


    public Node GetDestination() {

        return destination;
    }
    public void SetDestination(Node node)
    {
        this.destination = node;
        return;
    }

    public int GetLength()
    {

        return length;
    }
    public int GetDirection()
    {

        return direction;
    }
    public int GetCost() {

        return length;// / (type + 1);  //edit
    }

}
