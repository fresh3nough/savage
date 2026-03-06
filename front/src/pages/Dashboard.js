import React, { useEffect, useState } from 'react';
import { Box, Typography, Paper, Grid, CircularProgress } from '@mui/material';
import { Inventory, Store, LocationOn, LocalShipping } from '@mui/icons-material';
import { useAuth } from '../contexts/AuthContext';
import { inventoryApi, vendorApi, locationApi, shipmentApi } from '../services/api';

/**
 * Dashboard displaying summary stat cards. Admin sees all entities,
 * vendor sees only their inventory and shipment counts.
 */
function Dashboard() {
  const { user, isAdmin } = useAuth();
  const [stats, setStats] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    async function load() {
      try {
        const [inv, ship] = await Promise.all([
          inventoryApi.getAll(),
          shipmentApi.getAll(),
        ]);
        const data = { inventoryCount: inv.data.length, shipmentCount: ship.data.length };

        if (isAdmin) {
          const [vend, loc] = await Promise.all([vendorApi.getAll(), locationApi.getAll()]);
          data.vendorCount = vend.data.length;
          data.locationCount = loc.data.length;
        }
        setStats(data);
      } catch (err) {
        console.error('Dashboard load error:', err);
      } finally {
        setLoading(false);
      }
    }
    load();
  }, [isAdmin]);

  if (loading) return <Box sx={{ display: 'flex', justifyContent: 'center', mt: 8 }}><CircularProgress /></Box>;

  const cards = [
    { label: 'Inventory Items', value: stats?.inventoryCount ?? 0, icon: <Inventory />, color: '#00f0ff' },
    { label: 'Shipments', value: stats?.shipmentCount ?? 0, icon: <LocalShipping />, color: '#ff00ff' },
  ];
  if (isAdmin) {
    cards.push({ label: 'Vendors', value: stats?.vendorCount ?? 0, icon: <Store />, color: '#00ff88' });
    cards.push({ label: 'Locations', value: stats?.locationCount ?? 0, icon: <LocationOn />, color: '#ffaa00' });
  }

  return (
    <Box>
      <Typography variant="h4" sx={{ mb: 3, color: '#00f0ff', textShadow: '0 0 15px rgba(0,240,255,0.3)' }}>
        Welcome, {user?.username}
      </Typography>
      <Grid container spacing={3}>
        {cards.map((card) => (
          <Grid item xs={12} sm={6} md={3} key={card.label}>
            <Paper sx={{
              p: 3, textAlign: 'center',
              borderTop: `3px solid ${card.color}`,
              '&:hover': { boxShadow: `0 0 25px ${card.color}33` },
              transition: 'box-shadow 0.3s ease',
            }}>
              <Box sx={{ color: card.color, mb: 1 }}>{card.icon}</Box>
              <Typography variant="h3" sx={{ fontFamily: '"Orbitron"', color: card.color }}>{card.value}</Typography>
              <Typography variant="body2" color="text.secondary">{card.label}</Typography>
            </Paper>
          </Grid>
        ))}
      </Grid>
    </Box>
  );
}

export default Dashboard;
