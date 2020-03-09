using System.Collections;
using UnityEngine;

public class GridManager : MonoBehaviour {
     
    //地图行数
    public int numOfRows = 1;
    public int RowofMap { get { return numOfRows; } }

    //地图列数
    public int numOfColumns = 1;
    public int ColumnofMap{ get { return numOfColumns; } }

    //每个网格大小
    public static float gridCellSize = 0.7f;

    //显示网格
    public bool showGrid = false;

    //设置地图左下角位置
    private Vector3 origin = new Vector2(0, 0);

    //墙体列表
    private GameObject[] obstacleList;

    //网格中所有节点
    public Node[,] Nodes { get; set; }

    public Vector3 Origin
    {
        get { return origin; }
        set { origin = value; }
    }

    private static GridManager s_Instance = null;

    void Start()
    {
        numOfRows = 38;
        numOfColumns = 48;
        Origin = new Vector2(44.45f, 50.05f);
    }

    void Update()
    {
        //随机地图ok
        if(Level.mapIsOK)
        {
            transform.position = Map.Instance.GetMinPosOfMap();
            Origin = transform.position;
            if (obstacleList == null)
            {
                obstacleList = GameObject.FindGameObjectsWithTag("Obstacle");
                //确定网格行列数
                Vector2 minPos = Map.Instance.GetMinPosOfMap();
                Vector2 maxPos = Map.Instance.GetMaxPosOfMap();
                numOfColumns = Map.Instance.GetRowNumOfMap(minPos.x, maxPos.x) + 1;
                numOfRows = Map.Instance.GetColumnNumOfMap(minPos.y, maxPos.y) + 1;
                CalculateObstacles();
            }
        }
        //固定地图
        else if(ReadyBtn.isMultiGame)
        {
            if (obstacleList == null)
            {
                obstacleList = GameObject.FindGameObjectsWithTag("Obstacle");
                CalculateObstacles();
            }
        }
    }

    public static GridManager Instance
    {
        get
        {
            if(s_Instance == null)
                s_Instance = (GridManager)FindObjectOfType(typeof(GridManager));
            if (s_Instance == null)
                Debug.Log("Not Find GridManager in Scene!");
            return s_Instance;
        }
    }
    
    //找到所有障碍物
    void CalculateObstacles()
    {
        Nodes = new Node[numOfRows, numOfColumns];
        int index = 0;
        for(int i = 0; i < numOfRows; i++)
        {
            for(int j = 0; j < numOfColumns; j++)
            {
                Vector2 cellPos = GetGridCellCenter(index);
                Node node =  new Node(cellPos);
                Nodes[i, j] = node;
                index++;
            }
        }
        if(obstacleList != null && obstacleList.Length > 0)
        {
            foreach(GameObject curObstacle in obstacleList)
            {
                int indexCell = GetGridIndex(curObstacle.transform.position);
                int col = GetColumn(indexCell);
                int row = GetRow(indexCell);
                Nodes[row, col].MarkAsObstacle();
            }
        }
    }

    public int GetRow(int index)
    {
        return index / numOfColumns;
    }

    public int GetColumn(int index)
    {
        return index % numOfColumns;
    }

    public int GetGridIndex(Vector3 pos)
    {
        if(!IsInBounds(pos)) return -1;
        pos -= Origin;
        int col = (int)(pos.x / gridCellSize);
        int row = (int)(pos.y / gridCellSize);
        return (row * numOfColumns + col);
    }

    public bool IsInBounds(Vector3 pos)
    {
        float width = numOfColumns * gridCellSize;
        float height = numOfRows * gridCellSize;
        return (pos.x >= Origin.x && pos.x <= Origin.x + width && pos.y <= Origin.y + height && pos.y >= Origin.y);
    }

    public Vector2 GetGridCellCenter(int index)
    {
        Vector2 cellPos = GetGridCellPosition(index);
        cellPos.x += (gridCellSize / 2f);
        cellPos.y += (gridCellSize / 2f);
        return cellPos;
    }

    public Vector2 GetGridCellPosition(int index)
    {
        int row = GetRow(index);
        int col = GetColumn(index);
        float xPosInGrid = col * gridCellSize;
        float yPosInGrid = row * gridCellSize;
        return Origin + new Vector3(xPosInGrid, yPosInGrid);
    }

    //获取指定节点的相邻节点
    public void GetNeighbours(Node node,ArrayList neighbors)
    {
        Vector2 neighbourPos = node.position;
        int neighbourIndex = GetGridIndex(neighbourPos);
        int row = GetRow(neighbourIndex);
        int column = GetColumn(neighbourIndex);

        //下
        int leftNodeRow = row + 1;
        int leftNodeColumn = column;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //上
        leftNodeRow = row - 1;
        leftNodeColumn = column;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //右
        leftNodeRow = row;
        leftNodeColumn = column + 1;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);

        //左
        leftNodeRow = row;
        leftNodeColumn = column - 1;
        AssignNeighbour(leftNodeRow, leftNodeColumn, neighbors);
    }

    void AssignNeighbour(int row, int column, ArrayList neighbors)
    {
        if (row <= 0 || column <= 0) return;
        if(row!=-1&&column!=-1&&row<numOfRows&&column<numOfColumns)
        {
            if (Nodes == null) return;
            Node nodeToAdd = Nodes[row, column];
            if (!nodeToAdd.isObstacle)
                neighbors.Add(nodeToAdd);
        }
    }

    void OnDrawGizmos()
    {
        if(showGrid)
            DebugDrawGrid(transform.position, numOfRows, numOfColumns, gridCellSize, Color.black);
        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    void DebugDrawGrid(Vector3 origin, int numRows, int numCols, float cellSize, Color color)
    {
        float width = numCols * cellSize;
        float height = numRows * cellSize;

        for(int i = 0; i < numRows + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(0, 1, 0);
            Vector3 endPos = startPos + width * new Vector3(1, 0, 0);
            Debug.DrawLine(startPos, endPos, color);
        }

        for (int i = 0; i < numCols + 1; i++)
        {
            Vector3 startPos = origin + i * cellSize * new Vector3(1, 0, 0);
            Vector3 endPos = startPos + height * new Vector3(0, 1, 0);
            Debug.DrawLine(startPos, endPos, color);
        }
    }
}
