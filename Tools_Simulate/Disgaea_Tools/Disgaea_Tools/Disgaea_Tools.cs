using System;
using System.Collections.Generic;

namespace Disgaea_Tools
{
    public class Disgaea_Tools
    {
        #region Parameters

        /// <summary>
        /// 難度
        /// </summary>
        private int enemyDifficulty;

        /// <summary>
        /// 敵人數量
        /// </summary>
        private string enemyAmount;

        /// <summary>
        /// 是否有水晶
        /// </summary>
        private bool hasCrystal;

        /// <summary>
        /// 水晶出現機率
        /// </summary>
        private int crystalAppearProbability;

        /// <summary>
        /// 水晶數量
        /// </summary>
        private int crystalAmount;

        /// <summary>
        /// 有色地板數量
        /// </summary>
        private int crystalGroundAmount;

        /// <summary>
        /// 地板塊數量
        /// </summary>
        private int groundAmount;

        /// <summary>
        /// 是否寬廣
        /// </summary>
        private string IsWidth="";

        #endregion
        
        /// <summary>
        /// 程式起始點
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            Disgaea_Tools disgaea = new Disgaea_Tools();

            disgaea.Build();
        }
        
        Random random = new Random();

        /// <summary>
        /// 是否有水晶
        /// </summary>
        public bool IsExist(int Probability)
        {
            return (Probability > 30) ? true : false;
        }

        /// <summary>
        /// 建置地圖(原本方法)
        /// </summary>
        protected void Build()
        {
            crystalAppearProbability = random.Next(0,100);
            hasCrystal = IsExist(crystalAppearProbability);

            List<List<mapGround>> mapList = new List<List<mapGround>>();
            List<mapGround> colList = new List<mapGround>();

            groundAmount = random.Next(50,300);

            string strDescription = "";
            int count = groundAmount;
            int length = 0;
            int Row_Count = 0;
            int Col_Count = 0;

            #region 水晶
            List<orographicCrystal> crystalsList = new List<orographicCrystal>();
            List<crystalColor> crystalColorsList = new List<crystalColor>();
            //加入消滅水晶
            crystalColorsList.Add(new crystalColor("ELIMINATE",ConsoleColor.White));

            //地板水晶設定
            if (hasCrystal)
            {
                int max = groundAmount/10;

                if (max > 5)
                    max = 5;

                crystalAmount = random.Next(0, max);
                
                int countLog = crystalGroundAmount = random.Next(0, groundAmount);

                if(crystalAmount-1 > 0)
                {
                    for (int colorCount = crystalAmount - 1; colorCount > 0; colorCount--)
                    {
                        //可能改成用ENUM
                        if(colorCount == 1)
                            crystalColorsList.Add(new crystalColor("RED",ConsoleColor.Red));
                        else if (colorCount == 2)
                            crystalColorsList.Add(new crystalColor("BLUE",ConsoleColor.Blue));
                        else if (colorCount == 3)
                            crystalColorsList.Add(new crystalColor("GREEN",ConsoleColor.Green));
                        else if (colorCount == 4)
                            crystalColorsList.Add(new crystalColor("YELLOW",ConsoleColor.Yellow));
                    }

                    for (int crystalCount = crystalAmount; crystalCount > 0 ; crystalCount--)
                    {
                        orographicCrystal orographicCrystal = new orographicCrystal();

                        //水晶對應顏色地板的數量
                        countLog = random.Next(0, countLog);

                        orographicCrystal.groundCount = countLog;

                        countLog = crystalGroundAmount - countLog;

                        if (countLog < 0) countLog = 0;

                        //水晶顏色
                        foreach (crystalColor cyc in crystalColorsList)
                        {
                            if (!cyc.hasUsed)
                            {
                                orographicCrystal.Color = cyc.Color;
                                orographicCrystal.consoleColor = cyc.ConsoleColor;
                                cyc.hasUsed = true;
                            }
                        }

                        crystalsList.Add(orographicCrystal);
                    }
                }
                
            }

            #endregion

            //地圖地板建置
            while (groundAmount > 0)
            {

                length = random.Next(5, 15);

                //每一行
                colList = new List<mapGround>();

                //每行最多15塊地板
                for (int cnt = 1; cnt <= 15 ; cnt++)
                {
                    //每塊地板
                    mapGround mapBlock = new mapGround();
                    
                   //30%機率為空格
                   bool rate = random.NextDouble() < 0.3;
                   if (rate  && (cnt != length))
                   {
                        //是否為地板
                        mapBlock.IsGround = false;
                   }
                   else
                   {
                        //是否為地板
                        mapBlock.IsGround = true;
                        groundAmount--;
                        length--;

                        bool hasColor = random.NextDouble() < 0.3;
                        if (hasColor)
                        {
                            mapBlock.colorType = random.Next(0, crystalColorsList.Count);

                            int typeCount = mapBlock.colorType;

                            if (mapBlock.colorType == crystalColorsList.Count)
                                typeCount--;
                            
                            mapBlock.groundColor = crystalColorsList[typeCount].ConsoleColor;
                        }
                    }

                    colList.Add(mapBlock);
                }

                mapList.Add(colList);
                Row_Count++;
            }

            int Type = 1;

            //設置地圖上的區塊編號
            for (int TRow = 0; TRow < Row_Count; TRow++)
            {
                for (int TCol = 0; TCol < 15; TCol++)
                {
                    mapGround ground_Now = mapList[TRow][TCol];
                   
                    if (TRow == 0)//第一行
                    {
                        ground_Now.IsEdge = true;

                        if (ground_Now.IsGround)
                        {
                            if (TCol != 0 && (!mapList[TRow][TCol - 1].IsGround))
                                Type++;

                            ground_Now.Type = Type;
                        }
                        else
                            ground_Now.Type = 0;
                    }
                    else //中間行數
                    {
                        if (ground_Now.IsGround)
                        {
                            //判斷上左是否有值
                            if (mapList[TRow - 1][TCol].IsGround) // 上一行
                                ground_Now.Type = mapList[TRow - 1][TCol].Type;
                            else if (TCol != 0 && mapList[TRow][TCol - 1].IsGround) //左一行
                                ground_Now.Type = mapList[TRow][TCol - 1].Type;
                            else
                            {
                                Type++;
                                ground_Now.Type = Type;
                            }

                            if(TCol > 0)
                            {
                                mapGround upperGround = mapList[TRow - 1][TCol];
                                mapGround leftGround = mapList[TRow][TCol - 1];
                                if (upperGround.IsGround && leftGround.IsGround && (upperGround.Type != leftGround.Type))
                                {
                                    if (upperGround.Type > leftGround.Type)
                                    {
                                        mapList = SameTpyeChange(mapList, upperGround.Type, leftGround.Type);
                                       //Type = leftGround.Type++;
                                    }
                                    else
                                    {
                                        mapList = SameTpyeChange(mapList, leftGround.Type, upperGround.Type);
                                        //Type = upperGround.Type++;
                                    }
                                }
                            }
                        }
                        else
                            ground_Now.Type = 0;
                        
                        
                        if (TCol == 0 ||(TRow + 1 == Row_Count))//最後一行
                            ground_Now.IsEdge = true;
                        
                    }
                }
            }
            
            //設置出入口
            List<groundCount> countList = new List<groundCount>();
            foreach (List<mapGround> col in mapList)
            {
                foreach (mapGround mapGround in col)
                {
                    if(mapGround.IsGround)
                    {
                        if (countList == null)
                    {
                        groundCount gCount = new groundCount();
                        gCount.Type = mapGround.Type;
                        gCount.Amount = 1;
                        countList.Add(gCount);
                    }
                        else 
                    {
                        bool IsFlag = false;
                        foreach (groundCount gc in countList)
                        {
                            if (gc.Type == mapGround.Type)
                            {
                                gc.Amount += 1;
                                IsFlag = true;
                            }
                        }

                        if (!IsFlag)
                        {
                            groundCount gCount = new groundCount();
                            gCount.Type = mapGround.Type;
                            gCount.Amount = 1;
                            countList.Add(gCount);
                        }
                    }
                    }
                }
            }

            int MaxAmountType = 0;
            int typeR = 0;
            foreach (groundCount gcC in countList)
            {
                if (gcC.Amount > MaxAmountType)
                {
                    MaxAmountType = gcC.Amount;
                    typeR = gcC.Type;
                }
            }

            bool hasIN = false;
            bool hasOUT = false;

            while (!hasIN || !hasOUT)
            {
                int randomR = random.Next(0, Row_Count);
                int randomC = random.Next(0, 15);

                if(mapList[randomR][randomC].IsGround)
                {
                    if (mapList[randomR][randomC].Type == typeR)
                    {
                        if (!hasIN)
                            mapList[randomR][randomC].IsIN = hasIN = true;
                        else if (!hasOUT)
                            mapList[randomR][randomC].IsOUT = hasOUT = true;
                    }
                }
            }

            strDescription += "是否有地形水晶 : " + hasCrystal + "\n";

            if (hasCrystal)
            {
                strDescription += "地形水晶顏色 : ";

                foreach (crystalColor cy in crystalColorsList)
                {
                    strDescription += cy.ConsoleColor +" ";
                }

                strDescription +=  "\n";
            }

            strDescription += "地板數目 : " + count + "\n";

            DrawOnConsole(mapList, strDescription);

        }

        /// <summary>
        /// 相同種類值替換  #確認是否有替換
        /// </summary>
        private List<List<mapGround>> SameTpyeChange(List<List<mapGround>> mapList ,int bigType , int smallType)
        {
            foreach (List<mapGround> colList in mapList)
            {
                foreach (mapGround ground in colList)
                {
                    if (bigType.Equals(ground.Type))
                        ground.Type = smallType;
                }
            }
            return mapList;
        }

        /// <summary>
        /// Console顯示
        /// </summary>
        private void DrawOnConsole(List<List<mapGround>> mapList , string description)
        {
            foreach (List<mapGround> list in mapList)
            {
                foreach (mapGround ground in list)
                {
                    System.Console.ForegroundColor  = ground.groundColor;

                    if (ground.IsIN)
                    {
                        System.Console.BackgroundColor = ConsoleColor.Blue;
                        System.Console.ForegroundColor = ConsoleColor.White;
                        System.Console.Write("入");
                        System.Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else if (ground.IsOUT)
                    {
                        System.Console.BackgroundColor = ConsoleColor.Red;
                        System.Console.ForegroundColor = ConsoleColor.White;
                        System.Console.Write("出");
                        System.Console.BackgroundColor = ConsoleColor.Black;
                    }
                    else if (ground.IsGround)// map += ground.Type;
                        System.Console.Write("口");
                    else
                        System.Console.Write("  ");
                }

                System.Console.Write("\n");
            }

            //System.Console.WriteLine(map);
            //System.Console.WriteLine("地板數目 : " + count);
            //System.Console.WriteLine("層數 : " + count);

            System.Console.WriteLine(description);

            Console.ReadKey();
        }

        /// <summary>
        /// 建置地圖(新方法 點隨機擴散)
        /// </summary>
        protected void Bulid2()
        {
        }
    }

    public class groundCount
    {
        /// <summary>
        /// 種類
        /// </summary>
        public int Type;

        /// <summary>
        /// 數量
        /// </summary>
        public int Amount;
    }

    /// <summary>
    /// 地圖地板(塊)
    /// </summary>
    public class mapGround
    {
        /// <summary>
        /// 是否是地板
        /// </summary>
        public bool IsGround;

        /// <summary>
        /// 地板顏色
        /// </summary>
        public ConsoleColor groundColor = ConsoleColor.White;

        /// <summary>
        /// 顏色種類
        /// </summary>
        public int colorType;

        /// <summary>
        /// 用來紀錄連結區的編號
        /// </summary>
        public int Type;

        /// <summary>
        /// 是否為邊緣板塊
        /// </summary>
        public bool IsEdge = false;

        /// <summary>
        /// 入口
        /// </summary>
        public bool IsIN = false;

        /// <summary>
        /// 出口
        /// </summary>
        public bool IsOUT = false;
    }

    /// <summary>
    /// 地形水晶
    /// </summary>
    public class orographicCrystal
    {
        
        /// <summary>
        /// 顏色
        /// </summary>
        public string Color;

        /// <summary>
        /// 水晶顏色
        /// </summary>
        public ConsoleColor consoleColor;

        /// <summary>
        /// 地板數量
        /// </summary>
        public int groundCount;

        /// <summary>
        /// 水晶能力
        /// </summary>
        public string crystalAbility;
    }

    /// <summary>
    /// 水晶顏色
    /// </summary>
    public class crystalColor
    {
        public crystalColor(string color,ConsoleColor consoleColor)
        {
            Color = color;
            ConsoleColor = consoleColor;
            hasUsed = false;
        }

        /// <summary>
        /// 水晶顏色
        /// </summary>
        public string Color;

        /// <summary>
        /// 水晶顏色
        /// </summary>
        public ConsoleColor ConsoleColor;

        /// <summary>
        /// 是否使用
        /// </summary>
        public bool hasUsed;
    }

    /// <summary>
    /// 等級魚
    /// </summary>
    public class levelFish
    {
        /// <summary>
        /// 1:魚苗 / 2:幼魚 / 3:成魚
        /// </summary>
        public int level;
        /// <summary>
        /// 機率
        /// </summary>
        public decimal Probability;

    }

    /// <summary>
    /// 幸運板
    /// </summary>
    public class luckyBoard
    {
        /// <summary>
        /// 1:金錢 / 2:赫爾 / 3:瑪那
        /// </summary>
        public int type;
        /// <summary>
        /// 機率
        /// </summary>
        public decimal Probability;

    }

    /// <summary>
    /// 等級球
    /// </summary>
    public class levelBall
    {
        /// <summary>
        /// 1:一般 / 2:高級 / 3:超超高級
        /// </summary>
        public int level;
        /// <summary>
        /// 機率
        /// </summary>
        public decimal Probability;

    }

    /// <summary>
    /// 瓶罐人
    /// </summary>
    public class bottleCan
    {
        /// <summary>
        /// 1:一般 / 2:高級 / 3:超超高級
        /// </summary>
        public int level;
        /// <summary>
        /// 機率
        /// </summary>
        public decimal Probability;
    }

    #region 暫不管
    /// <summary>
    /// 侵略者
    /// </summary>
    public class Invasion
    {
        /// <summary>
        /// 數量
        /// </summary>
        public int Amount;
        /// <summary>
        /// 機率
        /// </summary>
        public decimal Probability;
    }

    /// <summary>
    /// 送蛋人
    /// </summary>
    public class quakerBird
    {
        /// <summary>
        /// 機率
        /// </summary>
        public decimal Probability;
    }
    #endregion
}
