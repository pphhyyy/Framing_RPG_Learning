using UnityEngine;

[ExecuteAlways] //添加ExecuteAlways 后，这个代码在编辑器状态下也可以运行
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = "";

    public string GUID { get => _gUID; set => _gUID = value; }

    private void Awake()
    {
        if(!Application.IsPlaying(gameObject)) // 这里IsPlaying 就是gameObject 在游戏状态时为真，而编辑器状态为假， 又因为加了 ！ ， 所以下面的代码只有编辑器状态下可以运行
        {
            if(_gUID == "")
            {
                _gUID = System.Guid.NewGuid().ToString();   //生成一个唯一的Guid（通过mac 和 时间之类的东西 ） ， 来表示当前的物体，也就是要保存的对象
            }
        }
    }
}
