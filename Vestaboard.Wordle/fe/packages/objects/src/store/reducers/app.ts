import { handleActions } from 'redux-actions';
import type { registerCompletedGame, setError, setIsLoading } from '../actions/app';
import {
  APP_REGISTER_COMPLETED_GAME,
  APP_SET_ERROR,
  APP_SET_IS_LOADING
} from '../actionTypes/app';
import { AppState } from '../shapes';

export default handleActions<AppState, any>(
  {
    [APP_SET_IS_LOADING]: (
      state,
      { payload: isLoading }: ReturnType<typeof setIsLoading>
    ) => ({
      ...state,
      isLoading,
    }),
    [APP_REGISTER_COMPLETED_GAME]: (
      { played, won, ...rest },
      { payload: victory }: ReturnType<typeof registerCompletedGame>
    ) => ({
      ...rest,
      played: played + 1,
      won: won + (victory ? 1 : 0),
    }),
    [APP_SET_ERROR]: (state, { payload: error }: ReturnType<typeof setError>) => ({
      ...state,
      error,
    }),
  },
  readInitialStateFromLocalStorage()
);

function readInitialStateFromLocalStorage(): AppState {
  const played = readFromLocalStorage('played');
  return {
    isLoading: false,
    played,
    won: Math.min(played, played ? readFromLocalStorage('won') : 0),
  };

  function readFromLocalStorage(key: keyof AppState) {
    const text = localStorage.getItem(key);
    const parsed = !text ? 0 : parseInt(text);
    return isNaN(parsed) ? 0 : parsed;
  }
}
