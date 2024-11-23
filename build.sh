#!/bin/bash

set -e

if ! type bun; then
  echo "### Installing bun ###"
  curl -fsSL https://bun.sh/install | bash
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
