using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CropDetailsList", menuName = "Scriptable Objects/Crop/Crop Details List")]
public class SO_CropDetailsList : ScriptableObject
{
    [SerializeField]
    public List<CropDetails> CropDetails;

    public CropDetails GetCropDetails(int seedItemCode)
    {
        // ���� seedItemCode �� CropDetails ��ȡ��Ӧ�� CropDetail
        return CropDetails.Find( x=> x.seedItemCode == seedItemCode);    
    }
}
