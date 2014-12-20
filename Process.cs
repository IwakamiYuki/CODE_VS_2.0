using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
namespace test
{
    class Process
    {
        bool debugFlag;

        int mapWidth;   // フィールドの幅
        int mapHeight;  // フィールドの高さ
        int packSize;    // パックのサイズ
        int sumNum;     // 投下するブロックの正方形領域の一辺の長さ
        int stepMax;    // 
        int[][] map;
        bool[][] deleteMap;
        bool[][] moveMap;
        int[][][] pack;
        int stepCnt;
        int processDepth;
        int X;
        int R;
        int score;

        // 思考深さ
        const int processDepthConst = 3;


        MyMath Math;

        public void setInput(int _X,int _R){
            if (stepCnt >= stepMax) return;
            X = _X;
            R = _R;

            int[][] _pack;
            _pack = new int[packSize][];
            for (int i = 0; i < packSize; i++)
            {
                _pack[i] = new int[packSize];
            }

            



            // 回転を考慮
            for (int i = 0; i < R; i++)
            {
                for (int j = 0; j < packSize; j++)
                {
                    for (int k = 0; k < packSize; k++)
                    {
                        _pack[j][packSize - k - 1] = pack[stepCnt][k][j];
                    }
                }
                for (int j = 0; j < packSize; j++)
                {
                    for (int k = 0; k < packSize; k++)
                    {
                        pack[stepCnt][j][k] = _pack[j][k];
                    }
                }
            }

            // グローバルマップにpackを追加する
            for (int i = 0; i < packSize; i++)
            {
                for (int j = 0; j < packSize; j++)
                {
                    if (pack[stepCnt][j][i] > 0)
                    {
                        if ((X + i < 0 || X + i >= mapWidth))
                        {
                            score = -100;
                            return;
                        }
                        else
                        {
                            map[X + i][mapHeight + packSize - j] = pack[stepCnt][j][i];
                        }
                    }
                }
            }


            // ■2. 終了条件を満たしていれば終了、そうでなければ 3 へ
            int chain;
            bool roop2_flag = true;
            for (chain = 1; roop2_flag; chain++)
            {
                roop2_flag = false;


                for (int i = 0; i < moveMap.Length; i++)
                {
                    for (int j = 0; j < moveMap[i].Length; j++)
                    {
                        moveMap[i][j] = false;
                    }
                }

                    // ■3. 落下できるブロックがあれば全て落下
                    for (int i = 0; i < mapWidth; i++)
                    {
                        for (int j = 0; j < mapHeight + packSize + 1; j++)
                        {
                            if (map[i][j] == 0)
                            {
                                for (int k = j; k < mapHeight + packSize + 1; k++)
                                {
                                    if (map[i][k] > 0)
                                    {
                                        map[i][j] = map[i][k];
                                        map[i][k] = 0;
                                        moveMap[i][j] = true;

                                        int l=1;
                                        int packSum = map[i][j];
                                        while (true) {
                                            if (i - l < 0) break;
                                            if (map[i - l][j] == 0) break;
                                            packSum += map[i - l][j];
                                            if (packSum > sumNum) break;
                                            moveMap[i - l][j] = true;
                                        } 
                                        l = 1;
                                        packSum = map[i][j];
                                        while (true)
                                        {
                                            if (j - l < 0) break;
                                            if (map[i][j - l] == 0) break;
                                            packSum += map[i][j - l];
                                            if (packSum > sumNum) break;
                                            moveMap[i][j - l] = true;
                                        }
                                        l = 1;
                                        packSum = map[i][j];
                                        while (true)
                                        {
                                            if (j - l < 0 || i - l < 0) break;
                                            if (map[i - l][j - l] == 0) break;
                                            packSum += map[i - l][j - l];
                                            if (packSum > sumNum) break;
                                            moveMap[i - l][j - l] = true;
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                    }
                for (int i = 0; i < mapWidth; i++)
                {
                    if (map[i][mapHeight] > 0)
                    {
                        score = -1;
                        return;
                    }
                }
               
                // ■4. 消去できるブロックがあれば全て消去
                int E = 0;  // 消滅カウントE
                int C = chain;  // チェインの数C
                int n = stepCnt / 100;  // 現在のターン数を100で整数除算したn
                int P = (mapWidth <= 15) ? 25 : (mapWidth <= 20) ? 30 : 35;   // ケースごとに与えられる値P

                for (int i = 0; i < mapWidth; i++)
                {
                    for (int j = 0; j < mapHeight + packSize + 1; j++)
                    {
                        deleteMap[i][j] = false;
                    }
                }
                // ray tracing
                for (int i = 0; i < mapWidth; i++)
                {
                    for (int j = 0; j < mapHeight; j++)
                    {
                        int k = 0;
                        int traceNum = 0;
                        if (map[i][j] == sumNum)
                        {
                            deleteMap[i][j] = true;
                            E++;
                        }
                        else
                        {
                            if (moveMap[i][j])
                            {
                                // 縦方向に探索(height方向)
                                while (true)
                                {
                                    if (j + k >= mapHeight) break;
                                    traceNum += map[i][j + k];
                                    if (map[i][j + k] == 0) break;
                                    if (traceNum > sumNum) break;
                                    if (traceNum == sumNum)
                                    {
                                        for (int l = 0; l <= k; l++)
                                        {
                                            deleteMap[i][j + l] = true;
                                            E++;
                                        }
                                    }
                                    k++;
                                }
                                k = 0; traceNum = 0;
                                // 横方向に探索(width方向)
                                while (true)
                                {
                                    if (i + k >= mapWidth) break;
                                    traceNum += map[i + k][j];
                                    if (map[i + k][j] == 0) break;
                                    if (traceNum > sumNum) break;
                                    if (traceNum == sumNum)
                                    {
                                        for (int l = 0; l <= k; l++)
                                        {
                                            deleteMap[i + l][j] = true;
                                            E++;
                                        }
                                    }
                                    k++;
                                }
                                k = 0; traceNum = 0;
                                // 斜め方向に探索(右上がり)
                                while (true)
                                {
                                    if (i + k >= mapWidth || j + k >= mapHeight) break;
                                    traceNum += map[i + k][j + k];
                                    if (map[i + k][j + k] == 0) break;
                                    if (traceNum > sumNum) break;
                                    if (traceNum == sumNum)
                                    {
                                        for (int l = 0; l <= k; l++)
                                        {
                                            deleteMap[i + l][j + l] = true;
                                            E++;
                                        }
                                    }
                                    k++;
                                }
                                k = 0; traceNum = 0;
                                // 斜め方向に探索(右下がり)
                                while (true)
                                {
                                    if (i + k >= mapWidth || j - k < 0) break;
                                    traceNum += map[i + k][j - k];
                                    if (map[i + k][j - k] == 0) break;
                                    if (traceNum > sumNum) break;
                                    if (traceNum == sumNum)
                                    {
                                        for (int l = 0; l <= k; l++)
                                        {
                                            deleteMap[i + l][j - l] = true;
                                            E++;
                                        }
                                    }
                                    k++;
                                }
                            }
                        }
                    }
                }
                // 削除する
                for (int i = 0; i < mapWidth; i++)
                {
                    for (int j = 0; j < mapHeight; j++)
                    {
                        if (deleteMap[i][j])
                        {
                            map[i][j] = 0;
                            deleteMap[i][j] = false;
                            roop2_flag = true;
                        }
                    }
                }
               // Console.WriteLine("E:" + E + " C:" + C + " n:" + n + " P:" + P);
                if (roop2_flag)
                {
                    //if (C < Min(10,stepMax-stepCnt-1) && processDepth == 1)
                    //{
                    //    score -= E;
                    //    //if (score < -10) roop2_flag=false;
                    //}
                    //else
                    //{
                        //score += Math.Pow(2, Min(C, P + n) - 1) * Max(1, C - (P + n) + 1) * E;
                        score += Math.Pow(2, C - 1) * E;
                    //}
                     
                }
               // Console.WriteLine("Score : " + score);

            }
            int maxScore = 0;
            if (processDepth < processDepthConst)
            {
                for (int i = 0 - (packSize - 1); i < mapWidth - packSize + 1 + (packSize - 1); i++)
                {
                    for (int j = 0; j < 4; j++)
                    {

                        Process p = new Process(debugFlag, mapWidth, mapHeight, packSize, sumNum, stepCnt + 1, stepMax, map, pack, processDepth + 1,Math);
                        p.setInput(i, j);
                        int _score = p.getScore();
                        if (maxScore < _score)
                        {
                            X = i;
                            R = j;
                            maxScore = _score;
                            if (debugFlag)scoreText = getScoreText();
                        }

                    }
                }
                if(debugFlag)scoreText = "" + maxScore + "+" + scoreText;
                score += maxScore;
            }

            if (debugFlag) scoreText = (score - maxScore) + "+" + scoreText;
        }
        public int getScore(){
            return score;
        }
        string scoreText = "-1";
        public string getScoreText()
        {
            return scoreText;
        }

        public Process(bool _debugFlag, int _mapWidth, int _mapHeight, int _packSize, int _sumNum, int _stepCnt, int _stepMax, int[][] _map, int[][][] _pack, int _processDepth, MyMath _Math)
        {
            debugFlag = _debugFlag;
            mapHeight = _mapWidth;
            mapWidth = _mapWidth;
            packSize = _packSize;
            sumNum = _sumNum;
            stepCnt = _stepCnt;
            stepMax = _stepMax;
            processDepth = _processDepth;

            map = new int[_map.Length][];
            deleteMap = new bool[_map.Length][];
            moveMap = new bool[map.Length][];
            for (int i = 0; i < _map.Length; i++)
            {
                map[i] = new int[_map[i].Length];
                deleteMap[i] = new bool[_map[i].Length];
                moveMap[i] = new bool[map[i].Length];
                for (int j = 0; j < _map[i].Length; j++)
                {
                    map[i][j] = _map[i][j];
                    deleteMap[i][j] = false;
                    moveMap[i][j] = false;

                   // Console.Write(map[i][j]);
                }
                //Console.WriteLine();
            }

            pack = new int[_pack.Length][][];

            for (int i = stepCnt; i < Min(stepCnt + processDepthConst+1-processDepth,stepMax); i++)
            //for (int i = 0; i < _pack.Length; i++)
            {
                pack[i] = new int[_pack[i].Length][];
                for (int j = 0; j < _pack[i].Length; j++)
                {
                    pack[i][j] = new int[_pack[i][j].Length];
                    for (int k = 0; k < _pack[i][j].Length; k++)
                    {
                        pack[i][j][k] = _pack[i][j][k];
                    }
                }
            }
            Math = _Math;


        }

 
        private static int Min(int a, int b)
        {
            return (a < b) ? a : b;
        }
        private static int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }
    }
}
