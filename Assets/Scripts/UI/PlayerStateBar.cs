using Manager;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI
{
    public class PlayerStateBar : MyScript
    {
        public GameObject nowBlood;
        public GameObject nowExperience;
        public TextMeshProUGUI bloodText;
        public TextMeshProUGUI levelNum;

        private void Awake()
        {
            nowBlood = Utils.FindChildByName(gameObject, "nowBlood");
            nowExperience = Utils.FindChildByName(gameObject, "nowExperience");
            bloodText = Utils.FindChildByName(gameObject, "bloodText").GetComponent<TextMeshProUGUI>();
            levelNum = Utils.FindChildByName(gameObject, "levelNum").GetComponent<TextMeshProUGUI>();

            PlayerManager.I.PlayerValue.ExperienceChange += ChangeHandler;
            PlayerManager.I.PlayerValue.BloodChange += ChangeHandler;
        }

        public void ChangeHandler()
        {
            var value = PlayerManager.I.PlayerValue;
            var allBlood = value.BaseBlood;
            nowBlood.transform.localScale = new Vector3(value.NowBlood * 1.0f / allBlood, 1, 1);
            nowBlood.transform.localPosition = new Vector3((allBlood - value.NowBlood) / allBlood * 200 * -0.5f, 0, 0);
            nowExperience.transform.localScale = new Vector3(value.NowExperience * 1.0f / value.UpExperience, 1, 1);
            nowExperience.transform.localPosition = new Vector3(
                (value.UpExperience - value.NowExperience) / value.UpExperience * 200 * -0.5f, 0, 0);
            bloodText.text = value.NowBlood.ToString() + "/" + allBlood.ToString();
            levelNum.text = value.Level.ToString();
        }
    }
}
