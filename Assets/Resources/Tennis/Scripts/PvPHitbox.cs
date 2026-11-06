using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PvPHitbox : MonoBehaviour
{
    public GameObject playerAvatar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(playerAvatar.transform.position.x, transform.position.y, transform.position.z);
    }
}
