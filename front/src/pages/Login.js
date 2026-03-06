import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  Box, TextField, Button, Typography, Paper, Alert, CircularProgress,
} from '@mui/material';
import { useAuth } from '../contexts/AuthContext';

/**
 * Login page with cyberpunk-styled card. Authenticates users via JWT.
 */
function Login() {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login(username, password);
      navigate('/dashboard');
    } catch {
      setError('Invalid credentials');
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{
      minHeight: '100vh',
      display: 'flex',
      alignItems: 'center',
      justifyContent: 'center',
      background: 'radial-gradient(ellipse at center, #12121a 0%, #0a0a0f 100%)',
    }}>
      <Paper sx={{
        p: 4, width: 400, maxWidth: '90vw',
        border: '1px solid rgba(0,240,255,0.25)',
        boxShadow: '0 0 40px rgba(0,240,255,0.1), 0 0 80px rgba(255,0,255,0.05)',
      }}>
        <Typography
          variant="h4"
          align="center"
          sx={{
            fontFamily: '"Orbitron", monospace',
            color: '#00f0ff',
            textShadow: '0 0 30px rgba(0,240,255,0.5)',
            mb: 3,
          }}
        >
          SAVAGE
        </Typography>
        <Typography variant="body2" align="center" color="text.secondary" sx={{ mb: 3 }}>
          Inventory Management System
        </Typography>
        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
        <form onSubmit={handleSubmit}>
          <TextField
            fullWidth label="Username" value={username}
            onChange={(e) => setUsername(e.target.value)}
            sx={{ mb: 2 }} required autoFocus
          />
          <TextField
            fullWidth label="Password" type="password" value={password}
            onChange={(e) => setPassword(e.target.value)}
            sx={{ mb: 3 }} required
          />
          <Button
            fullWidth type="submit" variant="contained" color="primary"
            disabled={loading} size="large"
          >
            {loading ? <CircularProgress size={24} /> : 'Sign In'}
          </Button>
        </form>
        <Typography variant="caption" display="block" align="center" color="text.secondary" sx={{ mt: 3 }}>
          Demo: admin / Admin123! | vendor1 / Vendor123!
        </Typography>
      </Paper>
    </Box>
  );
}

export default Login;
