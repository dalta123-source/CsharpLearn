// 2026-11-18
// C# 프로그래밍 입문
// 학습목표 : 화면에 내가 원하는 글자를 출력하고 싶다.

// 컴퓨터 : 작성한 글자를 어떻게 읽어야 해?
// 입력과 출력
//내가 어떤 크기에 어떤 물건을 넣어야 할지 딱 맞는 상자를 알려줄게 -> type 형태

//변수(
// 정수 int
// 실수 float
// 문자열 string
// 불 bool

// [데이터 + 로직] => 어떻게 해야 진짜 게임이 끝난처럼 표현이 될까요?
// 변수
// 로직 : 어떻게 사용을 해야 진짜 게임이 끝난처럼 표현이 될까요?

// 슬라임hp
// 용사hp
// 슬라임atk
// 용사atk

// 조건문을 이용하여 아주 간단한 승리 조건을 표현

using System.Numerics;
using System.Security.Cryptography;

namespace basic
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("연슴용 사가에 오신것을 환영합니다 새로온 신입");
            Console.WriteLine("당신의 이름은 무엇입니까?");

            int hp = 100;
            hp = hp - 100;
            
                float version = 1.01f;
            bool gameClaer = false;

            Console.WriteLine(hp);
            Console.WriteLine(version);
            Console.WriteLine(gameClaer);

            
            string name = Console.ReadLine();
            Console.WriteLine(name);

            //
            int slimehp = 100;
            int p_HP = 100;
            int s_ATK = 1;
            int p_ATK = 20;

            Console.WriteLine("[슬라임 턴]");
            p_HP = p_HP - s_ATK;
            Console.WriteLine($"용사의 체력은 : {p_HP}");

            if(gameClaer)
            {
                Console.WriteLine("출력이 되었나요?");
            }
            
        }
    }
}
