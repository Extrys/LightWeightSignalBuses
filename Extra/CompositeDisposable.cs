using System;
using System.Collections.Generic;

/// <summary>
/// A simple list of disposables. Its made for adding disposables at the end of their build pattern, making the code cleaner and easier to read<br/>
/// mainly used for <see cref="DisposableExtensions.AddTo(IDisposable, CompositeDisposable)"/>
/// </summary>
public class CompositeDisposable : IDisposable
{
	List<IDisposable> disposables = new List<IDisposable>();

	public void Dispose()
	{
		int iterations = disposables.Count;
		for (int i = 0; i < iterations; i++)
			disposables[i].Dispose();
	}

	public void Add(IDisposable disposable) => disposables.Add(disposable);
}