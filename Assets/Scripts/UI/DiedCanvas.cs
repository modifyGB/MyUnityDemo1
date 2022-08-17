using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class DiedCanvas : MyScript
    {

        public void BackTitle()
        {
            SceneManager.LoadScene(0);
        }

        public void Exit()
        {
            #if UNITY_EDITOR    //在编辑器模式下
                        EditorApplication.isPlaying = false;
            #else
                                Application.Quit();
            #endif
        }
    }
}
