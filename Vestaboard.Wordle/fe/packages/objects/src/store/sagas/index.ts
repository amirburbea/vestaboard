import { all, call } from 'redux-saga/effects';
import gameSaga from './game';
import appSaga from './app';

export default function* () {
  yield all([appSaga, gameSaga].map(call));
}
