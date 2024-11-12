
using UnityEngine;
using UnityEngine.UI;
public class HealthBar : MonoBehaviour
{
    public Image fillBar;
    public float health;
    //100 health => 1 fi;; amount
    //45 health => 0.45 fill amount
    public void LoseHealth(int value)
    {
        //Do nothing if you are out of health
        if (health <= 0) 
        {
            return;
        }
        //Reduce the health
        health -= value;
        //Refresh the UI fillBar
        fillBar.fillAmount = health/100;
        //check if your health is zero or less => Dead
        if (health <= 0)
        {
            Debug.Log("YOU DIED");
        }
    }
   
}