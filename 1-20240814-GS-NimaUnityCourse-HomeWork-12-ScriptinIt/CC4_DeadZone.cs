using UnityEngine;

public class CC4_DeadZone : MonoBehaviour
{

    // 当有其他Collider进入触发器时被调用 这个代码应该被放入禁区里面
    // Called when another Collider enters the trigger. This code should be placed in the restricted area.

    private void OnTriggerEnter(Collider other)
    {
        // 检查进入触发器的物体是否有"Player"标签
        // Check if the object entering the trigger has a "Player" tag
        if (other.CompareTag("MXXS_Player"))
        {
            // 打印调试信息 Print debug information
            Debug.Log("player已经调入DeadZone");

            // 把Player血弄成0 Set the player's health to 0
            CC0A_UserData.Instance.PlayerBlood = 0;

            // 播放死亡音效 //Play DEATH sound effects
            CC0A_UserData.Instance.PV002_PlayAudioSource(CC0A_UserData.Instance.AS_DieScream);

            //游戏 暂停 //Game Pause
            CC0A_UserData.Instance.isGamePaused = true;
        }
    }
}