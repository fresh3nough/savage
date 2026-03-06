import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { ThemeProvider, CssBaseline } from '@mui/material';
import cyberpunkTheme from './theme/cyberpunkTheme';
import { AuthProvider } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Layout from './components/Layout';
import Login from './pages/Login';
import Dashboard from './pages/Dashboard';
import InventoryPage from './pages/admin/InventoryPage';
import VendorsPage from './pages/admin/VendorsPage';
import LocationsPage from './pages/admin/LocationsPage';
import ShipmentsPage from './pages/admin/ShipmentsPage';

/**
 * Root application component. Provides theme, auth context, and routing.
 * Admin routes are restricted via requiredRole prop on ProtectedRoute.
 */
function App() {
  return (
    <ThemeProvider theme={cyberpunkTheme}>
      <CssBaseline />
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            <Route path="/login" element={<Login />} />
            <Route element={<ProtectedRoute><Layout /></ProtectedRoute>}>
              <Route path="/dashboard" element={<Dashboard />} />
              <Route path="/inventory" element={<InventoryPage />} />
              <Route path="/vendors" element={<ProtectedRoute requiredRole="Admin"><VendorsPage /></ProtectedRoute>} />
              <Route path="/locations" element={<ProtectedRoute requiredRole="Admin"><LocationsPage /></ProtectedRoute>} />
              <Route path="/shipments" element={<ShipmentsPage />} />
            </Route>
            <Route path="*" element={<Navigate to="/login" replace />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </ThemeProvider>
  );
}

export default App;
