Advent of Code 2024
===================

[Solutions](#puzzles) for the 2024 edition of the [Cygnified Advent of Code](https://aoc.cygni.se/).

Going to try out Bun this year, as well as more bleeding-edge .NET.


Examples
-------

The repo includes a series of base examples in JavaScript and TypeScript for Bun and C# and F# for .NET 9, neither with any external dependencies. As usual the idea is to reuse common infrastructure code between solutions, only modifying functions in the puzzle file and adding more files when necessary. A few more simplifications have been squeezed out of [last year's](https://github.com/lrc-se/aoc-2023) common code, and the run script remains. The *Dockerfile*s perform bytecode compilation for Bun and native AOT compilation for .NET, enhancing startup characteristics for both.

The environment variable `part` is recognized as follows:

- `part1`: only runs part one
- `part2`: only runs part two

Any other value will abort the execution.

### Run script

The base examples also include a shell script *run.sh*, which will time the execution of the solution. It has the following syntax:

`run.sh part [mode] [testsuffix]`

- `part`: which part to run (sets the `part` environment variable accordingly)
- `mode`:
  - `test`: activates test mode
  - `rel`: builds the solution in release mode (where available)
  - `test-rel`: combines `test` and `rel`
- `testsuffix`: suffix to add to test input filename when in test mode

If the `mode` argument is omitted, the puzzle will be run in normal mode without release optimizations. Input is read from *input.txt* in normal mode, and from *input-testX.txt* in test mode where *X* is set to the value of `testsuffix`.


Web app
-------

Also included is a simple web application which offers a terminal-like interface for running the entries.

### Server

The server application has Bun itself as its only dependency and provides API access to the various entries. The following endpoints are available:

`GET /entries`

Returns a list of available entries in the following format:

```json
[
  {
    "name": "day01",
    "language": "JavaScript",
    "tests": [1, 2, ...]
  },
  ...
]
```

`GET /entries/{name}?part={part}&test={test}`

Runs an entry based on its `name` from the response above. The `part` query parameter sets the corresponding environment variable, and the optional numeric `test` parameter determines which test input to use, if any, taken from the available `tests` in the response above (`0` or no value will use the full input). Response format:

```json
{
  "language": "JavaScript",
  "output": ["line 1", "line 2", "line 3", ...]
}
```

Errors have the following format:

```json
{
  "error": "Something happened"
}
```

### Client

The client application is built with Vue and Vite, and is also served by the server application above when built for production.

### Port

The server will listen on port 1337 by default, but this can be overridden by setting the `port` environment variable. In dev mode the client port is handled by Vite, and in production mode (i.e. when served by the backend) the client is accessible at the server root URL.

### Development

The following commands are available:

`dev-backend`: Start the server in watch mode  
`dev-frontend`: Start the client in dev mode  
`start`: Start the server  
`build`: Build the client

In development mode the server and the client must be run separately, since the Vite dev server cannot coexist on the same port with the API. Also note that the server requires that the .NET entries it runs be built in release mode, so either do that manually first or run the full build script (see below).

### Docker

The web application has been containerized, supporting both .NET and Bun. Its *Dockerfile* executes the *build.sh* script in the project root, which will install Bun (if not already installed), execute a secondary *build.sh* script in every entry directory where it exists (which is currently only in .NET entries), and finally build the frontend. Running the container will then launch the server; remember to expose whatever port is passed as an environment variable as per above, or just use `-p 1337:1337` in the default case.


Puzzles
-------

### Day 1 (C#)

.NET built-ins all the way.

### Day 2 (JavaScript)

Just some loops.

### Day 3 (TypeScript)

Regexes took care of this. Also note that part 2 requires test input 2.

### Day 4 (C#)

Scanning for both regular and reversed forms only in forward directions, with early returns.

### Day 5 (JavaScript)

Sets'n'sorts, with some JS-specific shortcuts.

### Day 6 (C#)

Infinite loops again! Solved part 2 brute-forcefully first just to get the answer, and then optimized it by only placing obstructions in the guard's original path.

### Day 7 (F#)

Yay, recursion! Good fit for a functional approach, and it scans rather well too.

### Day 8 (C#)

Just coordinates and combinatorics, nothing fancy.

### Day 9 (TypeScript/C#)

Since the total size of the disk didn't look too big even with the full input I just used direct memory transfers, keeping track of free blocks separately in part 2.
I originally wrote the solution in TS but then I thought I'd see if a C# version based on writable spans would be significantly faster – and since it turned out to be just that I'm going with the C# version as the main entry and have relegated the TS version to a separate folder. `Span` is *fast*!

### Day 10 (C#)

Recursion again, and it didn't even exponentially explode. There are some further optimizations that could be made, but it's already fast enough so I'll abstain for now.

### Day 11 (JavaScript)

At first I thought that I had seen this before and knew what part 2 would be like, so I used a linked list from the get go, which worked fine for part 1. This time, however, it *did* exponentially explode, so since the sequence as such was not important and only the total count was, I switched to only keeping count of unique stone numbers instead and just summed them up at the end.

### Day 12 (C#)

This day represented the first real snag of the year, as usual in part 2. For part 1 I used the by now familiar flood fill technique to measure region sizes, and for part 2 I had the idea to reuse the boundary positions identified in the fill step to trace the edges, first horizontally and then vertically. This seemed promising, but there were a whole bunch of edge cases to handle before it passed all the tests (and the full input). There are probably better ways to do it, and better ways to do it this particular way, but it does the trick and completes in a jiffy too.

### Day 13 (TypeScript/F#)

The problem was basically a linear equation system with two equations and two unknowns, so I just solved that first and then used the resulting formulae to arrive at the results, checking for fractions to determine impossible combinations. In this way part 2 was merely a matter of changing the input values and removing the max condition, with no change in execution speed. I do note, however, that TS apparently isn't smart enough to figure out that the `.filter()` call should remove `null` from the resulting element type, but oh well.
*__Update:__ Added an F# version as well, where the `Option` type provides better semantics.*

### Day 14 (JavaScript)

Part 1 was simple enough, and for part 2 I started out by looking for less-likely-than-chance patterns in the output frames manually, which yielded two offsets and periods (one for each dimension) which I could then use to find the convergence and gaze upon the tree. Since *all* robot positions recur at *every* step after the first full cycle I couldn't think of another way to identify the offsets (or indeed the tree frame itself, short of analyzing the picture as such), so in the end I just did more or less the same thing in code by calculating minimum average robot distances and using that to extract the offsets and periods based on the supposition that the salient frames will in fact be the ones with the highest robot densities.
Note that the code works for both the test input and the full input by relying on certain characteristics of both, but that the test input will give a zero result for part 2 since it has no tree. Running part 2 with the full input will also output the picture of the tree as simple ASCII art before the result.

### Day 15 (C#)

Rather than maintaining position indices I decided to work with the map tiles directly, which was very simple in part 1. In part 2 it brought with it some extra hoops to jump through, but the end result, which doubles down on C# OO shortcuts, is quite fast.

### Day 16 (C#)

Well well, Mr. Dijkstra makes his yearly appearance. After last year's rough formulation I had a better idea of how to tailor the graph, but finding a method to only count nodes belonging to valid paths in part 2 took some time. It's very fast, though.

### Day 17

*Solved, needs cleanup before posting*

### Day 18 (C#)

Dijkstra again! Also this time the code can handle both the test input and the full input by way of relying on their respective formats.

### Day 19 (JavaScript)

Using simple recursion with a first-letter index, and a memoization cache in part 2.

### Day 20 (C#)

I misread and misunderstood the instructions for part 2 several times, but once I registered that the whole thing is basically Manhattan distances with a max limit it just took a few minutes to code up a solution. The special-case shortcut I already had in place for part 1 remains, however, since it's much faster there. As usual test input and full input both work.
