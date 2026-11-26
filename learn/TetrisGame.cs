using System;
using System.Collections.Generic;
using System.Threading;

namespace TetrisApp
{
    public class TetrisGame
    {
        // ==========================================
        // 1. 블록 클래스 계층 구조
        // ==========================================
        public class 블록
        {
            public string shape = "■";

            public 블록(string shape)
            {
                this.shape = shape;
            }

            public virtual string GetDisplayShape()
            {
                return shape;
            }
        }

        // 일정 시간(3초) 후 파괴되며 위 블록을 아래로 내리는 블록
        public class TimerBlock : 블록
        {
            public int deathTimer = 3;
            private int elapsedMs = 0;

            public TimerBlock(string shape) : base(shape)
            {
            }

            public bool TimeDeath(int deltaMs)
            {
                elapsedMs += deltaMs;
                if (elapsedMs >= 1000)
                {
                    int sec = elapsedMs / 1000;
                    deathTimer -= sec;
                    elapsedMs %= 1000;
                }
                return deathTimer <= 0;
            }

            public override string GetDisplayShape()
            {
                return deathTimer > 0 ? deathTimer.ToString() : "!";
            }
        }

        // 줄이 꽉 차도 사라지지 않으며, 오른쪽 설계도 모양을 완성해야 사라지는 건축 블록
        public class BlueprintBlock : 블록
        {
            public BlueprintBlock() : base("ㅁ")
            {
            }
        }

        // ==========================================
        // 2. 게임 상태 및 상수
        // ==========================================
        private const int Width = 10;
        private const int Height = 20;

        private 블록[,] board = new 블록[Height, Width];
        private Random rand = new Random();

        // 테트로미노 7종 모양 정의
        private readonly int[][,] shapes = new int[][,]
        {
            new int[,] { {1, 1, 1, 1} },                 // I
            new int[,] { {1, 1}, {1, 1} },               // O
            new int[,] { {0, 1, 0}, {1, 1, 1} },         // T
            new int[,] { {1, 0, 0}, {1, 1, 1} },         // J
            new int[,] { {0, 0, 1}, {1, 1, 1} },         // L
            new int[,] { {0, 1, 1}, {1, 1, 0} },         // S
            new int[,] { {1, 1, 0}, {0, 1, 1} }          // Z
        };

        // 오른쪽에 표시될 건축 설계도 목록 (1: ㅁ 블록 위치, 0: 상관없음)
        private readonly (string name, int[,] grid)[] blueprints = new (string, int[,])[]
        {
            ("작은 벙커", new int[,] {
                { 0, 1, 0 },
                { 1, 1, 1 }
            }),
            ("감시 초소", new int[,] {
                { 1, 1 },
                { 1, 1 }
            }),
            ("성벽 아치", new int[,] {
                { 1, 1, 1 },
                { 1, 0, 1 }
            }),
            ("십자 기념비", new int[,] {
                { 0, 1, 0 },
                { 1, 1, 1 },
                { 0, 1, 0 }
            })
        };

        private int currentBlueprintIndex = 0;

        private int[,] currentShape;
        private 블록[,] currentPieceBlocks;
        private int curX;
        private int curY;

        private bool isGameOver = false;
        private int score = 0;
        private int constructionCount = 0;

        private int fallTimer = 0;
        private int fallInterval = 500;
        private const int TickMs = 50;

        private string statusMessage = "";
        private int messageTimerMs = 0;

        // ==========================================
        // 3. 진입점 및 메인 루프 (입력 -> 갱신 -> 렌더링)
        // ==========================================
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.CursorVisible = false;

            TetrisGame game = new TetrisGame();
            game.Run();
        }

        public void Run()
        {
            SpawnPiece();

            while (!isGameOver)
            {
                Input();
                Update(TickMs);
                Render();
                Thread.Sleep(TickMs);
            }

            Render();
            Console.SetCursorPosition(0, Height + 6);
            Console.WriteLine($"게임 오버! 최종 점수: {score} | 완공 횟수: {constructionCount}");
            Console.CursorVisible = true;
        }

        // ==========================================
        // 4. 입력 (Input)
        // ==========================================
        private void Input()
        {
            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);
                switch (keyInfo.Key)
                {
                    case ConsoleKey.LeftArrow:
                        if (IsValidPosition(currentShape, curX - 1, curY))
                            curX--;
                        break;

                    case ConsoleKey.RightArrow:
                        if (IsValidPosition(currentShape, curX + 1, curY))
                            curX++;
                        break;

                    case ConsoleKey.DownArrow:
                        if (IsValidPosition(currentShape, curX, curY + 1))
                        {
                            curY++;
                            fallTimer = 0;
                        }
                        break;

                    case ConsoleKey.UpArrow:
                        RotatePiece();
                        break;

                    case ConsoleKey.Spacebar:
                        while (IsValidPosition(currentShape, curX, curY + 1))
                        {
                            curY++;
                        }
                        LockPiece();
                        SpawnPiece();
                        fallTimer = 0;
                        break;
                }
            }
        }

        // ==========================================
        // 5. 갱신 (Update)
        // ==========================================
        private void Update(int deltaMs)
        {
            if (messageTimerMs > 0)
            {
                messageTimerMs -= deltaMs;
                if (messageTimerMs <= 0)
                {
                    statusMessage = "";
                }
            }

            // 1) 타이머 블록 시간 감소 및 파괴 처리
            bool anyTimerDestroyed = false;
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (board[y, x] is TimerBlock tb)
                    {
                        if (tb.TimeDeath(deltaMs))
                        {
                            board[y, x] = null;
                            anyTimerDestroyed = true;
                        }
                    }
                }
            }

            if (anyTimerDestroyed)
            {
                ApplyGravity();
                CheckBlueprintCompletion();
                ClearLines();
            }

            // 2) 현재 조작 중인 블록 자동 낙하
            fallTimer += deltaMs;
            if (fallTimer >= fallInterval)
            {
                fallTimer = 0;
                if (IsValidPosition(currentShape, curX, curY + 1))
                {
                    curY++;
                }
                else
                {
                    LockPiece();
                    SpawnPiece();
                }
            }
        }

        // ==========================================
        // 6. 렌더링 (Render)
        // ==========================================
        private void Render()
        {
            Console.SetCursorPosition(0, 0);

            string[,] display = new string[Height, Width];
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    display[y, x] = board[y, x] != null ? board[y, x].GetDisplayShape() : "·";
                }
            }

            if (currentShape != null && !isGameOver)
            {
                int rows = currentShape.GetLength(0);
                int cols = currentShape.GetLength(1);
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < cols; c++)
                    {
                        if (currentShape[r, c] == 1)
                        {
                            int bx = curX + c;
                            int by = curY + r;
                            if (by >= 0 && by < Height && bx >= 0 && bx < Width)
                            {
                                display[by, bx] = currentPieceBlocks[r, c].GetDisplayShape();
                            }
                        }
                    }
                }
            }

            var bp = blueprints[currentBlueprintIndex];
            int bpRows = bp.grid.GetLength(0);
            int bpCols = bp.grid.GetLength(1);

            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendLine($"점수: {score,-6} | 완공: {constructionCount}회          ");
            sb.AppendLine(string.IsNullOrEmpty(statusMessage)
                ? "                                            "
                : $"{statusMessage,-44}");

            sb.AppendLine("┌────────────────────┐    ┌──────────────────┐");
            for (int y = 0; y < Height; y++)
            {
                sb.Append("│");
                for (int x = 0; x < Width; x++)
                {
                    string cell = display[y, x];
                    if (cell.Length == 1 && (cell == "·" || char.IsDigit(cell[0]) || cell == "!"))
                        sb.Append(cell + " ");
                    else
                        sb.Append(cell);
                }
                sb.Append("│    ");

                // 오른쪽 패널 (설계도 및 안내)
                if (y == 0)
                    sb.Append($"│ [건설 설계도]    │");
                else if (y == 1)
                    sb.Append($"│ 목표: {bp.name,-10} │");
                else if (y >= 3 && y < 3 + bpRows)
                {
                    int r = y - 3;
                    sb.Append("│   ");
                    for (int c = 0; c < 6; c++)
                    {
                        if (c < bpCols && bp.grid[r, c] == 1)
                            sb.Append("ㅁ");
                        else
                            sb.Append("  ");
                    }
                    sb.Append("   │");
                }
                else if (y == 7)
                    sb.Append("├──────────────────┤");
                else if (y == 8)
                    sb.Append("│ ■ : 일반 블록    │");
                else if (y == 9)
                    sb.Append("│ 3 : 3초 후 파괴  │");
                else if (y == 10)
                    sb.Append("│ ㅁ: 건축 자재    │");
                else if (y == 11)
                    sb.Append("│  (줄 지우기 면역)│");
                else if (y == 12)
                    sb.Append("│  (설계도 완성 시 │");
                else if (y == 13)
                    sb.Append("│   제거 & 완공!)  │");
                else if (y == Height - 1)
                    sb.Append("└──────────────────┘");
                else
                    sb.Append("│                  │");

                sb.AppendLine();
            }
            sb.AppendLine("└────────────────────┘");
            sb.AppendLine("조작: ← → 이동 | ↑ 회전 | ↓ 내리기 | Space 즉시낙하");

            Console.Write(sb.ToString());
        }

        // ==========================================
        // 7. 핵심 게임 로직 (생성, 회전, 고정, 설계도 판정, 중력)
        // ==========================================
        private void SpawnPiece()
        {
            int idx = rand.Next(shapes.Length);
            currentShape = (int[,])shapes[idx].Clone();

            int rows = currentShape.GetLength(0);
            int cols = currentShape.GetLength(1);
            currentPieceBlocks = new 블록[rows, cols];

            // 블록 타입 결정: 40% 확률로 'ㅁ' 건축 자재 블록 조각 등장, 25% 타이머 블록, 35% 일반 블록
            int roll = rand.Next(100);
            int pieceType = roll < 40 ? 2 : (roll < 65 ? 1 : 0);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (currentShape[r, c] == 1)
                    {
                        if (pieceType == 2)
                            currentPieceBlocks[r, c] = new BlueprintBlock();
                        else if (pieceType == 1)
                            currentPieceBlocks[r, c] = new TimerBlock("■");
                        else
                            currentPieceBlocks[r, c] = new 블록("■");
                    }
                }
            }

            curX = (Width - cols) / 2;
            curY = 0;

            if (!IsValidPosition(currentShape, curX, curY))
            {
                isGameOver = true;
            }
        }

        private void RotatePiece()
        {
            int rows = currentShape.GetLength(0);
            int cols = currentShape.GetLength(1);
            int[,] rotatedShape = new int[cols, rows];
            블록[,] rotatedBlocks = new 블록[cols, rows];

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    rotatedShape[c, rows - 1 - r] = currentShape[r, c];
                    rotatedBlocks[c, rows - 1 - r] = currentPieceBlocks[r, c];
                }
            }

            if (IsValidPosition(rotatedShape, curX, curY))
            {
                currentShape = rotatedShape;
                currentPieceBlocks = rotatedBlocks;
            }
        }

        private bool IsValidPosition(int[,] shape, int nextX, int nextY)
        {
            int rows = shape.GetLength(0);
            int cols = shape.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (shape[r, c] == 1)
                    {
                        int bx = nextX + c;
                        int by = nextY + r;

                        if (bx < 0 || bx >= Width || by >= Height)
                            return false;

                        if (by >= 0 && board[by, bx] != null)
                            return false;
                    }
                }
            }
            return true;
        }

        private void LockPiece()
        {
            int rows = currentShape.GetLength(0);
            int cols = currentShape.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (currentShape[r, c] == 1)
                    {
                        int bx = curX + c;
                        int by = curY + r;
                        if (by >= 0 && by < Height && bx >= 0 && bx < Width)
                        {
                            board[by, bx] = currentPieceBlocks[r, c];
                        }
                    }
                }
            }

            // 설계도 완성 여부 먼저 체크 후 일반 줄 지우기 실행
            CheckBlueprintCompletion();
            ClearLines();
        }

        // 보드 전체를 탐색하여 오른쪽 설계도와 일치하는 'ㅁ' 패턴이 있으면 제거
        private void CheckBlueprintCompletion()
        {
            var bp = blueprints[currentBlueprintIndex];
            int bpRows = bp.grid.GetLength(0);
            int bpCols = bp.grid.GetLength(1);

            bool completedAny = false;

            for (int startY = 0; startY <= Height - bpRows; startY++)
            {
                for (int startX = 0; startX <= Width - bpCols; startX++)
                {
                    if (MatchesBlueprintAt(bp.grid, startX, startY))
                    {
                        // 일치한 'ㅁ' 블록들을 보드에서 제거
                        for (int r = 0; r < bpRows; r++)
                        {
                            for (int c = 0; c < bpCols; c++)
                            {
                                if (bp.grid[r, c] == 1)
                                {
                                    board[startY + r, startX + c] = null;
                                }
                            }
                        }
                        completedAny = true;
                        constructionCount++;
                        score += 500;
                    }
                }
            }

            if (completedAny)
            {
                statusMessage = "★ 건설에 성공했습니다 따바리쉬! ★";
                messageTimerMs = 3500;
                currentBlueprintIndex = (currentBlueprintIndex + 1) % blueprints.Length;
                ApplyGravity();
            }
        }

        private bool MatchesBlueprintAt(int[,] grid, int startX, int startY)
        {
            int bpRows = grid.GetLength(0);
            int bpCols = grid.GetLength(1);

            for (int r = 0; r < bpRows; r++)
            {
                for (int c = 0; c < bpCols; c++)
                {
                    if (grid[r, c] == 1)
                    {
                        블록 cell = board[startY + r, startX + c];
                        if (cell == null || cell.shape != "ㅁ")
                            return false;
                    }
                }
            }
            return true;
        }

        // 'ㅁ' 블록이 하나라도 포함된 줄은 꽉 차도 사라지지 않음
        private void ClearLines()
        {
            int linesCleared = 0;

            for (int y = Height - 1; y >= 0; y--)
            {
                bool isFull = true;
                bool hasBlueprintBlock = false;

                for (int x = 0; x < Width; x++)
                {
                    if (board[y, x] == null)
                    {
                        isFull = false;
                        break;
                    }
                    if (board[y, x].shape == "ㅁ")
                    {
                        hasBlueprintBlock = true;
                    }
                }

                // 꽉 찼더라도 'ㅁ' 블록이 포함되어 있으면 제거하지 않고 건너뜀
                if (isFull && !hasBlueprintBlock)
                {
                    linesCleared++;
                    for (int moveY = y; moveY > 0; moveY--)
                    {
                        for (int x = 0; x < Width; x++)
                        {
                            board[moveY, x] = board[moveY - 1, x];
                        }
                    }
                    for (int x = 0; x < Width; x++)
                    {
                        board[0, x] = null;
                    }
                    y++;
                }
            }

            if (linesCleared > 0)
            {
                score += linesCleared * 100;
            }
        }

        // 파괴/제거 후 공중에 뜬 블록들을 아래로 내림
        private void ApplyGravity()
        {
            for (int x = 0; x < Width; x++)
            {
                int writeY = Height - 1;
                for (int readY = Height - 1; readY >= 0; readY--)
                {
                    if (board[readY, x] != null)
                    {
                        블록 temp = board[readY, x];
                        board[readY, x] = null;
                        board[writeY, x] = temp;
                        writeY--;
                    }
                }
            }
        }
    }
}
