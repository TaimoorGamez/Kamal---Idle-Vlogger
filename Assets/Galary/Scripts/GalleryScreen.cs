using System.IO;
using UnityEngine;
using Core.Events;
using DG.Tweening;
using Core.GamePlay;
using UnityEngine.UI;
using System.Collections;
using Core.Plugins.Firebase;

namespace Core.Screen
{
    public class GalleryScreen : UiScreens
    {
        [SerializeField] PaintingView PaintingObj;
        [SerializeField] RectTransform Content;
        [SerializeField] ScrollRect GalaryScroll;
        [SerializeField] LvlPreview CurrentPreview;

        string _starsDataPath, _directoryPath;
        Coroutine _paintingRotine;

        private void OnEnable()
        {
            OnOpen();
        }

        private void OnDisable()
        {
            if (_paintingRotine != null)
                StopCoroutine(_paintingRotine);
        }

        private void Start()
        {
            _starsDataPath = Path.Combine(Application.persistentDataPath, "starsData.json");
            _directoryPath = Path.Combine(Application.persistentDataPath, "Paintings");
            _paintingRotine = StartCoroutine(PaintingsCorotine());
        }

        GameData LoadStars()
        {
            if (File.Exists(_starsDataPath))
            {
                string json = File.ReadAllText(_starsDataPath);
                return JsonUtility.FromJson<GameData>(json);
            }
            return new GameData();
        }

        IEnumerator PaintingsCorotine()
        {
            GameData starsData = LoadStars();
            yield return new WaitForSeconds(0.01f);
            for(int s = LevelsManager.I.MinLvlCount-1; s < starsData.Levels.Count; s++)
            {
                string filePath = Path.Combine(_directoryPath, $"Painting_{starsData.Levels[s].LevelNumber}.png");
                if (File.Exists(filePath))
                {
                    byte[] fileData = File.ReadAllBytes(filePath);
                    Texture2D texture = new Texture2D(2, 2);
                    if (texture.LoadImage(fileData))
                    {
                        yield return new WaitForSeconds(0.01f);
                        PaintingView newPainting = Instantiate(PaintingObj, Content);
                        newPainting.InitPainting(texture, starsData.Levels[s].LevelNumber, starsData.Levels[s].LevelIndex, starsData.Levels[s].Stars, CurrentPreview);
                    }
                }
            }
            GalaryScroll.DOVerticalNormalizedPos(0f, 1).SetEase(Ease.OutBack);
        }

        public override void OnOpen()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(3);
            Body.DOScale(1, _transitionDuration).SetEase(Ease.OutBack);
            FirebaseHandler.I?.LogEvent("Glry_Open");
        }

        public override void OnClose()
        {
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(2);
            Body.DOScale(0, _transitionDuration/2).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
            FirebaseHandler.I?.LogEvent("Glry_Close");
        }
    }
}
