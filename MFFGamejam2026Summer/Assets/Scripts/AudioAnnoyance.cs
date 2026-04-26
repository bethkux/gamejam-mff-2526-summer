using UnityEngine;

public class AudioAnnoyance : MonoBehaviour
{
    public string SFX = "Error";
    public float delayForNext = 0.5f;
    public AchievementHandler AchievementHandler;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        AudioManager.Instance.PlaySFX(SFX);
        
    }
    private void OnEnable()
    {
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
