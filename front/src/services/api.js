import axios from 'axios';

const API_BASE = process.env.REACT_APP_API_URL || 'http://localhost:5000';

/**
 * Axios instance with base URL and JSON defaults.
 * Automatically attaches JWT bearer token from localStorage on every request.
 */
const api = axios.create({
  baseURL: `${API_BASE}/api`,
  headers: { 'Content-Type': 'application/json' },
});

// Attach JWT token to every outgoing request
api.interceptors.request.use((config) => {
  const token = localStorage.getItem('savage_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

// Handle 401 responses by clearing auth state
api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      localStorage.removeItem('savage_token');
      localStorage.removeItem('savage_user');
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);

// ── In-Memory Cache with TTL ──

const cache = new Map();
const DEFAULT_TTL = 60000; // 60 seconds

/**
 * Retrieves data from cache if it exists and hasn't expired.
 * Returns null if cache miss or expired.
 */
function getCached(key) {
  const entry = cache.get(key);
  if (!entry) return null;
  if (Date.now() - entry.timestamp > entry.ttl) {
    cache.delete(key);
    return null;
  }
  return entry.data;
}

/**
 * Stores data in the cache with a TTL.
 */
function setCache(key, data, ttl = DEFAULT_TTL) {
  cache.set(key, { data, timestamp: Date.now(), ttl });
}

/**
 * Invalidates all cache entries matching a prefix.
 * Used after mutations (create/update/delete) to ensure fresh data.
 */
function invalidateCache(prefix) {
  for (const key of cache.keys()) {
    if (key.startsWith(prefix)) cache.delete(key);
  }
}

// ── Auth API ──

export const authApi = {
  login: (data) => api.post('/auth/login', data),
  register: (data) => api.post('/auth/register', data),
};

// ── Inventory API (cached GET) ──

export const inventoryApi = {
  getAll: async (params = {}) => {
    const key = `inventory:${JSON.stringify(params)}`;
    const cached = getCached(key);
    if (cached) return { data: cached };
    const res = await api.get('/inventory', { params });
    setCache(key, res.data);
    return res;
  },
  getById: (id) => api.get(`/inventory/${id}`),
  create: async (data) => {
    const res = await api.post('/inventory', data);
    invalidateCache('inventory');
    return res;
  },
  update: async (id, data) => {
    const res = await api.put(`/inventory/${id}`, data);
    invalidateCache('inventory');
    return res;
  },
  delete: async (id) => {
    const res = await api.delete(`/inventory/${id}`);
    invalidateCache('inventory');
    return res;
  },
};

// ── Vendor API (cached GET) ──

export const vendorApi = {
  getAll: async () => {
    const cached = getCached('vendors:all');
    if (cached) return { data: cached };
    const res = await api.get('/vendor');
    setCache('vendors:all', res.data);
    return res;
  },
  getById: (id) => api.get(`/vendor/${id}`),
  create: async (data) => {
    const res = await api.post('/vendor', data);
    invalidateCache('vendors');
    return res;
  },
  update: async (id, data) => {
    const res = await api.put(`/vendor/${id}`, data);
    invalidateCache('vendors');
    return res;
  },
  delete: async (id) => {
    const res = await api.delete(`/vendor/${id}`);
    invalidateCache('vendors');
    return res;
  },
};

// ── Location API (cached GET) ──

export const locationApi = {
  getAll: async () => {
    const cached = getCached('locations:all');
    if (cached) return { data: cached };
    const res = await api.get('/location');
    setCache('locations:all', res.data);
    return res;
  },
  getById: (id) => api.get(`/location/${id}`),
  create: async (data) => {
    const res = await api.post('/location', data);
    invalidateCache('locations');
    return res;
  },
  update: async (id, data) => {
    const res = await api.put(`/location/${id}`, data);
    invalidateCache('locations');
    return res;
  },
  delete: async (id) => {
    const res = await api.delete(`/location/${id}`);
    invalidateCache('locations');
    return res;
  },
};

// ── Shipment API (cached GET) ──

export const shipmentApi = {
  getAll: async () => {
    const cached = getCached('shipments:all');
    if (cached) return { data: cached };
    const res = await api.get('/shipment');
    setCache('shipments:all', res.data);
    return res;
  },
  getById: (id) => api.get(`/shipment/${id}`),
  create: async (data) => {
    const res = await api.post('/shipment', data);
    invalidateCache('shipments');
    return res;
  },
  update: async (id, data) => {
    const res = await api.put(`/shipment/${id}`, data);
    invalidateCache('shipments');
    return res;
  },
  delete: async (id) => {
    const res = await api.delete(`/shipment/${id}`);
    invalidateCache('shipments');
    return res;
  },
};

export default api;
