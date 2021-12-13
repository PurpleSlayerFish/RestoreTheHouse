using System;
using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.GameCycle;
using ECS.Views.GameCycle;
using Leopotam.Ecs;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace ECS.Game.Systems.GameCycle
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    public class ProjectileCollisionSystem : ReactiveSystem<EventAddComponent<ProjectileComponent>>, IDisposable
    {
        private CompositeDisposable _disposable = new CompositeDisposable();
        private const string Enemy = "Enemy";
        private const string Environment = "Environment";
        private readonly LayerMask _enemyLayerMask = LayerMask.NameToLayer(Enemy);
        private readonly LayerMask _environmentLayerMask = LayerMask.NameToLayer(Environment);
        protected override EcsFilter<EventAddComponent<ProjectileComponent>> ReactiveFilter { get; }

        protected override void Execute(EcsEntity entity)
        {
            ProjectileView view = entity.Get<LinkComponent>().View as ProjectileView;
            view.GetCollider().OnTriggerEnterAsObservable().Subscribe(
                    other =>
                    {
                        if (other.gameObject.layer == _environmentLayerMask)
                            view.Impact();
                        if (other.gameObject.layer == _enemyLayerMask)
                        {
                            view.Impact();
                            other.GetComponent<EnemyView>().InitHit();
                        }
                    })
                .AddTo(_disposable);
        }

        public void Dispose()
        {
            _disposable?.Dispose();
        }
    }
}