using System;
using System.Collections.Generic;
using System.Linq;

namespace StarCraftOOP
{
    public enum MovementType
    {
        Ground,
        Air
    }

    public enum UnitAttribute
    {
        Biological,
        Mechanical
    }

    // 5가지 핵심 인터페이스
    public interface ICloakable
    {
        bool IsCloaked { get; }
        void Cloak();
        void Decloak();
    }

    public interface IDetector
    {
        int DetectRange { get; }
        void Detect(IEnumerable<Unit> units);
    }

    public interface ITransport
    {
        int Capacity { get; }
        List<Unit> LoadedUnits { get; }
        void Load(Unit unit);
        void UnloadAll();
    }

    public interface IWorker
    {
        void GatherResource(string resourceName);
        void BuildStructure(string buildingName);
    }

    public interface IBurrowable
    {
        bool IsBurrowed { get; }
        void Burrow();
        void Unburrow();
    }

    // 최상위 추상 클래스: 유닛
    public abstract class Unit
    {
        public string Name { get; protected set; }
        public int Hp { get; protected set; }
        public int MaxHp { get; protected set; }
        public MovementType MoveType { get; protected set; }
        public UnitAttribute Attribute { get; protected set; }
        public int X { get; protected set; }
        public int Y { get; protected set; }

        protected Unit(string name, int maxHp, MovementType moveType, UnitAttribute attribute)
        {
            Name = name;
            MaxHp = maxHp;
            Hp = maxHp;
            MoveType = moveType;
            Attribute = attribute;
        }

        public virtual void Move(int x, int y)
        {
            X = x;
            Y = y;
            Console.WriteLine($"[{Name}] ({MoveType}/{Attribute}) 위치 ({X}, {Y})로 이동했습니다.");
        }

        public virtual void TakeDamage(int damage)
        {
            Hp = Math.Max(0, Hp - damage);
            Console.WriteLine($"[{Name}] {damage} 피해! 남은 체력: {Hp}/{MaxHp}");
        }

        public virtual void Update()
        {
            // 기본 업데이트 (하위 종족 클래스에서 오버라이드)
        }

        public virtual string GetStatus()
        {
            return $"{Name} [{MoveType}/{Attribute}] HP: {Hp}/{MaxHp} 위치: ({X},{Y})";
        }
    }

    // 종족 1: 테란
    public abstract class TerranUnit : Unit
    {
        protected TerranUnit(string name, int maxHp, MovementType moveType, UnitAttribute attribute)
            : base(name, maxHp, moveType, attribute) { }
    }

    // 종족 2: 프로토스 (실드 및 실드 재생)
    public abstract class ProtossUnit : Unit
    {
        public int Shield { get; protected set; }
        public int MaxShield { get; protected set; }

        protected ProtossUnit(string name, int maxHp, int maxShield, MovementType moveType, UnitAttribute attribute)
            : base(name, maxHp, moveType, attribute)
        {
            MaxShield = maxShield;
            Shield = maxShield;
        }

        public override void TakeDamage(int damage)
        {
            if (Shield > 0)
            {
                int absorbed = Math.Min(Shield, damage);
                Shield -= absorbed;
                damage -= absorbed;
                Console.WriteLine($"[{Name}] 실드로 {absorbed} 피해 흡수! 남은 실드: {Shield}/{MaxShield}");
            }

            if (damage > 0)
            {
                base.TakeDamage(damage);
            }
        }

        public override void Update()
        {
            base.Update();
            if (Shield < MaxShield)
            {
                Shield = Math.Min(MaxShield, Shield + 2);
            }
        }

        public override string GetStatus()
        {
            return $"{Name} [{MoveType}/{Attribute}] HP: {Hp}/{MaxHp}, Shield: {Shield}/{MaxShield} 위치: ({X},{Y})";
        }
    }

    // 종족 3: 저그 (체력 재생)
    public abstract class ZergUnit : Unit
    {
        protected ZergUnit(string name, int maxHp, MovementType moveType, UnitAttribute attribute)
            : base(name, maxHp, moveType, attribute) { }

        public override void Update()
        {
            base.Update();
            if (Hp > 0 && Hp < MaxHp)
            {
                Hp = Math.Min(MaxHp, Hp + 1);
            }
        }
    }

    // 구체 유닛 구현
    public class SCV : TerranUnit, IWorker
    {
        public SCV() : base("SCV", 60, MovementType.Ground, UnitAttribute.Mechanical) { }

        public void GatherResource(string resourceName)
        {
            Console.WriteLine($"[{Name}] {resourceName}을(를) 채취합니다.");
        }

        public void BuildStructure(string buildingName)
        {
            Console.WriteLine($"[{Name}] {buildingName} 건설을 시작합니다.");
        }
    }

    public class Wraith : TerranUnit, ICloakable
    {
        public bool IsCloaked { get; private set; }

        public Wraith() : base("레이스", 120, MovementType.Air, UnitAttribute.Mechanical) { }

        public void Cloak()
        {
            IsCloaked = true;
            Console.WriteLine($"[{Name}] 클로킹 활성화! 은폐 상태가 됩니다.");
        }

        public void Decloak()
        {
            IsCloaked = false;
            Console.WriteLine($"[{Name}] 클로킹 해제.");
        }

        public override string GetStatus()
        {
            return base.GetStatus() + (IsCloaked ? " [은폐중]" : "");
        }
    }

    public class Dropship : TerranUnit, ITransport
    {
        public int Capacity { get; } = 8;
        public List<Unit> LoadedUnits { get; } = new List<Unit>();

        public Dropship() : base("드랍쉽", 150, MovementType.Air, UnitAttribute.Mechanical) { }

        public void Load(Unit unit)
        {
            if (unit.MoveType != MovementType.Ground)
            {
                Console.WriteLine($"[{Name}] 지상 유닛만 탑승할 수 있습니다.");
                return;
            }
            if (LoadedUnits.Count >= Capacity)
            {
                Console.WriteLine($"[{Name}] 수송 공간이 가득 찼습니다.");
                return;
            }
            LoadedUnits.Add(unit);
            Console.WriteLine($"[{Name}] {unit.Name} 탑승 완료. ({LoadedUnits.Count}/{Capacity})");
        }

        public void UnloadAll()
        {
            Console.WriteLine($"[{Name}] 탑승 유닛 {LoadedUnits.Count}기 전원 하차!");
            LoadedUnits.Clear();
        }
    }

    public class Observer : ProtossUnit, ICloakable, IDetector
    {
        public bool IsCloaked { get; private set; } = true;
        public int DetectRange { get; } = 11;

        public Observer() : base("옵저버", 40, 20, MovementType.Air, UnitAttribute.Mechanical) { }

        public void Cloak()
        {
            IsCloaked = true;
            Console.WriteLine($"[{Name}] 영구 은폐 상태입니다.");
        }

        public void Decloak()
        {
            Console.WriteLine($"[{Name}] 옵저버는 은폐를 수동으로 해제할 수 없습니다.");
        }

        public void Detect(IEnumerable<Unit> units)
        {
            Console.WriteLine($"[{Name}] 디텍터 스캔 실행 (탐지 범위: {DetectRange})...");
            foreach (var u in units.Where(u => u != this))
            {
                if (u is ICloakable cloakable && cloakable.IsCloaked)
                {
                    Console.WriteLine($"  -> 은폐 유닛 포착: {u.Name}");
                }
                if (u is IBurrowable burrowable && burrowable.IsBurrowed)
                {
                    Console.WriteLine($"  -> 잠복 유닛 포착: {u.Name}");
                }
            }
        }

        public override string GetStatus()
        {
            return base.GetStatus() + " [영구은폐/디텍터]";
        }
    }

    public class Zergling : ZergUnit, IBurrowable
    {
        public bool IsBurrowed { get; private set; }

        public Zergling() : base("저글링", 35, MovementType.Ground, UnitAttribute.Biological) { }

        public void Burrow()
        {
            IsBurrowed = true;
            Console.WriteLine($"[{Name}] 땅속으로 잠복(Burrow)했습니다.");
        }

        public void Unburrow()
        {
            IsBurrowed = false;
            Console.WriteLine($"[{Name}] 잠복을 해제하고 지상으로 올라왔습니다.");
        }

        public override void Move(int x, int y)
        {
            if (IsBurrowed)
            {
                Console.WriteLine($"[{Name}] 잠복 상태에서는 이동할 수 없습니다!");
                return;
            }
            base.Move(x, y);
        }

        public override string GetStatus()
        {
            return base.GetStatus() + (IsBurrowed ? " [잠복중]" : "");
        }
    }

    // 프로그램 진입점 (Main) 및 Input -> Update -> Render 루프
    public class Program
    {
        private static List<Unit> units = new List<Unit>();
        private static bool isRunning = true;
        private static int turn = 1;

        public static void Main(string[] args)
        {
            Initialize();

            while (isRunning)
            {
                Render();
                Input();
                if (!isRunning) break;
                Update();
                turn++;
            }

            Console.WriteLine("프로그램을 종료합니다.");
        }

        private static void Initialize()
        {
            units.Add(new SCV());
            units.Add(new Wraith());
            units.Add(new Dropship());
            units.Add(new Observer());
            units.Add(new Zergling());

            // 재생 확인을 위한 초기 피해 부여
            units[3].TakeDamage(15); // 옵저버 실드 감소
            units[4].TakeDamage(10); // 저글링 체력 감소
        }

        private static void Input()
        {
            Console.WriteLine("\n[명령어 선택]");
            Console.WriteLine("1. 모든 유닛 이동 (다형성 Move)");
            Console.WriteLine("2. 일꾼 작업 실행 (IWorker)");
            Console.WriteLine("3. 은폐 및 잠복 실행 (ICloakable / IBurrowable)");
            Console.WriteLine("4. 옵저버 탐지 실행 (IDetector)");
            Console.WriteLine("5. 드랍쉽 수송 테스트 (ITransport)");
            Console.WriteLine("6. 턴만 넘기기 (실드/체력 재생 확인)");
            Console.WriteLine("0. 종료");
            Console.Write("입력 > ");

            string input = Console.ReadLine();
            Console.WriteLine();

            switch (input)
            {
                case "1":
                    foreach (var unit in units)
                    {
                        unit.Move(turn * 10, turn * 5);
                    }
                    break;

                case "2":
                    foreach (var worker in units.OfType<IWorker>())
                    {
                        worker.GatherResource("미네랄");
                        worker.BuildStructure("배럭");
                    }
                    break;

                case "3":
                    foreach (var cloakable in units.OfType<ICloakable>())
                    {
                        cloakable.Cloak();
                    }
                    foreach (var burrowable in units.OfType<IBurrowable>())
                    {
                        if (burrowable.IsBurrowed) burrowable.Unburrow();
                        else burrowable.Burrow();
                    }
                    break;

                case "4":
                    foreach (var detector in units.OfType<IDetector>())
                    {
                        detector.Detect(units);
                    }
                    break;

                case "5":
                    var dropship = units.OfType<ITransport>().FirstOrDefault();
                    if (dropship != null)
                    {
                        if (dropship.LoadedUnits.Count == 0)
                        {
                            foreach (var u in units.Where(u => u != dropship))
                            {
                                dropship.Load(u);
                            }
                        }
                        else
                        {
                            dropship.UnloadAll();
                        }
                    }
                    break;

                case "6":
                    Console.WriteLine("시간이 경과합니다...");
                    break;

                case "0":
                    isRunning = false;
                    break;

                default:
                    Console.WriteLine("잘못된 입력입니다.");
                    break;
            }
        }

        private static void Update()
        {
            foreach (var unit in units)
            {
                unit.Update();
            }
        }

        private static void Render()
        {
            Console.WriteLine($"\n================ [ Turn {turn} 상태 ] ================");
            foreach (var unit in units)
            {
                Console.WriteLine(unit.GetStatus());
            }
            Console.WriteLine("================================================");
        }
    }
}
