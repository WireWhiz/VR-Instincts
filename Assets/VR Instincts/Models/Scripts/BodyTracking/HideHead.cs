using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideHead : MonoBehaviour
{
    public Camera camera;
    public GameObject sphere;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (camera == Camera.main)
        {
            sphere.SetActive(true);
            transform.localScale= new Vector3(0, 0, 0);
        }
    }
}
