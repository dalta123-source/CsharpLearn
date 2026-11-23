using System;

class Program
{
    static void Main()
    
    {
        
        Random random = new Random();
        int answer = random.Next(1, 3); // 1(홀) 또는 2(짝) 랜덤 생성

        Console.Write("홀(1) 또는 짝(2)을 입력하세요: ");
        int userChoice = int.Parse(Console.ReadLine());

        if (userChoice == answer)
        {
            Console.WriteLine("정답입니다!");
        }
        else
        {
            Console.WriteLine("틀렸습니다.");
        }
    }
}
