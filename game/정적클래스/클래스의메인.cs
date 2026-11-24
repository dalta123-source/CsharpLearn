/*
 * 클래스의 생산자까지 사용 문법 실습한다.
 * 
 * -게임 폴더
 *      -program.cs
 *      -Unit.cs
 *      
 * AI코드를 하나
 */



class Program
{
    static void Main()
    {
        // 플레이어와 몬스터 생성
        Unit player = new Unit(100, 20, 5, 12);
        Unit monster = new Unit(80, 15, 3, 10);
        bool isRunning = true;

        // 초기 상태 렌더링
        Console.WriteLine("=== 전투 시작! ===");
        Console.WriteLine($"[플레이어] 체력: {player.Health} | [몬스터] 체력: {monster.Health}\n");

        // 게임 루프 (입력 -> 업데이트 -> 렌더링)
        while (isRunning)
        {
            // 1. 입력 (Input)
            Console.WriteLine("행동을 선택하세요: [1] 공격하기  [2] 도망치기");
            string input = Console.ReadLine();
            Console.Clear(); // 화면 깔끔하게 정리

            // 2. 업데이트 (Update)
            if (input == "1")
            {
                player.Battle(monster);
            }
            else if (input == "2")
            {
                Console.WriteLine("전투에서 도망쳤습니다!");
                isRunning = false;
                continue; // 루프 종료로 바로 이동
            }
            else
            {
                Console.WriteLine("잘못된 입력입니다. 1 또는 2를 입력해주세요.");
                continue;
            }

            // 승패 판정 업데이트
            if (player.Health <= 0 || monster.Health <= 0)
            {
                isRunning = false;
            }

            // 3. 렌더링 (Render)
            Console.WriteLine("-----------------------------------------");
            Console.WriteLine($"[현재 상태] 플레이어 체력: {player.Health} | 몬스터 체력: {monster.Health}");
            Console.WriteLine("-----------------------------------------\n");
        }

        // 최종 결과 출력
        if (player.Health <= 0)
        {
            Console.WriteLine("패배했습니다...");
        }
        else if (monster.Health <= 0)
        {
            Console.WriteLine("승리했습니다! 몬스터를 처치했습니다.");
        }
    }
}
