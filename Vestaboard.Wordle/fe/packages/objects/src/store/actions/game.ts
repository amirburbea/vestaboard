import { createAction } from 'redux-actions';
import { GameData } from '../../contracts';
import {
  GAME_PRESS_BACKSPACE,
  GAME_PRESS_CHAR,
  GAME_REFRESH_DATA,
  GAME_SET_DATA,
  GAME_SET_ENTRY,
  GAME_SET_SOLUTION,
  GAME_START_NEW,
  GAME_SUBMIT_GUESS,
} from '../actionTypes/game';

export const pressBackspace = createAction(GAME_PRESS_BACKSPACE, () => {});
export const pressChar = createAction<string>(GAME_PRESS_CHAR);
export const refreshData = createAction(GAME_REFRESH_DATA, () => {});
export const setData = createAction<GameData>(GAME_SET_DATA);
export const setEntry = createAction<string>(GAME_SET_ENTRY);
export const setSolution = createAction<string>(GAME_SET_SOLUTION);
export const startNewGame = createAction<boolean>(GAME_START_NEW); // argument is hard mode.
export const submitGuess = createAction(GAME_SUBMIT_GUESS, () => {});
