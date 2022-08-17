using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace UI
{
    public class ArchiveItem : MyScript, IPointerDownHandler, IPointerUpHandler
    {
        public ArchiveTable archiveTable;
        private TextMeshProUGUI textArchive;
        private TextMeshProUGUI textSize;
        private TextMeshProUGUI textSeed;

        private string _name;
        public string Name { get { return _name; } set { _name = value; } }

        private void Awake()
        {
            textArchive = Utils.FindChildByName(gameObject, "TextArchive").GetComponent<TextMeshProUGUI>();
            textSize = Utils.FindChildByName(gameObject, "TextSize").GetComponent<TextMeshProUGUI>();
            textSeed = Utils.FindChildByName(gameObject, "TextSeed").GetComponent<TextMeshProUGUI>();
        }

        public void Initialization(ArchiveData archiveData, ArchiveTable archiveTable)
        {
            Name = archiveData.name;
            textArchive.text = archiveData.name;
            textSize.text = archiveData.width + " * " + archiveData.height;
            textSeed.text = Convert.ToString(archiveData.seed);
            this.archiveTable = archiveTable;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            archiveTable.isDragging = true;
            archiveTable.beginDraggingPoint = Input.mousePosition;
            archiveTable.beginObjectPos = archiveTable.ArchiveList.transform.localPosition;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            if (archiveTable.pointTimer < 0.1) 
                StartManager.I.LoadInitialization(Name);
            archiveTable.isDragging = false;
            archiveTable.pointTimer = 0;
        }
    }
}
