using UnityEngine;

public class CC2B_PhysicsLogic : MonoBehaviour
{
    // ���Ҳ��ÿһ�������嵥�õģ������洢��ǰ�������Ѫ��
    // This is also used by each physical body to store the health of the current physical body

    [Header("float:��ǰ�������Ѫ��")] //float: the current health of the physics body
    public float PhysicBlood = 5f;

    [Header("float:��ǰ�������Ѫ����")] //float: The upper limit of the current physical body's health
    public float PhysicBloodMax = 5f;
}