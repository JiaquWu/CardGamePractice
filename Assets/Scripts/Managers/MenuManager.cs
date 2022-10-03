using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Menu {
    
}
public class MenuManager : SingletonManager<MenuManager> {
    public static Dictionary<Menu,GameObject> menus;
    protected override void Init() {
        menus = new Dictionary<Menu, GameObject>() {

        };
        foreach (var menu in menus.Values) {
            menu.SetActive(false);
        }
    }
}
