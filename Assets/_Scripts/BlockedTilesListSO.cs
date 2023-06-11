using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BlockedTilesData", menuName = "Custom/Blocked Tiles Data")]
public class BlockedTilesListSO : ScriptableObject
{
    public List<Sprite> blockedTileSprites = new List<Sprite>();
}
