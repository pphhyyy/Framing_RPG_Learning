using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIInventoryTextBox : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMeshTop1 = null;
    [SerializeField] private TextMeshProUGUI textMeshTop2 = null;
    [SerializeField] private TextMeshProUGUI textMeshTop3 = null;

    [SerializeField] private TextMeshProUGUI textMeshBottom1 = null;
    [SerializeField] private TextMeshProUGUI textMeshBottom2 = null;
    [SerializeField] private TextMeshProUGUI textMeshBottom3 = null;

    public void SetTextboxText(string t1 , string t2, string t3, string b1, string b2, string b3)
    {
        textMeshTop1.text = t1;
        textMeshTop2.text = t2;
        textMeshTop3.text = t3;
        textMeshBottom1.text = b1;
        textMeshBottom2.text = b2;
        textMeshBottom3.text = b3;
    }
}
