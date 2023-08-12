using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelData
{
    public CannonBarrel Barrel { get; private set; }
    public ClientPlayer Player { get; private set; }
    public GameObject Target { get; set; }
    public delegate void ActionDone(Vector3 position);
    public ActionDone Callback { get; private set; }

    public BarrelData(CannonBarrel barrel, ClientPlayer player, ActionDone callback)
    {
        Barrel = barrel;
        Player = player;
        Callback = callback;
    }
}
