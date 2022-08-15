using Manager;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LoadCanvas : MyScript
    {
        private TextMeshProUGUI loadText;
        private TextMeshProUGUI behaviourText;
        private GameObject load;

        private void Awake()
        {
            loadText = Utils.FindChildByName(gameObject, "LoadText").GetComponent<TextMeshProUGUI>();
            behaviourText = Utils.FindChildByName(gameObject, "BehaviourText").GetComponent<TextMeshProUGUI>();
            load = Utils.FindChildByName(gameObject, "Load");
        }

        public void Initialization(ArchiveData archiveData)
        {
            new Thread(new ParameterizedThreadStart(World.CreateWorld)).Start(archiveData);
            StartCoroutine(CreateArchive());
        }

        public void Initialization(string archiveName)
        {
            DontDestroyOnLoad(StartManager.I);
            World.archiveName = archiveName;
            StartCoroutine(LoadScene());
        }

        IEnumerator LoadScene()
        {
            behaviourText.text = "Load Map";
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(1);
            while (!asyncLoad.isDone)
            {
                load.transform.localScale = new Vector3(asyncLoad.progress, 1, 1);
                load.transform.localPosition = new Vector3((1 - asyncLoad.progress) * 400f * -0.5f, 0, 0);
                loadText.text = (asyncLoad.progress * 100).ToString("f2") + "%";
                yield return null;
            }
        }

        IEnumerator CreateArchive()
        {
            while (World.loadPer != 1)
            {
                load.transform.localScale = new Vector3((float)World.loadPer, 1, 1);
                load.transform.localPosition = new Vector3((1 - (float)World.loadPer) * 400f * -0.5f, 0, 0);
                loadText.text = (World.loadPer * 100).ToString("f2") + "%";

                if (World.loadPer < World.stepInit)
                    behaviourText.text = "World Initialization";
                else if (World.loadPer < World.stepInit + World.stepGround)
                    behaviourText.text = "Create Environment";
                else if (World.loadPer < World.stepInit + World.stepGround + World.stepPlace)
                    behaviourText.text = "Create Place";
                else if (World.loadPer < World.stepInit + World.stepGround + World.stepPlace + World.stepEnemy)
                    behaviourText.text = "Create Enemy";
                else
                    behaviourText.text = "Verify and Save Resources";
                yield return null;
            }
            StartManager.I.State = StartManager.StartState.ArchiveTable;
        }
    }
}
