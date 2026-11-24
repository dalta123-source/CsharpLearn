using System;
using WaveDefenceGame;

// 게임 실행 상태 및 메인 루프 (최상위 문)
Console.CursorVisible = false; //static Cursorvisible .커서 안보이게한다
GameEngine.Initialize();

while (GameEngine.IsRunning)
{
    GameEngine.ProcessInput(); // 1. 입력 (Input)
    GameEngine.Update();       // 2. 업데이트 (Update)
    GameEngine.Render();       // 3. 렌더링 (Render)

    Thread.Sleep(50);          // 프레임 속도 조절 (약 20 FPS)
}

Console.CursorVisible = true;
Console.Clear();
Console.WriteLine("게임을 종료합니다. 수고하셨습니다!");
