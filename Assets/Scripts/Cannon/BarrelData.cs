using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelData
{
    public GameObject Barrel { get; private set; }
    public ClientPlayer Player { get; private set; }

    public BarrelData(GameObject barrel, ClientPlayer player)
    {
        Barrel = barrel;
        Player = player;
    }
}
