using System.Collections.Generic;
using System;
using UnityEngine;

public class CC0A_UserData : MonoBehaviour
{
    /// <第一部分：单例化>
    /// Part001: 实例 CC0_UserData: 确保这个实例唯一存在，并且在整个游戏过程中不会被销毁。
    /// Part001: Instance CC0_UserData: to make sure this exist only 1, and won't be destroy throughout the game.

    // 静态变量来 存储 单例实例
    // Static variables are used to store singleton instances.
    private static CC0A_UserData _instance;

    // 公共静态属性来 访问 单例实例
    // Public static properties are used to access singleton instances.
    public static CC0A_UserData Instance
    {
        get
        {
            return _instance; //return private
        }
    }


    /// <第二部分：Awake Start Update 里面分别需要什么逻辑> 
    /// Part002: 开局做什么 更新做什么
    /// Part002: What logic is needed in `Awake`, `Start`, `Update`.

    // On Awake:
    private void Awake()
    {
        // 赋值 枪械的状态 为 没有在射击 这是一个enum值 
        // Assign the state of the firearm to NotShooting.
        CurrentGunState = GunState.NotShooting;

        // 如果 游戏刚开始 没有实例，则将当前实例赋值给_instance
        // If the game has just started and there is no instance, assign the current instance to _instance.
        if (_instance == null)
        {
            // _instance 赋值为 自己
            // Assign _instance to self.
            _instance = this;
            Debug.Log("CC0_UserData 的 Awake 程序已经启动\n" +
                      "CC0_UserData 的_instance已经被创建\n" +
                      "DEBUG编号CC0_UserData_Awake_001\n" +
                      "MXXS结束\n");

            // 确保对象在场景切换时不会被销毁
            // Ensure the object is not destroyed when switching scenes.
            DontDestroyOnLoad(this.gameObject);

            // 如果实例化成功后，不再进行进一步的检查，直接返回
            // If the instantiation is successful, no further checks are performed and the program returns directly.
            return;
        }

        // 如果已有实例 且 不是当前实例，销毁当前游戏对象(也就是新建的冲突的那一个)
        // If an instance already exists && it is not the current instance, destroy the current game object (i.e., the newly created conflicting
        // one).
        else if (_instance != this)
        {
            Destroy(gameObject);
            Debug.Log("CC0_UserData 的 Awake 程序已经启动\n" +
                      "_instance因为重复生成了，所以销毁新的那个\n" +
                       "DEBUG编号CC0_UserData_Awake_002\n" +
                      "MXXS结束\n");

            return;
        }
    }

    private void Update()
    {
        //时刻监测游戏的进程状态
        PV010_CheckGameState();
    }

    /// <第二部分：所有的用户数据>
    /// Part002: 用于储存所有的用户数据，使用public，以让其他的Cscript可以通过instance进行调用
    /// Part002: Used to store all user data, using public so that other scripts can call it through the Instance.

    // 自身血量
    // My health == Player Health
    [Header("float:我的当前血量（DEBUG用）")] //My Player health.
    public float PlayerBlood = 0f;//当前血量 游戏开始的时候为0 //Current health, which is 0 at the start of the game.
    [Header("float:我的血量上限")] //My Player maximum health.
    public float PlayerBloodMax = 120f;//血上限 //Player Maximum health.

    // 获取物品的概率
    // Get the probability of item drop
    [Header("float：击碎物体后，能获取到Pack的概率")] //The probability of obtaining a pack after breaking an object
    public float GainPackRate = 0.99f; //99% 因为目前没法随机生成可以击打的物理体 桶。所以概率基本上是满的  //99% probability, because currently
                                       //it's not possible to randomly
                                       //generate physical objects like barrels to hit. Therefore, the probability is effectively at its maximum.
    [Header("float：生成的物体中，是Ammopack的概率")] //The probability that the generated object is an AmmoPack.
    public float GainAmmoPackRate = 0.7f; //70% 玩家对弹药的需求量会更大 // 70% probability, as players will have a higher demand for ammunition.

    // 子弹相关
    // About Shoot Logic

    // 子弹射击时，对其他物体的RigidBody组件施加的推力：
    // The force applied to the RigidBody component of other objects when a bullet is fired:
    [Header("float:射线对其他RigidBody组件施加的当前推力（DEBUG用）")]//The current force applied to other RigidBody components by the raycast
                                                     //(for DEBUG purposes).
    public float CurrentPushPower = 5f;//长按 短按 分别都是不同的 默认为短按力度 //Long press and short press are different; the default is the
                                       //short press force.
    [Header("float:短按力度")] //Short Click
    public float RayPushPower = 30f;//短按力度更大 //Short press applies greater force.
    [Header("float:长按力度")] //Long Click (Hold the Mouse Button)
    public float RayPushPowerLong = 5f; //长按力度偏小 //Long press applies lesser force.

    [Header("float:子弹达到怪物身上的威力")]//The power of the bullet.
    public float GunHitPower = 1;//打到敌人身上掉几滴血 //The amount of blood lost when hitting an enemy:

    // 按住多少秒以后，算长按的时间:
    // The duration in seconds after which a press is considered a long press:
    [Header("float:鼠标判断为长按所需的CD时间")]//The time required for a mouse press to be considered a long press:
    public float GunShootCD = 0.18f;//经测算，符合人体工学 //Ergonomically tested and approved.

    // 子弹的数量：
    [Header("int:当前子弹的数量(DEBUG用)")]
    public int BulletAccount = 0;
    [Header("int:子弹槽的数量上限")]//The maximum number of bullets in the magazine.
    public int BulletAccountMAX = 30;//开局上限为30 //Initial is 30

    // 弹药包相关
    // Related to ammo packs
    [Header("int:当前弹药包的数量(DEBUG用)")] //The current number of ammo packs.
    public int AmmoPack = 3; //初始给3个 这样玩家初始有30+30*3=120个弹药 就算不打罐头 也足够打死敌人，还有冗余 //Initially provide 3 ammo packs,
                             //so the
                             //player starts with 30 + 30*3 = 120 bullets.
                             //Even without hitting cans, it's enough to kill enemies with some surplus.
    [Header("float:换弹药包需要的CD")] //Ammo cooldown time.
    public float AmoCD = 5f;//使用1次弹药包，经过需要5秒的CD时间 因为总是感觉子弹消耗很快 // Using an ammo pack requires a
                            //5-second cooldown because the bullets seem to be consumed quickly.
    [Header("float:弹药剩下的CD（DEBUG用）")] //Remaining cooldown time for ammo(for DEBUG purposes).
    public float AmoCDing = 0;//剩下的CD时间，需要计算得出 //The remaining cooldown time needs to be calculated.

    // 血包相关
    // Related to health packs
    [Header("int:当前血包的数量（DEBUG用）")] //The current number of health packs.
    public int HPPack = 3; //初始给3个 基本上是不会死的 弱保软完全体 //Initially provide 3 health packs; basically, the player won't die,
                           //offering a weak
                           //safeguard for the complete experience.
    [Header("float:血包需要的CD")]//Health pack cooldown time.
    public float HPCD = 10f;//使用1次血包，需要10秒的CD时间 //Using a health pack requires a 10-second cooldown time.
    [Header("float:血包剩下的CD（DEBUG用）")] //Remaining cooldown time for health packs (for DEBUG purposes).
    public float HPCDing = 0;//剩下的CD时间，需要计算得出 //The remaining cooldown time needs to be calculated.

    // Perfab相关 //Perfab related
    // Hit目标死亡后，生成的爆炸体Prefab
    // Explosion prefab generated upon the death of the hit target
    [Header("GameObject：请手动拖入射线击中物体后，物体死亡后，需要生成的爆炸体Prefab")] //Manually drag in the explosion prefab that should be
                                                             //generated
                                                             //after the object hit by the raycast dies.
    public GameObject HitExplodePrefab;
    [Header("GameObject：请手动拖入射线击中物体后，物体死亡后，需要生成的爆炸体小Prefab")] 
    public GameObject HitExplodePrefabSmall;

    // 可拾取Prefab
    // PickUp Prefab
    [Header("GameObject：填入代表可以拾取的AmmoPack的Prefab")]//Provide the prefab representing the AmmoPack that can be picked up.
    public GameObject AmmoPackPrefab;
    [Header("GameObject：填入代表可以拾取的HPPack的Prefab")]//Provide the prefab representing the HP Pack that can be picked up.
    public GameObject HPPackPrefab;

    // 死亡 和 胜利 的条件 以及UI相关的Perfab
    // Conditions for death and victory
    [Header("手动填入死亡UI的GameObject")] //Manually assign the GameObject for the death UI.
    public GameObject DeadUI;
    [Header("手动填入胜利UI的GameObject")] //Manually assign the GameObject for the Win UI.
    public GameObject WinUI;
    [Header("手动填入游戏暂停UI的GameObject")] //Manually populate the Game Pause UI GameObject
    public GameObject PauseUI;
    [Header("手动填入游戏教程UI的GameObject")] //Manually populate the Game Guide UI GameObject
    public GameObject GuideUI;

    // 音效相关
    // Sound effects related
    [Header("AS：需要手动拖入：拾取到补给品的音效")]
    public AudioSource AS_PickUpItem;
    [Header("AS：需要手动拖入：死亡嘶吼的音效")]
    public AudioSource AS_DieScream;

    // 游戏进程相关
    // Related to game progress
    [Header("bool:游戏是否停止（DEBUG用）")] //Is the game being paused?
    public bool isGamePaused = false; //主要适用于 玩家调取出UI 进行一系列操作的时候 // Mainly used when the player brings up the UI to perform
                                      //a series of operations
    bool isGamePausedLast = false;//上一次游戏的状态，用来监测游戏状态有没有发生变化 // The status of the last game, used to monitor whether the
                                  //game status has changed
    [Header("bool:玩家是否死亡（DEBUG用）")] //Is the player dead?
    public bool isPlayerDead = false; //为真 则继续死亡逻辑 比如展示死亡UI // If true, continue with death logic, such as displaying the death UI.
    [Header("bool:游戏是否胜利（DEBUG用）")] //Is the Player won the Game?
    public bool isPlayerWin = false; //为真 则继续胜利逻辑 比如展示胜利UI （和死亡的UI虽然现在看起来很像，但是未来肯定是不同的UI，所以我没有采取改
                                     //TMP_Text的方法，
                                     //这也是为了后续维护方便）
                                     // If true, continue with victory logic, such as displaying the victory UI. (Although it currently
                                     // looks similar to the death UI, they will be
                                     // different in the future,
                                     // so I did not use the same TMP_Text method. This is also to facilitate future maintenance.)

    // Enum量，用来表示当前的射击状态
    // Enum value used to represent the current shooting state.
    // 枪械的Enum量 //Enum quantity of firearms
    public enum GunState
    {
        NotShooting, // Not Shooting
        ClickShoot,//短按鼠标左键 Player click the LMB 
        LongShoot,//长按鼠标左键 Player Hold the LMB
    }
    [Header("enum:当前 射击状态（DEBUG用）")]//Current shooting state (for DEBUG purposes).
    public GunState CurrentGunState;//当前射击状态 //Current shooting state
    [Header("enum:上一步 射击状态（DEBUG用）")]//Previous shooting state (for DEBUG purposes).
    public GunState LastGunState;//上一次使用的射击状态 //Previous shooting state

    // 实例 方便删除
    [Header("不用填 Debug用")]
    public GameObject InstanceDeadUI;
    [Header("不用填 Debug用")]
    public GameObject InstanceWinUI;
    [Header("不用填 Debug用")]
    public GameObject InstancePauseUI;
    [Header("不用填 Debug用")]
    public GameObject InstanceGuideUI;

    // 状态输出相关（DEBUG用）
    // Status output related (for DEBUG purposes).
    [Header("string:玩家当前的enum状态（DEBUG用）")] //The player's current enum state (for DEBUG purposes).
    public string PlayerStateString; //因为玩家可以冲刺，冲刺也有CD时间，这里来debug显示冲刺的CD时间 //The player's current enum state
                                     //(for DEBUG purposes).
    [Header("string:射击当前的enum状态（DEBUG用）")]  //The current shooting enum state (for DEBUG purposes).
    public string GunStateString; //3种状态 短按 长按 没有射击 // Three states: short press, long press, no shooting.
    [Header("string:怪物当前的enum状态（DEBUG用）")] //The monster's current enum state (for DEBUG purposes).
    public string EnemyStateString;//3种状态 巡逻 追逐 攻击 // Three states: patrolling, chasing, attacking.


    /// <第三部分：一些需要全局通用的方法> //Part 3: Some methods that need to be globally applicable
    /// Part003: 一些需要全局通用的方法
    /// Part003: Some methods that need to be globally available

    // 通用方法001 监测一个enum的状态是否发生了变化：
    //General Method 001: Monitor whether the state of an enum has changed.
    // 当前enum值  上一次使用的enum值  需要更改的string值 
    //Current enum value, previous enum value, and the string value that needs to be changed.
    public void PV001_CheckEnumChange<ENUM>(ref ENUM currentState, ref ENUM lastState, ref string currentStateString) where ENUM : struct, Enum
    {
        if (!EqualityComparer<ENUM>.Default.Equals(currentState, lastState)) // 如果enum值不相等 那说明enum状态发生了变化 //If the enum state
                                                                             // changes
        {
            // 赋值string检测值 //Assign the string value for detection.
            currentStateString = currentState.ToString();//赋值现在的值

            // Console里面输出Debug //Output debug information in the Console.
            Debug.Log("CC0_UserData 的 PV001_CheckEnumChange<ENUM> 已经启用\n" +
                      $"当前的ENUM值{currentState}的状态变更为{currentStateString}了\n" +
                      "DEBUG编号CC0_PV001_CheckEnumChange<ENUM>_001\n" +
                      "MXXS结束\n");

            // 更新状态 //Update the state.
            lastState = currentState;
        }
    }


    // 通用方法002：播放声音 //General Method 002: Play Sound
    // 需要播放的AudioSource 以及如果需要一个条件判断 则填入对应条件
    // Specify the AudioSource to be played and, if necessary, provide the corresponding condition for playback.
    public void PV002_PlayAudioSource(AudioSource audioSource)
    {
        if (audioSource != null) //如果确实存在这一个组件 并且条件为真 //If the component exists and the condition is true
        {
            audioSource.Play();
        }
        else //确保我就算没有填入，Unity也不会报错，我自己给一个提醒就好咯！ 
             //Ensure that Unity doesn't throw an error even if nothing is provided; just give a reminder message instead.
        {
            Debug.Log("【MXXS的提醒】：CC0_UserData 的 PV002_PlayAudioSource 里面没有放入音乐哦~\n" +
                      "MXXS结束\n");
        }
    }


    // 通用方法003：更新CD时间 填入需要更新的时间float变量
    // General Method 003: Update cooldown time. Provide the float variable for the time to be updated.
    public void PV003_UpdateTimeTime(ref float CDTime)
    {
        CDTime = Time.time;
    }


    // 通用方法004：在消失点生成物体的方法 并且判断这个物体不为null 给爆炸prefab 弹药包 血包 prefab都可以使用的
    // General Method 004: Method to instantiate an object at a vanishing point and ensure the object is not null.
    public void PV004_SetaPrefab(GameObject Prefab, Vector3 position)//我的prefab物体 和我的位置 //My prefab object and my position
    {
        if (Prefab != null) //如果当前物体存在的话 //If the current object exists
        {
            Instantiate(Prefab, position, Quaternion.identity);
        }
        else
        {
            Debug.Log("【MXXS的提醒】：CC0_UserData 的 PV004_SetaPrefab 里面没有放入prefab哦~\n" +
                      "MXXS结束\n");
            //[MXXS's reminder]: There is no prefab in PV004_SetaPrefab of CC0_UserData~
        }
    }


    // 通用方法005：显示 身为Prefab状态下的UI 我的UIprefab 对应的GameObject变量
    // Generic Method 005: Display UI when in Prefab mode
    public void PV005_ShowPrefabUI(GameObject prefabui, ref GameObject InstanceUI)
    {
        if (prefabui != null)//首先我要确保我手动拖入了prefabUI //First I make sure I manually drag in the prefabUI
        {
            // 如果 UI 已经实例化，则先销毁之前的实例
            // If the UI is already instantiated, first destroy the previous instance
            if (InstanceUI != null)
            {
                Destroy(InstanceUI);
            }

            // 实例化 UI Prefab  // Instantiate the UI Prefab
            InstanceUI = Instantiate(prefabui);

            // 设置实例化的 UI 作为当前对象的子物体  // Set the instantiated UI as a child of the current object
            InstanceUI.transform.SetParent(transform, false);

            // 将该UI元素置于顶层  // Bring the UI element to the top layer 放到GameObject的最下面一层
            InstanceUI.transform.SetAsLastSibling();
            Debug.Log("CC0_UserData 的 PV005_ShowPrefabUI 已经启用\n" +
                      $"当前的{InstanceUI}已经被填入了{prefabui}哦！\n" +
                      "DEBUG编号CC0_PV005_001\n" +
                      "MXXS结束\n");

            // 调用UI的时候，顺便绑定BindButton1
            // When calling the UI, bind BindButton1
            CC0B_AcrossSceneUI.Instance.BindButtonsS1();
        }
        else
        {
            Debug.Log("【MXXS的提醒】：CC0_UserData 的  PV005_ShowPrefabUI 里面没有放入prefab UI 哦~\n" +
                      "MXXS结束\n");
        }
    }


    // 通用方法006：隐藏 身为Prefab状态下的UI
    // Generic Method 006: Hide UI in Prefab State
    public void PV006_HidePrefabUI(GameObject prefabui, ref GameObject InstanceUI)
    {
        if (InstanceUI != null)//如果Instance存在的话，则清空Instance //If the Instance exists, clear the Instance
        {
            Destroy(InstanceUI.gameObject);//存在UI的话，就删掉
            InstanceUI = null; // 清空当前 UI 实例引用 //Clear the reference to the current UI instance
        }
        else
        {
            Debug.Log($"【MXXS的提醒】：CC0_UserData 的 PV006_HidePrefabUI 里面 发现 InstanceUI 是空的呢，并不存在哦~\n" +
                      "MXXS结束\n");
        }
    }


    // 通用方法007：设置 游戏全局 是 暂停 还是 继续 的状态
    // General Method 007: The state of whether the game is paused or continued
    public void PV007_SetPauseState(bool pause) //当停止为真 //When paused is true 只填入IsGamePause这个值 如果是否 说明游戏在继续
    {
        //Debug.Log($"【！全局重要！】已调用 PV008_SetPauseState 逻辑，当前状态为{pause}");

        Time.timeScale = pause ? 0f : 1f; //为真时  全局暂停 //When true, global pause
        Cursor.lockState = pause ? CursorLockMode.None : CursorLockMode.Locked; //为真时 调用鼠标 //When true, call the mouse
        Cursor.visible = pause; //为真时 鼠标可见 //When true, the mouse is visible
    }


    // 通用方法008：当存在动画机的时候，重置所有动画bool为false 复位所有
    // General method 008: When an animation machine exists, reset all animation bools to false
    public void PV008_ResetAnimations(Animator ANI, bool value, params string[] aniNames) //一般这个bool值都为false //Generally this bool value
                                                                                          //is false
    {
        if (ANI != null)//当我的动画机确实填入的时候 //When my animator does fill in
        {
            foreach (string aniName in aniNames)
            {
                ANI.SetBool(aniName, value);
            }
        }
        else
        {
            Debug.Log($"【MXXS的提醒】：CC0_UserData 的 PV008_ResetAnimations 里面 发现 没有动画机呢喵~\n" +
                      "MXXS结束\n");
        }
    }


    // 通用方法009：播放某一个动画 也就是指定的动画
    // General method 009: Play an animation
    public void PV009_PlayAAnimation(Animator ANI, bool state, string animationName)
    {
        if (ANI != null)
        {
            //Debug.Log($"播放 {animationName} 动画");
            ANI.SetBool(animationName, state);
        }
        else
        {
            Debug.Log($"【MXXS的提醒】：CC0_UserData 的 PV009_PlayAAnimation 里面 发现 没有动画机呢喵~\n" +
                      "MXXS结束\n");
        }
    }


    // 方法010：监视 并且更新 游戏的暂停与否的状态 需要写在自己的Update里面
    // Method 010: Monitor and update the game's paused or not state. This needs to be written in your own Update
    void PV010_CheckGameState()
    {
        if (isGamePausedLast != isGamePaused)//当游戏状态变化的时候 // When the game state changes
        {
            //实施游戏状态的变化 // Implementing game state changes
            PV007_SetPauseState(isGamePaused);

            isGamePausedLast = isGamePaused;//更新游戏状态 // Update game status

            Debug.Log("CC0_UserData 的 PV010_CheckGameState 已经启用\n" +
                      $"这说明游戏状态已经变化了\n" +
                      $"当前游戏被停止了吗？{isGamePaused}\n" +
                      "MXXS结束\n");
        }
    }
}