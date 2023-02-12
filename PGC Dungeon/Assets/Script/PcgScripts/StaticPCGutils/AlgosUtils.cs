using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = UnityEngine.Random;
using Color = UnityEngine.Color;
using static Unity.VisualScripting.Metadata;


public static class AlgosUtils
{

    #region marching Cubes Rule

    //http://paulbourke.net/geometry/polygonise/

    public static int[,] triTable = new int[256, 16]
{{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 8, 3, 9, 8, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 2, 10, 0, 2, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}, 
{2, 8, 3, 2, 10, 8, 10, 9, 8, -1, -1, -1, -1, -1, -1, -1},
{3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 11, 2, 8, 11, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 11, 2, 1, 9, 11, 9, 8, 11, -1, -1, -1, -1, -1, -1, -1},  
{3, 10, 1, 11, 10, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 10, 1, 0, 8, 10, 8, 11, 10, -1, -1, -1, -1, -1, -1, -1},
{3, 9, 0, 3, 11, 9, 11, 10, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 3, 0, 7, 3, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 1, 9, 4, 7, 1, 7, 3, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 4, 7, 3, 0, 4, 1, 2, 10, -1, -1, -1, -1, -1, -1, -1},
{9, 2, 10, 9, 0, 2, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
{2, 10, 9, 2, 9, 7, 2, 7, 3, 7, 9, 4, -1, -1, -1, -1},
{8, 4, 7, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 4, 7, 11, 2, 4, 2, 0, 4, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 1, 8, 4, 7, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
{4, 7, 11, 9, 4, 11, 9, 11, 2, 9, 2, 1, -1, -1, -1, -1},
{3, 10, 1, 3, 11, 10, 7, 8, 4, -1, -1, -1, -1, -1, -1, -1},
{1, 11, 10, 1, 4, 11, 1, 0, 4, 7, 11, 4, -1, -1, -1, -1},
{4, 7, 8, 9, 0, 11, 9, 11, 10, 11, 0, 3, -1, -1, -1, -1},
{4, 7, 11, 4, 11, 9, 9, 11, 10, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 5, 4, 1, 5, 0, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 5, 4, 8, 3, 5, 3, 1, 5, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 1, 2, 10, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
{5, 2, 10, 5, 4, 2, 4, 0, 2, -1, -1, -1, -1, -1, -1, -1},
{2, 10, 5, 3, 2, 5, 3, 5, 4, 3, 4, 8, -1, -1, -1, -1},
{9, 5, 4, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 11, 2, 0, 8, 11, 4, 9, 5, -1, -1, -1, -1, -1, -1, -1},
{0, 5, 4, 0, 1, 5, 2, 3, 11, -1, -1, -1, -1, -1, -1, -1},
{2, 1, 5, 2, 5, 8, 2, 8, 11, 4, 8, 5, -1, -1, -1, -1},
{10, 3, 11, 10, 1, 3, 9, 5, 4, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 5, 0, 8, 1, 8, 10, 1, 8, 11, 10, -1, -1, -1, -1},
{5, 4, 0, 5, 0, 11, 5, 11, 10, 11, 0, 3, -1, -1, -1, -1},
{5, 4, 8, 5, 8, 10, 10, 8, 11, -1, -1, -1, -1, -1, -1, -1},
{9, 7, 8, 5, 7, 9, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 3, 0, 9, 5, 3, 5, 7, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 7, 8, 0, 1, 7, 1, 5, 7, -1, -1, -1, -1, -1, -1, -1},
{1, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 7, 8, 9, 5, 7, 10, 1, 2, -1, -1, -1, -1, -1, -1, -1},
{10, 1, 2, 9, 5, 0, 5, 3, 0, 5, 7, 3, -1, -1, -1, -1},
{8, 0, 2, 8, 2, 5, 8, 5, 7, 10, 5, 2, -1, -1, -1, -1},
{2, 10, 5, 2, 5, 3, 3, 5, 7, -1, -1, -1, -1, -1, -1, -1},
{7, 9, 5, 7, 8, 9, 3, 11, 2, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 7, 9, 7, 2, 9, 2, 0, 2, 7, 11, -1, -1, -1, -1},
{2, 3, 11, 0, 1, 8, 1, 7, 8, 1, 5, 7, -1, -1, -1, -1},
{11, 2, 1, 11, 1, 7, 7, 1, 5, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 8, 8, 5, 7, 10, 1, 3, 10, 3, 11, -1, -1, -1, -1},
{5, 7, 0, 5, 0, 9, 7, 11, 0, 1, 0, 10, 11, 10, 0, -1},
{11, 10, 0, 11, 0, 3, 10, 5, 0, 8, 0, 7, 5, 7, 0, -1},
{11, 10, 5, 7, 11, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 1, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 8, 3, 1, 9, 8, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 5, 2, 6, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 5, 1, 2, 6, 3, 0, 8, -1, -1, -1, -1, -1, -1, -1},
{9, 6, 5, 9, 0, 6, 0, 2, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 9, 8, 5, 8, 2, 5, 2, 6, 3, 2, 8, -1, -1, -1, -1},
{2, 3, 11, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 0, 8, 11, 2, 0, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 1, 9, 2, 9, 11, 2, 9, 8, 11, -1, -1, -1, -1},
{6, 3, 11, 6, 5, 3, 5, 1, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 11, 0, 11, 5, 0, 5, 1, 5, 11, 6, -1, -1, -1, -1},
{3, 11, 6, 0, 3, 6, 0, 6, 5, 0, 5, 9, -1, -1, -1, -1},
{6, 5, 9, 6, 9, 11, 11, 9, 8, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 3, 0, 4, 7, 3, 6, 5, 10, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 5, 10, 6, 8, 4, 7, -1, -1, -1, -1, -1, -1, -1},
{10, 6, 5, 1, 9, 7, 1, 7, 3, 7, 9, 4, -1, -1, -1, -1},
{6, 1, 2, 6, 5, 1, 4, 7, 8, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 5, 5, 2, 6, 3, 0, 4, 3, 4, 7, -1, -1, -1, -1},
{8, 4, 7, 9, 0, 5, 0, 6, 5, 0, 2, 6, -1, -1, -1, -1},
{7, 3, 9, 7, 9, 4, 3, 2, 9, 5, 9, 6, 2, 6, 9, -1},
{3, 11, 2, 7, 8, 4, 10, 6, 5, -1, -1, -1, -1, -1, -1, -1},
{5, 10, 6, 4, 7, 2, 4, 2, 0, 2, 7, 11, -1, -1, -1, -1},
{0, 1, 9, 4, 7, 8, 2, 3, 11, 5, 10, 6, -1, -1, -1, -1},
{9, 2, 1, 9, 11, 2, 9, 4, 11, 7, 11, 4, 5, 10, 6, -1},
{8, 4, 7, 3, 11, 5, 3, 5, 1, 5, 11, 6, -1, -1, -1, -1},
{5, 1, 11, 5, 11, 6, 1, 0, 11, 7, 11, 4, 0, 4, 11, -1},
{0, 5, 9, 0, 6, 5, 0, 3, 6, 11, 6, 3, 8, 4, 7, -1},
{6, 5, 9, 6, 9, 11, 4, 7, 9, 7, 11, 9, -1, -1, -1, -1},
{10, 4, 9, 6, 4, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 10, 6, 4, 9, 10, 0, 8, 3, -1, -1, -1, -1, -1, -1, -1},
{10, 0, 1, 10, 6, 0, 6, 4, 0, -1, -1, -1, -1, -1, -1, -1},
{8, 3, 1, 8, 1, 6, 8, 6, 4, 6, 1, 10, -1, -1, -1, -1},
{1, 4, 9, 1, 2, 4, 2, 6, 4, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 1, 2, 9, 2, 4, 9, 2, 6, 4, -1, -1, -1, -1},
{0, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 3, 2, 8, 2, 4, 4, 2, 6, -1, -1, -1, -1, -1, -1, -1},
{10, 4, 9, 10, 6, 4, 11, 2, 3, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 2, 2, 8, 11, 4, 9, 10, 4, 10, 6, -1, -1, -1, -1},
{3, 11, 2, 0, 1, 6, 0, 6, 4, 6, 1, 10, -1, -1, -1, -1},
{6, 4, 1, 6, 1, 10, 4, 8, 1, 2, 1, 11, 8, 11, 1, -1},
{9, 6, 4, 9, 3, 6, 9, 1, 3, 11, 6, 3, -1, -1, -1, -1},
{8, 11, 1, 8, 1, 0, 11, 6, 1, 9, 1, 4, 6, 4, 1, -1},
{3, 11, 6, 3, 6, 0, 0, 6, 4, -1, -1, -1, -1, -1, -1, -1},
{6, 4, 8, 11, 6, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 10, 6, 7, 8, 10, 8, 9, 10, -1, -1, -1, -1, -1, -1, -1},
{0, 7, 3, 0, 10, 7, 0, 9, 10, 6, 7, 10, -1, -1, -1, -1},
{10, 6, 7, 1, 10, 7, 1, 7, 8, 1, 8, 0, -1, -1, -1, -1},
{10, 6, 7, 10, 7, 1, 1, 7, 3, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 6, 1, 6, 8, 1, 8, 9, 8, 6, 7, -1, -1, -1, -1},
{2, 6, 9, 2, 9, 1, 6, 7, 9, 0, 9, 3, 7, 3, 9, -1},
{7, 8, 0, 7, 0, 6, 6, 0, 2, -1, -1, -1, -1, -1, -1, -1},
{7, 3, 2, 6, 7, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 11, 10, 6, 8, 10, 8, 9, 8, 6, 7, -1, -1, -1, -1},
{2, 0, 7, 2, 7, 11, 0, 9, 7, 6, 7, 10, 9, 10, 7, -1},
{1, 8, 0, 1, 7, 8, 1, 10, 7, 6, 7, 10, 2, 3, 11, -1},
{11, 2, 1, 11, 1, 7, 10, 6, 1, 6, 7, 1, -1, -1, -1, -1},
{8, 9, 6, 8, 6, 7, 9, 1, 6, 11, 6, 3, 1, 3, 6, -1},
{0, 9, 1, 11, 6, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 8, 0, 7, 0, 6, 3, 11, 0, 11, 6, 0, -1, -1, -1, -1},
{7, 11, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 8, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 9, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 1, 9, 8, 3, 1, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
{10, 1, 2, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 3, 0, 8, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
{2, 9, 0, 2, 10, 9, 6, 11, 7, -1, -1, -1, -1, -1, -1, -1},
{6, 11, 7, 2, 10, 3, 10, 8, 3, 10, 9, 8, -1, -1, -1, -1},
{7, 2, 3, 6, 2, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{7, 0, 8, 7, 6, 0, 6, 2, 0, -1, -1, -1, -1, -1, -1, -1},
{2, 7, 6, 2, 3, 7, 0, 1, 9, -1, -1, -1, -1, -1, -1, -1},
{1, 6, 2, 1, 8, 6, 1, 9, 8, 8, 7, 6, -1, -1, -1, -1},
{10, 7, 6, 10, 1, 7, 1, 3, 7, -1, -1, -1, -1, -1, -1, -1},
{10, 7, 6, 1, 7, 10, 1, 8, 7, 1, 0, 8, -1, -1, -1, -1},
{0, 3, 7, 0, 7, 10, 0, 10, 9, 6, 10, 7, -1, -1, -1, -1},
{7, 6, 10, 7, 10, 8, 8, 10, 9, -1, -1, -1, -1, -1, -1, -1},
{6, 8, 4, 11, 8, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 6, 11, 3, 0, 6, 0, 4, 6, -1, -1, -1, -1, -1, -1, -1},
{8, 6, 11, 8, 4, 6, 9, 0, 1, -1, -1, -1, -1, -1, -1, -1},   //
{9, 4, 6, 9, 6, 3, 9, 3, 1, 11, 3, 6, -1, -1, -1, -1},
{6, 8, 4, 6, 11, 8, 2, 10, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 3, 0, 11, 0, 6, 11, 0, 4, 6, -1, -1, -1, -1},
{4, 11, 8, 4, 6, 11, 0, 2, 9, 2, 10, 9, -1, -1, -1, -1},
{10, 9, 3, 10, 3, 2, 9, 4, 3, 11, 3, 6, 4, 6, 3, -1},
{8, 2, 3, 8, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1},
{0, 4, 2, 4, 6, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 9, 0, 2, 3, 4, 2, 4, 6, 4, 3, 8, -1, -1, -1, -1},
{1, 9, 4, 1, 4, 2, 2, 4, 6, -1, -1, -1, -1, -1, -1, -1},
{8, 1, 3, 8, 6, 1, 8, 4, 6, 6, 10, 1, -1, -1, -1, -1},
{10, 1, 0, 10, 0, 6, 6, 0, 4, -1, -1, -1, -1, -1, -1, -1},
{4, 6, 3, 4, 3, 8, 6, 10, 3, 0, 3, 9, 10, 9, 3, -1},
{10, 9, 4, 6, 10, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 5, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 4, 9, 5, 11, 7, 6, -1, -1, -1, -1, -1, -1, -1},
{5, 0, 1, 5, 4, 0, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
{11, 7, 6, 8, 3, 4, 3, 5, 4, 3, 1, 5, -1, -1, -1, -1},
{9, 5, 4, 10, 1, 2, 7, 6, 11, -1, -1, -1, -1, -1, -1, -1},
{6, 11, 7, 1, 2, 10, 0, 8, 3, 4, 9, 5, -1, -1, -1, -1},
{7, 6, 11, 5, 4, 10, 4, 2, 10, 4, 0, 2, -1, -1, -1, -1},
{3, 4, 8, 3, 5, 4, 3, 2, 5, 10, 5, 2, 11, 7, 6, -1},
{7, 2, 3, 7, 6, 2, 5, 4, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 5, 4, 0, 8, 6, 0, 6, 2, 6, 8, 7, -1, -1, -1, -1},
{3, 6, 2, 3, 7, 6, 1, 5, 0, 5, 4, 0, -1, -1, -1, -1},
{6, 2, 8, 6, 8, 7, 2, 1, 8, 4, 8, 5, 1, 5, 8, -1},
{9, 5, 4, 10, 1, 6, 1, 7, 6, 1, 3, 7, -1, -1, -1, -1},
{1, 6, 10, 1, 7, 6, 1, 0, 7, 8, 7, 0, 9, 5, 4, -1},
{4, 0, 10, 4, 10, 5, 0, 3, 10, 6, 10, 7, 3, 7, 10, -1},
{7, 6, 10, 7, 10, 8, 5, 4, 10, 4, 8, 10, -1, -1, -1, -1},
{6, 9, 5, 6, 11, 9, 11, 8, 9, -1, -1, -1, -1, -1, -1, -1},
{3, 6, 11, 0, 6, 3, 0, 5, 6, 0, 9, 5, -1, -1, -1, -1},
{0, 11, 8, 0, 5, 11, 0, 1, 5, 5, 6, 11, -1, -1, -1, -1},
{6, 11, 3, 6, 3, 5, 5, 3, 1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 10, 9, 5, 11, 9, 11, 8, 11, 5, 6, -1, -1, -1, -1},
{0, 11, 3, 0, 6, 11, 0, 9, 6, 5, 6, 9, 1, 2, 10, -1},
{11, 8, 5, 11, 5, 6, 8, 0, 5, 10, 5, 2, 0, 2, 5, -1},
{6, 11, 3, 6, 3, 5, 2, 10, 3, 10, 5, 3, -1, -1, -1, -1},
{5, 8, 9, 5, 2, 8, 5, 6, 2, 3, 8, 2, -1, -1, -1, -1},
{9, 5, 6, 9, 6, 0, 0, 6, 2, -1, -1, -1, -1, -1, -1, -1},
{1, 5, 8, 1, 8, 0, 5, 6, 8, 3, 8, 2, 6, 2, 8, -1},
{1, 5, 6, 2, 1, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 3, 6, 1, 6, 10, 3, 8, 6, 5, 6, 9, 8, 9, 6, -1},
{10, 1, 0, 10, 0, 6, 9, 5, 0, 5, 6, 0, -1, -1, -1, -1},
{0, 3, 8, 5, 6, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{10, 5, 6, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 5, 10, 7, 5, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{11, 5, 10, 11, 7, 5, 8, 3, 0, -1, -1, -1, -1, -1, -1, -1},
{5, 11, 7, 5, 10, 11, 1, 9, 0, -1, -1, -1, -1, -1, -1, -1},
{10, 7, 5, 10, 11, 7, 9, 8, 1, 8, 3, 1, -1, -1, -1, -1},
{11, 1, 2, 11, 7, 1, 7, 5, 1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 1, 2, 7, 1, 7, 5, 7, 2, 11, -1, -1, -1, -1},
{9, 7, 5, 9, 2, 7, 9, 0, 2, 2, 11, 7, -1, -1, -1, -1},
{7, 5, 2, 7, 2, 11, 5, 9, 2, 3, 2, 8, 9, 8, 2, -1},
{2, 5, 10, 2, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1},
{8, 2, 0, 8, 5, 2, 8, 7, 5, 10, 2, 5, -1, -1, -1, -1},
{9, 0, 1, 5, 10, 3, 5, 3, 7, 3, 10, 2, -1, -1, -1, -1},
{9, 8, 2, 9, 2, 1, 8, 7, 2, 10, 2, 5, 7, 5, 2, -1},
{1, 3, 5, 3, 7, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 7, 0, 7, 1, 1, 7, 5, -1, -1, -1, -1, -1, -1, -1},
{9, 0, 3, 9, 3, 5, 5, 3, 7, -1, -1, -1, -1, -1, -1, -1},
{9, 8, 7, 5, 9, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{5, 8, 4, 5, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1},
{5, 0, 4, 5, 11, 0, 5, 10, 11, 11, 3, 0, -1, -1, -1, -1},
{0, 1, 9, 8, 4, 10, 8, 10, 11, 10, 4, 5, -1, -1, -1, -1},
{10, 11, 4, 10, 4, 5, 11, 3, 4, 9, 4, 1, 3, 1, 4, -1},
{2, 5, 1, 2, 8, 5, 2, 11, 8, 4, 5, 8, -1, -1, -1, -1},
{0, 4, 11, 0, 11, 3, 4, 5, 11, 2, 11, 1, 5, 1, 11, -1},
{0, 2, 5, 0, 5, 9, 2, 11, 5, 4, 5, 8, 11, 8, 5, -1},
{9, 4, 5, 2, 11, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 5, 10, 3, 5, 2, 3, 4, 5, 3, 8, 4, -1, -1, -1, -1},
{5, 10, 2, 5, 2, 4, 4, 2, 0, -1, -1, -1, -1, -1, -1, -1},
{3, 10, 2, 3, 5, 10, 3, 8, 5, 4, 5, 8, 0, 1, 9, -1},
{5, 10, 2, 5, 2, 4, 1, 9, 2, 9, 4, 2, -1, -1, -1, -1},
{8, 4, 5, 8, 5, 3, 3, 5, 1, -1, -1, -1, -1, -1, -1, -1},
{0, 4, 5, 1, 0, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{8, 4, 5, 8, 5, 3, 9, 0, 5, 0, 3, 5, -1, -1, -1, -1},
{9, 4, 5, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 11, 7, 4, 9, 11, 9, 10, 11, -1, -1, -1, -1, -1, -1, -1},
{0, 8, 3, 4, 9, 7, 9, 11, 7, 9, 10, 11, -1, -1, -1, -1},
{1, 10, 11, 1, 11, 4, 1, 4, 0, 7, 4, 11, -1, -1, -1, -1},
{3, 1, 4, 3, 4, 8, 1, 10, 4, 7, 4, 11, 10, 11, 4, -1},
{4, 11, 7, 9, 11, 4, 9, 2, 11, 9, 1, 2, -1, -1, -1, -1},
{9, 7, 4, 9, 11, 7, 9, 1, 11, 2, 11, 1, 0, 8, 3, -1},
{11, 7, 4, 11, 4, 2, 2, 4, 0, -1, -1, -1, -1, -1, -1, -1},
{11, 7, 4, 11, 4, 2, 8, 3, 4, 3, 2, 4, -1, -1, -1, -1},
{2, 9, 10, 2, 7, 9, 2, 3, 7, 7, 4, 9, -1, -1, -1, -1},
{9, 10, 7, 9, 7, 4, 10, 2, 7, 8, 7, 0, 2, 0, 7, -1},
{3, 7, 10, 3, 10, 2, 7, 4, 10, 1, 10, 0, 4, 0, 10, -1},
{1, 10, 2, 8, 7, 4, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 1, 4, 1, 7, 7, 1, 3, -1, -1, -1, -1, -1, -1, -1},
{4, 9, 1, 4, 1, 7, 0, 8, 1, 8, 7, 1, -1, -1, -1, -1},
{4, 0, 3, 7, 4, 3, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{4, 8, 7, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{9, 10, 8, 10, 11, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 9, 3, 9, 11, 11, 9, 10, -1, -1, -1, -1, -1, -1, -1},
{0, 1, 10, 0, 10, 8, 8, 10, 11, -1, -1, -1, -1, -1, -1, -1},
{3, 1, 10, 11, 3, 10, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 2, 11, 1, 11, 9, 9, 11, 8, -1, -1, -1, -1, -1, -1, -1},
{3, 0, 9, 3, 9, 11, 1, 2, 9, 2, 11, 9, -1, -1, -1, -1},
{0, 2, 11, 8, 0, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{3, 2, 11, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 8, 2, 8, 10, 10, 8, 9, -1, -1, -1, -1, -1, -1, -1},
{9, 10, 2, 0, 9, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{2, 3, 8, 2, 8, 10, 0, 1, 8, 1, 10, 8, -1, -1, -1, -1},
{1, 10, 2, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{1, 3, 8, 9, 1, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 9, 1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{0, 3, 8, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1},
{-1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1}};

    #endregion

    #region PathFinding

    #region A*

    public static Tuple<List<BasicTile>, List<BasicTile>> A_StarPathfinding2DNorm(BasicTile[][] tileArray2D, Vector2Int start, Vector2Int end, bool euclideanDis = true, bool perf = false, bool diagonalTiles = false, bool useWeights = false, float[] arrWeights =null)
    {
        int timerStart = Environment.TickCount & Int32.MaxValue;

        bool checkForUse = useWeights == true && arrWeights != null ? true : false;

        List<AStar_Node> openList = new List<AStar_Node>();
        List<AStar_Node> closedList = new List<AStar_Node>();

        AStar_Node start_node = new AStar_Node(tileArray2D[start.y][start.x]);
        start_node.parent = null;

        AStar_Node end_node = new AStar_Node(tileArray2D[end.y][end.x]);

        int[,] childPosArry = new int[0, 0];

        if (diagonalTiles)
            childPosArry = GeneralUtil.childPosArry8Side;
        else
            childPosArry = GeneralUtil.childPosArry4Side;

        openList.Add(start_node);

        while (openList.Count > 0)
        {

            AStar_Node currNode = openList[0];
            int currIndex = 0;
            for (int i = 0; i < openList.Count; i++)
            {
                if (openList[i].f < currNode.f)
                {
                    currNode = openList[i];
                    currIndex = i;
                }
            }

            openList.RemoveAt(currIndex);

            closedList.Add(currNode);

            if (currNode.refToBasicTile.position.x == end_node.refToBasicTile.position.x && currNode.refToBasicTile.position.y == end_node.refToBasicTile.position.y)
            {
                List<AStar_Node> path = new List<AStar_Node>();

                AStar_Node current = currNode;

                while (current.parent != null)
                {
                    path.Add(current);
                    current = current.parent;
                }

                int timerEnd_ = Environment.TickCount & Int32.MaxValue;
                int totalTicks_ = timerEnd_ - timerStart;

                if (perf) { Debug.Log($"<color=yellow>Performance: The total time that destorying all the children has taken was {totalTicks_}</color>"); }

                var pathOfBasicTiles = new List<BasicTile>();

                foreach (var tile in path)
                {
                    pathOfBasicTiles.Add(tile.refToBasicTile);
                }

                var allVisiteBasicTiles = new List<BasicTile>();
                foreach (var tile in openList)
                {
                    allVisiteBasicTiles.Add(tile.refToBasicTile);
                }

                return new Tuple<List<BasicTile>, List<BasicTile>>(pathOfBasicTiles, allVisiteBasicTiles);
            }
            else
            {
                List<AStar_Node> children = new List<AStar_Node>();

                for (int i = 0; i < childPosArry.Length / 2; i++)
                {
                    int x_buff = childPosArry[i, 0];
                    int y_buff = childPosArry[i, 1];

                    int[] node_position = { currNode.refToBasicTile.position.x + x_buff, currNode.refToBasicTile.position.y + y_buff };


                    if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= tileArray2D[0].Length || node_position[1] >= tileArray2D.Length)
                    {
                        continue;
                    }
                    else
                    {
                        //here an if statment also saying that walkable 
                        AStar_Node new_node = new AStar_Node(tileArray2D[node_position[1]][node_position[0]]);
                        children.Add(new_node);
                    }
                }

                foreach (var child in children)
                {
                    foreach (var closedListItem in closedList)
                    {
                        if (child.refToBasicTile.position.x == closedListItem.refToBasicTile.position.x && child.refToBasicTile.position.y == closedListItem.refToBasicTile.position.y)
                        {
                            continue;
                        }
                    }


                    child.g = currNode.g + 0.5f;

                    if (euclideanDis)
                        child.h = GeneralUtil.EuclideanDistance2D(new Vector2(end_node.refToBasicTile.position.x, end_node.refToBasicTile.position.y), new Vector2(child.refToBasicTile.position.x, child.refToBasicTile.position.y));
                    else
                        child.h = GeneralUtil.ManhattanDistance2D(new Vector2(end_node.refToBasicTile.position.x, end_node.refToBasicTile.position.y), new Vector2(child.refToBasicTile.position.x, child.refToBasicTile.position.y));



                    if (checkForUse) 
                    {
                        child.f = child.g + child.h + arrWeights[(int)child.refToBasicTile.tileType];   //added value here
                        child.parent = currNode;
                    }
                    else 
                    {
                        child.f = child.g + child.h;   //added value here
                        child.parent = currNode;
                    }


                    foreach (var openListItem in openList)
                    {
                        if (child.refToBasicTile.position.x == openListItem.refToBasicTile.position.x && child.refToBasicTile.position.y == openListItem.refToBasicTile.position.y && child.g > openListItem.g)// 
                        {
                            continue;
                        }
                    }

                    openList.Add(child);

                }
            }
        }



        int timerEnd = Environment.TickCount & Int32.MaxValue;
        int totalTicks = timerEnd - timerStart;

        if (perf) { Debug.Log($"<color=yellow>Performance: The total time that destorying all the children has taken was {totalTicks}</color>"); }


        return null;

    }

    #endregion

    #region beizier
    public static Vector3 CubicBeizier(Vector2Int pos1, Vector2Int pos2, Vector2Int pos3, Vector2Int pos4, float t)
    {
        var correctedPos1 = new Vector3(pos1.x, 0, pos1.y);
        var correctedPos2 = new Vector3(pos2.x, 0, pos2.y);
        var correctedPos3 = new Vector3(pos3.x, 0, pos3.y);
        var correctedPos4 = new Vector3(pos4.x, 0, pos4.y);

        return (Mathf.Pow((1 - t), 3) * correctedPos1) + (3 * (Mathf.Pow((1 - t), 2)) * t * correctedPos2) + (3 * (1 - t) * t * t * correctedPos3) + t * t * t * correctedPos4;
    }

    public static Tuple<Vector2, Vector2> ExtrapolatePos(Vector2 startPos, Vector2 EndPos, int margin)
    {
        float lerpPoint2 = Random.Range(0.15f, 0.40f);
        float lerpPoint3 = Random.Range(0.60f, 0.80f);

        margin = Mathf.Abs(margin);

        Vector2 dir = startPos - EndPos;

        var normalised = Vector2.Perpendicular(dir).normalized;
        var point2 = Vector2.Lerp(startPos, EndPos, lerpPoint2);
        point2 = point2 + normalised * Random.Range(margin * -1, margin);


        normalised = Vector2.Perpendicular(dir).normalized;
        var point3 = Vector2.Lerp(startPos, EndPos, lerpPoint3);
        point3 = point3 + normalised * Random.Range(margin * -1, margin);


        return Tuple.Create(point2, point3);
    }


    #endregion

    #region Dijstra

    public static List<BasicTile> DijstraPathfinding(BasicTile[][] gridArr2d, Vector2Int startPoint, Vector2Int endPoint, bool avoidWalls = false) 
    {


        int[,] childPosArry = new int[0, 0];

        childPosArry = GeneralUtil.childPosArry4Side;

        List<DjNode> openListDjNodes = new List<DjNode>();
        DjNode[][] DjNodesArr = new DjNode[gridArr2d.Length][];

        for (int y = 0; y < gridArr2d.Length; y++)
        {
            DjNodesArr[y] = new DjNode[gridArr2d[0].Length];
            for (int x = 0; x < gridArr2d[0].Length; x++)
            {

                if (avoidWalls)
                {
                    if (gridArr2d[y][x].tileType != BasicTile.TileType.WALLCORRIDOR)
                    {
                        var newRef = new DjNode() { coord = new Vector2Int(x, y), distance = startPoint == new Vector2Int(x, y) ? 0 : 9999999, gridRefTile = gridArr2d[y][x], parentDJnode = null };

                        DjNodesArr[y][x] = newRef;
                        openListDjNodes.Add(newRef);

                    }
                }
                else
                {
                    var newRef = new DjNode() { coord = new Vector2Int(x, y), distance = startPoint == new Vector2Int(x, y) ? 0 : 9999999, gridRefTile = gridArr2d[y][x], parentDJnode = null };

                    DjNodesArr[y][x] = newRef;
                    openListDjNodes.Add(newRef);
                }
            }
        }

        DjNode lastNode = null;

        while (openListDjNodes.Count > 1)
        {
            float smallestDist = 99999999;
            DjNode currNode = null;

            foreach (var djNode in openListDjNodes)
            {
                if (djNode.distance < smallestDist) 
                {
                    smallestDist = djNode.distance;
                    currNode = djNode;
                }
            }


            openListDjNodes.Remove(currNode);


            for (int i = 0; i < childPosArry.Length / 2; i++)
            {
                int x_buff = childPosArry[i, 0];
                int y_buff = childPosArry[i, 1];

                int[] node_position = { currNode.coord.x + x_buff, currNode.coord.y + y_buff };


                if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= gridArr2d[0].Length || node_position[1] >= gridArr2d.Length)
                {
                    continue;
                }
                else
                {

                    if (avoidWalls)
                    {
                        if (gridArr2d[node_position[1]][node_position[0]].tileType != BasicTile.TileType.WALLCORRIDOR)
                        {
                            float newDist = currNode.distance + 1;

                            if (newDist < DjNodesArr[node_position[1]][node_position[0]].distance)
                            {
                                DjNodesArr[node_position[1]][node_position[0]].distance = newDist;
                                DjNodesArr[node_position[1]][node_position[0]].parentDJnode = currNode;

                            }

                        }
                    }
                    else
                    {
                        float newDist = currNode.distance + 1;

                        if (newDist < DjNodesArr[node_position[1]][node_position[0]].distance)
                        {
                            DjNodesArr[node_position[1]][node_position[0]].distance = newDist;
                            DjNodesArr[node_position[1]][node_position[0]].parentDJnode = currNode;

                        }
                    }

                }
            }

            if (currNode.coord == endPoint)
            {
                lastNode = currNode;

                break;
            }

        }

        var solutioPath = new List<BasicTile>();

        while(lastNode.parentDJnode != null) 
        {
            solutioPath.Add(lastNode.gridRefTile);

            lastNode = lastNode.parentDJnode;
        }

        return solutioPath;


    }

    #endregion


    #region BFS  -- to do

    public static void BFSPathFinding() 
    {
        
    }

    #endregion

    #region DFS  -- to do

    public static void DFSPathFinding()
    {

    }

    #endregion

    #endregion

    #region Random Walk


    public static BasicTile[][] RandomWalk2DCol(int iterations, bool alreadyPassed, int maxX, int maxY, float maxIterMultiplier = 1.4f, bool randomStart = true)
    {
        int iterationsLeft = iterations;


        BasicTile[][] _gridarray2D = new BasicTile[maxY][];

        for (int y = 0; y < maxY; y++)
        {
            _gridarray2D[y] = new BasicTile[maxX];

            for (int x = 0; x < maxX; x++)
            {
                _gridarray2D[y][x] = new BasicTile();
                _gridarray2D[y][x].position = new Vector2Int(x, y);
            }
        }


        Vector2Int currentHead = new Vector2Int(maxX / 2, maxY / 2);

        if (randomStart)
             currentHead = GeneralUtil.RanVector2Int(_gridarray2D[0].Length, _gridarray2D.Length);


        while (iterationsLeft > 0)
        {

      
            int ranDir = Random.Range(0, 4);

            switch (ranDir)
            {
                case 0:    //for

                    if (currentHead.y + 1 >= _gridarray2D.Length)
                    { }
                    else
                    {
                        currentHead.y++;
                    }

                    break;

                case 1:    //back
                    if (currentHead.y - 1 < 0)
                    { }
                    else
                    {
                        currentHead.y--;
                    }
                    break;

                case 2:    //left
                    if (currentHead.x - 1 < 0)
                    { }
                    else
                    {
                        currentHead.x--;
                    }
                    break;

                case 3:   //rigth
                    if (currentHead.x + 1 >= _gridarray2D[0].Length)
                    { }
                    else
                    {
                        currentHead.x++;
                    }
                    break;

                default:
                    break;
            }


            if (alreadyPassed)
            {
                if (_gridarray2D[(int)currentHead.y][(int)currentHead.x].tileWeight != 1)
                {
                    _gridarray2D[(int)currentHead.y][(int)currentHead.x].tileWeight = 1;
                    iterationsLeft--;
                }
            }
            else
            {
                _gridarray2D[(int)currentHead.y][(int)currentHead.x].tileWeight = 1;
                iterationsLeft--;
            }
        }

        return _gridarray2D;

    }


    public static BasicTile[][] CompartimentalisedRandomWalk(BoundsInt boundsRoom)
    {

        int maxY = boundsRoom.zMax - boundsRoom.zMin;
        int maxX = boundsRoom.xMax - boundsRoom.xMin;

        int iterations = (maxX) * (maxY);


        int iterationsLeft = iterations;


        BasicTile[][] _gridarray2D = new BasicTile[maxY][];

        for (int y = 0; y < maxY; y++)
        {
            _gridarray2D[y] = new BasicTile[maxX];

            for (int x = 0; x < maxX; x++)
            {
                _gridarray2D[y][x] = new BasicTile();
                _gridarray2D[y][x].position = new Vector2Int(x, y);
            }
        }


        Vector2Int currentHead = new Vector2Int(maxX / 2, maxY / 2);

        while (iterationsLeft > 0)
        {


            int ranDir = Random.Range(0, 4);

            switch (ranDir)
            {
                case 0:    //for

                    if (currentHead.y + 1 >= _gridarray2D.Length)
                    { }
                    else
                    {
                        currentHead.y++;
                    }

                    break;

                case 1:    //back
                    if (currentHead.y - 1 < 0)
                    { }
                    else
                    {
                        currentHead.y--;
                    }
                    break;

                case 2:    //left
                    if (currentHead.x - 1 < 0)
                    { }
                    else
                    {
                        currentHead.x--;
                    }
                    break;

                case 3:   //rigth
                    if (currentHead.x + 1 >= _gridarray2D[0].Length)
                    { }
                    else
                    {
                        currentHead.x++;
                    }
                    break;

                default:
                    break;
            }


                _gridarray2D[(int)currentHead.y][(int)currentHead.x].tileWeight = 1;
                iterationsLeft--;
            
        }



        RunCaIteration2D(_gridarray2D, 4);


        return _gridarray2D;

    }


    #endregion

    #region PerlinNoise

    public static BasicTile[][] PerlinNoise2D(BasicTile[][] _gridArray2D, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY, float threashold = 0)
    {
        float[,] noiseMap = new float[_gridArray2D[0].Length, _gridArray2D.Length];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxN = float.MinValue;
        float minN = float.MaxValue;


        for (int y = 0; y < _gridArray2D.Length; y++)
        {
            for (int x = 0; x < _gridArray2D[0].Length; x++)
            {

                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;


                for (int i = 0; i < octaves; i++)
                {

                    float sampleX = x / scale * freq + offsetX;
                    float sampleY = y / scale * freq + offsetY;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;

                    freq *= lacu;

                }


                if (noiseHeight > maxN) { maxN = noiseHeight; }
                else if (noiseHeight < minN) { minN = noiseHeight; }

                noiseMap[x, y] = noiseHeight;
            }
        }

        for (int y = 0; y < _gridArray2D.Length; y++)
        {
            for (int x = 0; x < _gridArray2D[0].Length; x++)
            {
                _gridArray2D[y][x].tileWeight = Mathf.InverseLerp(minN, maxN, noiseMap[x, y]);

                if (threashold != 0)
                {
                    if (threashold < _gridArray2D[y][x].tileWeight)
                        _gridArray2D[y][x].tileWeight = 1;
                    else
                        _gridArray2D[y][x].tileWeight = 0;
                }
            }
        }

        return _gridArray2D;
    }

    public static void DrawNoiseMap(Renderer meshRenderer, int widthX, int lengthY, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY, float threshold, bool threshBool)
    {
        var noiseMap = PerlinNoise2DPlane(widthX, lengthY, scale, octaves, persistance, lacu, offsetX, offsetY);

        int width = noiseMap.GetLength(0);
        int height = noiseMap.GetLength(1);

        Texture2D texture = new Texture2D(width, height);

        Color[] colourMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (threshBool)
                {
                    if (threshold > noiseMap[x, y])
                        colourMap[y * width + x] = Color.white;
                    else
                        colourMap[y * width + x] = Color.black;
                }
                else
                    colourMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
            }
        }
        texture.SetPixels(colourMap);
        texture.Apply();


        meshRenderer.sharedMaterial.mainTexture = texture;
        meshRenderer.transform.localScale = new Vector3(width, 1, height);


    }

    public static float[,] PerlinNoise2DPlane(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if (scale <= 0)
        {
            scale = 0.0001f;
        }

        float maxN = float.MinValue;
        float minN = float.MaxValue;


        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                float amplitude = 1;
                float freq = 1;
                float noiseHeight = 0;


                for (int i = 0; i < octaves; i++)
                {

                    float sampleX = x / scale * freq + offsetX;
                    float sampleY = y / scale * freq + offsetY;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;

                    noiseHeight += perlinValue * amplitude;

                    amplitude *= persistance;

                    freq *= lacu;

                }


                if (noiseHeight > maxN) { maxN = noiseHeight; }
                else if (noiseHeight < minN) { minN = noiseHeight; }

                noiseMap[x, y] = noiseHeight;
            }
        }
        for (int y = 0; y < mapHeight; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minN, maxN, noiseMap[x, y]);
            }
        }




        return noiseMap;
    }

    public static BasicTile[][] PerlinWorms(BasicTile[][] _gridArray2D, float scale, int octaves, float persistance, float lacu, int offsetX, int offsetY, float threshold, float minThreshold) 
    {
        // we create a copy arrya
        int height = _gridArray2D.Length;
        int width = _gridArray2D[0].Length;
        var gridArray2DToReturn = new BasicTile[height][];

        for (int y = 0; y < height; y++)
        {
            gridArray2DToReturn[y] = new BasicTile[width];

            for (int x = 0; x < width; x++)
            {
                gridArray2DToReturn[y][x] = new BasicTile();
                gridArray2DToReturn[y][x].position = new Vector2Int(x, y);
                gridArray2DToReturn[y][x].tileType = BasicTile.TileType.VOID;
            }
        }


        _gridArray2D = PerlinNoise2D(_gridArray2D,  scale,  octaves,  persistance,  lacu,  offsetX,  offsetY,  threshold);

        var rooms = GetAllRooms(_gridArray2D);

        _gridArray2D = PerlinNoise2D(_gridArray2D, scale, octaves, persistance, lacu, offsetX, offsetY);

        foreach (var roomTiles in rooms)
        {
            var currentPos = roomTiles[Random.Range(0, roomTiles.Count)].position;

            bool foundSmaller = true;

            int[,] childPosArry = new int[0, 0];
            int[] savedNextNode = new int[2];

            childPosArry = GeneralUtil.childPosArry8Side;

            gridArray2DToReturn[currentPos.y][currentPos.x].tileWeight = 1;

            while (foundSmaller) 
            {

                float savedWeight = _gridArray2D[currentPos.y][currentPos.x].tileWeight;
                foundSmaller = false;
               

                for (int i = 0; i < childPosArry.Length / 2; i++)
                {
                    int x_buff = childPosArry[i, 0];
                    int y_buff = childPosArry[i, 1];

                    int[] node_position = { currentPos.x + x_buff, currentPos.y + y_buff };


                    if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= _gridArray2D[0].Length || node_position[1] >= _gridArray2D.Length)
                    {
                        continue;
                    }
                    else
                    {
                        if (savedWeight > _gridArray2D[node_position[1]][node_position[0]].tileWeight)   // the weight is smaller
                        {
                            foundSmaller = true;
                            savedNextNode = node_position;
                            savedWeight = _gridArray2D[node_position[1]][node_position[0]].tileWeight;
                            gridArray2DToReturn[node_position[1]][node_position[0]].tileWeight = 1;
                        }
                    }
                }
                if (_gridArray2D[savedNextNode[1]][savedNextNode[0]].tileWeight <= minThreshold)
                {
                    foundSmaller = false;
                }
                else 
                {
                    gridArray2DToReturn[savedNextNode[1]][savedNextNode[0]].tileType = BasicTile.TileType.FLOORCORRIDOR;
                    currentPos = new Vector2Int(savedNextNode[0], savedNextNode[1]);
                }

            }
        }

        return gridArray2DToReturn;
    }


    #endregion

    #region Triangulation

    public static List<Edge> PrimAlgoNoDelu(List<Vector2> points)
    {
        var triangulation = DelunayTriangulation2D(points);

        return PrimAlgo(points, triangulation.Item1);
    }

    public static List<Edge> PrimAlgo(List<Vector2> points, List<Triangle> triangulation)
    {
        List<Edge> primsAlgo = new List<Edge>();

        HashSet<Vector2> visitedVertices = new HashSet<Vector2>();

        var ran = Random.Range(0, points.Count);
        var vertex = points[ran];

        visitedVertices.Add(vertex);

        while (visitedVertices.Count != points.Count)
        {

            HashSet<Edge> edgesWithPoint = new HashSet<Edge>();

            foreach (var trig in triangulation)    // we get all the edges
            {
                foreach (var edge in trig.edges)
                {
                    foreach (var point in visitedVertices)
                    {
                        if (visitedVertices.Contains(edge.edge[0]) && visitedVertices.Contains(edge.edge[0]))
                        {
                            // do nothing
                        }
                        else if (visitedVertices.Contains(edge.edge[0]))
                        {
                            edgesWithPoint.Add(edge);
                        }
                        else if (visitedVertices.Contains(edge.edge[1]))
                        {
                            edgesWithPoint.Add(edge);
                        }
                    }
                }
            }

            var edgesWithPointSort = edgesWithPoint.OrderBy(c => c.length).ToArray();   // we sort all the edges by the smallest to biggest


            visitedVertices.Add(edgesWithPointSort[0].edge[0]);
            visitedVertices.Add(edgesWithPointSort[0].edge[1]);
            primsAlgo.Add(edgesWithPointSort[0]);
        }


        return primsAlgo;
    }

    public static Tuple<List<Triangle>, List<Edge>> DelunayTriangulation2D(List<Vector2> points)
    {
        var triangulation = new List<Triangle>();

        Vector2 superTriangleA = new Vector2(10000, 10000 );
        Vector2 superTriangleB = new Vector2(10000, 0);
        Vector2 superTriangleC = new Vector2(0, 10000);

        triangulation.Add(new Triangle(superTriangleA, superTriangleB, superTriangleC));

        foreach (Vector2 point in points)
        {
            List<Triangle> badTriangles = new List<Triangle>();

            foreach (Triangle triangle in triangulation)
            {
                if (IspointInCircumcircle(triangle.a, triangle.b, triangle.c, point))
                {
                    badTriangles.Add(triangle);
                }
            }

            List<Edge> polygon = new List<Edge>();

            foreach (Triangle triangle in badTriangles)
            {
                foreach (Edge triangleEdge in triangle.edges)
                {
                    bool isShared = false;

                    foreach (Triangle otherTri in badTriangles)
                    {
                        if (otherTri == triangle) { continue; }

                        foreach (Edge otherEdge in otherTri.edges)
                        {
                            if (LineIsEqual(triangleEdge, otherEdge))
                            {
                                isShared = true;
                            }
                        }
                    }

                    if (isShared == false)
                    {
                        polygon.Add(triangleEdge);
                    }

                }
            }

            foreach (Triangle badTriangle in badTriangles)
            {
                triangulation.Remove(badTriangle);   // i think this is the issue here
            }

            foreach (Edge edge in polygon)
            {
                Triangle newTriangle = new Triangle(edge.edge[0], edge.edge[1], point);
                triangulation.Add(newTriangle);
            }
        }

        for (int i = triangulation.Count - 1; i >= 0; i--)
        {
            if (triangulation[i].HasVertex(superTriangleA) || triangulation[i].HasVertex(superTriangleB) || triangulation[i].HasVertex(superTriangleC))
            {
                triangulation.Remove(triangulation[i]);
            }
        }



        var edges = new List<Edge>();

        foreach (var tri in triangulation)
        {
            foreach (var edge in tri.edges)
            {
                edges.Add(edge);
            }
        }


        return new Tuple<List<Triangle>, List<Edge>>(triangulation, edges) ;

    }

    public static bool LineIsEqual(Edge A, Edge B)
    {
        if ((A.edge[0] == B.edge[0] && A.edge[1] == B.edge[1]) || (A.edge[0] == B.edge[1] && A.edge[1] == B.edge[0])) { return true; }
        else { return false; }
    }

    public static bool IspointInCircumcircle(Vector3 A, Vector3 B, Vector3 C, Vector3 D)
    {


        float ax_ = A[0] - D[0];
        float ay_ = A[1] - D[1];
        float bx_ = B[0] - D[0];
        float by_ = B[1] - D[1];
        float cx_ = C[0] - D[0];
        float cy_ = C[1] - D[1];



        if ((
            (ax_ * ax_ + ay_ * ay_) * (bx_ * cy_ - cx_ * by_) -
            (bx_ * bx_ + by_ * by_) * (ax_ * cy_ - cx_ * ay_) +
            (cx_ * cx_ + cy_ * cy_) * (ax_ * by_ - bx_ * ay_)
        ) < 0)
        {
            return true;
        }

        else { return false; }

    }

    #endregion

    #region Binary Partition System

    public static List<BoundsInt> BSPAlgo(BoundsInt toSplit, int minHeight, int minWidth)
    {
        var startTimer = GeneralUtil.PerfTimer(true);

        List<BoundsInt> roomList = new List<BoundsInt>();
        Queue<BoundsInt> roomsQueue = new Queue<BoundsInt>();

        roomsQueue.Enqueue(toSplit);   // enque add to que
        while (roomsQueue.Count > 0)
        {
            var room = roomsQueue.Dequeue();   // take out and split this

            // this room can either contain a room or split  room
            if (room.size.y >= minHeight && room.size.x >= minWidth)   // all rooms should at least be big enough
            {
                if (Random.value < 0.5f)
                {
                    if (room.size.y >= minHeight * 2 + 1)
                    {
                        SplitHori(minHeight, room, roomsQueue);
                    }
                    else if (room.size.x >= minWidth * 2 + 1)
                    {
                        SplitVert(minWidth, room, roomsQueue);
                    }
                    else
                    {
                        roomList.Add(room);
                    }
                }
                else
                {
                    if (room.size.x >= minWidth * 2 + 1)
                    {
                        SplitVert(minWidth, room, roomsQueue);
                    }
                    else if (room.size.y >= minHeight * 2 + 1)
                    {
                        SplitHori(minHeight, room, roomsQueue);
                    }
                    else
                    {
                        roomList.Add(room);
                    }
                }
            }
        }

        var endTimer = GeneralUtil.PerfTimer(false, startTimer);
        return roomList;
    }

    private static void SplitVert(int minWidth, BoundsInt room, Queue<BoundsInt> roomQue)
    {

        int minX = room.min.x;
        int maxX = room.max.x;

        int adjustedMinX = minX + minWidth;
        int adjustedMaxX = maxX - minWidth;

        var ranPosition = Random.Range(adjustedMinX, adjustedMaxX);

        BoundsInt roomLeft = new BoundsInt();

        roomLeft.min = new Vector3Int(room.min.x, room.min.y, 0);
        roomLeft.max = new Vector3Int(ranPosition, room.max.y, 0);

        BoundsInt roomRight = new BoundsInt();

        roomRight.min = new Vector3Int(ranPosition, room.min.y, 0);
        roomRight.max = new Vector3Int(room.max.x, room.max.y, 0);

        roomQue.Enqueue(roomRight);
        roomQue.Enqueue(roomLeft);
    }

    private static void SplitHori(int minHeight, BoundsInt room, Queue<BoundsInt> roomQue)
    {
        int minY = room.min.y;
        int maxY = room.max.y;

        int adjustedMinY = minY + minHeight;
        int adjustedMaxY = maxY - minHeight;

        var ranPosition = Random.Range(adjustedMinY, adjustedMaxY);

        BoundsInt roomTop = new BoundsInt();

        roomTop.min = new Vector3Int(room.min.x, ranPosition, 0);
        roomTop.max = new Vector3Int(room.max.x, room.max.y, 0);

        BoundsInt roomBot = new BoundsInt();

        roomBot.min = new Vector3Int(room.min.x, room.min.y, 0);
        roomBot.max = new Vector3Int(room.max.x, ranPosition, 0);

        roomQue.Enqueue(roomBot);
        roomQue.Enqueue(roomTop);
    }

    #endregion

    #region Flood Fill

    public static void ResetVisited(BasicTile[][] gridArray2D) 
    {
        for (int y = 0; y < gridArray2D.Length; y++)
        {
            for (int x = 0; x < gridArray2D[0].Length; x++)
            {
                gridArray2D[y][x].visited = false;
            }
        }
    }


    /// <summary>
    /// for now this is looking only for the white or wieght = 0 
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="list"></param>
    /// <param name="gridArray2D"></param>
    /// <returns>the returned list holds the coords of everything that was true</returns>
    public static List<Vector2Int> Flood2DAnchor(int x, int y, List<Vector2Int> list, BasicTile[][] gridArray2D)
    {
        
        if (y >= 0 && x >= 0 && y < gridArray2D.Length && x < gridArray2D[y].Length)
        {
            if (gridArray2D[y][x].tileWeight != 0 && gridArray2D[y][x].visited == false)
            {
                list.Add(new Vector2Int(x, y));
                gridArray2D[y][x].visited=true;
                Flood2DAnchor(x + 1, y, list, gridArray2D);
                Flood2DAnchor(x - 1, y, list, gridArray2D);
                Flood2DAnchor(x, y + 1, list, gridArray2D);
                Flood2DAnchor(x, y - 1, list, gridArray2D);
            }
        }

        return list;
    }


    #endregion

    #region Cellular Automata



    /// <summary>
    /// mainly used for CA given a % fill up with wieght 1
    /// </summary>
    /// <param name="gridArr"></param>
    /// <param name="ranValue"></param>
    public static void SpawnRandomPointsCA(BasicTile[][] gridArr, float ranValue) 
    {
        for (int y = 0; y < gridArr.Length; y++)
        {
            for (int x = 0; x < gridArr[0].Length; x++)
            {
                if (Random.value > ranValue) 
                {
                    gridArr[y][x].tileWeight = 0;
                }
                else 
                {
                    gridArr[y][x].tileWeight = 1;
                }
            }
        }
    }


    public static void RunCaIteration2D(BasicTile[][] gridArray2D, int neighboursNeeded) 
    {

        float[][] copyArrayStorage = new float[gridArray2D.Length][];

        for (int y = 0; y < gridArray2D.Length; y++)
        {
            copyArrayStorage[y] = new float[gridArray2D[y].Length];

            for (int x = 0; x < gridArray2D[y].Length; x++)
            {
                copyArrayStorage[y][x] = gridArray2D[y][x].tileWeight;
            }
        }



        for (int y = 0; y < gridArray2D.Length; y++)
        {

            for (int x = 0; x < gridArray2D[y].Length; x++)
            {
                int neighbours = 0;

                for (int col_offset = -1; col_offset < 2; col_offset++)
                {
                    for (int row_offset = -1; row_offset < 2; row_offset++)
                    {

                        if (y + col_offset < 0 || x + row_offset < 0 || y + col_offset >= gridArray2D.Length - 1 || x + row_offset >= gridArray2D[y].Length - 1)
                        {

                        }
                        else if (col_offset == 0 && row_offset == 0)
                        {

                        }
                        else
                        {
                            // this was !
                            if (copyArrayStorage[y + col_offset][x + row_offset] == 1)
                            {
                                neighbours++;
                            }
                        }
                    }
                }

                if (neighbours >= neighboursNeeded)
                {   //empty is = false therefore weight is there
                    gridArray2D[y][x].tileWeight = 1;
                }
                else
                {   //true
                    gridArray2D[y][x].tileWeight = 0;
                }
            }
        }
    }

    public static void CleanUp2dCA(BasicTile[][] gridArray2D, int neighboursNeeded)
    {

        float[][] copyArrayStorage = new float[gridArray2D.Length][];

        for (int y = 0; y < gridArray2D.Length; y++)
        {
            copyArrayStorage[y] = new float[gridArray2D[y].Length];

            for (int x = 0; x < gridArray2D[y].Length; x++)
            {
                copyArrayStorage[y][x] = gridArray2D[y][x].tileWeight;
            }
        }



        for (int y = 0; y < gridArray2D.Length; y++)
        {

            for (int x = 0; x < gridArray2D[y].Length; x++)
            {
                int neighbours = 0;
                if (copyArrayStorage[y][x] == 1)
                {
                    for (int col_offset = -1; col_offset < 2; col_offset++)
                    {
                        for (int row_offset = -1; row_offset < 2; row_offset++)
                        {

                            if (y + col_offset < 0 || x + row_offset < 0 || y + col_offset >= gridArray2D.Length - 1 || x + row_offset >= gridArray2D[y].Length - 1)
                            {

                            }
                            else if (col_offset == 0 && row_offset == 0)
                            {

                            }
                            else
                            {
                                // this was !
                                if (copyArrayStorage[y + col_offset][x + row_offset] == 1)
                                {
                                    neighbours++;
                                }
                            }
                        }
                    }

                    if (neighbours >= neighboursNeeded)
                    {   //empty is = false therefore weight is there
                        gridArray2D[y][x].tileWeight = 1;
                    }
                    else
                    {   //true
                        gridArray2D[y][x].tileWeight = 0;
                    }
                }
                
            }
        }
    }



    public static BasicTile[][] compartimentalisedCA(BoundsInt boundsRoom) 
    {

        int maxY = boundsRoom.zMax - boundsRoom.zMin;
        int maxX = boundsRoom.xMax - boundsRoom.xMin;

        BasicTile[][] _gridarray2D = new BasicTile[maxY][];

        for (int y = 0; y < maxY; y++)
        {
            _gridarray2D[y] = new BasicTile[maxX];

            for (int x = 0; x < maxX; x++)
            {
                _gridarray2D[y][x] = new BasicTile();
                _gridarray2D[y][x].position = new Vector2Int(x, y);
            }
        }


        SpawnRandomPointsCA(_gridarray2D, 0.55f);
        RunCaIteration2D(_gridarray2D, 4);
        RunCaIteration2D(_gridarray2D, 4);
        CleanUp2dCA(_gridarray2D, 4);

        return _gridarray2D;
    }




    #endregion

    //not working
    #region DiamondSquare algo

    public static bool DiamondSquare(int maxHeight, int minHeight, float roughness, BasicTile[][] gridArr)
    {

        int timerStart = GeneralUtil.PerfTimer(true);

        // get the size
        var mapSize = gridArr.Length;

        // start the grid
        float[,] grid2D = new float[mapSize, mapSize];

        //need to check for 2n + 1
        if (gridArr.Length != gridArr[0].Length || gridArr[0].Length % 2 == 0)
        {
            return false;
        }
        else
        {

            //set the 4 random corners
            grid2D[0, 0] = Random.Range(minHeight, maxHeight);   // top left
            grid2D[mapSize - 1, mapSize - 1] = Random.Range(minHeight, maxHeight);    // bot right
            grid2D[0, mapSize - 1] = Random.Range(minHeight, maxHeight); // top right
            grid2D[mapSize - 1, 0] = Random.Range(minHeight, maxHeight); // bot left

            var chunkSize = mapSize - 1;  //size of square in current iter of algo

            while (chunkSize > 1)
            {

                int halfChunk = chunkSize / 2;

                for (int y = 0; y < mapSize - 1; y = y + chunkSize)
                {
                    for (int x = 0; x < mapSize - 1; x = x + chunkSize)
                    {
                        grid2D[y + halfChunk, x + halfChunk] = (grid2D[y, x] + grid2D[y, x + chunkSize] + grid2D[y + chunkSize, x] + grid2D[y + chunkSize, x + chunkSize]) / 4 + Random.Range(-roughness, roughness);
                    }
                }

                for (int y = 0; y < mapSize; y = y + halfChunk)
                {
                    for (int x = (y + halfChunk) % chunkSize; x < mapSize; x = x + chunkSize)
                    {
                        grid2D[y, x] =
                            (grid2D[(y - halfChunk + mapSize) % mapSize, x] +
                                  grid2D[(y + halfChunk) % mapSize, x] +
                                  grid2D[y, (x + halfChunk) % mapSize] +
                                  grid2D[y, (x - halfChunk + mapSize) % mapSize]) / 4 + Random.Range(-roughness, roughness);
                    }
                }

                chunkSize = chunkSize / 2;
                roughness = roughness / 2;
            }
        }




        for (int y = 0; y < gridArr.Length; y++)
        {
            for (int x = 0; x < gridArr[0].Length; x++)
            {
                gridArr[y][x].tileWeight = grid2D[y,x];
            }
        }


        var end = GeneralUtil.PerfTimer(false, timerStart);
        return true;

    }


    #endregion

    #region Voronoi

    public static BasicTile[][] Voronoi2D(BasicTile[][] gridArray2D, int numOfPoints)
    {
        var pointsArr = new List<Vector2>();

        int totalSize = gridArray2D.Length * gridArray2D[0].Length;

        for (int i = 0; i < numOfPoints; i++)
        {
            int ran = Random.Range(0, totalSize);

            var wantedCoor = new Vector2(ran / gridArray2D[0].Length, ran % gridArray2D[0].Length);

            if (pointsArr.Contains(wantedCoor))
            {
                i--;
            }
            else
            {
                pointsArr.Add(wantedCoor);
            }
        }


        for (int y = 0; y < gridArray2D.Length; y++)
        {
            for (int x = 0; x < gridArray2D[y].Length; x++)
            {
                int closestIndex = 0;
                float closestDistance = -1;

                for (int i = 0; i < pointsArr.Count; i++)
                {
                    if (closestDistance < 0)
                    {
                        closestDistance = GeneralUtil.EuclideanDistance2D(pointsArr[i], new Vector2(gridArray2D[y][x].position.x, gridArray2D[y][x].position.y));
                    }
                    else
                    {
                        float newDist = GeneralUtil.EuclideanDistance2D(pointsArr[i], new Vector2(gridArray2D[y][x].position.x, gridArray2D[y][x].position.y));

                        if (closestDistance > newDist)
                        {
                            closestDistance = newDist;
                            closestIndex = i;
                        }
                    }
                }

                gridArray2D[y][x].idx = closestIndex;
            }
        }

        return GetBoundariesVoronoi(gridArray2D);
    }


    private static BasicTile[][] GetBoundariesVoronoi(BasicTile[][] gridArr2d) 
    {
        var childPosArry = GeneralUtil.childPosArry4Side;

        for (int y = 0; y < gridArr2d.Length; y++)
        {
            for (int x = 0; x < gridArr2d[0].Length; x++)
            {
                int wantedIdx = gridArr2d[y][x].idx;

                bool sameIdx = true;

                for (int i = 0; i < childPosArry.Length / 2; i++)
                {
                    int x_buff = childPosArry[i, 0];
                    int y_buff = childPosArry[i, 1];

                    int[] node_position = { x + x_buff, y + y_buff };


                    if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= gridArr2d[0].Length || node_position[1] >= gridArr2d.Length)
                    {
                        continue;
                    }
                    else
                    {
                        if (gridArr2d[node_position[1]][node_position[0]].idx == wantedIdx) 
                        {
                        
                        }
                        else 
                        {
                            sameIdx = false;
                            break;
                        }
                    }
                }


                if (sameIdx) 
                {
                    gridArr2d[y][x].tileWeight = 1;
                }
                else 
                {
                    gridArr2d[y][x].tileWeight = 0;
                }

            }
        }

        return gridArr2d;
    }

    #endregion

    #region Marching Cubes Generation


    public static MarchingCubeClass[,,] ExtrapolateMarchingCubes(BasicTile[][] gridArray2D, int roomHeight = 7) 
    {

        var marchingCubesArr = new MarchingCubeClass[gridArray2D[0].Length, gridArray2D.Length, roomHeight];



        for (int z = 0; z < marchingCubesArr.GetLength(2); z++)  // this is the heihgt of the room
        {
            for (int y = 0; y < marchingCubesArr.GetLength(1); y++)
            {
                for (int x = 0; x < marchingCubesArr.GetLength(0); x++)
                {
                    if (z==0 || z== marchingCubesArr.GetLength(2) - 1) //we draw everything as this is the ceiling and the floor
                    {
                        if (gridArray2D[y][x].tileType == BasicTile.TileType.WALL || gridArray2D[y][x].tileType == BasicTile.TileType.WALLCORRIDOR) 
                        {
                            marchingCubesArr[x, y, z] = new MarchingCubeClass(new Vector3Int(gridArray2D[y][x].position.x, z, gridArray2D[y][x].position.y), gridArray2D[y][x].tileWeight != 0 ? 1 : 0, 0.95f);
                        }
                        else 
                        {

                            marchingCubesArr[x, y, z] = new MarchingCubeClass(new Vector3Int(gridArray2D[y][x].position.x, z, gridArray2D[y][x].position.y), gridArray2D[y][x].tileWeight != 0 ? 1 : 0, 0.05f);
                        }

                    }
                    else // this is justt he wall
                    {

                        if (gridArray2D[y][x].tileType == BasicTile.TileType.WALL || gridArray2D[y][x].tileType == BasicTile.TileType.WALLCORRIDOR) // draw everything but the floor
                        {
                            marchingCubesArr[x, y, z] = new MarchingCubeClass(new Vector3Int(gridArray2D[y][x].position.x, z, gridArray2D[y][x].position.y), 1,1);
                        }
                        else // set the floor to 0 
                        {
                            marchingCubesArr[x, y, z] = new MarchingCubeClass(new Vector3Int(gridArray2D[y][x].position.x, z, gridArray2D[y][x].position.y), 0, 1);
                        }

                    }
                }
            }
        }

        return marchingCubesArr;
    }



    
    public static Mesh MarchingCubesAlgo(MarchingCubeClass[,,] positionVertex, bool inverse = false) 
    {

        Mesh mesh = new Mesh();

        mesh.Clear();

        List<int> triangles = new List<int>();
        List<Vector3> vertecies = new List<Vector3>();

        for (int z = 0; z < positionVertex.GetLength(2); z++)
        {
            for (int y = 0; y < positionVertex.GetLength(1); y++)
            {
                for (int x = 0; x < positionVertex.GetLength(0); x++)
                {

                    if (x + 1 >= positionVertex.GetLength(0) || y + 1 >= positionVertex.GetLength(1) || z + 1 >= positionVertex.GetLength(2))
                    {
                        continue;
                    }
                    
                    var midPosArr = new Vector3[12]
                    {
                            Vector3.Lerp(  positionVertex[x,y,z].position,              positionVertex[x + 1,y,z].position,      positionVertex[x,y,z].weight / (positionVertex[x,y,z].weight + positionVertex[x + 1,y,z].weight)),    //0   1
                            Vector3.Lerp(  positionVertex[x + 1,y,z].position,          positionVertex[x + 1,y + 1,z].position,  positionVertex[x + 1,y,z].weight / (positionVertex[x + 1,y,z].weight + positionVertex[x + 1,y + 1,z].weight)),   //1   2
                            Vector3.Lerp(  positionVertex[x + 1,y + 1,z].position,      positionVertex[x,y + 1,z].position,      positionVertex[x + 1,y + 1,z].weight / (positionVertex[x + 1,y + 1,z].weight + positionVertex[x,y + 1,z].weight)),       //2   3
                            Vector3.Lerp(  positionVertex[x,y + 1,z].position,          positionVertex[x,y,z].position,          positionVertex[x,y + 1,z].weight / (positionVertex[x,y + 1,z].weight + positionVertex[x,y,z].weight)),           //3   0
                           
                            Vector3.Lerp(  positionVertex[x,y,z + 1].position,          positionVertex[x+1,y,z+1].position,      positionVertex[x,y,z + 1].weight / (positionVertex[x,y,z + 1].weight + positionVertex[x+1,y,z+1].weight)),       //4   5
                            Vector3.Lerp(  positionVertex[x+1,y,z+1].position ,         positionVertex[x + 1,y+1,z+1].position,  positionVertex[x+1,y,z+1].weight / (positionVertex[x+1,y,z+1].weight + positionVertex[x + 1,y+1,z+1].weight)),   //5   6
                            Vector3.Lerp(  positionVertex[x + 1,y+1,z+1].position,        positionVertex[x,y+1,z+1].position,    positionVertex[x + 1,y+1,z+1].weight / (positionVertex[x + 1,y+1,z+1].weight + positionVertex[x,y+1,z+1].weight)),       //6   7
                            Vector3.Lerp(  positionVertex[x,y + 1,z +1].position,          positionVertex[x,y,z+1].position,     positionVertex[x,y + 1,z +1].weight / (positionVertex[x,y + 1,z +1].weight + positionVertex[x,y,z+1].weight)),          //7   4
                            
                            Vector3.Lerp(  positionVertex[x,y,z+1].position,            positionVertex[x,y,z].position,          positionVertex[x,y,z+1].weight / (positionVertex[x,y,z+1].weight + positionVertex[x,y,z].weight)),           //4   0
                            Vector3.Lerp(  positionVertex[x+1,y,z+1].position,          positionVertex[x + 1,y,z].position,      positionVertex[x+1,y,z+1].weight / (positionVertex[x+1,y,z+1].weight + positionVertex[x + 1,y,z].weight)),       //5   1
                            Vector3.Lerp(  positionVertex[x + 1,y+1,z+1].position,        positionVertex[x+1,y+1,z].position,    positionVertex[x + 1,y+1,z+1].weight / (positionVertex[x + 1,y+1,z+1].weight + positionVertex[x+1,y+1,z].weight)),       //6   2
                            Vector3.Lerp(  positionVertex[x,y+1,z+1].position,          positionVertex[x,y + 1,z].position,      positionVertex[x,y+1,z+1].weight / (positionVertex[x,y+1,z+1].weight + positionVertex[x,y + 1,z].weight))          //7   3
                    

                            //Vector3.Lerp(  positionVertex[x,y,z].position,              positionVertex[x + 1,y,z].position,    positionVertex[x,y,z].weight == 1 ? 0 : 0.5f),    //0   1
                            //Vector3.Lerp(  positionVertex[x + 1,y,z].position,          positionVertex[x + 1,y + 1,z].position,positionVertex[x + 1,y,z].weight == 1 ? 0 : 0.5f),   //1   2
                            //Vector3.Lerp(  positionVertex[x + 1,y + 1,z].position,      positionVertex[x,y + 1,z].position,    positionVertex[x + 1,y + 1,z].weight == 1 ? 0 : 0.5f),       //2   3
                            //Vector3.Lerp(  positionVertex[x,y + 1,z].position,          positionVertex[x,y,z].position,        positionVertex[x,y + 1,z].weight == 1 ? 0 : 0.5f),           //3   0
                            //Vector3.Lerp(  positionVertex[x,y,z + 1].position,          positionVertex[x+1,y,z+1].position,    positionVertex[x,y,z + 1].weight == 1 ? 0 : 0.5f),       //4   5
                            //Vector3.Lerp(  positionVertex[x+1,y,z+1].position ,         positionVertex[x + 1,y+1,z+1].position,positionVertex[x+1,y,z+1].weight == 1 ? 0 : 0.5f),   //5   6
                            //Vector3.Lerp(  positionVertex[x + 1,y+1,z+1].position,        positionVertex[x,y+1,z+1].position,  positionVertex[x + 1,y+1,z+1].weight == 1 ? 0 : 0.5f),       //6   7
                            //Vector3.Lerp(  positionVertex[x,y + 1,z +1].position,          positionVertex[x,y,z+1].position,   positionVertex[x,y + 1,z +1].weight == 1 ? 0 : 0.5f),          //7   4
                            //Vector3.Lerp(  positionVertex[x,y,z+1].position,            positionVertex[x,y,z].position,        positionVertex[x,y,z+1].weight == 1 ? 0 : 0.5f),           //4   0
                            //Vector3.Lerp(  positionVertex[x+1,y,z+1].position,          positionVertex[x + 1,y,z].position,    positionVertex[x+1,y,z+1].weight == 1 ? 0 : 0.5f),       //5   1
                            //Vector3.Lerp(  positionVertex[x + 1,y+1,z+1].position,        positionVertex[x+1,y+1,z].position,  positionVertex[x + 1,y+1,z+1].weight == 1 ? 0 : 0.5f),       //6   2
                            //Vector3.Lerp(  positionVertex[x,y+1,z+1].position,          positionVertex[x,y + 1,z].position,    positionVertex[x,y+1,z+1].weight == 1 ? 0 : 0.5f)          //7   3
                    
                    
                            //       Vector3.Lerp(  positionVertex[x,y,z].position,              positionVertex[x + 1,y,z].position,   0.5f),    //0   1
                            //Vector3.Lerp(  positionVertex[x + 1,y,z].position,          positionVertex[x + 1,y + 1,z].position,0.5f),   //1   2
                            //Vector3.Lerp(  positionVertex[x + 1,y + 1,z].position,      positionVertex[x,y + 1,z].position,0.5f),       //2   3
                            //Vector3.Lerp(  positionVertex[x,y + 1,z].position,          positionVertex[x,y,z].position,0.5f),           //3   0
                            //Vector3.Lerp(  positionVertex[x,y,z + 1].position,          positionVertex[x+1,y,z+1].position,0.5f),       //4   5
                            //Vector3.Lerp(  positionVertex[x+1,y,z+1].position ,         positionVertex[x + 1,y+1,z+1].position,0.5f),   //5   6
                            //Vector3.Lerp(  positionVertex[x + 1,y+1,z+1].position,        positionVertex[x,y+1,z+1].position,0.5f),       //6   7
                            //Vector3.Lerp(  positionVertex[x,y + 1,z +1].position,          positionVertex[x,y,z+1].position,0.5f),          //7   4
                            //Vector3.Lerp(  positionVertex[x,y,z+1].position,            positionVertex[x,y,z].position,0.5f),           //4   0
                            //Vector3.Lerp(  positionVertex[x+1,y,z+1].position,          positionVertex[x + 1,y,z].position,0.5f),       //5   1
                            //Vector3.Lerp(  positionVertex[x + 1,y+1,z+1].position,        positionVertex[x+1,y+1,z].position,0.5f),       //6   2
                            //Vector3.Lerp(  positionVertex[x,y+1,z+1].position,          positionVertex[x,y + 1,z].position,0.5f)          //7   3
                    
                    };





             //       var midPosArr = new Vector3[12]
             //{
             //               Vector3.Lerp(  positionVertex[x,y,z].position,              positionVertex[x + 1,y,z].position,   positionVertex[x,y,z].weight / (positionVertex[x,y,z].weight + positionVertex[x + 1,y,z].weight)),    //0   1
             //               Vector3.Lerp(  positionVertex[x + 1,y,z].position,          positionVertex[x + 1,y + 1,z].position,0.5f),   //1   2
             //               Vector3.Lerp(  positionVertex[x + 1,y + 1,z].position,      positionVertex[x,y + 1,z].position,0.5f),       //2   3
             //               Vector3.Lerp(  positionVertex[x,y + 1,z].position,          positionVertex[x,y,z].position,0.5f),           //3   0
             //               Vector3.Lerp(  positionVertex[x,y,z + 1].position,          positionVertex[x+1,y,z+1].position,0.5f),       //4   5
             //               Vector3.Lerp(  positionVertex[x+1,y,z+1].position ,         positionVertex[x + 1,y+1,z+1].position,0.5f),   //5   6
             //               Vector3.Lerp(  positionVertex[x + 1,y+1,z+1].position,        positionVertex[x,y+1,z+1].position,0.5f),       //6   7
             //               Vector3.Lerp(  positionVertex[x,y + 1,z +1].position,          positionVertex[x,y,z+1].position,0.5f),          //7   4
             //               Vector3.Lerp(  positionVertex[x,y,z+1].position,            positionVertex[x,y,z].position,0.5f),           //4   0
             //               Vector3.Lerp(  positionVertex[x+1,y,z+1].position,          positionVertex[x + 1,y,z].position,0.5f),       //5   1
             //               Vector3.Lerp(  positionVertex[x + 1,y+1,z+1].position,        positionVertex[x+1,y+1,z].position,0.5f),       //6   2
             //               Vector3.Lerp(  positionVertex[x,y+1,z+1].position,          positionVertex[x,y + 1,z].position,0.5f)          //7   3
             //};

                    //Vector3.Lerp(botLeft.position, botRight.position, botLeft.weigth / (botLeft.weigth + botRight.weigth));



                    int index = positionVertex[x, y, z].state * 1 +
                                    positionVertex[x + 1, y, z].state * 2 +
                                    positionVertex[x + 1, y + 1, z].state * 4 +
                                    positionVertex[x, y + 1, z].state * 8 +
                                    positionVertex[x, y, z + 1].state * 16 +
                                    positionVertex[x + 1, y, z + 1].state * 32 +
                                    positionVertex[x + 1, y + 1, z + 1].state * 64 +
                                    positionVertex[x, y + 1, z + 1].state * 128;


                    for (int i = 0; i < triTable.GetLength(1); i++)
                    {

                        if (triTable[index, i] == -1)
                            break;

                        triangles.Add(vertecies.Count());

                        vertecies.Add(midPosArr[triTable[index, i]]);

                    }
                }
            }
        }



        if (inverse)
        {
            triangles.Reverse();
        }

        Debug.Log(vertecies.Count());
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertecies.ToArray();
        mesh.triangles = triangles.ToArray();


        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();



        return mesh;





    }

    



    #endregion



    #region Type and Utility section

    public static void SetUpTileCorridorTypesUI(BasicTile[][] gridArr, int width) 
    {


        AlgosUtils.SetUpTileTypesCorridor(gridArr);

        for (int i = 0; i < width - 1; i++)
        {
            for (int y = 0; y < gridArr.Length; y++)
            {
                for (int x = 0; x < gridArr[0].Length; x++)
                {
                    if (gridArr[y][x].tileType == BasicTile.TileType.WALLCORRIDOR)
                    {
                        gridArr[y][x].tileType = BasicTile.TileType.FLOORCORRIDOR;
                    }
                    if (gridArr[y][x].tileType == BasicTile.TileType.FLOORCORRIDOR)
                    {
                    }
                }
            }

            AlgosUtils.SetUpTileTypesCorridor(gridArr);
        }

        AlgosUtils.SetUpTileTypesFloorWall(gridArr);
    }


    /// <summary>
    /// given a set of points finds the mid points of those points
    /// </summary>
    /// <param name="listOfPoints"></param>
    /// <returns></returns>
    public static Vector2 FindMiddlePoint(List<Vector2> listOfPoints)
    {
        var midPoint = new Vector2(0, 0);

        foreach (var point in listOfPoints)
        {
            midPoint.x += point.x;
            midPoint.y += point.y;
        }

        midPoint = new Vector2(midPoint.x / listOfPoints.Count, midPoint.y / listOfPoints.Count);

        return midPoint;

    }
    public static Vector2 FindMiddlePoint(List<BasicTile> listOfPoints)
    {
        var midPoint = new Vector2(0, 0);

        foreach (var point in listOfPoints)
        {
            midPoint.x += point.position.x;
            midPoint.y += point.position.y;
        }

        midPoint = new Vector2(midPoint.x / listOfPoints.Count, midPoint.y / listOfPoints.Count);

        return midPoint;

    }

    /// <summary>
    /// call this to set up the tile type, for wall and floor, algo that recognises walls. this recognise
    /// </summary>
    /// <param name="gridArray2D"></param>
    /// <param name="diagonalTiles">diag tile depends if it uses the 8 or 4 child arr</param>
    public static void SetUpTileTypesFloorWall(BasicTile[][] gridArray2D)
    {

        int[,] childPosArry = new int[0, 0];

        childPosArry = GeneralUtil.childPosArry4Side;


        for (int y = 0; y < gridArray2D.Length; y++)
        {
            for (int x = 0; x < gridArray2D[0].Length; x++)
            {
                if (gridArray2D[y][x].tileWeight != 0)
                {

                    bool wall = false;

                    for (int i = 0; i < childPosArry.Length / 2; i++)
                    {
                        int x_buff = childPosArry[i, 0];
                        int y_buff = childPosArry[i, 1];

                        int[] node_position = { x + x_buff, y + y_buff };


                        if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= gridArray2D[0].Length || node_position[1] >= gridArray2D.Length)
                        {
                            wall = true;
                            break;
                        }
                        else
                        {
                            if (gridArray2D[node_position[1]][node_position[0]].tileWeight == 0)
                            {
                                wall = true;
                                break;
                            }
                        }
                    }


                    if (wall)
                    {
                        gridArray2D[y][x].tileType = BasicTile.TileType.WALL;
                        gridArray2D[y][x].tileWeight = 1;
                    }
                    else
                    {
                        gridArray2D[y][x].tileType = BasicTile.TileType.FLOORROOM;
                        gridArray2D[y][x].tileWeight = 0.5f;
                    }
                }
            }
        }


        var copyArr = new BasicTile[gridArray2D.Length][];

        for (int y = 0; y < copyArr.Length; y++)
        {
            copyArr[y] = new BasicTile[gridArray2D[0].Length];

            for (int x = 0; x < copyArr[y].Length; x++)
            {

                copyArr[y][x] = new BasicTile();
                copyArr[y][x].tileType = gridArray2D[y][x].tileType;
                copyArr[y][x].tileWeight = gridArray2D[y][x].tileWeight;
            }
        }


        for (int y = 0; y < copyArr.Length; y++)
        {
            for (int x = 0; x < copyArr[0].Length; x++)
            {
                if (copyArr[y][x].tileWeight == 0)
                {

                    int neigh = 0;

                    for (int i = 0; i < childPosArry.Length / 2; i++)
                    {
                        int x_buff = childPosArry[i, 0];
                        int y_buff = childPosArry[i, 1];

                        int[] node_position = { x + x_buff, y + y_buff };

                        if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= copyArr[0].Length || node_position[1] >= copyArr.Length)
                        {
                            continue;
                        }
                        else if (copyArr[node_position[1]][node_position[0]].tileType != BasicTile.TileType.VOID)
                        {
                            neigh++;
                        }
                    }


                    if (neigh >= 2)
                    {
                        gridArray2D[y][x].tileWeight = 1;
                        gridArray2D[y][x].tileType = BasicTile.TileType.WALL;
                    }
                  
                }
            }
        }

    }

    public static void SetUpTileTypesCorridor(BasicTile[][] gridArray2D)
    {

        int[,] childPosArry = new int[0, 0];

        childPosArry = GeneralUtil.childPosArry4Side;


        var copyArr = new BasicTile[gridArray2D.Length][];

        for (int y = 0; y < copyArr.Length; y++)
        {
            copyArr[y] = new BasicTile[gridArray2D[0].Length];

            for (int x = 0; x < copyArr[y].Length; x++)
            {
                copyArr[y][x] = new BasicTile();
                copyArr[y][x].tileType = gridArray2D[y][x].tileType;
                copyArr[y][x].tileWeight = gridArray2D[y][x].tileWeight;
            }
        }


        for (int y = 0; y < copyArr.Length; y++)
        {
            for (int x = 0; x < copyArr[0].Length; x++)
            {
                if (copyArr[y][x].tileType == BasicTile.TileType.FLOORCORRIDOR)
                {

                    for (int i = 0; i < childPosArry.Length / 2; i++)
                    {
                        int x_buff = childPosArry[i, 0];
                        int y_buff = childPosArry[i, 1];

                        int[] node_position = { x + x_buff, y + y_buff };

                        if (node_position[0] < 0 || node_position[1] < 0 || node_position[0] >= copyArr[0].Length || node_position[1] >= copyArr.Length)
                        {
                            continue;
                        }
                        else if (copyArr[node_position[1]][node_position[0]].tileType == BasicTile.TileType.VOID)
                        {
                            gridArray2D[node_position[1]][node_position[0]].tileType = BasicTile.TileType.WALLCORRIDOR;
                            gridArray2D[node_position[1]][node_position[0]].tileWeight =1;
                        }
                    }
                }
            }
        }

    }


    /// <summary>
    /// recongnises all of the rooms, give true to set the colours
    /// </summary>
    /// <param name="gridArray2D"></param>
    /// <param name="colorDebug"></param>
    /// <returns>returns a list of a lists of basic tiles = to one room</returns>
    public static List<List<BasicTile>> GetAllRooms(BasicTile[][] gridArray2D, bool colorDebug = false)
    {
        var rooms = new List<List<BasicTile>>();

        ResetVisited(gridArray2D);  //sest everything back to false ready for flood

        List<Vector2Int> openCoords = new List<Vector2Int>();  // this has all the tile coords of the tiles that are considered roomable

        for (int y = 0; y < gridArray2D.Length; y++)
        {
            for (int x = 0; x < gridArray2D[0].Length; x++)
            {
                if (gridArray2D[y][x].tileWeight != 0)
                    openCoords.Add(new Vector2Int(x, y));
            }
        }

        int iter = 0;

        while (openCoords.Count > 2)   // until there is stuff in the open coords  then
        {

            if (iter >= 1000)
            {
                Debug.Log($"<color=red>Reached max number of rooms there might be an issue</color>");
                break;
            }

            iter++;

            var ranCoord = openCoords[Random.Range(0, openCoords.Count - 1)];   //get a random from the list of possible positionss

            var room = new List<Vector2Int>();

            room = Flood2DAnchor(ranCoord.x, ranCoord.y, room, gridArray2D);   // this returns 

            for (int i = openCoords.Count(); i-- > 0;) //for every open coord 
            {
                foreach (var coor in room)    //does the open cord contain then remove to satisfy infinte while loop
                {
                    if (openCoords[i] == coor)
                    {
                        openCoords.RemoveAt(i);
                        break;
                    }
                }
            }

            List<BasicTile> roomBasicTile = new List<BasicTile>();

            for (int y = 0; y < gridArray2D.Length; y++)
            {
                for (int x = 0; x < gridArray2D[0].Length; x++)
                {
                    foreach (var coord in room)
                    {
                        if (new Vector2Int(x, y) == coord)
                            roomBasicTile.Add(gridArray2D[y][x]);
                    }
                }
            }

            rooms.Add(roomBasicTile);

        }





        //debug
        if (colorDebug)
        {
            foreach (var room in rooms)
            {
                var col = new Color(Random.Range(0, 0.99f), Random.Range(0, 0.99f), Random.Range(0, 0.99f));

                foreach (var tile in room)
                {
                    tile.color = col;
                }
            }
        }




        return rooms;
    }

    public static BasicTile[][] RestartArr(BasicTile[][] gridArr) 
    {

        for (int y = 0; y < gridArr.Length; y++)
        {
            gridArr[y] = new BasicTile[gridArr[0].Length];

            for (int x = 0; x < gridArr[0].Length; x++)
            {
                gridArr[y][x] = new BasicTile();
                gridArr[y][x].position = new Vector2Int(x, y);
                gridArr[y][x].tileType = BasicTile.TileType.VOID;
                gridArr[y][x].color = Color.white;
            }
        }

        return gridArr;
    }
    #endregion

}

#region classes

/// <summary>
/// This is the basic tile call 
/// </summary>
public class BasicTile
{

    public Color32 color;
    public Vector2Int position = Vector2Int.zero;
    public float tileWeight;
    public float cost = 0;
    public int idx = 0;
    public bool visited = false;

    public enum TileType
    {
        VOID,
        FLOORROOM,
        WALL,
        WALLCORRIDOR,
        ROOF,
        FLOORCORRIDOR,
        AVOID
    }

    public TileType tileType = 0;
  
    public BasicTile() { }
    public BasicTile(BasicTile toCopy) 
    {
        this.color = toCopy.color;
        this.tileType = toCopy.tileType;
        this.position = toCopy.position;
        this.cost = toCopy.cost;
        this.idx = toCopy.idx;
        this.visited = toCopy.visited;
        this.tileWeight = toCopy.tileWeight;
    }

}




#region triangulation classes

public class Triangle
{
    public Vector3 a;
    public Vector3 b;
    public Vector3 c;

    public Edge[] edges = new Edge[3];

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        this.a = a;
        this.b = b;
        this.c = c;


        this.edges[0] = new Edge(a, b);
        this.edges[1] = new Edge(b, c);
        this.edges[2] = new Edge(c, a);
    }


    public bool HasVertex(Vector3 point)
    {
        if (a == point || b == point || c == point) { return true; }
        else { return false; }
    }

}

public class Edge
{
    public Vector3[] edge = new Vector3[2];
    public float length;
    public Edge(Vector3 a, Vector3 b)
    {
        edge[0] = a;
        edge[1] = b;

        length = Mathf.Abs(Vector3.Distance(a, b));
    }

}


#endregion

public class AStar_Node
{

    public BasicTile refToBasicTile;
    public AStar_Node parent;

    public float g = 0;
    public float f = 0;
    public float h = 0;

    public AStar_Node(BasicTile basicTile)
    {
        refToBasicTile = basicTile;
    }

}

public class MarchingCubeClass
{
    public Vector3Int position;
    public int state;
    public float weight;

    public MarchingCubeClass(Vector3Int position, int state, float weight)
    {
        this.position = position;
        this.state = state;
        this.weight = weight;
    }
}

public class DjNode 
{
    public float distance = 99999;
    public DjNode parentDJnode = null;
    public BasicTile gridRefTile = null;
    public Vector2Int coord = Vector2Int.zero;
}


#endregion