using System;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    Move bestMove = Move.NullMove;
    int[] material = {0, 159, 450, 434, 716, 1421, 0};

    private int Evaluate(Board board)
    {
        int score = 0;
        for (var pieceIndex = 0; ++pieceIndex <= 6;)
            score += material[pieceIndex] * 
                    (BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard((PieceType)pieceIndex, true))
                   - BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard((PieceType)pieceIndex, false)));
        return board.IsWhiteToMove ? score : -score;
    }

    private int Search(Board board, Timer timer, int allocatedTime, int ply, int depth)
    {
        if (ply > 0 && board.IsRepeatedPosition())
            return 0;

        if (depth == 0)
            return Evaluate(board);

        var moves = board.GetLegalMoves();
        if (moves.Length == 0)
            return board.IsInCheck() ? ply - 10000 : 0;

        int bestScore = -20000;

        foreach (var move in moves)
        {
            board.MakeMove(move);
            int score = -Search(board, timer, allocatedTime, ply + 1, depth - 1);
            board.UndoMove(move);

            if (depth > 1 && timer.MillisecondsElapsedThisTurn > allocatedTime)
                return bestScore;

            if (score > bestScore)
            {
                bestScore = score;
                if (ply == 0) bestMove = move;
            }
        }

        return bestScore;
    }

    public Move Think(Board board, Timer timer)
    {
        bestMove = Move.NullMove;
        var allocatedTime = timer.MillisecondsRemaining / 30;

        for (var depth = 0; ++depth < 128 && timer.MillisecondsElapsedThisTurn <= allocatedTime;)
            Search(board, timer, allocatedTime, 0, depth);

        return bestMove;
    }
}