using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

using UnityEngine.InputSystem;              // <-error
using UnityEngine.InputSystem.Controls;

public class AchievementHandler : MonoBehaviour
{

    public List<AchievementInternal> Achievments = new();
    int numOfClicks = 0;
    bool tmp = true;
    bool tmp2 = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Debug.Log("Clicked!");
            if (tmp2)
            {
                HandleAchievement(67);
                tmp2 = false;
            }

            numOfClicks++;
            Debug.Log(numOfClicks);
        }
        if (numOfClicks >= 5 && tmp)
        {
            //Debug.Log("numofclicksbig");
            HandleAchievement(20);
            tmp = false;
        }
    }
    public void HandleAchievement(int achID = 0)
    {
        Debug.Log("HandlerCalled");
        AchievementInternal toRemove = null ;
        
        foreach (var achiev in Achievments)
        {
            //if achiev.id == achID
            // achievements.check
            // delay
            // achiev.delete
            //AchievementInternal achievementInternal = achiev.GetComponent<AchievementInternal>();
            Debug.Log(achiev.ID);
            switch (achiev.ID)
            {
                case 20:
                    Debug.Log("removing 20 clucks");
                    toRemove = achiev;
                    achiev.Handle();
                    break;
                case 67:
                    toRemove = achiev;
                    achiev.Handle();
                    break;
                default:
                    Debug.Log("Tried to handle achievement number " +  achID + " but it does not exist");
                    break;
            }
            
        }
        Achievments.Remove(toRemove);
    }

    

}
