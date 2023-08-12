using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelData
{
    public CannonBarrel Barrel { get; private set; }
    public ClientPlayer Player { get; private set; }
    public Vector3 Target { get; set; }

    public BarrelData(CannonBarrel barrel, ClientPlayer player)
    {
        Barrel = barrel;
        Player = player;
    }
}
