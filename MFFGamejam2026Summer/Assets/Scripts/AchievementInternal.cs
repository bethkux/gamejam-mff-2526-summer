using UnityEngine;
using UnityEngine.UI;

public class AchievementInternal : MonoBehaviour
{
    Toggle toggle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        toggle = GetComponentInChildren<Toggle>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    


    public void Handle()
    {
        toggle.isOn = true;
    }
}
