using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mankalah
{
    // rename me
    public class tja9Player : Player // class must be public
    {
        public tja9Player(Position pos, int maxTimePerMove) // constructor must match this signature
            : base(pos, "tja9", maxTimePerMove) // choose a string other than "MyPlayer"
        {
        }

        // adapt all code from your player class into this

        public override string gloat()
        {
            return "I WIN!";
        }

        public override int chooseMove(Board b)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            int i = 1;
            Result move = new Result(0, 0, false);
            try
            {
                while (!move.isEndGame())
                {
                    move = minimaxVal(b, i++, watch, Int32.MinValue, Int32.MaxValue);
                }
            }
            catch (MoveTimedOutException)
            { }
            return move.getMove();
        }

        public String getImage() { return "pablo.png"; }

        private Result minimaxVal(Board b, int d, Stopwatch w, int alpha, int beta)
        {
            if (w.ElapsedMilliseconds > getTimePerMove()) throw new MoveTimedOutException();
            int bestMove = 0;
            int bestVal;
            bool gameCompleted = false;
            if (b.gameOver() || d == 0)
            {
                return new Result(0, evaluate(b), b.gameOver());
            }

            if (b.whoseMove() == Position.Top)
            {
                bestVal = Int32.MinValue;
                for (int move = 7; move <= 12; move++)
                {
                    if (b.legalMove(move))
                    {
                        Board b1 = new Board(b);
                        b1.makeMove(move, false);
                        Result val = minimaxVal(b1, d - 1, w, alpha, beta);
                        if (val.getScore() > bestVal)
                        {
                            bestVal = val.getScore();
                            bestMove = move;
                            gameCompleted = val.isEndGame();
                        }
                        if (bestVal > alpha)
                        {
                            alpha = bestVal;
                        }
                    }
                }
            }
            else
            {
                bestVal = Int32.MaxValue;
                for (int move = 0; move <= 5; move++)
                {
                    if (b.legalMove(move))
                    {
                        Board b1 = new Board(b);
                        b1.makeMove(move, false);
                        Result val = minimaxVal(b1, d - 1, w, alpha, beta);
                        if (val.getScore() < bestVal)
                        {
                            bestVal = val.getScore();
                            bestMove = move;
                            gameCompleted = val.isEndGame();
                        }
                        if (bestVal < beta)
                        {
                            beta = bestVal;
                        }
                    }
                }
            }
            return new Result(bestMove, bestVal, gameCompleted);
        }

        public override int evaluate(Board b)
        {
            int score = b.stonesAt(13) - b.stonesAt(6);
            int stonesTotal = 0;
            int goAgainsTotal = 0;
            int capturesTotal = 0;

            for (int i = 7; i <= 12; i++)
            {
                int priority = 0;
                int target = b.stonesAt(i) % (13 - i);
                int targetStonesAt = b.stonesAt(target + 7);
                if (b.whoseMove() == Position.Bottom)
                {
                    stonesTotal -= b.stonesAt(i);

                    if ((b.stonesAt(i) - (13 - i) == 0) || (b.stonesAt(i) - (13 - i)) == 13)
                    {
                        goAgainsTotal -= (1 + priority);
                    }
                    if (targetStonesAt == 0 && b.stonesAt(i) == (13 - i + target + 7))
                    {
                        capturesTotal += (b.stonesAt(i) + b.stonesAt(12 - target));
                    }
                }
                else
                {
                    stonesTotal += b.stonesAt(i);

                    if ((b.stonesAt(i) - (13 - i) == 0) || (b.stonesAt(i) - (13 - i)) == 13)
                    {
                        goAgainsTotal += (1 + priority);
                    }
                    if (targetStonesAt == 0 && b.stonesAt(i) == (13 - i + target + 7))
                    {
                        capturesTotal -= (b.stonesAt(i) + b.stonesAt(12 - target));
                    }
                }
                priority++;
            }

            for (int i = 0; i <= 5; i++)
            {
                int priority = 0;
                int target = b.stonesAt(i) % (13 - i);
                int targetStonesAt = b.stonesAt(target);
                if (b.whoseMove() == Position.Bottom)
                {
                    stonesTotal += b.stonesAt(i);

                    if ((b.stonesAt(i) - (6 - i) == 0) || (b.stonesAt(i) - (6 - i)) == 13)
                    {
                        goAgainsTotal -= (1 + priority);
                    }
                    if (targetStonesAt == 0 && b.stonesAt(i) == (13 - i + target))
                    {
                        capturesTotal -= (b.stonesAt(i) + b.stonesAt(12 - target));
                    }
                }
                else
                {
                    stonesTotal -= b.stonesAt(i);

                    if ((b.stonesAt(i) - (6 - i) == 0) || (b.stonesAt(i) - (6 - i)) == 13)
                    {
                        goAgainsTotal += (1 + priority);
                    }
                    if (targetStonesAt == 0 && b.stonesAt(i) == (13 - i + target))
                    {
                        capturesTotal += (b.stonesAt(i) + b.stonesAt(12 - target));
                    }
                }
                priority++;
            }

            score += stonesTotal + capturesTotal + goAgainsTotal;
            return score;
        }
    }
    class MoveTimedOutException : Exception { }

    class Result
    {
        private int Move;
        private int Score;
        private bool GameState;
        public Result(int move, int score, bool state)
        {
            Move = move;
            Score = score;
            GameState = state;
        }
        public int getMove()
        {
            return Move;
        }
        public int getScore()
        {
            return Score;
        }
        public bool isEndGame()
        {
            return GameState;
        }

    }
}