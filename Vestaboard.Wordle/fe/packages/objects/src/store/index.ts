import { combineReducers, Middleware } from 'redux';
import { configureStore } from '@reduxjs/toolkit';
import createSagaMiddleware from 'redux-saga';
import * as reducers from './reducers';
import rootSaga from './sagas';

export function createDataStore() {
  const sagaMiddleware = createSagaMiddleware();
  const middleware = [sagaMiddleware as Middleware];
  const store = configureStore({
    reducer: combineReducers(reducers),
    middleware: getDefaultMiddleware => getDefaultMiddleware().concat(middleware),
  });
  sagaMiddleware.run(rootSaga);
  return store;
}

export * as actions from './actions';
export * as selectors from './selectors';
export * from './shapes';
