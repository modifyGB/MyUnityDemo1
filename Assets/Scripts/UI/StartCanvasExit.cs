using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class StartCanvasExit : MyScript, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.I.buttonSource.Play();

            ExitGame();
        }

        public void ExitGame()
        {
            #if UNITY_EDITOR    //在编辑器模式下
                    EditorApplication.isPlaying = false;
            #else
                    Application.Quit();
            #endif
        }
    }
}
