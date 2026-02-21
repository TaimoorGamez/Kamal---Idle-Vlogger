using System;
using UnityEngine;
using System.Collections.Generic;

namespace Core.GamePlay
{
    public class ColorAssigner : MonoBehaviour
    {
        [SerializeField] protected Renderer MySkin;
        [SerializeField] protected Color SkinColor;

        protected MaterialPropertyBlock _propBlock;

       protected virtual void Start()
        {
            _propBlock = new MaterialPropertyBlock();
        }
    }

    [Serializable]
    public class LevelData
    {
        public int LevelNumber;
        public int LevelIndex;
        public int Stars;
    }

    [Serializable]
    public class GameData
    {
        public List<LevelData> Levels = new List<LevelData>();
    }

}
