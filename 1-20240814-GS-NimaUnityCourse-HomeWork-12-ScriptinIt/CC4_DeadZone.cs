using UnityEngine;

public class CC4_DeadZone : MonoBehaviour
{

    // ��������Collider���봥����ʱ������ �������Ӧ�ñ������������
    // Called when another Collider enters the trigger. This code should be placed in the restricted area.

    private void OnTriggerEnter(Collider other)
    {
        // �����봥�����������Ƿ���"Player"��ǩ
        // Check if the object entering the trigger has a "Player" tag
        if (other.CompareTag("MXXS_Player"))
        {
            // ��ӡ������Ϣ Print debug information
            Debug.Log("player�Ѿ�����DeadZone");

            // ��PlayerѪŪ��0 Set the player's health to 0
            CC0A_UserData.Instance.PlayerBlood = 0;

            // ����������Ч //Play DEATH sound effects
            CC0A_UserData.Instance.PV002_PlayAudioSource(CC0A_UserData.Instance.AS_DieScream);

            //��Ϸ ��ͣ //Game Pause
            CC0A_UserData.Instance.isGamePaused = true;
        }
    }
}