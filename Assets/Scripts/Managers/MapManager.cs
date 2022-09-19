using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : SingletonManager<MapManager> {//这个东西管理和地图有关的所有东西,包括quads
    [SerializeField]//目前还是在外部赋值吧
    private MapConfigurationSO currentMapConfiguration;//只有生成地图的时候会给这个赋值
    public MapConfigurationSO CurrentMapConfiguration {
        get {
            if(currentMapConfiguration == null) {
                Debug.LogError("map configuration is missing!!");
            }
            return currentMapConfiguration;
        }
    }
}
