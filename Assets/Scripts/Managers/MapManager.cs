using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonManager<MapManager> {
    [SerializeField]
    private MapConfigurationSO currentMapConfiguration;
    public MapConfigurationSO CurrentMapConfiguration {
        get {
            if(currentMapConfiguration == null) {
                Debug.LogError("map configuration is missing!!");
            }
            return currentMapConfiguration;
        }
    }
}
