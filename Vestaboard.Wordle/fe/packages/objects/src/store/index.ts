import { combineReducers, Reducer, Store } from 'redux';
import { configureStore } from '@reduxjs/toolkit';
import createSagaMiddleware from 'redux-saga';
import * as reducers from './reducers';
import rootSaga from './sagas';
import { StoreState } from './shapes';

export function createDataStore(): Store<StoreState> {
  const sagaMiddleware = createSagaMiddleware();
  const store = configureStore({
    reducer: combineReducers(reducers) as unknown as Reducer<StoreState>,
    middleware: getDefaultMiddleware => getDefaultMiddleware().concat([sagaMiddleware]),
  });
  sagaMiddleware.run(rootSaga);
  return store;
}

export * as actions from './actions';
export * as selectors from './selectors';
export * from './shapes';
