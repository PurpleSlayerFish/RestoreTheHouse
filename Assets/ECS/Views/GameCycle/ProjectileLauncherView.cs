using ECS.Views.Impls;
using UnityEngine;

namespace ECS.Views.GameCycle
{
    public class ProjectileLauncherView : LinkableView
    {
        [SerializeField] private ParticleSystem _launchEffect;

        public void PlayLaunchEffect()
        {
            _launchEffect.gameObject.SetActive(true);
        }
    }
}