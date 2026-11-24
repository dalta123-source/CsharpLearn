Enemy e1 = new Enemy("고블린"); //e1 클래스 객체 생성
Enemy e2 = new Enemy("고블린"); //e1 클래스 객체 생성
Enemy e3 = new Enemy("고블린"); //e1 클래스 객체 생성


// record
e1 = e2;
e1.Name = "A";
Console.WriteLine(e1.Name);


// 정적 클래스
// static 사용한 클래스

// 개별로 자신의 속성과 기능을 가질 수 있다.
// Enemy 몇마리 소환이 되었나요? => Enemy 
// statiic을 쓰면은 클래스 이름으로 바로 쓸 수 있다.-> Enemy

// 클래스이름


Console.WriteLine($"몬스터 수: {Enemy.Count}");

class Enemy
{
    public static int Count;    // 객체마다 따로 생긴다
    public string Name;        // 객체마다 하나


    public Enemy(string name)
    {
        Name = name;
        Count++;               // 모두 같은 Count
    }
    public static void PrintCount()
    {
        Console.WriteLine($"적 {Count}마리");
    }
}

