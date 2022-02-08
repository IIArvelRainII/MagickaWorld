using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class BoxDialog : MonoBehaviour
{
    public bool playerIsInTheDialogZone = false;
    public int numberOfDialog = 0;
    public GameObject[] text;
    private PlayerController playercontroller;
    private GameObject playercontrollerPOS;
    private float delayOfDialog = 0.5f;
    private AudioSource audioBox;
    


    private void Awake()
    {
        playercontroller = FindObjectOfType<PlayerController>();
        playercontrollerPOS = FindObjectOfType<PlayerController>().gameObject;
        audioBox = GetComponent<AudioSource>();
    }

    private void Update()
    {
        delayOfDialog -= Time.deltaTime;
    }

    private void OnTriggerStay(Collider other)
    {
        if (playercontroller)
        {
            playerIsInTheDialogZone = true;

            if (playerIsInTheDialogZone)
            {
                if (Input.GetKey(KeyCode.T) && delayOfDialog <= 0)
                {
                    numberOfDialog++;
                    if(numberOfDialog == 1)
                    {
                        delayOfDialog = 0.5f;
                        text[0].SetActive(false);
                        text[1].SetActive(true);
                        audioBox.Play();
                    }

                    if (numberOfDialog == 2)
                    {
                        delayOfDialog = 0.5f;
                        text[1].SetActive(false);
                        text[2].SetActive(true);
                        audioBox.Play();
                    }
                    if (numberOfDialog == 3)
                    {
                        delayOfDialog = 0.5f;
                        text[2].SetActive(false);
                        text[3].SetActive(true);
                        audioBox.Play();
                    }
                    if (numberOfDialog == 4)
                    {
                        delayOfDialog = 0.5f;
                        text[3].SetActive(false);
                        text[4].SetActive(true);
                        audioBox.Play();
                    }
                    if (numberOfDialog == 5)
                    {
                        delayOfDialog = 0.5f;
                        text[4].SetActive(false);
                        text[5].SetActive(true);
                        audioBox.Play();
                    }
                    if (numberOfDialog == 6)
                    {
                        delayOfDialog = 0.5f;
                        text[5].SetActive(false);
                        text[6].SetActive(true);
                        audioBox.Play();
                    }
                    if (numberOfDialog == 7)
                    {
                        delayOfDialog = 0.5f;
                        text[6].SetActive(false);
                        text[7].SetActive(true);
                        playercontroller.life = 20;
                        playercontroller.numberOfLifePotions = 3;
                        audioBox.Play();
                    }
                    if (numberOfDialog == 8)
                    {
                        delayOfDialog = 0.5f;
                        text[7].SetActive(false);
                        text[8].SetActive(true);
                        audioBox.Play();
                    }
                    if (numberOfDialog == 9)
                    {
                        delayOfDialog = 0.5f;
                        text[8].SetActive(false);
                        text[0].SetActive(true);
                        audioBox.Play();
                        numberOfDialog = 0;                       
                    }
                }

                if (Input.GetKey(KeyCode.P))
                {
                    SceneManager.LoadScene("Arena");
                }



            }
        }

      
    }

    private void OnTriggerExit(Collider other)
    {
        if (playerIsInTheDialogZone)
        {
            playerIsInTheDialogZone = false;
        }
    }
}
