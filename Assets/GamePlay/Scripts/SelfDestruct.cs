using UnityEngine;
using Core.Events;

public class SelfDestruct : MonoBehaviour
{
    [SerializeField] float DelayTime;
    [SerializeField] bool BlastOnStart = false;

    private void OnEnable()
    {
        SimpleEventsHolder.SelfDestructionEvent += BlastNow;
    }

    private void OnDisable()
    {
        SimpleEventsHolder.SelfDestructionEvent -= BlastNow;
    }

    private void Start()
    {
        if (BlastOnStart)
        {
            BlastNow();
        }
    }

    void BlastNow()
    {
        Destroy(gameObject, DelayTime);
    }
}
