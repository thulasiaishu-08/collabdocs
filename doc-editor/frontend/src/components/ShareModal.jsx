import { useState, useEffect } from 'react';
import { useAuth } from '../context/AuthContext';
import { api } from '../services/api';

export default function ShareModal({ docId, onClose }) {
  const { user }     = useAuth();
  const [users,    setUsers]    = useState([]);
  const [shares,   setShares]   = useState([]);
  const [loading,  setLoading]  = useState(true);
  const [sharing,  setSharing]  = useState(null);
  const [removing, setRemoving] = useState(null);
  const [error,    setError]    = useState('');

  useEffect(() => {
    Promise.all([api.getUsers(), api.getDocumentShares(docId)])
      .then(([u, s]) => { setUsers(u); setShares(s); setLoading(false); })
      .catch(err => { setError(err.message); setLoading(false); });
  }, [docId]);

  const sharedIds = new Set(shares.map(s => s.sharedWithUserId));
  const eligible  = users.filter(u => u.id !== user.id);

  async function handleShare(targetUserId) {
    setSharing(targetUserId);
    setError('');
    try {
      const share = await api.shareDocument(docId, targetUserId);
      setShares(prev => [...prev, share]);
    } catch (err) {
      setError(err.message);
    } finally {
      setSharing(null);
    }
  }

  async function handleRemove(shareId) {
    setRemoving(shareId);
    setError('');
    try {
      await api.removeShare(shareId);
      setShares(prev => prev.filter(s => s.id !== shareId));
    } catch (err) {
      setError(err.message);
    } finally {
      setRemoving(null);
    }
  }

  return (
    <div className="modal-backdrop" onClick={onClose}>
      <div className="modal" onClick={e => e.stopPropagation()}>
        <div className="modal-header">
          <h3>Share Document</h3>
          <button className="modal-close" onClick={onClose}>✕</button>
        </div>

        {loading ? (
          <p className="modal-loading">Loading…</p>
        ) : (
          <>
            {error && <p className="form-error">{error}</p>}
            <div className="share-section">
              <p className="share-section-label">People with access</p>
              {eligible.length === 0 ? (
                <p className="nav-empty">No other users available.</p>
              ) : (
                <ul className="share-list">
                  {eligible.map(u => {
                    const existingShare = shares.find(s => s.sharedWithUserId === u.id);
                    return (
                      <li key={u.id} className="share-row">
                        <span className="share-avatar">{u.username[0].toUpperCase()}</span>
                        <span className="share-name">{u.username}</span>
                        <span className="share-email">{u.email}</span>
                        {existingShare ? (
                          <button
                            className="btn-unshare"
                            disabled={removing === existingShare.id}
                            onClick={() => handleRemove(existingShare.id)}
                          >
                            {removing === existingShare.id ? '…' : 'Remove'}
                          </button>
                        ) : (
                          <button
                            className="btn-share-user"
                            disabled={sharing === u.id}
                            onClick={() => handleShare(u.id)}
                          >
                            {sharing === u.id ? '…' : 'Share'}
                          </button>
                        )}
                      </li>
                    );
                  })}
                </ul>
              )}
            </div>
          </>
        )}

        <div className="modal-footer">
          <button className="btn-primary" onClick={onClose}>Done</button>
        </div>
      </div>
    </div>
  );
}
