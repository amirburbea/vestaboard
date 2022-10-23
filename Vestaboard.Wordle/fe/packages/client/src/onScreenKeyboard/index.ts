import { actions, selectors, StoreState } from '@vestaboard-wordle/objects';
import { connect, MapStateToProps } from 'react-redux';
import { createSelector } from 'reselect';
import { control, OnScreenKeyboardProps } from './control';

type StateProps = Pick<OnScreenKeyboardProps, 'entryLength' | 'isGameOver' | 'keyColors'>;
type OwnProps = {};
type DispatchProps = Omit<OnScreenKeyboardProps, keyof StateProps>;

const {
  game: { submitGuess, pressChar: pressKey, pressBackspace },
} = actions;

const {
  game: { getEntry, getIsOver, getKeyColors },
} = selectors;

const mapStateToProps: MapStateToProps<StateProps, OwnProps, StoreState> = createSelector(
  getEntry,
  getIsOver,
  getKeyColors,
  ({ length: entryLength }, isGameOver, keyColors): StateProps => ({
    keyColors,
    isGameOver,
    entryLength,
  })
);

const dispatchProps: DispatchProps = {
  pressBackspace,
  pressEnterKey: submitGuess,
  pressKey,
};

export const OnScreenKeyboard: React.ComponentType<OwnProps> = connect(
  mapStateToProps,
  dispatchProps
)(control);
