using UnityEngine;

public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    //来自misc 文件夹 misc其实是英文miscellaneous的前四个字母，杂项、混合体、大杂烩的意思。
    private static T instance;
    public static T Instance
    {
        get 
        {
            return instance;
        }
    }

    protected virtual void Awake()
    {
        if(instance == null)
        {
            instance = this as T;
        }
        else //单例，如果场景上有第二个该物体被创建并调用了awake 函数，就会进入 else 被destory
        {
            Destroy(gameObject);
        }
    }
}
