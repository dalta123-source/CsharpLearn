//상수

//변하지 않는 수
//const int ROCK = 2;
//int choise = 1;

// 왜 상수를 썼을까? 상수를 써서 비교를 할때 명확하게 쓸 수 있다.
//AI야 너는 내가 지시하지 않은 문법에 대해서는 작성하지 말고, 문제가 있다면 원인을 말해봐
// 아이템 ID가 어떤 역할을 할까?

//아이템 상호작용이 다른 아이템이 있을까?

const int equip = 1;
const int consumable = 2;
const int quest = 3;
const int material = 4;
const int curreny = 5;

int ItemTagID = 1;

if (ItemTagID == equip)
{

}

// 열거형

//무엇이 상수와 다른가?
// 상수의 표현 문제점 => 물,불,땅


GameData.monsterType type = GameData.monsterType.Fire;

GameData.Title g_tile = GameData.Title.Fire;