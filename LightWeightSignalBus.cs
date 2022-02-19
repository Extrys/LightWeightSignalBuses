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
		return new SignalSubscription(action);
	}

	public static void Fire(T t) => action?.Invoke(t);

	public static void Unsusbscribe(Action<T> action) => SignalBus<T>.action -= action;

	//Subscription registered for Lightweight version
	public class Subscription : IDisposable
	{
		Action<T> bindedAction;
		public SignalSubscription(Action<T> bindedAction) => this.bindedAction = bindedAction;
		public void Dispose() => Unsusbscribe(bindedAction);
	}
}