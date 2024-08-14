using UnityEngine;
//给IEnumeratot用的
using System.Collections;

public class CC5_PlayerMoveRotate : MonoBehaviour
{
    //这个是给 第一人称 玩家 进行 移动 旋转 逻辑用的。请附着到玩家的身上
    //This is for the first-person player to move and rotate logic. Please attach to the player

    /// <第一部分：调用CharacterController>
    /// Part001: Calling CharacterController

    void Awake()
    {
        CC = GetComponent<CharacterController>();
    }
    CharacterController CC;//调用CC Awake //Call CC Awake

    /// <第二部分：数值> Part 2: Values
    /// Part002: 玩家移动 跳跃 等的速度
    /// Part002: The speed at which the player moves, jumps, etc.

    //初始化Y的分量 //Initialize the Y component
    float Y = 0;
    //跳跃 //jump
    [Header("float:跳跃高度")]
    [SerializeField] float JumpSpeed = 5f;
    //移动 //Move
    [Header("float:移动速度")]
    [SerializeField] float Speed = 5f;
    [SerializeField] float WalkSpeed = 5f;
    [SerializeField] float RunSpeed = 12f;
    //冲刺CD时 禁止移动的Bool //Bool to prohibit movement during sprint CD
    bool CanChangeSpeed = true;

    //On Update
    private void Update()
    {
        V001_MoveandGravity();
        V002_MouseRotation();
    }

    /// <第三部分：冲刺> //Part 3: Sprint
    /// Part003: 冲刺CD的逻辑
    /// Part003: The logic of sprint CD

    IEnumerator ChangeSpeed()
    {
        //已经执行 冲刺 禁止冲刺
        //Sprint has been executed Sprint is prohibited
        CanChangeSpeed = false;

        //进入冲刺状态
        //Entering the sprint state
        Speed = RunSpeed;
        CC0A_UserData.Instance.PlayerStateString = "Run";

        yield return new WaitForSeconds(1);
        //回归走路状态 进入CD时间
        //Return to walking state and enter CD time
        Speed = WalkSpeed;
        CC0A_UserData.Instance.PlayerStateString = "CDing";

        yield return new WaitForSeconds(1);
        //CD结束，可以继续冲刺啦！
        //The CD is over, you can continue sprinting!
        CanChangeSpeed = true;
        CC0A_UserData.Instance.PlayerStateString = "Walk";
    }

    /// <第四部分：移动和重力的逻辑>
    /// Part004: Movement and Gravity Logic
    public void V001_MoveandGravity()
    {
        //冲刺逻辑 鼠标右键冲刺
        //Sprint logic Right click to sprint
        if (Input.GetMouseButtonDown(1) && CanChangeSpeed)
        {
            StartCoroutine(ChangeSpeed());
        }

        //判断物体是否在地面？
        //Determine whether the object is on the ground?
        bool IsOnGround = CC.isGrounded;

        // 使用 Raycast 进行额外的地面检测 //Additional ground detection using Raycast
        // 着地为真 不着地为否 没有着地的情况下，说明在飞翔 //True if it landed, false if it did not land. If it did not land, it means it was
        // flying.
        if (!IsOnGround) // 如果 isGrounded 返回 false  //If isGrounded returns false
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f)) // 从角色中心向下发射射线 
                //Shoot a ray downward from the center of the character
            {
                IsOnGround = true; // 如果射线检测到地面，认为角色在地面上 //If the ray hits the ground, the character is considered to be on
                                   // the ground
            }
        }

        //如果在地面上： //If on the ground:
        if (IsOnGround)
        {
            //添加摩擦力 //Adding Friction
            Y = -2f;
            //允许跳跃 //Allow jumping
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Y = JumpSpeed;
            }
            if (CanChangeSpeed)//正常状态下才可以walk状态，如果是冲刺，则进入冲刺状态 
                //You can only walk in normal state. If you are sprinting, you will enter the sprinting state.
            {
                CC0A_UserData.Instance.PlayerStateString = "Walk"; // 更新状态 //Update Status
            }
        }
        else if (!IsOnGround) //那就是在飞翔啊（暂时不区分 掉落 和 跳跃的 情况）
            //That means flying (not distinguishing between falling and jumping for the time being)
        {
            Y += Time.deltaTime * -9.81f; // Y分量重力加速度 //Y component gravity acceleration
            CC0A_UserData.Instance.PlayerStateString = "flying"; // 更新状态 //Update Status
        }

        //调用内置WASD输入值 //Call built-in WASD input value
        float T = Time.deltaTime; //速度 //speed
        float AD = Input.GetAxis("Horizontal"); //AD输入值 //AD input value
        float WS = Input.GetAxis("Vertical"); //WS输入值 //WS input value

        //设置速度 //Setting the speed
        Vector3 WASD = new Vector3(AD * Speed, Y, WS * Speed);
        WASD = this.transform.TransformDirection(WASD);//速度转向 //Speed ​​steering

        //执行位移 //Execute displacement
        CC.Move(WASD * T);

    }//Void001结束

    /// <第五部分：鼠标旋转的逻辑>
    /// Part005: Mouse rotation logic

    //旋转 //Rotation
    [Header("float:鼠标灵敏度")]
    [SerializeField] float MouseSensitivity = 1f;

    float Camx = 0;//Camx 旋转分量 默认朝向 //Camx rotation component default orientation
    public void V002_MouseRotation()
    {

        //this朝向MouseXs //This is towards MouseXs
        float MouseX = Input.GetAxis("Mouse X");
        //
        Vector3 RotateX = this.transform.localEulerAngles;//当前Y旋转分量 //Current Y rotation component
        RotateX.y += MouseX * MouseSensitivity;//加等 鼠标Y旋转输入 //Add mouse Y rotation input
        this.transform.localEulerAngles = RotateX;//执行旋转 //Performing a rotation

        //Camera朝向MouseY //Camera faces MouseY
        float MouseY = Input.GetAxis("Mouse Y");
        //
        Camx -= MouseY * MouseSensitivity;//减等 默认值0 //Decrement Default value 0
        Camx = Mathf.Clamp(Camx, -89f, 89f);//限制值 防止视觉颠倒 //Limit value to prevent visual inversion
        Camera.main.transform.localRotation = Quaternion.Euler(Camx, 0, 0);//执行 主摄像机旋转 //Execute Main Camera Rotation

    }//Void002结束
}