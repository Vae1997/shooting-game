using UnityEngine;

public class CameraFollow : MonoBehaviour {

    //相机跟随的玩家
    private Transform followPlayer;
	
	// Update is called once per frame
	void Update () {

        if(followPlayer == null)
        {
            if(ReadyBtn.isMultiGame)
                followPlayer = GameObject.Find(NetAsyn.id).transform;
            else
                followPlayer = GameObject.Find("Player").transform;
        }
        
        transform.position = new Vector3(followPlayer.position.x,followPlayer.position.y, transform.position.z);
    }
}
