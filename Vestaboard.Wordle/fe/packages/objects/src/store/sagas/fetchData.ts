import { call, Effect, put } from 'redux-saga/effects';
import { setError, setIsLoading } from '../actions/app';

export default function* fetchData(
  uriSuffix: string = '',
  body?: unknown
): Generator<Effect, unknown, unknown> {
  try {
    yield put(setIsLoading(true));
    return yield call(fetchDataViaApi, uriSuffix, body);
  } catch (error: any) {
    yield put(setError((error as Error).message));
    throw error;
  } finally {
    yield put(setIsLoading(false));
  }
}

async function fetchDataViaApi(uriSuffix: string = '', body?: unknown) {
  const response = await fetch(`/api/game${uriSuffix}`, {
    method: body == null ? 'GET' : 'POST',
    ...(body != null && {
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(body),
    }),
  });
  if (response.status !== 200) {
    throw new Error(await response.text());
  }
  return response.json();
}
