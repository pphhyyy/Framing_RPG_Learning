using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ObscuringItemFader : MonoBehaviour
{
    private SpriteRenderer SpriteRenderer;

    private void Awake()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();    
    }
    
    public void FadeOut()
    {
        StartCoroutine(FadeOutRoutine());//开启淡出携程，让物体的透明度减少，同时不影响主线程的运行
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }



    private IEnumerator FadeInRoutine()
    {
        float cur_A = SpriteRenderer.color.a;
        float distance = 1f - cur_A;

        while (1f - cur_A > 0.01f) // 让cur_A 操着 1 逼近，实现淡入的效果
        {
            cur_A = cur_A + distance / Settings.targetAlpha * Time.deltaTime;
            SpriteRenderer.color = new Color(1f, 1f, 1f, cur_A);
            yield return null;
        }

        SpriteRenderer.color = new Color(1f, 1f, 1f, 1f);
    }
    private IEnumerator FadeOutRoutine()
    {
        float cur_A = SpriteRenderer.color.a;
        float distance = cur_A - Settings.targetAlpha;

        while(cur_A- Settings.targetAlpha > 0.01f) // 让cur_A 操着 Settings.targetAlpha逼近，实现淡出的效果
        {
            cur_A = cur_A - distance/ Settings.targetAlpha * Time.deltaTime;
            SpriteRenderer.color = new Color(1f, 1f, 1f, cur_A);
            yield return null;
        }
        
        SpriteRenderer.color = new Color(1f,1f,1f,Settings.targetAlpha);
    }
}
