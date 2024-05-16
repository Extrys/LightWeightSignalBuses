using System;
using System.Collections.Generic;
using UnityEngine;

///<Summary>Interface compatible version</Summary>
///<remarks>dont use with struct signals to avoid garbage generation by boxing </remarks>
public static class SignalBus
{
	static Dictionary<Type, Action<object>> actions;

	static Dictionary<Type, Type[]> cachedInterfacesByType;

	public static IDisposable Subscribe<T>(Action<T> action)
	{
		Action<object> objectAction = new Action<object>(o => action((T)o));
		Type paramType = typeof(T);
		if (actions.ContainsKey(paramType))
			actions[paramType] += objectAction;
		else
			actions.Add(paramType, objectAction);

		return new Subscription(paramType, objectAction);
	}

	public static void Fire<T>(T t)
	{
		if (actions.TryGetValue(typeof(T), out var action))
			action(t);
		else Debug.LogError($"Type of {typeof(T)} is trying to be fired as signal but is not subscribed anywhere");
	}

	public static void AbstractFire<T>(T t, bool includeConcrete = false)
	{
		if (includeConcrete)
			Fire(t);

		Action<object> actionFromDic;
		Type[] types = GetOrCreateInterfaceArrayForType(typeof(T));
		foreach (Type type in types)
		{
			if (actions.TryGetValue(type, out actionFromDic))
				actionFromDic?.Invoke(t);
		}
	}

	static Type[] GetOrCreateInterfaceArrayForType(Type type)
	{
		Type[] interfaces;
		if (cachedInterfacesByType.TryGetValue(type, out interfaces))
			return interfaces;

		interfaces = type.GetInterfaces();
		cachedInterfacesByType.Add(type, interfaces);
		return interfaces;
	}

	static void Unsusbscribe(Type paramType, Action<object> action)
	{
		if (actions.ContainsKey(paramType))
		{
			actions[paramType] -= action;
		}
	}

	//Subscription registered for Interface compatible version
	class Subscription : IDisposable
	{
		Type paramType;
		Action<object> boundAction;

		public Subscription(Type paramType, Action<object> boundAction)
		{
			this.paramType = paramType;
			this.boundAction = boundAction;
		}

		public void Dispose() => Unsusbscribe(paramType, boundAction);
	}
}
