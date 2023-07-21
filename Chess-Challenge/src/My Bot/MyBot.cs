using System;
using ChessChallenge.API;

public class MyBot : IChessBot
{
    public Move Think(Board board, Timer timer)
    {
        Move[] allMoves = board.GetLegalMoves();
        int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
        // Pick a random move to play if nothing better is found
        Random rng = new();
        Move moveToPlay = allMoves[rng.Next(allMoves.Length)];
        int highestValueCapture = 0;

        foreach (Move move in allMoves)
        {
            // Always play checkmate in one
            if (MoveIsCheckmate(board, move))
            {
                moveToPlay = move;
                break;
            }

      
            

            // Find highest value capture
            Piece capturedPiece = board.GetPiece(move.TargetSquare);
            int capturedPieceValue = pieceValues[(int)capturedPiece.PieceType];

            board.MakeMove(move);
            var opponentMoves = board.GetLegalMoves();
            var opponentBestMove = 0;
            foreach (var opponentMove in opponentMoves)
            {
                if (MoveIsCheckmate(board, opponentMove))
                {
                    opponentBestMove = 100000;
                }
                Piece myCapturedPiece = board.GetPiece(opponentMove.TargetSquare);
                int myCapturedPieceValue = pieceValues[(int)myCapturedPiece.PieceType];

                if (myCapturedPieceValue > opponentBestMove)
                {
                    opponentBestMove = myCapturedPieceValue;
                }
            }

            capturedPieceValue -= opponentBestMove;
            board.UndoMove(move);
            
            if (capturedPieceValue > highestValueCapture)
            {
                moveToPlay = move;
                highestValueCapture = capturedPieceValue;
            }
        }

        return moveToPlay;
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