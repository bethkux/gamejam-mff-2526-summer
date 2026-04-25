using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class NotificationManager : MonoBehaviour
{
    public List<GameObject> notifikace = new List<GameObject>();
    //public GameObject prefab;
    //public Canvas canvas;
    public GameObject imagePrefab;
    GameObject instancePrefabu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Random rng = new Random();

        float rngf = (float)rng.Next(1, 100) / 100;
        //Debug.Log(rngf);
        if (rngf > 0.90)
        {
            //Debug.Log(rngf);
            //instancePrefabu = Instantiate(prefab);
            //instancePrefabu.transform.localScale = new Vector3(rngf, rngf, rngf);
            //instancePrefabu.transform.SetParent(canvas.transform, false);
            //notifikace.Add(instancePrefabu);
            //notifikace.Add(
            Spawn();
        }

    }
    void Spawn()
    {
        // Walk up to find the Canvas parent OwO
        Canvas canvas = GetComponentInParent<Canvas>();

        if (canvas == null) return; // safety check~

        GameObject newImage = Instantiate(imagePrefab);
        newImage.transform.SetParent(canvas.transform, false);

        
    }
}
