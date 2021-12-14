using System;
using System.Diagnostics.CodeAnalysis;
using ECS.Core.Utils.ReactiveSystem;
using ECS.Core.Utils.ReactiveSystem.Components;
using ECS.Game.Components;
using ECS.Game.Components.Flags;
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
#pragma warning disable 649
        private readonly EcsFilter<PlayerComponent, InTrialComponent> _player;
        private readonly EcsFilter<ChestComponent, HealthPointComponent, LinkComponent> _chest;
#pragma warning restore 649

        private CompositeDisposable _disposable = new CompositeDisposable();
        private const string Enemy = "Enemy";
        private const string Environment = "Environment";
        private const string Chest = "Chest";
        private readonly LayerMask _enemyLayerMask = LayerMask.NameToLayer(Enemy);
        private readonly LayerMask _environmentLayerMask = LayerMask.NameToLayer(Environment);
        private readonly LayerMask _chestLayerMask = LayerMask.NameToLayer(Chest);
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
                        
                        if (other.gameObject.layer == _chestLayerMask)
                        {
                            view.Impact();
                            foreach (var i in _player)
                            {
                                other.GetComponent<ChestView>().InitHit();
                            }
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