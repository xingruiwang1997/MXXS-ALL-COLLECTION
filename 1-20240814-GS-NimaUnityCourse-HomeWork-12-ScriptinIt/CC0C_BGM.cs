using UnityEngine;
using UnityEngine.Audio;

public class CC0C_BGM : MonoBehaviour
{
    /// <˵��> illustrate
    /// PartAll������������Ϸ�����г�����ֻ������һ��BGM������ʹ�����ַ�����ȷ����Ϸ�ӿ�ʼ���˳������BGM��Զ�����
    /// PartAll��Since only this one BGM is played in all scenes of the entire game, this method ensures 
    /// that this BGM will never be interrupted from the start to the exit of the game.

    // AudioSource��� AudioSource Component
    [Header("AudioSource�����ֶ�����BGM����Ч")] //AudioSource: Please manually drag in the BGM sound effects
    [SerializeField] private AudioSource AS_BG;

    // ��������ȷ��ֻ����1�� BGM //Singleton, ensure that BGM is played only once
    // ����public ��Ϊ���BGM�������ᱻ�������κδ�����õ������Ƕ�������
    // No need to use public because this BGM will not be called by any other code at all. It is beautiful alone.
    private static CC0C_BGM instance;

    // Awake
    void Awake()
    {
        // ����Ѿ���instance���������Լ�
        // If there is already an instance, destroy itself
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // ����ʵ��Ϊ��ǰ����
        // Set the instance as the current object
        instance = this;

        // ������Ƿ������ֶ�����BGM��
        if (AS_BG == null)
        {
            Debug.Log("CC0C_BGM ���� ����������BGM����");
            return;
        }

        // ������û�в��ŵ�ʱ���򲥷�����
        // When the music is not playing, play the music
        if (!AS_BG.isPlaying)
        {
            AS_BG.Play();
        }
    }
}