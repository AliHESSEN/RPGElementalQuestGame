using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camerControll : MonoBehaviour
{
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // gj�r slik at kamera f�lger player

        transform.position = new Vector3(target.transform.position.x, target.transform.position.y, transform.position.z); // kj�rer z-aksen uten target, slik at holder seg p� samme verdi
    }
}
