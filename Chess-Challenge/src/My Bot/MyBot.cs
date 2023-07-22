using System;
using System.Linq;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] allMoves = board.GetLegalMoves();
        Random rng = new();
        Move moveToPlay = allMoves[rng.Next(allMoves.Length)];

        var best = Int32.MinValue;
        
        foreach (var move in allMoves)
        {
            board.MakeMove(move);
        
            var moveScore = Search(board, 4, int.MinValue, int.MaxValue);
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
            return board.IsInCheckmate() ? int.MinValue : 0;
        }

        foreach (var move in moves)
        {
            board.MakeMove(move);
            int score = -Search(board, depth - 1, -beta, -alpha);
            board.UndoMove(move);

            if (score >= beta)
            {
                // move was too good, opponent will avoid this position
                return beta;
            }

            alpha = Math.Max(alpha, score);
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
            var piecesValue = pieceValues[(int)pieceList.TypeOfPieceInList] * pieceList.Count;
            if (pieceList.IsWhitePieceList)
            {
                whites += piecesValue;
            }
            else
            {
                blacks += piecesValue;
            }
        }

        return (whites - blacks) * (board.IsWhiteToMove ? 1 : -1);
    }
}