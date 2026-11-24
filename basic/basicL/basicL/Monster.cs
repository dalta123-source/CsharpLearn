using System;

namespace WaveDefenceGame
{
    /// <summary>
    /// 개별 몬스터의 위치와 체력을 담당하는 클래스
    /// </summary>
    public class Monster
    {
        public int Id { get; }
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Hp { get; private set; }
        public bool IsAlive => Hp > 0;

        public Monster(int id, int startX, int startY, int hp)
        {
            Id = id;
            X = startX;
            Y = startY;
            Hp = hp;
        }

        /// <summary>
        /// 몬스터가 피해를 입고 사망 시 정적 매니저에 알립니다.
        /// </summary>
        public void TakeDamage(int damage)
        {
            if (!IsAlive) return;

            Hp -= damage;
            if (Hp <= 0)
            {
                Hp = 0;
                WaveManager.OnMonsterKilled();
            }
        }

        /// <summary>
        /// 플레이어 방향(왼쪽)으로 한 칸씩 이동합니다.
        /// </summary>
        public void MoveLeft()
        {
            if (IsAlive && X > 2)
            {
                X--;
            }
        }
    }
}
