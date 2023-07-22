using System;
using System.Data;
using System.Linq;
using System.Runtime.InteropServices;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] allMoves = board.GetLegalMoves();
        // Pick a random move to play if nothing better is found
        Random rng = new();
        Move moveToPlay = allMoves[rng.Next(allMoves.Length)];

        var best = Int32.MinValue;
        
        foreach (var move in allMoves)
        {
            board.MakeMove(move);
        
            var moveScore = Search(board, 4, Int32.MinValue, Int32.MaxValue);
            if (moveScore > best)
            {
                best = moveScore;
                moveToPlay = move;
            }
            
            board.UndoMove(move);
        }

        return moveToPlay;
    }


    public int Search(Board board, int depth, int alpha, int beta)
    {
        if (depth == 0)
        {
            return Evaluate(board);
        }

        var moves = board.GetLegalMoves();

        if (!moves.Any())
        {
            if (board.IsInCheckmate())
            {
                return Int32.MinValue;
            }

            return 0;
        }

        foreach (var move in moves)
        {
            board.MakeMove(move);
            int evaluation = -Search(board, depth - 1, -beta, -alpha);
            board.UndoMove(move);
            if (evaluation >= beta)
            {
                // move was too good, opponent will avoid this position
                return beta;
            }

            alpha = Math.Max(alpha, evaluation);
        }

        return alpha;

    }

    private int Evaluate(Board board)
    {
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
        var pieceLists = board.GetAllPieceLists();
        var whites = 0;
        var blacks = 0;

        foreach (var pieceList in pieceLists)
        {
            var pieceValue = pieceValues[(int)pieceList.TypeOfPieceInList];
            if (pieceList.IsWhitePieceList)
            {
                whites += pieceList.Count() * pieceValue;
            }
            else
            {
                blacks += pieceList.Count() * pieceValue;
            }
        }

        return (whites - blacks) * (board.IsWhiteToMove ? 1 : -1);
    }
}