import { composeWithDevToolsLogOnlyInProduction } from '@redux-devtools/extension';
import {
  applyMiddleware,
  combineReducers,
  legacy_createStore as createStore,
  Middleware,
  Store,
} from 'redux';
import createSagaMiddleware from 'redux-saga';
import * as reducers from './reducers';
import rootSaga from './sagas';
import { StoreState } from './shapes';

export function createDataStore() {
  const sagaMiddleware = createSagaMiddleware();
  const composeEnhancers = composeWithDevToolsLogOnlyInProduction({});
  const middleware = [sagaMiddleware as Middleware];
  const store: Store<StoreState> = createStore(
    combineReducers(reducers),
    composeEnhancers(applyMiddleware<any, StoreState>(...middleware))
  );
  sagaMiddleware.run(rootSaga);
  return store;
}

export * as actions from './actions';
export * as selectors from './selectors';
export * from './shapes';
