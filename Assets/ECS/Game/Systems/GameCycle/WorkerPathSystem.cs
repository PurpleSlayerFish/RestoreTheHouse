using ECS.Core.Utils.SystemInterfaces;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
using ECS.Game.Components.GameCycle;
using ECS.Game.Components.General;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using Runtime.DataBase.Game;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    public class WorkerPathSystem : IEcsUpdateSystem
    {
#pragma warning disable 649
        private readonly EcsFilter<GameStageComponent> _gameStage;
        private readonly EcsFilter<WorkerComponent, LinkComponent, PositionComponent> _workers;
#pragma warning restore 649

        private const float TOLERANCE = 0.05f;
        private WorkerView _view;
        private EcsEntity _entity;
        public void Run()
        {
            if (_gameStage.Get1(0).Value != EGameStage.Play) return;
            
            foreach (var i in _workers)
            {
                _entity = _workers.GetEntity(i);
                _view = _entity.Get<LinkComponent>().Get<WorkerView>();
                if (!_view.gameObject.activeSelf)
                    return;
                if (Vector3.Distance(_view.GetTargetPointPosition(), _workers.Get3(i).Value) <= TOLERANCE)
                    SetStage();
            }
        }

        private void SetStage()
        {
            if (_view.GetTargetPointType() == EPathPointType.Get)
            {
                if (_view.WorkerStage == EWorkerStage.Walk)
                {
                    _view.WorkerStage = EWorkerStage.Idle;
                    _entity.Del<IsMovingComponent>();
                }
                if (_view.WorkerStage == EWorkerStage.Idle)
                {
                    if (_entity.Get<ElapsedTimeComponent>().Value >= _view.GetInteractionDuration())
                    {
                        _view.WorkerStage = EWorkerStage.Carry;
                        _view.SetActiveResourceStack(true);
                        _entity.Del<ElapsedTimeComponent>();
                    }
                    else
                        return;
                }
                if (_view.WorkerStage == EWorkerStage.Carry)
                {
                    if (_entity.Get<ElapsedTimeComponent>().Value >= _view.GetInteractionDuration())
                    {
                        _view.WorkerStage = EWorkerStage.CarryingWalk;
                        _entity.Get<IsMovingComponent>();
                        _entity.Del<ElapsedTimeComponent>();
                        MoveToNext();
                    }
                    return;
                }
            }
            
            if (_view.GetTargetPointType() == EPathPointType.Put)
            {
                if (_view.WorkerStage == EWorkerStage.CarryingWalk)
                {
                    _view.WorkerStage = EWorkerStage.Carry;
                    _entity.Del<IsMovingComponent>();
                }
                if (_view.WorkerStage == EWorkerStage.Carry)
                {
                    if (_entity.Get<ElapsedTimeComponent>().Value >= _view.GetInteractionDuration())
                    {
                        _view.WorkerStage = EWorkerStage.Idle;
                        _view.SetActiveResourceStack(false);
                        _entity.Del<ElapsedTimeComponent>();
                    }
                    else
                        return;
                }
                if (_view.WorkerStage == EWorkerStage.Idle)
                {
                    if (_entity.Get<ElapsedTimeComponent>().Value >= _view.GetInteractionDuration())
                    {
                        _view.WorkerStage = EWorkerStage.Walk;
                        _entity.Get<IsMovingComponent>();
                        _entity.Del<ElapsedTimeComponent>();
                        MoveToNext();
                    }
                    return;
                }
            }

            if (_view.GetTargetPointType() == EPathPointType.Default)
            {
                MoveToNext();
            }
        }

        private void MoveToNext()
        {
            _view.SetNextTargetPoint();
            _entity.Get<TargetPositionComponent>().Value = _view.GetTargetPointPosition();
            _entity.Get<RotationComponent>().Value = Quaternion.Euler(_view.GetTargetRotationDirection());
        }
    }
}