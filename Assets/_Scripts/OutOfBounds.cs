using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfBounds : MonoBehaviour
{
    private GameObject playerPref;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;
    [SerializeField] private float minZ;
    [SerializeField] private float maxZ;



    private void Awake()
    {
        playerPref = FindObjectOfType<PlayerController>().gameObject;

    }


    // Update is called once per frame
    void Update()
    {
        if (playerPref.transform.position.x < minX)
        {
            playerPref.transform.position = new Vector3(minX, playerPref.transform.position.y, playerPref.transform.position.z);
        }
        if (playerPref.transform.position.x > maxX)
        {
            playerPref.transform.position = new Vector3(maxX, playerPref.transform.position.y, playerPref.transform.position.z);
        }
        if (playerPref.transform.position.z < minZ)
        {
            playerPref.transform.position = new Vector3(playerPref.transform.position.x, playerPref.transform.position.y, minZ);
        }
        if (playerPref.transform.position.z > maxZ)
        {
            playerPref.transform.position = new Vector3(playerPref.transform.position.x, playerPref.transform.position.y, maxZ);
        }
    }
}
