using Ecs.Views.Linkable.Impl;
using UnityEngine;

public class FinishVisualView : LinkableView
{
    [SerializeField] private GameObject[] stages;
    [SerializeField] private Transform level;
    
    public void ShowFinish(int stageIndex)
    {
        level.gameObject.SetActive(false);
        var stage = stages[stageIndex];
        stages.ForEach(x => x.SetActive(false));
        stage.SetActive(true);
    }
}
