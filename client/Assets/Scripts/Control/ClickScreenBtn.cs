using UnityEngine;
using UnityEngine.UI;

public class ClickScreenBtn : MonoBehaviour {

    //点击按钮的文本
    public Text curText;
    //闪烁间隔
    public float fadeInterval = 0.5f;
    //当前文本透明度
    private float alpha;
    //Option
    public GameObject option;
    //Tips
    public GameObject tips;
    //当前tip
    private GameObject curTip;
    //canvas
    public GameObject canvas;
    // Update is called once per frame
    void Update () 
    {
        if (curText == null) return;
        //文本透明度
        alpha = curText.canvasRenderer.GetAlpha();
        if (curText.IsActive())
        {
            if (alpha == 0)
                curText.CrossFadeAlpha(1, fadeInterval, false);
            else if (alpha == 1)
                curText.CrossFadeAlpha(0, fadeInterval, false);
        }
    }

    public void OnClick()
    {
        //点击开始
        if (curText.name == "startText")
        {
            if (curText.IsActive())
            {
                //点击开始，文本消失，操作按钮显示
                curText.gameObject.SetActive(false);
                option.SetActive(true);
            }
            else
            {
                //再次点击，操作按钮消失，文本显示
                curText.gameObject.SetActive(true);
                option.SetActive(false);
            }
        }
        //点击继续
        if (curText.name == "GoOnText")
        {
            tips.transform.GetChild(0).gameObject.SetActive(true);
            for(int i = 0; i < canvas.transform.childCount - 2; i++)
            {
                if(i < 5)
                    canvas.transform.GetChild(i).gameObject.SetActive(false);
                else
                    canvas.transform.GetChild(i).gameObject.SetActive(true);
            }
            //开始挂载脚本
            GameObject.Find("GridManager").AddComponent<Level>();
            GameObject.Find("GridManager").AddComponent<GridManager>();
            GameObject.Find("MainCamera").AddComponent<CameraFollow>();
        }
        //下一步
        if(curText.name == "nextText")
        {
            tips = GameObject.Find("Tips");
            curTip = transform.parent.gameObject;
            if (curTip.name == "t1")
                tips.transform.GetChild(1).gameObject.SetActive(true);
            else if(curTip.name == "t2")
                tips.transform.GetChild(2).gameObject.SetActive(true);
            else if(curTip.name == "t3")
                //战斗开始
                Level.startBattle = true;
            else if(curTip.name == "NextLevel")
            {
                //清除玩家
                GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
                foreach (GameObject t in players)
                    Destroy(t.gameObject);
                //重新挂载Level脚本
                GameObject.Find("GridManager").AddComponent<Level>();
                //重新挂载GridManager脚本
                GameObject.Find("GridManager").AddComponent<GridManager>();
                Level.startBattle = true;
            }
            curTip.SetActive(false);
        }
        //网络连接失败
        if(curText.name == "failText")
            transform.parent.gameObject.SetActive(false);
    }
}
