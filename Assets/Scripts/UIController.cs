using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public Image img_health;
    public Image img_stamina;
    
    // Start is called before the first frame update
    void Start()
    {
        img_health.fillAmount = 0.6f;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
