using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;

public class MyScript : MonoBehaviour
{
    public float scale = 1;
    public UnityEvent superevent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void OnEnable()
    {
        scale = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        scale += 0.1f;
        gameObject.transform.localScale = new Vector3(scale, scale, scale);

        if (scale > 4)
        {
            superevent.Invoke();
        }
    }
}
