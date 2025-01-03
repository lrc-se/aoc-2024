import { $, file, serve } from "bun";
import { join, resolve } from "path";
import { readdir } from "node:fs/promises";

const Entrypoints = {
  JavaScript: "aoc.js",
  TypeScript: "aoc.ts",
  "C#": "Aoc.cs",
  "F#": "Aoc.fs"
};

const entries = {};

const distPath = resolve("./dist");

const dirs = await readdir(".", { withFileTypes: true });
for (const dir of dirs.filter(d => d.isDirectory() && (d.name.startsWith("day") || d.name.startsWith("example-")))) {
  const entrypoint = await getEntry(dir);
  if (entrypoint) {
    entries[entrypoint.name] = entrypoint;
  }
}

const server = serve({
  async fetch(req) {
    const url = new URL(req.url);
    if (url.pathname === "/entries") {
      return createResponse(getEntries());
    }
    const matchEntry = /^\/entries\/([a-zA-Z0-9-]+)$/.exec(url.pathname);
    if (matchEntry) {
      return runEntry(matchEntry[1], url.searchParams.get("part"), Number(url.searchParams.get("test")));
    }
    const staticFile = await getStaticFile(url.pathname);
    if (staticFile) {
      return new Response(staticFile);
    }
    return createErrorResponse("Route not found", 404);
  },
  port: process.env.port || 1337
});
console.log(`Running on port ${server.port}`);

process.on("SIGINT", async () => {
  console.log("Shutting down");
  await server.stop();
  process.exit();
});


async function getEntry(dir) {
  for (const [language, entrypoint] of Object.entries(Entrypoints)) {
    if (await file(join(dir.name, entrypoint)).exists()) {
      const files = await readdir(join(dir.parentPath, dir.name));
      const tests = files
        .filter(file => file.startsWith("input-test"))
        .map(file => parseInt(file.substring(10)) || 1)
        .toSorted((a, b) => a - b);
      return {
        name: dir.name,
        cmd: getCommand(entrypoint),
        language,
        tests
      };
    }
  }
  return null;
}

function getCommand(entrypoint) {
  switch (entrypoint) {
    case Entrypoints.JavaScript:
    case Entrypoints.TypeScript:
      return `bun run ${entrypoint}`;
    case Entrypoints["C#"]:
    case Entrypoints["F#"]:
      return "./bin/Release/net9.0/Aoc";
    default:
      throw Error("Unreachable");
  }
}

function getEntries() {
  return Object.values(entries)
    .map(entry => ({ name: entry.name, language: entry.language, tests: entry.tests }))
    .toSorted((a, b) => a.name.localeCompare(b.name));
}

async function runEntry(name, part, test) {
  const entry = entries[name];
  if (!entry) {
    return createErrorResponse("Entry not found", 404);
  }
  const filename = test
    ? (test > 1 ? `input-test${test}.txt` : "input-test.txt")
    : "input.txt";
  try {
    const output = await $`${{ raw: entry.cmd }} ${filename}`
      .cwd(entry.name)
      .env({ ...process.env, part: part ?? "" })
      .text();
    if (!output) {
      return createErrorResponse("Running entry produced no output", 500);
    }
    return createResponse({
      language: entry.language,
      output: output.trimEnd().split(/\r?\n/)
    });
  } catch {
    return createErrorResponse("Error running entry", 500);
  }
}

async function getStaticFile(path) {
  const filePath = resolve(join(distPath, path === "/" ? "index.html" : path));
  if (!filePath.startsWith(distPath)) {
    return null;
  }
  const staticFile = file(filePath);
  if (await staticFile.exists()) {
    return staticFile;
  }
  return null;
}

function createResponse(data, status = 200) {
  const response = Response.json(data, { status });
  response.headers.set("Access-Control-Allow-Origin", "*");
  return response;
}

function createErrorResponse(error, status) {
  return createResponse({ error }, status);
}
