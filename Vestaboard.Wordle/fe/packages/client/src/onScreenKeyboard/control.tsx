import { Button, ButtonGroup, HotkeyConfig, HotkeysTarget2 } from '@blueprintjs/core';
import { IconNames } from '@blueprintjs/icons';
import { Color, Dictionary } from '@vestaboard-wordle/objects';
import { FunctionComponent, useMemo } from 'react';
import getIntent from '../getIntent';
import * as styles from './styles.scss';

export type OnScreenKeyboardProps = {
  entryLength: number;
  isGameOver: boolean;
  keyColors: Dictionary<Color>;
  pressBackspace: () => void;
  pressEnterKey: () => void;
  pressKey: (char: string) => void;
};

export const control: FunctionComponent<OnScreenKeyboardProps> = ({
  entryLength,
  keyColors,
  isGameOver,
  pressBackspace,
  pressEnterKey,
  pressKey,
}) => {
  /** The keys that can be pressed at the current state. */
  const pressable = useMemo((): Keys => {
    if (isGameOver) {
      return Keys.none;
    }
    return (entryLength && Keys.backspace) | (entryLength === 5 ? Keys.enter : Keys.key);
  }, [isGameOver, entryLength]);

  const hotKeysConfig = useMemo(
    (): HotkeyConfig[] => [
      {
        combo: 'enter',
        global: true,
        label: 'Enter',
        onKeyDown: hasFlag(pressable, Keys.enter) ? pressEnterKey : undefined,
      },
      {
        combo: 'backspace',
        global: true,
        label: 'Backspace',
        onKeyDown: hasFlag(pressable, Keys.backspace) ? pressBackspace : undefined,
      },
      ...Array.from('ABCDEFGHIJKLMNOPQRSTUVWXYZ').map(
        (key): HotkeyConfig => ({
          combo: key,
          global: true,
          label: key,
          onKeyDown: hasFlag(pressable, Keys.key) ? () => pressKey(key) : undefined,
        }),
      ),
    ],
    [pressable, pressKey, pressBackspace, pressEnterKey],
  );

  return (
    <HotkeysTarget2 hotkeys={hotKeysConfig}>
      <div className={styles.container}>
        <ButtonGroup className={styles.row} large>
          <KeyboardRow
            row={firstRow}
            disabled={!hasFlag(pressable, Keys.key)}
            keyColors={keyColors}
            pressKey={pressKey}
          />
          <Button
            disabled={!hasFlag(pressable, Keys.backspace)}
            outlined
            icon={IconNames.KEY_BACKSPACE}
            onClick={pressBackspace}
          />
        </ButtonGroup>
        <ButtonGroup className={styles.row} large>
          <KeyboardRow
            row={secondRow}
            disabled={!hasFlag(pressable, Keys.key)}
            keyColors={keyColors}
            pressKey={pressKey}
          />
        </ButtonGroup>
        <ButtonGroup className={styles.row} large>
          <KeyboardRow
            row={thirdRow}
            disabled={!hasFlag(pressable, Keys.key)}
            keyColors={keyColors}
            pressKey={pressKey}
          />
          <Button
            disabled={!hasFlag(pressable, Keys.enter)}
            outlined
            icon={IconNames.KEY_ENTER}
            onClick={pressEnterKey}
          />
        </ButtonGroup>
      </div>
    </HotkeysTarget2>
  );
};

const firstRow = Array.from('QWERTYUIOP');
const secondRow = Array.from('ASDFGHJKL');
const thirdRow = Array.from('ZXCVBNM');

interface KeyboardRowProps {
  row: readonly string[];
  disabled: boolean;
  keyColors: Dictionary<Color>;
  pressKey: (char: string) => void;
}

const KeyboardRow: FunctionComponent<KeyboardRowProps> = ({
  row,
  disabled,
  keyColors,
  pressKey,
}) => (
  <>
    {row.map(char => (
      <Button
        key={char}
        text={char}
        onClick={() => pressKey(char)}
        intent={getIntent(keyColors[char])}
        outlined={!(char in keyColors)}
        disabled={disabled}
      />
    ))}
  </>
);

const enum Keys {
  none = 0,
  key = 1,
  backspace = 2,
  enter = 4,
}

const hasFlag = <T extends number>(value: T, flag: T) => (value & flag) === flag;
