using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ReadyBtn : MonoBehaviour {

    public static bool isMultiGame = false;
    public Text countText;
    public void OnClickReadyBtn()
    {
        isMultiGame = true;
        if (countText.text != "")
        {
            Debug.Log("开始多人游戏");
            //切换到多人游戏固定场景
            SceneManager.LoadScene(1);
        }
    }
}
