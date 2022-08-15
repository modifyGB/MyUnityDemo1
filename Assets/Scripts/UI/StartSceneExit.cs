using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class StartSceneExit : MyScript, IPointerClickHandler
    {
        public StartManager.StartState lastState = StartManager.StartState.StartCanvas;

        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.I.buttonSource.Play();

            StartManager.I.State = lastState;
        }
    }
}
