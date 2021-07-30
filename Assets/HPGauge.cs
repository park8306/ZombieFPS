using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPGauge : MonoBehaviour
{
    Image HPBar;
    // Start is called before the first frame update
    void Start()
    {
        HPBar = transform.Find("HPBar").GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        HPBar.fillAmount = FpsControllerLPFP.instance.playerHP / 100f;
    }
}
