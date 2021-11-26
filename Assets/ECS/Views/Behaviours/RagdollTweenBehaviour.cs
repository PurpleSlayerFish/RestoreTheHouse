using DG.Tweening;
using UnityEngine;

namespace ECS.Views.Behaviours
{
    public class RagdollTweenBehaviour : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRenderer1;
        [SerializeField] private LineRenderer _lineRenderer2;
        [SerializeField] private Transform _ropeBand1;
        [SerializeField] private Transform _ropeBand2;
        [SerializeField] private Transform _fastening1;
        [SerializeField] private Transform _fastening2;
    
        private void Start()
        {
            _fastening1.DOMoveY(8.3f, 4).SetEase(Ease.Unset).SetLoops(-1, LoopType.Yoyo);
            _fastening2.DOMoveY(8.3f, 3.5f).SetEase(Ease.Unset).SetLoops(-1, LoopType.Yoyo);
        }
    
        private void FixedUpdate()
        {
            UpdateRope();
        }

        private void UpdateRope()
        {
            _lineRenderer1.SetPosition(0, _ropeBand1.position);
            _lineRenderer1.SetPosition(1, _fastening1.position);
            _lineRenderer2.SetPosition(0, _ropeBand2.position);
            _lineRenderer2.SetPosition(1, _fastening2.position);
        }
    }
}
