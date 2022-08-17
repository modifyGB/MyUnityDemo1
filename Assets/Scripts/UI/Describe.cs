using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Describe : MyScript, ICanvasRaycastFilter
    {
        private Text Name;
        private Text Text;

        private void Awake()
        {
            Name = Utils.FindChildByName(gameObject, "Name").GetComponent<Text>();
            Text = Utils.FindChildByName(gameObject, "Text").GetComponent<Text>();
        }

        public void Initialization(string name, string text)
        {
            Name.text = name;
            Text.text = text;
        }

        public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera)
        {
            return false;
        }
    }
}
