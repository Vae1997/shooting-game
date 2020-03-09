using UnityEngine;

public enum Tile
{
    Default,
    DirtWall,// 墙壁
    DirtFloor,// 房间地板
    Wall_w,//上方的墙
    Wall_s,//下方的墙
    Wall_a,//左方的墙
    Wall_d,//右方的墙
    Corridor_ad,//横向走廊
    Corridor_ws,//纵向走廊
    Door,// 房门
    UpStairs,// 入口
    DownStairs// 出口
}

public class Map
{
    private Tile[,] full_map;//地图

    private Vector2 minPos, maxPos;

    //房间的最大最小宽度，地图的最大长宽，房间的个数
    private int 
        room_max_width,
        room_min_length,
        room_min_width,
        map_max_length,
        map_max_width,
        room_num,
        min_corridor_len,
        max_corridor_len;

    private static Map instance;

    public static Map Instance
    {
        get
        {
            if (null == instance)
            {
                instance = new Map();
            }
            return instance;
        }
    }

    //是否第一次生成地图
    private bool _first;

    public void Init(int room_max_length, int room_max_width, int room_min_length, int room_min_width, int map_max_length, int map_max_width, int room_num, int min_corridor_len, int max_corridor_len)
    {
        full_map = new Tile[map_max_width, map_max_length];
        this.room_max_width = room_max_width;
        this.room_min_length = room_min_length;
        this.room_min_width = room_min_width;
        this.map_max_length = map_max_length;
        this.map_max_width = map_max_width;
        this.room_num = room_num;
        this.min_corridor_len = min_corridor_len;
        this.max_corridor_len = max_corridor_len;
        _first = true;
    }

    public void Make_dungeon(int step)
    {
        int x, y;
        int num = 0;
        for (int i = 0; i < step; i++)
        {
            x = Random.Range(0, map_max_width);
            y = Random.Range(0, map_max_length);
            if (MakeRoomAndCorridor(x, y))
            {
                num++;
            }
            if (num == room_num)
            {
                break;
            }
        }
        if (num < room_num)
        {
            Debug.Log("无法生成指定个数的房间！请确认数据的合法性或加大步数");
        }
    }

    private bool MakeRoomAndCorridor(int x, int y)
    {
        int xStart = -1, xEnd = -1, yStart = -1, yEnd = -1, width, length;
        width = Random.Range(room_min_width, room_max_width);
        length = Random.Range(room_min_length, room_min_length);//随机长宽
        if (_first)
        {
            xStart = map_max_width / 2 - width / 2;
            yStart = map_max_length / 2 - length / 2;
            xEnd = xStart + width;
            yEnd = yStart + length;

            if (IsInBounds_x(xStart) && IsInBounds_x(xEnd) && IsInBounds_y(yStart) && (IsInBounds_y(yEnd)))
            {
                if (IsAreaUnused(xStart, yStart, xEnd, yEnd))
                {
                    _first = false;//如果是第一次创建房间的话，就在地图中间生成一个
                    Creat_room(xStart, yStart, xEnd, yEnd);
                    return true;
                }
                else
                    return false;
            }
        }
        else
        {
            if ((full_map[x, y] == Tile.Wall_a) 
                || (full_map[x, y] == Tile.Wall_w) 
                || (full_map[x, y] == Tile.Wall_s) 
                || (full_map[x, y] == Tile.Wall_d))
            {
                //生成走廊与房间
                int corridor_length = Random.Range(min_corridor_len - 2, max_corridor_len - 1);
                int c_xStart = -1, c_xEnd = -1, c_yStart = -1, c_yEnd = -1;
                int away = Random.Range(1, length - 1);//偏移量，房间之间的错位
                Tile type = Tile.Default;
                //根据得到的墙壁类型决定房间和走廊的位置
                switch (full_map[x, y])
                {
                    case (Tile.Wall_a)://下墙
                        xStart = x - away;
                        xEnd = x + width;
                        yEnd = y - corridor_length - 1;
                        yStart = yEnd - length;
                        c_yEnd = y;
                        c_yStart = y - corridor_length - 1;
                        c_xEnd = x;
                        c_xStart = x;
                        type = Tile.Corridor_ad;
                        break;
                    case (Tile.Wall_d)://上墙
                        xStart = x - away;
                        xEnd = x + width;
                        yStart = y + corridor_length + 1;
                        yEnd = yStart + length;
                        c_yStart = y;
                        c_yEnd = y + corridor_length + 1;
                        c_xEnd = x;
                        c_xStart = x;
                        type = Tile.Corridor_ad;
                        break;
                    case (Tile.Wall_w)://右墙
                        yStart = y - away;
                        yEnd = yStart + length;
                        xStart = x + corridor_length + 1;
                        xEnd = xStart + width;
                        c_xStart = x;
                        c_xEnd = x + corridor_length + 1;
                        c_yStart = y;
                        c_yEnd = y;
                        type = Tile.Corridor_ws;
                        break;
                    case (Tile.Wall_s)://左墙
                        yStart = y - away;
                        yEnd = yStart + length;
                        xEnd = x - corridor_length - 1;
                        xStart = xEnd - width;
                        c_xEnd = x;
                        c_xStart = x - corridor_length - 1;
                        c_yStart = y;
                        c_yEnd = y;
                        type = Tile.Corridor_ws;
                        break;
                    default:
                        break;
                }
                if (IsAreaUnused(xStart, yStart, xEnd, yEnd))
                {
                    Creat_room(xStart, yStart, xEnd, yEnd);
                    Creat_Corridor(c_xStart, c_yStart, c_xEnd, c_yEnd, type);//判断是否在地图内，然后生成
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        return true;
    }

    private void Creat_Corridor(int xStart, int yStart, int xEnd, int yEnd, Tile t)
    {
        for (int i = xStart; i <= xEnd; i++)
            for (int j = yStart; j <= yEnd; j++)
                full_map[i, j] = t;
    }

    private void Creat_room(int xStart, int yStart, int xEnd, int yEnd)
    {
        for (int i = xStart + 1; i < xEnd; i++)
            for (int j = yStart + 1; j < yEnd; j++)
                full_map[i, j] = Tile.DirtFloor;
        for (int i = xStart + 1; i < xEnd; i++)
        {
            full_map[i, yStart] = Tile.Wall_a;
            full_map[i, yEnd] = Tile.Wall_d;
        }
        for (int j = yStart + 1; j < yEnd; j++)
        {
            full_map[xStart, j] = Tile.Wall_s;
            full_map[xEnd, j] = Tile.Wall_w;
        }
    }

    private bool IsAreaUnused(int xStart, int yStart, int xEnd, int yEnd)
    {
        for (int i = xStart; i <= xEnd; i++)
            for (int j = yStart; j <= yEnd; j++)
                if (full_map[i, j] != Tile.Default)
                    return false;
        return true;
    }

    private bool IsInBounds_y(int y)
    {
        if ((y < 0) || (y > map_max_length - 1))
            return false;
        else
            return true;
    }

    private bool IsInBounds_x(int x)
    {
        if ((x < 0) || (x > map_max_width - 1))
            return false;
        else
            return true;
    }

    public Tile[,] Getmap()
    {
        return (full_map);
    }

    //获取地图左下角坐标，用于设置Grid初始位置
    public Vector2 GetMinPosOfMap()
    {
        float x = float.MaxValue;
        float y = float.MaxValue;
        Transform dungeon = GameObject.Find("dungeon").transform;
        for (int i = 0; i < dungeon.childCount; i++)
        {
            Transform curTile = dungeon.GetChild(i);
            float curX = curTile.position.x;
            float curY = curTile.position.y;
            if (curX < x)
                x = curX;
            if (curY < y)
                y = curY;
        }
        minPos = new Vector2(x - 0.35f, y - 0.35f);
        return minPos;
    }

    //获取地图右上角坐标
    public Vector2 GetMaxPosOfMap()
    {
        float x = 0;
        float y = 0;
        Transform dungeon = GameObject.Find("dungeon").transform;
        for (int i = 0; i < dungeon.childCount; i++)
        {
            Transform curTile = dungeon.GetChild(i);
            float curX = curTile.position.x;
            float curY = curTile.position.y;
            if (curX > x)
                x = curX;
            if (curY > y)
                y = curY;
        }
        maxPos = new Vector2(x, y);
        return maxPos;
    }

    //获取地图网格行数
    public int GetRowNumOfMap(float minY, float maxY)
    {
        int rowNum = 0;
        float disY = maxY - minY;
        rowNum = (int)(disY / GridManager.gridCellSize);
        return rowNum;
    }

    //获取地图网格列数
    public int GetColumnNumOfMap(float minX, float maxX)
    {
        int colNum = 0;
        float disX = maxX - minX;
        colNum = (int)(disX / GridManager.gridCellSize);
        return colNum;
    }
}
