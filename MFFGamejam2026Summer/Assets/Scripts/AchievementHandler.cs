using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;



public class AchievementHandler : MonoBehaviour, IPointerClickHandler
{

    public List<GameObject> Achievments = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HandleAchievement(int achID = 0)
    {
        foreach (var achiev in Achievments)
        {
            //if achiev.id == achID
            // achievements.check
            // delay
            // achiev.delete
            AchievementInternal achievementInternal = achiev.GetComponent<AchievementInternal>();
            if (achievementInternal != null)
            {
                achievementInternal.Handle();

            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("AAAAAAAAAAAAAA");
    }

}
