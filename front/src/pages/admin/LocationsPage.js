import React, { useEffect, useState } from 'react';
import {
  Box, Typography, Table, TableHead, TableRow, TableCell, TableBody,
  Paper, Button, TextField, IconButton, Dialog, DialogTitle,
  DialogContent, DialogActions, Chip, CircularProgress, Alert, TableContainer,
  FormControl, InputLabel, Select, MenuItem, LinearProgress,
} from '@mui/material';
import { Add, Edit, Delete } from '@mui/icons-material';
import { locationApi } from '../../services/api';

const TYPES = ['Warehouse', 'Distribution', 'Store'];
const emptyForm = { name: '', address: '', type: 'Warehouse', capacity: 0, currentOccupancy: 0 };

/**
 * Location management page for admins. Shows occupancy bar per location.
 */
function LocationsPage() {
  const [locations, setLocations] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editId, setEditId] = useState(null);
  const [form, setForm] = useState(emptyForm);

  const loadLocations = async () => {
    try { setLoading(true); const { data } = await locationApi.getAll(); setLocations(data); }
    catch { setError('Failed to load locations'); }
    finally { setLoading(false); }
  };

  useEffect(() => { loadLocations(); }, []);

  const handleSave = async () => {
    try {
      if (editId) await locationApi.update(editId, form);
      else await locationApi.create(form);
      setDialogOpen(false); setForm(emptyForm); setEditId(null); loadLocations();
    } catch { setError('Failed to save location'); }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this location?')) return;
    try { await locationApi.delete(id); loadLocations(); }
    catch { setError('Failed to delete location'); }
  };

  const openEdit = (l) => {
    setEditId(l.id);
    setForm({ name: l.name, address: l.address, type: l.type, capacity: l.capacity, currentOccupancy: l.currentOccupancy });
    setDialogOpen(true);
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h4" sx={{ color: '#00f0ff' }}>Locations</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={() => { setEditId(null); setForm(emptyForm); setDialogOpen(true); }}>
          Add Location
        </Button>
      </Box>
      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}
      {loading ? <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}><CircularProgress /></Box> : (
        <TableContainer component={Paper}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Name</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Address</TableCell>
                <TableCell>Occupancy</TableCell>
                <TableCell align="center">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {locations.map((l) => {
                const pct = l.capacity > 0 ? (l.currentOccupancy / l.capacity) * 100 : 0;
                return (
                  <TableRow key={l.id} hover>
                    <TableCell>{l.name}</TableCell>
                    <TableCell><Chip label={l.type} size="small" variant="outlined" /></TableCell>
                    <TableCell>{l.address}</TableCell>
                    <TableCell sx={{ minWidth: 180 }}>
                      <Box sx={{ display: 'flex', alignItems: 'center', gap: 1 }}>
                        <LinearProgress
                          variant="determinate" value={Math.min(pct, 100)}
                          sx={{ flex: 1, height: 8, borderRadius: 4,
                            '& .MuiLinearProgress-bar': { background: pct > 80 ? '#ff3366' : '#00f0ff' } }}
                        />
                        <Typography variant="caption">{l.currentOccupancy}/{l.capacity}</Typography>
                      </Box>
                    </TableCell>
                    <TableCell align="center">
                      <IconButton size="small" onClick={() => openEdit(l)} sx={{ color: '#00f0ff' }}><Edit fontSize="small" /></IconButton>
                      <IconButton size="small" onClick={() => handleDelete(l.id)} sx={{ color: '#ff3366' }}><Delete fontSize="small" /></IconButton>
                    </TableCell>
                  </TableRow>
                );
              })}
            </TableBody>
          </Table>
        </TableContainer>
      )}
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editId ? 'Edit Location' : 'Add Location'}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: '8px !important' }}>
          <TextField label="Name" value={form.name} onChange={(e) => setForm({ ...form, name: e.target.value })} />
          <TextField label="Address" value={form.address} onChange={(e) => setForm({ ...form, address: e.target.value })} />
          <FormControl>
            <InputLabel>Type</InputLabel>
            <Select value={form.type} label="Type" onChange={(e) => setForm({ ...form, type: e.target.value })}>
              {TYPES.map((t) => <MenuItem key={t} value={t}>{t}</MenuItem>)}
            </Select>
          </FormControl>
          <TextField label="Capacity" type="number" value={form.capacity} onChange={(e) => setForm({ ...form, capacity: parseInt(e.target.value) || 0 })} />
          {editId && <TextField label="Current Occupancy" type="number" value={form.currentOccupancy} onChange={(e) => setForm({ ...form, currentOccupancy: parseInt(e.target.value) || 0 })} />}
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleSave} variant="contained">Save</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default LocationsPage;
