import { AppState, StoreState } from '../shapes';

function keySelector<Key extends keyof AppState>(key: Key) {
  return ({ app }: StoreState) => app[key];
}

export const getError = keySelector('error');
export const getIsLoading = keySelector('isLoading');
export const getGamesPlayed = keySelector('played');
export const getGamesWon = keySelector('won');
