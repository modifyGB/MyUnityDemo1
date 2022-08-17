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
            #if UNITY_EDITOR    //�ڱ༭��ģʽ��
                        EditorApplication.isPlaying = false;
            #else
                                Application.Quit();
            #endif
        }
    }
}
