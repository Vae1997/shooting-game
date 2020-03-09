using System.Collections;
using UnityEngine;

public class AIPath {

    public Node StartNode { get; set; }
    public Node GoalNode { get; set; }
    public ArrayList pathArray;

    //所有路点
    public Vector2[] wayPoints;

    //当前路点索引
    public int index = -1;

    //当前路点
    public Vector2 wayPoint;

    //路点半径
    public float deviation = 0.3f;

    //是否完成
    public bool isFinish = false;

    //是否到达当前路点
    public bool IsReach(Transform transform)
    {
        float distance = Vector2.Distance(wayPoint, transform.position);
        return distance <= deviation;
    }

    //下一个路点
    public void NextWaypoint()
    {
        if (index < 0)
            return;
        if(index < wayPoints.Length - 1)
            index++;
        else
            isFinish = true;
        wayPoint = wayPoints[index];
    }

    //根据A*算法初始化路径
    public void InitByAStarPath(Vector2 startPos,Vector2 endPos)
    {
        //重置
        wayPoints = null;
        index = -1;
        //计算路径
        StartNode = new Node(GridManager.Instance.GetGridCellCenter(GridManager.Instance.GetGridIndex(startPos)));
        GoalNode = new Node(GridManager.Instance.GetGridCellCenter(GridManager.Instance.GetGridIndex(endPos)));
        pathArray = AStar.FindPath(StartNode, GoalNode);
        if (pathArray == null) return;
        int length = pathArray.Count;
        wayPoints = new Vector2[length];
        for (int i = 0; i < length; i++)
        {
            //将路径节点的坐标存入
            Node node = (Node)pathArray[i];
            wayPoints[i] = node.position;
        }
        index = 0;
        wayPoint = wayPoints[index];
        isFinish = false;
    }

    //可视化调试路径
    public void DrawWaypoints()
    {
        if (pathArray == null) return;
            
        if (pathArray.Count > 0)
        {
            for (int index = 0; index < pathArray.Count; index++)
            {
                Node node = (Node)pathArray[index];
                if (index + 1 == pathArray.Count)
                    break;
                Node nextNode = (Node)pathArray[index + 1];
                Debug.DrawLine(node.position, nextNode.position, Color.green);
            }
        }
    }
}
