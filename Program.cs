using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace test
{
    
    class Program
    {
        const bool debugFlag = false;  // デバッグ

        static int[][] map;    // フィールドの値
        static bool[][] deleteMap;  // 削除するかどうか

        static int[][][] pack;  //　パック

        static int mapWidth;   // フィールドの幅
        static int mapHeight;  // フィールドの高さ
        static int packSize;    // パックのサイズ
        static int sumNum;     // 投下するブロックの正方形領域の一辺の長さ
        static int stepMax;    // 与えられるパックの数

        static int maxChain = 0;   // 最大連鎖
        static int score = 0;  // スコア


        static void Main(string[] args)
        {
            MyMath Math = new MyMath();


                if (debugFlag)
                {
                    using (StreamReader r = new StreamReader(@"sample_input.txt"))
                    {
                        string textLine = r.ReadLine();
                        string[] s = textLine.Split(' ');
                        mapWidth = int.Parse(s[0]);
                        mapHeight = int.Parse(s[1]);
                        packSize = int.Parse(s[2]);
                        sumNum = int.Parse(s[3]);
                        stepMax = int.Parse(s[4]);

                        map = new int[mapWidth][];
                        deleteMap = new bool[mapWidth][];



                        for (int i = 0; i < mapWidth; i++)
                        {
                            map[i] = new int[mapHeight + packSize + 1];
                            deleteMap[i] = new bool[mapHeight + packSize + 1];
                        }

                        pack = new int[stepMax][][];
                        for (int i = 0; i < stepMax; i++)
                        {
                            pack[i] = new int[packSize][];
                            for (int j = 0; j < packSize; j++)
                            {
                                pack[i][j] = new int[packSize];
                            }
                        }

                        WriteLine(mapWidth + " " + mapHeight + " " + sumNum + " " + stepMax);   // 確認出力

                        // パック入力
                        for (int i = 0; i < stepMax; i++)
                        {
                            for (int j = 0; j < packSize; j++)
                            {
                                textLine = r.ReadLine();
                                s = textLine.Split(' ');
                                for (int k = 0; k < packSize; k++)
                                {
                                    pack[i][j][k] = int.Parse(s[k]);
                                }
                            }
                            r.ReadLine();
                        }
                    }
                }
                else
                {
                    // 入力
                    string textLine = System.Console.ReadLine();
                    string[] s = textLine.Split(' ');
                    mapWidth = int.Parse(s[0]);
                    mapHeight = int.Parse(s[1]);
                    packSize = int.Parse(s[2]);
                    sumNum = int.Parse(s[3]);
                    stepMax = int.Parse(s[4]);

                    map = new int[mapWidth][];
                    deleteMap = new bool[mapWidth][];

                    for (int i = 0; i < mapWidth; i++)
                    {
                        map[i] = new int[mapHeight + packSize + 1];
                        deleteMap[i] = new bool[mapHeight + packSize + 1];
                    }

                    pack = new int[stepMax][][];
                    for (int i = 0; i < stepMax; i++)
                    {
                        pack[i] = new int[packSize][];
                        for (int j = 0; j < packSize; j++)
                        {
                            pack[i][j] = new int[packSize];
                        }
                    }

                    WriteLine(mapWidth + " " + mapHeight + " " + sumNum + " " + stepMax);   // 確認出力

                    // パック入力
                    for (int i = 0; i < stepMax; i++)
                    {
                        for (int j = 0; j < packSize; j++)
                        {
                            textLine = System.Console.ReadLine();
                            s = textLine.Split(' ');
                            for (int k = 0; k < packSize; k++)
                            {
                                pack[i][j][k] = int.Parse(s[k]);
                            }
                        }
                        System.Console.ReadLine();
                    }
                }

            for (int stepCnt = 0; stepCnt < stepMax; stepCnt++)
            {
                // ■1. パックの回転、投下位置の指定
                int X = 0;	// パックの落とす位置
                int R = 0;	// パックを回転する回数
                int maxScore = -1;
                for (int i = 0 - (packSize - 1); i < mapWidth - packSize + 1 + (packSize - 1); i++)
                {
                    for (int j = 0; j < 4; j++)
                    {

                        Process p = new Process(debugFlag, mapWidth, mapHeight, packSize, sumNum, stepCnt, stepMax, map, pack, 1, Math);
                        p.setInput(i, j);
                        int _score = p.getScore();
                        WriteLine(i+","+j+" : "+_score+"  "+p.getScoreText());
                        if (maxScore < _score)
                        {
                            X = i;
                            R = j;
                            maxScore = _score;
                        }

                    }
                }
                WriteLine("("+X+","+R+") "+maxScore);



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

                    printMap();
                    WriteLine((chain == 0) ? "Pack入力" : "削除");
                    pause();

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
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    printMap();
                    WriteLine("落下");
                    pause();

                    // ■4. 消去できるブロックがあれば全て消去
                    int E = 0;  // 消滅カウントE
                    int C = chain;  // チェインの数C
                    int n = stepCnt / 100;  // 現在のターン数を100で整数除算したn
                    int P = (mapWidth<=15)?25:(mapWidth<=20)?30:35;   // ケースごとに与えられる値P

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
                    int s=0;
                    if (roop2_flag)
                    {
                        s = Math.Pow(2, Min(C, P + n) - 1) * Max(1, C - (P + n) + 1) * E;
                        score += s;
                    }
                    WriteLine("S:"+s+"  E:" + E + " C:" + C + " n:" + n + " P:" + P);
                    WriteLine("Score : " + score);
                }


                if (maxChain < chain - 2) maxChain = chain - 2;

                printMap();
                WriteLine("Turn : " + (stepCnt+1));
                WriteLine("Chain : " + (chain - 2));
                WriteLine("Score : " + score);
                WriteLine("maxChain : " + maxChain);

                pause();

                pause();

                Console.WriteLine(X + " " + R);

            }



                WriteLine("end");
                pause();



        }

        private static void WriteLine(string s){
            if (debugFlag)
                Console.WriteLine(s);
        }

        private static int Min(int a, int b) {
            return (a < b) ? a : b;
        }
        private static int Max(int a, int b)
        {
            return (a > b) ? a : b;
        }



        private static void pause(){
            if (debugFlag)
            {
                string s = "q";
                if (s.CompareTo(Console.ReadLine()) == 0)
                    Environment.Exit(0x8020);
            }

        }

        private static void printMap()
        {
            if (debugFlag)
            {
                Console.WriteLine();
                for (int i = mapHeight + packSize; i >= 0; i--)
                {
                    Console.Write(i % 10 + " ");
                    for (int j = 0; j < mapWidth; j++)
                    {
                        if (map[j][i] == 0)
                        {
                            if (i >= mapHeight)
                            {
                                Console.Write("-");
                            }
                            else
                            {
                                Console.Write("+");
                            }
                        }
                        else if (map[j][i] < 10)
                        {
                            Console.Write(map[j][i]);
                        }
                        else
                        {
                            switch (map[j][i])
                            {
                                case 10: Console.Write("A"); break;
                                default: Console.Write("*"); break;

                            }
                        }
                    }
                    Console.WriteLine();
                }
            }
        }
    }
}
