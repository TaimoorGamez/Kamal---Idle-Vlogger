using UnityEngine;
using DG.Tweening;
using Core.Events;
using System.Collections;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Core.GamePlay
{
    public class CashParticle : MonoBehaviour
    {
        [SerializeField] GameObject cashPrefab;
        [SerializeField] float SpawnRate = 0.2f, MoveDistance = 4f, Duration = 1f;
        [SerializeField] Camera MainCamera;

        Queue<GameObject> _pool = new Queue<GameObject>();
        int _poolSize = 10;
        bool _isSpawning;
        Coroutine _cashCorotine;

        public static bool _isTapped = false;

        private void Start()
        {
            for (int i = 0; i < _poolSize; i++)
            {
                GameObject obj = Instantiate(cashPrefab, transform);
                obj.SetActive(false);
                _pool.Enqueue(obj);
            }
        }

        public void OnButtonClick()
        {
            _isTapped = true;
            if (!_isSpawning)
                _cashCorotine = StartCoroutine(SpawnCash());
        }

        public void OnButtonRelease()
        {
            _isTapped = false;
            _isSpawning = false;
            if (_cashCorotine != null)
                StopCoroutine(_cashCorotine);
        }

        IEnumerator SpawnCash()
        {
            _isSpawning = true;

            while (_isSpawning)
            {
                Spawn();
                yield return new WaitForSeconds(SpawnRate);
            }
        }

        private void Spawn()
        {
            if (_pool.Count == 0 || !_isSpawning) return;

            Vector2 inputPos;

            #if UNITY_EDITOR || UNITY_STANDALONE
                inputPos = Mouse.current.position.ReadValue();

            #elif UNITY_ANDROID || UNITY_IOS
                inputPos = Touchscreen.current.primaryTouch.position.ReadValue();
            #endif

            Vector3 worldPos = MainCamera.ScreenToWorldPoint(new Vector3(inputPos.x, inputPos.y, 10f));
            GameObject cash = _pool.Dequeue();

            float randomRot = Random.Range(-180f, 180f);

            cash.transform.position = worldPos;
            cash.SetActive(true);

            Sequence seq = DOTween.Sequence();
            seq.Append(cash.transform.DOMoveY(transform.position.y + MoveDistance, Duration).SetEase(Ease.OutQuad));
            seq.Join(cash.transform.DORotate(new Vector3(0, 0, randomRot), Duration, RotateMode.FastBeyond360));
            seq.OnComplete(() =>
            {
                cash.SetActive(false);
                _pool.Enqueue(cash);
            });
            SingleIntegerEventsHolder.SoundEffectEvent?.Invoke(1);
        }
    }
}