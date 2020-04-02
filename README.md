# Simplexity Competition - Bee AI

## Authors of Bee

* [Rodrigo Pinheiro](https://github.com/ThomasFranque) a21802488
  * Heuristic algorithm
  * Move Ordering
  * Negascout
* [Tomás Franco](https://github.com/ThomasFranque) a21803301
  * Heuristic algorithm
  * ~~Iterative deepening~~
  * Negamax Implementation

## AI Description

### The Negamax Algorithm

#### Specifics about our Negamax approach

#### Move Ordering
  
  The AI checks if the search has already been done and avoid repeating it.

#### Alpha Beta Cuts

  Alpha Beta cuts are used to prevent the AI to keep looking for plays on
  "branches" that are not worth it.

#### Negascout

  ```I negate the scout```

#### Iterative Deepening

  Iterative deepening is not currently implemented, too much time was going into
  trying to implement it and was decided to let it out of the equation.

### Heuristic evaluation

#### How we approached the problem

  It was decided that the heuristic should keep in mind if there were consequent
  shapes or colors of either player and evaluate the board with that as a basis,
  as there is a higher chance of winning or loosing if there are consequent
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

## References

### From [Chess Wiki](https://www.chessprogramming.org/Main_Page)

[Negamax](https://www.chessprogramming.org/Negamax)

[Move ordering](https://www.chessprogramming.org/Move_Ordering)

### Others

[YouTube - Transposition Tables & Zobrist Keys](https://www.youtube.com/watch?v=QYNRvMolN20)

Help from [Miguel Fernandez](https://github.com/MizuRyujin)
(21803644) and [João Rebelo](https://github.com/JBernardoRebelo)
(21805230) in some of logic and debugging.

#### The project was developed on [This Repository](https://github.com/RodrigoPrinheiro/AI_ColorShapes_Simplexity)
