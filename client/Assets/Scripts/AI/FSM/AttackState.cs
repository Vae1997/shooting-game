using UnityEngine;

//进攻状态
public class AttackState : FSMState
{
    public AttackState(Transform[] wp)
    {
        waypoints = wp;
        stateID = FSMStateID.Attacking;
        curRotSpeed = 12.0f;
        curSpeed = 100.0f;
    }

    public override void Reason(Transform player, Transform npc)
    {
        //玩家死亡
        if (player == null)
        {
            //巡逻
            npc.GetComponent<AIController>().SetTransition(Transition.LostPlayer);
            return;
        }
        float dist = Vector3.Distance(npc.position, player.position);
        //进攻外,追击内
        if (dist >= attackDistance && dist < chaseDistance)
            //追击
            npc.GetComponent<AIController>().SetTransition(Transition.SawPlayer);
        //追击范围外
        else if (dist >= chaseDistance)
            //巡逻
            npc.GetComponent<AIController>().SetTransition(Transition.LostPlayer);
    }

    public override void Act(Transform player, Transform npc)
    {
        if (player == null) return;
        //获取脚本
        AIController aiCtrl = npc.GetComponent<AIController>();
        //目标位置
        destPos = player.position;
        //寻路
        aiCtrl.path.InitByAStarPath(npc.position, destPos);
        //npc位置
        Vector2 npcPos = new Vector2(npc.position.x, npc.position.y);
        //方向
        Vector2 dir = destPos - npcPos;
        //转向
        float targetRotation = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 30;
        //在T时间内完成旋转
        npc.rotation = Quaternion.Slerp(npc.rotation, Quaternion.Euler(0, 0, targetRotation), Time.deltaTime * curRotSpeed);
        //加载子弹预制体
        aiCtrl.bulletPrefab = Resources.Load("Bullet", typeof(Transform)) as Transform;
        //射击
        aiCtrl.ShootBullet(dir.normalized,npcPos + dir.normalized * 0.5f);
    }
}
