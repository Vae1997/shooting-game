using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Param : MonoBehaviour {

    public Text room_max_length_default,
        room_max_width_default,
        room_min_length_default,
        room_min_width_default,
        map_max_length_default,
        map_max_width_default,
        room_num_default,
        min_corridor_len_default,
        max_corridor_len_default,
        step_default,
        minBattleCount_default,
        maxBattleCount_default,
        minEnemyCount_default,
        maxEnemyCount_default,

        FPS_font_size_default,
        FPS_offset_y_default,

        room_max_length_set,
        room_max_width_set,
        room_min_length_set,
        room_min_width_set,
        map_max_length_set,
        map_max_width_set,
        room_num_set,
        min_corridor_len_set,
        max_corridor_len_set,
        step_set,
        minBattleCount_set,
        maxBattleCount_set,
        minEnemyCount_set,
        maxEnemyCount_set,

        FPS_font_size_set,
        FPS_offset_y_set;

    public static bool setParam = false;
    public static int room_max_length,
        room_max_width,
        room_min_length,
        room_min_width,
        map_max_length,
        map_max_width,
        room_num,
        min_corridor_len,
        max_corridor_len,
        step,
        minBattleCount,
        maxBattleCount,
        minEnemyCount,
        maxEnemyCount,

        FPS_font_size,
        FPS_offset_y;

    void FixedUpdate()
    {
        if (setParam)
        {
            SetNewParam();
        }
        else//默认
        {
            SetDefaultParam();
        }
    }

    private void SetDefaultParam()
    {
        room_max_length = int.Parse(room_max_length_default.text);
        room_max_width = int.Parse(room_max_width_default.text);
        room_min_length = int.Parse(room_min_length_default.text);
        room_min_width = int.Parse(room_min_width_default.text);
        map_max_length = int.Parse(map_max_length_default.text);
        map_max_width = int.Parse(map_max_width_default.text);
        room_num = int.Parse(room_num_default.text);
        min_corridor_len = int.Parse(min_corridor_len_default.text);
        max_corridor_len = int.Parse(max_corridor_len_default.text);
        step = int.Parse(step_default.text);
        minBattleCount = int.Parse(minBattleCount_default.text);
        maxBattleCount = int.Parse(maxBattleCount_default.text);
        minEnemyCount = int.Parse(minEnemyCount_default.text);
        maxEnemyCount = int.Parse(maxEnemyCount_default.text);

        FPS_font_size = int.Parse(FPS_font_size_default.text);
        FPS_offset_y = int.Parse(FPS_offset_y_default.text);
    }

    private void SetNewParam()
    {
        if (room_max_length_set.text != "")
            room_max_length = int.Parse(room_max_length_set.text);
        else
            room_max_length_default.text = room_max_length.ToString();

        if (room_max_width_set.text != "")
            room_max_width = int.Parse(room_max_width_set.text);
        else
            room_max_width_default.text = room_max_width.ToString();

        if (room_min_length_set.text != "")
            room_min_length = int.Parse(room_min_length_set.text);
        else
            room_min_length_default.text = room_min_length.ToString();

        if (room_min_width_set.text != "")
            room_min_width = int.Parse(room_min_width_set.text);
        else
            room_min_width_default.text = room_min_width.ToString();

        if (map_max_length_set.text != "")
            map_max_length = int.Parse(map_max_length_set.text);
        else
            map_max_length_default.text = map_max_length.ToString();

        if (map_max_width_set.text != "")
            map_max_width = int.Parse(map_max_width_set.text);
        else
            map_max_width_default.text = map_max_width.ToString();

        if (room_num_set.text != "")
            room_num = int.Parse(room_num_set.text);
        else
            room_num_default.text = room_num.ToString();

        if (min_corridor_len_set.text != "")
            min_corridor_len = int.Parse(min_corridor_len_set.text);
        else
            min_corridor_len_default.text = min_corridor_len.ToString();

        if (max_corridor_len_set.text != "")
            max_corridor_len = int.Parse(max_corridor_len_set.text);
        else
            max_corridor_len_default.text = max_corridor_len.ToString();

        if (step_set.text != "")
            step = int.Parse(step_set.text);
        else
            step_default.text = step.ToString();

        if (minBattleCount_set.text != "")
            minBattleCount = int.Parse(minBattleCount_set.text);
        else
            minBattleCount_default.text = minBattleCount.ToString();

        if (maxBattleCount_set.text != "")
            maxBattleCount = int.Parse(maxBattleCount_set.text);
        else
            maxBattleCount_default.text = maxBattleCount.ToString();

        if (minEnemyCount_set.text != "")
            minEnemyCount = int.Parse(minEnemyCount_set.text);
        else
            minEnemyCount_default.text = minEnemyCount.ToString();

        if (maxEnemyCount_set.text != "")
            maxEnemyCount = int.Parse(maxEnemyCount_set.text);
        else
            maxEnemyCount_default.text = maxEnemyCount.ToString();

        if (FPS_font_size_set.text != "")
            FPS_font_size = int.Parse(FPS_font_size_set.text);
        else
            FPS_font_size_default.text = FPS_font_size.ToString();

        if (FPS_offset_y_set.text != "")
            FPS_offset_y = int.Parse(FPS_offset_y_set.text);
        else
            FPS_offset_y_default.text = FPS_offset_y.ToString();
    }
}
