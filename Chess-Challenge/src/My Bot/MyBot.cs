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

            var moveScore = MiniMax(board, 2, true);
            if (moveScore >= best)
            {
                best = moveScore;
                moveToPlay = move;
            }
            
            board.UndoMove(move);
        }

        return moveToPlay;
    }


    public int MiniMax(Board board, int depth, bool maximizingPlayer)
    {
        if (depth == 0)
        {
            return EvaluateBoard(board, board.IsWhiteToMove);
        }

        if (maximizingPlayer)
        {
            var value = -Int32.MaxValue;
            var moves = board.GetLegalMoves();

            foreach (var move in moves)
            {
                board.MakeMove(move);
                value = Math.Max(value, MiniMax(board, depth - 1, false));
                board.UndoMove(move);
            }

            return value;
        }
        else
        {
            var value = Int32.MaxValue;
            var moves = board.GetLegalMoves();
            foreach (var move in moves)
            {
                board.MakeMove(move);
                value = Math.Min(value, MiniMax(board, depth - 1, true));
                board.UndoMove(move);
            }

            return value;
        }
    }

    private int EvaluateBoard(Board board, bool isWhite)
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

        
        if(isWhite)
        {
            return whites - blacks;
        }
        else
        {
            return blacks - whites;
        }
    }


    // Test if this move gives checkmate
    bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }
}