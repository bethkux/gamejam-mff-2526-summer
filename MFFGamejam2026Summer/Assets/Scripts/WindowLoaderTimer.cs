using System.Collections.Generic;
using UnityEngine;

public class WindowLoaderTimer : MonoBehaviour
{

    public List<GameObject> windowsToLoad = new List<GameObject>();
    public float defaulutDelay = 0.5f;
    float timer = 0;
    int windowPointer = 0;
    private float delay = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = 0;
        delay = defaulutDelay;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= delay)
        {
            //spawn window
            if (windowPointer < windowsToLoad.Count)
            {
                windowsToLoad[windowPointer].SetActive(true);
                AudioAnnoyance aa = windowsToLoad[windowPointer].GetComponent<AudioAnnoyance>();
                if (aa != null)
                {
                    delay = aa.delayForNext;
                }
                else
                {
                    delay = defaulutDelay;
                }
                windowPointer++;
            }
            timer = 0;
        }
    }
}
