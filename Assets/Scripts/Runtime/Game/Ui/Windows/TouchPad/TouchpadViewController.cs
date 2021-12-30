using ECS.Game.Components.Input;
using Leopotam.Ecs;
using PdUtils.Interfaces;
using SimpleUi.Abstracts;
using UnityEngine.EventSystems;

namespace Runtime.Game.Ui.Windows.TouchPad
{
	public interface ITouchpadViewController
	{
		void SetActive(bool value);
	}
	
	public class TouchpadViewController : UiController<TouchpadView>, IUiInitializable, ITouchpadViewController
	{
		private readonly EcsWorld _world;
		private bool _active;
		
		public TouchpadViewController(EcsWorld world)
		{
			_world = world;
		}
		public void Initialize()
		{
			View.SetDragAction(OnDragAction);
			View.SetPointerDownAction(OnPointerDownAction);
			View.SetPointerUpAction(OnPointerUpAction);
		}

		private void OnDragAction(PointerEventData eventData)
		{
			// if(eventData.delta.sqrMagnitude <= 1) return;
			// var worldPos = eventData.pointerCurrentRaycast;
			ref var drag = ref _world.NewEntity().Get<PointerDragComponent>();
			drag.Position = eventData.pointerCurrentRaycast.screenPosition;
		}

		private void OnPointerDownAction(PointerEventData eventData)
		{
			if(!_active)
				return;
			_world.NewEntity().Get<PointerDownComponent>().Position = eventData.pointerCurrentRaycast.screenPosition;
			// var entity = _world.GetEntity<PlayerComponent>();
			// if (!entity.IsNull())
				// entity.GetAndFire<RemapPointComponent>().Input = eventData.pointerCurrentRaycast.worldPosition;
		}

		private void OnPointerUpAction(PointerEventData eventData)
		{
			if(!_active)
				return;
			_world.NewEntity().Get<PointerUpComponent>().Position = eventData.pointerCurrentRaycast.screenPosition;
		}

		public void SetActive(bool value) => _active = value;
	}
}