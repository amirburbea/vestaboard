import { HotkeysProvider } from '@blueprintjs/core';
import { createDataStore } from '@vestaboard-wordle/objects';
import { createRoot } from 'react-dom/client';
import { Provider } from 'react-redux';
import { App } from './app';
import './styles/main.scss';

const root = createRoot(document.getElementById('root')!);

const store = createDataStore();
root.render(
  <Provider store={store}>
    <HotkeysProvider>
      <App />
    </HotkeysProvider>
  </Provider>
);
