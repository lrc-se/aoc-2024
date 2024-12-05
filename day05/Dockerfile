FROM oven/bun:alpine

COPY . .
RUN bun build --compile --bytecode aoc.js

CMD ./aoc input.txt
