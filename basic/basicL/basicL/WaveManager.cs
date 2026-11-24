using System;

namespace WaveDefenceGame
{
    /// <summary>
    /// 몬스터의 생존 수와 웨이브 진행을 관리하는 정적 클래스
    /// </summary>
    public static class WaveManager
    {
        public static int CurrentWave { get; private set; } = 0;
        public static int AliveMonsters { get; private set; } = 0;
        public static int TotalSpawnedInWave { get; private set; } = 0;

        /// <summary>
        /// 새로운 웨이브를 시작하고 몬스터 수를 초기화합니다.
        /// </summary>
        public static void StartNextWave()
        {
            CurrentWave++;
            // 웨이브가 올라갈수록 몬스터 수(n) 증가 (예: 1웨이브 3마리, 2웨이브 5마리...)
            int monsterCount = 1 + (CurrentWave * 2);
            
            TotalSpawnedInWave = monsterCount;
            AliveMonsters = monsterCount;
        }

        /// <summary>
        /// 몬스터가 처치되었을 때 호출되어 생존 수를 감소시킵니다.
        /// </summary>
        public static void OnMonsterKilled()
        {
            if (AliveMonsters > 0)
            {
                AliveMonsters--;
            }
        }

        /// <summary>
        /// 현재 웨이브의 모든 몬스터가 사망했는지 확인합니다.
        /// </summary>
        public static bool IsWaveCleared()
        {
            return AliveMonsters <= 0;
        }

        /// <summary>
        /// 상태 초기화
        /// </summary>
        public static void Reset()
        {
            CurrentWave = 0;
            AliveMonsters = 0;
            TotalSpawnedInWave = 0;
        }
    }
}
