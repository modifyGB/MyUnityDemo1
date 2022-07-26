using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class MakeTableButton : MyScript, IPointerDownHandler
    {
        public Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UIManager.I.UILeftState = UILeftState.Make;

            SoundManager.I.buttonSource.Play();
        }
    }
}
