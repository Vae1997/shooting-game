using UnityEngine;

public class Player : MonoBehaviour {

    //子弹预制体
    private Transform prefabBullet;

    //上次发射子弹时间
    public float lastShootTime;

    //发射时间间隔
    public float shootInterval;

    //射速
    public float shootV;

    //dir
    public Transform dir;

    //player
    public Transform player;

    //当前HP
    public float hp;
    public float Hp { get { return hp; } }

    //玩家状态
    public enum PlayerState { died, player, }

    public PlayerState state = PlayerState.player;

    //发射方向
    private Vector2 playerShootDir;

    //dir位置
    private Vector2 dirPos;

    //player位置
    private Vector2 playerPos;

    //clone的子弹
    private Transform playerInsBullet;

    //当前需要实例化的子弹初始位置
    private Vector2 playerCurBulletPos;

	void Update ()
    {
        PlayerCtrl();
        if(NetAsyn.id != "Player")
        {
            if (playerInsBullet != null)
            {
                if(!playerInsBullet.GetComponent<Bullet>().isColled)
                {
                    SendBulletPos();
                }
                else
                {
                    Destroy(playerInsBullet);
                }
            }
        }
    }

    //玩家控制
    public void PlayerCtrl()
    {
        dirPos = dir.transform.position;
        playerPos = player.transform.position;
        //确定子弹发射方向
        playerShootDir = (dirPos - playerPos).normalized;
        //在dirPos基础上+一定的偏移，防止子弹和自己碰撞
        playerCurBulletPos = dirPos + playerShootDir * 0.5f;
    }

    //玩家被攻击
    public void BeAttacked(float attack)
    {
        if (hp <= 0)
            return;
        if(hp > 0)
            hp -= attack;
        if(hp <= 0)
        {
            //死亡变色
            player.GetComponent<SpriteRenderer>().color = Color.gray;
            //死亡层级为0
            player.GetComponent<SpriteRenderer>().sortingOrder = 0;
            player.GetChild(0).GetComponent<SpriteRenderer>().sortingOrder = 0;
            //死亡可通过,移除无用组件
            Destroy(player.GetComponent<CircleCollider2D>());
            Destroy(player.GetComponent<Rigidbody2D>());
            Destroy(player.GetComponent<AIController>());
            //死亡标记
            state = PlayerState.died;
            //死亡Tag重置
            player.transform.tag = "Untagged";
        }
    }

    //玩家射击
    public void Shoot()
    {
        //子弹冷却时间
        if (Time.time - lastShootTime < shootInterval)
            return;
        prefabBullet = Resources.Load("Bullet", typeof(Transform)) as Transform;
        //实例化子弹
        playerInsBullet = Instantiate(prefabBullet, playerCurBulletPos, new Quaternion(0, 0, 0, 0));
        //修改颜色区别子弹
        playerInsBullet.GetComponent<SpriteRenderer>().color = Color.blue;
        if(NetAsyn.id != "Player")
        {
            MultiLevel.bulletTag++;
            MultiLevel.bullets.Add(NetAsyn.id +" "+ MultiLevel.bulletTag, playerInsBullet.gameObject);
        }
        //子弹发射
        playerInsBullet.GetComponent<Rigidbody2D>().velocity = playerShootDir * shootV * Time.deltaTime;
        lastShootTime = Time.time;
    }

    //发送位置协议
    private void SendBulletPos()
    {
        Vector2 pos = playerInsBullet.transform.position;
        //组装协议
        string str = "BulletPOS ";
        str += NetAsyn.id + " ";
        str += MultiLevel.bulletTag + " ";
        str += pos.x.ToString() + " ";
        str += pos.y.ToString() + " ";

        byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
        NetAsyn.socket.Send(bytes);
        //Debug.Log("发送 " + str);
    }
}
