using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD_Weapon_Panel : MonoBehaviour {

    [SerializeField] private Text weaponNameText;
    [SerializeField] private Text resourceNameText;

    [SerializeField] private Slider resourceSlider;
    [SerializeField] private Image sliderFill;

    [SerializeField] private Color weaponAllowedColour;
    [SerializeField] private Color weaponDisallowedColour;

    [SerializeField] private Image weaponIcon;

    public void SetValues (RobotWeaponData weaponData)
    {
        weaponNameText.text = weaponData.weaponName;
        resourceNameText.text = "(" + weaponData.resourceType.ToString () + ")";
        weaponIcon.sprite = weaponData.sprite;
    }

    public void UpdateResourceValue(float value, float maxValue)
    {
        resourceSlider.value = Mathf.Lerp ( 0.0f, 1.0f, value / maxValue );
    }

    public void SetSliderState(bool canUseWeapon)
    {
        if (canUseWeapon)
        {
            sliderFill.color = weaponAllowedColour;
        }
        else
        {
            sliderFill.color = weaponDisallowedColour;
        }
    }
}
