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

    public void FadeAndLoadScene(string sceneName, Vector3 spawnPosition)  // 这里Vector3 spawnPosition 用于指明玩家在新场景中的位置
    {
        if (!isFading) // 如果没有在Fading ，就执行Fade 与 场景切换
        {
            StartCoroutine(FadeAndSwitchScenes(sceneName, spawnPosition));
        }
    }

    private IEnumerator FadeAndSwitchScenes(string sceneName, Vector3 spawnPosition)
    {

        /*      yield return的用法
                yield return null;          // 下一帧再执行后续代码
                yield return 0;             //下一帧再执行后续代码
                yield return 6;//(任意数字) 下一帧再执行后续代码
                yield break; //直接结束该协程的后续操作
                yield return asyncOperation;//等异步操作结束后再执行后续代码
                yield return StartCoroution(某个协程);//等待某个协程执行完毕后再执行后续代码
                yield return WWW();//等待WWW操作完成后再执行后续代码
                yield return new WaitForEndOfFrame();//等待帧结束,等待直到所有的摄像机和GUI被渲染完成后，在该帧显示在屏幕之前执行
                yield return new WaitForSeconds(0.3f);//等待0.3秒，一段指定的时间延迟之后继续执行，在所有的Update函数完成调用的那一帧之后（这里的时间会受到Time.timeScale的影响）;
                yield return new WaitForSecondsRealtime(0.3f);//等待0.3秒，一段指定的时间延迟之后继续执行，在所有的Update函数完成调用的那一帧之后（这里的时间不受到Time.timeScale的影响）;
                yield return WaitForFixedUpdate();//等待下一次FixedUpdate开始时再执行后续代码
                yield return new WaitUntil()//将协同执行直到 当输入的参数（或者委托）为true的时候....如:yield return new WaitUntil(() => frame >= 10);
                yield return new WaitWhile()//将协同执行直到 当输入的参数（或者委托）为false的时候.... 如:yield return new WaitWhile(() => frame < 10);*/


        //调用场景卸载前的淡出事件
        EventHandler.CallBeforeSceneUnloadFadeOutEvent();

        //过渡到全黑，并等待到Fade 协程执行完毕 再执行下面的代码
        yield return StartCoroutine(Fade(1f));

        //储存当前场景的数据
        SaveLoadManager.Instance.StoreCurrentSceneData();

        //设置玩家在新场景中的位置
        Player.Instance.gameObject.transform.position = spawnPosition;
        Camera.main.transform.position = spawnPosition;

        //执行场景卸载前的事件 
        EventHandler.CallBeforeSceneUnloadEvent();

        //异步卸载当前场景 ， 并等待异步操作结束后再执行下面的代码
        yield return SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);

        //开启加载新场景并设置Active 的协程 ， 并等待到 LoadSceneAndSetActive 协程执行完毕 再执行下面的代码
        yield return StartCoroutine(LoadSceneAndSetActive(sceneName));

        //新场景加载结束后调用事件
        EventHandler.CallAfterSceneloadEvent();

        //恢复新场景中的item 
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        //Fade 回原来的画面，并等待到Fade 协程执行完毕 再执行下面的代码
        yield return StartCoroutine(Fade(0f));

        //调用 新场景加载后 的淡入事件
        EventHandler.CallAfterSceneloadFadeInEvent();
    }

    private IEnumerator LoadSceneAndSetActive(string sceneName)
    {
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //这里是寻找 最后被加载进来的场景 (可以把场景理解成栈 ， 后进先出，上面的代码把新场景 压入 这个“栈” 然后这里又从“栈” 中取出需要的场景 , 并设定为有效
        Scene newlyLoadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount - 1);
        SceneManager.SetActiveScene(newlyLoadedScene);
    }

    private IEnumerator Start()
    {
        faderImage.color = new Color(0f,0f, 0f, 1f);    
        faderCanvasGroup.alpha = 1f;

        yield return StartCoroutine(LoadSceneAndSetActive(StartingSceneName.ToString()));
        EventHandler.CallAfterSceneloadEvent();

        //开始时，也要恢复新（目前）场景中的item 
        SaveLoadManager.Instance.RestoreCurrentSceneData();

        StartCoroutine(Fade(0f));
    }

    private IEnumerator Fade(float finalAlpha)
    {
        isFading = true;

        faderCanvasGroup.blocksRaycasts = true;

        //fadeDuration = (finalAlpha == 0) ? 3f : 1f;

        
        float fadeSpeed = Mathf.Abs(faderCanvasGroup.alpha - finalAlpha) / fadeDuration;

        while(!Mathf.Approximately(faderCanvasGroup.alpha , finalAlpha)) // 只要faderCanvasGroup.alpha 没有无限接近 finalAlpha 就执行这个循环
        {
            faderCanvasGroup.alpha = Mathf.MoveTowards(faderCanvasGroup.alpha , finalAlpha , fadeSpeed * Time.deltaTime);

            yield return null; // 等待一帧后再继续
        }

        isFading = false;

        faderCanvasGroup.blocksRaycasts = false;

    }
}
