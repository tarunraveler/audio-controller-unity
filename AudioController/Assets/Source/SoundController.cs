﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using OSSC.Model;

namespace OSSC
{
    [RequireComponent(typeof(ObjectPool))]
    public class SoundController : MonoBehaviour
    {
        #region Serialized Data
        public GameObject _defaultPrefab;

        #endregion

        #region Private fields

        private ObjectPool _pool;

        #endregion

        #region Public methods and properties

        public string _dbName;

        public AudioControllerData _database;

        public GameObject defaultPrefab
        {
            set
            {
                _defaultPrefab = value;
            }
        }

        public SoundCue Play(string[] names)
        {
            return Play(names, (Transform)(null));
        }
        public SoundCue Play(string[] names, Transform parent)
        {
            return Play(names, parent, null);
        }
        public SoundCue Play(string[] names, Transform parent, string categoryName)
        {
            return Play(names, parent, 0f, 0f, categoryName);
        }
        public SoundCue Play(string[] names, float fadeInTime = 0, float fadeOutTime = 0f)
        {
            return Play(names, null, fadeInTime, fadeOutTime);
        }
        public SoundCue Play(string[] names, Transform parent, float fadeInTime, float fadeOutTime = 0f)
        {
            return Play(names, parent, fadeInTime, fadeOutTime, null);
        }
        public SoundCue Play(string[] names, string categoryName)
        {
            return Play(names, null, 0f, 0f, categoryName);
        }
        public SoundCue Play(string name)
        {
            return Play(new[] {name});
        }
        public SoundCue Play(string name, Transform parent)
        {
            return Play(new[] {name}, parent);
        }
        public SoundCue Play(string name, Transform parent, string categoryName)
        {
            return Play(new [] {name}, parent, categoryName);
        }
        public SoundCue Play(string name, Transform parent, float fadeInTime, float fadeOutTime = 0f)
        {
            return Play(new [] {name}, parent, fadeInTime, fadeOutTime);
        }
        public SoundCue Play(string name, string categoryName = null)
        {
            return Play(new[] { name }, categoryName);
        }
        public SoundCue Play(string name, float fadeInTime = 0, float fadeOutTime = 0f)
        {
            return Play(new[] { name }, fadeInTime, fadeOutTime);
        }

        public SoundCue Play(string[] names, Transform parent = null, float fadeInTime = 0f, float fadeOutTime = 0f, string categoryName = null)
        {
            UnityEngine.Assertions.Assert.IsNotNull(names, "[AudioController] names cannot be null");
            if (names != null)
                UnityEngine.Assertions.Assert.IsFalse(names.Length == 0, "[AudioController] names cannot have 0 strings");

            CategoryItem category = null;
            GameObject prefab = null;
            List<SoundItem> items = new List<SoundItem>();
            List<float> catVolumes = new List<float>();

            if (string.IsNullOrEmpty(categoryName) == false)
            {
                category = System.Array.Find(_database.items, (item) =>
                {
                    return item.name == categoryName;
                });

                Debug.Log(category);
                if (category == null)
                    return null;

                prefab = category.usingDefaultPrefab ? _defaultPrefab : category.audioObjectPrefab;
                for (int i = 0; i < names.Length; i++)
                {
                    SoundItem item = System.Array.Find(category.soundItems, (x) =>
                    {
                        return x.name == names[i];
                    });

                    if (item != null)
                    {
                        catVolumes.Add(category.categoryVolume);
                        items.Add(item);
                    }
                }
            }
            else
            {
                prefab = _defaultPrefab;
                CategoryItem[] categoryItems = _database.items;
                for (int i = 0; i < names.Length; i++)
                {
                    SoundItem item = null;
                    item = items.Find((x) => names[i] == x.name);
                    if (item != null)
                    {
                        catVolumes.Add(catVolumes[items.IndexOf(item)]);
                        items.Add(item);
                        continue;
                    }

                    for (int j = 0; j < categoryItems.Length; j++)
                    {
                        item = System.Array.Find(categoryItems[j].soundItems, (x) => x.name == names[i]);
                        if (item != null)
                        {
                            catVolumes.Add(categoryItems[j].categoryVolume);
                            break;
                        }
                    }
                    if (item != null)
                        items.Add(item);
                }
            }

            if (items.Count == 0)
                return null;

            SoundCue cue = new SoundCue();
            SoundCueData data;
            data.audioPrefab = prefab;
            data.sounds = items.ToArray();
            data.categoryVolumes = catVolumes.ToArray();
            data.fadeInTime = fadeInTime;
            data.fadeOutTime = fadeOutTime;
            data.isFadeIn = data.fadeInTime >= 0.1f;
            data.isFadeOut = data.fadeOutTime >= 0.1f;
            cue.audioObject = _pool.GetFreeObject(prefab).GetComponent<SoundObject>();
            if (parent != null)
                cue.audioObject.transform.SetParent(parent, false);
            cue.Play(data);

            return cue;
        }

        /// <summary>
        /// Creates a new copy of the AudioCue and plays it.
        /// </summary>
        /// <param name="cue">AudioCue to clone and play.</param>
        /// <returns>Cloned AudioCue.</returns>
        public SoundCue Play(SoundCue cue)
        {
            return Play(cue, null);
        }

        public SoundCue Play(SoundCue cue, Transform parent)
        {
            return Play(cue, parent, 0f);
        }

        public SoundCue Play(SoundCue cue, Transform parent, float fadeInTime, float fadeOutTime = 0f)
        {
            var ncue = new SoundCue();
            ncue.audioObject = _pool.GetFreeObject(cue.data.audioPrefab).GetComponent<SoundObject>();
            if (parent != null)
                ncue.audioObject.transform.SetParent(parent, false);
            SoundCueData data = cue.data;
            data.fadeInTime = fadeInTime;
            data.fadeOutTime = fadeOutTime;
            data.isFadeIn = data.fadeInTime >= 0.1f;
            data.isFadeOut = data.fadeOutTime >= 0.1f;
            ncue.Play(data);
            return ncue;
        }

        #endregion

        #region Private methods
        [ContextMenu("Play test1")]
        private void TestPlay()
        {
            Play("test1", 2f, 3f);
        }
        #endregion

        #region MonoBehaviour methods

        void Awake()
        {
            _pool = GetComponent<ObjectPool>();
        }

        #endregion
    }

}