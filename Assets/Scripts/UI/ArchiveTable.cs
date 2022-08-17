using Manager;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class ArchiveTable : MyScript
    {
        private Transform archiveList;
        public Transform ArchiveList { get { return archiveList; } }
        public Vector3 leftPos = new Vector3(-265, 0, 0);
        public Vector3 rightPos = new Vector3(-265, 0, 0);

        [HideInInspector]
        public bool isDragging;
        [HideInInspector]
        public Vector3 beginDraggingPoint;
        [HideInInspector]
        public Vector3 beginObjectPos;
        [HideInInspector]
        public float pointTimer = 0;

        private void Awake()
        {
            archiveList = Utils.FindChildByName(gameObject, "ArchiveList").transform;
            Refresh();
        }

        private void Update()
        {
            if (isDragging)
            {
                var nowDraggingPoint = Input.mousePosition;
                ArchiveList.localPosition = new Vector3(beginObjectPos.x
                    + nowDraggingPoint.x - beginDraggingPoint.x, 0, 0);
            }
            else
            {
                if (archiveList.transform.localPosition.x < leftPos.x)
                    archiveList.transform.localPosition = Vector3.Lerp(archiveList.transform.localPosition, leftPos, 0.1f);
                else if (archiveList.transform.localPosition.x > rightPos.x)
                    ArchiveList.transform.localPosition = Vector3.Lerp(archiveList.transform.localPosition, rightPos, 0.1f);
            }
        }

        private void FixedUpdate()
        {
            if (isDragging)
                pointTimer += Time.fixedDeltaTime;
        }

        public void Refresh()
        {
            var right = -520;
            StartManager.I.UpdateArchiveList();
            Utils.ClearChilds(archiveList.gameObject);
            foreach (var file in StartManager.I.FileList)
            {
                var newObject = Instantiate(StartManager.I.archiveItemPrefab);
                newObject.Initialization(Utils.LoadObject<ArchiveData>("Archive/" + file + "/archiveData.json"), this);
                newObject.transform.SetParent(archiveList, false);
                right += 210;
            }
            var newObject1 = Instantiate(StartManager.I.createArchiveButtonPrefab);
            newObject1.Initialization(this);
            newObject1.transform.SetParent(archiveList, false);
            leftPos = new Vector3(-Mathf.Clamp(right, 0, float.MaxValue) - 265, 0, 0);
        }
    }
}
