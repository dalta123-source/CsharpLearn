// AI 임의로 구성한 내용을 파악하고 이 부분을 AI로 만들어지는 결과를 일정한 수준으로 유도 작업

// '상속'
//내가 만들고 싶은 것을 클래스로
// abstract 추상 : 기능을 구현하지 않을거지만 너가 이 내용을 무조건 구현해야해

using System.Text;

abstract class BGE
{
    //입력기능
    //갱신기능
    //렌더링기능

    abstract public void Input();
    abstract public void Update();
    abstract public void Rendering();
}

class MGE : BGE
{
    protected int Width { get; private set; }
    protected int Height { get; private set; }

    private string[,] screenBuffer;
    private StringBuilder renderBuilder = new StringBuilder();
    private string statusText = "";

    protected ConsoleKeyInfo? CurrentKey { get; private set; }

    public MGE(int width = 20, int height = 20)
    {
        Width = width;
        Height = height;
        screenBuffer = new string[Height, Width];
        Console.CursorVisible = false;
        ClearBuffer();
    }

    // 버퍼 초기화 (가로 2칸 공백으로 채워 2칸 밀림 방지)
    protected void ClearBuffer()
    {
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                screenBuffer[y, x] = "  ";
            }
        }
    }

    // 하위 게임 클래스가 화면에 2칸짜리 타일을 그릴 때 사용하는 엔진 API
    protected void SetPixel(int x, int y, string twoWidthChar)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            screenBuffer[y, x] = twoWidthChar;
        }
    }

    // 상단 UI 텍스트 설정 API
    protected void SetStatusText(string text)
    {
        statusText = text;
    }

    public override void Input()
    {
        if (Console.KeyAvailable)
        {
            CurrentKey = Console.ReadKey(true);
        }
        else
        {
            CurrentKey = null;
        }
    }

    public override void Update()
    {
        // 하위 클래스에서 오버라이드하여 게임 로직 구현
    }

    public override void Rendering()
    {
        // 깜빡임 해결: 커서를 (0, 0)으로 이동해 덮어쓰기
        Console.SetCursorPosition(0, 0);
        renderBuilder.Clear();

        if (!string.IsNullOrEmpty(statusText))
        {
            renderBuilder.AppendLine(statusText);
        }

        // 2칸 밀림 해결: 버퍼에 담긴 2칸 단위 문자열을 한 번에 조립
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                renderBuilder.Append(screenBuffer[y, x]);
            }
            renderBuilder.AppendLine();
        }

        Console.Write(renderBuilder.ToString());
    }
}

