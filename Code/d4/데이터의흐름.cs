using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// 가위 바위 보를 조건문으로 만들때 조건을 내가 만든다.

// 나 : 가위, 


namespace basic
{
    internal class 데이터의흐름
    {
        static void Main()
        {
            //if ,else if , if
            bool GameFinish = true;
            int Score = 80;

            if(GameFinish)
            {

                if (60 <= Score && 80 > Score)
                {
                    Console.WriteLine("B");
                        }
                else if (80 <= Score && 100 > Score)
                { 
                    Console.WriteLine("A"); 
                }
                string name = "파이리";
                // switch 스위치
                switch(name)
                {
                    case  "파이리":
                            Console.WriteLine("파이리");
                        break;
                }
            };
        }
    }
}
