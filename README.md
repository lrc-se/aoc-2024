Advent of Code 2024
===================

Solutions for the 2024 edition of the [Cygnified Advent of Code](https://aoc.cygni.se/).

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


Server
------

Also included is a simple server application, with Bun itself as its only dependency, which provides API access to the various entries. The following endpoints are available:

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

### Port

The server will listen on port 1337 by default, but this can be overridden by setting the `port` environment variable.

### Docker

The server has been containerized, supporting both .NET and Bun. Its *Dockerfile* executes the *build.sh* script in the project root, which will install Bun (if not already installed) and then execute a secondary *build.sh* script in every entry directory where it exists (which is currently only in .NET entries). Running the container will then launch the server; remember to expose whatever port is passed as an environment variable as per above, or just use `-p 1337:1337` in the default case.


Puzzles
-------

TBA
