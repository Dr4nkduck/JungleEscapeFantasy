using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public static CoinManager instance;
    public int points;
    List<Coin> availableCoins;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        points = 0;
        availableCoins = FindObjectsByType<Coin>().ToList();
    }

    public void AddPoint(Coin coin, int coinValue)
    {
        points += coinValue;
        availableCoins.Remove(coin);
        UIManager.instance.UpdateCoinText(points);
        if (IsOutOfCoins()) GameManager.instance.SetGateActive(true);
    }

    public bool IsOutOfCoins()
    {
        return availableCoins.Count == 0;
    }
}
