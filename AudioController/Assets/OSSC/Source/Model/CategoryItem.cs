﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OSSC.Model
{
    [System.Serializable]
    public class CategoryItem
    {
        public string name;
        public SoundItem[] soundItems;
        public GameObject audioObjectPrefab;
        public bool usingDefaultPrefab = true;

        [Range(0f, 1f)]
        public float categoryVolume = 1f;

        public bool foldOutSoundItems = false;
        public string soundsSearchName = "";
        public bool isMute = false;
    }

}
