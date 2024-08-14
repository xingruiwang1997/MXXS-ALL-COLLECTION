using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CC0B_AcrossSceneUI : MonoBehaviour
{
    /// <第零部分：我的数据> //<Part 0: My Data>
    /// Part000: 我所有需要的数据
    /// Part000: All the data I need

    // 这个决定了我摁下ESC的时候，删除什么物体? 删的是哪一个UI GameObject Prefab？
    // This determines what object is deleted when I press ESC?
    // ESC堆叠 这个决定了我摁下ESC的时候，删除什么物体?
    // ESC Stack This determines what object is deleted when I press ESC?
    private Stack<GameObject> ESCStack = new Stack<GameObject>();

    // 点击按钮的音效 (因为是有关场景跳转的逻辑，所以我放在CC0B里面)
    // Button click sound effect (Because it is about the logic of scene jump, I put it in CC0B)
    [Header("AudioSource: 请手动拖入 UI的点击音效")] //AudioSource: Please manually drag in the UI click sound effects
    [SerializeField] private AudioSource CC0B_AS_ClickUI;


    /// <第一部分：单例化> //Part 1: Singleton
    /// Part001: CC0B 不需要 写 DontDestroy 因为它是和CC0A 放在一起的，唯一的区别就是这个是处理
    /// Part001: CC0B does not need to write DontDestroy because it is placed together with CC0A. The only difference is that this one handles

    // 静态变量来 存储 单例实例
    // Static variables are used to store singleton instances.
    private static CC0B_AcrossSceneUI _instance;

    // 公共静态属性来 访问 单例实例
    // Public static properties are used to access singleton instances.
    public static CC0B_AcrossSceneUI Instance
    {
        get
        {
            return _instance; //return private
        }
    }


    /// <第二部分：Awake Start Update FixedUpdate 里面分别需要什么逻辑> 
    /// Part002: 开局做什么 更新做什么 //What to do at the beginning What to do at the update
    /// Part002: What logic is needed in `Awake`, `Start`, `Update`, and `FixedUpdate`.

    //On Awake
    void Awake()
    {
        if (_instance == null)
        {
            // 赋值为自己
            // Assign to this self
            _instance = this;
            Debug.Log("CC0B_AcrossSceneUI 的 Awake 程序已经启动\n" +
                      "CC0B_AcrossSceneUI 的 _instance已经被创建\n" +
                      "DEBUG编号CC0B_AcrossSceneUI_Awake_001\n" +
                      "MXXS结束\n");

            // 初始绑定按钮 S0代表的是场景0
            // Initial binding button
            BindButtonsS0();

            // 添加场景事件 这个是CC0B里面特有的东西
            // Adding scene events
            SceneManager.sceneLoaded += CC0B_VA_OnSceneLoaded;

            // 如果实例化成功后，不再进行进一步的检查，直接返回
            // If the instantiation is successful, no further checks are performed and the program returns directly.
            return;
        }

        else if (_instance != this)
        {
            // 如果重复生成instance 则销毁新生成的重复的物体
            // If the instance is generated repeatedly, the newly generated duplicate object is destroyed
            Destroy(gameObject);
            Debug.Log("CC0B_AcrossSceneUI 的 Awake 程序已经启动\n" +
                      "_instance因为重复生成了，所以销毁新的那个\n" +
                       "DEBUG编号CC0B_AcrossSceneUI_Awake_002\n" +
                      "MXXS结束\n");

            return;
        }
    }

    //On Update
    void Update()
    {
        // UpDate001 游戏继续的时候 When the game continues （这个是ESC的逻辑）
        // UpDate001 MXXS 监测 是否在游戏继续时 摁下ESC键？ Detect if the ESC key is pressed while the game is continuing?
        if (!CC0A_UserData.Instance.isGamePaused)//不是继续 就是停止
        {
            CC0B_V001_EscOpenUI(); //游戏继续的话，那肯定是调用UI If the game continues, then it must call the UI
        }
        else // MXXS 监测是否在游戏暂停时 摁下ESC键？  游戏暂停的时候，那只会退出
             // Detect if the ESC key is pressed when the game is paused? When the game is paused, it will only exit
        {
            CC0B_V006_EscCloseUI();
        }

        // ******特殊情况 //Special circumstances
        // UpDate002 检查当前场景编号 如果为0，不管怎么样都暂停游戏 因为S0 里面只有UI
        // UpDate002 Check the current scene number. If it is 0, pause the game anyway because S0 only has UI.
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            // 如果场景编号为0，则将游戏暂停 //If the scene number is 0, the game is paused
            if (!CC0A_UserData.Instance.isGamePaused)
            {
                CC0A_UserData.Instance.isGamePaused = true;//让游戏暂停 //Pause the game 为了调用鼠标， To call the mouse,
            }
        }
    }

    // 取消事件引用 //Unreference an event
    // 只有当这个DontDestroy被销毁的时候，才会使用到呀！ //It will only be used when this DontDestroy is destroyed!
    private void OnDestroy()
    {
        if (_instance == this)
        {
            SceneManager.sceneLoaded -= CC0B_VA_OnSceneLoaded;
        }
    }


    /// <第三部分：主要功能区> //Part 3: Main Functionality Area
    /// Part003: 里面放了各种void 方法
    /// Part003: It contains various `void` methods.

    // 【VA】OnSceneLoaded 当场景加载的时候 绑定什么逻辑？
    // 【VA】OnSceneLoaded When the scene is added
    void CC0B_VA_OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 防止出Bug 删除所有的UI Prefab 的 Instance //为了给重新开始游戏用的清理逻辑
        // To prevent bugs, delete all UI Prefab instances
        CC0A_UserData.Instance.PV006_HidePrefabUI(CC0A_UserData.Instance.DeadUI, ref CC0A_UserData.Instance.InstanceDeadUI);
        CC0A_UserData.Instance.PV006_HidePrefabUI(CC0A_UserData.Instance.WinUI, ref CC0A_UserData.Instance.InstanceWinUI);
        CC0A_UserData.Instance.PV006_HidePrefabUI(CC0A_UserData.Instance.PauseUI, ref CC0A_UserData.Instance.InstancePauseUI);
        CC0A_UserData.Instance.PV006_HidePrefabUI(CC0A_UserData.Instance.GuideUI, ref CC0A_UserData.Instance.InstanceGuideUI);

        switch (scene.buildIndex) //根据不同场景绑定不同事件 //Bind different events according to different scenarios
        {
            case 0:
                BindButtonsS0();

                // 让游戏暂停 Game Stop
                CC0A_UserData.Instance.isGamePaused = true;

                break;
            case 1:
                BindButtonsS1();//本身是没有用的，但是先放在这里，万一以后可以用得到 //It is useless by itself, but I put it here in case it can be used later.

                // 让游戏继续 Keep the Game On
                CC0A_UserData.Instance.isGamePaused = false;

                // 复位一切，让游戏重新开始！ Reset everything and start the game over again!

                // 复位玩家的状态 Reset the game state
                CC0A_UserData.Instance.isPlayerDead = false;
                CC0A_UserData.Instance.isPlayerWin = false;

                //复位血包和弹药包的初始值 Reset the initial values ​​of health packs and ammo packs
                CC0A_UserData.Instance.AmmoPack = 3;
                CC0A_UserData.Instance.HPPack = 3;

                // 销毁一切客栈堆叠 Destroy all inn stacks
                ESCStack.Clear();  // Replacing null assignment with Clear() for better practice 

                break;
            default:
                //Debug.Log("调用到了一个不存在的场景咩");
                break;
        }
    }

    // 绑定各种按钮的逻辑
    // Binding logic of various buttons
    public void BindButtonsS0()
    {
        // 使用 FindObjectsByType 并指定排序模式为 None
        // Use FindObjectsByType and specify the sort mode as None
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            if (button.gameObject.name == "BTN_S0toS1")
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(VBTN_S0toS1);
            }
            else if (button.gameObject.name == "BTN_OpenGuide")
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(CC0B_V003_OpenGuide);
            }
        }
    }

    public void BindButtonsS1()
    {
        // 使用 FindObjectsByType 并指定排序模式为 None
        // Use FindObjectsByType and specify the sort mode as None
        Button[] buttons = FindObjectsByType<Button>(FindObjectsSortMode.None);

        foreach (Button button in buttons)
        {
            if (button.gameObject.name == "BTN_CloseUI")
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(CC0B_V004_CloseUI);
            }
            else if (button.gameObject.name == "BTN_S1toS0")
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(VBTN_S1toS0);
            }
            else if (button.gameObject.name == "BTN_OpenGuide")
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(CC0B_V003_OpenGuide);
            }
        }
    }

    // 场景切换的逻辑：
    // The logic of scene switching:
    private void VBTN_S0toS1() //进入场景1的时候 When entering scene 1
    {
        // 播放按钮音效 Play sound effects
        CC0A_UserData.Instance.PV002_PlayAudioSource(CC0B_AS_ClickUI);

        // 切换场景 Switch scenes
        SceneManager.LoadScene(1);
    }

    private void VBTN_S1toS0() //进入场景0的时候 When entering scene 0
    {
        // 播放音效 Play sound effects
        CC0A_UserData.Instance.PV002_PlayAudioSource(CC0B_AS_ClickUI);

        // 切换场景 Switch scenes
        SceneManager.LoadScene(0);
    }

    /// <第四部分：有关ESC按钮的逻辑> <Part 4: Logic about the ESC button>
    /// Part004: ESC关闭UI是 一个额外的逻辑 这样子把方法封装起来，可以让按钮Button 和 ESC 可以调用到同一个方法，实现同样的功能
    /// Part004: ESC closes the UI. This is an additional logic. By encapsulating the method, the Button and ESC can 
    /// call the same method to achieve the same function.


    // V001 第一步：当游戏运行的时候，我会摁下ESC键，来调取出Pause UI 并且暂停游戏
    // V001 Step 1: When the game is running, I will press the ESC key to bring up the Pause UI and pause the game
    void CC0B_V001_EscOpenUI()//只有当游戏继续 的时候才会被调用 //It will only be called when the game continues
    {
        if (Input.GetKeyDown(KeyCode.Escape))//第一层：当我摁下Esc的时候 且 游戏继续的时候//First level: When I press Esc and the game continues
        {
            Debug.Log("已调用：CC0B_V001_EscOpenUI\n" +
              "【游戏处于运行状态，我摁下了ESC键，暂停游戏 并且打开PauseUI方法】\n\n" +
              "MXXS结束\n");

            //打开专门的PauseUI Open the dedicated PauseUI
            CC0B_V002_Scene001_ESCCallPauseUI();
        }
    }

    // V002 第二步：唤醒PauseUI 并且暂停游戏 的方法，目前只能被ESC键 单独调用
    // V002 Step 2: The method to wake up PauseUI and pause the game can currently only be called by the ESC key alone
    void CC0B_V002_Scene001_ESCCallPauseUI()
    {
        //生成暂停UI Generate Pause UI
        if (CC0A_UserData.Instance.PauseUI != null) //当我已经填入PauseUI的时候 手动填入的，防止漏掉 
                                                    //When I have already filled in PauseUI, I filled it in manually to prevent missing it
        {
            Debug.Log("已调用：CC0B_V002_Scene001_CallPauseUI方法\n" +
                      "说明CC0A_UserData.Instance.PauseUI存在（已经被我填入）\n" +
                      "接下来应该会调用CC0B_V002_SetESCObject\n" +
                      "MXXS结束\n");

            //生成UI Generate UI
            CC0A_UserData.Instance.PV005_ShowPrefabUI(CC0A_UserData.Instance.PauseUI, ref CC0A_UserData.Instance.InstancePauseUI);

            //UI 堆叠到客栈 Pause UI stack to inn Pause
            CC0B_V005_SetESCObject(CC0A_UserData.Instance.InstancePauseUI);
        }
        else
        {
            Debug.Log("CC0B_V005_Scene001_CallPauseUI里面\n" +
                      "发现没有填入需要的PauseUI的Prefab哦~\n" +
                      "MXXS结束\n");
        }
    }

    // V003 第三步：我会通过Button打开OpenGuide 界面
    // V003 Step 3: I will open the OpenGuide interface through the Button
    public void CC0B_V003_OpenGuide()
    {
        Debug.Log("已调用：CC0B_V003_OpenGuide逻辑\n" +
                  "当前逻辑开始，正在打开GuideUI\n" +
                  "MXXS结束\n");

        //播放音乐 Play Music
        CC0A_UserData.Instance.PV002_PlayAudioSource(CC0B_AS_ClickUI);

        //调用Guide UI //Calling Guide UI
        CC0A_UserData.Instance.PV005_ShowPrefabUI(CC0A_UserData.Instance.GuideUI, ref CC0A_UserData.Instance.InstanceGuideUI);

        if (CC0A_UserData.Instance.InstanceGuideUI != null)//防止报错 Preventing errors
        {
            //UI 堆叠到客栈 Guide //UI Stack to Inn Guide
            CC0B_V005_SetESCObject(CC0A_UserData.Instance.InstanceGuideUI);
        }
        else
        {
            Debug.Log("【喵小小酥的提醒】：CC0B_V003 里的 void VBTN_OpenGuide 里的 CC0A_UserData.Instance.InstanceGuideUI 它的值等于Null啊");
        }
    }

    // V004 第四步：然后我会关闭当前UI
    // V004 Step 4: Then I will close the current UI
    void CC0B_V004_CloseUI() //因为同时要被ESC 和 Button的listener调用 Because it is called by the listeners of ESC and Button at the same time
    {
        Debug.Log("已调用：CC0B_V004_CloseUI逻辑\n" +
          "当前逻辑开始\n" +
          "MXXS结束\n");

        // 防御性编程：检查 ESCStack 是否为 null
        // Defensive Programming: Check if ESCStack is null
        if (ESCStack == null || ESCStack.Count == 0)//说明我根本就没有打开的UI This means I didn't open the UI at all.
        {
            Debug.Log("CC0B_V004_CloseUI 里面检查到 并不存在什么ESCStack 所以跳过逻辑了喵\n" +
                      "ESCStack为空或已为空，没有什么好关掉的啦！！\n" +

                       "DEBUG编号CC0B_V004_001\n" +
                       "MXXS结束\n");

            return;//跳出整个CC0B_V004_CloseUI的方法哦 //exit the entire CC0B_V004_CloseUI
        }

        try //ESCStack.Count>0的情况 //When ESCStack.Count>0
        {
            // 销毁堆栈顶端的ESCObject //Destroy the ESCObject at the top of the stack
            Destroy(ESCStack.Pop());
            Debug.Log("CC0B_V004_CloseUI 销毁了堆栈顶端的 ESCObject 哦\n" +

                      "DEBUG编号CC0B_V004_002\n" +
                      "MXXS结束\n");

            // 播放UI点击音效 关闭UI的音效 //Play UI click sound effects Turn off UI sound effects
            CC0A_UserData.Instance.PV002_PlayAudioSource(CC0B_AS_ClickUI);
        }

        catch (Exception ex)
        {
            Debug.Log($"CC0B_V004_CloseUI进入了catch程序哦: {ex.Message}\n" +

                      "DEBUG编号CC0B_V004_003\n" +
                      "MXXS结束\n");
        }

        if (ESCStack.Count == 0)//说明我的UI全部都删除干净了 This means that all my UI has been deleted.
        {
            // 这个时候说明没有UI了 那么游戏就可以继续了 This means there is no UI, so the game can continue.
            CC0A_UserData.Instance.isGamePaused = false;

            Debug.Log("CC0B_V004_CloseUI进里面发现ESCStack==0了，UI都被删除干净了\n" +
                      "游戏继续咯！" +

                      "DEBUG编号CC0B_V004_004\n" +
                      "MXXS结束\n");
        }
    }

    // V005 第五步：堆叠UI的方法
    // V005 这个用来给各种按钮 堆叠UI 使用
    // V005 is used for stacking UI for various buttons
    public void CC0B_V005_SetESCObject(GameObject newUI)
    {
        Debug.Log("已调用：CC0B_V005_SetESCObject(GameObject newUI)逻辑\n" +
                  "当前逻辑开始\n" +
                  "MXXS结束\n");

        // 防御性编程：检查 ESCStack 是否为 null
        // Defensive Programming: Check if ESCStack is null
        if (ESCStack == null) //如果是Null的话，就生成新的Stack
        {
            ESCStack = new Stack<GameObject>();
        }

        // 防御性编程：检查 newUI 是否为 null
        // Defensive Programming: Check if newUI is null
        if (newUI == null)//当物体为空的时候
        {
            return; // 直接返回，避免后续操作 //Return directly to avoid subsequent operations
        }

        // 将当前ESC物体 堆叠到顶部
        // Stack the current ESC object to the top
        ESCStack.Push(newUI);

        Debug.Log("CC0B_V005_SetESCObject语句：\n" +

                  $"【已经把{newUI}堆叠至 ESCStack 数组里面了】\n" +
                  $"【现在ESCStack的值为{ESCStack}】\n" +

                  $"【当前的ESCStack.length的长度为{ESCStack.Count}】\n" +

                  "DEBUG编号CC0B_V005_001\n" +
                  "MXXS结束\n");

        // 并且让游戏暂停 因为UI的时候，必然游戏是暂停的
        // And pause the game because when the UI is on, the game must be paused
        CC0A_UserData.Instance.isGamePaused = true;

        // 播放音效 Play sound effects
        CC0A_UserData.Instance.PV002_PlayAudioSource(CC0B_AS_ClickUI);
    }

    // V006 摁住ESC键 或 Listener 调用同一个东西 让ESC也可以调用 关闭UI的逻辑 和button达成同样的效果
    // V006 Press the ESC key or Listener to call the same thing
    void CC0B_V006_EscCloseUI() //只会在游戏暂停的时候被调用 It will only be called when the game is paused.
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("CC0B_V006_EscCloseUI语句已经被调用：\n" +
                      "DEBUG编号CC0B_V006_001\n" +
                      "游戏处于暂停状态，我摁下了ESC键，进入CloseUI程序\n\n" +
                      "MXXS结束\n");

            //关闭当前UI Close the current UI
            CC0B_V004_CloseUI();
        }
    }
}