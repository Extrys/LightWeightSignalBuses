using System;

public static class DisposableExtensions
{
	/// <summary> Adds disposables at the end of their build pattern, making the code cleaner and easier to read </summary>
	public static void AddTo(this IDisposable disposable, CompositeDisposable compositeDisposable) => compositeDisposable.Add(disposable);
}
