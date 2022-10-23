import { AppState } from './app';
import { GameState } from './game';

export interface StoreState {
  game: GameState;
  app: AppState;
}
