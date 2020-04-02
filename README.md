# Simplexity Competition - Bee AI

## Authors of Bee

* [Rodrigo Pinheiro](https://github.com/ThomasFranque) a21802488
  * Heuristic algorithm
  * Simple Move Ordering
  * Transposition Table and Zobrist Hashing
  * ~~Negascout~~ (tried but failed)
* [Tomás Franco](https://github.com/ThomasFranque) a21803301
  * Heuristic algorithm
  * ~~Iterative deepening~~ (tried but failed)
  * Negamax Implementation

## AI Description

### The Negamax Algorithm

  We decided to use the Negamax search algorithm with Alpha-Beta pruning for our AI
  based on that we can have more malleability and in the long run be easier to read and
  understand what we were doing.

  Another thing that led us to use Negamax right from the start was that a lot of 
  the source material and search we've done was based on it and improved upon
  Negamax instead of using Minimax.

#### Specifics about our Negamax approach

  How we've done the Negamax algorithm doesn't have anything really special, and the
  only thing to account for here is that we pass the beta to the child node directly
  with `-Math.Max(alpha, best.score)` cutting an if statement further on, making
  the code more readable.

#### Move Ordering
  
  Our move ordering is simple and quick, as we lost more time researching what we 
  could improve upon in the general algorithm and failing then focusing in this aspect.
  
  Mainly, the AI checks if the search has already been done and avoid repeating it
  through the transposition table.
  
  We also favor moves with our shape and run through them first.

#### Alpha Beta Cuts

  Alpha Beta cuts are used to prevent the AI to keep looking for plays on
  "branches" or "nodes" that are not worth it.

#### Negascout

  Negascout is a really good improvement in the search algorithm of the AI but it
  requires a more complex form of move ordering that we didn't have.
  Ultimately not having good move ordering makes the AI run at the same speed
  as a normal Negamax.

  In the tests using this algorithm we verified that most of the times we would get
  the same answer in the same time, slower time, or breaking the AI completely
  from pruning a branch that wasn't suppose to.

#### Iterative Deepening

  Iterative deepening is not currently implemented, too much time was going into
  trying to implement it and was decided to let it out of the equation.

### Heuristic evaluation

#### How we approached the problem

  It was decided that the heuristic should keep in mind if there were consecutive
  shapes or colors of either player and evaluate the board with that as a basis,
  as there is a higher chance of winning or loosing if there are consecutive
  pieces.

#### Specifics about our Heuristic

  The heuristic will begin by initializing the base value that will be
  decremented throughout the analysis.
  For every win corridor we check every occupied spot and analyse the piece.
  If the piece is of our color or shape it will count towards our consequent
  lines else, it will count towards the enemy. It is also taken into account
  the shape and the color, both having values, being the color the most
  valuable.

  If a checkmate is found, the win value is instantly returned, preventing
  further operations.

  In case we find a check by the other player a flag is activated and, on the
  final return, if there is no checkmates from our part, it will return the
  lowest possible value.

#### What's missing from the Heuristic

  The heuristic is quite simple as it is right now, and it requires an extra step
  to become usable in the tournament. If further development is the case we would
  have to maybe make constant tables for heuristic points for specific positions in
  the board.

  In the following example, taking a board of size 6x7:

  ```cs
  boardGrades[] = new array[]
  {
      0,   0,   0,   0,   0,   0,   0,
      0.1, 0.1, 0.2, 0.2, 0.2, 0.1, 0.1,
      0.2, 0.3, 0.4, 5, 0.4, 0.1, 0.1,
      0.2, 0.3, 5,   5,   5, 0.1, 0.1,
      0.2, 0.3, 5,   5,   5, 0.1, 0.1,
      1,   1,   0,   0,   0,  1,    1,

  }
  ```
  Using a table similar to this one to evaluate each position in the board and
  relative to a whole game would wield way better results than only checking
  if any line in the board was to be completed or was already completed.

  Multiple times with our static evaluation we end up placing a piece on top of
  an impossible stack (line that cannot be completed) at the top of the board,
  rendering this move useless.

## References

### From [Chess Wiki](https://www.chessprogramming.org/Main_Page)

[Negamax](https://www.chessprogramming.org/Negamax)

[Move ordering](https://www.chessprogramming.org/Move_Ordering)

### Others

[YouTube - Transposition Tables & Zobrist Keys](https://www.youtube.com/watch?v=QYNRvMolN20) 

[C# time elapse computation in milliseconds](https://stackoverflow.com/questions/13589853/time-elapse-computation-in-milliseconds-c-sharp);

Help from [Miguel Fernandez](https://github.com/MizuRyujin)
(21803644) and [João Rebelo](https://github.com/JBernardoRebelo)
(21805230) in some of logic and debugging.

#### The project was developed on [This Repository](https://github.com/RodrigoPrinheiro/AI_ColorShapes_Simplexity).
