using Manager;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI
{
    public class CreateArchive : MyScript, IPointerClickHandler
    {
        public TMP_InputField textName;
        public TMP_InputField textWidth;
        public TMP_InputField textHeight;
        public TMP_InputField textSeed;
        public TextMeshProUGUI textError;

        public void OnPointerClick(PointerEventData eventData)
        {
            string name;
            int width;
            int height;
            int seed;

            if (string.IsNullOrEmpty(textName.text))
            {
                textError.text = "Archive name can't be empty";
                return;
            }
            else if (StartManager.I.FileList.Contains(textName.text))
            {
                textError.text = "Archive already exists";
                return;
            }
            else
                name = textName.text;
            if (string.IsNullOrEmpty(textWidth.text))
                width = 100;
            else
                width = Convert.ToInt32(textWidth.text);
            if (string.IsNullOrEmpty(textHeight.text))
                height = 100;
            else
                height = Convert.ToInt32(textHeight.text);
            if (string.IsNullOrEmpty(textSeed.text))
                seed = UnityEngine.Random.Range(0, int.MaxValue);
            else
                seed = Convert.ToInt32(textSeed.text);

            var archiveData = new ArchiveData();
            archiveData.name = name;
            archiveData.width = width;
            archiveData.height = height;
            archiveData.seed = seed;
            StartManager.I.LoadInitialization(archiveData);
        }
    }
}
