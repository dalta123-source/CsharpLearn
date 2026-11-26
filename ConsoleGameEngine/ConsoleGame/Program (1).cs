using System;
using System.Text;
using System.Threading;

// ==========================================
// 최상위 문 (Top-level statements) 실행 영역
// ==========================================
Console.OutputEncoding = Encoding.UTF8;

// 테트리스 보드(가로 10, 세로 20) + 좌우 벽(2칸) + 바닥(1칸) = 가로 12, 세로 21
TetrisGame game = new TetrisGame(12, 21);

while (!game.IsGameOver)
{
    game.Input();
    game.Update();
    game.Rendering();
    Thread.Sleep(50); // 20 FPS 기준 루프 딜레이
}

Console.SetCursorPosition(0, 23);
Console.WriteLine("게임 오버! 아무 키나 누르면 종료합니다...");
Console.ReadKey(true);


// ==========================================
// 1. 기존 제공 엔진 클래스 (BGE & MGE)
// ==========================================
abstract class BGE
{
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

    protected void SetPixel(int x, int y, string twoWidthChar)
    {
        if (x >= 0 && x < Width && y >= 0 && y < Height)
        {
            screenBuffer[y, x] = twoWidthChar;
        }
    }

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
    }

    public override void Rendering()
    {
        Console.SetCursorPosition(0, 0);
        renderBuilder.Clear();

        if (!string.IsNullOrEmpty(statusText))
        {
            renderBuilder.AppendLine(statusText);
        }

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


// ==========================================
// 2. MGE를 상속받아 구현한 테트리스 게임 클래스
// ==========================================
class TetrisGame : MGE
{
    public bool IsGameOver { get; private set; } = false;

    private const int BoardWidth = 10;
    private const int BoardHeight = 20;
    private int[,] board = new int[BoardHeight, BoardWidth];

    // 7가지 테트로미노 블록 정의 (I, O, T, S, Z, J, L)
    private readonly int[][,] shapes = new int[][,]
    {
        new int[,] { {0,0,0,0}, {1,1,1,1}, {0,0,0,0}, {0,0,0,0} }, // I
        new int[,] { {1,1}, {1,1} },                               // O
        new int[,] { {0,1,0}, {1,1,1}, {0,0,0} },                   // T
        new int[,] { {0,1,1}, {1,1,0}, {0,0,0} },                   // S
        new int[,] { {1,1,0}, {0,1,1}, {0,0,0} },                   // Z
        new int[,] { {1,0,0}, {1,1,1}, {0,0,0} },                   // J
        new int[,] { {0,0,1}, {1,1,1}, {0,0,0} }                    // L
    };

    private int[,] currentPiece;
    private int pieceX, pieceY;
    private int score = 0;

    private int fallTimer = 0;
    private int fallInterval = 10; // 50ms * 10 = 0.5초마다 자동 낙하
    private Random rand = new Random();

    public TetrisGame(int width = 12, int height = 21) : base(width, height)
    {
        SpawnPiece();
    }

    private void SpawnPiece()
    {
        int index = rand.Next(shapes.Length);
        currentPiece = (int[,])shapes[index].Clone();
        pieceX = BoardWidth / 2 - currentPiece.GetLength(1) / 2;
        pieceY = 0;

        // 생성 직후 충돌하면 게임 오버
        if (!IsValidPosition(currentPiece, pieceX, pieceY))
        {
            IsGameOver = true;
        }
    }

    public override void Update()
    {
        if (IsGameOver) return;

        // 1. 키 입력 처리 (엔진의 CurrentKey 활용)
        if (CurrentKey.HasValue)
        {
            switch (CurrentKey.Value.Key)
            {
                case ConsoleKey.LeftArrow:
                case ConsoleKey.A:
                    if (IsValidPosition(currentPiece, pieceX - 1, pieceY)) pieceX--;
                    break;
                case ConsoleKey.RightArrow:
                case ConsoleKey.D:
                    if (IsValidPosition(currentPiece, pieceX + 1, pieceY)) pieceX++;
                    break;
                case ConsoleKey.DownArrow:
                case ConsoleKey.S:
                    if (IsValidPosition(currentPiece, pieceX, pieceY + 1)) pieceY++;
                    break;
                case ConsoleKey.UpArrow:
                case ConsoleKey.W:
                    int[,] rotated = RotateMatrix(currentPiece);
                    if (IsValidPosition(rotated, pieceX, pieceY)) currentPiece = rotated;
                    break;
                case ConsoleKey.Spacebar:
                    // 하드 드롭 (바닥까지 즉시 낙하)
                    while (IsValidPosition(currentPiece, pieceX, pieceY + 1))
                    {
                        pieceY++;
                    }
                    LockPiece();
                    break;
            }
        }

        // 2. 자동 낙하 타이머
        fallTimer++;
        if (fallTimer >= fallInterval)
        {
            fallTimer = 0;
            if (IsValidPosition(currentPiece, pieceX, pieceY + 1))
            {
                pieceY++;
            }
            else
            {
                LockPiece();
            }
        }

        // 3. 엔진 화면 버퍼 갱신
        DrawToBuffer();
    }

    private void LockPiece()
    {
        int size = currentPiece.GetLength(0);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (currentPiece[y, x] != 0)
                {
                    int boardY = pieceY + y;
                    int boardX = pieceX + x;
                    if (boardY >= 0 && boardY < BoardHeight && boardX >= 0 && boardX < BoardWidth)
                    {
                        board[boardY, boardX] = 1;
                    }
                }
            }
        }

        ClearLines();
        SpawnPiece();
    }

    private void ClearLines()
    {
        int linesCleared = 0;

        for (int y = BoardHeight - 1; y >= 0; y--)
        {
            bool isFull = true;
            for (int x = 0; x < BoardWidth; x++)
            {
                if (board[y, x] == 0)
                {
                    isFull = false;
                    break;
                }
            }

            if (isFull)
            {
                linesCleared++;
                // 윗줄들을 한 칸씩 아래로 내림
                for (int moveY = y; moveY > 0; moveY--)
                {
                    for (int x = 0; x < BoardWidth; x++)
                    {
                        board[moveY, x] = board[moveY - 1, x];
                    }
                }
                // 맨 윗줄 비우기
                for (int x = 0; x < BoardWidth; x++)
                {
                    board[0, x] = 0;
                }
                y++; // 내려온 줄 다시 검사
            }
        }

        if (linesCleared > 0)
        {
            score += linesCleared * 100;
            // 점수에 따라 낙하 속도 약간 증가 (최소 3틱)
            fallInterval = Math.Max(3, 10 - (score / 500));
        }
    }

    private bool IsValidPosition(int[,] piece, int targetX, int targetY)
    {
        int size = piece.GetLength(0);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (piece[y, x] != 0)
                {
                    int newX = targetX + x;
                    int newY = targetY + y;

                    // 좌우 및 바닥 경계 체크
                    if (newX < 0 || newX >= BoardWidth || newY >= BoardHeight)
                        return false;

                    // 이미 쌓인 블록과 충돌 체크
                    if (newY >= 0 && board[newY, newX] != 0)
                        return false;
                }
            }
        }
        return true;
    }

    private int[,] RotateMatrix(int[,] matrix)
    {
        int size = matrix.GetLength(0);
        int[,] result = new int[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                result[x, size - 1 - y] = matrix[y, x];
            }
        }
        return result;
    }

    private void DrawToBuffer()
    {
        ClearBuffer();
        SetStatusText($"[TETRIS] Score: {score} | 조작: 방향키(이동/회전), Space(즉시낙하)");

        // 1. 테두리 벽 그리기 (2칸 특수문자 사용)
        for (int y = 0; y <= BoardHeight; y++)
        {
            SetPixel(0, y, "🧱");
            SetPixel(BoardWidth + 1, y, "🧱");
        }
        for (int x = 0; x <= BoardWidth + 1; x++)
        {
            SetPixel(x, BoardHeight, "🧱");
        }

        // 2. 바닥에 쌓인 블록 그리기
        for (int y = 0; y < BoardHeight; y++)
        {
            for (int x = 0; x < BoardWidth; x++)
            {
                if (board[y, x] != 0)
                {
                    SetPixel(x + 1, y, "🟦");
                }
            }
        }

        // 3. 현재 조작 중인 블록 그리기
        int size = currentPiece.GetLength(0);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                if (currentPiece[y, x] != 0)
                {
                    SetPixel(pieceX + x + 1, pieceY + y, "🟨");
                }
            }
        }
    }
}
