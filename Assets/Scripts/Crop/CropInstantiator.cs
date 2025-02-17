using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ������ crop Ԥ������ ������ grid ���� �ֵ����ֵ
/// </summary>
public class CropInstantiator : MonoBehaviour
{
    private Grid grid;
    [SerializeField] private int daysSinceDug = -1;
    [SerializeField] private int daysSinceWatered = -1;
    [ItemCodeDescription]
    [SerializeField] private int seedItemCode = 0;
    [SerializeField] private int growthDays = 0;

    private void OnDisable()
    {
        EventHandler.InstantiateCropPrefabsEvent -= InstantiateCropPrefabs;

    }

    private void OnEnable()
    {
        EventHandler.InstantiateCropPrefabsEvent += InstantiateCropPrefabs;

    }

    private void InstantiateCropPrefabs()
    {
        Debug.Log("��ʼ�����������е�crop ��");
        //�ӵ�ǰ���ص������� ��ȡ grid ����
        grid = GameObject.FindObjectOfType<Grid>();

        //�ӵ�ǰ��crop �� ��ȡ grid �� λ�� 
        Vector3Int cropGridPosition = grid.WorldToCell(transform.position);

        //���� crop grid ������ 
        SetCropGridProperties(cropGridPosition);

        //��Ϊ�� ��ʼ������intantiator) ���������� �����Ժ� �Ϳ���������� ��Ϸ������
        Destroy(gameObject);
    }

    private void SetCropGridProperties(Vector3Int cropGridPosition)
    {
        GridPropertyDetails gridPropertyDetails;

        //�ӵ�ǰ cropgrid ���� ��ȡ grid ������ϸ��
        gridPropertyDetails = GridPropertIesManager.Instance.GetGridPropertyDetails(cropGridPosition.x, cropGridPosition.y);

        if (gridPropertyDetails == null) // �����ǰ�� cropGridPosition ��û���ҵ� gridPropertyDetails ���Լ�ʵ����һ�� 
        {
            Debug.Log("debug");
            gridPropertyDetails = new GridPropertyDetails();
        }
        
        // �� CropInstantiator �����õ� gridPropertyDetails
        // ��ֵ�� ���Ǵ� GridPropertIesManager �У�ͨ�� grid �����õ� gridPropertyDetails
        gridPropertyDetails.daysSinceDug = daysSinceDug;
        gridPropertyDetails.daySinceWatered = daysSinceWatered;
        gridPropertyDetails.seedItemCode = seedItemCode;
        gridPropertyDetails.growthDays = growthDays;

        //�� cropgrid �ĸ������ �ش��� GridPropertIesManager ����һ�����꣬��ʼ��������� 
        GridPropertIesManager.Instance.SetGridPropertyDetials(cropGridPosition.x,cropGridPosition.y , gridPropertyDetails);

    }
}