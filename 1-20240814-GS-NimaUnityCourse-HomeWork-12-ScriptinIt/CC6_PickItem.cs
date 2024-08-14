using System.Net.NetworkInformation;
using UnityEngine;

public class CC6_PickItem : MonoBehaviour
{
    //这一部分是使用拾取物的逻辑，也是附加在玩家身上，这样子玩家可以使用背包中的物品
    //This part uses the logic of picking up items and is also attached to the player so that the player can use the items in the backpack.

    /// <第一部分：需要的变量类型>
    /// Part001: Calling CharacterController

    [Header("AudioSource：需要手动拖入：装填子弹的音效")]//Need to be manually dragged in: the sound effect of loading bullets
    [SerializeField] AudioSource AS_ReloadAmmo;//装填子弹的声音 //The sound of loading bullets
    [Header("AudioSource：需要手动拖入：使用回血包的音效")]//Need to be dragged in manually: Use the sound effect of the blood recovery pack
    [SerializeField] AudioSource AS_RecoverBlood;//使用回血包的声音 //The sound of using a blood pack

    float currentAmmoTime = 0f;// 记录填充子弹的时间 //Record the time to reload the bullet
    float currentHPTime = 0f;// 记录填充血量的时间 //Record the time to fill the blood volume


    /// <第二部分：Awake Start Update 里面分别需要什么逻辑>  Part 2: What logic is needed in Awake Start Update
    /// Part002: 开局做什么 更新做什么
    /// Part002: What logic is needed in `Awake`, `Start`, `Update`.

    void Start() //游戏开始的时候 复位一切 //Reset everything when the game starts
    {
        //填充子弹 把我空的子弹数量填满 //Fill up my empty bullets
        CC6_V001_FillAmmo();

        //补充血量 把我空的血量填满 //Replenish my blood. Fill up my empty blood.
        CC6_V002_FillMyBlood();

        // 设置currentAmmoTime为一个负值，以确保游戏一开始不触发CD
        // Set currentAmmoTime to a negative value to ensure that the CD is not triggered at the beginning of the game
        currentAmmoTime = -CC0A_UserData.Instance.AmoCD;
        currentHPTime = -CC0A_UserData.Instance.HPCD;
    }

    void Update()
    {
        CC6_V003_EnterReloadAmmo(); // Enter装填子弹 //EnterReload bullet

        CC6_V004_BackSpaceRefillMyBlood();//BackSpace回血 //BackSpace Recovery

        CC6_V005_CalculateAmmoCDingString(CC0A_UserData.Instance.HPCD, ref CC0A_UserData.Instance.HPCDing, currentHPTime);
        CC6_V005_CalculateAmmoCDingString(CC0A_UserData.Instance.AmoCD, ref CC0A_UserData.Instance.AmoCDing, currentAmmoTime);
    }


    /// <第三部分：主要功能区> //Part 3: Main Functionality Area
    /// Part003: 里面放了各种void 方法
    /// Part003: It contains various `void` methods.

    // 【V001】填充子弹的逻辑 Logic for filling bullets
    void CC6_V001_FillAmmo()
    {
        CC0A_UserData.Instance.BulletAccount = CC0A_UserData.Instance.BulletAccountMAX;
        //Debug.Log("游戏开始 已填充完毕子弹啦~！");
    }

    // 【V002】补充血量 Replenish blood
    void CC6_V002_FillMyBlood()
    {

        CC0A_UserData.Instance.PlayerBlood = CC0A_UserData.Instance.PlayerBloodMax;
        //CC0_UserData.Instance.PlayerBlood = 1; //暂时挂一下(方便我死掉 测试UI跳转
        //Debug.Log("游戏开始 血量已经补充完毕啦~！");
    }

    // 【V003】通过按住Enter键，进行子弹的填充 By holding down the Enter key, the bullet is refilled
    void CC6_V003_EnterReloadAmmo()
    {
        if (Input.GetKeyDown(KeyCode.Return) && CC0A_UserData.Instance.AmmoPack > 0)//如果摁下Enter 且存在弹药包 
                                                                                    //If Enter is pressed and an ammo pack is present
        {
            if (Time.time - currentAmmoTime > CC0A_UserData.Instance.AmoCD)  //已经过了CD时间 CD time has passed
            {
                //复位时间 Reset time
                CC0A_UserData.Instance.PV003_UpdateTimeTime(ref currentAmmoTime);

                //减去弹药包数量 Subtract the number of ammo packs
                CC0A_UserData.Instance.AmmoPack--;

                //播放音效 Play sound effects
                CC0A_UserData.Instance.PV002_PlayAudioSource(AS_ReloadAmmo);

                //填充子弹 Filling bullets
                CC6_V001_FillAmmo();
            }
        }
    }

    // 【V004】通过按住BackSpace键，进行血量的填充 By holding down the BackSpace key, you can fill up your health.
    void CC6_V004_BackSpaceRefillMyBlood()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) && CC0A_UserData.Instance.HPPack > 0)//如果摁下Enter 且存在弹药包 
                                                                                     //If Enter is pressed and an ammo pack is present
        {
            if (Time.time - currentHPTime > CC0A_UserData.Instance.HPCD)  //已经过了CD时间 //CD time has passed
            {
                //复位时间 Reset time
                CC0A_UserData.Instance.PV003_UpdateTimeTime(ref currentHPTime);

                //减去血包数量 Subtract the number of HP packs
                CC0A_UserData.Instance.HPPack--;

                //播放音效 Play sound effects
                CC0A_UserData.Instance.PV002_PlayAudioSource(AS_RecoverBlood);

                //补充我的血量 FillingMyBlood
                CC6_V002_FillMyBlood();
            }
        }
    }

    // 【V014】计算CD的显示时间 Calculate the display time of a CD
    void CC6_V005_CalculateAmmoCDingString(float CD, ref float CDing, float currentTime)
    {
        if (Time.time - currentTime < CD) //如果还在CD时间内 If it is still within the CD time
        {
            //则计算CD的时间，并保留小数点后1位 Then calculate the CD time and keep 1 decimal place
            float ammoCDing = CD - (Time.time - currentTime);
            CDing = Mathf.Max(0, Mathf.Round(ammoCDing * 10f) / 10f); //并且使得它不能小于0 And make it not less than 0
        }
        else
        {
            CDing = 0;
        }
    }
}