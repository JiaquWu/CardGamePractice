using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Menu {

}
public class UIManager : SingletonManager<UIManager> {
    static Dictionary<Menu,GameObject> menus;

    void InitDict() {
        if(menus == null) {
            menus = new Dictionary<Menu, GameObject>();
        }
    }
}
