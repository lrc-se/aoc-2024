#!/bin/bash

export part=$1
if [[ $2 == "test" ]] || [[ $2 == "test-rel" ]]; then
  echo "### TEST MODE ###"
  input="input-test$3.txt"
else
  input=input.txt
fi
time bun run aoc.js "$input"
