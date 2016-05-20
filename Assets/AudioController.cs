using System.Collections;
using Assets.CustomAssets.Scripts.CustomInput;
using UnityEngine;

namespace Assets {
    public class AudioController : MonoBehaviour {

        public AudioSource wind_loop { get; private set; }
        public AudioSource music_1 { get; private set; }
        public AudioSource music_2 { get; private set; }
        public AudioSource music_3 { get; private set; }
        public AudioSource music_perc { get; private set; }
        private IEnumerator coroutine_music_1;
        private IEnumerator coroutine_music_2;
        private IEnumerator coroutine_music_3;
        private IEnumerator coroutine_music_perc;
        private IEnumerator coroutine_cart_loop;

        public AudioSource bell { get; private set; }
        public AudioSource cart_loop { get; private set; }
        public AudioSource ending_door { get; private set; }
        public readonly AudioSource[] fsteps = new AudioSource[5];
        public AudioSource burnt_house       {get; private set;}
        public AudioSource pickup_coffin     {get; private set;}
        public readonly AudioSource[] shovel = new AudioSource[3];
        public AudioSource throw_coffin { get; private set; }
        public readonly AudioSource[] tone = new AudioSource[6];
        public AudioSource crumbling { get; private set; }

        public void Awake() {
            bell = gameObject.AddComponent<AudioSource>();
            bell.clip = Resources.Load<AudioClip>("Audio/BELL");
            bell.volume = 1f;

            cart_loop = gameObject.AddComponent<AudioSource>();
            cart_loop.clip = Resources.Load<AudioClip>("Audio/cart loop");
            cart_loop.volume = 0f;
            cart_loop.loop = true;

            ending_door = gameObject.AddComponent<AudioSource>();
            ending_door.clip = Resources.Load<AudioClip>("Audio/ending door");
            ending_door.volume = 0f;

            fsteps[0] = gameObject.AddComponent<AudioSource>();
            fsteps[0].clip = Resources.Load<AudioClip>("Audio/fsteps1");
            fsteps[1] = gameObject.AddComponent<AudioSource>();
            fsteps[1].clip = Resources.Load<AudioClip>("Audio/fsteps2");
            fsteps[2] = gameObject.AddComponent<AudioSource>();
            fsteps[2].clip = Resources.Load<AudioClip>("Audio/fsteps3");
            fsteps[3] = gameObject.AddComponent<AudioSource>();
            fsteps[3].clip = Resources.Load<AudioClip>("Audio/fsteps4");
            fsteps[4] = gameObject.AddComponent<AudioSource>();
            fsteps[4].clip = Resources.Load<AudioClip>("Audio/fsteps5");
            foreach (var x in fsteps) x.volume = .1f;

            burnt_house = gameObject.AddComponent<AudioSource>();
            burnt_house.clip = Resources.Load<AudioClip>("Audio/LOOP burning house");
            burnt_house.volume = 0f;
            burnt_house.loop = true;

            wind_loop = gameObject.AddComponent<AudioSource>();
            wind_loop.clip = Resources.Load<AudioClip>("Audio/loop wind");
            wind_loop.volume = 0f;
            wind_loop.loop = true;

            music_1 = gameObject.AddComponent<AudioSource>();
            music_1.clip = Resources.Load<AudioClip>("Audio/music_1");
            music_1.volume = 0f;
            music_1.loop = true;
            music_2 = gameObject.AddComponent<AudioSource>();
            music_2.clip = Resources.Load<AudioClip>("Audio/music_2");
            music_2.volume = 0f;
            music_2.loop = true;
            music_3 = gameObject.AddComponent<AudioSource>();
            music_3.clip = Resources.Load<AudioClip>("Audio/music_3");
            music_3.volume = 0f;
            music_3.loop = true;
            music_perc = gameObject.AddComponent<AudioSource>();
            music_perc.clip = Resources.Load<AudioClip>("Audio/music_perc");
            music_perc.volume = 0f;
            music_perc.loop = true;

            pickup_coffin = gameObject.AddComponent<AudioSource>();
            pickup_coffin.clip = Resources.Load<AudioClip>("Audio/pickup coffin");
            pickup_coffin.volume = 0f;

            shovel[0] = gameObject.AddComponent<AudioSource>();
            shovel[0].clip = Resources.Load<AudioClip>("Audio/shovel1");
            shovel[1] = gameObject.AddComponent<AudioSource>();
            shovel[1].clip = Resources.Load<AudioClip>("Audio/shovel2");
            shovel[2] = gameObject.AddComponent<AudioSource>();
            shovel[2].clip = Resources.Load<AudioClip>("Audio/shovel3");
            foreach (var x in shovel) x.volume = .50f;

            throw_coffin = gameObject.AddComponent<AudioSource>();
            throw_coffin.clip = Resources.Load<AudioClip>("Audio/throw coffin");
            throw_coffin.volume = .50f;

            tone[0] = gameObject.AddComponent<AudioSource>();
            tone[0].clip = Resources.Load<AudioClip>("Audio/tone1");
            tone[1] = gameObject.AddComponent<AudioSource>();
            tone[1].clip = Resources.Load<AudioClip>("Audio/tone2");
            tone[2] = gameObject.AddComponent<AudioSource>();
            tone[2].clip = Resources.Load<AudioClip>("Audio/tone3");
            tone[3] = gameObject.AddComponent<AudioSource>();
            tone[3].clip = Resources.Load<AudioClip>("Audio/tone4");
            tone[4] = gameObject.AddComponent<AudioSource>();
            tone[4].clip = Resources.Load<AudioClip>("Audio/tone5");
            tone[5] = gameObject.AddComponent<AudioSource>();
            tone[5].clip = Resources.Load<AudioClip>("Audio/tone6");
            foreach (var x in tone) x.volume = 1f;

            crumbling = gameObject.AddComponent<AudioSource>();
            crumbling.clip = Resources.Load<AudioClip>("Audio/crumbling");
        }

        public void Start() {
            wind_loop.Play();
            music_1.Play();
            music_2.Play();
            music_3.Play();
            music_perc.Play();
            cart_loop.Play();
            burnt_house.Play();

            wind_loop.volume = 1f;
        }

        public void enter_music_1() {
            addChannel(coroutine_music_1, music_1);
        }

        public void enter_music_2() {
            addChannel(coroutine_music_2, music_2);
        }

        public void enter_music_3() {
            addChannel(coroutine_music_3, music_3);
        }

        public void enter_music_perc() {
            addChannel(coroutine_music_perc, music_perc);
        }
        public void exit_music_perc() {
            removeChannel(coroutine_music_perc, music_perc);
        }

        public void bellDing() {
            bell.Play();
        }

        public void playTone() {
            int i = Random.Range(0, tone.Length-1);
            if (!tone[i].isPlaying)
                tone[i].Play();
        }

        public void playDig() {
            /*
            int i = Random.Range(0, shovel.Length-1);
            if (!shovel[i].isPlaying)
                shovel[i].Play();
            */
            for (int i = 0; i < shovel.Length; ++i) {
                if (!shovel[i].isPlaying) {
                    shovel[i].Play();
                    return;
                }
            }
        }

        public void playFootsteps() {
            int i = Random.Range(0, fsteps.Length - 1);
            if (!fsteps[i].isPlaying)
                fsteps[i].Play();
        }

        public void enter_cart() {
            addChannel(coroutine_cart_loop, cart_loop, .08f);
        }
        public void exit_cart() {
            removeChannel(coroutine_cart_loop, cart_loop, .08f);
        }

        private void addChannel(IEnumerator coroutine, AudioSource channel, float step = 0.016f) {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = addChannelCo(channel, step);
            StartCoroutine(coroutine);
        }
        private void removeChannel(IEnumerator coroutine, AudioSource channel, float step = 0.016f) {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = removeChannelCo(channel, step);
            StartCoroutine(coroutine);
        }

        private IEnumerator addChannelCo(AudioSource channel, float step = 0.016f) {
            while (channel.volume < 1.0f) {
                channel.volume += step;
                yield return new WaitForSeconds(.25f);
            }
            channel.volume = 1.0f;
        }

        private IEnumerator removeChannelCo(AudioSource channel, float step = 0.016f) {
            while (channel.volume > 0.0f) {
                channel.volume -= step;
                yield return new WaitForSeconds(.25f);
            }
            channel.volume = 0.0f;
        }

    }
}
