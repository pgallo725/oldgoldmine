using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace OldGoldMine.Engine
{
    /// <summary>
    /// Static class responsible for handling the storage and playback of
    /// all music and sound effects in the game, with volume control.
    /// </summary>
    public static class AudioManager
    {
        /* CUSTOM SOUND EFFECT CLASS */
        private class SFX
        {
            private readonly SoundEffect effect;
            private readonly List<SoundEffectInstance> instances;

            /// <summary>
            /// Create a playable SFX object for the specified SoundEffect
            /// </summary>
            public SFX(SoundEffect effect)
            {
                this.effect = effect;
                this.instances = new List<SoundEffectInstance>();
            }

            /// <summary>
            /// Play back the SoundEffect on a dedicated instance.
            /// </summary>
            /// <param name="looped">Whether the sound effect should keep repeating after playback.</param>
            /// <param name="volume">Volume of the sound effect, in the range [0, 1].</param>
            /// <param name="pitch">Pitch adjustment of the sound effect, in the range [-1, 1].</param>
            /// <returns>The SoundEffectInstance playing the requested sound effect.</returns>
            public SoundEffectInstance Play(bool looped, float volume, float pitch)
            {
                // Look among the available instances to see if one can play the sound
                foreach (SoundEffectInstance instance in instances)
                {
                    if (instance.State != SoundState.Playing)
                    {
                        instance.Stop();
                        instance.Volume = volume;
                        instance.IsLooped = looped;
                        instance.Pitch = pitch;
                        instance.Play();

                        return instance;
                    }
                }

                // If no available instance was found, create a new one to play the sound
                SoundEffectInstance newInstance = effect.CreateInstance();
                instances.Add(newInstance);
                newInstance.Volume = volume;
                newInstance.IsLooped = looped;
                newInstance.Pitch = pitch;
                newInstance.Play();

                return newInstance;
            }

            /// <summary>
            /// Stop the playback of all current instances of this SoundEffect.
            /// </summary>
            /// <param name="immediate">Whether the sound has to be stopped abruptedly or
            /// is allowed to play it's release phase (e.g. fade-out).</param>
            public void Stop(bool immediate)
            {
                foreach (SoundEffectInstance instance in instances)
                {
                    instance.Stop(immediate);
                }
            }
        }


        // MUSIC AND SFX COLLECTIONS
        private static readonly Dictionary<string, SFX> soundEffects = new Dictionary<string, SFX>();
        private static readonly Dictionary<string, Song> soundtrack = new Dictionary<string, Song>();

        // FADE IN/OUT EFFECT PARAMETERS
        private static bool fadeout = false;
        private static float fadeSpeed = 0f;

        // VOLUME
        private static float MediaPlayerTargetVolume = 0f;


        /// <summary>
        /// Set the desired volume levels for each sound type in the game.
        /// </summary>
        /// <param name="masterVolume">Global volume level of the application, in the range [0, 100].</param>
        /// <param name="musicVolume">Volume level of soundtrack items, in the range [0, 100].</param>
        /// <param name="effectsVolume">Volume level of sound effects, in the range [0, 100].</param>
        public static void SetVolume(int masterVolume, int musicVolume, int effectsVolume)
        {
            SoundEffect.MasterVolume = (effectsVolume / 100f) * (masterVolume / 100f);
            MediaPlayer.Volume = (musicVolume / 100f) * (masterVolume / 100f);

            // Store the target volume to restore it after fade-out effects
            MediaPlayerTargetVolume = MediaPlayer.Volume;
        }

        /// <summary>
        /// Update the internal status of the AudioManager over time.
        /// </summary>
        public static void Update(GameTime gameTime)
        {
            if (fadeout)
            {
                MediaPlayer.Volume -= fadeSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (MediaPlayer.Volume <= 0.0f)
                {
                    StopMusic();
                    fadeout = false;
                }
            }
        }


        /* MUSIC */

        /// <summary>
        /// Add the provided Song to the soundtrack library.
        /// </summary>
        /// <param name="songName">The name used to identify the Song.</param>
        /// <param name="song">The Song object that has to be added.</param>
        public static void AddSong(string songName, Song song)
        {
            if (!soundtrack.TryAdd(songName, song))
            {
                Console.WriteLine($"The soundtrack already contains a Song named '{songName}'");
            }
        }

        /// <summary>
        /// Remove the requested Song from the soundtrack library.
        /// </summary>
        /// <param name="songName">Name of the Song that has to be removed.</param>
        public static void RemoveSong(string songName)
        {
            if (!soundtrack.Remove(songName))
            {
                Console.WriteLine($"The soundtrack doesn't contain any Song named '{songName}'");
            }
        }

        /// <summary>
        /// Start playback of the specified Song, which must have previously added to the soundtrack using AddSong().
        /// </summary>
        /// <param name="songName">The name of the Song that has to be played.</param>
        /// <param name="loop">Whether the current song has to be repeated after it ends (default = false).</param>
        public static void PlaySong(string songName, bool loop = false)
        {
            if (soundtrack.TryGetValue(songName, out Song song))
            {
                fadeout = false;

                // Restore the volume value, which may have been altered by fading out
                MediaPlayer.Volume = MediaPlayerTargetVolume;
                
                MediaPlayer.IsRepeating = loop;
                MediaPlayer.Play(song);
            }
            else
            {
                Console.WriteLine($"The soundtrack doesn't contain any Song named '{songName}'");
            }
        }

        /// <summary>
        /// Move the current song index of the MediaPlayer to the next song in the queue.
        /// </summary>
        public static void NextSong()
        {
            fadeout = false;

            // Restore the volume value, which may have been altered by fading out
            MediaPlayer.Volume = MediaPlayerTargetVolume;

            MediaPlayer.MoveNext();
        }

        /// <summary>
        /// Move the current song index of the MediaPlayer to the previous song in the queue.
        /// </summary>
        public static void PreviousSong()
        {
            fadeout = false;

            // Restore the volume value, which may have been altered by fading out
            MediaPlayer.Volume = MediaPlayerTargetVolume;

            MediaPlayer.MovePrevious();
        }

        /// <summary>
        /// Pause the playback of soundtrack music (this has no effect if no track is being played).
        /// </summary>
        public static void PauseMusic()
        {
            fadeout = false;
            if (MediaPlayer.State == MediaState.Playing)
                MediaPlayer.Pause();
        }

        /// <summary>
        /// Resume the playback of soundtrack music (this has no effect if the player has not previously been paused).
        /// </summary>
        public static void ResumeMusic()
        {
            fadeout = false;
            if (MediaPlayer.State == MediaState.Paused)
                MediaPlayer.Resume();
        }

        /// <summary>
        /// Immediately stop the playback of any soundtrack music.
        /// </summary>
        public static void StopMusic()
        {
            fadeout = false;
            MediaPlayer.Stop();
        }

        /// <summary>
        /// Fade-out the volume of the currently playing track, then stop the music playback.
        /// </summary>
        /// <param name="fadeTime">The duration (in seconds) of the fade-out effect.</param>
        public static void FadeOutMusic(float fadeTime)
        {
            if (MediaPlayer.State != MediaState.Stopped)
            {
                fadeout = true;
                fadeSpeed = MediaPlayer.Volume / fadeTime;
            }
        }


        /* SOUND EFFECTS */

        /// <summary>
        /// Add the provided SoundEffect to the sound collection.
        /// </summary>
        /// <param name="effectName">The name used to identify the SoundEffect.</param>
        /// <param name="effect">The SoundEffect object that has to be added.</param>
        public static void AddSoundEffect(string effectName, SoundEffect effect)
        {
            if (!soundEffects.TryAdd(effectName, new SFX(effect)))
            {
                Console.WriteLine($"The sound collection already contains a SoundEffect named '{effectName}'");
            }
        }

        /// <summary>
        /// Remove the provided SoundEffect from the sound collection.
        /// </summary>
        /// <param name="effectName">The name of the SoundEffect that has to be removed.</param>
        public static void RemoveSoundEffect(string effectName)
        {
            if (!soundEffects.Remove(effectName))
            {
                Console.WriteLine($"The sound collection doesn't contain any SoundEffect named '{effectName}'");
            }
        }

        /// <summary>
        /// Play back the requested SoundEffect.
        /// </summary>
        /// <param name="loop">Whether the sound effect should keep repeating after playback.</param>
        /// <param name="volume">Volume of the sound effect, in the range [0, 1].</param>
        /// <param name="pitch">Pitch adjustment of the sound effect, in the range [-1, 1].</param>
        /// <returns>The SoundEffectInstance playing the requested sound effect.</returns>
        public static SoundEffectInstance PlaySoundEffect(string effectName, 
            bool loop = false, float volume = 1f, float pitch = 0f)
        {
            if (soundEffects.TryGetValue(effectName, out SFX effect))
            {
                return effect.Play(loop, volume, pitch);
            }
            else
            {
                Console.WriteLine($"The sound collection doesn't contain any SoundEffect named '{effectName}'");

                return null;
            }
        }

        /// <summary>
        /// Stop the playback of the specified SoundEffect (all of its instances).
        /// </summary>
        /// <param name="effectName">The name of the SoundEffect that has to be stopped.</param>
        /// <param name="immediate">Whether the sound has to be stopped abruptedly or
        /// is allowed to play it's release phase (e.g. fade-out).</param>
        public static void StopSoundEffect(string effectName, bool immediate = true)
        {
            if (soundEffects.TryGetValue(effectName, out SFX effect))
            {
                effect.Stop(immediate);
            }
            else
            {
                Console.WriteLine($"The sound collection doesn't contain any SoundEffect named '{effectName}'");
            }
        }

        /// <summary>
        /// Stop all currently playing SoundEffect instances at once.
        /// </summary>
        /// <param name="immediate">Whether the sounds must be stopped abruptedly or
        /// are allowed to play their release phase (e.g. fade-out).</param>
        public static void StopAllSoundEffects(bool immediate = true)
        {
            foreach (SFX effect in soundEffects.Values)
            {
                effect.Stop(immediate);
            }
        }

    }
}
