using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExperienceBarController : MonoBehaviour
{
    // Still viewable and setable in the inspector and is not visable to other classes.
    [SerializeField] ExperienceBar experienceBar;
    
    public void ExpSetup(Player player)
    {
        experienceBar.Setup(player);
    } 
}