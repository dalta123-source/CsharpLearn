// 배열

// 왜 쓰는가?
//반복문, 갯수가 정해졌을 때 쓰면 'for'

// 어떻게 배열을 사용하면 n개를 하나의 코드로 표현할 수 있을까요?
// 배열의 구현 원리를 알면 설명할 수 있다. -> 자료구조.

int[] numbers = new int[10];
for (int i = 0; i < 10; i++)
{
    numbers[i] = i;
    Console.WriteLine($"현재 값은 : {numbers[i]}");
}

Console.WriteLine(numbers[0]);

// 배열은 같은 자료형을 한번에 표현할 수 있다.
// 갯수를 정하면 시작부터 끝까지 데이터를 저장하고 언제든지 꺼내서 쓸 수 있다

// 가격은 가격인데... (xx)가격은 너무 많은데? price 마음껏 써도 되나?

const int ITEM_MAX_COUNT = 10;

int[] prices = new int[ITEM_MAX_COUNT];
for (int i = 0; i < ITEM_MAX_COUNT; i++)
{
    prices[i] = 10 + 10 * i;
}

// 4번째 등급의 아이템 가격은 얼마인
Console.WriteLine("");