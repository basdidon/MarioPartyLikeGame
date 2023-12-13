using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    void Start()
    {
        transform.position = NodeRegistry.Instance.startNode.transform.position;
    }

}
