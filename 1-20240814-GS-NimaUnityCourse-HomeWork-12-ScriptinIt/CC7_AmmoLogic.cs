using NUnit.Framework;
using UnityEngine;

public class CC7_AmmoLogic : MonoBehaviour
{

    // 当有其他Collider进入触发器时被调用
    // Called when another Collider enters the trigger

    private void OnTriggerEnter(Collider other)
    {
        // 检查进入触发器的物体是否有"Player"标签
        // Check if the object entering the trigger has a "Player" tag
        if (other.CompareTag("MXXS_Player"))
        {
            // 打印调试信息 Print debug information
            Debug.Log("player已经拾取到ammo");

            //子弹数量+1 Number of AmmoPack +1
            CC0A_UserData.Instance.AmmoPack += 1;

            // 播放音效 //Play sound effects
            CC0A_UserData.Instance.PV002_PlayAudioSource(CC0A_UserData.Instance.AS_PickUpItem);

            //让物体不显示 Make the object invisible
            //获取物体上的 Renderer 组件 Get the Renderer component on the object
            Renderer renderer = GetComponent<Renderer>(); 
            //如果物体有 Renderer 组件，将其禁用 If the object has a Renderer component, disable it
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            //时隔1秒后 销毁物体 After 1 second, destroy the object
            Invoke("ToDestroy", 1f);
        }
    }

    private void ToDestroy()
    {
        // 销毁这个物体 Destroy this object
        Destroy(gameObject);
    }
}