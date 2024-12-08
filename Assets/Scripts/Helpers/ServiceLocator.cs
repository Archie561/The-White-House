using System;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public static void RegisterService<T>(T service) => _services[typeof(T)] = service;

    public static void UnregisterService<T>()
    {
        var type = typeof(T);
        if (_services.ContainsKey(type)) _services.Remove(type);
    }

    public static T GetService<T>()
    {
        var type = typeof(T);
        if (_services.ContainsKey(type)) return (T)_services[type];
        else
        {
            Debug.LogError($"Service {type} not found.");
            return default;
        }
    }
}