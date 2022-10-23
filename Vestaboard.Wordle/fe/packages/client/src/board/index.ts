import { StoreState, selectors } from '@vestaboard-wordle/objects';
import { connect, MapStateToProps } from 'react-redux';
import { createSelector } from 'reselect';
import { BoardProps, control } from './control';

type OwnProps = {};

const {
  game: { getGuesses, getColors, getEntry, getIsOver },
} = selectors;

const mapStateToProps: MapStateToProps<BoardProps, OwnProps, StoreState> = createSelector(
  getGuesses,
  getColors,
  getEntry,
  getIsOver,
  (guesses, colors, entry, isGameOver): BoardProps => ({
    guesses,
    colors,
    entry,
    isGameOver,
  })
);

export const Board: React.ComponentType<OwnProps> = connect(mapStateToProps)(control);
