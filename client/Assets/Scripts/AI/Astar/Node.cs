using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable {

    //A*中的G开销，从开始节点到当前节点的成本
    public float nodeTotalCost;
    //A*中的H开销，从当前节点到目标节点的成本
    public float estimatedCost;
    //节点是否为障碍物
    public bool isObstacle;
    //父节点
    public Node parent;
    //节点位置
    public Vector2 position;

    public Node()
    {
        this.nodeTotalCost = 1f;
        this.estimatedCost = 0f;
        this.isObstacle = false;
        this.parent = null;
    }

    public Node(Vector2 pos)
    {
        this.nodeTotalCost = 1f;
        this.estimatedCost = 0f;
        this.isObstacle = false;
        this.parent = null;
        this.position = pos;
    }

    public void MarkAsObstacle()
    {
        this.isObstacle = true;
    }

    public int CompareTo(object obj)
    {
        //用于排序
        Node node = (Node)obj;
        //this节点在路径列表中位于node节点后
        if (this.estimatedCost < node.estimatedCost)
            return -1;
        if (this.estimatedCost > node.estimatedCost)
            return 1; 
        return 0;
    }
}
