using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BackBtn : MonoBehaviour {

    public GameObject canvas;

    public void OnBackBtnClick()
    {
        if(transform.name == "BackBtn" || transform.name == "OKBtn")
        {
            canvas.transform.GetChild(0).gameObject.SetActive(false);
            canvas.transform.GetChild(4).gameObject.SetActive(false);
            canvas.transform.GetChild(2).gameObject.SetActive(false);
            canvas.transform.GetChild(1).gameObject.SetActive(true);
            //startText置为true
            canvas.transform.GetChild(1).transform.GetChild(0).gameObject.SetActive(true);
            //开始按钮为true
            transform.parent.GetChild(transform.parent.childCount - 1).gameObject.SetActive(true);
            if(transform.name == "OKBtn")
            {
                //取值并设置 
                Param.setParam = true;
            }
        }
        if (transform.name == "OverBtn" || transform.name == "GoHome")
        {
            Debug.Log("游戏结束");
            if(NetAsyn.socket!=null && NetAsyn.id != "Player")
            {
                NetAsyn.socket.Send(System.Text.Encoding.Default.GetBytes("LEAVE " + NetAsyn.id));
            }
            if(transform.name == "GoHome")
            {
                if (!ReadyBtn.isMultiGame)
                {
                    //路点列表清空
                    CreateDragon.wayPointsPos.Clear();
                    Level.startBattle = false;
                }
                else
                {
                    NetAsyn.id = "Player";
                    ReadyBtn.isMultiGame = false;
                }
                SceneManager.LoadScene(0);
            }
            else
            {
                #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                #else
                    Application.Quit();
                #endif
            }
        }
    }
}
