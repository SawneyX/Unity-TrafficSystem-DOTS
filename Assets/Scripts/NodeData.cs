using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeData
{

    private Node node;

    private int distanceToEnd;
    private int distanceFromStart;

    private NodeData previousNode;

    private int score;
    private bool open = true;



   

    public NodeData(Node newNode, Node endNode) {

        this.node = newNode;
        this.distanceToEnd = node.CalcDistanceToEnd(endNode);
        this.distanceFromStart = 0;
        this.UpdateScore();

    }
    public NodeData(Node newNode, Node endNode, NodeData previousNode, Connection connectingEdge)
    {

        this.node = newNode;
        this.distanceToEnd = node.CalcDistanceToEnd(endNode);
        
        this.distanceFromStart = connectingEdge.GetCost() + previousNode.distanceFromStart;
        this.previousNode = previousNode;
        this.UpdateScore();
        

    }


    public void TryDifferentPath(NodeData previousNode, Connection connectingEdge) {

        int newDistanceFromStart = previousNode.distanceFromStart + connectingEdge.GetCost();

        if (newDistanceFromStart < this.distanceFromStart) {
            this.previousNode = previousNode;
            this.distanceFromStart = newDistanceFromStart;
            UpdateScore();
        }

    }


    public LinkedList<Node> GetPath(LinkedList<Node> path) {

        path.AddFirst(node);
        if (previousNode != null) {
            previousNode.GetPath(path);
        }

        return path;
    }

    public Node GetNode() {

        return node;
    }

    public int GetScore() {
        return score;
    }
    public bool isOpen() {
        return open;
    }
    public void Close()
    {
        this.open = false;
    }

    private void UpdateScore() {
        this.score = distanceFromStart + distanceToEnd;
    }

}
