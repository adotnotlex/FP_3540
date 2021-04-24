using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAxe : MonoBehaviour
{
    private int destroyDuration = 3;

    private Transform child;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.childCount > 0)
        {
            child = transform.GetChild(0);
            Destroy(child.gameObject, destroyDuration);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.childCount > 0)
        {
            child = transform.GetChild(0);
            Destroy(child.gameObject, destroyDuration);
        }
        
    }
}
