import {
  Alignment,
  Button,
  Classes,
  Dialog,
  Intent,
  Navbar,
  NavbarDivider,
  NavbarGroup,
  NavbarHeading,
  Position,
  Toaster,
} from '@blueprintjs/core';
import { IconNames } from '@blueprintjs/icons';
import classNames from 'classnames';
import { FunctionComponent, useEffect, useRef, useState } from 'react';
import { Board } from '../board';
import { OnScreenKeyboard } from '../onScreenKeyboard';
import styles from './styles.scss';

export interface AppProps {
  error?: string;
  gamesPlayed: number;
  gamesWon: number;
  guessCount: number;
  id: number;
  isLoading: boolean;
  isSolved: boolean;
  word?: string;
  clearError: () => void;
  refreshData: () => void;
  requestRender: () => void;
  startNewGame: (hardMode: boolean) => void;
}

export const control: FunctionComponent<AppProps> = ({
  error,
  gamesPlayed,
  gamesWon,
  guessCount,
  id,
  isLoading,
  isSolved,
  word,
  clearError,
  refreshData,
  requestRender,
  startNewGame,
}) => {
  const toaster = useRef<Toaster>(null);
  const [dismissedDialog, setDismissedDialog] = useState(false);

  useEffect(() => {
    if (error) {
      toaster.current?.show({
        intent: Intent.DANGER,
        message: error,
        onDismiss: clearError,
      });
    }
  }, [error]);

  useEffect(() => {
    if (word) {
      setDismissedDialog(false);
    }
  }, [word]);

  return (
    <div className={classNames(styles.container, Classes.DARK)}>
      <Navbar>
        <NavbarGroup>
          <NavbarHeading>Vestaboard Wordle</NavbarHeading>
          <NavbarDivider />
          <Button
            minimal
            disabled={isLoading}
            text="New"
            icon={IconNames.NEW_OBJECT}
            onClick={() => startNewGame(false)}
            onMouseUp={e => (e.target as HTMLButtonElement).blur()}
          />
          <Button minimal text="Refresh" icon={IconNames.REFRESH} onClick={refreshData} />
        </NavbarGroup>
        <NavbarGroup align={Alignment.RIGHT}>
          <span>Game #{id}</span>
        </NavbarGroup>
      </Navbar>
      <Board />
      <OnScreenKeyboard />
      <Toaster position={Position.TOP_RIGHT} ref={toaster} />
      <Dialog
        className={Classes.DARK}
        isOpen={!!word && !dismissedDialog}
        title={`You ${isSolved ? 'Win' : 'Lose'}`}
        icon={IconNames.CloudDownload}
        onClose={() => setDismissedDialog(true)}
      >
        <div className={Classes.DIALOG_BODY}>
          <p>
            The word was <b>{word}</b>. &nbsp;
            {isSolved ? `You guessed it in ${guessCount}.` : null}
          </p>
          <p>
            {gamesPlayed === 1
              ? `You have played one game, which you ${gamesWon === 1 ? 'won' : 'lost'}.`
              : `You have won ${gamesWon} out of ${gamesPlayed} games played.`}
          </p>
        </div>
        <div className={Classes.DIALOG_FOOTER}>
          <div className={Classes.DIALOG_FOOTER_ACTIONS}>
            <Button
              onClick={() => startNewGame(false)}
              icon={IconNames.NEW_OBJECT}
              text="New"
            />
          </div>
        </div>
      </Dialog>
    </div>
  );
};
