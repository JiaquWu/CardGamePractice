using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Menu {
    
}
public class MenuManager : SingletonManager<MenuManager> {
//先不用,后面如果有暂停菜单那种才会用到
    public static Dictionary<Menu,GameObject> menus;
    protected override void Init() {
        menus = new Dictionary<Menu, GameObject>() {

        };
        foreach (var menu in menus.Values) {
            menu.SetActive(false);
        }
    }
}
