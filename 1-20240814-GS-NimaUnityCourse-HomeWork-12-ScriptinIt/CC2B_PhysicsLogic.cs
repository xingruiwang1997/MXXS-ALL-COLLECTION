using UnityEngine;

public class CC2B_PhysicsLogic : MonoBehaviour
{
    // 这个也是每一个物理体单用的，用来存储当前物理体的血量
    // This is also used by each physical body to store the health of the current physical body

    [Header("float:当前物理体的血量")] //float: the current health of the physics body
    public float PhysicBlood = 5f;

    [Header("float:当前物理体的血上限")] //float: The upper limit of the current physical body's health
    public float PhysicBloodMax = 5f;
}