using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>Lightweight version of SignalBus</summary>
/// <typeparam name="T">Type of signal</typeparam>
public static class SignalBus<T>
{
	static Action<T> action;

	public static IDisposable Subscribe(Action<T> action)
	{
		SignalBus<T>.action += action;
		return new Subscription(action);
	}

	public static void Fire(T t) => action?.Invoke(t);

	public static void Unsusbscribe(Action<T> action) => SignalBus<T>.action -= action;

	//Subscription registered for Lightweight version
	public class Subscription : IDisposable
	{
		Action<T> boundAction;
		public Subscription(Action<T> boundAction) => this.boundAction = boundAction;
		public void Dispose() => Unsusbscribe(boundAction);
	}
}
