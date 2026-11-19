// 반복문
// for 반복문, whlie 반복문
// 왜 쓰는가?

for (int i = 1; i<= 100; i++)
{
    Console.WriteLine(i);
}

//while 반복문
// 무한 반복시 사용
bool gamefinish = false;
int hp = 0;
while (true)
{
    if (hp <= 0) gamefinish = true;
    if(gamefinish)
    {
        break;
    }
}

// 반복문을 사용하는 게임 추천

while (true) 
{
    
}