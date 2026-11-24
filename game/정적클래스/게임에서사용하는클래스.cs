//AI를 활용한 배운 문법을 작성하는 페이지 입니다.

//클래스, 생성자.
//클래스 무엇을 할래?

//RPG 턴제, 전투, 포켓몬스터
//'플레이어,(똑같은 녀석)'적'(도감에 있는 모든 몬스터)
//Unit - 전투를 한다. 플레이어도하고 상대방도 똑같은 방식으로 한다.

//AI '읽을 수 있는 범위' 만큼만 만들어달라.
// 클래스 한개만 만들어라.
// 내가 어떻게 만들건지 문법을 설명할거다

// AI. 정해지지 않은 내용은 자기가 알아서 만든다. 다음 똑같이 프롬프트를 입력해도 결과가 다르게 나



//클래스를 직접 설계 한 다음

public class Unit
{
    // 속성 (Properties)
    public int Health { get; private set; }      // 체력
    public int AttackPower { get; private set; } // 공격력
    public int Defense { get; private set; }     // 방어력
    public int Speed { get; private set; }       // 속도

    // 생성자 (Constructor)
    public Unit(int health, int attackPower, int defense, int speed)
    {
        Health = health;
        AttackPower = attackPower;
        Defense = defense;
        Speed = speed;
    }

    // 기능: 체력, 공격력, 방어력, 속도를 사용한 전투 메서드
    public void Battle(Unit target)
    {
        // 1. 속도 비교로 선공(먼저 공격할 유닛)과 후공 결정
        Unit first = this.Speed >= target.Speed ? this : target;
        Unit second = this.Speed >= target.Speed ? target : this;

        // 2. 선공 유닛의 공격 (최소 1의 데미지 보장)
        int damageToSecond = Math.Max(1, first.AttackPower - second.Defense);
        second.Health = Math.Max(0, second.Health - damageToSecond);

        Console.WriteLine($"선공자의 공격! 데미지: {damageToSecond}, 후공자 남은 체력: {second.Health}");

        // 3. 후공 유닛이 생존해 있다면 반격
        if (second.Health > 0)
        {
            int damageToFirst = Math.Max(1, second.AttackPower - first.Defense);
            first.Health = Math.Max(0, first.Health - damageToFirst);
            Console.WriteLine($"후공자의 반격! 데미지: {damageToFirst}, 선공자 남은 체력: {first.Health}");
        }
    }
}