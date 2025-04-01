using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBar : MonoBehaviour
{
    Character character;

    public Text text;

    public Image TertiaryBar;
    public GameObject TertiaryParent;

    public Image staminabar;
    public Image staminaDelayBar;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponentInParent<Character>();
    }

    // Update is called once per frame
    void Update()
    {
        if (character != null)
        {
            text.text = character.myName;
            staminabar.fillAmount = character.characterStamina.Stamina / character.characterStamina.MaxStamina;
        }

        TertiaryParent.SetActive(false); //unused for now
    }
}
