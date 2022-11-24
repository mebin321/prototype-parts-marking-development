import React from 'react';
import ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { applyMiddleware, compose, createStore } from 'redux';
import thunk from 'redux-thunk';

import { extractErrorDetails } from './api/utilities';
import App from './containers/App';
import reportWebVitals from './reportWebVitals';
import { preloadStateFromLocalStorage, reducers } from './store';
import { toastDistinctError } from './utilities/toast';

import 'react-toastify/dist/ReactToastify.min.css';
import 'semantic-ui-css/semantic.min.css';
import './styles.css';

const devToolsCompose = process.env.NODE_ENV === 'development'
  ? Object.getOwnPropertyDescriptor(window, '__REDUX_DEVTOOLS_EXTENSION_COMPOSE__')?.value
  : null;
const composeEnhancers = devToolsCompose || compose;
export const store = createStore(
  reducers,
  preloadStateFromLocalStorage(),
  composeEnhancers(applyMiddleware(thunk))
);

// global async calls error handler (called when promise rejection is not handled)
window.addEventListener('unhandledrejection', event =>
{
  console.log(event.reason);
  toastDistinctError(extractErrorDetails(event.reason));
});

ReactDOM.render(
  <Provider store={store}>
    <React.StrictMode>
      <App />
    </React.StrictMode>
  </Provider>,
  document.getElementById('root')
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
