using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CreateArchiveButton : MyScript, IPointerDownHandler, IPointerUpHandler
    {
        public ArchiveTable archiveTable;

        public void OnPointerDown(PointerEventData eventData)
        {
            archiveTable.isDragging = true;
            archiveTable.beginDraggingPoint = Input.mousePosition;
            archiveTable.beginObjectPos = archiveTable.ArchiveList.transform.localPosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (archiveTable.pointTimer < 0.1)
            {
                SoundManager.I.buttonSource.Play();
                StartManager.I.State = StartManager.StartState.CreateArchive;
                return;
            }
            archiveTable.isDragging = false;
            archiveTable.pointTimer = 0;
        }

        public void Initialization(ArchiveTable archiveTable)
        {
            this.archiveTable = archiveTable;
        }
    }
}
