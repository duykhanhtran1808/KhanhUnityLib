using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] 
    float health = 100f;
    [SerializeField]
    TextMeshProUGUI txtHealth;

    Animator anim;
    void Start()
    {
        anim = GetComponent<Animator>();
      //  PlayerPrefs.SetFloat("PlayerHealth", health);
        ChangeHealthText();
    }

    private void ChangeHealthText()
    {
        txtHealth.text = "HP: " + PlayerPrefs.GetFloat("PlayerHealth", health);
    }

    public void IncreaseHealth(float amount)
    {
        this.health += amount;
        PlayerPrefs.SetFloat("PlayerHealth", health);
        ChangeHealthText();
    }

    public void ReduceHealth(float amount)
    {
        this.health -= amount;
        PlayerPrefs.SetFloat("PlayerHealth", health);
        ChangeHealthText();
        if(this.health <= 0)
        {
            PlayerPrefs.SetFloat("PlayerHealth", 0);
            txtHealth.text = "Game Over";
            GetComponent<ControlPlayer>().enabled = false;
            anim.Play("robotBoy_death");
            ObserverManager.Instance.OnPlayerDeath();
        }
    }
}
