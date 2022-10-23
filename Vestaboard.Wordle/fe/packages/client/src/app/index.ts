import { actions, selectors, StoreState } from '@vestaboard-wordle/objects';
import type { ComponentType } from 'react';
import { connect, MapStateToProps } from 'react-redux';
import { createSelector } from 'reselect';
import { AppProps, control } from './control';

type StateProps = Pick<
  AppProps,
  | 'error'
  | 'gamesPlayed'
  | 'gamesWon'
  | 'guessCount'
  | 'id'
  | 'isLoading'
  | 'isSolved'
  | 'word'
>;
type OwnProps = {};
type DispatchProps = Omit<AppProps, keyof StateProps>;

const {
  game: { refreshData, startNewGame },
  app: { requestRender, setError },
} = actions;

const {
  app: { getIsLoading, getError, getGamesPlayed, getGamesWon },
  game: { getGameId, getGuesses, getIsSolved, getWord },
} = selectors;

const mapStateToProps: MapStateToProps<StateProps, OwnProps, StoreState> = createSelector(
  getError,
  getGameId,
  getIsLoading,
  getIsSolved,
  getWord,
  getGamesPlayed,
  getGamesWon,
  getGuesses,
  (error, id, isLoading, isSolved, word, gamesPlayed, gamesWon, guesses): StateProps => ({
    error,
    id,
    isLoading,
    isSolved,
    word,
    gamesPlayed,
    gamesWon,
    guessCount: guesses.length,
  })
);

const dispatchProps: DispatchProps = {
  clearError: () => setError(''),
  requestRender,
  startNewGame,
  refreshData,
};

export const App: ComponentType<OwnProps> = connect(
  mapStateToProps,
  dispatchProps
)(control);
