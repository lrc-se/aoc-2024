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
