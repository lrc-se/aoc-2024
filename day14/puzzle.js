const re = /p=(\d+),(\d+) v=(-?\d+),(-?\d+)/;

function createRobot(line) {
  const match = re.exec(line);
  return {
    // @ts-ignore not null
    position: { x: Number(match[1]), y: Number(match[2]) },
    // @ts-ignore not null
    velocity: { x: Number(match[3]), y: Number(match[4]) }
  };
}

function mod(a, b) {
  return ((a % b) + b) % b;
}

function moveRobots(input) {
  for (const robot of input.robots) {
    robot.position.x = mod(robot.position.x + robot.velocity.x, input.width);
    robot.position.y = mod(robot.position.y + robot.velocity.y, input.height);
  }
}

function getSafetyFactor(input) {
  const midX = input.width >> 1;
  const midY = input.height >> 1;
  return input.robots.filter(robot => robot.position.x < midX && robot.position.y < midY).length *
    input.robots.filter(robot => robot.position.x > midX && robot.position.y < midY).length *
    input.robots.filter(robot => robot.position.x < midX && robot.position.y > midY).length *
    input.robots.filter(robot => robot.position.x > midX && robot.position.y > midY).length;
}

function getAverageMinDistance(robots) {
  let totalMinDist = 0;
  const count = robots.length - 1;
  for (let i = 0; i < count; ++i) {
    let minDist = Number.MAX_SAFE_INTEGER;
    for (let j = i + 1; j < robots.length; ++j) {
      const dist = Math.abs(robots[i].position.x - robots[j].position.x) + Math.abs(robots[i].position.y - robots[j].position.y);
      if (dist < minDist) {
        minDist = dist;
      }
    }
    totalMinDist += minDist;
  }
  return totalMinDist / count;
}

export function parseInput(rawInput) {
  const lines = rawInput.split("\n");
  const robots = lines.map(createRobot);
  return {
    robots,
    width: Math.max(...robots.map(robot => robot.position.x)) + 1,
    height: Math.max(...robots.map(robot => robot.position.y)) + 1
  };
}

export function runPartOne(input) {
  for (let i = 0; i < 100; ++i) {
    moveRobots(input);
  }
  return getSafetyFactor(input);
}

export function runPartTwo(input) {
  const max = Math.max(input.width, input.height) * 2;
  const minDists = [];
  for (let i = 0; i < max; ++i) {
    moveRobots(input);
    minDists.push({ minDist: getAverageMinDistance(input.robots), offset: i });
  }
  minDists.sort((a, b) => a.minDist - b.minDist);
  const offsets = minDists.slice(0, 4).map(minDist => minDist.offset).sort((a, b) => a - b);
  const period1 = offsets[2] - offsets[0];
  const period2 = offsets[3] - offsets[1];

  for (let i = max, max2 = input.width * input.height; i < max2; ++i) {
    moveRobots(input);
    if (i % period1 === offsets[0] && i % period2 === offsets[1]) {
      const picture = new Array(input.height);
      for (let i = 0; i < picture.length; ++i) {
        picture[i] = new Array(input.width).fill(".");
      }
      for (const robot of input.robots) {
        picture[robot.position.y][robot.position.x] = "#";
      }
      for (const row of picture) {
        console.log(row.join(""));
      }
      return i + 1;
    }
  }
  return 0;
}
