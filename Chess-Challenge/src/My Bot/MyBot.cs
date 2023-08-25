using System; // #DEBUG
using ChessChallenge.API;

public class MyBot : IChessBot
{
    int[] material = {0, 159, 450, 434, 716, 1421};

    private int Evaluate(Board board)
    {
        int score = 0;
        for (int pieceIndex = 0; ++pieceIndex <= 5;)
            score += material[pieceIndex] * 
                    (BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard((PieceType)pieceIndex, board.IsWhiteToMove))
                   - BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard((PieceType)pieceIndex, !board.IsWhiteToMove)));
        return score;
    }

    private int Search(Board board, Timer timer, int allocatedTime, int ply, int depth, out Move bestMove)
    {
        bestMove = Move.NullMove;

        if (ply > 0 && board.IsRepeatedPosition())
            return 0;

        if (depth == 0)
            return Evaluate(board);

        var moves = board.GetLegalMoves();
        if (moves.Length == 0)
            return board.IsInCheck() ? ply - 10000 : 0;

        int bestScore = -20000;

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int score = -Search(board, timer, allocatedTime, ply + 1, depth - 1, out _);
            board.UndoMove(move);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }

            if (depth > 1 && timer.MillisecondsElapsedThisTurn > allocatedTime)
                return bestScore;
        }

        return bestScore;
    }

    public Move Think(Board board, Timer timer)
    {
        Move bestMove = Move.NullMove;
        int allocatedTime = timer.MillisecondsRemaining / 30;

        for (int depth = 1;;depth++)
        {
            Search(board, timer, allocatedTime, 0, depth, out var move);

            if (timer.MillisecondsElapsedThisTurn > allocatedTime) break;

            bestMove = move;
            Console.WriteLine($"depth {depth} {bestMove}"); // #DEBUG
        }

        return bestMove;
    }
}