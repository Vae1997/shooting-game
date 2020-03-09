using System.Collections;
using UnityEngine;

public class AStar : MonoBehaviour {

    public static PriorityQueue closedList, openList;

    private static float HeuristicEstimateCost(Node curNode,Node goalNode)
    {
        Vector3 vecCost = curNode.position - goalNode.position;
        return vecCost.magnitude;
    }

    public static ArrayList FindPath(Node start,Node goal)
    {
        openList = new PriorityQueue();
        openList.Push(start);
        start.nodeTotalCost = 0f;
        start.estimatedCost = HeuristicEstimateCost(start, goal);

        closedList = new PriorityQueue();
        Node node = null;

        while(openList.Length!=0)
        {
            node = openList.First();
            if(node.position == goal.position)
                return CalculatePath(node);

            ArrayList neighbours = new ArrayList();
            GridManager.Instance.GetNeighbours(node, neighbours);

            for (int i = 0; i < neighbours.Count; i++)
            {
                Node neighbourNode = (Node)neighbours[i];

                if(!closedList.Contains(neighbourNode))
                {
                    float cost = HeuristicEstimateCost(node, neighbourNode);
                    float totalCost = node.nodeTotalCost + cost;
                    float neighbourNodeEstCost = HeuristicEstimateCost(neighbourNode, goal);
                    neighbourNode.nodeTotalCost = totalCost;
                    neighbourNode.parent = node;
                    neighbourNode.estimatedCost = totalCost + neighbourNodeEstCost;

                    if(!openList.Contains(neighbourNode))
                        openList.Push(neighbourNode);
                }
            }
            closedList.Push(node);
            openList.Remove(node);
        }
        if(node.position != goal.position) return null;
        return CalculatePath(node);
    }

    private static ArrayList CalculatePath(Node node)
    {
        ArrayList list = new ArrayList();
        while(node!=null)
        {
            list.Add(node);
            node = node.parent;
        }
        list.Reverse();
        return list;
    }
}
