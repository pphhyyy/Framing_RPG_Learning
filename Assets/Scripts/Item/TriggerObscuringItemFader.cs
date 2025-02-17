using UnityEngine;

public class TriggerObscuringItemFader : MonoBehaviour
{

    //所有和该脚本（玩家） 碰撞的有ObscuringItemFader 的物体 都执行淡入淡出的协程
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
