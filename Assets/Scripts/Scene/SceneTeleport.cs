using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))] //会自动在 挂在了该组件的物体 上 挂一个BoxCollider2D
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameGoto = SceneName.scene2_Field;
    [SerializeField] private Vector3 scenePositionGoto = new Vector3(); // 指定的要传送到新场景中的位置
    private SpriteRenderer childSprite;
    private bool trigger = false;
    private Player player;
    private void OnEnable()
    {
        childSprite = GetComponentInChildren<SpriteRenderer>();
        childSprite.enabled = false;
    }

   
    private void OnTriggerStay2D(Collider2D collision)
    {
        player = collision.GetComponent<Player>();
        Debug.Log("按E进入");
        childSprite.enabled = true;
        trigger = true;
        if (Input.GetKeyUp(KeyCode.E) && trigger)
        {
            if (player != null)
            {

                //三目运算符 ， 这里 Approximately 是因为有有些浮点数 表示的数相同，但由于移码之类的原因，在计算机中的表现不同，所以要用Approximately 来判断两个浮点数是否相等（接近）
                float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;
                //如果 scenePositionGoto.x 为零，也就是新场景中没有指定的位置，这里就直接用玩家当前的x值来代替

                float yPosition = Mathf.Approximately(scenePositionGoto.y, 0f) ? player.transform.position.y : scenePositionGoto.y;

                float zPosition = 0f;

                SceneControllerManager.Instance.FadeAndLoadScene(sceneNameGoto.ToString(), new Vector3(xPosition, yPosition, zPosition));
            }
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        childSprite.enabled = false;
        trigger = false;
    }
}
