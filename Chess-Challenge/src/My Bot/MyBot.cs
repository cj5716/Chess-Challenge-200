using System; // #DEBUG
using ChessChallenge.API;

public class MyBot : IChessBot
{
    private int Search(Board board, Timer timer, int depth, out Move bestMove)
    {
        bestMove = Move.NullMove;

        int score = 0;
        if (depth == 0)
        {
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
            score = -Search(board, timer, depth - 1, out _);
            board.UndoMove(move);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }

        return bestScore;
    }

    public Move Think(Board board, Timer timer)
    {
        int allocatedTime = timer.MillisecondsRemaining / 40;

        Move move = Move.NullMove;
        for (int depth = 0; timer.MillisecondsElapsedThisTurn <= allocatedTime;)
        { // #DEBUG
            Search(board, timer, ++depth, out move);
            Console.WriteLine($"depth {depth} {move}"); // #DEBUG
        } // #DEBUG

        return move;
    }
}