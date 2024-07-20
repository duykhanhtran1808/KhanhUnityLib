using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObserverManager : MonoBehaviour
{
    public static ObserverManager Instance;
    private List<IObserver> observers = new List<IObserver>();

    private void Awake()
    {
        Instance = this;
    }
    public void AddObserver(IObserver observer)
    {
    observers.Add(observer); 
    }

    public void RemoveObserver(IObserver observer)
    {
        observers.Remove(observer);
    }

    public void OnPlayerDeath()
    {
        foreach(IObserver observer in observers)
        {
            observer.OnPlayerDeath();
        }
    }
}
