using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;


namespace oldgoldmine_game.Engine
{
    public static class AudioManager
    {

        private class SFX
        {
            SoundEffect effect;
            List<SoundEffectInstance> instances;

            public SFX(SoundEffect effect)
            {
                this.effect = effect;
                this.instances = new List<SoundEffectInstance>();
            }

            public SoundEffectInstance Play(bool looped, float volume, float pitch)
            {
                // Look among the available instances to see if one can play the desired sound

                for (int i = 0; i < instances.Count; ++i)
                {
                    SoundEffectInstance cur = instances[i];
                    if (cur.State != SoundState.Playing)
                    {
                        cur.Stop();
                        cur.Volume = volume;
                        cur.IsLooped = looped;
                        cur.Pitch = pitch;
                        cur.Play();

                        return cur;
                    }
                }

                // If no reusable instance was found, create a new one to play the sound

                SoundEffectInstance newInstance = effect.CreateInstance();
                instances.Add(newInstance);
                newInstance.Volume = volume;
                newInstance.IsLooped = looped;
                newInstance.Pitch = pitch;
                newInstance.Play();

                return newInstance;
            }

            public void Stop(bool immediate)
            {
                foreach (SoundEffectInstance instance in instances)
                {
                    instance.Stop(immediate);
                }
            }
        }


        static Dictionary<string, Song> soundtrack = new Dictionary<string, Song>();
        static int curSongIndex = 0;
        
        static Dictionary<string, SFX> soundEffects = new Dictionary<string, SFX>();

        public static void SetVolume(int masterVolume, int musicVolume, int effectsVolume)
        {
            SoundEffect.MasterVolume = (effectsVolume / 100f) * (masterVolume / 100f);
            MediaPlayer.Volume = (musicVolume / 100f) * (masterVolume / 100f);
        }

        // Music

        public static void AddSong(string songName, Song song)
        {
            if (!soundtrack.ContainsKey(songName))
            {
                soundtrack.Add(songName, song);
            }
            else
            {
                Console.WriteLine("The soundtrack already contains a Song named '" +
                    songName + "', operation cancelled");
            }
        }

        public static void RemoveSong(string songName)
        {
            if (!soundtrack.Remove(songName))
            {
                Console.WriteLine("No Song named '" + songName +
                    "' was found in the soundtrack, operation cancelled");
            }
        }


        public static string ActiveSong
        {
            get { return string.Empty; }
            set { PlaySong(value); }
        }


        public static void PlaySong(string songName, bool loop = false)
        {
            Song song;
            if (soundtrack.TryGetValue(songName, out song))
            {
                MediaPlayer.IsRepeating = loop;
                MediaPlayer.Play(song);
            }
            else
            {
                Console.WriteLine("No Song named '" + songName +
                    "' was found in the soundtrack, operation cancelled");
            }
        }

        public static void NextSong()
        {
            // TODO: organize songs in an ordered list so that the player can go to the next one
            // (by TrackNumber and song Name)
        }

        public static void PreviousSong()
        {
            // TODO: organize songs in an ordered list so that the player can go to the previous one
            // (by TrackNumber and song Name)
        }


        public static void PauseMusic()
        {
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
        }

        public static void ResumeMusic()
        {
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
        }

        public static void StopMusic()
        {
            MediaPlayer.Stop();
        }


        // Sound Effects

        public static void AddSoundEffect(string effectName, SoundEffect effect)
        {
            SFX soundEffect = new SFX(effect);

            if (!soundEffects.ContainsKey(effectName))
            {
                soundEffects.Add(effectName, soundEffect);
            }
            else
            {
                Console.WriteLine("The sound collection already contains a SoundEffect named '" +
                    effectName + "', operation cancelled");
            }
        }

        public static void RemoveSoundEffect(string effectName)
        {
            if (!soundEffects.Remove(effectName))
            {
                Console.WriteLine("No SoundEffect named '" + effectName +
                    "' was found in the effects collection, operation cancelled");
            }
        }


        public static SoundEffectInstance PlaySoundEffect(string effectName, 
            bool loop = false, float volume = 1f, float pitch = 0f)
        {
            SFX effect;

            if (soundEffects.TryGetValue(effectName, out effect))
            {
                return effect.Play(loop, volume, pitch);
            }
            else
            {
                Console.WriteLine("No SoundEffect named '" + effectName +
                    "' was found in the effects collection, operation cancelled");

                return null;
            }
        }

        public static void PlaySpatialSound(string effectName, Vector3 position, Vector3 velocity,
            bool loop = false, float volume = 1f, float pitch = 0f)
        {
            SFX effect;

            if (soundEffects.TryGetValue(effectName, out effect))
            {
                // TODO: play effect instance with AudioEmitter
            }
            else
            {
                Console.WriteLine("No SoundEffect named '" + effectName +
                    "' was found in the effects collection, operation cancelled");
            }
        }

        public static void StopEffectsByName(string effectName, bool immediate = true)
        {
            SFX effect;

            if (soundEffects.TryGetValue(effectName, out effect))
            {
                effect.Stop(immediate);
            }
            else
            {
                Console.WriteLine("No SoundEffect named '" + effectName +
                    "' was found in the effects collection, operation cancelled");
            }
        }

        public static void StopAllEffects(bool immediate = true)
        {
            foreach (SFX effect in soundEffects.Values)
            {
                effect.Stop(immediate);
            }
        }

    }
}
