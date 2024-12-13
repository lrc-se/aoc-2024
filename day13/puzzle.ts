interface Offset {
  x: number;
  y: number;
}

interface ClawMachine {
  buttonA: Offset;
  buttonB: Offset;
  prize: Offset;
}

interface Pushes {
  buttonA: number;
  buttonB: number;
}

type Input = ClawMachine[];

function getPushes(machine: ClawMachine): Pushes | null {
  const buttonA = (machine.prize.y * machine.buttonB.x - machine.prize.x * machine.buttonB.y) / (machine.buttonA.y * machine.buttonB.x - machine.buttonB.y * machine.buttonA.x);
  if (Math.floor(buttonA) !== buttonA) {
    return null;
  }
  const buttonB = (machine.prize.x - buttonA * machine.buttonA.x) / machine.buttonB.x;
  if (Math.floor(buttonB) !== buttonB) {
    return null;
  }
  return { buttonA, buttonB };
}

function getTokens(pushes: Pushes[]) {
  return pushes.reduce((tokens, push) => tokens + push.buttonA * 3 + push.buttonB, 0);
}

export function parseInput(rawInput: string): Input {
  const buttonRe = /X\+(\d+), Y\+(\d+)/;
  const prizeRe = /X=(\d+), Y=(\d+)/;
  const machines: ClawMachine[] = [];
  for (const section of rawInput.split("\n\n")) {
    const lines = section.split("\n");
    const buttonA = buttonRe.exec(lines[0])!;
    const buttonB = buttonRe.exec(lines[1])!;
    const prize = prizeRe.exec(lines[2])!;
    machines.push({
      buttonA: { x: +buttonA[1], y: +buttonA[2] },
      buttonB: { x: +buttonB[1], y: +buttonB[2] },
      prize: { x: +prize[1], y: +prize[2] },
    });
  }
  return machines;
}

export function runPartOne(input: Input) {
  const allPushes = input.map(getPushes).filter(push => push && push.buttonA <= 100 && push.buttonB <= 100);
  return getTokens(allPushes as Pushes[]);
}

export function runPartTwo(input: Input) {
  for (const machine of input) {
    machine.prize.x += 10000000000000;
    machine.prize.y += 10000000000000;
  }
  const allPushes = input.map(getPushes).filter(Boolean);
  return getTokens(allPushes as Pushes[]);
}
