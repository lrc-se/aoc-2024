<script setup lang="ts">
import { nextTick, onMounted, reactive, ref, useTemplateRef, watch } from "vue";

interface Entry {
  name: string;
  language: string;
  tests: number[];
}

interface EntryResult {
  language: string;
  output: string[];
}

interface ApiError {
  error: string;
}

interface OutputLine {
  type: "prompt" | "formatted" | "output" | "error";
  text: string;
}

let curEntries: Entry[] = reactive([]);
const curCmd = ref("");
const cmdHistory: string[] = [];
const historyIndex = ref(0);
const isBusy = ref(false);
const outputLines: OutputLine[] = reactive([{ type: "formatted", text: "<strong>[System online]</strong> Enter <strong>help</strong> for help." }]);

const outputRef = useTemplateRef("output");
const inputRef = useTemplateRef("input");

function handleInputKey(e: KeyboardEvent) {
  switch (e.key) {
    case "Enter":
      runCommand();
      break;
    case "ArrowUp":
      showHistory(historyIndex.value - 1);
      break;
    case "ArrowDown":
      showHistory(historyIndex.value + 1);
      break;
    case "Escape":
      curCmd.value = "";
      break;
    default:
      return;
  }
  e.preventDefault();
}

function focusInput() {
  inputRef.value?.focus();
}

function showHistory(index: number) {
  if (index < 0) {
    return;
  } else if (index >= cmdHistory.length) {
    curCmd.value = "";
    historyIndex.value = cmdHistory.length;
  } else {
    curCmd.value = cmdHistory[index];
    historyIndex.value = index;
  }
}

function runCommand() {
  if (!curCmd.value) {
    return;
  }
  const [cmd, ...args] = curCmd.value.trim().toLowerCase().split(/\s+/);
  addOutput({ type: "prompt", text: curCmd.value });
  cmdHistory.push(curCmd.value);
  historyIndex.value = cmdHistory.length;
  curCmd.value = "";
  switch (cmd) {
    case "help":
      addOutput([
        { type: "output", text: "Commands:" },
        { type: "formatted", text: "  <strong>help</strong>                   Display this help" },
        { type: "formatted", text: "  <strong>clear</strong>                  Clear screen" },
        { type: "formatted", text: "  <strong>load</strong>                   Load entries" },
        { type: "formatted", text: "  <strong>list</strong>                   List loaded entries" },
        { type: "formatted", text: "  <strong>test entry part [test]</strong> Test specified part for specified entry using specified test input (<strong>1</strong> if omitted)" },
        { type: "formatted", text: "  <strong>run entry part</strong>         Run specified part for specified entry" },
        { type: "output", text: " " },
        { type: "output", text: "Keys:" },
        { type: "formatted", text: "  <strong>↑</strong>                      Navigate back through command history" },
        { type: "formatted", text: "  <strong>↓</strong>                      Navigate forward through command history" },
        { type: "formatted", text: "  <strong>Esc</strong>                    Clear current line" }
      ]);
      break;
    case "clear":
      outputLines.length = 0;
      break;
    case "load":
      loadEntries();
      break;
    case "list":
      listEntries();
      break;
    case "test":
      if (args.length > 3) {
        outputError(`Unrecognized arguments: '${args.slice(3).join("', '")}'`);
        return;
      }
      runEntry(Number(args[0]) - 1, Number(args[1]), args[2] ? Number(args[2]) : 1);
      break;
    case "run":
      if (args.length > 2) {
        outputError(`Unrecognized arguments: '${args.slice(2).join("', '")}'`);
        return;
      }
      runEntry(Number(args[0]) - 1, Number(args[1]));
      break;
    default:
      outputError(`Unknown command: '${cmd}'`);
  }
}

function addOutput(lines: OutputLine | OutputLine[]) {
  if (Array.isArray(lines)) {
    outputLines.push(...lines);
  } else {
    outputLines.push(lines);
  }
}

function outputText(lines: string | string[]) {
  if (Array.isArray(lines)) {
    addOutput(lines.map(line => ({ type: "output", text: line })));
  } else {
    addOutput({ type: "output", text: lines });
  }
}

function outputError(error: string) {
  addOutput({ type: "error", text: error });
}

async function callApi<T>(route: string) {
  isBusy.value = true;
  try {
    const response = await fetch(`${API_HOST}${route}`);
    if (response.ok) {
      return await response.json() as T;
    } else {
      const error: ApiError = await response.json();
      outputError(error.error);
    }
  } catch (err) {
    outputError("Network error");
  } finally {
    isBusy.value = false;
    nextTick(focusInput);
  }
  return null;
}

async function loadEntries() {
  const entries = await callApi<Entry[]>("/entries");
  curEntries = entries ?? [];
  if (entries) {
    if (entries.length) {
      listEntries();
    } else {
      outputText("No entries found");
    }
  }
}

function listEntries() {
  if (curEntries.length) {
    addOutput(curEntries.map((entry, i) => ({
      type: "formatted",
      text: `<strong>#${i + 1}:</strong> ${entry.name} (${entry.language}) [<strong>tests:</strong> ${entry.tests.join(", ")}]`
    })));
  } else {
    outputText("No entries loaded");
  }
}

async function runEntry(num: number, part: number, test?: number) {
  if (!curEntries.length) {
    outputError("No entries loaded");
    return;
  }
  if (num == null) {
    outputError("No entry specified");
    return;
  }
  if (Number.isNaN(num) || !curEntries[num]) {
    outputError("Invalid entry");
    return;
  }
  if (Number.isNaN(part)) {
    outputError("Invalid part");
    return;
  }
  if (test != null && (Number.isNaN(test) || test < 1)) {
    outputError("Invalid test input");
    return;
  }
  const entry = curEntries[num];
  outputText(`Running part ${part} for entry '${entry.name}' using ${test ? `test input ${test}` : "full input"}...`);
  const result = await callApi<EntryResult>(`/entries/${entry.name}?part=part${part}&test=${test}`);
  if (result) {
    outputText(result.output);
  }
}

onMounted(focusInput);

watch(outputLines, () => {
  const el = outputRef.value;
  if (el) {
    nextTick(() => {
      el.scrollTop = el.scrollHeight;
    });
  }
});
</script>

<template>
  <div class="screen" @click="focusInput()">
    <h1 class="title">Advent of Code 2024</h1>
    <output ref="output" class="output-container">
      <div v-for="(line, i) in outputLines" :key="i" :class="line.type">
        <template v-if="line.type === 'prompt'">
          <span class="prefix">&gt;</span>
          {{ line.text }}
        </template>
        <template v-else-if="line.type === 'formatted'">
          <span v-html="line.text"></span>
        </template>
        <template v-else-if="line.type === 'error'">
          <span class="prefix">ERROR:</span>
          {{ line.text }}
        </template>
        <template v-else>
          {{ line.text }}
        </template>
      </div>
      <div v-if="isBusy" class="spinner">
        <span>/</span>
        <span>-</span>
        <span>\</span>
        <span>|</span>
      </div>
      <div v-else class="input-container">
        <span class="prefix">&gt;</span>
        <input ref="input" v-model="curCmd" class="input" type="text" @keydown="handleInputKey">
      </div>
    </output>
  </div>
</template>

<style>
.screen {
  --color-hi: #0f0;
  --color-mid: #0c0;
  --color-lo: #090;
  --color-bg: #000;
  --bold-weight: 700;

  background-color: var(--color-bg);
  bottom: 0;
  color: var(--color-mid);
  font-family: monospace;
  left: 0;
  padding: 1rem;
  position: absolute;
  right: 0;
  text-align: left;
  top: 0;
  width: 100%;
}

.title {
  background-color: var(--color-bg);
  font-size: 1rem;
  left: 50%;
  margin: 0;
  padding: 0 1ch;
  position: absolute;
  top: 1rem;
  transform: translate(-50%, -50%);
  white-space: nowrap;
}

.output-container {
  border: 1px solid var(--color-mid);
  display: block;
  height: 100%;
  overflow: scroll;
  padding: .75rem 1rem;
  scrollbar-width: none;
  white-space: pre-wrap;
}

.formatted strong {
  color: var(--color-mid);
  font-weight: var(--bold-weight);
}

.formatted,
.output {
  color: var(--color-lo);
}

.error {
  color: var(--color-hi);
}

.error > .prefix {
  font-weight: var(--bold-weight);
}

.input-container {
  display: flex;
}

.input-container > .prefix {
  color: var(--color-hi);
  font-weight: var(--bold-weight);
}

.input {
  appearance: none;
  background: none;
  border: none;
  caret-color: var(--color-hi);
  color: inherit;
  flex-grow: 1;
  font: inherit;
  margin-left: 1ch;
  padding: 0;
}

.input:focus {
  outline: none;
}

.input::selection {
  background-color: var(--color-mid);
  color: var(--color-bg);
}

.spinner {
  color: var(--color-hi);
  font-weight: var(--bold-weight);
  position: relative;
}

.spinner > span {
  left: 0;
  opacity: 0;
  position: absolute;
  top: 0;
}

.spinner > span:first-child {
  animation: spin1 .3s step-end infinite;
}

.spinner > span:nth-child(2) {
  animation: spin2 .3s step-end infinite;
}

.spinner > span:nth-child(3) {
  animation: spin3 .3s step-end infinite;
}

.spinner > span:last-child {
  animation: spin4 .3s step-end infinite;
}

@keyframes spin1 {
  0% { opacity: 1; }
  25% { opacity: 0; }
}

@keyframes spin2 {
  25% { opacity: 1; }
  50% { opacity: 0; }
}

@keyframes spin3 {
  50% { opacity: 1; }
  75% { opacity: 0; }
}

@keyframes spin4 {
  75% { opacity: 1; }
  100% { opacity: 0; }
}
</style>
