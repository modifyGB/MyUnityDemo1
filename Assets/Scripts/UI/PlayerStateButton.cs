using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class PlayerStateButton : MyScript, IPointerDownHandler
    {
        public Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            UIManager.I.UILeftState = UILeftState.State;

            SoundManager.I.buttonSource.Play();
        }
    }
}
