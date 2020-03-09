using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FPS : MonoBehaviour
{

    float lastTime = 0f;
    float timePerFrame = 0f;
    float updateFrequence = 0.5f;
    int frameCount;
    string strResult;

    float tpf_sum;
    float timePerFrame_min, timePerFrame_mean, timePerFrame_max;
    float tpf_InvalidMin, tpf_InvalidMax;
    int recordedTimePerFrameCount = 0;
    List<float> tpfRecord = new List<float>();

    Rect displayRect = new Rect(0, 0, 150, 100);

    private float m_fScaleWidth;
    private float m_fScaleHeight;

    void Start()
    {
        m_fScaleWidth = float.Parse(Screen.width.ToString()) / 1024;
        m_fScaleHeight = float.Parse(Screen.height.ToString()) / 768;

        Application.targetFrameRate = 600;

        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        Reset();
    }

    void LateUpdate()
    {
        frameCount++;
        if (frameCount == 0)
        {
            lastTime = Time.unscaledTime;
            return;
        }
        var curTime = Time.unscaledTime;
        var timeDiff = curTime - lastTime;
        if (timeDiff >= updateFrequence && frameCount > 0)
        {
            timePerFrame = 1000 * timeDiff / frameCount;
            RefreshFps();
            frameCount = 0;
            lastTime = curTime;
        }
    }

    void RefreshFps()
    {
        tpf_sum += timePerFrame;
        recordedTimePerFrameCount++;
        if (recordedTimePerFrameCount > 4)
        {
            if (timePerFrame < tpf_InvalidMin)
            {
                timePerFrame_min = tpf_InvalidMin;
                tpf_InvalidMin = timePerFrame;
            }
            else if (timePerFrame < timePerFrame_min)
                timePerFrame_min = timePerFrame;

            if (timePerFrame > tpf_InvalidMax)
            {
                timePerFrame_max = tpf_InvalidMax;
                tpf_InvalidMax = timePerFrame;
            }
            else if (timePerFrame > timePerFrame_max)
                timePerFrame_max = timePerFrame;

            timePerFrame_mean = (tpf_sum - tpf_InvalidMax - tpf_InvalidMin) / (recordedTimePerFrameCount - 2);
            strResult = string.Format(
                "{0:F1}FPS, {1:F1}ms\nms:{2:F1}, {3:F1}, {4:F1}",
                1000f / timePerFrame, timePerFrame,
                timePerFrame_min, timePerFrame_mean, timePerFrame_max
                );
        }
        else
        {
            tpfRecord.Add(timePerFrame);
            if (recordedTimePerFrameCount == 4)
            {
                tpfRecord.Sort();
                tpf_InvalidMin = tpfRecord[0];
                timePerFrame_min = tpfRecord[1];
                timePerFrame_max = tpfRecord[2];
                tpf_InvalidMax = tpfRecord[3];
            }
            strResult = string.Format(
                "{0:F1}FPS, {1:F1}ms",
                1000f / timePerFrame, timePerFrame
                );
        }
    }

    void Reset()
    {
        tpfRecord.Clear();
        strResult = "Calculating FPS...";
        tpf_sum = 0;
        timePerFrame_min = float.MaxValue;
        tpf_InvalidMin = float.MaxValue;
        timePerFrame_mean = 0;
        timePerFrame_max = float.MinValue;
        tpf_InvalidMax = float.MinValue;
        recordedTimePerFrameCount = 0;
        frameCount = -1;
    }

    void OnGUI()
    {
        GUIStyle bb = new GUIStyle();
        bb.normal.background = null;    //背景填充的
        bb.normal.textColor = Color.green;
        bb.fontSize = Param.FPS_font_size;               //字体大小

        displayRect = new Rect(0, Screen.height - Param.FPS_offset_y, 100 * m_fScaleWidth, 100 * m_fScaleHeight);
        if (GUI.Button(displayRect, strResult,bb))
            Reset();
    }
}
