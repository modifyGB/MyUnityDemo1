using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Manager
{
    public enum SettingState { Sound, Button, System }

    public class SettingTable : MyScript
    {
        public Button soundButton;
        public Button buttonButton;
        public Button systemButton;
        public GameObject sound;
        public GameObject button;
        public GameObject system;

        private SettingState state = SettingState.Sound;
        public SettingState State
        {
            get { return state; }
            set
            {
                state = value;

                if (value == SettingState.Sound)
                {
                    soundButton.interactable = false;
                    buttonButton.interactable = true;
                    if (systemButton != null)
                        systemButton.interactable = true;
                    sound.SetActive(true);
                    button.SetActive(false);
                    if (system != null)
                        system.SetActive(false);
                }
                else if (value == SettingState.Button)
                {
                    soundButton.interactable = true;
                    buttonButton.interactable = false;
                    if (systemButton != null)
                        systemButton.interactable = true;
                    sound.SetActive(false);
                    button.SetActive(true);
                    if (system != null)
                        system.SetActive(false);
                }
                else if (value == SettingState.System)
                {
                    soundButton.interactable = true;
                    buttonButton.interactable = true;
                    systemButton.interactable = false;
                    sound.SetActive(false);
                    button.SetActive(false);
                    system.SetActive(true);
                }
            }
        }

        private void Awake()
        {
            State = SettingState.Sound;
        }

        public void SetSettingState(int settingState)
        {
            State = (SettingState)settingState;
            SoundManager.I.buttonSource.Play();
        }
    }
}
