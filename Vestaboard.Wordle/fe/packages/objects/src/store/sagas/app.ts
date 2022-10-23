import { select, takeEvery } from 'redux-saga/effects';
import { APP_REGISTER_COMPLETED_GAME } from '../actionTypes/app';
import * as selectors from '../selectors';

const {
  app: { getGamesPlayed, getGamesWon },
} = selectors;

export default function* () {
  yield takeEvery(APP_REGISTER_COMPLETED_GAME, writeDataToLocalStorage);
}

function* writeDataToLocalStorage() {
  const won: number = yield select(getGamesWon);
  const played: number = yield select(getGamesPlayed);
  localStorage.setItem('won', String(won));
  localStorage.setItem('played', String(played));
}
