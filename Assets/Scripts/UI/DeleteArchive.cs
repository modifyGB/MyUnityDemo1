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
            Utils.DeleteFile("Archive/" + archiveItem.Name + ".json");
            Utils.DeleteFile("Archive/" + archiveItem.Name + ".list.json");
            StartManager.I.ArchiveTable.Refresh();
        }
    }
}
