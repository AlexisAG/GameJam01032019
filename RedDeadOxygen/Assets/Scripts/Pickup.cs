using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Pickup
{

    // called when is pciked by a player
    void IsPick();

    // called when the item need to be activate
    void Activate();

    // called when the item need to respawn on the map
    void Respawn();

}
