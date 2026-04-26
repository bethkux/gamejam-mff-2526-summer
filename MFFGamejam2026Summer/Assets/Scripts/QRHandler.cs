using UnityEngine;
using UnityEngine.UI;

public class QRHandler : MonoBehaviour
{
    public Button YesButton;
    public Button NoButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        YesButton.onClick.AddListener(stuff);
        NoButton.onClick.AddListener(stuff);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void stuff()
    {
        Debug.Log("QR button was pressed");
        Destroy(gameObject);
    }

}
