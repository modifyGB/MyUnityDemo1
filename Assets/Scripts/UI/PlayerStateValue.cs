using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerStateValue : MyScript
    {
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI attackText;
        public TextMeshProUGUI defenceText;
        public TextMeshProUGUI bloodText;
        public TextMeshProUGUI experienceText;

        private void Awake()
        {
            levelText = Utils.FindChildByName(gameObject, "levelText").GetComponent<TextMeshProUGUI>();
            attackText = Utils.FindChildByName(gameObject, "attackText").GetComponent<TextMeshProUGUI>();
            defenceText = Utils.FindChildByName(gameObject, "defenceText").GetComponent<TextMeshProUGUI>();
            bloodText = Utils.FindChildByName(gameObject, "bloodText").GetComponent<TextMeshProUGUI>();
            experienceText = Utils.FindChildByName(gameObject, "experienceText").GetComponent<TextMeshProUGUI>();

            PlayerManager.I.PlayerValue.ExperienceChange += ChangeHandler;
            PlayerManager.I.PlayerValue.BloodChange += ChangeHandler;
        }

        public void ChangeHandler()
        {
            var value = PlayerManager.I.PlayerValue;
            var allBlood = value.BaseBlood;
            var allAttack = value.BaseAttack;
            var allDefence = value.BaseDefence;
            if (PlayerManager.I.Weapon != null)
                allAttack += PlayerManager.I.Weapon.WeaponSO.attack;

            levelText.text = value.Level.ToString();
            attackText.text = allAttack.ToString();
            defenceText.text = allDefence.ToString();
            bloodText.text = value.NowBlood.ToString() + "/" + allBlood.ToString();
            experienceText.text = value.NowExperience.ToString() + "/" + value.UpExperience.ToString();
        }
    }
}
