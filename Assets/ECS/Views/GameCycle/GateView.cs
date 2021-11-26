using System.ComponentModel;
using DataBase.Game;
using ECS.Game.Components.Events;
using Leopotam.Ecs;
using Runtime.Game.Utils.MonoBehUtils;
using TMPro;
using UnityEngine;
using Zenject;

namespace ECS.Views.GameCycle
{
    public class GateView : InteractableView
    {
        [Inject] private GateColors _gateColors;

        [SerializeField] private EImpactType impactType;
        [SerializeField] private GameObject neighbour;
        // Bad DrawCall
        [SerializeField] private MeshRenderer _pile1;
        [SerializeField] private MeshRenderer _pile2;
        [SerializeField] private MeshRenderer _glass;
        [SerializeField] private TMP_Text _text;

        public override void Link(EcsEntity entity)
        {
            base.Link(entity);
            Entity.Get<ImpactTypeComponent>().Value = impactType;
            InitColors();
            InitText();
        }

        private void InitColors()
        {
            if (impactType == EImpactType.Addition || impactType == EImpactType.Multiplication)
            {
                _pile1.material = _gateColors.PositivePile;
                _pile2.material = _gateColors.PositivePile;
                _glass.material = _gateColors.PositiveGlass;
                _text.font = _gateColors.PositiveFont;
            }
            else
            {
                _pile1.material = _gateColors.NegativePile;
                _pile2.material = _gateColors.NegativePile;
                _glass.material = _gateColors.NegativeGlass;
                _text.font = _gateColors.NegativeFont;
            }
        }

        private void InitText()
        {
            string prefix;
            switch (impactType)
            {
                case EImpactType.Addition:
                    prefix = "+";
                    break;
                case EImpactType.Subtraction:
                    prefix = "-";
                    break;
                case EImpactType.Multiplication:
                    prefix = "\u00D7";
                    break;
                case EImpactType.Division:
                    prefix = "\u00F7";
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
            _text.text = prefix + impact;
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            base.OnTriggerEnter(other);
            neighbour.gameObject.GetComponent<Collider>().enabled = false;
        }
    }
}