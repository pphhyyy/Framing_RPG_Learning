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
        // 根据 seedItemCode 中 CropDetails 获取对应的 CropDetail
        return CropDetails.Find( x=> x.seedItemCode == seedItemCode);    
    }
}
