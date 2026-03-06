import React, { useEffect, useState } from 'react';
import {
  Box, Typography, Table, TableHead, TableRow, TableCell, TableBody,
  Paper, Button, TextField, IconButton, Dialog, DialogTitle,
  DialogContent, DialogActions, Chip, CircularProgress, Alert, TableContainer,
  FormControl, InputLabel, Select, MenuItem,
} from '@mui/material';
import { Add, Edit, Delete } from '@mui/icons-material';
import { useAuth } from '../../contexts/AuthContext';
import { shipmentApi } from '../../services/api';

const SHIP_STATUSES = ['Pending', 'InTransit', 'Delivered', 'Cancelled'];
const statusColor = { Pending: 'warning', InTransit: 'info', Delivered: 'success', Cancelled: 'error' };

/**
 * Shipments page. Admins can create/edit/delete shipments and update status.
 * Vendors see read-only view of their shipments.
 */
function ShipmentsPage() {
  const { isAdmin } = useAuth();
  const [shipments, setShipments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [dialogOpen, setDialogOpen] = useState(false);
  const [editId, setEditId] = useState(null);
  const [form, setForm] = useState({ status: 'Pending', trackingNumber: '', destination: '', notes: '' });

  const loadShipments = async () => {
    try { setLoading(true); const { data } = await shipmentApi.getAll(); setShipments(data); }
    catch { setError('Failed to load shipments'); }
    finally { setLoading(false); }
  };

  useEffect(() => { loadShipments(); }, []);

  const handleSave = async () => {
    try {
      if (editId) await shipmentApi.update(editId, form);
      else await shipmentApi.create(form);
      setDialogOpen(false); setEditId(null); loadShipments();
    } catch { setError('Failed to save shipment'); }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Delete this shipment?')) return;
    try { await shipmentApi.delete(id); loadShipments(); }
    catch { setError('Failed to delete shipment'); }
  };

  const openEdit = (s) => {
    setEditId(s.id);
    setForm({ status: s.status, trackingNumber: s.trackingNumber, destination: s.destination, notes: s.notes });
    setDialogOpen(true);
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h4" sx={{ color: '#00f0ff' }}>Shipments</Typography>
        {isAdmin && (
          <Button variant="contained" startIcon={<Add />} onClick={() => {
            setEditId(null); setForm({ origin: '', destination: '', trackingNumber: '', notes: '', inventoryItemIds: [] }); setDialogOpen(true);
          }}>Add Shipment</Button>
        )}
      </Box>
      {error && <Alert severity="error" sx={{ mb: 2 }} onClose={() => setError('')}>{error}</Alert>}
      {loading ? <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}><CircularProgress /></Box> : (
        <TableContainer component={Paper}>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Tracking #</TableCell>
                <TableCell>Origin</TableCell>
                <TableCell>Destination</TableCell>
                <TableCell>Status</TableCell>
                <TableCell>Items</TableCell>
                <TableCell>Shipped</TableCell>
                {isAdmin && <TableCell align="center">Actions</TableCell>}
              </TableRow>
            </TableHead>
            <TableBody>
              {shipments.map((s) => (
                <TableRow key={s.id} hover>
                  <TableCell sx={{ fontFamily: 'monospace', color: '#ff00ff' }}>{s.trackingNumber || 'N/A'}</TableCell>
                  <TableCell>{s.origin}</TableCell>
                  <TableCell>{s.destination}</TableCell>
                  <TableCell><Chip label={s.status} color={statusColor[s.status] || 'default'} size="small" /></TableCell>
                  <TableCell>{s.inventoryItemIds?.length || 0}</TableCell>
                  <TableCell>{s.shippedDate ? new Date(s.shippedDate).toLocaleDateString() : '-'}</TableCell>
                  {isAdmin && (
                    <TableCell align="center">
                      <IconButton size="small" onClick={() => openEdit(s)} sx={{ color: '#00f0ff' }}><Edit fontSize="small" /></IconButton>
                      <IconButton size="small" onClick={() => handleDelete(s.id)} sx={{ color: '#ff3366' }}><Delete fontSize="small" /></IconButton>
                    </TableCell>
                  )}
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      )}
      <Dialog open={dialogOpen} onClose={() => setDialogOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>{editId ? 'Update Shipment' : 'Create Shipment'}</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: '8px !important' }}>
          {!editId && (
            <>
              <TextField label="Origin" value={form.origin || ''} onChange={(e) => setForm({ ...form, origin: e.target.value })} />
              <TextField label="Destination" value={form.destination || ''} onChange={(e) => setForm({ ...form, destination: e.target.value })} />
              <TextField label="Tracking Number" value={form.trackingNumber} onChange={(e) => setForm({ ...form, trackingNumber: e.target.value })} />
            </>
          )}
          {editId && (
            <>
              <FormControl>
                <InputLabel>Status</InputLabel>
                <Select value={form.status} label="Status" onChange={(e) => setForm({ ...form, status: e.target.value })}>
                  {SHIP_STATUSES.map((s) => <MenuItem key={s} value={s}>{s}</MenuItem>)}
                </Select>
              </FormControl>
              <TextField label="Tracking Number" value={form.trackingNumber} onChange={(e) => setForm({ ...form, trackingNumber: e.target.value })} />
              <TextField label="Destination" value={form.destination} onChange={(e) => setForm({ ...form, destination: e.target.value })} />
            </>
          )}
          <TextField label="Notes" value={form.notes} onChange={(e) => setForm({ ...form, notes: e.target.value })} multiline rows={2} />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDialogOpen(false)}>Cancel</Button>
          <Button onClick={handleSave} variant="contained">Save</Button>
        </DialogActions>
      </Dialog>
    </Box>
  );
}

export default ShipmentsPage;
