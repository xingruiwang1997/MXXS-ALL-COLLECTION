using System.Collections;
using UnityEngine;
using static CC2A_EnemyLogic;
using static CC0A_UserData;
using Unity.VisualScripting;

public class CC1_GunShootWhole : MonoBehaviour
{
    // CC1_GunShootWhole 代码 包括了 和 子弹射击 相关的全部逻辑： 包括用户输入、音效、动画、射线 等内容
    // The `CC1_GunShootWhole` code includes all the logic related to bullet shooting: this encompasses user input, sound effects, animations,
    // and raycasting.

    /// <第一部分：需要的美术资源> Part 1: Required Art Assets
    /// Part001: 包括各种 子弹发射音效 击中音效 //枪械动画 //击中逻辑 击中生成体 等
    /// Part001: Including various bullet firing sound effects, impact sound effects, weapon animations, hit logic, and impact effects, etc.

    // 音效资源 记得在 Inspector界面 取消勾选 PlayOnAwake
    // For sound effect resources, remember to uncheck "Play On Awake" in the Inspector window.
    [Header("AudioSource：需要手动拖入：射击的音效")] //Manually drag in: shooting sound effects
    [SerializeField] AudioSource AS_GunShoot;//枪械射击的声音 //The sound of gunfire
    [Header("AudioSource：需要手动拖入：击中金属体的音效")] //Manually drag in: sound effect for hitting metal objects
    [SerializeField] AudioSource AS_MetalHit; //击中金属物体，比如罐头之类的 //For hitting metal objects, such as cans.
    [Header("AudioSource：需要手动拖入：击中生物体的音效1")]//Manually drag in: sound effect for hitting biological targets 1
    [SerializeField] AudioSource AS_BioHit1;//击中肉类的声音 //Sound effect for hitting flesh meet
    [Header("AudioSource：需要手动拖入：击中生物体的音效2")]//Manually drag in: sound effect for hitting biological targets 2
    [SerializeField] AudioSource AS_BioHit2;//血液喷溅的余音缭绕 //The lingering sound of blood splatter
    [Header("AudioSource：需要手动拖入：击中爆炸音效")]//Manually drag in: sound effect for explosions on impact.
    [SerializeField] AudioSource AS_Explode;//怪物死亡后绝望的爆炸声音，给玩家听觉上的冲击力 //Desperate explosion sound after the monster's
                                            //death, providing an auditory impact for the player.

    // 动画资源
    // Animation assets
    Animator GunAnimator;//射击的动画 //因为只有一个Animator Component，因此在Awake里面直接查找即可 // Shooting animation // Since there is only one
                         //Animator component, it can be found directly in the `Awake` method.

    // 记录当前射击时间 用于计算射击CD //Record the shooting time to calculate the shooting cooldown.
    float currentShootTime = 0f;//通过调用到 CC0预设 的 CD施加实现 //Achieve cooldown application by calling the CC0 prefab.

    // 射线相关
    // Raycasting-related
    Ray ray;//射线 //The Ray
    RaycastHit hit;//射线击中物 //The Raycast Hit
    int maxRaycastIterations = 10; // 添加最大循环次数，防止无限循环 //Add a maximum number of loops to prevent infinite loops


    /// <第二部分：Awake Start Update FixedUpdate 里面分别需要什么逻辑> 
    /// Part002: 开局做什么 更新做什么
    /// Part002: What logic is needed in `Awake`, `Start`, `Update`, and `FixedUpdate`.

    //On Awake
    void Awake()
    {
        // 调用动画机组件
        // Call the Animator component
        GunAnimator = GetComponent<Animator>();
        if (GunAnimator != null) // 如果没有找到，说明我没有拖入 //If not found, it means I haven't dragged it in.
        {
            Debug.Log("CC1_GunShootWhole 已加载 枪械 动画机组件\n" +
                      "MXXS结束\n"); //`CC1_GunShootWhole` has loaded the firearm Animator component.
        }
        else //使其 就算没有塞Animator GunAnimator也不会报错 //Ensure that it doesn't throw an error even if the `Animator` component, `GunAnimator`, is not
             //assigned.
        {
            Debug.Log("【MXXS的提醒】：CC1_GunShootWhole 的 Awake 里面 需要一个动画机组件喵，快点给我拖进来啊喵！！！\n" +
                      "MXXS结束\n");
            //[MXXS Reminder]:An Animator component is needed, meow. Please drag it in quickly!
        }

        //枪械切换为没有任何状态  //Switch to no state.
        CC0A_UserData.Instance.CurrentGunState = GunState.NotShooting;
    }

    //On Update
    void Update()
    {
        // 监视 当 射击状态变化时，输出debug，表达当前进入了什么状态
        // Monitor when the shooting state changes, output debug information to indicate the current state entered.
        CC0A_UserData.Instance.PV001_CheckEnumChange<GunState>
            (ref CC0A_UserData.Instance.CurrentGunState, ref CC0A_UserData.Instance.LastGunState, ref CC0A_UserData.Instance.GunStateString);

        // 处理不同 GunState 射击状态 包括转换值 播放动画等
        // Handle different `GunState` shooting states, including transitioning values, playing animations, etc.
        CC1_V000_GunStateActions();

        // 监视 玩家是否死亡 也就是我，Player
        // Monitor whether the player (i.e., me) is dead.
        CC1_V005_CheckPlayerDead();

        // 根据鼠标输入 切换状态 
        // Switch states based on mouse input.
        CC1_V001A_HandleGunInput(); //涉及到物理，必须放在FixedUpdate里面才行
    }

    //On FixedUpdate
    void FixedUpdate()
    {
        // 根据鼠标输入 切换状态 
        // Switch states based on mouse input.
        //CC1_V001A_HandleGunInput(); //涉及到物理，必须放在FixedUpdate里面才行
    }

    /// <第三部分：主要功能区> //Part 3: Main Functionality Area
    /// Part003: 里面放了各种void 方法
    /// Part003: It contains various `void` methods.

    //【V000】处理不同GunState的enum 分别需要触发什么？
    // [V000] Handle what needs to be triggered for different `GunState` enums.
    void CC1_V000_GunStateActions()
    {
        switch (CC0A_UserData.Instance.CurrentGunState)
        {
            case CC0A_UserData.GunState.NotShooting:
                //动画 //Animation
                CC0A_UserData.Instance.PV008_ResetAnimations(GunAnimator, false, "Fire", "FireLoop");//重置所有动画 //Reset All Animations
                break;

            case CC0A_UserData.GunState.ClickShoot:
                //修改推力数值 //Change the Push Value
                CC0A_UserData.Instance.CurrentPushPower = CC0A_UserData.Instance.RayPushPower;//力量比较大的
                //动画 //Animation
                CC0A_UserData.Instance.PV009_PlayAAnimation(GunAnimator, true, "Fire");
                break;

            case CC0A_UserData.GunState.LongShoot:
                //修改推力数值 //Change the Push Value
                CC0A_UserData.Instance.CurrentPushPower = CC0A_UserData.Instance.RayPushPowerLong;//力量比较小的
                //动画 //Animation
                CC0A_UserData.Instance.PV009_PlayAAnimation(GunAnimator, true, "FireLoop");
                break;
        }
    }

    // 【CC1_V001A】处理 枪械射击 逻辑 中的 鼠标输入 并调用射线释放
    // [CC1_V001A] Handle mouse input in the firearm shooting logic and trigger the raycast.
    void CC1_V001A_HandleGunInput()
    {
        //这个是鼠标左键点击事件 //This is the left mouse button click event
        if (Input.GetMouseButtonDown(0) && CC0A_UserData.Instance.BulletAccount > 0)//当鼠标输入 且 子弹存在时 //When there is mouse input and bullets are
                                                                                    //available
        {
            //切换为短按状态 Switch to short press state
            CC0A_UserData.Instance.CurrentGunState = GunState.ClickShoot;
            //执行射击！ Execute the shooting!
            CC1_V002_ShootRay();//只会执行1次， It will only be executed once.
        }

        //这个是鼠标左键按住事件 //This is the left mouse button press （hold）event
        else if (Input.GetMouseButton(0) && CC0A_UserData.Instance.BulletAccount > 0) //当鼠标长按 且 子弹存在时 //When the mouse is long pressed and the bullet
                                                                                      //exists 
        {
            if (Time.time - currentShootTime > CC0A_UserData.Instance.GunShootCD) //当已经超过了CD时间 才能执行长按逻辑
                                                                                  //When the CD time has exceeded, the long press logic can be executed                                                                                                                    //When the cooldown time has elapsed and bullets are available
            {
                CC0A_UserData.Instance.CurrentGunState = GunState.LongShoot;
                //执行射击！ Execute the shooting!
                CC1_V002_ShootRay();//可以持续执行 每一次大于CD值 都会执行 //可以持续执行 每一次大于CD值 都会执行
            }
        }
        else if (Input.GetMouseButtonUp(0))//鼠标抬起时 //When the mouse is released
        {
            //切换为没有任何状态  //Switch to no state.
            CC0A_UserData.Instance.CurrentGunState = GunState.NotShooting;
        }
        else //如果还有其他所有状态 那就都是没有状态 All other states are no state
        {
            //切换为没有任何状态  //Switch to no state.
            CC0A_UserData.Instance.CurrentGunState = GunState.NotShooting;
        }
    }


    //【V002】处理实际上的射线释放 + 射击音效 并调用判断Tag 也即是真是的射击逻辑
    // [V002] Handle the actual raycasting and shooting sound effects, and check the tag.
    void CC1_V002_ShootRay()
    {
        if (!CC0A_UserData.Instance.isGamePaused)//当游戏全局被暂停的时候，不做任何处理 //Do nothing when the game is globally paused. 且没有射击
        {
            // 不然的话，就开始射击：执行下面的一系列语句:
            // Otherwise, start shooting: execute the following sequence of statements:

            // 更新子弹时间 //Update bullet time.
            CC0A_UserData.Instance.PV003_UpdateTimeTime(ref currentShootTime);

            // 减少子弹 // Decrease the bullet count
            // Debug.Log("子弹减少了1颗呢"); //One bullet has been reduced.
            CC0A_UserData.Instance.BulletAccount--;
            Debug.Log($"CC1_V002_ShootRay 显示：当前的子弹数量{CC0A_UserData.Instance.BulletAccount}");

            // 播放射击音效 // Play shooting sound effect
            CC0A_UserData.Instance.PV002_PlayAudioSource(AS_GunShoot);

            // 从屏幕中心创建一个射线 // Create a ray from the center of the screen 进行实际上的射线监测 Conducting actual radiation monitoring
            ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

            bool hitTarget = false; // 用于标记 是否 集中了有效的物体 // Used to mark whether a valid object has been hit

            for (int i = 0; i < maxRaycastIterations; i++)//给循环上限制，避免无限循环的情况 Limit the loop to avoid infinite loops
            {
                if (Physics.Raycast(ray, out hit))
                {
                    // 检查击中物体的标签 是否需要忽略 
                    // Check if the tag of the object being hit needs to be ignored
                    if (hit.transform.CompareTag("MXXS_PickUpItem"))
                    {
                        // 如果确实是需要忽略的，那么就进行偏移
                        // If it really needs to be ignored, then offset it
                        ray.origin = hit.point + ray.direction * 0.01f;
                        continue;
                    }

                    // 有效命中处理 Effective hit processing
                    hitTarget = true;
                    Debug.Log($"CC1_V001Add1显示：射线击中了物体: {hit.transform.name}\n" +
                      "MXXS结束\n");

                    // 调用逻辑：处理命中物体的Tag Calling logic: Processing the tag of the hit object
                    CC1_V003_HandleHitTags(hit);
                    break;
                }
            }

            // 如果没有击中有效物体，输出空弹的调试信息 //If no valid object is hit, output debug information for an empty shot.
            if (!hitTarget)
            {
                Debug.Log("CC1_V001Add1显示：射线没有击中任何物体，是空弹哦~杂鱼！\n" +
                      "MXXS结束\n");//The ray didn't hit any objects—it's an empty shot, just a stray!
            }
        }
        else
        {
            Debug.Log("CC1_V002 游戏已经暂停了，当然是不可以射击的啦！\n" +
                      "MXXS结束\n"); //Out of bullets!
        }
    }


    // 【CC1_V003】处理射线击中物体 根据不同的tag，进行不同的处理
    //  [CC1_V003] Handle the raycast hit objects and perform different actions based on the tag.
    void CC1_V003_HandleHitTags(RaycastHit hit)
    {
        switch (hit.transform.tag)
        {
            case "MXXS_Enemy":
                CC1_V004_Tag1_HandleEnemyHit(hit);
                break;
            case "MXXS_Physics":
                CC1_V004_Tag2_HandlePhysicsHit(hit);
                break;
            default:
                Debug.Log("CC1_V003_HandleHitTags 里面 击中了一些不需要处理的tag啦！\n" +
                      "MXXS结束\n");
                break;
        }
    }

    // 【CC1_V004_Tag1】处理击中敌人的逻辑 如果Tag为Enemy
    //  [CC1_V004_Tag1] Handle the logic for hitting an enemy if the tag is `Enemy`.
    void CC1_V004_Tag1_HandleEnemyHit(RaycastHit hit)
    {

        //播放爆炸 击中生物体 音效 //Play the explosion sound effect for hitting biological targets.
        //为了艺术效果，我是同时播放2个音效的，为了声音更加好听 //To enhance the artistic effect, I play two sound effects simultaneously to make the sound
        //more pleasing.
        CC0A_UserData.Instance.PV002_PlayAudioSource(AS_BioHit1);
        CC0A_UserData.Instance.PV002_PlayAudioSource(AS_BioHit2);

        //视图调用到CC2_Enemy 附着在敌人身上的代码块 //Call the code block attached to the enemy in `CC2_Enemy`.
        CC2A_EnemyLogic cC2_EnemyLogic;
        cC2_EnemyLogic = hit.transform.GetComponent<CC2A_EnemyLogic>();

        if (cC2_EnemyLogic != null)//如果代码块存在 则 //If the code block exists, then
        {
            //执行减血 //Execute health reduction.
            cC2_EnemyLogic.MonsterBlood -= CC0A_UserData.Instance.GunHitPower;

            //让怪物开始追逐我
            cC2_EnemyLogic.ShouldChasePlayer = true;

            // 输出怪物当前的血量
            // Debug.Log("怪物当前的血量：" + cC2_EnemyLogic.MonsterBlood);

            //当怪物的血=0的时候 //When the monster's health is 0
            if (cC2_EnemyLogic.MonsterBlood <= 0)
            {
                //destroy 怪物 //Destroy the monster.
                Destroy(hit.transform.gameObject);

                //播放爆炸 怪物死亡 音效 //Play the explosion sound effect for monster death.
                CC0A_UserData.Instance.PV002_PlayAudioSource(AS_Explode);

                CC0A_UserData.Instance.PV004_SetaPrefab(CC0A_UserData.Instance.HitExplodePrefab, hit.transform.position);

                if (!CC0A_UserData.Instance.isPlayerWin) //当角色还没有胜利的时候 允许玩家胜利 不然允许启动携程 
                                                         // If the character has not yet won, allow the player to win; otherwise, allow starting the coroutine.
                {
                    // 启动协程 // Start the coroutine
                    StartCoroutine(CC1_V006_IE_CheckPlayerWin()); //这个用来监视 玩家有没有击败敌人，击败了的话，那就是赢了游戏
                                                                  //This is used to monitor whether the player has defeated the enemy. If defeated, it means
                                                                  //winning the game.
                }
            }
        }
        else
        {
            Debug.Log("【MXXS的提醒】：Enemy对象身上没有查询到 CC2_EnemyLogic 的代码块"); 
            //[MXXS Reminder]: The `Enemy` object does not have the `CC2_EnemyLogic` code block attached.
        }
    }


    // 【CC1_V004_Tag2】处理击中物理对象的逻辑 如果Tag为Physics
    //  [CC1_V004_Tag2] Handle the logic for hitting physical objects if the tag is `Physics`.
    void CC1_V004_Tag2_HandlePhysicsHit(RaycastHit hit)
    {
        // 调用到CC2_Physic 附着在敌人身上的Rigidbody组件
        // Call the `Rigidbody` component attached to the enemy in `CC2_Physic`.
        Rigidbody rb = hit.transform.GetComponent<Rigidbody>();

        if (rb != null && !rb.isKinematic) //如果组件存在，且 可以移动 //If the component exists and is movable
        {
            //播放 打击到金属体 音效 // Play the sound effect for hitting metal objects
            CC0A_UserData.Instance.PV002_PlayAudioSource(AS_MetalHit);

            //施加推力 // Apply force
            Vector3 forceDirection = (hit.point - Camera.main.transform.position).normalized;//计算位置 并且normalize 
            // Calculate the position and normalize it
            rb.AddForce(forceDirection * CC0A_UserData.Instance.CurrentPushPower, ForceMode.Impulse); //CurrentPushPower 会根据状态变化 
            // `CurrentPushPower` will change based on the state.
        }
        else
        {
            Debug.Log("【MXXS的提醒】：Physics对象身上没有查询到 RigidBody 的组件"); //[MXXS Reminder]: The Physics object does not have the
                                                                   //`Rigidbody` component attached.
        }

        //调用到CC4_PhysicsLogic 附着在物理体身上的代码块 // Call the code block attached to the physical object in `CC4_PhysicsLogic`.
        CC2B_PhysicsLogic cC2B_PhysicsLogic;
        cC2B_PhysicsLogic = hit.transform.GetComponent<CC2B_PhysicsLogic>();

        if (cC2B_PhysicsLogic != null)//如果代码块存在 则 //If the code block exists, then
        {
            //执行减血 // Execute health reduction
            cC2B_PhysicsLogic.PhysicBlood -= 1;
            Debug.Log("物理体当前的血量：" + cC2B_PhysicsLogic.PhysicBlood);

            //当物理体的血=0的时候 // When the physical object's health is 0
            if (cC2B_PhysicsLogic.PhysicBlood <= 0)
            {
                //destroy 当前Hit // Destroy the current hit object
                Destroy(hit.transform.gameObject);

                //生成爆炸体 // Instantiate the explosion prefab
                CC0A_UserData.Instance.PV004_SetaPrefab(CC0A_UserData.Instance.HitExplodePrefabSmall, hit.transform.position);

                //随机生成Package // Randomly generate a package
                float randomValue = UnityEngine.Random.value; // 获取 [0, 1) 区间的随机值 // Get a random value in the range [0, 1)

                if (randomValue <= CC0A_UserData.Instance.GainPackRate)//如果概率落入区间 // If the probability falls within the range
                {
                    float ammoRandomValue = UnityEngine.Random.value; // 再获取 [0, 1) 区间的随机值 这个值讲代表生成那个物体 
                    // Get a random value in the range [0, 1)
                    if (ammoRandomValue <= CC0A_UserData.Instance.GainAmmoPackRate) // 如果落入AmmoPack概率区间 
                        // If it falls within the AmmoPack probability range
                    {
                        // 生成AmmoPack
                        Instantiate(CC0A_UserData.Instance.AmmoPackPrefab, hit.transform.position, Quaternion.identity);
                    }
                    else // 否则生成替换物件，也就是HP Package // Otherwise, instantiate a replacement object
                    {
                        Instantiate(CC0A_UserData.Instance.HPPackPrefab, hit.transform.position, Quaternion.identity);
                    }
                } // 没有落入区间 则不执行 If it does not fall within the interval, it will not be executed.
            } // 血 不为零 则 不执行 If blood is not zero, it will not be executed.
        }
        else
        {
            Debug.Log("【MXXS的提醒】：Physics对象身上没有查询到 CC4_PhysicsLogic 的代码块");
        }
    }


    //【V005】角色 玩家 死亡逻辑 
    // [V005] The Death logic
    void CC1_V005_CheckPlayerDead() //这是一个 监视 逻辑，是调用到Update里面的 This is a monitoring logic, which is called into Update
    {
        if (CC0A_UserData.Instance.PlayerBlood <= 0 && !CC0A_UserData.Instance.isPlayerDead) //玩家没有死的时候 且 没有血了 的时候 才可以死 
                                                                                             //The player can die only when he is not dead and has no blood
        {
            if (CC0A_UserData.Instance.DeadUI != null)//如果UI存在 //If GameObject Exist
            {
                Debug.Log("已调用CC1_V004_V003_CheckPlayerDead逻辑");

                //调用死亡UI //Call the Death UI
                CC0A_UserData.Instance.PV005_ShowPrefabUI(CC0A_UserData.Instance.DeadUI, ref CC0A_UserData.Instance.InstanceDeadUI);
            }
            else
            {
                Debug.Log("【MXXS的提醒】：缺少一个死亡的UI哦"); //[MXXS Reminder]: A death UI is missing.
            }

            //玩家 死了 //Player Dead
            CC0A_UserData.Instance.isPlayerDead = true;

            // 播放死亡音效 //Play DEATH sound effects
            CC0A_UserData.Instance.PV002_PlayAudioSource(CC0A_UserData.Instance.AS_DieScream);

            //游戏 暂停 //Game Pause
            CC0A_UserData.Instance.isGamePaused = true;
        }
    }

    //【V006】角色 玩家 胜利逻辑 游戏胜利以后 该干什么干什么
    // [V006] Victory logic// Things happened after game Wins
    IEnumerator CC1_V006_IE_CheckPlayerWin() //胜利逻辑 Victory Logic
    {
        if (!CC0A_UserData.Instance.isPlayerWin)//没有胜利才可以胜利 There is no victory to win
        {
            Debug.Log("已调用CC1_V004_CheckPlayerWin逻辑");

            // 等待 1 秒 //wait for 1 second
            yield return new WaitForSeconds(1f);

            // 等待 1 秒，期间检查游戏状态是否发生变化 一个防御性的代码 //Wait 1 second, during which time check if the game state has changed A defensive code
            float waitTime = 1f;

            while (waitTime > 0f)
            {
                if (CC0A_UserData.Instance.isPlayerDead) //添加死亡状态检查，防止在等待期间玩家死亡 //Added death state check to prevent player death during
                                                         //waiting period
                {
                    Debug.LogWarning("在胜利检查过程中，玩家死亡，终止胜利逻辑。");  //During the victory check, the player dies, terminating the victory logic.
                    yield break;
                }

                waitTime -= Time.deltaTime;
                yield return null;
            }

            if (CC0A_UserData.Instance.WinUI != null) //如果UI存在 //If GameObject Exist
            {
                //调用胜利UI //Call the Win UI
                CC0A_UserData.Instance.PV005_ShowPrefabUI(CC0A_UserData.Instance.WinUI, ref CC0A_UserData.Instance.InstanceWinUI);
            }
            else
            {
                Debug.Log("【MXXS的提醒】：缺少一个胜利的UI哦"); //[MXXS Reminder]: A victory UI is missing.
            }

            //玩家 赢了 //Player win the Game!
            CC0A_UserData.Instance.isPlayerWin = true;
            //游戏 暂停 //Game Pause
            CC0A_UserData.Instance.isGamePaused = true;
        }
    }
}