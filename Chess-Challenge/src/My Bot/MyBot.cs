using System; // #DEBUG
using ChessChallenge.API;

public class MyBot : IChessBot
{
    int allocatedTime = 0;
    private int Search(Board board, Timer timer, int depth, out Move bestMove)
    {
        bestMove = Move.NullMove;

        if (depth == 0)
        {
            int score = 0;
            for (int pieceIndex = 0; ++pieceIndex <= 5;)
                score += 148 * pieceIndex * 
                        (BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard((PieceType)pieceIndex, board.IsWhiteToMove))
                       - BitboardHelper.GetNumberOfSetBits(board.GetPieceBitboard((PieceType)pieceIndex, !board.IsWhiteToMove)));
            return score;
        }

        var moves = board.GetLegalMoves();
        if (moves.Length == 0)
            return board.IsInCheck() ? -10000 : 0;

        int bestScore = -20000;

        foreach (Move move in moves)
        {
            board.MakeMove(move);
            int score = -Search(board, timer, depth - 1, out _);
            board.UndoMove(move);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }

            if (timer.MillisecondsElapsedThisTurn > allocatedTime)
                return bestScore;
        }

        return bestScore;
    }

    public Move Think(Board board, Timer timer)
    {
        Move bestMove = Move.NullMove;
        allocatedTime = timer.MillisecondsRemaining / 30;

        for (int depth = 1;;depth++)
        {
            Search(board, timer, depth, out var move);

            if (timer.MillisecondsElapsedThisTurn > allocatedTime) break;

            bestMove = move;
            Console.WriteLine($"depth {depth} {bestMove}"); // #DEBUG
        }

        return bestMove;
    }
}