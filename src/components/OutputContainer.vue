<script setup lang="ts">
import { nextTick, onMounted, ref, useTemplateRef, watch } from "vue";

export interface OutputLine {
  type: "prompt" | "formatted" | "output" | "error";
  text: string;
}

const props = defineProps<{
  lines: OutputLine[],
  busy?: boolean
}>();

const emit = defineEmits<{
  command: [command: string]
}>();

defineExpose({ focusInput });

const curCmd = ref("");
const cmdHistory: string[] = [];
const historyIndex = ref(0);

const outputRef = useTemplateRef("output");
const inputRef = useTemplateRef("input");

function handleInputKey(e: KeyboardEvent) {
  switch (e.key) {
    case "Enter":
      if (curCmd.value) {
        emit("command", curCmd.value);
        cmdHistory.push(curCmd.value);
        historyIndex.value = cmdHistory.length;
        curCmd.value = "";
      }
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

onMounted(focusInput);

watch(props.lines, () => {
  const el = outputRef.value;
  if (el) {
    nextTick(() => {
      el.scrollTop = el.scrollHeight;
    });
  }
});
</script>

<template>
  <output ref="output" class="output-container">
    <div v-for="(line, i) in lines" :key="i" :class="line.type">
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
    <div v-if="busy" class="spinner">
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
</template>

<style scoped>
.output-container {
  border: 1px solid var(--color-mid);
  display: block;
  height: 100%;
  overflow: scroll;
  padding: .75rem 1rem;
  scrollbar-width: none;
  white-space: pre-wrap;
}

.formatted,
.output {
  color: var(--color-lo);
}

.formatted:deep(strong) {
  color: var(--color-mid);
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
  opacity: 0;
}

.spinner > span:not(:first-child) {
  left: 0;
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
