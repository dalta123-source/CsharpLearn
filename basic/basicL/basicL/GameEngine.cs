using System;
using System.Collections.Generic;
using System.Linq;

namespace WaveDefenceGame
{
    /// <summary>
    /// 게임의 3요소(입력, 업데이트, 렌더링)를 처리하는 핵심 엔진 클래스
    /// </summary>
    public static class GameEngine
    {
        public static bool IsRunning { get; private set; } = true;
        private static List<Monster> _monsters = new List<Monster>();
        private static int _tickCount = 0;
        private static string _lastActionLog = "스페이스바(Space)를 눌러 가장 가까운 몬스터를 공격하세요!";
        private static Random _rand = new Random();

        public static void Initialize()
        {
            WaveManager.Reset();
            SpawnWave();
        }

        private static void SpawnWave()
        {
            WaveManager.StartNextWave();
            _monsters.Clear();

            for (int i = 0; i < WaveManager.TotalSpawnedInWave; i++)
            {
                // 오른쪽 영역(X: 35~55, Y: 4~12)에 무작위 배치
                int x = _rand.Next(35, 55);
                int y = _rand.Next(4, 12);
                int hp = 10 + (WaveManager.CurrentWave * 5); // 웨이브마다 체력 증가
                _monsters.Add(new Monster(i + 1, x, y, hp));
            }

            _lastActionLog = $"[알림] Wave {WaveManager.CurrentWave} 시작! 몬스터 {WaveManager.AliveMonsters}마리 출현!";
        }

        // 1. 입력 (Input): 키보드 입력 감지
        public static void ProcessInput()
        {
            if (!Console.KeyAvailable) return;

            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);

            if (keyInfo.Key == ConsoleKey.Escape)
            {
                IsRunning = false;
            }
            else if (keyInfo.Key == ConsoleKey.Spacebar)
            {
                AttackNearestMonster();
            }
        }

        // 몬스터를 죽이는 방식: 가장 가까운(X좌표가 작은) 살아있는 몬스터 타격
        private static void AttackNearestMonster()
        {
            var target = _monsters
                .Where(m => m.IsAlive)
                .OrderBy(m => m.X)
                .FirstOrDefault();

            if (target != null)
            {
                int damage = 15;
                target.TakeDamage(damage);

                if (target.IsAlive)
                    _lastActionLog = $"몬스터 #{target.Id} 공격! (남은 HP: {target.Hp})";
                else
                    _lastActionLog = $"몬스터 #{target.Id} 처치 완료! (남은 몬스터: {WaveManager.AliveMonsters}마리)";
            }
        }

        // 2. 업데이트 (Update): 몬스터 이동 및 웨이브 클리어 판정
        public static void Update()
        {
            _tickCount++;

            // 10틱(약 0.5초)마다 몬스터가 전진
            if (_tickCount % 10 == 0)
            {
                foreach (var monster in _monsters.Where(m => m.IsAlive))
                {
                    monster.MoveLeft();
                }
            }

            // 모든 몬스터가 사망하면 다음 Wave 발생
            if (WaveManager.IsWaveCleared())
            {
                SpawnWave();
            }
        }

        // 3. 렌더링 (Render): 콘솔 화면 출력
        public static void Render()
        {
            Console.SetCursorPosition(0, 0);

            Console.WriteLine("=====================================================================");
            Console.WriteLine($" [WAVE {WaveManager.CurrentWave}]   생존 몬스터: {WaveManager.AliveMonsters} / {WaveManager.TotalSpawnedInWave} 마리   |   [Space]: 공격  [ESC]: 종료");
            Console.WriteLine("=====================================================================");
            Console.WriteLine($" 로그: {_lastActionLog.PadRight(55)}");
            Console.WriteLine("---------------------------------------------------------------------");

            // 게임 필드 그리기 (세로 4~12)
            for (int y = 4; y <= 12; y++)
            {
                char[] line = new char[60];
                Array.Fill(line, ' ');

                // 플레이어 위치 표시
                if (y == 8) line[1] = 'P';

                // 살아있는 몬스터 위치 표시
                foreach (var m in _monsters.Where(m => m.IsAlive && m.Y == y))
                {
                    if (m.X >= 0 && m.X < line.Length)
                    {
                        line[m.X] = 'M';
                    }
                }

                Console.WriteLine(new string(line));
            }
            Console.WriteLine("=====================================================================");
        }
    }
}
