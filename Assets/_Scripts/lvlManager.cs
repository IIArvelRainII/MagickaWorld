using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class lvlManager : MonoBehaviour
{
    private PlayerController playercontroller;
    private BossDog bossdog;

    public GameObject winUI;
    public GameObject losseUI;

    private void Awake()
    {
        playercontroller = FindObjectOfType<PlayerController>();
        bossdog = FindObjectOfType<BossDog>();
    }

    // Update is called once per frame
    void Update()
    {
        if(playercontroller.life <= 0)
        {
            losseUI.SetActive(true);
        }
        
        if(bossdog.lifeAI <= 0)
        {         
            winUI.SetActive(true);
        }
    }
}
