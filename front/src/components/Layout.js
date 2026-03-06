import React, { useState } from 'react';
import { Outlet, useNavigate, useLocation } from 'react-router-dom';
import {
  AppBar, Toolbar, Typography, Drawer, List, ListItemButton,
  ListItemIcon, ListItemText, Box, IconButton, Divider, Chip,
} from '@mui/material';
import {
  Dashboard, Inventory, LocalShipping, Store, LocationOn,
  Menu as MenuIcon, Logout, Person,
} from '@mui/icons-material';
import { useAuth } from '../contexts/AuthContext';

const DRAWER_WIDTH = 240;

/**
 * Main application layout with cyberpunk-styled sidebar and top bar.
 * Shows different nav items based on user role (Admin vs Vendor).
 */
function Layout() {
  const { user, logout, isAdmin } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [mobileOpen, setMobileOpen] = useState(false);

  const adminNav = [
    { text: 'Dashboard', icon: <Dashboard />, path: '/dashboard' },
    { text: 'Inventory', icon: <Inventory />, path: '/inventory' },
    { text: 'Vendors', icon: <Store />, path: '/vendors' },
    { text: 'Locations', icon: <LocationOn />, path: '/locations' },
    { text: 'Shipments', icon: <LocalShipping />, path: '/shipments' },
  ];

  const vendorNav = [
    { text: 'Dashboard', icon: <Dashboard />, path: '/dashboard' },
    { text: 'My Inventory', icon: <Inventory />, path: '/inventory' },
    { text: 'My Shipments', icon: <LocalShipping />, path: '/shipments' },
  ];

  const navItems = isAdmin ? adminNav : vendorNav;

  const handleLogout = () => {
    logout();
    navigate('/login');
  };

  const drawer = (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Box sx={{ p: 2, textAlign: 'center' }}>
        <Typography
          variant="h5"
          sx={{
            fontFamily: '"Orbitron", monospace',
            color: '#00f0ff',
            textShadow: '0 0 20px rgba(0,240,255,0.5)',
            mb: 1,
          }}
        >
          SAVAGE
        </Typography>
        <Chip
          icon={<Person />}
          label={`${user?.username} (${user?.role})`}
          size="small"
          sx={{
            borderColor: user?.role === 'Admin' ? '#ff00ff' : '#00f0ff',
            color: user?.role === 'Admin' ? '#ff00ff' : '#00f0ff',
          }}
          variant="outlined"
        />
      </Box>
      <Divider sx={{ borderColor: 'rgba(0,240,255,0.15)' }} />
      <List sx={{ flex: 1, px: 1 }}>
        {navItems.map((item) => (
          <ListItemButton
            key={item.path}
            onClick={() => { navigate(item.path); setMobileOpen(false); }}
            selected={location.pathname === item.path}
            sx={{
              borderRadius: 1,
              mb: 0.5,
              '&.Mui-selected': {
                backgroundColor: 'rgba(0,240,255,0.1)',
                borderLeft: '3px solid #00f0ff',
                '& .MuiListItemIcon-root': { color: '#00f0ff' },
              },
              '&:hover': { backgroundColor: 'rgba(0,240,255,0.05)' },
            }}
          >
            <ListItemIcon sx={{ color: 'text.secondary', minWidth: 40 }}>
              {item.icon}
            </ListItemIcon>
            <ListItemText primary={item.text} />
          </ListItemButton>
        ))}
      </List>
      <Divider sx={{ borderColor: 'rgba(0,240,255,0.15)' }} />
      <List sx={{ px: 1 }}>
        <ListItemButton onClick={handleLogout} sx={{ borderRadius: 1 }}>
          <ListItemIcon sx={{ color: '#ff3366', minWidth: 40 }}>
            <Logout />
          </ListItemIcon>
          <ListItemText primary="Logout" sx={{ '& .MuiTypography-root': { color: '#ff3366' } }} />
        </ListItemButton>
      </List>
    </Box>
  );

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <AppBar position="fixed" sx={{ zIndex: (t) => t.zIndex.drawer + 1, display: { md: 'none' } }}>
        <Toolbar>
          <IconButton color="inherit" edge="start" onClick={() => setMobileOpen(!mobileOpen)}>
            <MenuIcon />
          </IconButton>
          <Typography variant="h6" sx={{ fontFamily: '"Orbitron"', color: '#00f0ff' }}>
            SAVAGE
          </Typography>
        </Toolbar>
      </AppBar>
      <Drawer
        variant="temporary"
        open={mobileOpen}
        onClose={() => setMobileOpen(false)}
        sx={{ display: { xs: 'block', md: 'none' }, '& .MuiDrawer-paper': { width: DRAWER_WIDTH } }}
      >
        {drawer}
      </Drawer>
      <Drawer
        variant="permanent"
        sx={{ display: { xs: 'none', md: 'block' }, '& .MuiDrawer-paper': { width: DRAWER_WIDTH, boxSizing: 'border-box' } }}
      >
        {drawer}
      </Drawer>
      <Box component="main" sx={{ flexGrow: 1, p: 3, ml: { md: `${DRAWER_WIDTH}px` }, mt: { xs: 8, md: 0 } }}>
        <Outlet />
      </Box>
    </Box>
  );
}

export default Layout;
