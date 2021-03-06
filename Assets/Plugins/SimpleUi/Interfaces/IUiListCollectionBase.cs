namespace SimpleUi.Interfaces
{
	public interface IUiListCollectionBase<TView> : IUiCollectionBase<TView> where TView : IUiView
	{
		TView this[int index] { get; }

		void Remove(TView view);

		void RemoveAt(int index);
	}

	public interface IUiListCollection<TView> : IUiListCollectionBase<TView>, IUiCollection<TView>
		where TView : IUiView
	{
	}

	public interface IUiListCollection<TParam1, TView> : IUiListCollectionBase<TView>,
		IUiCollection<TParam1, TView>
		where TView : IUiView, IParametrizedView<TParam1>
	{
	}

	public interface IUiListCollection<TParam1, TParam2, TView> : IUiListCollectionBase<TView>,
		IUiCollection<TParam1, TParam2, TView>
		where TView : IUiView, IParametrizedView<TParam1, TParam2>
	{
	}

	public interface IUiListCollection<TParam1, TParam2, TParam3, TView> : IUiListCollectionBase<TView>,
		IUiCollection<TParam1, TParam2, TParam3, TView>
		where TView : IUiView, IParametrizedView<TParam1, TParam2, TParam3>
	{
	}

	public interface IUiListCollection<TParam1, TParam2, TParam3, TParam4, TView> : IUiListCollectionBase<TView>,
		IUiCollection<TParam1, TParam2, TParam3, TParam4, TView>
		where TView : IUiView, IParametrizedView<TParam1, TParam2, TParam3, TParam4>
	{
	}

	public interface IUiListCollection<TParam1, TParam2, TParam3, TParam4, TParam5, TView> :
		IUiListCollectionBase<TView>,
		IUiCollection<TParam1, TParam2, TParam3, TParam4, TParam5, TView>
		where TView : IUiView, IParametrizedView<TParam1, TParam2, TParam3, TParam4, TParam5>
	{
	}

	public interface IUiListCollection<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6,
		TView> : IUiListCollectionBase<TView>,
		IUiCollection<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TView>
		where TView : IUiView, IParametrizedView<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6>
	{
	}

	public interface IUiListCollection<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7,
		TView> : IUiListCollectionBase<TView>,
		IUiCollection<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7, TView>
		where TView : IUiView, IParametrizedView<TParam1, TParam2, TParam3, TParam4, TParam5, TParam6, TParam7>
	{
	}
}