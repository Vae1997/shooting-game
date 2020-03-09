using UnityEngine;

public class Shoot : MonoBehaviour {
    //实现玩家点击按钮射击
    public void Shooting()
    {
        Player player = GameObject.Find(NetAsyn.id).GetComponent<Player>();
        player.Shoot();
    }
}
