import { createAction } from 'redux-actions';
import {
  APP_REGISTER_COMPLETED_GAME,
  APP_REQUEST_RENDER,
  APP_SET_ERROR,
  APP_SET_IS_LOADING,
} from '../actionTypes/app';

export const registerCompletedGame = createAction<boolean>(APP_REGISTER_COMPLETED_GAME);
export const requestRender = createAction(APP_REQUEST_RENDER, () => {});
export const setError = createAction<string | undefined>(APP_SET_ERROR);
export const setIsLoading = createAction<boolean>(APP_SET_IS_LOADING);
