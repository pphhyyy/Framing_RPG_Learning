using UnityEngine;

[ExecuteAlways] //���ExecuteAlways ����������ڱ༭��״̬��Ҳ��������
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = "";

    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        if(!Application.IsPlaying(gameObject)) // ����IsPlaying ����gameObject ����Ϸ״̬ʱΪ�棬���༭��״̬Ϊ�٣� ����Ϊ���� �� �� ��������Ĵ���ֻ�б༭��״̬�¿�������
        {
            if(_gUID == "")
            {
                _gUID = System.Guid.NewGuid().ToString();   //����һ��Ψһ��Guid��ͨ��mac �� ʱ��֮��Ķ��� �� �� ����ʾ��ǰ�����壬Ҳ����Ҫ����Ķ���
            }
        }
    }
}
