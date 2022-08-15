using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Manager
{
    public class SoundSlide : MyScript, IPointerUpHandler, IPointerDownHandler
    {
        public AudioMixer audioMixer;

        private void Awake()
        {
            var slider = GetComponent<Slider>();
            audioMixer.GetFloat("MyExposedParam", out var value);
            slider.value = value;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            SoundManager.I.buttonSource.Play();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SoundManager.I.buttonSource.Play();
        }

        public void SetMixer(Slider s)
        {
            audioMixer.SetFloat("MyExposedParam", s.value);
        }
    }
}
