using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CloseButton : MyScript, IPointerUpHandler
    {

        private void Awake()
        {
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            UIManager.I.UIState = UIState.Play;

            SoundManager.I.buttonSource.Play();
        }
    }
}
