using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class StartCanvasSetting : MyScript, IPointerClickHandler
    {
        public void OnPointerClick(PointerEventData eventData)
        {
            StartManager.I.State = StartManager.StartState.SettingTable;
        }
    }
}