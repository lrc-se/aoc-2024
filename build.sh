#!/bin/bash

set -e

if ! type bun; then
  echo "### Installing bun ###"
  curl -fsSL https://bun.sh/install | bash
  export BUN_INSTALL="$HOME/.bun"
  export PATH="$BUN_INSTALL/bin:$PATH"
fi

for d in */; do
  if [ -d "$d" ] && [ -e "$d/build.sh" ]; then
    echo
    echo "### Building $d ###"
    cd $d
    ./build.sh
    cd ..
  fi
done

echo
echo "### Building frontend ###"
bun install
bun run node_modules/vite/bin/vite.js build
