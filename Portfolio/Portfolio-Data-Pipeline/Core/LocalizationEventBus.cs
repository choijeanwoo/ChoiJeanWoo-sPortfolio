using UnityEngine;
using System.Collections.Generic;

public class LocalizationEventBus
{
    private static readonly List<ILocalizable> _localizables = new List<ILocalizable>();

    public static void Register(ILocalizable target)
    {
        if (!_localizables.Contains(target))
        {
            _localizables.Add(target);
        }
    }

    public static void Unregister(ILocalizable target)
    {
        _localizables.Remove(target);
    }

    public static void UpdateAll()
    {
        foreach (var localizable in _localizables)
        {
            localizable.UpdateLocalization();
        }
    }
}
