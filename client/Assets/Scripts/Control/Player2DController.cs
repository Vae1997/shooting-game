using UnityEngine;

public class Player2DController : MonoBehaviour {

    //玩家移动速度
    public float playerV;

    //玩家视野范围
    public float playerSight;

    //border对象  
    public Transform border;

    //摇杆
    public Transform rocker;

    //键盘旋转
    public float rotationSpeed;

    //玩家
    private Transform player;

    //玩家的dir
    private Transform dir;

    //遥杆中心位置  
    private Vector2 centerPos;

    //触摸位置（鼠标点击位置）
    private Vector3 touchRockerPos;

    //鼠标在世界坐标系中的位置
    private Vector2 mouseWorldPos;

    //虚拟方向按钮可移动的半径  
    private float r;    

    //移动方向
    private Vector2 moveDir;   

    // Use this for initialization
    void Start ()
    {
        r = Vector2.Distance(transform.position, border.transform.position);
    }

    // Update is called once per frame
    void Update ()
    {
        //单人游戏控制
        if (NetAsyn.id == "Player")
        {
            if (player == null)
            {
                if (GameObject.Find("Player") == null) return;
                player = GameObject.Find("Player").transform;
                dir = player.GetChild(0);
            }
            //键盘控制移动
            // 使用上下方向键或者W、S键来控制前进后退
            float translation = Input.GetAxis("Vertical") * 5f * Time.deltaTime;
            //使用左右方向键或者A、D键来控制左右旋转
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            player.transform.Translate(0, -translation, 0); //沿着Z轴移动
            player.transform.Rotate(0, 0, -rotation); //绕Z轴旋转

            //实时更新当前摇杆中心位置
            centerPos = transform.position;
        }
        else if (NetAsyn.id == "")
            return;
        else
        {
            //多人游戏
            player = MultiLevel.players[NetAsyn.id].transform;
            dir = player.GetChild(0);
            //键盘控制移动
            // 使用上下方向键或者W、S键来控制前进后退
            float translation = Input.GetAxis("Vertical") * 5f * Time.deltaTime;
            //使用左右方向键或者A、D键来控制左右旋转
            float rotation = Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;
            player.transform.Translate(0, -translation, 0); //沿着Z轴移动
            player.transform.Rotate(0, 0, -rotation); //绕Z轴旋转

            //实时更新当前摇杆中心位置
            centerPos = transform.position;
        }
    }

    //拖拽摇杆  
    public void OnDragIng()
    {
        if (player.GetComponent<Player>().Hp == 0)
            return;
        //记录当前触控点数目
        int count = Input.touchCount;
        if(count == 0)
        {
            //方便电脑测试
            touchRockerPos = Input.mousePosition;
        }
        //单点触控,首个触控点的标志是0
        if (count == 1)
        {
            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                touchRockerPos = Input.GetTouch(0).position;
            }
        }
        else
        {
            //多点触控,遍历每个触摸点
            for (int i = 0; i < count; i++)
            {
                //有个手指正在移动
                if (Input.GetTouch(i).phase == TouchPhase.Moved)
                {
                    //位置在屏幕左下角
                    if (Input.GetTouch(i).position.x < Screen.width / 4 &&
                        Input.GetTouch(i).position.y < Screen.height / 2)
                    {
                        touchRockerPos = Input.GetTouch(i).position;
                    }
                }
            }
        }
        mouseWorldPos = Camera.main.ScreenToWorldPoint(touchRockerPos);//坐标转换
        if (Vector2.Distance(mouseWorldPos, centerPos) < r)//在圆圈内拖拽
        {
            rocker.transform.position = mouseWorldPos;
        }
        else
        {
            //计算出鼠标和原点之间的向量 
            Vector2 curdir = mouseWorldPos - centerPos;
            //关于normalized属性的解释，见日志18.01.03
            rocker.transform.position = centerPos + curdir.normalized * r;
        }        
        //确定移动方向
        moveDir = (mouseWorldPos - centerPos).normalized;
        //角色移动
        player.GetComponent<Rigidbody2D>().velocity = moveDir * playerV * Time.deltaTime;
        //确定黑色圆圈dir位置
        SetDirPos();
        //多人游戏拖拽发送位置信息
        if (NetAsyn.id != "Player")
        {
            SendPos();
            SendDirPos();
        }
            
    }

    private void SetDirPos()
    {
        //玩家临时瞄准的目标
        GameObject target = null;
        //最近敌人距离
        float minDistance = float.MaxValue;
        //所有敌人
        GameObject[] enemys = GameObject.FindGameObjectsWithTag("Enemy");
        for (int i = 0; i < enemys.Length; i++)
        {
            AIController enemy = enemys[i].GetComponent<AIController>();
            //当前敌人死亡
            if (enemy == null ||enemy.CurrentStateID == FSMStateID.Dead) continue;
            //当前敌人在视野外
            Vector2 playerPos = player.transform.position;
            Vector2 enemyPos = enemy.transform.position;
            float curDis = Vector2.Distance(playerPos, enemyPos);
            if (curDis > playerSight)
                continue;
            else
            {
                //当前视野内敌人距离
                if(curDis < minDistance)
                {
                    target = enemy.gameObject;
                    minDistance = curDis;
                }
            }
        }
        //敌人全部在视野外
        if (target == null)
        {
            //黑色圆圈位置跟随摇杆位置(player + 一定位移，0.25可调)
            dir.transform.position = new Vector2(player.transform.position.x, player.transform.position.y) + moveDir * 0.25f;
        }
        else
        {
            //如果有敌人target在视野之内，自动瞄准target
            Vector3 v = (target.transform.position - player.position).normalized;
            dir.transform.position = player.position + v * 0.25f;
        }
    }

    //发送位置协议
    private void SendPos()
    {
        Vector2 pos = player.transform.position;
        //组装协议
        string str = "POS ";
        str += NetAsyn.id + " ";
        str += pos.x.ToString() + " ";
        str += pos.y.ToString() + " ";

        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        NetAsyn.socket.Send(bytes);
        //Debug.Log("发送 " + str);
    }

    //发送玩家dir位置协议
    private void SendDirPos()
    {
        Vector2 pos = dir.transform.position;
        //组装协议
        string str = "DIRPOS ";
        str += NetAsyn.id + " ";
        str += pos.x.ToString() + " ";
        str += pos.y.ToString() + " ";

        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        NetAsyn.socket.Send(bytes);
        //Debug.Log("发送 " + str);
    }

    //松开摇杆  
    public void OnDragEnd()
    {
        if (player.GetComponent<Player>().Hp == 0)
            return;
        //松开鼠标虚拟摇杆回到原点  
        rocker.transform.localPosition = new Vector2(0, 0);
        //角色停止移动
        player.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
        player.GetComponent<Rigidbody2D>().angularVelocity = 0;
    }
}
