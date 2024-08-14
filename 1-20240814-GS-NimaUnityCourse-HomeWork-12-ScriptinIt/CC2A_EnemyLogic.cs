using UnityEngine;
using UnityEngine.AI;//给Nav Mesh用的 //For Nev Mesh Pro
using UnityEngine.UI;//给Image的UI用的 //For UI_Image


public class CC2A_EnemyLogic : MonoBehaviour
{
    /// <第一部分：怪物的状态> //Part 1: Monster States
    /// Part001: 这里调用到怪物的各种状态 包括怪物的enum 血量 侦测范围 攻击力 攻击声音 巡查点位等。虽然现在只有1个怪物，但是当以后怪物多起来 这些值都可以随意调整
    /// （不放在UserData里面）确保每个怪物的值都可以不一样 //(Not in UserData) Make sure each monster has a different value
    /// Part001: Here, various monster states are referenced, including the monster's enum, health, detection range, attack power, attack sound, patrol points, 
                 // and so on. Although there is currently only one monster, these values can be easily adjusted as more monsters are added in the future.

    // Enum量，用来表示当前的怪物状态
    // Enum value used to represent the current monster state.
    public enum EnemyState //怪物的3个Enum值 // The 3 enum values for the monster
    {
        Patrolling, //巡逻 //Patrolling
        Chasing, //追逐玩家 //Chasing Player
        Attacking, //进行攻击 //Attack Player
    }
    [Header("enum:当前 怪物状态（DEBUG用）")] //enum: Current monster state (for debugging)
    public EnemyState CurrentEnemyState;//当前怪物状态 需要public给UserData和UI调用 // The current monster state needs to be public for UserData and UI access
    [Header("enum:上一步 怪物状态（DEBUG用）")] //enum: Previous monster state (for debugging)
    public EnemyState LastEnemyState;//上一次的怪物状态 //// The previous monster state

    // 怪物的范围 我会在inspector里面去详细修改这些参数 可视化调整 这里的话，只放入最初始的值
    // The monster's range. I will adjust these parameters in the Inspector for detailed and visual adjustments.
    [Header("float:填写怪物的监测范围")] //float: Specify the monster's detection range
    [SerializeField] float MonsterDetectRange = 1.0f;
    [Header("float:填写怪物的攻击范围")] //float: Specify the monster's attack range
    [SerializeField] float MonsterAttackRange = 1.0f;

    // 在Scene里面显示怪物的范围，这个是只给开发者自己看的 也是在Scene里面调整范围的重要步骤
    // Display the monster's range in the Scene view; this is for the developer's reference only.
    void OnDrawGizmosSelected() //在视图里面画一个Gizmo // Draw a Gizmo in the view
    {
        //怪物的侦测范围  // Monster's detection range
        Gizmos.color = Color.yellow; //监测到玩家的时候是黄色  // Turns yellow when detecting the player
        Gizmos.DrawWireSphere(transform.position, MonsterDetectRange);

        //怪物的攻击范围  // Monster's attack range
        Gizmos.color = Color.red; //开始攻击玩家的时候是红色  // Turns red when starting to attack the player
        Gizmos.DrawWireSphere(transform.position, MonsterAttackRange);
    }

    // 怪物的血量  // Monster's health
    [Header("float:当前怪物的血量")] //float: Current health of the monster
    public float MonsterBlood = 100f;
    [Header("float:当前怪物的血上限")] //float: Current maximum health of the monster
    public float MonsterBloodMax = 100f;

    // 怪物的攻击力 // Monster's attack power
    [Header("float:怪物的攻击力")] //Monster's attack power
    [SerializeField] float MonsterAttack = 10f;
    [Header("float:怪物的攻击CD")] //float: Monster's attack cooldown
    [SerializeField] float MonsterAttackCD = 2f;

    // 怪物攻击时 转向玩家的速度
    // The speed at which the monster turns to face the player when attacking
    [Header("float:怪物的攻击时 转向玩家的速度")] //float: The speed at which the monster turns to face the player when attacking
    [SerializeField] float MonsterRotateSpeed = 10f;

    // 怪物攻击的声音 // Monster's attack sound
    [Header("audiosource:手动填入怪物攻击的声音")] //audiosource: Manually assign the monster's attack sound
    [SerializeField] AudioSource AS_EnemyAttackSound1_Monster; //这个是怪物攻击的声音 // This is the sound of the monster's attack
    [SerializeField] AudioSource AS_EnemyAttackSound2_PlayerShout; //这个是我自己 玩家 在叫的声音 // This is the sound of the player calling out

    NavMeshAgent navMeshAgent;//AI组件 通过代码调用到即可 无需暴露 // AI component can be accessed through code; no need to expose it.

    // 没有监测到玩家时候的巡逻点位 //Patrol point when no player is detected
    [Header("Transform:手动填入巡逻位置的GameObject 是数组(把所有相关的都塞进来吧)！")] //Transform: manually fill in the patrol position GameObject is an array!
    [SerializeField] private Transform[] PatrolPointsArray;
    int targetIndex = 0;

    // 侦测到玩家时 玩家的位置 代码调用 无需暴露// When the player is detected, the player's position code is called
    Transform playerTransform;//代表追逐战时，跟随角色的位置 通过find调用到主角 代码自动寻找，所以不需要我的手动拖入 
                              //During a chase, the character's position is followed. The code automatically finds the protagonist through the find call,
                              //so I don't need to drag it in manually.

    [Header("LayerMask：手动选择怪物检测玩家的Layer(请选择Player，不然的话，怪物监测不到玩家)")] //Manually select the monster detection player's layer
    [SerializeField] LayerMask PlayerLayerMask;//Inspector里面手动选择玩家的Layer //Manually select the player's Layer in the Inspector

    // 调用到怪物的材质里面的颜色 //Call the color in the monster's material
    [Header("GameObject：手动填入子物体：有Renderer 放怪物材质的哪个物体 不然的话，怪物无法改变自身的颜色")] //Manually fill in sub-objects: which object has the
                                                                         //Renderer to put the material on?
    [SerializeField] GameObject RendererObject;//调用到放了MeshRenderer的那个子物体 //Call the child object where the MeshRenderer is placed
    Color OriginalColor;//记录怪物初始的颜色 //Record the monster's initial color

    float OriginalNMAAISpeed;//记录怪物初始的行进速度 无需暴露 //Record the monster's initial speed

    // 怪物的血槽显示 //Monster health bar display
    [Header("Image：请手动拖入怪物的血量槽UI")] //Please manually drag into the monster's health slot UI
    [SerializeField] Image UImage_MonsterHPUI;

    // 怪物的动画机 代码调用 无需暴露//Monster Animator
    Animator MonsterAnimator;

    // 当前怪物的打击时间 计算CD用的 无需暴露//The current monster's attack time
    float currentMonsterAttackTime = 0;

    // 如果玩家打到怪物了，那么我肯定要去追逐玩家了，不管它有没有进入我的范围。在这段代码之外，这个值会被调整
    // If the player hits the monster, then I must chase the player, regardless of whether it enters my range or not. Outside of this code,
    // this value will be adjusted
    [Header("bool：判断是否应该追逐玩家(DEBUG用)")]
    public bool ShouldChasePlayer = false;

    /// <第二部分：Awake Start Update 里面分别需要什么逻辑> 
    /// Part002: 开局做什么 更新做什么
    /// Part002: What logic is needed in `Awake`, `Start`, `Update`.

    //On Awake
    void Awake()
    {

        //找到我的Nav组件 //Find my Nav component
        navMeshAgent = GetComponent<NavMeshAgent>();

        //找到动画机组件 //Find the Animator component
        MonsterAnimator = GetComponent<Animator>();
        if (MonsterAnimator != null)
        {
            //Debug.Log("CC2_EnemyLogic已经找到并且载入Monster动画机"); //CC2_EnemyLogic has found and loaded the Monster animator
        }

        //记录AI行走初始的速度 //Record the initial speed of AI walking
        OriginalNMAAISpeed = GetComponent<NavMeshAgent>().speed;
        //记录物体初始的颜色 //Record the initial color of the object
        OriginalColor = RendererObject.transform.GetComponent<Renderer>().material.color;
    }

    private void Start()
    {
        //初始状态为 巡逻 //The initial state is patrol
        CurrentEnemyState = EnemyState.Patrolling;
    }

    //On Update
    void Update()
    {
        // 监视 当 怪物状态变化时，输出debug，表达当前进入了什么状态 //Monitor when the monster's state changes, output debug information to
        // indicate the current state entered.
        CC0A_UserData.Instance.PV001_CheckEnumChange<EnemyState>
            (ref CurrentEnemyState, ref LastEnemyState, ref CC0A_UserData.Instance.EnemyStateString);

        // 怪物Enum状态什么时候状态会发生变化
        // When will the monster Enum status change?
        CC2A_V001_WhyEnemyStateChange();

        // 每一个状态 分别需要 执行什么逻辑
        // What logic needs to be executed in each state?
        CC2A_V002_HandleEnemyStateActions();

        // 更新血量UI
        // Updated health UI
        CC2A_V005_MonsterBloodUI();
    }

    /// <第三部分：主要功能区> //Part 3: Main Functionality Area
    /// Part003: 里面放了各种void 方法
    /// Part003: It contains various `void` methods.

    //【V001】负责处理 怪物的enum状态 为什么会发生变化？
    // [V001] Responsible for handling the monster's enum status. Why does it change?
    void CC2A_V001_WhyEnemyStateChange() //状态为什么会变化 //Why does the status change?
    {
        // 为了防止我没有玩家的时候 出错昂
        // To prevent errors when I don't have a player
        if (playerTransform == null)//代码自动寻找主角的的位置 //The code automatically finds the position of the protagonist
        {
            var player = GameObject.Find("Player");
            if (player != null)//如果找到了 //If you find
            {
                Debug.Log("CC2A_V001__已经寻找到主角组件");
                playerTransform = player.transform;
            }
            else
            {
                Debug.Log("CC2A_V001_未找到玩家，怪物保持当前状态 也就是巡逻。"); // Player not found, monster maintains current state.
            }
        }

        // 判断玩家是否进入范围：
        // 缓存检测结果，避免重复调用 // Cache the detection results to avoid redundant calls
        bool isPlayerInDetectRange = Physics.CheckSphere(this.transform.position, MonsterDetectRange, PlayerLayerMask);
        bool isPlayerInAttackRange = Physics.CheckSphere(this.transform.position, MonsterAttackRange, PlayerLayerMask);

        // 当存在玩家时
        // When there are players
        if (playerTransform != null)
        {
            if (!ShouldChasePlayer)// 如果不需要追逐玩家，那么就是正常的状态
            {
                //没有仇恨值的正常状态
                if (isPlayerInDetectRange) // 玩家进入侦测范围 //Player enters detection range 
                {
                    CurrentEnemyState = EnemyState.Chasing;

                    if (isPlayerInAttackRange) // 玩家进入攻击范围 //Player enters attack range
                    {
                        CurrentEnemyState = EnemyState.Attacking;
                    }
                }
                else
                {
                    CurrentEnemyState = EnemyState.Patrolling; // 玩家不在范围内，保持巡逻状态 //Player is out of range, keep patrolling
                }
            }
            else //否则说明需要追逐玩家 Otherwise, it means you need to chase the player
            {
                CurrentEnemyState = EnemyState.Chasing;

                if (isPlayerInDetectRange) // 玩家进入侦测范围 //Player enters detection range 
                {
                    ShouldChasePlayer = false;//那么清空这个子弹造成的仇恨值 Then clear the hatred value caused by this bullet
                }
            }
        } // 否则 说明不存在玩家 //Otherwise, there is no player.
    }

    //【V002】根据不同enum状态进行不同反应 执行不同逻辑
    // [V002] According to different enum states, different reactions and different logics are executed
    void CC2A_V002_HandleEnemyStateActions()
    {
        switch (CurrentEnemyState)
        {
            case EnemyState.Patrolling:

                //执行 巡逻 语句！ //Execute the patrol statement!
                CC2A_V004_Enum1_Patrolling();

                //恢复颜色 //Restore Color
                CC2A_V003_ChangeColor(OriginalColor);

                //重置所有动画 //Reset All Animations
                CC0A_UserData.Instance.PV008_ResetAnimations(MonsterAnimator, false, "IfRun", "IfAttack");

                //恢复初始速度 //Restore initial speed
                GetComponent<NavMeshAgent>().speed = OriginalNMAAISpeed;

                break;

            case EnemyState.Chasing:

                //执行 追逐 语句！ //Execute the chase statement!
                CC2A_V004_Enum2_Chasing();

                //变成黄色 //Turn yellow
                CC2A_V003_ChangeColor(Color.yellow);

                //跑步动画 //Animation //Running animation
                CC0A_UserData.Instance.PV009_PlayAAnimation(MonsterAnimator, true, "IfRun");
                CC0A_UserData.Instance.PV009_PlayAAnimation(MonsterAnimator, false, "IfAttack");

                //让速度加快 //Speed ​​up
                GetComponent<NavMeshAgent>().speed = 4F;

                break;

            case EnemyState.Attacking:

                if (CC0A_UserData.Instance.PlayerBlood > 0)//如果玩家还有血的话，才进行攻击 //If the player still has blood, then attack
                {
                    //执行 攻击 语句！ //Execute the statement!
                    CC2A_V004_Enum3_Attacking(); // 启动攻击 //Launch the attack
                }

                //变成红色 //Turns red
                CC2A_V003_ChangeColor(Color.red);

                //攻击动画 //Animation //Attack Animation
                CC0A_UserData.Instance.PV009_PlayAAnimation(MonsterAnimator, true, "IfAttack");
                CC0A_UserData.Instance.PV009_PlayAAnimation(MonsterAnimator, false, "IfRun");

                //怪物不可以移动 //Monsters cannot move
                GetComponent<NavMeshAgent>().speed = 0;

                break;
        }
    }

    //【V002_Extra1_】有组件的时候 变色的逻辑
    // [V002_Extra1_] Color change logic when there are components
    void CC2A_V003_ChangeColor(Color TargetColor)
    {
        //如果确实填入 且 找到了 Renderer 组件 则改变颜色 //If it is indeed filled in and the Renderer component is found, then change the color
        if (RendererObject != null)
        {
            RendererObject.transform.GetComponent<Renderer>().material.color = TargetColor;
        }
    }

    //【V002Add1】巡逻的逻辑
    // [V002Add1] The logic of patrolling
    void CC2A_V004_Enum1_Patrolling()
    {
        if (PatrolPointsArray != null && PatrolPointsArray.Length > 0)//如果我确实填入了物体Array的位置 //If I do fill in the location of the object array
        {
            navMeshAgent.SetDestination(PatrolPointsArray[targetIndex].position);

            if (CC2A_V004_Add1_IsNearDestination(PatrolPointsArray[targetIndex].position, 0.5f))//当开始接近一个目标时，转移到下一个目标 
                                                                                                //When you start to get close to a target,
                                                                                                //move on to the next target
            {
                //变换随机值 //Transform random values
                targetIndex = Random.Range(0, PatrolPointsArray.Length);
            }
        }
    }

    //【V002Extra1】方法：判断我和 目标位置 的距离 如果小于某个距离 则为真
    // [V002Extra1] Method: Determine the distance between me and the target position. If it is less than a certain distance, it is true.
    bool CC2A_V004_Add1_IsNearDestination(Vector3 destination, float Distance)
    {
        return Vector3.Distance(transform.position, destination) < Distance; //小于为真 大于为否 //Less than is true, greater than is false
    }

    //【V002Add2】追逐的逻辑
    // [V002Add2] The logic of pursuit
    void CC2A_V004_Enum2_Chasing()
    {
        if (playerTransform != null) // 确保找到玩家 Make sure you find the player
        {
            navMeshAgent.SetDestination(playerTransform.position);//并且追逐玩家 And chase the player
        }
    }

    //【V002Add3】攻击的逻辑
    // [V002Add3] The logic of Attack
    void CC2A_V004_Enum3_Attacking()
    {
        // 让怪物面向玩家 但是这个时候怪物就不推挤我这个玩家咯
        // Let the monster face the player, but at this time the monster will not push me, the player.
        if (playerTransform != null)
        {
            // 计算目标方向 //Calculate target direction
            Vector3 direction = playerTransform.position - transform.position;
            direction.y = 0; // 保持物体在水平面上的方向 //Keep the object oriented on the horizontal plane

            // 计算目标旋转 //Calculate target rotation
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 平滑过渡到目标旋转 //Smooth transition to target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, MonsterRotateSpeed * Time.deltaTime);
        }

        if (Time.time - currentMonsterAttackTime > MonsterAttackCD)//如果大于了攻击CD 也就是可以攻击 //If it is greater than the attack CD, it can attack
        {
            // 减血逻辑 //Blood loss logic
            CC0A_UserData.Instance.PlayerBlood -= MonsterAttack;

            // 播放音效 //Play sound effects
            CC0A_UserData.Instance.PV002_PlayAudioSource(AS_EnemyAttackSound1_Monster);//怪物攻击的吼叫声 The roar of the monster attacking
            CC0A_UserData.Instance.PV002_PlayAudioSource(AS_EnemyAttackSound2_PlayerShout);//我玩家的惨叫声 My player's screams

            // 输出玩家的血量 //Output player's health
            // Debug.Log("玩家当前血量：" + CC0_UserData.Instance.PlayerBlood);

            //更新打击时间 //Update strike time
            CC0A_UserData.Instance.PV003_UpdateTimeTime(ref currentMonsterAttackTime);
        }

    }


    //【V005】更新怪物血量UI的逻辑
    // [V005] Updated the logic of monster health UI
    void CC2A_V005_MonsterBloodUI() //这个是调用在Update里面的，因为要实时更新 This is called in Update because it needs to be updated in real time.
    {
        if (UImage_MonsterHPUI != null) //当我确实手动拖入了一个血量槽的时候 When I did manually drag in a health bar
        {
            UImage_MonsterHPUI.fillAmount = Mathf.InverseLerp(0, MonsterBloodMax, MonsterBlood);//0 血量 血上限 //0 HP HP Limit
        }
        else
        {
            Debug.Log("【MXXS的提醒】：需要拖入一个 怪物的血量槽UI"); //[MXXS's reminder]: You need to drag in a monster's health slot UI
        }
    }
}