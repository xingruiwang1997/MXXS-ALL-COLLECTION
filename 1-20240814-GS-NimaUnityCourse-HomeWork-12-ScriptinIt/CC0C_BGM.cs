using UnityEngine;
using UnityEngine.Audio;

public class CC0C_BGM : MonoBehaviour
{
    /// <说明> illustrate
    /// PartAll：由于整个游戏，所有场景，只播放这一个BGM，所以使用这种方法，确保游戏从开始到退出，这个BGM永远不会断
    /// PartAll：Since only this one BGM is played in all scenes of the entire game, this method ensures 
    /// that this BGM will never be interrupted from the start to the exit of the game.

    // AudioSource组件 AudioSource Component
    [Header("AudioSource：请手动拖入BGM的音效")] //AudioSource: Please manually drag in the BGM sound effects
    [SerializeField] private AudioSource AS_BG;

    // 单例化，确保只播放1次 BGM //Singleton, ensure that BGM is played only once
    // 不用public 因为这个BGM根本不会被其他的任何代码调用到，它是独自美丽
    // No need to use public because this BGM will not be called by any other code at all. It is beautiful alone.
    private static CC0C_BGM instance;

    // Awake
    void Awake()
    {
        // 如果已经有instance，则销毁自己
        // If there is already an instance, destroy itself
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        // 设置实例为当前对象
        // Set the instance as the current object
        instance = this;

        // 检查我是否忘记手动拖入BGM了
        if (AS_BG == null)
        {
            Debug.Log("CC0C_BGM 里面 你忘记填入BGM啦！");
            return;
        }

        // 当音乐没有播放的时候，则播放音乐
        // When the music is not playing, play the music
        if (!AS_BG.isPlaying)
        {
            AS_BG.Play();
        }
    }
}