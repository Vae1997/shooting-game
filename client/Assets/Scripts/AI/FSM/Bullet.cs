using UnityEngine;

public class Bullet : MonoBehaviour {

    //子弹生命期
    public float destroyTime;

    //子弹威力
    private int damage = 1;
    public int Damage { get { return damage; } }

    //子弹速度
    private float speed = 1000f;
    public float Speed { get { return speed; } }

    //当前子弹是否已经碰撞
    public bool isColled = false;

	void Update () {
        //没有碰到的子弹会消失
		if(!isColled)
            Destroy(transform.gameObject, destroyTime);
	}

    //子弹是触发器
    void OnTriggerEnter2D(Collider2D coll)
    {
        //子弹碰到关卡触发器、子弹碰子弹，不处理
        if (coll.gameObject.name.Contains("Bullet"))
            return;
        isColled = true;
        //敌人子弹
        if (transform.GetComponent<SpriteRenderer>().color == Color.red)
            //碰敌人
            if (coll.gameObject.name.Contains("Enemy")) return;
        else if (transform.GetComponent<SpriteRenderer>().color == Color.blue)//玩家子弹
            //碰玩家
            if (coll.gameObject.name.Contains("Player") || coll.transform.parent.name == "OtherPlayers") return;
        Destroy(transform.gameObject);
        Player p = coll.gameObject.GetComponent<Player>();
        //碰到有效目标（敌人<-->玩家）
        if (p != null)
            //目标HP相应减少
            p.BeAttacked(Damage);
        AIController ai = coll.gameObject.GetComponent<AIController>();
        if (ai != null)
            ai.health -= Damage;
    }  
}
