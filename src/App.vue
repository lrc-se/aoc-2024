<script lang="ts">
import { nextTick, reactive, ref, useTemplateRef } from "vue";
import OutputContainer, { type OutputLine } from "./components/OutputContainer.vue";

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

export default {
  components: { OutputContainer },
  setup() {
    let curEntries: Entry[] = reactive([]);
    const isBusy = ref(false);
    const outputLines: OutputLine[] = reactive([{ type: "formatted", text: "<strong>[System online]</strong> Enter <strong>help</strong> for help." }]);
    const outputRef = useTemplateRef<InstanceType<typeof OutputContainer>>("output");

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

    function runCommand(command: string) {
      addOutput({ type: "prompt", text: command });
      const [cmd, ...args] = command.trim().toLowerCase().split(/\s+/);
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
      const entry = curEntries[num];
      if (!entry) {
        outputError("Invalid entry");
        return;
      }
      if (Number.isNaN(part)) {
        outputError("Invalid part");
        return;
      }
      if (test != null && !entry.tests.includes(test)) {
        outputError("Invalid test input");
        return;
      }
      outputText(`Running part ${part} for entry '${entry.name}' (${entry.language}) using ${test ? `test input ${test}` : "full input"}...`);
      const result = await callApi<EntryResult>(`/entries/${entry.name}?part=part${part}&test=${test}`);
      if (result) {
        outputText(result.output);
      }
    }

    function focusInput() {
      outputRef.value?.focusInput();
    }

    return {
      isBusy,
      outputLines,
      screenRef: outputRef,
      runCommand,
      focusInput
    };
  }
};
</script>

<template>
  <div class="screen" @click="focusInput">
    <h1 class="title">Advent of Code 2024</h1>
    <OutputContainer ref="output" :lines="outputLines" :busy="isBusy" @command="runCommand" />
  </div>
</template>

<style>
.screen {
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
</style>
