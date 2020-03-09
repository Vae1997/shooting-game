using System;
using System.Collections.Generic;
using UnityEngine;

public class CreateDragon
{
    private static CreateDragon instance;

    private float floor_length = 0.7f;

    private Transform floor, wall_ws, wall_ad;

    public static List<Vector2> wayPointsPos = new List<Vector2>();

    public static int wallNum = 0;

    private CreateDragon()
    {
        floor = Resources.Load("waypoint", typeof(Transform)) as Transform;
        wall_ws = Resources.Load("wall", typeof(Transform)) as Transform;
        wall_ad = Resources.Load("wall", typeof(Transform)) as Transform;
    }

    public static CreateDragon Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CreateDragon();
            }
            return instance;
        }
    }

    public void Creat(Tile[,] map, Action<Transform, Vector2> Instantiate)
    {
        for (int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                switch (map[i, j])
                {
                    case (Tile.DirtFloor)://导航点
                        Instantiate(floor, new Vector2(i * floor_length,j * floor_length));
                        wayPointsPos.Add(new Vector2(i * floor_length, j * floor_length));
                        break;
                    case (Tile.Wall_s)://左墙
                        Instantiate(wall_ws, new Vector2((i) * floor_length,j * floor_length));
                        wallNum++;
                        break;
                    case (Tile.Wall_w)://右墙
                        Instantiate(wall_ws, new Vector2((i) * floor_length,j * floor_length));
                        wallNum++;
                        break;
                    case (Tile.Wall_a)://下墙
                        Instantiate(wall_ad, new Vector2((i) * floor_length,(j) * floor_length));
                        wallNum++;
                        break;
                    case (Tile.Wall_d)://上墙
                        Instantiate(wall_ad, new Vector2((i) * floor_length,(j) * floor_length));
                        wallNum++;
                        break;
                    case (Tile.Corridor_ad):
                        Instantiate(floor, new Vector2(i * floor_length,j * floor_length));
                        Instantiate(wall_ws, new Vector2((i + 1f) * floor_length,j * floor_length));
                        Instantiate(wall_ws, new Vector2((i - 1f) * floor_length,j * floor_length));
                        wallNum+=2;
                        break;
                    case (Tile.Corridor_ws):
                        Instantiate(floor, new Vector2(i * floor_length,j * floor_length));
                        Instantiate(wall_ad, new Vector2(i * floor_length,(j + 1f) * floor_length));
                        Instantiate(wall_ad, new Vector2(i * floor_length,(j - 1f) * floor_length));
                        wallNum += 2;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
