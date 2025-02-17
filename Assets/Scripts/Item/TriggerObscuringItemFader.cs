using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{

    //���к͸ýű�����ң� ��ײ����ObscuringItemFader ������ ��ִ�е��뵭����Э��
    private void OnTriggerEnter2D(Collider2D collision)
    {
        ObscuringItemFader[] obscuringItemFaders = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if(obscuringItemFaders.Length > 0 )
        {
            for(int i = 0; i < obscuringItemFaders.Length; i++)
            {
                obscuringItemFaders[i].FadeOut();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ObscuringItemFader[] obscuringItemFaders = collision.gameObject.GetComponentsInChildren<ObscuringItemFader>();

        if(obscuringItemFaders.Length > 0 )
        {
            for(int i = 0; i < obscuringItemFaders.Length; i++)
            {
                obscuringItemFaders[i].FadeIn();
            }
        }
    }
}
