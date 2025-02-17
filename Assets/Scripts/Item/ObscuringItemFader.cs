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
        StartCoroutine(FadeOutRoutine());//��������Я�̣��������͸���ȼ��٣�ͬʱ��Ӱ�����̵߳�����
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInRoutine());
    }



    private IEnumerator FadeInRoutine()
    {
        float cur_A = SpriteRenderer.color.a;
        float distance = 1f - cur_A;

        while (1f - cur_A > 0.01f) // ��cur_A ���� 1 �ƽ���ʵ�ֵ����Ч��
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

        while(cur_A- Settings.targetAlpha > 0.01f) // ��cur_A ���� Settings.targetAlpha�ƽ���ʵ�ֵ�����Ч��
        {
            cur_A = cur_A - distance/ Settings.targetAlpha * Time.deltaTime;
            SpriteRenderer.color = new Color(1f, 1f, 1f, cur_A);
            yield return null;
        }
        
        SpriteRenderer.color = new Color(1f,1f,1f,Settings.targetAlpha);
    }
}
