using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))] //���Զ��� �����˸���������� �� ��һ��BoxCollider2D
public class SceneTeleport : MonoBehaviour
{
    [SerializeField] private SceneName sceneNameGoto = SceneName.scene2_Field;
    [SerializeField] private Vector3 scenePositionGoto = new Vector3(); // ָ����Ҫ���͵��³����е�λ��
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
        Debug.Log("��E����");
        childSprite.enabled = true;
        trigger = true;
        if (Input.GetKeyUp(KeyCode.E) && trigger)
        {
            if (player != null)
            {

                //��Ŀ����� �� ���� Approximately ����Ϊ����Щ������ ��ʾ������ͬ������������֮���ԭ���ڼ�����еı��ֲ�ͬ������Ҫ��Approximately ���ж������������Ƿ���ȣ��ӽ���
                float xPosition = Mathf.Approximately(scenePositionGoto.x, 0f) ? player.transform.position.x : scenePositionGoto.x;
                //��� scenePositionGoto.x Ϊ�㣬Ҳ�����³�����û��ָ����λ�ã������ֱ������ҵ�ǰ��xֵ������

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
