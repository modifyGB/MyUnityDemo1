using Manager;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class DeleteArchive : MyScript, IPointerClickHandler
    {
        public ArchiveItem archiveItem;

        public void OnPointerClick(PointerEventData eventData)
        {
            SoundManager.I.buttonSource.Play();
            Utils.DeleteDirectory("Archive/" + archiveItem.Name);
            StartManager.I.ArchiveTable.Refresh();
        }
    }
}
