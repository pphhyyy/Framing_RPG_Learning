using System.Collections;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using System.ComponentModel;

public class SceneControllerManager : SingletonMonobehaviour<SceneControllerManager>
{
    private bool isFading;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private CanvasGroup faderCanvasGroup = null;
    [SerializeField] private Image faderImage = null;
    public SceneName StartingSceneName;

    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)  // ����Vector3 spawnPosition ����ָ��������³����е�λ��
    {
        if (!isFading) // ���û����Fading ����ִ��Fade �� �����л�
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {

        /*      yield return���÷�
                yield return null;          // ��һ֡��ִ�к�������
                yield return 0;             //��һ֡��ִ�к�������
                yield return 6;//(��������) ��һ֡��ִ�к�������
                yield break; //ֱ�ӽ�����Э�̵ĺ�������
                yield return asyncOperation;//���첽������������ִ�к�������
                yield return StartCoroution(ĳ��Э��);//�ȴ�ĳ��Э��ִ����Ϻ���ִ�к�������
                yield return WWW();//�ȴ�WWW������ɺ���ִ�к�������
                yield return new WaitForEndOfFrame();//�ȴ�֡����,�ȴ�ֱ�����е��������GUI����Ⱦ��ɺ��ڸ�֡��ʾ����Ļ֮ǰִ��
                yield return new WaitForSeconds(0.3f);//�ȴ�0.3�룬һ��ָ����ʱ���ӳ�֮�����ִ�У������е�Update������ɵ��õ���һ֮֡�������ʱ����ܵ�Time.timeScale��Ӱ�죩;
                yield return new WaitForSecondsRealtime(0.3f);//�ȴ�0.3�룬һ��ָ����ʱ���ӳ�֮�����ִ�У������е�Update������ɵ��õ���һ֮֡�������ʱ�䲻�ܵ�Time.timeScale��Ӱ�죩;
                yield return WaitForFixedUpdate();//�ȴ���һ��FixedUpdate��ʼʱ��ִ�к�������
                yield return new WaitUntil()//��Эִͬ��ֱ�� ������Ĳ���������ί�У�Ϊtrue��ʱ��....��:yield return new WaitUntil(() => frame >= 10);
                yield return new WaitWhile()//��Эִͬ��ֱ�� ������Ĳ���������ί�У�Ϊfalse��ʱ��.... ��:yield return new WaitWhile(() => frame < 10);*/


        //���ó���ж��ǰ�ĵ����¼�
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        //���ɵ�ȫ�ڣ����ȴ���Fade Э��ִ����� ��ִ������Ĵ���
        yield return StartCoroutine(Fade(1f));

        //���浱ǰ����������
        SaveLoadManager.Instance.StoreCurrentSceneData();

        //����������³����е�λ��
        Player.Instance.gameObject.transform.position = spawnPosition;
        Camera.main.transform.position = spawnPosition;

        //ִ�г���ж��ǰ���¼� 
        EventHandler.CallBeforeSceneUnloadEvent();

        //�첽ж�ص�ǰ���� �� ���ȴ��첽������������ִ������Ĵ���
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        //���������³���������Active ��Э�� �� ���ȴ��� LoadSceneAndSetActive Э��ִ����� ��ִ������Ĵ���
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        //�³������ؽ���������¼�
        EventHandler.CallAfterSceneloadEvent();

        //�ָ��³����е�item 
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        //Fade ��ԭ���Ļ��棬���ȴ���Fade Э��ִ����� ��ִ������Ĵ���
        yield return StartCoroutine(Fade(0f));

        //���� �³������غ� �ĵ����¼�
        EventHandler.CallAfterSceneloadFadeInEvent();
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //������Ѱ�� ��󱻼��ؽ����ĳ��� (���԰ѳ�������ջ �� ����ȳ�������Ĵ�����³��� ѹ�� �����ջ�� Ȼ�������ִӡ�ջ�� ��ȡ����Ҫ�ĳ��� , ���趨Ϊ��Ч
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private IEnumerator Start()
    {
        faderImage.color = new Color(0f,0f, 0f, 1f);    
        faderCanvasGroup.alpha = 1f;

        yield return StartCoroutine(LoadSceneAndSetActive(StartingSceneName.ToString()));
        EventHandler.CallAfterSceneloadEvent();

        //��ʼʱ��ҲҪ�ָ��£�Ŀǰ�������е�item 
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float finalAlpha)
    {
        isFading = true;

        faderCanvasGroup.blocksRaycasts = true;

        //fadeDuration = (finalAlpha == 0) ? 3f : 1f;

        
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        while(!Mathf.Approximately(faderCanvasGroup.alpha , finalAlpha)) // ֻҪfaderCanvasGroup.alpha û�����޽ӽ� finalAlpha ��ִ�����ѭ��
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha , finalAlpha , fadeSpeed * Time.deltaTime);

            yield return null; // �ȴ�һ֡���ټ���
        }

        isFading = false;

        faderCanvasGroup.blocksRaycasts = false;

    }
}
