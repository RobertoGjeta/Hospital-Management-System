import axios from 'axios';

const TOKEN_KEY = 'ivf_auth_token';

export const getStoredToken = () => sessionStorage.getItem(TOKEN_KEY);
export const setStoredToken = (token) => {
  if (token) sessionStorage.setItem(TOKEN_KEY, token);
  else sessionStorage.removeItem(TOKEN_KEY);
};

const axiosClient = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL || '/api',
  headers: { 'Content-Type': 'application/json' },
});

axiosClient.interceptors.request.use((config) => {
  const token = getStoredToken();
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

axiosClient.interceptors.response.use(
  (response) => response,
  (error) => {
    const status = error.response?.status;
    const message =
      error.response?.data?.title ||
      error.response?.data?.message ||
      (typeof error.response?.data === 'string' ? error.response.data : null) ||
      error.message ||
      'Request failed';

    const validation = error.response?.data?.errors
      ? Object.entries(error.response.data.errors)
          .map(([field, msgs]) => `${field}: ${Array.isArray(msgs) ? msgs.join(', ') : msgs}`)
          .join(' | ')
      : null;

    if (status === 401 && !error.config?.url?.includes('/Auth/login')) {
      setStoredToken(null);
      sessionStorage.removeItem('ivf_auth_user');
      if (window.location.pathname !== '/login') {
        window.location.href = '/login';
      }
    }

    return Promise.reject({
      status,
      message: validation ? `${message} — ${validation}` : message,
      raw: error,
    });
  }
);

export default axiosClient;
