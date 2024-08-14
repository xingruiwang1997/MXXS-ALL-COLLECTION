using UnityEngine;

public class CC7_HPPackLogic : MonoBehaviour
{

    // ��������Collider���봥����ʱ������
    // Called when another Collider enters the trigger

    private void OnTriggerEnter(Collider other)
    {
        // �����봥�����������Ƿ���"Player"��ǩ
        // Check if the object entering the trigger has a "Player" tag
        if (other.CompareTag("MXXS_Player"))
        {
            // ��ӡ������Ϣ Print debug information
            Debug.Log("player�Ѿ�ʰȡ��HPpack");

            //Ѫ������+1 Number of HPPack +1
            CC0A_UserData.Instance.HPPack += 1;

            // ������Ч //Play sound effects
            CC0A_UserData.Instance.PV002_PlayAudioSource(CC0A_UserData.Instance.AS_PickUpItem);

            //�����岻��ʾ Make the object invisible
            //��ȡ�����ϵ� Renderer ��� Get the Renderer component on the object
            Renderer renderer = GetComponent<Renderer>();
            //��������� Renderer ������������ If the object has a Renderer component, disable it
            if (renderer != null)
            {
                renderer.enabled = false;
            }

            //ʱ��1��� �������� After 1 second, destroy the object
            Invoke("ToDestroy", 1f);
        }
    }

    private void ToDestroy()
    {
        // ����������� Destroy this object
        Destroy(gameObject);
    }
}