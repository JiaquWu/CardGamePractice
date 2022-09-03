using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRulesManager {
    public static Dictionary<int,List<float>> championDropRatesByLevel = new Dictionary<int, List<float>>() {
        {1,new List<float>(){1f,0f,0f,0f,0f}},
        {2,new List<float>(){1f,0f,0f,0f,0f}},
        {3,new List<float>(){0.75f,0.25f,0f,0f,0f}},
        {4,new List<float>(){0.55f,0.3f,0.15f,0f,0f}},
        {5,new List<float>(){0.45f,0.33f,0.2f,0.02f,0f}},
        {6,new List<float>(){0.25f,0.4f,0.3f,0.05f,0f}},
        {7,new List<float>(){0.19f,0.3f,0.35f,0.15f,0.01f}},
        {8,new List<float>(){0.16f,0.2f,0.35f,0.25f,0.04f}},
        {9,new List<float>(){0.09f,0.15f,0.3f,0.3f,0.16f}},
        {10,new List<float>(){0.05f,0.1f,0.2f,0.4f,0.25f}},

    };
    public static Dictionary<int,int> championPoolSizeByLevel = new Dictionary<int, int>() {
        {1,20},
        {2,15},
        {3,13},
        {4,12},
        {5,10},
    };
}
