using UnityEngine;

public abstract class SingletonMonobehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    //����misc �ļ��� misc��ʵ��Ӣ��miscellaneous��ǰ�ĸ���ĸ���������塢���ӻ����˼��
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
        else //����������������еڶ��������屻������������awake �������ͻ���� else ��destory
        {
            Destroy(gameObject);
        }
    }
}
