using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CC3_UI : MonoBehaviour
{
    /// <第一部分：手动拖入所有的UI> Part 1: Manually drag in all UI
    /// Part01: CC3代码是给所有的存在于Scene1场景中的UI使用的，不包括场景跳转，以及全局UI的情况. 这些都是画面中，需要实时更新的UI
    /// Part01: CC3 code is used for all UIs existing in Scene1, excluding scene jumps and global UIs. 
    /// These are all UIs in the screen that need to be updated in real time.

    //用来显示子弹的量 Used to display the amount of bullets
    [Header("Image：请手动拖入自己的子弹槽UI_Image,有fillamount的那个")] //Image: Please manually drag your own bullet slot UI_Image, the one with fillamount
    [SerializeField] private Image bulletUIImage;

    //用来显示自己的血量 Used to display your blood volume
    [Header("Image：请手动拖入自己的血量槽UI_Image,有fillamount的那个")] //Image: Please manually drag your own health bar UI_Image, the one with fillamount
    [SerializeField] private Image hpUIImage;

    //用来显示弹药包的数量 Used to display the number of ammunition packs
    [Header("TMP_Text：请手动拖入弹药包的Text")] //TMP_Text: Please manually drag in the text of the ammunition pack
    [SerializeField] private TMP_Text ammoCountText;

    //用来显示弹药包CD的时间 Used to display the CD time of the ammo pack
    [Header("TMP_Text：请手动拖入弹药包CD的Text")] //TMP_Text: Please manually drag in the Text of the ammunition pack CD
    [SerializeField] private TMP_Text ammoCDText;

    //用来显示血包的数量 Used to display the number of blood bags
    [Header("TMP_Text：请手动拖入血包的Text")] //TMP_Text: Please manually drag the text of the blood pack
    [SerializeField] private TMP_Text hpCountText;

    //用来显示血包CD的时间 Used to display the CD time of the health pack
    [Header("TMP_Text：请手动拖入血包CD的Text")] //TMP_Text: Please manually drag in the text of the health pack CD
    [SerializeField] private TMP_Text hpCDText;

    //用来显示一些Debug信息 Used to display some Debug information
    [Header("TMP_Text：请手动拖入自己的Debug的UI")] //TMP_Text: Please manually drag in your own Debug UI
    [SerializeField] private TMP_Text DebugUI_Text;

    //一些CD值的显示信息 无需暴露 Some CD value display information does not need to be exposed
    private const string AmmoPrefix = "Ammo: ";
    private const string CDingPrefix = "CDing: ";
    private const string HPPackPrefix = "HPPack: ";


    /// <第二部分：Awake Start Update 里面分别需要什么逻辑>  Part 2: What logic is needed in Awake Start Update
    /// Part002: 开局做什么 更新做什么
    /// Part002: What logic is needed in `Awake`, `Start`, `Update`.

    //On Start
    void Start()
    {
        //每当这个代码被调用的时候，游戏就继续 Whenever this code is called, the game continues
        CC0A_UserData.Instance.isGamePaused = false;

        //初始化DebugUI的值 
        CC0A_UserData.Instance.PlayerStateString = "Walk";
        CC0A_UserData.Instance.GunStateString = "NotShooting";
        CC0A_UserData.Instance.EnemyStateString = "Patrolling";
        V002_UpdateUIText(DebugUI_Text, $"PlayerState:{CC0A_UserData.Instance.PlayerStateString}\n" +
                                        $"GunState:{CC0A_UserData.Instance.GunStateString}\n" +
                                        $"EnemyState:{CC0A_UserData.Instance.EnemyStateString}",
                                        "");
    }

    //On Update
    void Update()
    {
        //更新子弹槽 Updated bullet slot
        V001_UpdateFillAmount(bulletUIImage, CC0A_UserData.Instance.BulletAccount, CC0A_UserData.Instance.BulletAccountMAX, "我的子弹槽 UI_Image");
        //更新血槽 Update health bar
        V001_UpdateFillAmount(hpUIImage, CC0A_UserData.Instance.PlayerBlood, CC0A_UserData.Instance.PlayerBloodMax, "我的血槽 UI_Image");
        //更新弹药包数量 Updated ammo pack quantity
        V002_UpdateUIText(ammoCountText, CC0A_UserData.Instance.AmmoPack.ToString(), AmmoPrefix);
        //更新弹药包CD值 Updated the CD value of ammo packs
        V002_UpdateUIText(ammoCDText, CC0A_UserData.Instance.AmoCDing.ToString(), CDingPrefix);
        //更新血包数量 Update blood pack quantity
        V002_UpdateUIText(hpCountText, CC0A_UserData.Instance.HPPack.ToString(), HPPackPrefix);
        //更新血包CD值 Updated blood pack CD value
        V002_UpdateUIText(hpCDText, CC0A_UserData.Instance.HPCDing.ToString(), CDingPrefix);

        //更新DebugUI的值 Update the value of DebugUI
        V002_UpdateUIText(DebugUI_Text, $"PlayerState:{CC0A_UserData.Instance.PlayerStateString}\n" +
                                        $"GunState:{CC0A_UserData.Instance.GunStateString}\n" +
                                        $"EnemyState:{CC0A_UserData.Instance.EnemyStateString}",
                                        "");
    }

    /// <第三部分：主要功能区> //Part 3: Main Functionality Area
    /// Part003: 里面放了各种void 方法
    /// Part003: It contains various `void` methods.

    //【V001】更新 UI 各种槽 的 值
    //【V001】Update the values ​​of various UI slots
    private void V001_UpdateFillAmount(Image image, float currentValue, float maxValue, string debugMessage)
    {
        if (image != null)
        {
            image.fillAmount = Mathf.InverseLerp(0, maxValue, currentValue);//转换为0-1之间的值
        }
        else
        {
            //Debug.Log($"【MXXS的提醒】：需要拖入一个 {debugMessage}");
        }
    }

    //【V002】更新 UI 各种Text 的 值
    //【V002】Update the values ​​of various Text in UI
    private void V002_UpdateUIText(TMP_Text textElement, string value, string prefix)
    {
        if (textElement != null)
        {
            textElement.text = $"{prefix}{value}";
        }
        else
        {
            //Debug.Log($"【MXXS的提醒】：需要拖入一个 {prefix} 数值 UI_Text");
        }
    }
}