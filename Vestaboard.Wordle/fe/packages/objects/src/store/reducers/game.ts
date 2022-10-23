import { handleActions } from 'redux-actions';
import type { pressChar, setData, setEntry, setSolution } from '../actions/game';
import {
  GAME_PRESS_BACKSPACE,
  GAME_PRESS_CHAR,
  GAME_SET_DATA,
  GAME_SET_ENTRY,
  GAME_SET_SOLUTION,
} from '../actionTypes/game';
import { GameState } from '../shapes';

export default handleActions<GameState, any>(
  {
    [GAME_SET_DATA]: (state, { payload: data }: ReturnType<typeof setData>) => ({
      ...state,
      data,
      entry: '',
    }),
    [GAME_SET_ENTRY]: (state, { payload: entry }: ReturnType<typeof setEntry>) => ({
      ...state,
      entry,
    }),
    [GAME_SET_SOLUTION]: (
      { data, ...rest },
      { payload: word }: ReturnType<typeof setSolution>
    ) => ({
      ...rest,
      data: { ...data, word },
      entry: '',
    }),
    [GAME_PRESS_CHAR]: (
      { entry, ...rest },
      { payload: char }: ReturnType<typeof pressChar>
    ) => ({
      ...rest,
      entry: `${entry}${char}`,
    }),
    [GAME_PRESS_BACKSPACE]: ({ entry, ...rest }) => ({
      ...rest,
      entry: entry.substring(0, entry.length - 1),
    }),
  },
  {
    data: {
      id: -1,
      colors: [],
      guesses: [],
      keyColors: Object.create(null),
      hardMode: false,
      uuid: '',
    },
    entry: '',
  }
);
