using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvPCameraScript : MonoBehaviour
{
    public GameObject playerAvatar;
    private float playerS;
    private float distX;
    private float distY;
    private float distZ;
    // Start is called before the first frame update
    void Start()
    {
        // dist = 3.4f;
        playerS = GameObject.Find(playerAvatar.name).GetComponent<PvpPlayerController>().player;
        distZ = 5f;
        distZ = distZ * 2f * (playerS - 1) - distZ;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerAvatar.transform.position.x, transform.position.y, playerAvatar.transform.position.z + distZ);
    }
}
