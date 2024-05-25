import { Button } from '@blueprintjs/core';
import { ColorRow } from '@vestaboard-wordle/objects';
import classNames from 'classnames';
import { FunctionComponent } from 'react';
import getIntent from '../getIntent';
import * as styles from './styles.scss';

export interface BoardProps {
  colors: readonly ColorRow[];
  entry: string;
  guesses: readonly string[];
  isGameOver: boolean;
}

export const control: FunctionComponent<BoardProps> = ({
  colors,
  entry,
  guesses,
  isGameOver,
}) => (
  <div className={classNames(styles.container, { [styles['game-over']]: isGameOver })}>
    {[...Array(6).keys()].map(index => (
      <BoardRow
        key={index}
        guess={guesses[index]}
        colors={colors[index]}
        entry={guesses.length === index ? entry : undefined}
      />
    ))}
  </div>
);

interface BoardRowProps {
  colors?: ColorRow;
  entry?: string;
  guess?: string;
}

const BoardRow: FunctionComponent<BoardRowProps> = ({ guess, colors, entry }) => (
  <div className={styles.row}>
    {[...Array(5).keys()].map(index => (
      <Button
        large
        key={index}
        text={(guess ?? entry)?.charAt(index) ?? ' '}
        intent={getIntent(colors?.[index])}
        outlined={!guess}
        tabIndex={-1}
      />
    ))}
  </div>
);
