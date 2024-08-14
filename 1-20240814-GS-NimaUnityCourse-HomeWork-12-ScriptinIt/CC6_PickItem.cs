using System.Net.NetworkInformation;
using UnityEngine;

public class CC6_PickItem : MonoBehaviour
{
    //��һ������ʹ��ʰȡ����߼���Ҳ�Ǹ�����������ϣ���������ҿ���ʹ�ñ����е���Ʒ
    //This part uses the logic of picking up items and is also attached to the player so that the player can use the items in the backpack.

    /// <��һ���֣���Ҫ�ı�������>
    /// Part001: Calling CharacterController

    [Header("AudioSource����Ҫ�ֶ����룺װ���ӵ�����Ч")]//Need to be manually dragged in: the sound effect of loading bullets
    [SerializeField] AudioSource AS_ReloadAmmo;//װ���ӵ������� //The sound of loading bullets
    [Header("AudioSource����Ҫ�ֶ����룺ʹ�û�Ѫ������Ч")]//Need to be dragged in manually: Use the sound effect of the blood recovery pack
    [SerializeField] AudioSource AS_RecoverBlood;//ʹ�û�Ѫ�������� //The sound of using a blood pack

    float currentAmmoTime = 0f;// ��¼����ӵ���ʱ�� //Record the time to reload the bullet
    float currentHPTime = 0f;// ��¼���Ѫ����ʱ�� //Record the time to fill the blood volume


    /// <�ڶ����֣�Awake Start Update ����ֱ���Ҫʲô�߼�>  Part 2: What logic is needed in Awake Start Update
    /// Part002: ������ʲô ������ʲô
    /// Part002: What logic is needed in `Awake`, `Start`, `Update`.

    void Start() //��Ϸ��ʼ��ʱ�� ��λһ�� //Reset everything when the game starts
    {
        //����ӵ� ���ҿյ��ӵ��������� //Fill up my empty bullets
        CC6_V001_FillAmmo();

        //����Ѫ�� ���ҿյ�Ѫ������ //Replenish my blood. Fill up my empty blood.
        CC6_V002_FillMyBlood();

        // ����currentAmmoTimeΪһ����ֵ����ȷ����Ϸһ��ʼ������CD
        // Set currentAmmoTime to a negative value to ensure that the CD is not triggered at the beginning of the game
        currentAmmoTime = -CC0A_UserData.Instance.AmoCD;
        currentHPTime = -CC0A_UserData.Instance.HPCD;
    }

    void Update()
    {
        CC6_V003_EnterReloadAmmo(); // Enterװ���ӵ� //EnterReload bullet

        CC6_V004_BackSpaceRefillMyBlood();//BackSpace��Ѫ //BackSpace Recovery

        CC6_V005_CalculateAmmoCDingString(CC0A_UserData.Instance.HPCD, ref CC0A_UserData.Instance.HPCDing, currentHPTime);
        CC6_V005_CalculateAmmoCDingString(CC0A_UserData.Instance.AmoCD, ref CC0A_UserData.Instance.AmoCDing, currentAmmoTime);
    }


    /// <�������֣���Ҫ������> //Part 3: Main Functionality Area
    /// Part003: ������˸���void ����
    /// Part003: It contains various `void` methods.

    // ��V001������ӵ����߼� Logic for filling bullets
    void CC6_V001_FillAmmo()
    {
        CC0A_UserData.Instance.BulletAccount = CC0A_UserData.Instance.BulletAccountMAX;
        //Debug.Log("��Ϸ��ʼ ���������ӵ���~��");
    }

    // ��V002������Ѫ�� Replenish blood
    void CC6_V002_FillMyBlood()
    {

        CC0A_UserData.Instance.PlayerBlood = CC0A_UserData.Instance.PlayerBloodMax;
        //CC0_UserData.Instance.PlayerBlood = 1; //��ʱ��һ��(���������� ����UI��ת
        //Debug.Log("��Ϸ��ʼ Ѫ���Ѿ����������~��");
    }

    // ��V003��ͨ����סEnter���������ӵ������ By holding down the Enter key, the bullet is refilled
    void CC6_V003_EnterReloadAmmo()
    {
        if (Input.GetKeyDown(KeyCode.Return) && CC0A_UserData.Instance.AmmoPack > 0)//�������Enter �Ҵ��ڵ�ҩ�� 
                                                                                    //If Enter is pressed and an ammo pack is present
        {
            if (Time.time - currentAmmoTime > CC0A_UserData.Instance.AmoCD)  //�Ѿ�����CDʱ�� CD time has passed
            {
                //��λʱ�� Reset time
                CC0A_UserData.Instance.PV003_UpdateTimeTime(ref currentAmmoTime);

                //��ȥ��ҩ������ Subtract the number of ammo packs
                CC0A_UserData.Instance.AmmoPack--;

                //������Ч Play sound effects
                CC0A_UserData.Instance.PV002_PlayAudioSource(AS_ReloadAmmo);

                //����ӵ� Filling bullets
                CC6_V001_FillAmmo();
            }
        }
    }

    // ��V004��ͨ����סBackSpace��������Ѫ������� By holding down the BackSpace key, you can fill up your health.
    void CC6_V004_BackSpaceRefillMyBlood()
    {
        if (Input.GetKeyDown(KeyCode.Backspace) && CC0A_UserData.Instance.HPPack > 0)//�������Enter �Ҵ��ڵ�ҩ�� 
                                                                                     //If Enter is pressed and an ammo pack is present
        {
            if (Time.time - currentHPTime > CC0A_UserData.Instance.HPCD)  //�Ѿ�����CDʱ�� //CD time has passed
            {
                //��λʱ�� Reset time
                CC0A_UserData.Instance.PV003_UpdateTimeTime(ref currentHPTime);

                //��ȥѪ������ Subtract the number of HP packs
                CC0A_UserData.Instance.HPPack--;

                //������Ч Play sound effects
                CC0A_UserData.Instance.PV002_PlayAudioSource(AS_RecoverBlood);

                //�����ҵ�Ѫ�� FillingMyBlood
                CC6_V002_FillMyBlood();
            }
        }
    }

    // ��V014������CD����ʾʱ�� Calculate the display time of a CD
    void CC6_V005_CalculateAmmoCDingString(float CD, ref float CDing, float currentTime)
    {
        if (Time.time - currentTime < CD) //�������CDʱ���� If it is still within the CD time
        {
            //�����CD��ʱ�䣬������С�����1λ Then calculate the CD time and keep 1 decimal place
            float ammoCDing = CD - (Time.time - currentTime);
            CDing = Mathf.Max(0, Mathf.Round(ammoCDing * 10f) / 10f); //����ʹ��������С��0 And make it not less than 0
        }
        else
        {
            CDing = 0;
        }
    }
}