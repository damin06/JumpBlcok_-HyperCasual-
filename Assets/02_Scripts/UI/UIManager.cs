using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI m_socreTXT;

    private void Start()
    {

    }

    public void ModifyScoreTXT(int score)
    {
        m_socreTXT.text = score.ToString();
    }
}
