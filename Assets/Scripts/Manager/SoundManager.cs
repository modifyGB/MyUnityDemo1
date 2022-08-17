using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Manager
{
    public class SoundManager : Singleton<SoundManager>
    {

        public List<AudioClip> walkSounds = new List<AudioClip>();
        public List<AudioClip> runSounds = new List<AudioClip>();
        public List<AudioClip> backgroundSounds = new List<AudioClip>();
        public List<AudioClip> placeSounds = new List<AudioClip>();
        public List<AudioClip> otherSounds = new List<AudioClip>();
        public AudioSource buttonSource;
        public AudioSource moveSource;
        public AudioSource placeSource;
        public AudioSource backgroundSource;
        public AudioSource otherSource;

        private System.Random random = new System.Random();

        public override void Awake()
        {
            base.Awake();
        }

        public void Walk()
        {
            var num = random.Next(0, walkSounds.Count);
            moveSource.clip = walkSounds[num];
            moveSource.Play();
        }

        public void Run()
        {
            var num = random.Next(0, runSounds.Count);
            moveSource.clip = runSounds[num];
            moveSource.Play();
        }

        public void Place(int num)
        {
            placeSource.clip = placeSounds[num];
            placeSource.Play();
        }

        public void Attack(int num)
        {
            otherSource.clip = otherSounds[num];
            otherSource.Play();
        }

        public void Pick()
        {
            otherSource.clip = otherSounds[2];
            otherSource.Play();
        }

        public void ChangeBackground(int num)
        {
            backgroundSource.clip = backgroundSounds[num];
            backgroundSource.Play();
        }
    }
}
