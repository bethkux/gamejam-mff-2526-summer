using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AchievementInternal : MonoBehaviour
{
    Toggle toggle;
    public int ID;
    float timeToDie = 0;
    bool die = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggle = GetComponentInChildren<Toggle>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

    private IEnumerator Kill()
    {
        float delay = 5f;
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
    public void Handle()
    {
        toggle.isOn = true;
        StartCoroutine(Kill());
    }
}
