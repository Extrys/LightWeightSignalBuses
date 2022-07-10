using System;
using System.Collections.Generic;

///<Summary>Interface compatible version</Summary>
///<remarks>dont use with struct signals to avoid garbage generation by boxing </remarks>
public static class SignalBus
{
	static Dictionary<Type, Action<object>> actions = new Dictionary<Type, Action<object>>();
	static Dictionary<Type, Type[]> cachedInterfacedByType = new Dictionary<Type, Type[]>();

	public static IDisposable Subscribe<T>(Action<T> action)
	{

		Action<object> objectAction = new Action<object>(o => action((T)o));
		Type paramType = typeof(T);
		if (actions.TryGetValue(paramType, out Action<object> actionGroup))
			actionGroup += objectAction;
		else actions.Add(paramType, objectAction);

		return new Subscription(paramType, objectAction);
	}

	public static void Fire<T>(T t)
	{
		if (actions.TryGetValue(typeof(T), out var action))
			action(t);
		else
		{
#if UNITY_STANDALONE
			UnityEngine.Debug.LogError($"Type of {typeof(T)} is trying to be fired as signal but is not subscribed anywhere");
#else
			Console.WriteLine($"Type of {typeof(T)} is trying to be fired as signal but is not subscribed anywhere");
#endif
		}
	}

	public static void AbstractFire<T>(T t)
	{
		Action<object> actionFromDic;
		Type[] types = GetOrCreateInterfaceArrayForType(typeof(T));
		foreach (Type type in types)
		{
			if (actions.TryGetValue(type, out actionFromDic))
				actionFromDic(t);
		}
	}

	static Type[] GetOrCreateInterfaceArrayForType(Type type)
	{
		Type[] interfaces;
		if (cachedInterfacedByType.TryGetValue(type, out interfaces))
			return interfaces;

		interfaces = type.GetInterfaces();
		cachedInterfacedByType.Add(type, interfaces);
		return interfaces;
	}

	public static void Unsusbscribe(Type paramType, Action<object> action)
	{
		if (actions.TryGetValue(paramType, out Action<object> actionFromDic))
			actionFromDic -= action;
	}


	//Subscription registered for Interface compatible version
	public class Subscription : IDisposable
	{
		Type paramType;
		Action<object> bindedAction;

		public Subscription(Type paramType, Action<object> bindedAction)
		{
			this.paramType = paramType;
			this.bindedAction = bindedAction;
		}

		public void Dispose() => Unsusbscribe(paramType, bindedAction);
	}
}

