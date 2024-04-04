import { Action } from 'redux-actions';
import { call, put, select, takeEvery } from 'redux-saga/effects';
import { Color, ColorRow, GameData } from '../../contracts';
import * as actions from '../actions';
import {
  GAME_REFRESH_DATA,
  GAME_START_NEW,
  GAME_SUBMIT_GUESS,
} from '../actionTypes/game';
import * as selectors from '../selectors';
import fetchData from './fetchData';

const {
  app: { registerCompletedGame },
  game: { setData, setEntry, setSolution },
} = actions;

const {
  game: { getEntry, getGameData },
} = selectors;

export default function* () {
  yield call(refreshGameData);
  yield takeEvery(GAME_SUBMIT_GUESS, submitGuess);
  yield takeEvery(GAME_REFRESH_DATA, refreshGameData);
  yield takeEvery(GAME_START_NEW, startNewGame);
}

function combineColors(x: Color, y?: Color): Color {
  return Math.max(x, y ?? Color.none);
}

function* refreshGameData() {
  const data: GameData = yield call(fetchData);
  if (!data.word && GameData.isOver(data)) {
    data.word = GameData.isSolved(data)
      ? data.guesses.at(-1)
      : yield call(requestSolution, data.uuid);
  }
  yield put(setData(data));
}

function* requestSolution(uuid: string) {
  const word: string = yield call(fetchData, `/${uuid}/solution`);
  return word;
}

function* startNewGame({ payload: hardMode }: Action<boolean>) {
  try {
    const data: GameData = yield call(fetchData, '/new', hardMode);
    yield put(setData(data));
  } catch {}
}

function* submitGuess() {
  const data: GameData = yield select(getGameData);
  if (!data.uuid) {
    return;
  }
  const guess: string = yield select(getEntry);
  let answer: ColorRow;
  try {
    answer = yield call(fetchData, `/${data.uuid}/guess`, guess);
  } catch {
    yield put(setEntry(''));
    return;
  }
  const nextKeyColors = { ...data.keyColors };
  for (let index = 0; index < 5; index++) {
    const char = guess.charAt(index);
    nextKeyColors[char] = combineColors(answer[index], nextKeyColors[char]);
  }
  const nextColors = [...data.colors, answer];
  const next: GameData = {
    ...data,
    colors: nextColors,
    guesses: [...data.guesses, guess],
    keyColors: nextKeyColors,
  };
  let isSolved = answer.every(color => color === Color.green);
  if (isSolved) {
    next.word = guess;
  }
  let isOver = isSolved || next.guesses.length === 6;
  yield put(setData(next));
  if (!isOver) {
    return;
  }
  if (!next.word) {
    const word: string = yield call(requestSolution, next.uuid);
    yield put(setSolution(word));
  }
  yield put(registerCompletedGame(isSolved));
}
