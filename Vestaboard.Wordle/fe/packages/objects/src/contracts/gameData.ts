import { Color, ColorRow } from './color';
import { Dictionary } from './dictionary';

/** JSON format for Game data. */
export interface GameData {
  colors: readonly ColorRow[];
  guesses: readonly string[];
  hardMode: boolean;
  id: number;
  keyColors: Dictionary<Color>;
  uuid: string;
  word?: string;
}

export namespace GameData {
  /** Checks if the game is over (meaning it is solved or 6 guesses have been made). */
  export function isOver({ colors, guesses }: Pick<GameData, 'colors' | 'guesses'>) {
    return guesses.length === 6 || isSolved({ colors });
  }

  /** Checks if the game is solved (meaning the last row of colors is all green). */
  export function isSolved({ colors }: Pick<GameData, 'colors'>) {
    return !!colors.length && colors.at(-1)!.every(c => c === Color.green);
  }
}
