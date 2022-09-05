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
    public static Dictionary<int,int> experienceRequirementByLevel = new Dictionary<int, int>() {//key级升到下一级还需要value点经验
        {1,2},
        {2,4},
        {3,6},
        {4,10},
        {5,20},
        {6,36},
        {7,56},
        {8,80},
        {9,100},       
    };
    public static int defaultRefreshCost = 2;
    public static int defaultBuyExpCost = 4;
    public static int defaultExpIncrementEachBuy = 4;
}
