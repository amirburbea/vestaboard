import { GameData } from '../../contracts';
import { StoreState } from '../shapes';

function dataSelector<Key extends keyof GameData>(key: Key) {
  return ({ game: { data } }: StoreState) => data[key];
}

export const getColors = dataSelector('colors');
export const getEntry = ({ game: { entry } }: StoreState) => entry;
export const getGameData = ({ game: { data } }: StoreState) => data;
export const getGameId = dataSelector('id');
export const getGuesses = dataSelector('guesses');
export const getIsSolved = ({ game: { data } }: StoreState) => GameData.isSolved(data);
export const getIsOver = ({ game: { data } }: StoreState) => GameData.isOver(data);
export const getKeyColors = dataSelector('keyColors');
export const getUuid = dataSelector('uuid');
export const getWord = dataSelector('word');
