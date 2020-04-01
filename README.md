# Colorshape AI Competition Bee AI

## Authors

* Rodrigo Pinheiro a21802488
  * Heuristic algorithm
  * Move Ordering
  * Negascout
* Tomás Franco a21803301
  * Heuristic algorithm
  * ~~Iterative deepening~~
  * Negamax Implementation

## AI Description

### The Negamax Algorithm

#### Specifics about our Negamax approach

#### Move Ordering
  
  The AI checks if the search has already been done and avoid repeating it.

#### Alpha Beta Cuts

  Alpha Beta cuts are used to prevent the AI to keep looking for good plays on
  "branches" that are not worth it.

#### Negascout

#### Iterative Deepening

  Iterative deepening is not currently implemented, too much time was going into
  trying to implement it and was decided to let it out of the equation.

### Heuristic evaluation

#### How we approached the problem

  It was decided that the heuristic should keep in mind if there were consequent
  shapes or colors of either player and evaluate the board with that as a basis,
  as there is a higher chance of winning or loosing if there are consequent
  pieces. It will then take into account the proximity to the center, since
  pieces located on the center are more likely to spread and be of more use / 
  dangerous.

#### Specifics about our Heuristic
  In the next episode of dragon ball z

## References
