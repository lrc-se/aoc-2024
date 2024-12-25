FROM oven/bun:alpine

COPY . .
RUN bun build --compile --bytecode aoc.ts

CMD ./aoc input.txt
