using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    Transform actorTransfrom;
    void Start()
    {
        actorTransfrom = FindAnyObjectByType<Actor>().transform;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(actorTransfrom.position.x, 8, actorTransfrom.position.z);
    }
}
